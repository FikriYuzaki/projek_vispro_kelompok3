Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports System.Reflection
Imports CuppaClub.Core
Imports CuppaClub.Models
Imports CuppaClub.Services

Namespace Features
    Public Class FriendsOrderControl
        Inherits UserControl

        Private Enum RoomViewState
            Unconfigured
            Unauthenticated
            Loading
            ErrorState
            Ready
        End Enum

        Private Enum DrawerMode
            None
            CreateRoom
            JoinRoom
        End Enum

        Private Enum BannerTone
            Success
            Warning
            [Error]
            Info
        End Enum

        Private NotInheritable Class QuickMenuOptionItem
            Public ReadOnly Property MenuItem As Models.MenuItem

            Public Sub New(item As Models.MenuItem)
                Me.MenuItem = item
            End Sub

            Public Overrides Function ToString() As String
                If MenuItem Is Nothing Then Return String.Empty
                Return $"{MenuItem.Name} — {AppTheme.FormatRupiah(MenuItem.BasePrice)}"
            End Function
        End Class

        Private ReadOnly _service As FriendsOrderService
        Private ReadOnly _toolTip As New ToolTip()
        Private ReadOnly _stateOverlay As New Panel()
        Private ReadOnly _stateCard As New Panel()
        Private ReadOnly _stateBadge As New Label()
        Private ReadOnly _stateTitle As New Label()
        Private ReadOnly _stateMessage As New Label()
        Private ReadOnly _stateProgress As New ProgressBar()
        Private ReadOnly _stateActionBtn As New Button()

        Private _myRooms As New List(Of OrderRoom)()
        Private _availableMenus As New List(Of Models.MenuItem)()
        Private _selectedRoomId As String = String.Empty
        Private _currentOverview As RoomOverviewSnapshot
        Private _lastCreatedCode As String = String.Empty
        Private _lastObservedUserId As String = String.Empty
        Private _drawerMode As DrawerMode = DrawerMode.None
        Private _currentOverlayState As RoomViewState = RoomViewState.Loading
        Private _loadVersion As Integer
        Private _overviewLoadVersion As Integer
        Private _isMutating As Boolean
        Private _runtimeInitialized As Boolean
        Private _cleanupDone As Boolean

        Private _realtimeSubscription As FriendsRoomRealtimeSubscription
        Private _realtimeSubscribedRoomId As String = String.Empty
        Private _realtimeSubscribedOrderId As String = String.Empty
        Private _realtimeDebounceTimer As Timer

        Public Sub New()
            Me.New(Nothing)
        End Sub

        Public Sub New(service As FriendsOrderService)
            InitializeComponent()

            If LicenseManager.UsageMode = LicenseUsageMode.Designtime OrElse DesignMode Then
                Return
            End If

            _service = If(service, New FriendsOrderService())
            _lastObservedUserId = GetCurrentUserId()
            _runtimeInitialized = True

            WireRuntimeBehavior()

            AddHandler AppSession.SessionChanged, AddressOf OnSessionChanged
            AddHandler SupabaseClientProvider.ConfigurationChanged, AddressOf OnConfigurationChanged
            AddHandler DrinkDraftHandoff.PendingDraftsChanged, AddressOf OnPendingDraftsChanged
            AddHandler Me.Load, AddressOf OnControlLoad
        End Sub

        Private Sub OnControlLoad(sender As Object, e As EventArgs)
            If IsDisposed OrElse LicenseManager.UsageMode = LicenseUsageMode.Designtime OrElse DesignMode Then
                Return
            End If

            LayoutToolbar()
            LayoutWorkspace()
            UpdatePendingDraftsUi()
            ReloadAllAsync()
        End Sub

        Private Sub WireRuntimeBehavior()
            EnableDoubleBuffer(_roomsListPanel)
            EnableDoubleBuffer(_overviewScrollPanel)
            EnableDoubleBuffer(_membersItemsPanel)

            _realtimeDebounceTimer = New Timer() With {
                .Interval = 350
            }
            AddHandler _realtimeDebounceTimer.Tick, AddressOf OnRealtimeDebounceTick

            AppTheme.StylePrimaryButton(_createRoomToggleBtn)
            AppTheme.StyleSecondaryButton(_joinRoomToggleBtn)
            AppTheme.StyleSecondaryButton(_refreshBtn)
            AppTheme.StylePrimaryButton(_submitCreateBtn)
            AppTheme.StyleSecondaryButton(_cancelCreateBtn)
            AppTheme.StyleSecondaryButton(_copyCreatedCodeBtn)
            AppTheme.StylePrimaryButton(_submitJoinBtn)
            AppTheme.StyleSecondaryButton(_cancelJoinBtn)
            AppTheme.StyleSecondaryButton(_bannerDismissBtn)
            AppTheme.StyleSecondaryButton(_copyCodeBtn)
            AppTheme.StyleDangerButton(_lockRoomBtn)
            AppTheme.StyleAccentButton(_importDraftsBtn)
            AppTheme.StylePrimaryButton(_quickAddBtn)

            _createRoomToggleBtn.Width = AppTheme.MeasureButtonWidth(_createRoomToggleBtn.Text, AppTheme.FontBodyBold, 124, 24)
            _joinRoomToggleBtn.Width = AppTheme.MeasureButtonWidth(_joinRoomToggleBtn.Text, AppTheme.FontBodyBold, 134, 24)
            _refreshBtn.Width = AppTheme.MeasureButtonWidth(_refreshBtn.Text, AppTheme.FontBodyBold, 110, 24)
            _lockRoomBtn.Width = AppTheme.MeasureButtonWidth(_lockRoomBtn.Text, AppTheme.FontBodyBold, 124, 24)
            _importDraftsBtn.Width = AppTheme.MeasureButtonWidth(_importDraftsBtn.Text, AppTheme.FontBodyBold, 210, 24)
            _quickAddBtn.Width = AppTheme.MeasureButtonWidth(_quickAddBtn.Text, AppTheme.FontBodyBold, 114, 20)

            AddHandler _toolbarPanel.Paint, Sub(s, e)
                                                Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                                    e.Graphics.DrawLine(pen, 0, _toolbarPanel.Height - 1, _toolbarPanel.Width, _toolbarPanel.Height - 1)
                                                End Using
                                            End Sub
            AddHandler _actionDrawerPanel.Paint, Sub(s, e)
                                                     Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                                         e.Graphics.DrawLine(pen, 0, _actionDrawerPanel.Height - 1, _actionDrawerPanel.Width, _actionDrawerPanel.Height - 1)
                                                     End Using
                                                 End Sub
            AddHandler _roomsPane.Paint, Sub(s, e)
                                             Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                                 e.Graphics.DrawLine(pen, _roomsPane.Width - 1, 0, _roomsPane.Width - 1, _roomsPane.Height)
                                             End Using
                                         End Sub
            AddHandler _roomsHeaderPanel.Paint, Sub(s, e)
                                                    Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                                        e.Graphics.DrawLine(pen, 0, _roomsHeaderPanel.Height - 1, _roomsHeaderPanel.Width - 1, _roomsHeaderPanel.Height - 1)
                                                    End Using
                                                End Sub
            AddHandler _groupTotalBar.Paint, Sub(s, e)
                                                 Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                                     e.Graphics.DrawLine(pen, 0, 0, _groupTotalBar.Width, 0)
                                                 End Using
                                             End Sub
            AddHandler _roomHeaderCard.Paint, AddressOf PaintCardBorder
            AddHandler _addItemsCard.Paint, AddressOf PaintCardBorder

            AddHandler _toolbarPanel.Resize, Sub() LayoutToolbar()
            AddHandler _workspaceHost.Resize, Sub() LayoutWorkspace()
            AddHandler _overviewScrollPanel.Resize, Sub() LayoutOverviewCards()
            AddHandler _groupTotalBar.Resize, Sub() LayoutGroupTotalBar()

            AddHandler _createRoomToggleBtn.Click, Sub() ToggleDrawer(DrawerMode.CreateRoom)
            AddHandler _joinRoomToggleBtn.Click, Sub() ToggleDrawer(DrawerMode.JoinRoom)
            AddHandler _refreshBtn.Click, Sub() ReloadAllAsync()
            AddHandler _cancelCreateBtn.Click, Sub() SetDrawerMode(DrawerMode.None)
            AddHandler _cancelJoinBtn.Click, Sub() SetDrawerMode(DrawerMode.None)
            AddHandler _bannerDismissBtn.Click, Sub() HideBanner()

            AddHandler _submitCreateBtn.Click, AddressOf OnCreateRoomSubmitClick
            AddHandler _copyCreatedCodeBtn.Click, Sub() CopyCodeToClipboard(_lastCreatedCode, "Kode undangan room baru disalin ke clipboard.")
            AddHandler _submitJoinBtn.Click, AddressOf OnJoinRoomSubmitClick
            AddHandler _copyCodeBtn.Click, Sub()
                                               If _currentOverview IsNot Nothing AndAlso _currentOverview.Room IsNot Nothing Then
                                                   CopyCodeToClipboard(_currentOverview.Room.RoomCode, "Kode undangan room disalin ke clipboard.")
                                               End If
                                           End Sub
            AddHandler _lockRoomBtn.Click, AddressOf OnLockRoomClick
            AddHandler _importDraftsBtn.Click, AddressOf OnImportDrinkDraftsClick
            AddHandler _quickAddBtn.Click, AddressOf OnQuickAddMenuItemClick

            AddHandler _createTitleTextBox.KeyDown, Sub(s, e)
                                                        If e.KeyCode = Keys.Enter Then
                                                            e.SuppressKeyPress = True
                                                            OnCreateRoomSubmitClick(s, EventArgs.Empty)
                                                        End If
                                                    End Sub
            AddHandler _joinCodeTextBox.KeyDown, Sub(s, e)
                                                     If e.KeyCode = Keys.Enter Then
                                                         e.SuppressKeyPress = True
                                                         OnJoinRoomSubmitClick(s, EventArgs.Empty)
                                                     End If
                                                 End Sub

            BuildStateOverlay()
        End Sub

        Private Sub BuildStateOverlay()
            _stateOverlay.BackColor = AppTheme.CanvasCream
            _stateOverlay.Dock = DockStyle.Fill
            _stateOverlay.Visible = False

            _stateCard.BackColor = AppTheme.SurfaceMilk
            _stateCard.Size = New Size(460, 236)
            AddHandler _stateCard.Paint, AddressOf PaintCardBorder

            _stateBadge.Location = New Point(24, 24)
            AppTheme.ApplyStatusTag(_stateBadge, "Memuat", AppTheme.TaupeTint, AppTheme.TaupeText)

            _stateTitle.Font = AppTheme.FontSectionTitle
            _stateTitle.ForeColor = AppTheme.TextPrimary
            _stateTitle.Location = New Point(24, 58)
            _stateTitle.Size = New Size(412, 26)

            _stateMessage.Font = AppTheme.FontBody
            _stateMessage.ForeColor = AppTheme.TextSecondary
            _stateMessage.Location = New Point(24, 90)
            _stateMessage.Size = New Size(412, 68)

            _stateProgress.Style = ProgressBarStyle.Marquee
            _stateProgress.MarqueeAnimationSpeed = 28
            _stateProgress.Location = New Point(24, 166)
            _stateProgress.Size = New Size(220, 14)
            _stateProgress.Visible = False

            _stateActionBtn.Location = New Point(24, 174)
            _stateActionBtn.Size = New Size(148, 34)
            AppTheme.StylePrimaryButton(_stateActionBtn)
            AddHandler _stateActionBtn.Click, Sub() ReloadAllAsync()

            _stateCard.Controls.AddRange(New Control() {
                _stateBadge, _stateTitle, _stateMessage, _stateProgress, _stateActionBtn
            })
            _stateOverlay.Controls.Add(_stateCard)
            AddHandler _stateOverlay.Resize, Sub() CenterStateCard()

            _workspaceHost.Controls.Add(_stateOverlay)
            _stateOverlay.BringToFront()
        End Sub

        Private Sub CenterStateCard()
            Dim x = Math.Max(16, (_stateOverlay.ClientSize.Width - _stateCard.Width) \ 2)
            Dim y = Math.Max(16, (_stateOverlay.ClientSize.Height - _stateCard.Height) \ 2)
            _stateCard.Location = New Point(x, y)
        End Sub

        Private Sub LayoutToolbar()
            _joinRoomToggleBtn.Left = _createRoomToggleBtn.Right + 10
            _refreshBtn.Left = _joinRoomToggleBtn.Right + 10
            _realtimeStatusTag.Left = Math.Max(_refreshBtn.Right + 12, _toolbarPanel.ClientSize.Width - _realtimeStatusTag.Width - 20)
        End Sub

        Private Sub LayoutWorkspace()
            Dim totalWidth = _workspaceHost.ClientSize.Width
            If totalWidth <= 0 Then Return

            _roomsPane.Width = If(totalWidth < 820, 256, 296)
            _roomsCountLabel.Left = Math.Max(_roomsHeadingLabel.Right + 8, _roomsHeaderPanel.ClientSize.Width - _roomsCountLabel.Width - 16)
            LayoutOverviewCards()
            LayoutGroupTotalBar()
            CenterStateCard()
        End Sub

        Private Sub LayoutOverviewCards()
            Dim availWidth = Math.Max(380, _overviewScrollPanel.ClientSize.Width - 48)
            _roomHeaderCard.Width = availWidth
            _addItemsCard.Width = availWidth
            _membersItemsPanel.Width = availWidth

            _lockRoomBtn.Left = Math.Max(220, availWidth - _lockRoomBtn.Width - 16)
            _overviewRoleTag.Left = Math.Max(200, availWidth - _overviewRoleTag.Width - 16)
            _overviewStatusTag.Left = Math.Max(120, _overviewRoleTag.Left - _overviewStatusTag.Width - 8)
            _overviewTitleLabel.Width = Math.Max(160, _overviewStatusTag.Left - 28)
            _lockedNoticeLabel.Width = availWidth - 32

            _importDraftsBtn.Left = Math.Max(16, availWidth - _importDraftsBtn.Width - 16)
            _handoffInfoLabel.Width = Math.Max(160, _importDraftsBtn.Left - 24)

            _quickAddBtn.Left = Math.Max(280, availWidth - _quickAddBtn.Width - 16)
            Dim comboWidth = Math.Max(150, Math.Min(230, availWidth \ 3))
            _quickMenuCombo.Width = comboWidth
            _quickQtyLabel.Left = _quickMenuCombo.Right + 10
            _quickQtyUpDown.Left = _quickQtyLabel.Left
            _quickNotesTextBox.Left = _quickQtyUpDown.Right + 10
            _quickNotesTextBox.Width = Math.Max(80, _quickAddBtn.Left - _quickNotesTextBox.Left - 10)

            For Each ctrl As Control In _membersItemsPanel.Controls
                ctrl.Width = availWidth
            Next
        End Sub

        Private Sub LayoutGroupTotalBar()
            Dim rightEdge = _groupTotalBar.ClientSize.Width - 20
            _groupTotalValueLabel.Left = Math.Max(240, rightEdge - _groupTotalValueLabel.Width)
            _groupTotalCaptionLabel.Left = Math.Max(160, _groupTotalValueLabel.Left - _groupTotalCaptionLabel.Width - 8)
        End Sub

        Private Sub PaintCardBorder(sender As Object, e As PaintEventArgs)
            Dim panel = TryCast(sender, Control)
            If panel Is Nothing Then Return
            Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1)
            End Using
        End Sub

        Private Sub ToggleDrawer(targetMode As DrawerMode)
            If _drawerMode = targetMode Then
                SetDrawerMode(DrawerMode.None)
            Else
                SetDrawerMode(targetMode)
            End If
        End Sub

        Private Sub SetDrawerMode(mode As DrawerMode)
            _drawerMode = mode
            _createPanel.Visible = (mode = DrawerMode.CreateRoom)
            _joinPanel.Visible = (mode = DrawerMode.JoinRoom)
            _actionDrawerPanel.Visible = (mode <> DrawerMode.None)

            If mode = DrawerMode.CreateRoom Then
                _copyCreatedCodeBtn.Visible = Not String.IsNullOrEmpty(_lastCreatedCode)
                _createTitleTextBox.Focus()
            ElseIf mode = DrawerMode.JoinRoom Then
                _joinCodeTextBox.Focus()
            End If
        End Sub

        Private Sub ShowBanner(message As String, tone As BannerTone)
            _bannerMessageLabel.Text = message
            Select Case tone
                Case BannerTone.Success
                    _bannerPanel.BackColor = AppTheme.SageTint
                    _bannerMessageLabel.ForeColor = AppTheme.SageText
                Case BannerTone.Warning
                    _bannerPanel.BackColor = AppTheme.WarningTint
                    _bannerMessageLabel.ForeColor = AppTheme.WarningText
                Case BannerTone.[Error]
                    _bannerPanel.BackColor = AppTheme.ErrorTint
                    _bannerMessageLabel.ForeColor = AppTheme.ErrorText
                Case Else
                    _bannerPanel.BackColor = AppTheme.TaupeTint
                    _bannerMessageLabel.ForeColor = AppTheme.TaupeText
            End Select
            _bannerPanel.Visible = True
        End Sub

        Private Sub HideBanner()
            _bannerPanel.Visible = False
            _bannerMessageLabel.Text = String.Empty
        End Sub

        Private Sub ShowOverlay(state As RoomViewState, badgeText As String, title As String, message As String, showAction As Boolean, Optional actionLabel As String = "Coba Lagi")
            _currentOverlayState = state
            _stateTitle.Text = title
            _stateMessage.Text = message
            _stateProgress.Visible = (state = RoomViewState.Loading)
            _stateActionBtn.Visible = showAction
            _stateActionBtn.Text = actionLabel

            Select Case state
                Case RoomViewState.Unconfigured
                    AppTheme.ApplyStatusTag(_stateBadge, badgeText, AppTheme.WarningTint, AppTheme.WarningText)
                Case RoomViewState.Unauthenticated
                    AppTheme.ApplyStatusTag(_stateBadge, badgeText, AppTheme.AccentStrawberryTint, AppTheme.AccentStrawberryText)
                Case RoomViewState.ErrorState
                    AppTheme.ApplyStatusTag(_stateBadge, badgeText, AppTheme.ErrorTint, AppTheme.ErrorText)
                Case Else
                    AppTheme.ApplyStatusTag(_stateBadge, badgeText, AppTheme.TaupeTint, AppTheme.TaupeText)
            End Select

            _stateOverlay.Visible = True
            _stateOverlay.BringToFront()
            CenterStateCard()
            UpdateToolbarEnabledState()
        End Sub

        Private Sub HideOverlay()
            _currentOverlayState = RoomViewState.Ready
            _stateOverlay.Visible = False
            UpdateToolbarEnabledState()
        End Sub

        Private Sub UpdateToolbarEnabledState()
            Dim canUseActions = (_currentOverlayState = RoomViewState.Ready) AndAlso AppSession.IsAuthenticated AndAlso Not _isMutating
            _createRoomToggleBtn.Enabled = canUseActions
            _joinRoomToggleBtn.Enabled = canUseActions
            _refreshBtn.Enabled = _service.IsConfigured AndAlso AppSession.IsAuthenticated AndAlso Not _isMutating
        End Sub

        Private Sub SetRealtimeStatus(state As FriendsRoomRealtimeState, detail As String)
            If IsDisposed Then Return
            If InvokeRequired Then
                Try
                    BeginInvoke(Sub() SetRealtimeStatus(state, detail))
                Catch
                End Try
                Return
            End If

            Select Case state
                Case FriendsRoomRealtimeState.Connected
                    AppTheme.ApplyStatusTag(_realtimeStatusTag, "Realtime: Aktif", AppTheme.SageTint, AppTheme.SageText)
                Case FriendsRoomRealtimeState.Connecting, FriendsRoomRealtimeState.Reconnecting
                    AppTheme.ApplyStatusTag(_realtimeStatusTag, "Realtime: Menyambung...", AppTheme.WarningTint, AppTheme.WarningText)
                Case FriendsRoomRealtimeState.[Error]
                    AppTheme.ApplyStatusTag(_realtimeStatusTag, "Realtime: Gangguan", AppTheme.ErrorTint, AppTheme.ErrorText)
                Case Else
                    AppTheme.ApplyStatusTag(_realtimeStatusTag, "Realtime: Manual", AppTheme.TaupeTint, AppTheme.TaupeText)
            End Select

            _toolTip.SetToolTip(_realtimeStatusTag, If(String.IsNullOrWhiteSpace(detail), "Gunakan tombol Muat Ulang untuk sinkronisasi manual.", detail))
            LayoutToolbar()
        End Sub

        Private Async Sub ReloadAllAsync()
            If IsDisposed OrElse Not _runtimeInitialized Then Return

            Dim version = Threading.Interlocked.Increment(_loadVersion)
            HideBanner()

            If Not _service.IsConfigured Then
                ClearRoomState()
                ShowOverlay(
                    RoomViewState.Unconfigured,
                    "Belum Dikonfigurasi",
                    "Koneksi Supabase belum diatur",
                    "Buka tombol Koneksi Supabase pada sidebar atau header untuk memasukkan URL proyek dan publishable/anon key sebelum menggunakan Friends Order Room.",
                    True,
                    "Periksa Lagi")
                Return
            End If

            If Not AppSession.IsAuthenticated Then
                ClearRoomState()
                ShowOverlay(
                    RoomViewState.Unauthenticated,
                    "Perlu Masuk Akun",
                    "Masuk untuk menggunakan Friends Order Room",
                    "Membuat room, bergabung melalui kode undangan, menambahkan item, dan mengunci room memerlukan akun pelanggan atau staf yang aktif.",
                    False)
                Return
            End If

            ShowOverlay(
                RoomViewState.Loading,
                "Memuat",
                "Memuat daftar Friends Order Room...",
                "Mengambil room yang Anda ikuti dan katalog menu tersedia dari Supabase.",
                False)

            Try
                Dim roomsTask = _service.GetMyRoomsAsync()
                Dim menusTask = _service.GetAvailableMenuItemsAsync()
                Await Task.WhenAll(roomsTask, menusTask)

                If IsDisposed OrElse version <> _loadVersion Then Return

                _myRooms = roomsTask.Result
                _availableMenus = menusTask.Result
                BindQuickMenuCombo()
                HideOverlay()
                RenderMyRoomsList()

                If _myRooms.Count = 0 Then
                    _selectedRoomId = String.Empty
                    _currentOverview = Nothing
                    DisposeRealtimeSubscription()
                    SetRealtimeStatus(FriendsRoomRealtimeState.Disconnected, "Belum ada room aktif.")
                    RenderEmptyOverview()
                Else
                    Dim keepCurrent = Not String.IsNullOrWhiteSpace(_selectedRoomId) AndAlso
                                      _myRooms.Any(Function(r) String.Equals(r.Id, _selectedRoomId, StringComparison.OrdinalIgnoreCase))
                    If Not keepCurrent Then
                        _selectedRoomId = _myRooms(0).Id
                    End If
                    RenderMyRoomsList()
                    Await LoadSelectedRoomOverviewAsync(_selectedRoomId)
                End If
            Catch ex As FriendsOrderException
                If IsDisposed OrElse version <> _loadVersion Then Return
                HandleLoadException(ex)
            Catch ex As Exception
                If IsDisposed OrElse version <> _loadVersion Then Return
                HandleLoadException(FriendsOrderService.MapException(ex, "Gagal memuat Friends Order Room."))
            End Try
        End Sub

        Private Sub HandleLoadException(ex As FriendsOrderException)
            Select Case ex.ErrorKind
                Case FriendsOrderErrorKind.Unconfigured
                    ShowOverlay(RoomViewState.Unconfigured, "Belum Dikonfigurasi", "Koneksi Supabase belum diatur", ex.Message, True, "Periksa Lagi")
                Case FriendsOrderErrorKind.Unauthenticated
                    ShowOverlay(RoomViewState.Unauthenticated, "Perlu Masuk Akun", "Sesi tidak aktif", ex.Message, False)
                Case FriendsOrderErrorKind.Unauthorized, FriendsOrderErrorKind.RlsBlocked
                    ShowOverlay(RoomViewState.ErrorState, "Akses Ditolak", "Tidak memiliki izin akses", ex.Message, True, "Coba Lagi")
                Case Else
                    ShowOverlay(RoomViewState.ErrorState, "Gagal Memuat", "Tidak dapat memuat data room", ex.Message, True, "Coba Lagi")
            End Select
        End Sub

        Private Sub BindQuickMenuCombo()
            Dim selectedId As String = String.Empty
            Dim currentChoice = TryCast(_quickMenuCombo.SelectedItem, QuickMenuOptionItem)
            If currentChoice IsNot Nothing AndAlso currentChoice.MenuItem IsNot Nothing Then
                selectedId = currentChoice.MenuItem.Id
            End If

            _quickMenuCombo.BeginUpdate()
            Try
                _quickMenuCombo.Items.Clear()
                For Each item In _availableMenus
                    _quickMenuCombo.Items.Add(New QuickMenuOptionItem(item))
                Next

                If _quickMenuCombo.Items.Count > 0 Then
                    Dim matchedIndex = -1
                    If Not String.IsNullOrWhiteSpace(selectedId) Then
                        For i = 0 To _quickMenuCombo.Items.Count - 1
                            Dim opt = TryCast(_quickMenuCombo.Items(i), QuickMenuOptionItem)
                            If opt IsNot Nothing AndAlso String.Equals(opt.MenuItem.Id, selectedId, StringComparison.OrdinalIgnoreCase) Then
                                matchedIndex = i
                                Exit For
                            End If
                        Next
                    End If
                    _quickMenuCombo.SelectedIndex = If(matchedIndex >= 0, matchedIndex, 0)
                End If
            Finally
                _quickMenuCombo.EndUpdate()
            End Try
        End Sub

        Private Sub RenderMyRoomsList()
            _roomsCountLabel.Text = $"{_myRooms.Count} room"
            _roomsCountLabel.Left = Math.Max(_roomsHeadingLabel.Right + 8, _roomsHeaderPanel.ClientSize.Width - _roomsCountLabel.Width - 16)

            _roomsListPanel.SuspendLayout()
            Try
                Dim existingControls = _roomsListPanel.Controls.Cast(Of Control)().ToList()
                _roomsListPanel.Controls.Clear()
                For Each existing In existingControls
                    existing.Dispose()
                Next

                Dim cardWidth = Math.Max(220, _roomsListPanel.ClientSize.Width - 28)

                If _myRooms.Count = 0 Then
                    Dim emptyCard As New Panel With {
                        .Width = cardWidth,
                        .Height = 166,
                        .BackColor = AppTheme.CanvasCream,
                        .Margin = New Padding(0, 4, 0, 8)
                    }
                    AddHandler emptyCard.Paint, AddressOf PaintCardBorder

                    Dim emptyTitle As New Label With {
                        .Text = "Belum ada room",
                        .Font = AppTheme.FontCardTitle,
                        .ForeColor = AppTheme.TextPrimary,
                        .Location = New Point(14, 14),
                        .AutoSize = True
                    }
                    Dim emptyDesc As New Label With {
                        .Text = "Buat room baru untuk mengundang teman, atau masukkan kode undangan yang dibagikan host.",
                        .Font = AppTheme.FontCaption,
                        .ForeColor = AppTheme.TextSecondary,
                        .Location = New Point(14, 40),
                        .Size = New Size(cardWidth - 28, 48)
                    }
                    Dim quickCreateBtn As New Button With {
                        .Text = "Buat Room",
                        .Location = New Point(14, 98),
                        .Size = New Size(102, 30)
                    }
                    AppTheme.StylePrimaryButton(quickCreateBtn)
                    AddHandler quickCreateBtn.Click, Sub() SetDrawerMode(DrawerMode.CreateRoom)

                    Dim quickJoinBtn As New Button With {
                        .Text = "Gabung Kode",
                        .Location = New Point(124, 98),
                        .Size = New Size(106, 30)
                    }
                    AppTheme.StyleSecondaryButton(quickJoinBtn)
                    AddHandler quickJoinBtn.Click, Sub() SetDrawerMode(DrawerMode.JoinRoom)

                    emptyCard.Controls.AddRange(New Control() {emptyTitle, emptyDesc, quickCreateBtn, quickJoinBtn})
                    _roomsListPanel.Controls.Add(emptyCard)
                    Return
                End If

                Dim currentUserId = GetCurrentUserId()
                For Each room In _myRooms
                    Dim capturedRoom = room
                    Dim isSelected = String.Equals(room.Id, _selectedRoomId, StringComparison.OrdinalIgnoreCase)
                    Dim isHost = room.IsHost(currentUserId)

                    Dim card As New Panel With {
                        .Width = cardWidth,
                        .Height = 90,
                        .BackColor = If(isSelected, AppTheme.SidebarOat, AppTheme.SurfaceMilk),
                        .Cursor = Cursors.Hand,
                        .Margin = New Padding(0, 0, 0, 8),
                        .TabStop = True
                    }

                    AddHandler card.Paint, Sub(s, e)
                                               Dim borderColor = If(isSelected OrElse card.Focused, AppTheme.FocusMocha, AppTheme.DividerAlmond)
                                               Dim penWidth = If(isSelected OrElse card.Focused, 2.0F, 1.0F)
                                               Using pen As New Pen(borderColor, penWidth)
                                                   e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1)
                                               End Using
                                           End Sub

                    Dim titleLbl As New Label With {
                        .Text = If(String.IsNullOrWhiteSpace(room.Title), "Room Tanpa Judul", room.Title),
                        .Font = AppTheme.FontCardTitle,
                        .ForeColor = AppTheme.TextPrimary,
                        .Location = New Point(12, 10),
                        .Size = New Size(cardWidth - 24, 22),
                        .AutoEllipsis = True,
                        .Cursor = Cursors.Hand
                    }

                    Dim codeLbl As New Label With {
                        .Text = $"Kode: {room.RoomCode}",
                        .Font = AppTheme.FontCaptionBold,
                        .ForeColor = AppTheme.PrimaryMocha,
                        .Location = New Point(12, 34),
                        .AutoSize = True,
                        .Cursor = Cursors.Hand
                    }

                    Dim statusTag As New Label With {
                        .Location = New Point(12, 58),
                        .Cursor = Cursors.Hand
                    }
                    ApplyRoomStatusTag(statusTag, room.Status)

                    Dim roleTag As New Label With {
                        .Location = New Point(statusTag.Right + 6, 58),
                        .Cursor = Cursors.Hand
                    }
                    If isHost Then
                        AppTheme.ApplyStatusTag(roleTag, "Host", AppTheme.AccentStrawberryTint, AppTheme.AccentStrawberryText)
                    Else
                        AppTheme.ApplyStatusTag(roleTag, "Anggota", AppTheme.TaupeTint, AppTheme.TaupeText)
                    End If

                    Dim selectAction As Action = Sub() SelectRoom(capturedRoom.Id)
                    AddHandler card.Click, Sub() selectAction()
                    AddHandler titleLbl.Click, Sub() selectAction()
                    AddHandler codeLbl.Click, Sub() selectAction()
                    AddHandler statusTag.Click, Sub() selectAction()
                    AddHandler roleTag.Click, Sub() selectAction()
                    AddHandler card.KeyDown, Sub(s, e)
                                                 If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Space Then
                                                     e.SuppressKeyPress = True
                                                     selectAction()
                                                 End If
                                             End Sub
                    AddHandler card.GotFocus, Sub() card.Invalidate()
                    AddHandler card.LostFocus, Sub() card.Invalidate()

                    card.Controls.AddRange(New Control() {titleLbl, codeLbl, statusTag, roleTag})
                    roleTag.Left = statusTag.Left + statusTag.PreferredWidth + 6
                    _roomsListPanel.Controls.Add(card)
                Next
            Finally
                _roomsListPanel.ResumeLayout(True)
            End Try
        End Sub

        Private Async Sub SelectRoom(roomId As String)
            If String.IsNullOrWhiteSpace(roomId) Then Return
            _selectedRoomId = roomId
            RenderMyRoomsList()
            Await LoadSelectedRoomOverviewAsync(roomId)
        End Sub

        Private Async Function LoadSelectedRoomOverviewAsync(roomId As String) As Task
            If IsDisposed OrElse String.IsNullOrWhiteSpace(roomId) Then Return

            Dim version = Threading.Interlocked.Increment(_overviewLoadVersion)
            Try
                Dim snapshot = Await _service.GetRoomOverviewAsync(roomId)
                If IsDisposed OrElse version <> _overviewLoadVersion Then Return

                _currentOverview = snapshot
                RenderRoomOverview(snapshot)
                EnsureRealtimeSubscriptionAsync(snapshot)
            Catch ex As FriendsOrderException
                If IsDisposed OrElse version <> _overviewLoadVersion Then Return
                ShowBanner(ex.Message, If(ex.ErrorKind = FriendsOrderErrorKind.Unauthorized OrElse ex.ErrorKind = FriendsOrderErrorKind.RlsBlocked, BannerTone.Warning, BannerTone.[Error]))
            Catch ex As Exception
                If IsDisposed OrElse version <> _overviewLoadVersion Then Return
                ShowBanner(FriendsOrderService.MapException(ex, "Gagal memuat detail room.").Message, BannerTone.[Error])
            End Try
        End Function

        Private Sub RenderEmptyOverview()
            _roomHeaderCard.Visible = True
            _addItemsCard.Visible = False
            _membersSectionLabel.Visible = False
            _membersItemsPanel.Visible = False

            _overviewTitleLabel.Text = "Belum ada room dipilih"
            AppTheme.ApplyStatusTag(_overviewStatusTag, "Kosong", AppTheme.TaupeTint, AppTheme.TaupeText)
            AppTheme.ApplyStatusTag(_overviewRoleTag, "-", AppTheme.TaupeTint, AppTheme.TaupeText)
            _overviewCodeLabel.Text = "Kode Undangan: -"
            _copyCodeBtn.Visible = False
            _overviewHostLabel.Text = "Buat room baru atau gabung dengan kode undangan untuk memulai pesanan bersama."
            _overviewMemberCountLabel.Text = String.Empty
            _overviewOrderNumberLabel.Text = String.Empty
            _lockRoomBtn.Visible = False
            _lockedNoticeLabel.Visible = False

            _groupSummaryMetaLabel.Text = "0 anggota · 0 item"
            _groupTotalValueLabel.Text = AppTheme.FormatRupiah(0D)
            LayoutOverviewCards()
            LayoutGroupTotalBar()
        End Sub

        Private Sub RenderRoomOverview(snapshot As RoomOverviewSnapshot)
            If snapshot Is Nothing OrElse snapshot.Room Is Nothing Then
                RenderEmptyOverview()
                Return
            End If

            Dim room = snapshot.Room
            _roomHeaderCard.Visible = True
            _addItemsCard.Visible = True
            _membersSectionLabel.Visible = True
            _membersItemsPanel.Visible = True

            _overviewTitleLabel.Text = If(String.IsNullOrWhiteSpace(room.Title), "Room Tanpa Judul", room.Title)
            ApplyRoomStatusTag(_overviewStatusTag, room.Status)

            If snapshot.IsCurrentUserHost Then
                AppTheme.ApplyStatusTag(_overviewRoleTag, "Anda Host", AppTheme.AccentStrawberryTint, AppTheme.AccentStrawberryText)
            Else
                AppTheme.ApplyStatusTag(_overviewRoleTag, "Anggota", AppTheme.TaupeTint, AppTheme.TaupeText)
            End If

            _overviewCodeLabel.Text = $"Kode Undangan: {room.RoomCode}"
            _copyCodeBtn.Visible = True
            _copyCodeBtn.Left = _overviewCodeLabel.Right + 12

            _overviewHostLabel.Text = $"Host: {snapshot.HostDisplayName}"
            _overviewMemberCountLabel.Text = $"Anggota: {snapshot.Members.Count}"
            _overviewMemberCountLabel.Left = Math.Max(220, _overviewHostLabel.Right + 24)

            If snapshot.ParentOrder IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(snapshot.ParentOrder.OrderNumber) Then
                _overviewOrderNumberLabel.Text = $"Order grup: {snapshot.ParentOrder.OrderNumber} (Status: {snapshot.ParentOrder.Status})"
            Else
                _overviewOrderNumberLabel.Text = "Order grup: Akan dibuat otomatis saat item pertama ditambahkan."
            End If

            ' Only host sees active Lock Room action on open room; non-host never gets lock action.
            _lockRoomBtn.Visible = snapshot.IsCurrentUserHost AndAlso room.IsOpen
            _lockRoomBtn.Enabled = snapshot.CanLockRoom AndAlso Not _isMutating

            If room.IsLocked Then
                _lockedNoticeLabel.Text = "Room dikunci oleh host. Anggota baru dan penambahan item telah dihentikan."
                _lockedNoticeLabel.BackColor = AppTheme.WarningTint
                _lockedNoticeLabel.ForeColor = AppTheme.WarningText
                _lockedNoticeLabel.Visible = True
            ElseIf room.IsCancelled Then
                _lockedNoticeLabel.Text = "Room ini dibatalkan dan tidak menerima perubahan lagi."
                _lockedNoticeLabel.BackColor = AppTheme.ErrorTint
                _lockedNoticeLabel.ForeColor = AppTheme.ErrorText
                _lockedNoticeLabel.Visible = True
            ElseIf room.IsSubmitted Then
                _lockedNoticeLabel.Text = "Pesanan room ini sudah dikirim."
                _lockedNoticeLabel.BackColor = AppTheme.SageTint
                _lockedNoticeLabel.ForeColor = AppTheme.SageText
                _lockedNoticeLabel.Visible = True
            Else
                _lockedNoticeLabel.Visible = False
            End If

            UpdateAddItemsEnabledState()
            UpdatePendingDraftsUi()
            RenderMemberSummaries(snapshot)

            _groupSummaryMetaLabel.Text = $"{snapshot.Members.Count} anggota · {snapshot.TotalItemCount} item"
            _groupTotalValueLabel.Text = AppTheme.FormatRupiah(snapshot.GroupTotal)

            LayoutOverviewCards()
            LayoutGroupTotalBar()
        End Sub

        Private Sub UpdateAddItemsEnabledState()
            Dim canAdd = _currentOverview IsNot Nothing AndAlso _currentOverview.CanAddItems AndAlso Not _isMutating
            _quickMenuCombo.Enabled = canAdd AndAlso _availableMenus.Count > 0
            _quickQtyUpDown.Enabled = canAdd AndAlso _availableMenus.Count > 0
            _quickNotesTextBox.Enabled = canAdd AndAlso _availableMenus.Count > 0
            _quickAddBtn.Enabled = canAdd AndAlso _availableMenus.Count > 0

            If _currentOverview IsNot Nothing AndAlso _currentOverview.Room IsNot Nothing AndAlso Not _currentOverview.Room.IsOpen Then
                _addItemsHeadingLabel.Text = $"Penambahan Pesanan Dihentikan ({_currentOverview.Room.StatusDisplayLabel})"
            Else
                _addItemsHeadingLabel.Text = "Tambahkan Pesanan Anda"
            End If
        End Sub

        Private Sub UpdatePendingDraftsUi()
            If IsDisposed Then Return
            If InvokeRequired Then
                Try
                    BeginInvoke(Sub() UpdatePendingDraftsUi())
                Catch
                End Try
                Return
            End If

            Dim pendingCount = If(AppSession.IsAuthenticated, DrinkDraftHandoff.PendingCount, 0)
            Dim canAdd = _currentOverview IsNot Nothing AndAlso _currentOverview.CanAddItems AndAlso Not _isMutating

            If pendingCount > 0 Then
                _handoffInfoLabel.Text = $"Draft My Drink Creator milik Anda: {pendingCount} item siap dimasukkan."
                _handoffInfoLabel.ForeColor = AppTheme.AccentStrawberryText
                _importDraftsBtn.Text = $"Masukkan Draft ({pendingCount})"
            Else
                _handoffInfoLabel.Text = "Draft My Drink Creator: 0 item menunggu. Buat minuman di My Drink Creator lalu masukkan ke sini."
                _handoffInfoLabel.ForeColor = AppTheme.TextSecondary
                _importDraftsBtn.Text = "Masukkan Draft (0)"
            End If

            _importDraftsBtn.Width = AppTheme.MeasureButtonWidth(_importDraftsBtn.Text, AppTheme.FontBodyBold, 178, 24)
            _importDraftsBtn.Enabled = canAdd AndAlso pendingCount > 0
            LayoutOverviewCards()
        End Sub

        Private Sub RenderMemberSummaries(snapshot As RoomOverviewSnapshot)
            _membersItemsPanel.SuspendLayout()
            Try
                Dim existingControls = _membersItemsPanel.Controls.Cast(Of Control)().ToList()
                _membersItemsPanel.Controls.Clear()
                For Each existing In existingControls
                    existing.Dispose()
                Next

                Dim cardWidth = Math.Max(380, _overviewScrollPanel.ClientSize.Width - 48)
                Dim currentUserId = GetCurrentUserId()

                If snapshot.MemberSummaries.Count = 0 Then
                    Dim emptyLabel As New Label With {
                        .Text = "Belum ada anggota tercatat pada room ini.",
                        .Font = AppTheme.FontBody,
                        .ForeColor = AppTheme.TextSecondary,
                        .AutoSize = True,
                        .Margin = New Padding(0, 6, 0, 6)
                    }
                    _membersItemsPanel.Controls.Add(emptyLabel)
                    Return
                End If

                For Each summary In snapshot.MemberSummaries
                    Dim isSelf = String.Equals(summary.MemberUserId, currentUserId, StringComparison.OrdinalIgnoreCase)

                    Dim memberCard As New Panel With {
                        .Width = cardWidth,
                        .BackColor = AppTheme.SurfaceMilk,
                        .Margin = New Padding(0, 0, 0, 10),
                        .Padding = New Padding(14, 12, 14, 12)
                    }
                    AddHandler memberCard.Paint, AddressOf PaintCardBorder

                    Dim headerY = 12
                    Dim nameLabel As New Label With {
                        .Text = summary.DisplayName,
                        .Font = AppTheme.FontCardTitle,
                        .ForeColor = AppTheme.TextPrimary,
                        .Location = New Point(14, headerY),
                        .AutoSize = True
                    }
                    memberCard.Controls.Add(nameLabel)

                    Dim tagLeft = nameLabel.Left + nameLabel.PreferredWidth + 8
                    If summary.IsHost Then
                        Dim hostTag As New Label With {.Location = New Point(tagLeft, headerY + 1)}
                        AppTheme.ApplyStatusTag(hostTag, "Host", AppTheme.AccentStrawberryTint, AppTheme.AccentStrawberryText)
                        memberCard.Controls.Add(hostTag)
                        tagLeft += hostTag.PreferredWidth + 6
                    End If
                    If isSelf Then
                        Dim selfTag As New Label With {.Location = New Point(tagLeft, headerY + 1)}
                        AppTheme.ApplyStatusTag(selfTag, "Anda", AppTheme.SageTint, AppTheme.SageText)
                        memberCard.Controls.Add(selfTag)
                    End If

                    Dim subtotalLabel As New Label With {
                        .Text = $"Subtotal: {AppTheme.FormatRupiah(summary.Subtotal)} ({summary.ItemCount} item)",
                        .Font = AppTheme.FontBodyBold,
                        .ForeColor = AppTheme.PrimaryMocha,
                        .AutoSize = True
                    }
                    subtotalLabel.Location = New Point(Math.Max(220, cardWidth - subtotalLabel.PreferredWidth - 14), headerY + 1)
                    memberCard.Controls.Add(subtotalLabel)

                    Dim cursorY = headerY + 30
                    If summary.Items Is Nothing OrElse summary.Items.Count = 0 Then
                        Dim noItemLbl As New Label With {
                            .Text = "Belum menambahkan item pesanan.",
                            .Font = AppTheme.FontCaption,
                            .ForeColor = AppTheme.TextSecondary,
                            .Location = New Point(14, cursorY),
                            .AutoSize = True
                        }
                        memberCard.Controls.Add(noItemLbl)
                        cursorY += 24
                    Else
                        For Each item In summary.Items
                            Dim itemRow As New Panel With {
                                .Location = New Point(14, cursorY),
                                .Width = cardWidth - 28,
                                .BackColor = AppTheme.CanvasCream
                            }

                            Dim unitTotal = item.UnitBasePrice + item.OptionsExtraPrice
                            Dim itemTitle As New Label With {
                                .Text = $"{item.Quantity}x {item.ItemNameSnapshot}",
                                .Font = AppTheme.FontBodyBold,
                                .ForeColor = AppTheme.TextPrimary,
                                .Location = New Point(10, 6),
                                .AutoSize = True
                            }
                            Dim lineTotalLbl As New Label With {
                                .Text = AppTheme.FormatRupiah(item.LineTotal),
                                .Font = AppTheme.FontBodyBold,
                                .ForeColor = AppTheme.TextPrimary,
                                .AutoSize = True
                            }
                            lineTotalLbl.Location = New Point(Math.Max(180, itemRow.Width - lineTotalLbl.PreferredWidth - 10), 6)

                            Dim metaParts As New List(Of String) From {
                                $"Harga/unit: {AppTheme.FormatRupiah(unitTotal)}"
                            }
                            Dim optSummary = RoomOrderItemPayload.FormatOrderItemOptions(item.OptionsSnapshot)
                            If Not String.IsNullOrWhiteSpace(optSummary) Then
                                metaParts.Add(optSummary)
                            End If
                            If Not String.IsNullOrWhiteSpace(item.Notes) Then
                                metaParts.Add($"Catatan: {item.Notes}")
                            End If

                            Dim metaLbl As New Label With {
                                .Text = String.Join("  |  ", metaParts),
                                .Font = AppTheme.FontCaption,
                                .ForeColor = AppTheme.TextSecondary,
                                .Location = New Point(10, 26),
                                .Size = New Size(itemRow.Width - 20, 18),
                                .AutoEllipsis = True
                            }

                            itemRow.Height = 50
                            itemRow.Controls.AddRange(New Control() {itemTitle, lineTotalLbl, metaLbl})
                            memberCard.Controls.Add(itemRow)
                            cursorY += itemRow.Height + 6
                        Next
                    End If

                    memberCard.Height = cursorY + 8
                    _membersItemsPanel.Controls.Add(memberCard)
                Next
            Finally
                _membersItemsPanel.ResumeLayout(True)
            End Try
        End Sub

        Private Async Sub OnCreateRoomSubmitClick(sender As Object, e As EventArgs)
            If _isMutating Then Return
            _isMutating = True
            _submitCreateBtn.Enabled = False
            _createFeedbackLabel.ForeColor = AppTheme.TextSecondary
            _createFeedbackLabel.Text = "Membuat room baru dan kode undangan..."
            UpdateToolbarEnabledState()

            Try
                Dim created = Await _service.CreateRoomAsync(_createTitleTextBox.Text)
                If IsDisposed Then Return

                _lastCreatedCode = created.RoomCode
                _createTitleTextBox.Clear()
                _createFeedbackLabel.ForeColor = AppTheme.SageText
                _createFeedbackLabel.Text = $"Room '{created.Title}' berhasil dibuat! Kode undangan: {created.RoomCode}"
                _copyCreatedCodeBtn.Visible = True
                ShowBanner($"Room '{created.Title}' siap digunakan. Bagikan kode undangan {created.RoomCode} ke teman Anda.", BannerTone.Success)

                _selectedRoomId = created.Id
                _myRooms = Await _service.GetMyRoomsAsync()
                If IsDisposed Then Return
                RenderMyRoomsList()
                Await LoadSelectedRoomOverviewAsync(created.Id)
            Catch ex As FriendsOrderException
                If IsDisposed Then Return
                _createFeedbackLabel.ForeColor = AppTheme.ErrorText
                _createFeedbackLabel.Text = ex.Message
            Catch ex As Exception
                If IsDisposed Then Return
                _createFeedbackLabel.ForeColor = AppTheme.ErrorText
                _createFeedbackLabel.Text = FriendsOrderService.MapException(ex, "Gagal membuat room.").Message
            Finally
                If Not IsDisposed Then
                    _isMutating = False
                    _submitCreateBtn.Enabled = True
                    UpdateToolbarEnabledState()
                End If
            End Try
        End Sub

        Private Async Sub OnJoinRoomSubmitClick(sender As Object, e As EventArgs)
            If _isMutating Then Return
            _isMutating = True
            _submitJoinBtn.Enabled = False
            _joinFeedbackLabel.ForeColor = AppTheme.TextSecondary
            _joinFeedbackLabel.Text = "Memvalidasi kode undangan melalui RPC server..."
            UpdateToolbarEnabledState()

            Try
                Dim joined = Await _service.JoinRoomByCodeAsync(_joinCodeTextBox.Text)
                If IsDisposed Then Return

                _joinCodeTextBox.Clear()
                _joinFeedbackLabel.ForeColor = AppTheme.SageText
                _joinFeedbackLabel.Text = $"Berhasil bergabung ke room '{joined.Title}'."
                ShowBanner($"Anda berhasil bergabung ke room '{joined.Title}'.", BannerTone.Success)

                _selectedRoomId = joined.RoomId
                _myRooms = Await _service.GetMyRoomsAsync()
                If IsDisposed Then Return
                RenderMyRoomsList()
                Await LoadSelectedRoomOverviewAsync(joined.RoomId)
                SetDrawerMode(DrawerMode.None)
            Catch ex As FriendsOrderException
                If IsDisposed Then Return
                _joinFeedbackLabel.ForeColor = If(ex.ErrorKind = FriendsOrderErrorKind.RoomLocked OrElse ex.ErrorKind = FriendsOrderErrorKind.RpcNotDeployed, AppTheme.WarningText, AppTheme.ErrorText)
                _joinFeedbackLabel.Text = ex.Message
            Catch ex As Exception
                If IsDisposed Then Return
                _joinFeedbackLabel.ForeColor = AppTheme.ErrorText
                _joinFeedbackLabel.Text = FriendsOrderService.MapException(ex, "Gagal bergabung ke room.").Message
            Finally
                If Not IsDisposed Then
                    _isMutating = False
                    _submitJoinBtn.Enabled = True
                    UpdateToolbarEnabledState()
                End If
            End Try
        End Sub

        Private Async Sub OnLockRoomClick(sender As Object, e As EventArgs)
            If _isMutating OrElse _currentOverview Is Nothing OrElse _currentOverview.Room Is Nothing Then Return

            Dim room = _currentOverview.Room
            Dim confirm = MessageBox.Show(
                Me,
                $"Kunci room '{room.Title}' ({room.RoomCode}) sekarang?" & Environment.NewLine & Environment.NewLine &
                "Setelah dikunci:" & Environment.NewLine &
                "- Anggota baru tidak dapat bergabung lagi." & Environment.NewLine &
                "- Penambahan item baru akan dihentikan." & Environment.NewLine &
                "- Status room menjadi 'Terkunci' (belum dikirim sebagai order final).",
                "Konfirmasi Kunci Room",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2)

            If confirm <> DialogResult.Yes Then Return

            _isMutating = True
            UpdateToolbarEnabledState()
            UpdateAddItemsEnabledState()

            Try
                Await _service.LockRoomAsync(room.Id)
                If IsDisposed Then Return

                ShowBanner($"Room '{room.Title}' berhasil dikunci. Tidak ada anggota atau item baru yang dapat masuk.", BannerTone.Warning)
                _myRooms = Await _service.GetMyRoomsAsync()
                If IsDisposed Then Return
                RenderMyRoomsList()
                Await LoadSelectedRoomOverviewAsync(room.Id)
            Catch ex As FriendsOrderException
                If IsDisposed Then Return
                ShowBanner(ex.Message, BannerTone.[Error])
            Catch ex As Exception
                If IsDisposed Then Return
                ShowBanner(FriendsOrderService.MapException(ex, "Gagal mengunci room.").Message, BannerTone.[Error])
            Finally
                If Not IsDisposed Then
                    _isMutating = False
                    UpdateToolbarEnabledState()
                    UpdateAddItemsEnabledState()
                End If
            End Try
        End Sub

        Private Async Sub OnImportDrinkDraftsClick(sender As Object, e As EventArgs)
            If _isMutating OrElse _currentOverview Is Nothing OrElse _currentOverview.Room Is Nothing Then Return

            Dim pendingDrafts = DrinkDraftHandoff.GetPendingDrafts()
            If pendingDrafts.Count = 0 Then
                ShowBanner("Tidak ada draft dari My Drink Creator untuk akun aktif.", BannerTone.Info)
                UpdatePendingDraftsUi()
                Return
            End If

            _isMutating = True
            UpdateToolbarEnabledState()
            UpdateAddItemsEnabledState()
            UpdatePendingDraftsUi()

            Dim addedCount = 0
            Dim needsRefreshAfterPartial = False
            Dim roomId = _currentOverview.Room.Id
            Try
                For i = 0 To pendingDrafts.Count - 1
                    Await _service.AddDraftToRoomAsync(roomId, pendingDrafts(i))
                    DrinkDraftHandoff.RemovePendingAt(0)
                    addedCount += 1
                Next

                If IsDisposed Then Return
                ShowBanner($"{addedCount} draft minuman dari My Drink Creator berhasil dimasukkan ke pesanan grup.", BannerTone.Success)
                Await LoadSelectedRoomOverviewAsync(roomId)
            Catch ex As FriendsOrderException
                If IsDisposed Then Return
                Dim prefix = If(addedCount > 0, $"{addedCount} draft sempat masuk sebelum gagal: ", String.Empty)
                ShowBanner(prefix & ex.Message, BannerTone.[Error])
                needsRefreshAfterPartial = (addedCount > 0)
            Catch ex As Exception
                If IsDisposed Then Return
                ShowBanner(FriendsOrderService.MapException(ex, "Gagal memasukkan draft minuman ke room.").Message, BannerTone.[Error])
                needsRefreshAfterPartial = (addedCount > 0)
            Finally
                If Not IsDisposed Then
                    _isMutating = False
                    UpdateToolbarEnabledState()
                    UpdateAddItemsEnabledState()
                    UpdatePendingDraftsUi()
                End If
            End Try

            If needsRefreshAfterPartial AndAlso Not IsDisposed Then
                Await LoadSelectedRoomOverviewAsync(roomId)
            End If
        End Sub

        Private Async Sub OnQuickAddMenuItemClick(sender As Object, e As EventArgs)
            If _isMutating OrElse _currentOverview Is Nothing OrElse _currentOverview.Room Is Nothing Then Return

            Dim selectedOption = TryCast(_quickMenuCombo.SelectedItem, QuickMenuOptionItem)
            If selectedOption Is Nothing OrElse selectedOption.MenuItem Is Nothing Then
                ShowBanner("Pilih menu terlebih dahulu.", BannerTone.Warning)
                Return
            End If

            _isMutating = True
            UpdateToolbarEnabledState()
            UpdateAddItemsEnabledState()

            Try
                Dim qty = CInt(_quickQtyUpDown.Value)
                Await _service.AddMenuItemToRoomAsync(_currentOverview.Room.Id, selectedOption.MenuItem, qty, _quickNotesTextBox.Text)
                If IsDisposed Then Return

                _quickQtyUpDown.Value = 1D
                _quickNotesTextBox.Clear()
                ShowBanner($"Berhasil menambahkan {qty}x {selectedOption.MenuItem.Name} ke pesanan Anda.", BannerTone.Success)
                Await LoadSelectedRoomOverviewAsync(_currentOverview.Room.Id)
            Catch ex As FriendsOrderException
                If IsDisposed Then Return
                ShowBanner(ex.Message, BannerTone.[Error])
            Catch ex As Exception
                If IsDisposed Then Return
                ShowBanner(FriendsOrderService.MapException(ex, "Gagal menambahkan item ke room.").Message, BannerTone.[Error])
            Finally
                If Not IsDisposed Then
                    _isMutating = False
                    UpdateToolbarEnabledState()
                    UpdateAddItemsEnabledState()
                End If
            End Try
        End Sub

        Private Async Sub EnsureRealtimeSubscriptionAsync(snapshot As RoomOverviewSnapshot)
            If IsDisposed OrElse snapshot Is Nothing OrElse snapshot.Room Is Nothing Then Return

            Dim roomId = snapshot.Room.Id
            Dim orderId = If(snapshot.ParentOrder?.Id, String.Empty)

            If _realtimeSubscription IsNot Nothing AndAlso
               Not _realtimeSubscription.IsDisposed AndAlso
               String.Equals(_realtimeSubscribedRoomId, roomId, StringComparison.OrdinalIgnoreCase) AndAlso
               String.Equals(_realtimeSubscribedOrderId, orderId, StringComparison.OrdinalIgnoreCase) Then
                Return
            End If

            DisposeRealtimeSubscription()
            _realtimeSubscribedRoomId = roomId
            _realtimeSubscribedOrderId = orderId

            Try
                Dim subRef = Await _service.SubscribeRoomUpdatesAsync(
                    roomId,
                    orderId,
                    Sub(changedTable)
                        ScheduleRealtimeRefresh()
                    End Sub,
                    Sub(state, detail)
                        SetRealtimeStatus(state, detail)
                    End Sub)

                If IsDisposed OrElse Not String.Equals(_selectedRoomId, roomId, StringComparison.OrdinalIgnoreCase) Then
                    subRef?.Dispose()
                    Return
                End If

                _realtimeSubscription = subRef
            Catch ex As Exception
                SetRealtimeStatus(FriendsRoomRealtimeState.[Error], "Realtime tidak tersedia: " & ex.Message & " Gunakan Muat Ulang.")
            End Try
        End Sub

        Private Sub ScheduleRealtimeRefresh()
            If IsDisposed OrElse _realtimeDebounceTimer Is Nothing Then Return
            If InvokeRequired Then
                Try
                    BeginInvoke(Sub() ScheduleRealtimeRefresh())
                Catch
                End Try
                Return
            End If

            _realtimeDebounceTimer.Stop()
            _realtimeDebounceTimer.Start()
        End Sub

        Private Async Sub OnRealtimeDebounceTick(sender As Object, e As EventArgs)
            _realtimeDebounceTimer.Stop()
            If IsDisposed OrElse Not AppSession.IsAuthenticated OrElse String.IsNullOrWhiteSpace(_selectedRoomId) Then Return

            Try
                _myRooms = Await _service.GetMyRoomsAsync()
                If IsDisposed Then Return
                RenderMyRoomsList()
                Await LoadSelectedRoomOverviewAsync(_selectedRoomId)
            Catch
                ' Keep current view intact; user can manually refresh.
            End Try
        End Sub

        Private Sub DisposeRealtimeSubscription()
            If _realtimeDebounceTimer IsNot Nothing Then
                _realtimeDebounceTimer.Stop()
            End If
            If _realtimeSubscription IsNot Nothing Then
                Try
                    _realtimeSubscription.Dispose()
                Catch
                End Try
                _realtimeSubscription = Nothing
            End If
            _realtimeSubscribedRoomId = String.Empty
            _realtimeSubscribedOrderId = String.Empty
        End Sub

        Private Sub ClearRoomState()
            DisposeRealtimeSubscription()
            _myRooms.Clear()
            _selectedRoomId = String.Empty
            _currentOverview = Nothing
            _lastCreatedCode = String.Empty
            _createTitleTextBox.Clear()
            _joinCodeTextBox.Clear()
            _quickNotesTextBox.Clear()
            _copyCreatedCodeBtn.Visible = False
            SetDrawerMode(DrawerMode.None)
            HideBanner()
            RenderMyRoomsList()
            RenderEmptyOverview()
            SetRealtimeStatus(FriendsRoomRealtimeState.Disconnected, "Tidak ada sesi room aktif.")
            UpdatePendingDraftsUi()
        End Sub

        Private Sub OnSessionChanged(sender As Object, e As EventArgs)
            If IsDisposed Then Return
            If InvokeRequired Then
                Try
                    BeginInvoke(Sub() OnSessionChanged(sender, e))
                Catch
                End Try
                Return
            End If

            Dim nextUserId = GetCurrentUserId()
            If Not String.Equals(_lastObservedUserId, nextUserId, StringComparison.OrdinalIgnoreCase) Then
                _lastObservedUserId = nextUserId
                ClearRoomState()
            End If

            UpdatePendingDraftsUi()
            ReloadAllAsync()
        End Sub

        Private Sub OnConfigurationChanged(sender As Object, e As EventArgs)
            If IsDisposed Then Return
            If InvokeRequired Then
                Try
                    BeginInvoke(Sub() OnConfigurationChanged(sender, e))
                Catch
                End Try
                Return
            End If

            ClearRoomState()
            ReloadAllAsync()
        End Sub

        Private Sub OnPendingDraftsChanged(sender As Object, e As EventArgs)
            UpdatePendingDraftsUi()
        End Sub

        Private Sub CopyCodeToClipboard(code As String, successMessage As String)
            If String.IsNullOrWhiteSpace(code) Then Return
            Try
                Clipboard.SetText(code)
                ShowBanner(successMessage, BannerTone.Success)
            Catch
                ShowBanner($"Kode undangan: {code}", BannerTone.Info)
            End Try
        End Sub

        Private Shared Sub ApplyRoomStatusTag(label As Label, statusValue As String)
            Dim text = OrderRoom.GetStatusDisplayLabel(statusValue)
            Select Case If(statusValue, String.Empty).Trim().ToLowerInvariant()
                Case "open"
                    AppTheme.ApplyStatusTag(label, text, AppTheme.SageTint, AppTheme.SageText)
                Case "locked"
                    AppTheme.ApplyStatusTag(label, text, AppTheme.WarningTint, AppTheme.WarningText)
                Case "submitted"
                    AppTheme.ApplyStatusTag(label, text, AppTheme.AccentStrawberryTint, AppTheme.AccentStrawberryText)
                Case "cancelled"
                    AppTheme.ApplyStatusTag(label, text, AppTheme.ErrorTint, AppTheme.ErrorText)
                Case Else
                    AppTheme.ApplyStatusTag(label, text, AppTheme.TaupeTint, AppTheme.TaupeText)
            End Select
        End Sub

        Private Shared Function GetCurrentUserId() As String
            Dim user = AppSession.CurrentUser
            If user Is Nothing OrElse String.IsNullOrWhiteSpace(user.Id) Then
                Return String.Empty
            End If
            Return user.Id.Trim()
        End Function

        Private Shared Sub EnableDoubleBuffer(target As Control)
            GetType(Control).GetProperty("DoubleBuffered", BindingFlags.Instance Or BindingFlags.NonPublic)?.SetValue(target, True, Nothing)
        End Sub

        Private Sub CleanupRuntimeResources()
            If _cleanupDone Then Return
            _cleanupDone = True

            If _runtimeInitialized Then
                RemoveHandler AppSession.SessionChanged, AddressOf OnSessionChanged
                RemoveHandler SupabaseClientProvider.ConfigurationChanged, AddressOf OnConfigurationChanged
                RemoveHandler DrinkDraftHandoff.PendingDraftsChanged, AddressOf OnPendingDraftsChanged
            End If

            DisposeRealtimeSubscription()

            If _realtimeDebounceTimer IsNot Nothing Then
                RemoveHandler _realtimeDebounceTimer.Tick, AddressOf OnRealtimeDebounceTick
                _realtimeDebounceTimer.Dispose()
                _realtimeDebounceTimer = Nothing
            End If

            _toolTip.Dispose()
        End Sub
    End Class
End Namespace
