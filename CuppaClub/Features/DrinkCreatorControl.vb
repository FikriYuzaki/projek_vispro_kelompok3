Option Strict On
Option Explicit On

Imports System.Collections.Concurrent
Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Net.Http
Imports System.Reflection
Imports CuppaClub.Core
Imports CuppaClub.Forms
Imports CuppaClub.Models
Imports CuppaClub.Services

Namespace Features
    Public Class DrinkCreatorControl
        Inherits UserControl

        Private Enum CreatorViewState
            Unconfigured
            Loading
            ErrorState
            EmptyDatabase
            AllSoldOut
            EmptyFilter
            Ready
        End Enum

        Private Enum OptionsViewState
            Idle
            Loading
            ErrorState
            Unsupported
            EmptyOptions
            Ready
        End Enum

        Private Shared ReadOnly ImageHttpClient As New HttpClient With {
            .Timeout = TimeSpan.FromSeconds(8),
            .MaxResponseContentBufferSize = 4L * 1024L * 1024L
        }
        Private Shared ReadOnly ImageBytesCache As New ConcurrentDictionary(Of String, Byte())(StringComparer.OrdinalIgnoreCase)

        Private ReadOnly _drinkOptionService As DrinkOptionService
        Private ReadOnly _categoryButtons As New Dictionary(Of String, Button)(StringComparer.OrdinalIgnoreCase)
        Private ReadOnly _toolTip As New ToolTip()

        Private _totalMenuCount As Integer
        Private _availableItems As New List(Of Models.MenuItem)()
        Private _filteredItems As New List(Of Models.MenuItem)()
        Private _selectedCategory As String = "Semua"
        Private _selectedItemId As String = String.Empty
        Private _selectedItem As Models.MenuItem
        Private _currentItemOptions As New List(Of MenuOption)()
        Private _selectedOptionIds As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Private _optionsState As OptionsViewState = OptionsViewState.Idle
        Private _optionsErrorMessage As String = String.Empty
        Private _isSubmittingDraft As Boolean
        Private _isUpdatingSelectionControls As Boolean
        Private _menuLoadVersion As Integer
        Private _optionsLoadVersion As Integer
        Private _runtimeInitialized As Boolean
        Private _cleanupDone As Boolean

        Private _currentDraft As ConfiguredDrinkDraft
        Private _lastSubmittedDraft As ConfiguredDrinkDraft

        Private ReadOnly _stateOverlay As New Panel()
        Private ReadOnly _stateCard As New Panel()
        Private ReadOnly _stateBadge As New Label()
        Private ReadOnly _stateTitle As New Label()
        Private ReadOnly _stateMessage As New Label()
        Private ReadOnly _stateProgress As New ProgressBar()
        Private ReadOnly _stateActionBtn As New Button()
        Private _currentOverlayState As CreatorViewState = CreatorViewState.Loading

        Public Event DraftSubmitted As EventHandler(Of DrinkDraftSubmittedEventArgs)

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property CurrentDraft As ConfiguredDrinkDraft
            Get
                Return If(_currentDraft IsNot Nothing, _currentDraft.Clone(), Nothing)
            End Get
        End Property

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property LastSubmittedDraft As ConfiguredDrinkDraft
            Get
                Return If(_lastSubmittedDraft IsNot Nothing, _lastSubmittedDraft.Clone(), Nothing)
            End Get
        End Property

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property SubmittedDrafts As IReadOnlyList(Of ConfiguredDrinkDraft)
            Get
                Return DrinkDraftHandoff.GetPendingDrafts()
            End Get
        End Property

        Public Sub New()
            Me.New(Nothing)
        End Sub

        Public Sub New(drinkOptionService As DrinkOptionService)
            InitializeComponent()

            If LicenseManager.UsageMode = LicenseUsageMode.Designtime OrElse DesignMode Then
                Return
            End If

            _drinkOptionService = If(drinkOptionService, New DrinkOptionService())
            _runtimeInitialized = True
            WireRuntimeBehavior()

            AddHandler SupabaseClientProvider.ConfigurationChanged, AddressOf OnConfigurationChanged
            AddHandler AppSession.SessionChanged, AddressOf OnSessionChanged
            AddHandler DrinkDraftHandoff.PendingDraftsChanged, AddressOf OnPendingDraftsChanged
            AddHandler Me.Load, AddressOf OnControlLoad
        End Sub

        Private Sub OnControlLoad(sender As Object, e As EventArgs)
            If IsDisposed OrElse LicenseManager.UsageMode = LicenseUsageMode.Designtime OrElse DesignMode Then
                Return
            End If

            LayoutToolbar()
            LayoutWorkspace()
            UpdateDraftButtonState()
            LoadAvailableMenuAsync()
        End Sub

        Private Sub WireRuntimeBehavior()
            EnableDoubleBuffer(_drinkCardsPanel)
            EnableDoubleBuffer(_builderBodyPanel)

            AddHandler _toolbarPanel.Paint, Sub(s, e)
                                                Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                                    e.Graphics.DrawLine(pen, 0, _toolbarPanel.Height - 1, _toolbarPanel.Width, _toolbarPanel.Height - 1)
                                                End Using
                                            End Sub
            AddHandler _toolbarPanel.Resize, Sub() LayoutToolbar()
            AddHandler _workspaceHost.Resize, Sub() LayoutWorkspace()
            AddHandler _builderPane.Paint, Sub(s, e)
                                               Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                                   e.Graphics.DrawLine(pen, 0, 0, 0, _builderPane.Height)
                                               End Using
                                           End Sub
            AddHandler _selectedHeaderPanel.Paint, Sub(s, e)
                                                       Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                                           e.Graphics.DrawLine(pen, 1, _selectedHeaderPanel.Height - 1, _selectedHeaderPanel.Width, _selectedHeaderPanel.Height - 1)
                                                       End Using
                                                   End Sub
            AddHandler _summaryPanel.Paint, Sub(s, e)
                                                Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                                    e.Graphics.DrawLine(pen, 1, 0, _summaryPanel.Width, 0)
                                                    e.Graphics.DrawLine(pen, 20, 65, _summaryPanel.Width - 20, 65)
                                                    Dim splitX = CalculateSummarySplitX(_summaryPanel.ClientSize.Width)
                                                    e.Graphics.DrawLine(pen, splitX, 71, splitX, 113)
                                                End Using
                                            End Sub
            AddHandler _summaryPanel.Resize, Sub()
                                                 LayoutSummaryPanel()
                                                 _summaryPanel.Invalidate()
                                             End Sub
            AddHandler _builderBodyPanel.Resize, Sub() LayoutBuilderBody()
            AddHandler _drinkCardsPanel.Resize, Sub() ResizeDrinkCards()
            AddHandler _orderMetaCard.Paint, Sub(s, e)
                                                 Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                                     e.Graphics.DrawRectangle(pen, 0, 0, _orderMetaCard.Width - 1, _orderMetaCard.Height - 1)
                                                 End Using
                                             End Sub
            AddHandler _selectedPhotoBox.Paint, AddressOf OnSelectedPhotoPaint

            _qtyMinusBtn.AccessibleName = "Kurangi kuantitas"
            _qtyPlusBtn.AccessibleName = "Tambah kuantitas"
            _quantityInput.AccessibleName = "Jumlah kuantitas"
            _searchTextBox.AccessibleName = "Cari minuman"
            _notesTextBox.AccessibleName = "Catatan pesanan"
            _toolTip.SetToolTip(_qtyMinusBtn, "Kurangi kuantitas")
            _toolTip.SetToolTip(_qtyPlusBtn, "Tambah kuantitas")

            _clearSearchBtn.Width = AppTheme.MeasureButtonWidth(_clearSearchBtn.Text, AppTheme.FontBodyBold, 94, 24)
            _reloadBtn.Width = AppTheme.MeasureButtonWidth(_reloadBtn.Text, AppTheme.FontBodyBold, 112, 24)
            _handoffBtn.Width = AppTheme.MeasureButtonWidth(_handoffBtn.Text, AppTheme.FontBodyBold, 188, 28)
            _resetOptionsBtn.Width = AppTheme.MeasureButtonWidth(_resetOptionsBtn.Text, AppTheme.FontBodyBold, 114, 24)
            _viewDraftsBtn.Width = AppTheme.MeasureButtonWidth("Lihat Draft (99)", AppTheme.FontBodyBold, 128, 24)

            AppTheme.StyleSecondaryButton(_clearSearchBtn)
            AppTheme.StyleSecondaryButton(_reloadBtn)
            AppTheme.StyleSecondaryButton(_qtyMinusBtn)
            AppTheme.StyleSecondaryButton(_qtyPlusBtn)
            AppTheme.StylePrimaryButton(_handoffBtn)
            AppTheme.StyleSecondaryButton(_resetOptionsBtn)
            AppTheme.StyleSecondaryButton(_viewDraftsBtn)

            AttachButtonFocusIndicator(_clearSearchBtn)
            AttachButtonFocusIndicator(_reloadBtn)
            AttachButtonFocusIndicator(_qtyMinusBtn)
            AttachButtonFocusIndicator(_qtyPlusBtn)
            AttachButtonFocusIndicator(_handoffBtn)
            AttachButtonFocusIndicator(_resetOptionsBtn)
            AttachButtonFocusIndicator(_viewDraftsBtn)
            AttachButtonEnabledStyle(_handoffBtn, isPrimary:=True)
            AttachButtonEnabledStyle(_resetOptionsBtn, isPrimary:=False)
            AttachButtonEnabledStyle(_viewDraftsBtn, isPrimary:=False)
            AttachButtonEnabledStyle(_clearSearchBtn, isPrimary:=False)
            AttachButtonEnabledStyle(_reloadBtn, isPrimary:=False)
            AttachButtonEnabledStyle(_qtyMinusBtn, isPrimary:=False)
            AttachButtonEnabledStyle(_qtyPlusBtn, isPrimary:=False)

            AddHandler _searchTextBox.TextChanged, AddressOf OnSearchTextChanged
            AddHandler _clearSearchBtn.Click, Sub()
                                                  _searchTextBox.Clear()
                                                  _searchTextBox.Focus()
                                              End Sub
            AddHandler _reloadBtn.Click, Sub() LoadAvailableMenuAsync()
            AddHandler _qtyMinusBtn.Click, AddressOf OnQuantityMinusClick
            AddHandler _qtyPlusBtn.Click, AddressOf OnQuantityPlusClick
            AddHandler _quantityInput.ValueChanged, AddressOf OnQuantityChanged
            AddHandler _quantityInput.TextChanged, AddressOf OnQuantityChanged
            Dim qtyEditBox = _quantityInput.Controls.OfType(Of TextBox)().FirstOrDefault()
            If qtyEditBox IsNot Nothing Then
                AddHandler qtyEditBox.TextChanged, AddressOf OnQuantityChanged
            End If
            AddHandler _notesTextBox.TextChanged, Sub() RefreshLiveSummary()
            AddHandler _handoffBtn.Click, AddressOf OnPrepareDraftClick
            AddHandler _resetOptionsBtn.Click, AddressOf OnResetSelectionClick
            AddHandler _viewDraftsBtn.Click, AddressOf OnViewDraftsClick

            _categoryButtons("Semua") = _catAllBtn
            _categoryButtons("Coffee") = _catCoffeeBtn
            _categoryButtons("Non-Coffee") = _catNonCoffeeBtn
            _categoryButtons("Pastry") = _catPastryBtn
            _categoryButtons("Dessert") = _catDessertBtn

            For Each kvp In _categoryButtons
                Dim captured = kvp.Key
                kvp.Value.Width = AppTheme.MeasureButtonWidth(kvp.Value.Text, AppTheme.FontBodyBold, 70, 22)
                AttachButtonFocusIndicator(kvp.Value)
                AddHandler kvp.Value.Click, Sub()
                                                _selectedCategory = captured
                                                UpdateCategoryButtonStyles()
                                                ApplyFiltersAndRender()
                                            End Sub
            Next

            BuildStateOverlay()
            UpdateCategoryButtonStyles()
            LayoutToolbar()
            LayoutWorkspace()
        End Sub

        Private Shared Sub EnableDoubleBuffer(target As Control)
            GetType(Control).GetProperty("DoubleBuffered", BindingFlags.Instance Or BindingFlags.NonPublic)?.SetValue(target, True, Nothing)
        End Sub

        Private Shared Sub AttachButtonFocusIndicator(button As Button)
            AddHandler button.Paint, Sub(s, e)
                                         Dim btn = DirectCast(s, Button)
                                         If btn.Focused AndAlso btn.Enabled Then
                                             Dim ringColor = If(btn.BackColor = AppTheme.PrimaryMocha OrElse btn.BackColor = AppTheme.AccentStrawberryDark,
                                                                AppTheme.SurfaceMilk,
                                                                AppTheme.FocusMocha)
                                             Using pen As New Pen(ringColor, 2.0F)
                                                 e.Graphics.DrawRectangle(pen, 2, 2, btn.Width - 5, btn.Height - 5)
                                             End Using
                                         End If
                                     End Sub
            AddHandler button.GotFocus, Sub(s, e) DirectCast(s, Control).Invalidate()
            AddHandler button.LostFocus, Sub(s, e) DirectCast(s, Control).Invalidate()
        End Sub

        Private Shared Sub AttachButtonEnabledStyle(button As Button, isPrimary As Boolean)
            Dim applyState As Action =
                Sub()
                    If button.Enabled Then
                        If isPrimary Then
                            AppTheme.StylePrimaryButton(button)
                        Else
                            AppTheme.StyleSecondaryButton(button)
                        End If
                    Else
                        button.Cursor = Cursors.Default
                        button.BackColor = AppTheme.TaupeTint
                        button.ForeColor = AppTheme.TaupeText
                        button.FlatAppearance.BorderColor = AppTheme.DividerAlmond
                    End If
                End Sub
            AddHandler button.EnabledChanged, Sub() applyState()
            applyState()
        End Sub

        Private Shared Function CalculateSummarySplitX(panelWidth As Integer) As Integer
            Dim contentWidth = Math.Max(260, panelWidth - 40)
            Return 20 + CInt(Math.Round(contentWidth * 0.45))
        End Function

        Private Sub LayoutToolbar()
            Dim w = _toolbarPanel.ClientSize.Width
            If w <= 0 Then Return

            Dim leftColumnX = 20 + Math.Max(_searchLabel.PreferredWidth, _categoryLabel.PreferredWidth) + 12

            _reloadBtn.Location = New Point(w - _reloadBtn.Width - 20, 11)
            Dim searchWidth = Math.Max(140, Math.Min(340, _reloadBtn.Left - leftColumnX - _clearSearchBtn.Width - 20))
            _searchTextBox.Location = New Point(leftColumnX, 13)
            _searchTextBox.Width = searchWidth
            _clearSearchBtn.Location = New Point(_searchTextBox.Right + 8, 11)

            Dim xPos = leftColumnX
            For Each catName In _categoryButtons.Keys
                Dim btn = _categoryButtons(catName)
                btn.Location = New Point(xPos, 52)
                xPos += btn.Width + 6
            Next

            Dim countWidth = _availableCountLabel.PreferredWidth
            Dim countLeft = w - countWidth - 20
            _availableCountLabel.Visible = (countLeft >= xPos + 12) AndAlso _availableItems.Count > 0 AndAlso Not _stateOverlay.Visible
            If _availableCountLabel.Visible Then
                _availableCountLabel.Location = New Point(countLeft, 59)
            End If
        End Sub

        Private Sub LayoutWorkspace()
            Dim totalWidth = _workspaceHost.ClientSize.Width
            If totalWidth <= 0 Then Return

            Dim desiredLeftWidth As Integer
            If totalWidth < 800 Then
                desiredLeftWidth = 286
            ElseIf totalWidth < 900 Then
                desiredLeftWidth = 324
            Else
                desiredLeftWidth = 352
            End If

            If _menuListPane.Width <> desiredLeftWidth Then
                _menuListPane.Width = desiredLeftWidth
            End If

            LayoutSelectedHeader()
            LayoutBuilderBody()
            LayoutSummaryPanel()
            ResizeDrinkCards()
            CenterStateCard()
        End Sub

        Private Sub LayoutSelectedHeader()
            Dim contentWidth = Math.Max(180, _selectedHeaderPanel.ClientSize.Width - 130)
            _selectedTitleLabel.Width = contentWidth
            _selectedDescriptionLabel.Width = contentWidth

            Dim categoryRight = _selectedCategoryLabel.Left + _selectedCategoryLabel.PreferredWidth
            _selectedAvailabilityTag.Location = New Point(categoryRight + 8, 10)
            Dim availRight = _selectedAvailabilityTag.Left + _selectedAvailabilityTag.PreferredWidth
            _selectedCustomTag.Location = New Point(availRight + 6, 10)
        End Sub

        Private Sub LayoutBuilderBody()
            Dim reservedScroll = If(_builderBodyPanel.VerticalScroll.Visible, 4, SystemInformation.VerticalScrollBarWidth + 4)
            Dim scrollWidth = _builderBodyPanel.ClientSize.Width - _builderBodyPanel.Padding.Horizontal - reservedScroll
            Dim targetWidth = Math.Max(280, scrollWidth)

            _builderFlow.Width = targetWidth
            _optionsHostPanel.Width = targetWidth
            _orderMetaCard.Width = targetWidth
            _notesTextBox.Width = Math.Max(220, targetWidth - 28)

            Dim hintFits = (_qtyPlusBtn.Right + 12 + _quantityHintLabel.PreferredWidth) <= (targetWidth - 14)
            _quantityHintLabel.Visible = hintFits
            If hintFits Then
                _quantityHintLabel.Location = New Point(_qtyPlusBtn.Right + 12, 19)
            End If

            For Each ctrl As Control In _optionsHostPanel.Controls
                ctrl.Width = targetWidth
                SyncOptionGroupCardLayout(ctrl, targetWidth)
            Next
        End Sub

        Private Sub SyncOptionGroupCardLayout(card As Control, cardWidth As Integer)
            Dim innerWidth = Math.Max(220, cardWidth - 28)

            If String.Equals(CStr(card.Tag), "status-card", StringComparison.Ordinal) Then
                Dim y = 14
                For Each child As Control In card.Controls
                    If TypeOf child Is Label AndAlso String.Equals(CStr(child.Tag), "wrap-label", StringComparison.Ordinal) Then
                        child.MaximumSize = New Size(innerWidth, 0)
                        Dim pref = child.GetPreferredSize(New Size(innerWidth, 0))
                        child.Location = New Point(14, y)
                        child.Size = New Size(Math.Min(innerWidth, pref.Width), Math.Max(20, pref.Height))
                        y = child.Bottom + 12
                    ElseIf TypeOf child Is ProgressBar Then
                        child.Location = New Point(14, y)
                        child.Width = innerWidth
                        y = child.Bottom + 12
                    ElseIf TypeOf child Is Button Then
                        child.Location = New Point(14, y)
                        y = child.Bottom + 12
                    Else
                        child.Location = New Point(14, y)
                        y = child.Bottom + 10
                    End If
                Next
                card.Height = Math.Max(76, y + 4)
                Return
            End If

            Dim maxBottom = 0
            For Each child As Control In card.Controls
                If TypeOf child Is FlowLayoutPanel Then
                    child.Width = innerWidth
                    Dim pref = child.GetPreferredSize(New Size(innerWidth, 0))
                    child.Height = Math.Max(56, pref.Height)
                ElseIf TypeOf child Is Panel AndAlso String.Equals(CStr(child.Tag), "radio-group-panel", StringComparison.Ordinal) Then
                    child.Width = innerWidth
                    For Each rowCtrl As Control In child.Controls
                        rowCtrl.Width = innerWidth
                    Next
                ElseIf TypeOf child Is Panel AndAlso String.Equals(CStr(child.Tag), "option-row", StringComparison.Ordinal) Then
                    child.Width = innerWidth
                ElseIf TypeOf child Is Button AndAlso String.Equals(CStr(child.Tag), "clear-group-btn", StringComparison.Ordinal) Then
                    child.Location = New Point(Math.Max(180, cardWidth - child.Width - 14), 10)
                End If

                If child.Bottom > maxBottom Then
                    maxBottom = child.Bottom
                End If
            Next

            If maxBottom > 0 Then
                card.Height = Math.Max(76, maxBottom + 12)
            End If
        End Sub

        Private Sub LayoutSummaryPanel()
            Dim w = _summaryPanel.ClientSize.Width
            If w <= 0 Then Return

            Dim contentWidth = Math.Max(260, w - 40)
            _summaryOptionsLabel.Width = contentWidth
            _feedbackBanner.Width = contentWidth
            _actionRowPanel.Width = contentWidth

            Dim splitX = CalculateSummarySplitX(w)
            Dim leftRightEdge = splitX - 12
            Dim rightColX = splitX + 12
            Dim rightEdge = w - 20

            Dim leftValWidth = Math.Max(76, leftRightEdge - (_extraPriceCaptionLabel.Left + _extraPriceCaptionLabel.PreferredWidth + 6))
            _basePriceValueLabel.Size = New Size(leftValWidth, 16)
            _basePriceValueLabel.Location = New Point(leftRightEdge - leftValWidth, 72)
            _extraPriceValueLabel.Size = New Size(leftValWidth, 16)
            _extraPriceValueLabel.Location = New Point(leftRightEdge - leftValWidth, 93)

            _unitTotalCaptionLabel.Location = New Point(rightColX, 72)
            _subtotalCaptionLabel.Location = New Point(rightColX, 92)

            Dim unitValWidth = Math.Max(82, rightEdge - (_unitTotalCaptionLabel.Right + 6))
            _unitTotalValueLabel.Size = New Size(unitValWidth, 18)
            _unitTotalValueLabel.Location = New Point(rightEdge - unitValWidth, 71)

            Dim subValWidth = Math.Max(92, rightEdge - (_subtotalCaptionLabel.Right + 6))
            _subtotalValueLabel.Size = New Size(subValWidth, 24)
            _subtotalValueLabel.Location = New Point(rightEdge - subValWidth, 88)

            Dim targetHeight = If(_feedbackBanner.Visible, 194, 166)
            Dim targetActionTop = If(_feedbackBanner.Visible, 150, 122)
            _actionRowPanel.Location = New Point(20, targetActionTop)
            If _summaryPanel.Height <> targetHeight Then
                _summaryPanel.Height = targetHeight
            End If

            Dim fullHandoffText = If(_isSubmittingDraft, "Memvalidasi...", "Siapkan Draft Pesanan")
            Dim fullResetText = "Reset Pilihan"
            Dim viewText = $"Lihat Draft ({DrinkDraftHandoff.PendingCount})"

            Dim fullHandoffW = AppTheme.MeasureButtonWidth(fullHandoffText, AppTheme.FontBodyBold, 184, 26)
            Dim fullResetW = AppTheme.MeasureButtonWidth(fullResetText, AppTheme.FontBodyBold, 110, 22)
            Dim viewW = AppTheme.MeasureButtonWidth(viewText, AppTheme.FontBodyBold, 118, 22)

            If fullHandoffW + 8 + fullResetW + 8 + viewW <= contentWidth Then
                _handoffBtn.Text = fullHandoffText
                _resetOptionsBtn.Text = fullResetText
                _viewDraftsBtn.Text = viewText
                _handoffBtn.Width = fullHandoffW
                _resetOptionsBtn.Width = fullResetW
                _viewDraftsBtn.Width = viewW
            Else
                Dim compactHandoffText = If(_isSubmittingDraft, "Validasi...", "Siapkan Draft")
                Dim compactResetText = "Reset"
                Dim compactViewText = $"Draft ({DrinkDraftHandoff.PendingCount})"
                Dim compactResetW = AppTheme.MeasureButtonWidth(compactResetText, AppTheme.FontBodyBold, 68, 18)
                Dim compactViewW = AppTheme.MeasureButtonWidth(compactViewText, AppTheme.FontBodyBold, 84, 18)
                Dim availableForHandoff = Math.Max(110, contentWidth - compactResetW - compactViewW - 16)

                _handoffBtn.Text = compactHandoffText
                _resetOptionsBtn.Text = compactResetText
                _viewDraftsBtn.Text = compactViewText
                _handoffBtn.Width = availableForHandoff
                _resetOptionsBtn.Width = compactResetW
                _viewDraftsBtn.Width = compactViewW
            End If

            _handoffBtn.Location = New Point(0, 0)
            _resetOptionsBtn.Location = New Point(_handoffBtn.Right + 8, 0)
            _viewDraftsBtn.Location = New Point(_resetOptionsBtn.Right + 8, 0)
        End Sub

        Private Sub ResizeDrinkCards()
            Dim reservedScroll = If(_drinkCardsPanel.VerticalScroll.Visible, 4, SystemInformation.VerticalScrollBarWidth + 4)
            Dim cardWidth = Math.Max(200, _drinkCardsPanel.ClientSize.Width - _drinkCardsPanel.Padding.Horizontal - reservedScroll - 4)
            _drinkCardsPanel.SuspendLayout()
            Try
                For Each ctrl As Control In _drinkCardsPanel.Controls
                    ctrl.Width = cardWidth
                Next
            Finally
                _drinkCardsPanel.ResumeLayout(True)
            End Try
            _drinkCardsPanel.AutoScroll = False
            _drinkCardsPanel.HorizontalScroll.Enabled = False
            _drinkCardsPanel.HorizontalScroll.Visible = False
            _drinkCardsPanel.HorizontalScroll.Maximum = 0
            _drinkCardsPanel.AutoScroll = True
        End Sub

        Private Sub BuildStateOverlay()
            _stateOverlay.Dock = DockStyle.Fill
            _stateOverlay.BackColor = AppTheme.CanvasCream
            _stateOverlay.Visible = False
            AddHandler _stateOverlay.Resize, Sub() CenterStateCard()

            _stateCard.Width = 540
            _stateCard.BackColor = AppTheme.SurfaceMilk
            _stateCard.Padding = New Padding(28, 24, 28, 24)
            AddHandler _stateCard.Paint, Sub(s, e)
                                             Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                                 e.Graphics.DrawRectangle(pen, 0, 0, _stateCard.Width - 1, _stateCard.Height - 1)
                                             End Using
                                         End Sub

            Dim stateFlow As New FlowLayoutPanel With {
                .Dock = DockStyle.Fill,
                .FlowDirection = FlowDirection.TopDown,
                .WrapContents = False,
                .AutoSize = True,
                .BackColor = Color.Transparent
            }

            _stateBadge.Margin = New Padding(0, 0, 0, 10)

            _stateTitle.Font = AppTheme.FontPageTitle
            _stateTitle.ForeColor = AppTheme.TextPrimary
            _stateTitle.MaximumSize = New Size(480, 0)
            _stateTitle.AutoSize = True
            _stateTitle.Margin = New Padding(0, 0, 0, 8)

            _stateMessage.Font = AppTheme.FontBody
            _stateMessage.ForeColor = AppTheme.TextSecondary
            _stateMessage.MaximumSize = New Size(480, 0)
            _stateMessage.AutoSize = True
            _stateMessage.Margin = New Padding(0, 0, 0, 16)

            _stateProgress.Style = ProgressBarStyle.Marquee
            _stateProgress.MarqueeAnimationSpeed = 28
            _stateProgress.Size = New Size(480, 14)
            _stateProgress.Margin = New Padding(0, 0, 0, 16)
            _stateProgress.Visible = False

            _stateActionBtn.Size = New Size(204, 36)
            AppTheme.StylePrimaryButton(_stateActionBtn)
            AttachButtonFocusIndicator(_stateActionBtn)
            AddHandler _stateActionBtn.Click, AddressOf OnStateActionClick

            stateFlow.Controls.AddRange({
                _stateBadge,
                _stateTitle,
                _stateMessage,
                _stateProgress,
                _stateActionBtn
            })

            _stateCard.Controls.Add(stateFlow)
            _stateCard.Height = stateFlow.PreferredSize.Height + _stateCard.Padding.Vertical + 6
            _stateOverlay.Controls.Add(_stateCard)
            _workspaceHost.Controls.Add(_stateOverlay)
            _stateOverlay.BringToFront()
        End Sub

        Private Shared Sub DisposeChildControls(container As Control)
            Dim existing = container.Controls.Cast(Of Control)().ToArray()
            container.Controls.Clear()
            For Each ctrl In existing
                ctrl.Dispose()
            Next
        End Sub

        Public Async Sub LoadAvailableMenuAsync(Optional preserveFeedback As Boolean = False)
            _menuLoadVersion += 1
            Dim activeVersion = _menuLoadVersion

            If Not _drinkOptionService.IsDataSourceConfigured Then
                _totalMenuCount = 0
                _availableItems = New List(Of Models.MenuItem)()
                _filteredItems = New List(Of Models.MenuItem)()
                ClearSelectedItem()
                ShowStateOverlay(CreatorViewState.Unconfigured, String.Empty)
                Return
            End If

            ShowStateOverlay(CreatorViewState.Loading, String.Empty)
            _reloadBtn.Enabled = False
            If Not preserveFeedback Then
                HideFeedbackBanner()
            End If

            Try
                Dim result = Await _drinkOptionService.GetAvailableMenuItemsAsync()
                If IsDisposed OrElse Not IsHandleCreated OrElse activeVersion <> _menuLoadVersion Then
                    Return
                End If

                _totalMenuCount = result.TotalMenuCount
                _availableItems = result.AvailableItems
                _reloadBtn.Enabled = True
                ApplyFiltersAndRender(preserveFeedback, forceReloadOptions:=True)
            Catch ex As SupabaseNotConfiguredException
                If IsDisposed OrElse Not IsHandleCreated OrElse activeVersion <> _menuLoadVersion Then
                    Return
                End If
                _reloadBtn.Enabled = True
                ShowStateOverlay(CreatorViewState.Unconfigured, ex.Message)
            Catch ex As Exception
                If IsDisposed OrElse Not IsHandleCreated OrElse activeVersion <> _menuLoadVersion Then
                    Return
                End If
                _reloadBtn.Enabled = True
                ShowStateOverlay(CreatorViewState.ErrorState, FormatUserError(ex, "Tidak dapat memuat daftar minuman dari Supabase."))
            End Try
        End Sub

        Private Sub ApplyFiltersAndRender(
            Optional preserveFeedback As Boolean = False,
            Optional forceReloadOptions As Boolean = False
        )
            If Not _drinkOptionService.IsDataSourceConfigured Then
                ShowStateOverlay(CreatorViewState.Unconfigured, String.Empty)
                Return
            End If

            If _totalMenuCount = 0 Then
                _filteredItems = New List(Of Models.MenuItem)()
                ClearSelectedItem()
                ShowStateOverlay(CreatorViewState.EmptyDatabase, String.Empty)
                Return
            End If

            If _availableItems.Count = 0 Then
                _filteredItems = New List(Of Models.MenuItem)()
                ClearSelectedItem()
                ShowStateOverlay(CreatorViewState.AllSoldOut, String.Empty)
                Return
            End If

            _filteredItems = MenuService.FilterMenuItems(
                _availableItems,
                _searchTextBox.Text,
                _selectedCategory,
                showUnavailable:=False)

            If _filteredItems.Count = 0 Then
                ClearSelectedItem()
                ShowStateOverlay(CreatorViewState.EmptyFilter, String.Empty)
                Return
            End If

            _stateOverlay.Visible = False
            _menuListPane.Visible = True
            _builderPane.Visible = True
            _availableCountLabel.Text = $"Menampilkan {_filteredItems.Count} dari {_availableItems.Count} item tersedia"
            LayoutToolbar()

            RenderDrinkSelectorCards(preserveFeedback, forceReloadOptions)
        End Sub

        Private Sub RenderDrinkSelectorCards(
            Optional preserveFeedback As Boolean = False,
            Optional forceReloadOptions As Boolean = False
        )
            Dim nextSelection = _filteredItems.FirstOrDefault(Function(x) String.Equals(x.Id, _selectedItemId, StringComparison.OrdinalIgnoreCase))
            If nextSelection Is Nothing AndAlso _filteredItems.Count > 0 Then
                nextSelection = _filteredItems(0)
            End If

            Dim selectionChanged = nextSelection IsNot Nothing AndAlso
                                   (forceReloadOptions OrElse
                                    _selectedItem Is Nothing OrElse
                                    Not String.Equals(_selectedItem.Id, nextSelection.Id, StringComparison.OrdinalIgnoreCase) OrElse
                                    _selectedItem.SupportsCustomization <> nextSelection.SupportsCustomization OrElse
                                    _optionsState = OptionsViewState.Idle OrElse
                                    preserveFeedback)

            _selectedItemId = If(nextSelection?.Id, String.Empty)

            _drinkCardsPanel.SuspendLayout()
            Try
                DisposeChildControls(_drinkCardsPanel)

                Dim reservedScroll = If(_drinkCardsPanel.VerticalScroll.Visible, 4, SystemInformation.VerticalScrollBarWidth + 4)
                Dim cardWidth = Math.Max(200, _drinkCardsPanel.ClientSize.Width - _drinkCardsPanel.Padding.Horizontal - reservedScroll - 4)
                Dim tabIdx = 10
                For Each item In _filteredItems
                    Dim card As New DrinkSelectorCard(item) With {
                        .Size = New Size(cardWidth, 92),
                        .Margin = New Padding(0, 0, 0, 10),
                        .TabIndex = tabIdx,
                        .IsSelected = String.Equals(item.Id, _selectedItemId, StringComparison.OrdinalIgnoreCase)
                    }
                    tabIdx += 1
                    Dim captured = item
                    AddHandler card.Activated, Sub() SelectMenuItem(captured, forceReloadOptions:=False)
                    _drinkCardsPanel.Controls.Add(card)
                    LoadCardPhotoAsync(card, item.ImageUrl, _menuLoadVersion)
                Next
            Finally
                _drinkCardsPanel.ResumeLayout(True)
            End Try

            If nextSelection IsNot Nothing Then
                If selectionChanged Then
                    SelectMenuItem(nextSelection, forceReloadOptions:=True, preserveFeedback:=preserveFeedback)
                Else
                    _selectedItem = nextSelection
                    UpdateSelectedHeader(nextSelection)
                    RefreshLiveSummary()
                End If
            End If
        End Sub

        Private Sub SelectMenuItem(
            item As Models.MenuItem,
            Optional forceReloadOptions As Boolean = False,
            Optional preserveFeedback As Boolean = False
        )
            If item Is Nothing Then Return

            Dim isSameItem = _selectedItem IsNot Nothing AndAlso
                             String.Equals(_selectedItem.Id, item.Id, StringComparison.OrdinalIgnoreCase)

            _selectedItem = item
            _selectedItemId = If(item.Id, String.Empty)
            If Not preserveFeedback Then
                HideFeedbackBanner()
            End If

            For Each ctrl As Control In _drinkCardsPanel.Controls
                Dim card = TryCast(ctrl, DrinkSelectorCard)
                If card IsNot Nothing Then
                    card.IsSelected = String.Equals(card.Item.Id, _selectedItemId, StringComparison.OrdinalIgnoreCase)
                End If
            Next

            UpdateSelectedHeader(item)
            LoadSelectedPhotoAsync(item.ImageUrl, _menuLoadVersion, _selectedItemId)

            If Not isSameItem OrElse forceReloadOptions Then
                _isUpdatingSelectionControls = True
                Try
                    _quantityInput.Value = 1D
                    _notesTextBox.Clear()
                Finally
                    _isUpdatingSelectionControls = False
                End Try

                _currentItemOptions = New List(Of MenuOption)()
                _selectedOptionIds = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

                If Not item.SupportsCustomization Then
                    _optionsLoadVersion += 1
                    _optionsState = OptionsViewState.Unsupported
                    _optionsErrorMessage = String.Empty
                    RenderOptionsSection()
                    RefreshLiveSummary()
                Else
                    LoadOptionsForSelectedItemAsync(item)
                End If
            Else
                RefreshLiveSummary()
            End If
        End Sub

        Private Async Sub LoadOptionsForSelectedItemAsync(item As Models.MenuItem)
            If item Is Nothing OrElse String.IsNullOrWhiteSpace(item.Id) Then Return

            _optionsLoadVersion += 1
            Dim activeVersion = _optionsLoadVersion
            Dim expectedItemId = item.Id

            _optionsState = OptionsViewState.Loading
            _optionsErrorMessage = String.Empty
            _currentItemOptions = New List(Of MenuOption)()
            _selectedOptionIds = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            RenderOptionsSection()
            RefreshLiveSummary()

            Try
                Dim loadedOptions = Await _drinkOptionService.GetOptionsForMenuItemAsync(expectedItemId)
                If IsDisposed OrElse Not IsHandleCreated OrElse activeVersion <> _optionsLoadVersion Then
                    Return
                End If
                If Not String.Equals(_selectedItemId, expectedItemId, StringComparison.OrdinalIgnoreCase) Then
                    Return
                End If

                _currentItemOptions = loadedOptions
                If _currentItemOptions.Count = 0 Then
                    _optionsState = OptionsViewState.EmptyOptions
                    _selectedOptionIds = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                Else
                    _optionsState = OptionsViewState.Ready
                    _selectedOptionIds = DrinkOptionService.GetDefaultOptionIds(_currentItemOptions)
                End If

                RenderOptionsSection()
                RefreshLiveSummary()
            Catch ex As Exception
                If IsDisposed OrElse Not IsHandleCreated OrElse activeVersion <> _optionsLoadVersion Then
                    Return
                End If
                If Not String.Equals(_selectedItemId, expectedItemId, StringComparison.OrdinalIgnoreCase) Then
                    Return
                End If

                _optionsState = OptionsViewState.ErrorState
                _optionsErrorMessage = FormatUserError(ex, "Gagal memuat opsi kustomisasi dari Supabase.")
                _currentItemOptions = New List(Of MenuOption)()
                _selectedOptionIds = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                RenderOptionsSection()
                RefreshLiveSummary()
            End Try
        End Sub

        Private Sub ClearSelectedItem()
            _selectedItem = Nothing
            _selectedItemId = String.Empty
            _currentDraft = Nothing
            _currentItemOptions = New List(Of MenuOption)()
            _selectedOptionIds = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            _optionsState = OptionsViewState.Idle
            SetSelectedPhoto(Nothing)
        End Sub

        Private Sub UpdateSelectedHeader(item As Models.MenuItem)
            _selectedCategoryLabel.Text = item.Category
            AppTheme.ApplyStatusTag(_selectedAvailabilityTag, "Tersedia", AppTheme.SageTint, AppTheme.SageText)

            If item.SupportsCustomization Then
                AppTheme.ApplyStatusTag(_selectedCustomTag, "Bisa Dikustom", AppTheme.AccentStrawberryTint, AppTheme.AccentStrawberryText)
            Else
                AppTheme.ApplyStatusTag(_selectedCustomTag, "Resep Standar", AppTheme.TaupeTint, AppTheme.TaupeText)
            End If

            _selectedTitleLabel.Text = item.Name
            Dim descText = If(
                String.IsNullOrWhiteSpace(item.Description),
                "Belum ada deskripsi untuk item ini.",
                item.Description.Trim())
            _selectedDescriptionLabel.Text = descText
            _toolTip.SetToolTip(_selectedDescriptionLabel, descText)
            _selectedBasePriceLabel.Text = "Harga dasar: " & AppTheme.FormatRupiah(item.BasePrice)

            LayoutSelectedHeader()
        End Sub

        Private Sub RenderOptionsSection()
            Dim cardWidth = Math.Max(280, _builderBodyPanel.ClientSize.Width - _builderBodyPanel.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth - 4)
            _builderFlow.Width = cardWidth
            _optionsHostPanel.Width = cardWidth

            _optionsHostPanel.SuspendLayout()
            Try
                DisposeChildControls(_optionsHostPanel)

                Select Case _optionsState
                    Case OptionsViewState.Loading
                        _optionsHostPanel.Controls.Add(BuildOptionsStatusCard(
                            cardWidth,
                            "Memuat Opsi Kustomisasi",
                            "Mengambil opsi ukuran, gula, es, susu, topping, dan espresso yang aktif dari Supabase...",
                            AppTheme.TaupeTint,
                            AppTheme.TaupeText,
                            showProgress:=True,
                            actionText:=String.Empty,
                            onAction:=Nothing))

                    Case OptionsViewState.ErrorState
                        _optionsHostPanel.Controls.Add(BuildOptionsStatusCard(
                            cardWidth,
                            "Opsi Kustomisasi Gagal Dimuat",
                            _optionsErrorMessage,
                            AppTheme.ErrorTint,
                            AppTheme.ErrorText,
                            showProgress:=False,
                            actionText:="Muat Ulang Opsi",
                            onAction:=Sub()
                                          If _selectedItem IsNot Nothing Then
                                              LoadOptionsForSelectedItemAsync(_selectedItem)
                                          End If
                                      End Sub))

                    Case OptionsViewState.Unsupported
                        _optionsHostPanel.Controls.Add(BuildOptionsStatusCard(
                            cardWidth,
                            "Tanpa Kustomisasi Tambahan",
                            "Item ini disajikan sesuai resep standar cafe (supports_customization = false). Anda tetap dapat mengatur kuantitas, catatan, dan menyiapkan draft pesanan dengan harga dasar.",
                            AppTheme.TaupeTint,
                            AppTheme.TaupeText,
                            showProgress:=False,
                            actionText:=String.Empty,
                            onAction:=Nothing))

                    Case OptionsViewState.EmptyOptions
                        _optionsHostPanel.Controls.Add(BuildOptionsStatusCard(
                            cardWidth,
                            "Belum Ada Opsi di Database",
                            "Item ini mendukung kustomisasi, tetapi belum memiliki opsi aktif di tabel menu_options. Anda tetap dapat menyiapkan draft dengan harga dasar atau memuat ulang opsi.",
                            AppTheme.WarningTint,
                            AppTheme.WarningText,
                            showProgress:=False,
                            actionText:="Muat Ulang Opsi",
                            onAction:=Sub()
                                          If _selectedItem IsNot Nothing Then
                                              LoadOptionsForSelectedItemAsync(_selectedItem)
                                          End If
                                      End Sub))

                    Case OptionsViewState.Ready
                        Dim buckets = DrinkOptionService.GroupOptions(_currentItemOptions)
                        If buckets.Count = 0 Then
                            _optionsHostPanel.Controls.Add(BuildOptionsStatusCard(
                                cardWidth,
                                "Belum Ada Opsi Aktif",
                                "Tidak ada kelompok opsi yang aktif untuk item ini.",
                                AppTheme.WarningTint,
                                AppTheme.WarningText,
                                showProgress:=False,
                                actionText:="Muat Ulang Opsi",
                                onAction:=Sub()
                                              If _selectedItem IsNot Nothing Then
                                                  LoadOptionsForSelectedItemAsync(_selectedItem)
                                              End If
                                          End Sub))
                        Else
                            For Each bucket In buckets
                                _optionsHostPanel.Controls.Add(BuildOptionGroupCard(bucket, cardWidth))
                            Next
                        End If
                End Select
            Finally
                _optionsHostPanel.ResumeLayout(True)
            End Try

            LayoutBuilderBody()
        End Sub

        Private Function BuildOptionsStatusCard(
            width As Integer,
            title As String,
            message As String,
            badgeBg As Color,
            badgeFg As Color,
            showProgress As Boolean,
            actionText As String,
            onAction As Action
        ) As Panel
            Dim card As New Panel With {
                .Width = width,
                .BackColor = AppTheme.CanvasCream,
                .Padding = New Padding(14),
                .Margin = New Padding(0, 0, 0, 10),
                .Tag = "status-card"
            }
            AddHandler card.Paint, Sub(s, e)
                                       Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                           e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1)
                                       End Using
                                   End Sub

            Dim innerWidth = Math.Max(220, width - 28)
            Dim y = 14
            Dim titleLbl As New Label With {
                .AutoSize = True,
                .Font = AppTheme.FontBodyBold,
                .BackColor = badgeBg,
                .ForeColor = badgeFg,
                .Padding = New Padding(8, 3, 8, 3),
                .Location = New Point(14, y),
                .Text = title
            }
            card.Controls.Add(titleLbl)
            y += titleLbl.PreferredHeight + 10

            Dim msgLbl As New Label With {
                .AutoSize = True,
                .Font = AppTheme.FontBody,
                .ForeColor = AppTheme.TextSecondary,
                .MaximumSize = New Size(innerWidth, 0),
                .Location = New Point(14, y),
                .Text = message,
                .Tag = "wrap-label"
            }
            Dim msgPref = msgLbl.GetPreferredSize(New Size(innerWidth, 0))
            msgLbl.Size = New Size(Math.Min(innerWidth, msgPref.Width), Math.Max(20, msgPref.Height))
            card.Controls.Add(msgLbl)
            y += msgLbl.Height + 12

            If showProgress Then
                Dim bar As New ProgressBar With {
                    .Style = ProgressBarStyle.Marquee,
                    .MarqueeAnimationSpeed = 28,
                    .Location = New Point(14, y),
                    .Size = New Size(innerWidth, 12)
                }
                card.Controls.Add(bar)
                y += bar.Height + 12
            End If

            If Not String.IsNullOrWhiteSpace(actionText) AndAlso onAction IsNot Nothing Then
                Dim actionBtn As New Button With {
                    .Text = actionText,
                    .Location = New Point(14, y),
                    .Size = New Size(AppTheme.MeasureButtonWidth(actionText, AppTheme.FontBodyBold, 140, 24), 32)
                }
                AppTheme.StyleSecondaryButton(actionBtn)
                AttachButtonFocusIndicator(actionBtn)
                AddHandler actionBtn.Click, Sub() onAction()
                card.Controls.Add(actionBtn)
                y += actionBtn.Height + 12
            End If

            card.Height = Math.Max(76, y + 4)
            Return card
        End Function

        Private Function BuildOptionGroupCard(bucket As DrinkOptionGroupBucket, cardWidth As Integer) As Panel
            Dim card As New Panel With {
                .Width = cardWidth,
                .BackColor = AppTheme.CanvasCream,
                .Margin = New Padding(0, 0, 0, 10)
            }
            AddHandler card.Paint, Sub(s, e)
                                       Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                           e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1)
                                       End Using
                                   End Sub

            Dim titleLabel As New Label With {
                .AutoSize = True,
                .Font = AppTheme.FontCardTitle,
                .ForeColor = AppTheme.TextPrimary,
                .Location = New Point(14, 12),
                .Text = bucket.DisplayName
            }
            card.Controls.Add(titleLabel)

            Dim hintText = If(bucket.AllowMultiple, "Boleh pilih lebih dari satu", "Pilih satu")
            Dim hintLabel As New Label With {
                .AutoSize = True,
                .Font = AppTheme.FontCaption,
                .ForeColor = AppTheme.TextSecondary,
                .Location = New Point(titleLabel.Left + titleLabel.PreferredWidth + 10, 15),
                .Text = hintText
            }
            card.Controls.Add(hintLabel)

            Dim yCursor = 40
            Dim clearGroupAction As Action = Nothing

            If String.Equals(bucket.GroupKey, "size", StringComparison.OrdinalIgnoreCase) Then
                Dim innerWidth = Math.Max(220, cardWidth - 28)
                Dim sizeFlow As New FlowLayoutPanel With {
                    .Location = New Point(14, yCursor),
                    .Width = innerWidth,
                    .AutoSize = True,
                    .AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    .FlowDirection = FlowDirection.LeftToRight,
                    .WrapContents = True,
                    .BackColor = Color.Transparent
                }

                Dim sizeButtons As New List(Of RadioButton)()
                For Each opt In bucket.Options
                    Dim capturedOpt = opt
                    Dim isSelected = _selectedOptionIds.Contains(opt.Id)
                    Dim priceLine = If(opt.ExtraPrice > 0D, "+" & AppTheme.FormatRupiah(opt.ExtraPrice), "Harga dasar")
                    Dim nameWidth = AppTheme.MeasureButtonWidth(opt.OptionName, AppTheme.FontBodyBold, 104, 24)
                    Dim priceWidth = AppTheme.MeasureButtonWidth(priceLine, AppTheme.FontBodyBold, 104, 24)
                    Dim btnWidth = Math.Max(nameWidth, priceWidth)

                    Dim rb As New RadioButton With {
                        .Appearance = Appearance.Button,
                        .AutoCheck = True,
                        .Checked = isSelected,
                        .Cursor = Cursors.Hand,
                        .FlatStyle = FlatStyle.Flat,
                        .Font = AppTheme.FontBodyBold,
                        .Text = $"{opt.OptionName}{Environment.NewLine}{priceLine}",
                        .AccessibleName = $"{bucket.DisplayName} {opt.OptionName}, {priceLine}",
                        .TextAlign = ContentAlignment.MiddleCenter,
                        .Size = New Size(btnWidth, 52),
                        .Margin = New Padding(0, 0, 8, 8),
                        .Tag = opt.Id
                    }
                    StyleSizeRadioButton(rb)
                    AddHandler rb.Paint, Sub(s, e)
                                             Dim target = DirectCast(s, RadioButton)
                                             If target.Focused Then
                                                 Using pen As New Pen(AppTheme.FocusMocha, 2.0F)
                                                     e.Graphics.DrawRectangle(pen, 3, 3, target.Width - 7, target.Height - 7)
                                                 End Using
                                             End If
                                         End Sub
                    AddHandler rb.GotFocus, Sub(s, e) DirectCast(s, Control).Invalidate()
                    AddHandler rb.LostFocus, Sub(s, e) DirectCast(s, Control).Invalidate()
                    sizeButtons.Add(rb)

                    AddHandler rb.CheckedChanged, Sub(sender, e)
                                                      If _isUpdatingSelectionControls Then Return
                                                      StyleSizeRadioButton(rb)
                                                      If rb.Checked Then
                                                          SelectSingleOptionInGroup(bucket, capturedOpt.Id)
                                                          For Each sibling In sizeButtons
                                                              StyleSizeRadioButton(sibling)
                                                          Next
                                                      End If
                                                  End Sub
                    sizeFlow.Controls.Add(rb)
                Next

                clearGroupAction = Sub()
                                       _isUpdatingSelectionControls = True
                                       Try
                                           For Each sibling In sizeButtons
                                               sibling.Checked = False
                                               StyleSizeRadioButton(sibling)
                                           Next
                                       Finally
                                           _isUpdatingSelectionControls = False
                                       End Try
                                       ClearSingleSelectGroupState(bucket)
                                   End Sub

                Dim flowPref = sizeFlow.GetPreferredSize(New Size(innerWidth, 0))
                sizeFlow.Height = Math.Max(60, flowPref.Height)
                card.Controls.Add(sizeFlow)
                yCursor = sizeFlow.Bottom + 8
            ElseIf bucket.AllowMultiple Then
                Dim rowWidth = Math.Max(220, cardWidth - 28)
                For Each opt In bucket.Options
                    Dim capturedOpt = opt
                    Dim row = BuildCheckboxOptionRow(opt, rowWidth, _selectedOptionIds.Contains(opt.Id),
                        Sub(isChecked)
                            If _isUpdatingSelectionControls Then Return
                            If isChecked Then
                                _selectedOptionIds.Add(capturedOpt.Id)
                            Else
                                _selectedOptionIds.Remove(capturedOpt.Id)
                            End If
                            HideFeedbackBanner()
                            RefreshLiveSummary()
                        End Sub)
                    row.Location = New Point(14, yCursor)
                    card.Controls.Add(row)
                    yCursor = row.Bottom + 6
                Next
            Else
                Dim rowWidth = Math.Max(220, cardWidth - 28)
                Dim radioGroupPanel As New Panel With {
                    .Location = New Point(14, yCursor),
                    .Width = rowWidth,
                    .BackColor = Color.Transparent,
                    .Tag = "radio-group-panel"
                }

                Dim innerY = 0
                For Each opt In bucket.Options
                    Dim capturedOpt = opt
                    Dim radioRow = BuildRadioOptionRow(opt, rowWidth, _selectedOptionIds.Contains(opt.Id), radioGroupPanel,
                        Sub(selectedId)
                            If _isUpdatingSelectionControls Then Return
                            SelectSingleOptionInGroup(bucket, selectedId)
                        End Sub)
                    radioRow.Location = New Point(0, innerY)
                    radioGroupPanel.Controls.Add(radioRow)
                    innerY = radioRow.Bottom + 6
                Next

                clearGroupAction = Sub()
                                       _isUpdatingSelectionControls = True
                                       Try
                                           For Each siblingRow As Control In radioGroupPanel.Controls
                                               For Each inner As Control In siblingRow.Controls
                                                   Dim siblingRb = TryCast(inner, RadioButton)
                                                   If siblingRb IsNot Nothing Then
                                                       siblingRb.Checked = False
                                                   End If
                                               Next
                                               siblingRow.BackColor = AppTheme.SurfaceMilk
                                               siblingRow.Invalidate()
                                           Next
                                       Finally
                                           _isUpdatingSelectionControls = False
                                       End Try
                                       ClearSingleSelectGroupState(bucket)
                                   End Sub

                radioGroupPanel.Height = Math.Max(34, innerY)
                card.Controls.Add(radioGroupPanel)
                yCursor = radioGroupPanel.Bottom + 6
            End If

            If Not bucket.AllowMultiple AndAlso clearGroupAction IsNot Nothing Then
                Dim clearBtnWidth = AppTheme.MeasureButtonWidth("Kosongkan", AppTheme.FontCaptionBold, 78, 16)
                Dim clearGroupBtn As New Button With {
                    .Text = "Kosongkan",
                    .AccessibleName = $"Kosongkan pilihan {bucket.DisplayName}",
                    .Font = AppTheme.FontCaptionBold,
                    .Size = New Size(clearBtnWidth, 24),
                    .Location = New Point(Math.Max(180, cardWidth - clearBtnWidth - 14), 10),
                    .Anchor = AnchorStyles.Top Or AnchorStyles.Right,
                    .Tag = "clear-group-btn"
                }
                AppTheme.StyleSecondaryButton(clearGroupBtn)
                clearGroupBtn.Font = AppTheme.FontCaptionBold
                AttachButtonFocusIndicator(clearGroupBtn)
                Dim capturedClear = clearGroupAction
                AddHandler clearGroupBtn.Click, Sub() capturedClear()
                card.Controls.Add(clearGroupBtn)
            End If

            card.Height = yCursor + 8
            Return card
        End Function

        Private Shared Sub StyleSizeRadioButton(rb As RadioButton)
            If rb.Checked Then
                rb.BackColor = AppTheme.AccentStrawberryTint
                rb.ForeColor = AppTheme.AccentStrawberryText
                rb.FlatAppearance.BorderSize = 2
                rb.FlatAppearance.BorderColor = AppTheme.AccentStrawberryDark
                rb.FlatAppearance.CheckedBackColor = AppTheme.AccentStrawberryTint
                rb.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#F6DCE1")
            Else
                rb.BackColor = AppTheme.SurfaceMilk
                rb.ForeColor = AppTheme.TextPrimary
                rb.FlatAppearance.BorderSize = 1
                rb.FlatAppearance.BorderColor = AppTheme.ControlBorder
                rb.FlatAppearance.CheckedBackColor = AppTheme.AccentStrawberryTint
                rb.FlatAppearance.MouseOverBackColor = AppTheme.SidebarOat
            End If
        End Sub

        Private Function BuildRadioOptionRow(
            opt As MenuOption,
            width As Integer,
            isSelected As Boolean,
            groupContainer As Panel,
            onSelected As Action(Of String)
        ) As Panel
            Dim isHovered As Boolean = False
            Dim row As New Panel With {
                .Width = width,
                .Height = 36,
                .Cursor = Cursors.Hand,
                .BackColor = If(isSelected, ColorTranslator.FromHtml("#FAF2E8"), AppTheme.SurfaceMilk),
                .Tag = "option-row"
            }

            Dim priceText = If(opt.ExtraPrice > 0D, "+" & AppTheme.FormatRupiah(opt.ExtraPrice), "Standar")
            Dim priceLabel As New Label With {
                .AutoSize = False,
                .Width = 104,
                .Height = 24,
                .Cursor = Cursors.Hand,
                .BackColor = Color.Transparent,
                .Font = AppTheme.FontCaptionBold,
                .ForeColor = If(opt.ExtraPrice > 0D, AppTheme.PrimaryMocha, AppTheme.TextSecondary),
                .TextAlign = ContentAlignment.MiddleRight,
                .Anchor = AnchorStyles.Top Or AnchorStyles.Right,
                .Location = New Point(width - 114, 6),
                .Text = priceText
            }

            ' RadioButtons live inside separate row panels, so mutual exclusion is coordinated manually.
            Dim rb As New RadioButton With {
                .AutoCheck = False,
                .Checked = isSelected,
                .Cursor = Cursors.Hand,
                .BackColor = Color.Transparent,
                .Font = AppTheme.FontBody,
                .ForeColor = AppTheme.TextPrimary,
                .Location = New Point(10, 7),
                .Size = New Size(Math.Max(120, width - 130), 22),
                .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right,
                .Text = opt.OptionName,
                .AccessibleName = $"{opt.OptionName}, {priceText}",
                .Tag = opt.Id
            }

            Dim refreshRowVisual As Action =
                Sub()
                    If rb.Checked Then
                        row.BackColor = ColorTranslator.FromHtml("#FAF2E8")
                    ElseIf isHovered Then
                        row.BackColor = ColorTranslator.FromHtml("#FCF7F0")
                    Else
                        row.BackColor = AppTheme.SurfaceMilk
                    End If
                    row.Invalidate()
                End Sub

            AddHandler row.Paint, Sub(s, e)
                                      Dim borderColor = AppTheme.ControlBorder
                                      Dim borderWidth = 1.0F
                                      If rb.Focused OrElse rb.Checked Then
                                          borderColor = AppTheme.FocusMocha
                                          borderWidth = 2.0F
                                      End If
                                      Using pen As New Pen(borderColor, borderWidth)
                                          e.Graphics.DrawRectangle(pen, 0, 0, row.Width - 1, row.Height - 1)
                                      End Using
                                  End Sub

            Dim activateRow As Action =
                Sub()
                    If _isUpdatingSelectionControls Then Return
                    _isUpdatingSelectionControls = True
                    Try
                        For Each siblingRow As Control In groupContainer.Controls
                            For Each inner As Control In siblingRow.Controls
                                Dim siblingRb = TryCast(inner, RadioButton)
                                If siblingRb IsNot Nothing Then
                                    Dim nextChecked = Object.ReferenceEquals(siblingRb, rb)
                                    siblingRb.Checked = nextChecked
                                    siblingRow.BackColor = If(nextChecked, ColorTranslator.FromHtml("#FAF2E8"), AppTheme.SurfaceMilk)
                                    siblingRow.Invalidate()
                                End If
                            Next
                        Next
                    Finally
                        _isUpdatingSelectionControls = False
                    End Try
                    onSelected(opt.Id)
                End Sub

            AddHandler rb.Click, Sub() activateRow()
            AddHandler rb.GotFocus, Sub() refreshRowVisual()
            AddHandler rb.LostFocus, Sub() refreshRowVisual()
            AddHandler rb.KeyDown, Sub(sender, e)
                                       If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Space Then
                                           e.Handled = True
                                           e.SuppressKeyPress = True
                                           activateRow()
                                       End If
                                   End Sub

            Dim onEnter As EventHandler = Sub()
                                              isHovered = True
                                              refreshRowVisual()
                                          End Sub
            Dim onLeave As EventHandler = Sub()
                                              isHovered = False
                                              refreshRowVisual()
                                          End Sub
            AddHandler row.MouseEnter, onEnter
            AddHandler row.MouseLeave, onLeave
            AddHandler rb.MouseEnter, onEnter
            AddHandler rb.MouseLeave, onLeave
            AddHandler priceLabel.MouseEnter, onEnter
            AddHandler priceLabel.MouseLeave, onLeave

            AddHandler row.Click, Sub()
                                      rb.Focus()
                                      activateRow()
                                  End Sub
            AddHandler priceLabel.Click, Sub()
                                             rb.Focus()
                                             activateRow()
                                         End Sub

            row.Controls.Add(rb)
            row.Controls.Add(priceLabel)
            Return row
        End Function

        Private Function BuildCheckboxOptionRow(
            opt As MenuOption,
            width As Integer,
            isSelected As Boolean,
            onToggled As Action(Of Boolean)
        ) As Panel
            Dim isHovered As Boolean = False
            Dim row As New Panel With {
                .Width = width,
                .Height = 36,
                .Cursor = Cursors.Hand,
                .BackColor = If(isSelected, ColorTranslator.FromHtml("#FAF2E8"), AppTheme.SurfaceMilk),
                .Tag = "option-row"
            }

            Dim priceText = If(opt.ExtraPrice > 0D, "+" & AppTheme.FormatRupiah(opt.ExtraPrice), "Gratis")
            Dim priceLabel As New Label With {
                .AutoSize = False,
                .Width = 104,
                .Height = 24,
                .Cursor = Cursors.Hand,
                .BackColor = Color.Transparent,
                .Font = AppTheme.FontCaptionBold,
                .ForeColor = If(opt.ExtraPrice > 0D, AppTheme.PrimaryMocha, AppTheme.TextSecondary),
                .TextAlign = ContentAlignment.MiddleRight,
                .Anchor = AnchorStyles.Top Or AnchorStyles.Right,
                .Location = New Point(width - 114, 6),
                .Text = priceText
            }

            Dim cb As New CheckBox With {
                .Checked = isSelected,
                .Cursor = Cursors.Hand,
                .BackColor = Color.Transparent,
                .Font = AppTheme.FontBody,
                .ForeColor = AppTheme.TextPrimary,
                .Location = New Point(10, 7),
                .Size = New Size(Math.Max(120, width - 130), 22),
                .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right,
                .Text = opt.OptionName,
                .AccessibleName = $"{opt.OptionName}, {priceText}",
                .Tag = opt.Id
            }

            Dim refreshRowVisual As Action =
                Sub()
                    If cb.Checked Then
                        row.BackColor = ColorTranslator.FromHtml("#FAF2E8")
                    ElseIf isHovered Then
                        row.BackColor = ColorTranslator.FromHtml("#FCF7F0")
                    Else
                        row.BackColor = AppTheme.SurfaceMilk
                    End If
                    row.Invalidate()
                End Sub

            AddHandler row.Paint, Sub(s, e)
                                      Dim borderColor = AppTheme.ControlBorder
                                      Dim borderWidth = 1.0F
                                      If cb.Focused OrElse cb.Checked Then
                                          borderColor = AppTheme.FocusMocha
                                          borderWidth = 2.0F
                                      End If
                                      Using pen As New Pen(borderColor, borderWidth)
                                          e.Graphics.DrawRectangle(pen, 0, 0, row.Width - 1, row.Height - 1)
                                      End Using
                                  End Sub

            AddHandler cb.CheckedChanged, Sub()
                                              refreshRowVisual()
                                              onToggled(cb.Checked)
                                          End Sub
            AddHandler cb.GotFocus, Sub() refreshRowVisual()
            AddHandler cb.LostFocus, Sub() refreshRowVisual()
            AddHandler cb.KeyDown, Sub(sender, e)
                                       If e.KeyCode = Keys.Enter Then
                                           e.Handled = True
                                           e.SuppressKeyPress = True
                                           cb.Checked = Not cb.Checked
                                       End If
                                   End Sub

            Dim onEnter As EventHandler = Sub()
                                              isHovered = True
                                              refreshRowVisual()
                                          End Sub
            Dim onLeave As EventHandler = Sub()
                                              isHovered = False
                                              refreshRowVisual()
                                          End Sub
            AddHandler row.MouseEnter, onEnter
            AddHandler row.MouseLeave, onLeave
            AddHandler cb.MouseEnter, onEnter
            AddHandler cb.MouseLeave, onLeave
            AddHandler priceLabel.MouseEnter, onEnter
            AddHandler priceLabel.MouseLeave, onLeave

            AddHandler row.Click, Sub()
                                      cb.Focus()
                                      cb.Checked = Not cb.Checked
                                  End Sub
            AddHandler priceLabel.Click, Sub()
                                             cb.Focus()
                                             cb.Checked = Not cb.Checked
                                         End Sub

            row.Controls.Add(cb)
            row.Controls.Add(priceLabel)
            Return row
        End Function

        Private Sub SelectSingleOptionInGroup(bucket As DrinkOptionGroupBucket, selectedOptionId As String)
            For Each opt In bucket.Options
                _selectedOptionIds.Remove(opt.Id)
            Next
            If Not String.IsNullOrWhiteSpace(selectedOptionId) Then
                _selectedOptionIds.Add(selectedOptionId)
            End If
            HideFeedbackBanner()
            RefreshLiveSummary()
        End Sub

        Private Sub ClearSingleSelectGroupState(bucket As DrinkOptionGroupBucket)
            For Each opt In bucket.Options
                _selectedOptionIds.Remove(opt.Id)
            Next
            HideFeedbackBanner()
            RefreshLiveSummary()
        End Sub

        Private Sub RefreshLiveSummary()
            If _selectedItem Is Nothing Then
                _currentDraft = Nothing
                _summaryOptionsLabel.Text = "Belum ada item dipilih."
                _toolTip.SetToolTip(_summaryOptionsLabel, String.Empty)
                _basePriceValueLabel.Text = AppTheme.FormatRupiah(0D)
                _extraPriceValueLabel.Text = AppTheme.FormatRupiah(0D)
                _unitTotalValueLabel.Text = AppTheme.FormatRupiah(0D)
                _subtotalCaptionLabel.Text = "Subtotal (0x)"
                _subtotalValueLabel.Text = AppTheme.FormatRupiah(0D)
                _handoffBtn.Enabled = False
                _resetOptionsBtn.Enabled = False
                _qtyMinusBtn.Enabled = False
                _qtyPlusBtn.Enabled = False
                _quantityInput.Enabled = False
                _notesTextBox.Enabled = False
                Return
            End If

            _quantityInput.Enabled = Not _isSubmittingDraft
            _notesTextBox.Enabled = Not _isSubmittingDraft

            Dim quantity = ReadSafeQuantity()
            If quantity <= 0 Then
                _currentDraft = Nothing
                Dim qtyMsg = $"{_selectedItem.Name}: kuantitas harus lebih dari 0."
                _summaryOptionsLabel.Text = qtyMsg
                _toolTip.SetToolTip(_summaryOptionsLabel, qtyMsg)
                _basePriceValueLabel.Text = AppTheme.FormatRupiah(_selectedItem.BasePrice)
                _extraPriceValueLabel.Text = AppTheme.FormatRupiah(0D)
                _unitTotalValueLabel.Text = AppTheme.FormatRupiah(_selectedItem.BasePrice)
                _subtotalCaptionLabel.Text = "Subtotal (0x)"
                _subtotalValueLabel.Text = AppTheme.FormatRupiah(0D)
                _handoffBtn.Enabled = False
                _resetOptionsBtn.Enabled = Not _isSubmittingDraft
                _qtyMinusBtn.Enabled = False
                _qtyPlusBtn.Enabled = Not _isSubmittingDraft
                Return
            End If

            Dim effectiveOptions = If(_selectedItem.SupportsCustomization, _currentItemOptions, New List(Of MenuOption)())
            Dim effectiveIds As IEnumerable(Of String) = If(_selectedItem.SupportsCustomization, CType(_selectedOptionIds, IEnumerable(Of String)), Array.Empty(Of String)())

            Dim validation = DrinkOptionService.ValidateSelection(
                _selectedItem,
                effectiveOptions,
                effectiveIds,
                quantity,
                _notesTextBox.Text)

            If Not validation.IsValid OrElse validation.Draft Is Nothing Then
                _currentDraft = Nothing
                _summaryOptionsLabel.Text = validation.ErrorMessage
                _toolTip.SetToolTip(_summaryOptionsLabel, validation.ErrorMessage)
                _basePriceValueLabel.Text = AppTheme.FormatRupiah(Math.Max(0D, _selectedItem.BasePrice))
                _extraPriceValueLabel.Text = AppTheme.FormatRupiah(0D)
                _unitTotalValueLabel.Text = AppTheme.FormatRupiah(Math.Max(0D, _selectedItem.BasePrice))
                _subtotalCaptionLabel.Text = $"Subtotal ({quantity}x)"
                _subtotalValueLabel.Text = AppTheme.FormatRupiah(0D)
                _handoffBtn.Enabled = False
                _resetOptionsBtn.Enabled = Not _isSubmittingDraft
                _qtyMinusBtn.Enabled = (quantity > 1) AndAlso (Not _isSubmittingDraft)
                _qtyPlusBtn.Enabled = (quantity < DrinkOptionService.MaxQuantity) AndAlso (Not _isSubmittingDraft)
                Return
            End If

            _currentDraft = validation.Draft
            Dim optionsSummary = _currentDraft.FormatSelectedOptionsSummary()
            Dim fullSummaryText = $"{_currentDraft.ItemNameSnapshot} | {optionsSummary}"
            _summaryOptionsLabel.Text = fullSummaryText
            _toolTip.SetToolTip(_summaryOptionsLabel, fullSummaryText)
            _basePriceValueLabel.Text = AppTheme.FormatRupiah(_currentDraft.UnitBasePrice)
            _extraPriceValueLabel.Text = If(_currentDraft.OptionsExtraPrice > 0D, "+" & AppTheme.FormatRupiah(_currentDraft.OptionsExtraPrice), AppTheme.FormatRupiah(0D))
            _unitTotalValueLabel.Text = AppTheme.FormatRupiah(_currentDraft.UnitTotal)
            _subtotalCaptionLabel.Text = $"Subtotal ({_currentDraft.Quantity}x)"
            _subtotalValueLabel.Text = AppTheme.FormatRupiah(_currentDraft.LineTotal)

            Dim optionsBlocking = (_optionsState = OptionsViewState.Loading) OrElse (_optionsState = OptionsViewState.ErrorState)
            _handoffBtn.Enabled = (Not _isSubmittingDraft) AndAlso (Not optionsBlocking)
            _resetOptionsBtn.Enabled = Not _isSubmittingDraft
            _qtyMinusBtn.Enabled = (quantity > 1) AndAlso (Not _isSubmittingDraft)
            _qtyPlusBtn.Enabled = (quantity < DrinkOptionService.MaxQuantity) AndAlso (Not _isSubmittingDraft)
        End Sub

        Private Function ReadSafeQuantity() As Integer
            Dim qtyEditBox = _quantityInput.Controls.OfType(Of TextBox)().FirstOrDefault()
            Dim sourceText = If(qtyEditBox IsNot Nothing, qtyEditBox.Text, _quantityInput.Text)
            Dim rawText = If(sourceText, String.Empty).Trim()
            If String.IsNullOrEmpty(rawText) Then
                Return 0
            End If

            Dim parsed As Integer
            If Not Integer.TryParse(rawText, parsed) Then
                Return 0
            End If

            Return parsed
        End Function

        Private Sub SetQuantityControlValue(quantity As Integer)
            Dim clamped = Math.Max(1, Math.Min(DrinkOptionService.MaxQuantity, quantity))
            _quantityInput.Value = CDec(clamped)
            Dim qtyEditBox = _quantityInput.Controls.OfType(Of TextBox)().FirstOrDefault()
            If qtyEditBox IsNot Nothing AndAlso Not String.Equals(qtyEditBox.Text, clamped.ToString(), StringComparison.Ordinal) Then
                qtyEditBox.Text = clamped.ToString()
            End If
        End Sub

        Private Sub OnQuantityMinusClick(sender As Object, e As EventArgs)
            Dim current = ReadSafeQuantity()
            Dim nextQty = If(current > 1, current - 1, 1)
            SetQuantityControlValue(nextQty)
            RefreshLiveSummary()
        End Sub

        Private Sub OnQuantityPlusClick(sender As Object, e As EventArgs)
            Dim current = ReadSafeQuantity()
            If current < DrinkOptionService.MaxQuantity Then
                SetQuantityControlValue(Math.Max(1, current + 1))
            End If
            RefreshLiveSummary()
        End Sub

        Private Sub OnQuantityChanged(sender As Object, e As EventArgs)
            If _isUpdatingSelectionControls Then Return
            HideFeedbackBanner()
            RefreshLiveSummary()
        End Sub

        Private Sub OnResetSelectionClick(sender As Object, e As EventArgs)
            If _selectedItem Is Nothing Then Return

            _isUpdatingSelectionControls = True
            Try
                SetQuantityControlValue(1)
                _notesTextBox.Clear()
            Finally
                _isUpdatingSelectionControls = False
            End Try

            If _selectedItem.SupportsCustomization AndAlso _optionsState = OptionsViewState.Ready Then
                _selectedOptionIds = DrinkOptionService.GetDefaultOptionIds(_currentItemOptions)
                RenderOptionsSection()
            Else
                _selectedOptionIds.Clear()
            End If

            HideFeedbackBanner()
            RefreshLiveSummary()
        End Sub

        Private Async Sub OnPrepareDraftClick(sender As Object, e As EventArgs)
            If _selectedItem Is Nothing OrElse _isSubmittingDraft Then Return

            Dim quantity = ReadSafeQuantity()
            If quantity <= 0 Then
                ShowFeedbackBanner("Kuantitas wajib lebih dari 0.", isError:=True)
                Return
            End If

            _isSubmittingDraft = True
            _handoffBtn.Enabled = False
            _resetOptionsBtn.Enabled = False
            LayoutSummaryPanel()
            HideFeedbackBanner()

            Dim itemId = _selectedItem.Id
            Dim selectedIds = If(_selectedItem.SupportsCustomization, _selectedOptionIds.ToList(), New List(Of String)())
            Dim notes = _notesTextBox.Text

            Try
                Dim validation = Await _drinkOptionService.ValidateAndBuildDraftAsync(itemId, selectedIds, quantity, notes)
                If IsDisposed OrElse Not IsHandleCreated Then Return

                _isSubmittingDraft = False
                LayoutSummaryPanel()

                If Not validation.IsValid OrElse validation.Draft Is Nothing Then
                    ShowFeedbackBanner(validation.ErrorMessage, isError:=True)
                    RefreshLiveSummary()
                    If validation.RefreshSuggested Then
                        LoadAvailableMenuAsync(preserveFeedback:=True)
                    End If
                    Return
                End If

                Dim finalizedDraft = validation.Draft.Clone()
                _currentDraft = finalizedDraft
                _lastSubmittedDraft = finalizedDraft
                DrinkDraftHandoff.Publish(finalizedDraft)
                RaiseEvent DraftSubmitted(Me, New DrinkDraftSubmittedEventArgs(finalizedDraft))

                UpdateDraftButtonState()
                RefreshLiveSummary()
                ShowFeedbackBanner(
                    $"Draft siap diteruskan: {finalizedDraft.Quantity}x {finalizedDraft.ItemNameSnapshot} ({AppTheme.FormatRupiah(finalizedDraft.LineTotal)}).",
                    isError:=False)
            Catch ex As Exception
                If IsDisposed OrElse Not IsHandleCreated Then Return
                _isSubmittingDraft = False
                LayoutSummaryPanel()
                ShowFeedbackBanner(FormatUserError(ex, "Gagal memvalidasi pilihan ke Supabase."), isError:=True)
                RefreshLiveSummary()
            End Try
        End Sub

        Private Sub OnViewDraftsClick(sender As Object, e As EventArgs)
            Using dlg As New DrinkDraftPreviewDialog()
                dlg.ShowDialog(FindForm())
            End Using
            UpdateDraftButtonState()
        End Sub

        Private Sub UpdateDraftButtonState()
            Dim count = DrinkDraftHandoff.PendingCount
            _viewDraftsBtn.Enabled = count > 0
            LayoutSummaryPanel()
        End Sub

        Private Sub ShowFeedbackBanner(message As String, isError As Boolean)
            _feedbackBanner.Text = message
            _toolTip.SetToolTip(_feedbackBanner, message)
            If isError Then
                _feedbackBanner.BackColor = AppTheme.ErrorTint
                _feedbackBanner.ForeColor = AppTheme.ErrorText
            Else
                _feedbackBanner.BackColor = AppTheme.SageTint
                _feedbackBanner.ForeColor = AppTheme.SageText
            End If
            _feedbackBanner.Visible = True
            LayoutSummaryPanel()
        End Sub

        Private Sub HideFeedbackBanner()
            If Not _feedbackBanner.Visible AndAlso String.IsNullOrEmpty(_feedbackBanner.Text) Then Return
            _feedbackBanner.Visible = False
            _feedbackBanner.Text = String.Empty
            _toolTip.SetToolTip(_feedbackBanner, String.Empty)
            LayoutSummaryPanel()
        End Sub

        Private Sub ShowStateOverlay(state As CreatorViewState, detailError As String)
            _currentOverlayState = state
            _menuListPane.Visible = False
            _builderPane.Visible = False
            _availableCountLabel.Visible = False
            _stateOverlay.Visible = True
            _stateProgress.Visible = (state = CreatorViewState.Loading)
            _stateActionBtn.Visible = True

            Select Case state
                Case CreatorViewState.Unconfigured
                    AppTheme.ApplyStatusTag(_stateBadge, "Belum Dikonfigurasi", AppTheme.WarningTint, AppTheme.WarningText)
                    _stateTitle.Text = "Koneksi Supabase Belum Diatur"
                    _stateMessage.Text = "Hubungkan aplikasi ke proyek Supabase (URL dan Publishable/Anon Key) agar daftar minuman dan opsi kustomisasi dapat dimuat."
                    _stateActionBtn.Text = "Atur Koneksi Supabase"
                    AppTheme.StylePrimaryButton(_stateActionBtn)

                Case CreatorViewState.Loading
                    AppTheme.ApplyStatusTag(_stateBadge, "Memuat Data", AppTheme.TaupeTint, AppTheme.TaupeText)
                    _stateTitle.Text = "Memuat Daftar Minuman"
                    _stateMessage.Text = "Mengambil item menu aktif yang tersedia dari Supabase..."
                    _stateActionBtn.Visible = False

                Case CreatorViewState.ErrorState
                    AppTheme.ApplyStatusTag(_stateBadge, "Gagal Memuat", AppTheme.ErrorTint, AppTheme.ErrorText)
                    _stateTitle.Text = "Daftar Minuman Tidak Dapat Dimuat"
                    _stateMessage.Text = If(String.IsNullOrWhiteSpace(detailError), "Terjadi kesalahan saat menghubungi Supabase.", detailError)
                    _stateActionBtn.Text = "Coba Lagi"
                    AppTheme.StylePrimaryButton(_stateActionBtn)

                Case CreatorViewState.EmptyDatabase
                    AppTheme.ApplyStatusTag(_stateBadge, "Menu Kosong", AppTheme.TaupeTint, AppTheme.TaupeText)
                    _stateTitle.Text = "Belum Ada Item Menu di Supabase"
                    _stateMessage.Text = "Tabel menu_items masih kosong. Tambahkan item melalui Cafe Menu Gallery atau jalankan seed menu di Supabase terlebih dahulu."
                    _stateActionBtn.Text = "Muat Ulang"
                    AppTheme.StylePrimaryButton(_stateActionBtn)

                Case CreatorViewState.AllSoldOut
                    AppTheme.ApplyStatusTag(_stateBadge, "Semua Item Habis", AppTheme.WarningTint, AppTheme.WarningText)
                    _stateTitle.Text = "Tidak Ada Menu yang Sedang Tersedia"
                    _stateMessage.Text = "Seluruh item pada menu_items saat ini berstatus habis (is_available = false). Muat ulang setelah staf memperbarui ketersediaan."
                    _stateActionBtn.Text = "Muat Ulang"
                    AppTheme.StylePrimaryButton(_stateActionBtn)

                Case CreatorViewState.EmptyFilter
                    AppTheme.ApplyStatusTag(_stateBadge, "Filter Kosong", AppTheme.TaupeTint, AppTheme.TaupeText)
                    _stateTitle.Text = "Tidak Ada Item yang Cocok"
                    _stateMessage.Text = "Tidak ada item tersedia yang cocok dengan kata kunci atau kategori yang dipilih."
                    _stateActionBtn.Text = "Reset Filter"
                    AppTheme.StyleSecondaryButton(_stateActionBtn)
            End Select

            _stateActionBtn.Width = AppTheme.MeasureButtonWidth(_stateActionBtn.Text, AppTheme.FontBodyBold, 160, 28)

            Dim flow = TryCast(_stateCard.Controls(0), FlowLayoutPanel)
            If flow IsNot Nothing Then
                _stateCard.Height = flow.PreferredSize.Height + _stateCard.Padding.Vertical + 6
            End If
            CenterStateCard()
        End Sub

        Private Sub CenterStateCard()
            Dim x = Math.Max(16, (_stateOverlay.Width - _stateCard.Width) \ 2)
            Dim y = Math.Max(16, (_stateOverlay.Height - _stateCard.Height) \ 2)
            _stateCard.Location = New Point(x, y)
        End Sub

        Private Sub OnStateActionClick(sender As Object, e As EventArgs)
            Select Case _currentOverlayState
                Case CreatorViewState.Unconfigured
                    Using dlg As New SupabaseConfigForm()
                        dlg.ShowDialog(FindForm())
                    End Using
                Case CreatorViewState.ErrorState, CreatorViewState.EmptyDatabase, CreatorViewState.AllSoldOut
                    LoadAvailableMenuAsync()
                Case CreatorViewState.EmptyFilter
                    _searchTextBox.Clear()
                    _selectedCategory = "Semua"
                    UpdateCategoryButtonStyles()
                    ApplyFiltersAndRender()
            End Select
        End Sub

        Private Sub OnSearchTextChanged(sender As Object, e As EventArgs)
            _clearSearchBtn.Enabled = _searchTextBox.TextLength > 0
            ApplyFiltersAndRender()
        End Sub

        Private Sub UpdateCategoryButtonStyles()
            For Each kvp In _categoryButtons
                If String.Equals(kvp.Key, _selectedCategory, StringComparison.OrdinalIgnoreCase) Then
                    AppTheme.StyleAccentButton(kvp.Value)
                Else
                    AppTheme.StyleSecondaryButton(kvp.Value)
                End If
            Next
        End Sub

        Private Async Sub LoadCardPhotoAsync(card As DrinkSelectorCard, imageUrl As String, loadVersion As Integer)
            Dim bytes = Await FetchImageBytesAsync(imageUrl)
            If IsDisposed OrElse Not IsHandleCreated OrElse card.IsDisposed OrElse loadVersion <> _menuLoadVersion Then
                Return
            End If

            If bytes Is Nothing OrElse bytes.Length = 0 Then Return

            Try
                Using ms As New MemoryStream(bytes)
                    Using decoded = Image.FromStream(ms)
                        card.SetPhoto(New Bitmap(decoded))
                    End Using
                End Using
            Catch
                ' Keep warm placeholder frame when image payload cannot be decoded.
            End Try
        End Sub

        Private Async Sub LoadSelectedPhotoAsync(imageUrl As String, loadVersion As Integer, expectedItemId As String)
            SetSelectedPhoto(Nothing)
            Dim bytes = Await FetchImageBytesAsync(imageUrl)
            If IsDisposed OrElse Not IsHandleCreated OrElse loadVersion <> _menuLoadVersion Then
                Return
            End If
            If Not String.Equals(_selectedItemId, expectedItemId, StringComparison.OrdinalIgnoreCase) Then
                Return
            End If
            If bytes Is Nothing OrElse bytes.Length = 0 Then Return

            Try
                Using ms As New MemoryStream(bytes)
                    Using decoded = Image.FromStream(ms)
                        SetSelectedPhoto(New Bitmap(decoded))
                    End Using
                End Using
            Catch
                SetSelectedPhoto(Nothing)
            End Try
        End Sub

        Private Shared Async Function FetchImageBytesAsync(imageUrl As String) As Task(Of Byte())
            Dim cleanUrl = If(imageUrl, String.Empty).Trim()
            If String.IsNullOrEmpty(cleanUrl) Then Return Nothing

            Dim cached As Byte() = Nothing
            If ImageBytesCache.TryGetValue(cleanUrl, cached) Then
                Return cached
            End If

            Dim parsed As Uri = Nothing
            If Not Uri.TryCreate(cleanUrl, UriKind.Absolute, parsed) OrElse
               (parsed.Scheme <> Uri.UriSchemeHttps AndAlso parsed.Scheme <> Uri.UriSchemeHttp) Then
                Return Nothing
            End If

            Try
                Dim bytes = Await ImageHttpClient.GetByteArrayAsync(parsed).ConfigureAwait(False)
                If bytes IsNot Nothing AndAlso bytes.Length > 0 Then
                    If ImageBytesCache.Count > 100 Then
                        ImageBytesCache.Clear()
                    End If
                    ImageBytesCache.TryAdd(cleanUrl, bytes)
                End If
                Return bytes
            Catch
                Return Nothing
            End Try
        End Function

        Private Sub SetSelectedPhoto(nextImage As Image)
            Dim previous = _selectedPhotoBox.Image
            _selectedPhotoBox.Image = nextImage
            If previous IsNot Nothing Then
                previous.Dispose()
            End If
        End Sub

        Private Sub OnSelectedPhotoPaint(sender As Object, e As PaintEventArgs)
            Using borderPen As New Pen(AppTheme.DividerAlmond, 1.0F)
                e.Graphics.DrawRectangle(borderPen, 0, 0, _selectedPhotoBox.Width - 1, _selectedPhotoBox.Height - 1)
            End Using

            If _selectedPhotoBox.Image Is Nothing Then
                TextRenderer.DrawText(
                    e.Graphics,
                    "Tanpa foto",
                    AppTheme.FontCaption,
                    New Rectangle(2, 2, _selectedPhotoBox.Width - 4, _selectedPhotoBox.Height - 4),
                    AppTheme.TextSecondary,
                    TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.WordBreak)
            End If
        End Sub

        Private Sub OnConfigurationChanged(sender As Object, e As EventArgs)
            If IsDisposed Then Return
            If InvokeRequired Then
                BeginInvoke(New Action(Sub() LoadAvailableMenuAsync()))
            Else
                LoadAvailableMenuAsync()
            End If
        End Sub

        Private Sub OnSessionChanged(sender As Object, e As EventArgs)
            If IsDisposed OrElse Not IsHandleCreated Then Return
            If InvokeRequired Then
                BeginInvoke(New Action(Sub() LoadAvailableMenuAsync()))
            Else
                LoadAvailableMenuAsync()
            End If
        End Sub

        Private Sub OnPendingDraftsChanged(sender As Object, e As EventArgs)
            If IsDisposed OrElse Not IsHandleCreated Then Return
            If InvokeRequired Then
                BeginInvoke(New Action(AddressOf UpdateDraftButtonState))
            Else
                UpdateDraftButtonState()
            End If
        End Sub

        Private Shared Function FormatUserError(ex As Exception, defaultMessage As String) As String
            If ex Is Nothing Then
                Return defaultMessage
            End If

            Dim raw = If(ex.Message, String.Empty)
            If raw.IndexOf("401", StringComparison.OrdinalIgnoreCase) >= 0 OrElse
               raw.IndexOf("403", StringComparison.OrdinalIgnoreCase) >= 0 OrElse
               raw.IndexOf("permission denied", StringComparison.OrdinalIgnoreCase) >= 0 OrElse
               raw.IndexOf("row-level security", StringComparison.OrdinalIgnoreCase) >= 0 Then
                Return "Akses ke data menu/opsi ditolak oleh kebijakan izin database. Periksa sesi akun atau konfigurasi RLS Supabase."
            End If

            Return AuthService.FormatException(ex, defaultMessage)
        End Function

        Private Sub CleanupRuntimeResources()
            If _cleanupDone Then Return
            _cleanupDone = True
            _menuLoadVersion += 1
            _optionsLoadVersion += 1

            If _runtimeInitialized Then
                RemoveHandler SupabaseClientProvider.ConfigurationChanged, AddressOf OnConfigurationChanged
                RemoveHandler AppSession.SessionChanged, AddressOf OnSessionChanged
                RemoveHandler DrinkDraftHandoff.PendingDraftsChanged, AddressOf OnPendingDraftsChanged
                RemoveHandler Me.Load, AddressOf OnControlLoad
                _runtimeInitialized = False
            End If

            _toolTip.Dispose()
            SetSelectedPhoto(Nothing)
        End Sub

        Private NotInheritable Class DrinkSelectorCard
            Inherits Control

            Private _photo As Bitmap
            Private _isSelected As Boolean
            Private _isHovered As Boolean

            Public ReadOnly Property Item As Models.MenuItem
            Public Event Activated As EventHandler

            Public Sub New(item As Models.MenuItem)
                Me.Item = item
                DoubleBuffered = True
                Cursor = Cursors.Hand
                TabStop = True
                SetStyle(ControlStyles.Selectable Or ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer, True)

                Dim customText = If(item.SupportsCustomization, "Bisa Dikustom", "Resep Standar")
                AccessibleName = $"{item.Name}, {item.Category}, {AppTheme.FormatRupiah(item.BasePrice)}, {customText}"
                AccessibleRole = AccessibleRole.PushButton
            End Sub

            <Browsable(False)>
            <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
            Public Property IsSelected As Boolean
                Get
                    Return _isSelected
                End Get
                Set(value As Boolean)
                    If _isSelected <> value Then
                        _isSelected = value
                        Invalidate()
                    End If
                End Set
            End Property

            Public Sub SetPhoto(photo As Bitmap)
                Dim old = _photo
                _photo = photo
                If old IsNot Nothing Then
                    old.Dispose()
                End If
                Invalidate()
            End Sub

            Protected Overrides Sub OnMouseEnter(e As EventArgs)
                _isHovered = True
                Invalidate()
                MyBase.OnMouseEnter(e)
            End Sub

            Protected Overrides Sub OnMouseLeave(e As EventArgs)
                _isHovered = False
                Invalidate()
                MyBase.OnMouseLeave(e)
            End Sub

            Protected Overrides Sub OnClick(e As EventArgs)
                Focus()
                RaiseEvent Activated(Me, EventArgs.Empty)
                MyBase.OnClick(e)
            End Sub

            Protected Overrides Sub OnGotFocus(e As EventArgs)
                Invalidate()
                MyBase.OnGotFocus(e)
            End Sub

            Protected Overrides Sub OnLostFocus(e As EventArgs)
                Invalidate()
                MyBase.OnLostFocus(e)
            End Sub

            Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
                If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Space Then
                    e.Handled = True
                    RaiseEvent Activated(Me, EventArgs.Empty)
                End If
                MyBase.OnKeyDown(e)
            End Sub

            Protected Overrides Sub OnPaint(e As PaintEventArgs)
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic

                Dim cardBounds As New Rectangle(0, 0, Width - 1, Height - 1)
                Dim bgColor = AppTheme.SurfaceMilk
                If _isSelected Then
                    bgColor = ColorTranslator.FromHtml("#FAF2E8")
                ElseIf _isHovered Then
                    bgColor = ColorTranslator.FromHtml("#FCF7F0")
                End If

                Using cardPath = AppTheme.CreateRoundedRectPath(cardBounds, 8)
                    Using bgBrush As New SolidBrush(bgColor)
                        e.Graphics.FillPath(bgBrush, cardPath)
                    End Using

                    Dim borderColor = AppTheme.ControlBorder
                    Dim borderWidth = 1.0F
                    If Focused OrElse _isSelected Then
                        borderColor = AppTheme.FocusMocha
                        borderWidth = 2.0F
                    End If

                    Using borderPen As New Pen(borderColor, borderWidth)
                        e.Graphics.DrawPath(borderPen, cardPath)
                    End Using
                End Using

                Dim photoSize = If(Width < 268, 62, 72)
                Dim photoTop = (Height - photoSize) \ 2
                Dim photoRect As New Rectangle(10, photoTop, photoSize, photoSize)
                Using photoPath = AppTheme.CreateRoundedRectPath(photoRect, 6)
                    Using frameBrush As New SolidBrush(AppTheme.SidebarOat)
                        e.Graphics.FillPath(frameBrush, photoPath)
                    End Using

                    If _photo IsNot Nothing Then
                        Dim state = e.Graphics.Save()
                        Try
                            e.Graphics.SetClip(photoPath)
                            Dim drawRect = FitInside(_photo.Size, photoRect)
                            e.Graphics.DrawImage(_photo, drawRect)
                        Finally
                            e.Graphics.Restore(state)
                        End Try
                    Else
                        TextRenderer.DrawText(
                            e.Graphics,
                            "Tanpa foto",
                            AppTheme.FontCaption,
                            photoRect,
                            AppTheme.TextSecondary,
                            TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.WordBreak)
                    End If

                    Using framePen As New Pen(AppTheme.DividerAlmond, 1.0F)
                        e.Graphics.DrawPath(framePen, photoPath)
                    End Using
                End Using

                Dim textX = photoRect.Right + 12
                Dim textWidth = Math.Max(80, Width - textX - 10)

                Dim nameRect As New Rectangle(textX, 10, textWidth, 22)
                TextRenderer.DrawText(
                    e.Graphics,
                    Item.Name,
                    AppTheme.FontCardTitle,
                    nameRect,
                    AppTheme.TextPrimary,
                    TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)

                Dim catRect As New Rectangle(textX, 34, textWidth, 18)
                TextRenderer.DrawText(
                    e.Graphics,
                    Item.Category,
                    AppTheme.FontCaptionBold,
                    catRect,
                    AppTheme.TextSecondary,
                    TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)

                Dim customText = If(Item.SupportsCustomization, "Bisa Dikustom", "Resep Standar")
                Dim compactCustomText = If(Item.SupportsCustomization, "Kustom", "Standar")
                Dim customBg = If(Item.SupportsCustomization, AppTheme.AccentStrawberryTint, AppTheme.TaupeTint)
                Dim customFg = If(Item.SupportsCustomization, AppTheme.AccentStrawberryText, AppTheme.TaupeText)
                Dim priceStr = AppTheme.FormatRupiah(Item.BasePrice)
                Dim minPriceWidth = TextRenderer.MeasureText(priceStr, AppTheme.FontBodyBold, Size.Empty, TextFormatFlags.SingleLine).Width + 6

                Dim badgeWidth = TextRenderer.MeasureText(customText, AppTheme.FontCaptionBold, Size.Empty, TextFormatFlags.SingleLine).Width + 10
                Dim displayBadgeText = customText
                If textX + minPriceWidth + 6 + badgeWidth > Width - 10 Then
                    displayBadgeText = compactCustomText
                    badgeWidth = TextRenderer.MeasureText(displayBadgeText, AppTheme.FontCaptionBold, Size.Empty, TextFormatFlags.SingleLine).Width + 10
                End If

                Dim badgeX = Math.Max(textX + minPriceWidth + 4, Width - 10 - badgeWidth)
                Dim priceWidth = Math.Max(66, badgeX - textX - 4)

                Dim priceRect As New Rectangle(textX, 58, priceWidth, 22)
                TextRenderer.DrawText(
                    e.Graphics,
                    priceStr,
                    AppTheme.FontBodyBold,
                    priceRect,
                    AppTheme.PrimaryMocha,
                    TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)

                Dim badgeRect As New Rectangle(badgeX, 59, Math.Min(badgeWidth, Width - badgeX - 10), 20)
                If badgeRect.Width >= 42 Then
                    Using badgePath = AppTheme.CreateRoundedRectPath(badgeRect, 4)
                        Using badgeBrush As New SolidBrush(customBg)
                            e.Graphics.FillPath(badgeBrush, badgePath)
                        End Using
                    End Using
                    TextRenderer.DrawText(
                        e.Graphics,
                        displayBadgeText,
                        AppTheme.FontCaptionBold,
                        badgeRect,
                        customFg,
                        TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
                End If
            End Sub

            Private Shared Function FitInside(sourceSize As Size, target As Rectangle) As Rectangle
                If sourceSize.Width <= 0 OrElse sourceSize.Height <= 0 Then Return target
                Dim scale = Math.Min(CDbl(target.Width) / sourceSize.Width, CDbl(target.Height) / sourceSize.Height)
                Dim drawW = Math.Max(1, CInt(Math.Round(sourceSize.Width * scale)))
                Dim drawH = Math.Max(1, CInt(Math.Round(sourceSize.Height * scale)))
                Dim drawX = target.X + (target.Width - drawW) \ 2
                Dim drawY = target.Y + (target.Height - drawH) \ 2
                Return New Rectangle(drawX, drawY, drawW, drawH)
            End Function

            Protected Overrides Sub Dispose(disposing As Boolean)
                If disposing AndAlso _photo IsNot Nothing Then
                    _photo.Dispose()
                    _photo = Nothing
                End If
                MyBase.Dispose(disposing)
            End Sub
        End Class

        Private NotInheritable Class DrinkDraftPreviewDialog
            Inherits Form

            Private ReadOnly _draftListBox As New ListBox()
            Private ReadOnly _jsonPreviewBox As New TextBox()
            Private ReadOnly _totalLabel As New Label()
            Private ReadOnly _copyStatusLabel As New Label()
            Private ReadOnly _removeBtn As New Button()
            Private ReadOnly _clearAllBtn As New Button()
            Private ReadOnly _copyJsonBtn As New Button()
            Private ReadOnly _closeBtn As New Button()
            Private ReadOnly _monoFont As New Font("Consolas", 8.5F, FontStyle.Regular, GraphicsUnit.Point)
            Private _drafts As New List(Of ConfiguredDrinkDraft)()

            Public Sub New()
                Text = "Draft Pesanan Siap Diteruskan"
                FormBorderStyle = FormBorderStyle.FixedDialog
                StartPosition = FormStartPosition.CenterParent
                MaximizeBox = False
                MinimizeBox = False
                ClientSize = New Size(660, 480)
                BackColor = AppTheme.CanvasCream
                Font = AppTheme.FontBody
                ForeColor = AppTheme.TextPrimary
                CancelButton = _closeBtn

                Dim headerLbl As New Label With {
                    .AutoSize = True,
                    .Font = AppTheme.FontSectionTitle,
                    .ForeColor = AppTheme.TextPrimary,
                    .Location = New Point(20, 16),
                    .TabIndex = 0,
                    .Text = "Daftar Draft Minuman (Handoff ke Alur Pesanan)"
                }

                Dim infoLbl As New Label With {
                    .AutoSize = False,
                    .Font = AppTheme.FontCaption,
                    .ForeColor = AppTheme.TextSecondary,
                    .Location = New Point(20, 40),
                    .Size = New Size(620, 32),
                    .TabIndex = 1,
                    .Text = "Draft ini disimpan sementara di memori untuk diserahkan ke Friends Order Room atau alur pesanan. Belum menulis record orders atau order_items ke database."
                }

                _draftListBox.Location = New Point(20, 78)
                _draftListBox.Size = New Size(280, 294)
                _draftListBox.BackColor = AppTheme.SurfaceMilk
                _draftListBox.ForeColor = AppTheme.TextPrimary
                _draftListBox.Font = AppTheme.FontBody
                _draftListBox.BorderStyle = BorderStyle.FixedSingle
                _draftListBox.AccessibleName = "Daftar draft minuman"
                _draftListBox.TabIndex = 2
                AddHandler _draftListBox.SelectedIndexChanged, Sub() UpdateSelectedJsonPreview()

                Dim jsonLbl As New Label With {
                    .AutoSize = True,
                    .Font = AppTheme.FontCaptionBold,
                    .ForeColor = AppTheme.TextSecondary,
                    .Location = New Point(314, 78),
                    .TabIndex = 3,
                    .Text = "Snapshot JSON (siap dipetakan ke order_items)"
                }

                _jsonPreviewBox.Location = New Point(314, 98)
                _jsonPreviewBox.Size = New Size(326, 274)
                _jsonPreviewBox.Multiline = True
                _jsonPreviewBox.ReadOnly = True
                _jsonPreviewBox.ScrollBars = ScrollBars.Vertical
                _jsonPreviewBox.BackColor = AppTheme.SurfaceMilk
                _jsonPreviewBox.ForeColor = AppTheme.TextPrimary
                _jsonPreviewBox.Font = _monoFont
                _jsonPreviewBox.BorderStyle = BorderStyle.FixedSingle
                _jsonPreviewBox.AccessibleName = "Preview JSON draft terpilih"
                _jsonPreviewBox.TabIndex = 4

                _totalLabel.AutoSize = True
                _totalLabel.Font = AppTheme.FontBodyBold
                _totalLabel.ForeColor = AppTheme.PrimaryMocha
                _totalLabel.Location = New Point(20, 382)
                _totalLabel.TabIndex = 5

                _copyStatusLabel.AutoSize = True
                _copyStatusLabel.Font = AppTheme.FontCaptionBold
                _copyStatusLabel.ForeColor = AppTheme.SageText
                _copyStatusLabel.Location = New Point(20, 404)
                _copyStatusLabel.Visible = False
                _copyStatusLabel.TabIndex = 6

                _removeBtn.Text = "Hapus Terpilih"
                _removeBtn.Size = New Size(130, 34)
                _removeBtn.Location = New Point(20, 428)
                _removeBtn.TabIndex = 7
                AppTheme.StyleSecondaryButton(_removeBtn)
                AttachButtonFocusIndicator(_removeBtn)
                AttachButtonEnabledStyle(_removeBtn, isPrimary:=False)
                AddHandler _removeBtn.Click, AddressOf OnRemoveSelectedClick

                _clearAllBtn.Text = "Bersihkan Semua"
                _clearAllBtn.Size = New Size(142, 34)
                _clearAllBtn.Location = New Point(158, 428)
                _clearAllBtn.TabIndex = 8
                AppTheme.StyleDangerButton(_clearAllBtn)
                AttachButtonFocusIndicator(_clearAllBtn)
                AddHandler _clearAllBtn.Click, AddressOf OnClearAllClick

                _copyJsonBtn.Text = "Salin JSON"
                _copyJsonBtn.Size = New Size(112, 34)
                _copyJsonBtn.Location = New Point(410, 428)
                _copyJsonBtn.TabIndex = 9
                AppTheme.StyleSecondaryButton(_copyJsonBtn)
                AttachButtonFocusIndicator(_copyJsonBtn)
                AttachButtonEnabledStyle(_copyJsonBtn, isPrimary:=False)
                AddHandler _copyJsonBtn.Click, AddressOf OnCopyJsonClick

                _closeBtn.Text = "Tutup"
                _closeBtn.Size = New Size(110, 34)
                _closeBtn.Location = New Point(530, 428)
                _closeBtn.DialogResult = DialogResult.Cancel
                _closeBtn.TabIndex = 10
                AppTheme.StylePrimaryButton(_closeBtn)
                AttachButtonFocusIndicator(_closeBtn)

                Controls.AddRange({
                    headerLbl,
                    infoLbl,
                    _draftListBox,
                    jsonLbl,
                    _jsonPreviewBox,
                    _totalLabel,
                    _copyStatusLabel,
                    _removeBtn,
                    _clearAllBtn,
                    _copyJsonBtn,
                    _closeBtn
                })

                ReloadDrafts()
            End Sub

            Private Sub ReloadDrafts()
                _drafts = DrinkDraftHandoff.GetPendingDrafts()
                _draftListBox.Items.Clear()
                _copyStatusLabel.Visible = False

                Dim grandTotal As Decimal = 0D
                For i = 0 To _drafts.Count - 1
                    Dim d = _drafts(i)
                    grandTotal += d.LineTotal
                    _draftListBox.Items.Add($"{i + 1}. {d.Quantity}x {d.ItemNameSnapshot} ({AppTheme.FormatRupiah(d.LineTotal)})")
                Next

                _totalLabel.Text = $"Total draft siap kirim: {_drafts.Count} item | Estimasi subtotal: {AppTheme.FormatRupiah(grandTotal)}"
                _clearAllBtn.Enabled = _drafts.Count > 0

                If _draftListBox.Items.Count > 0 Then
                    _draftListBox.SelectedIndex = _draftListBox.Items.Count - 1
                Else
                    UpdateSelectedJsonPreview()
                End If
            End Sub

            Private Sub UpdateSelectedJsonPreview()
                _copyStatusLabel.Visible = False
                Dim idx = _draftListBox.SelectedIndex
                If idx < 0 OrElse idx >= _drafts.Count Then
                    _jsonPreviewBox.Text = "Belum ada draft tersimpan."
                    _removeBtn.Enabled = False
                    _copyJsonBtn.Enabled = False
                    Return
                End If

                _removeBtn.Enabled = True
                _copyJsonBtn.Enabled = True
                _jsonPreviewBox.Text = _drafts(idx).ToDraftJson(indented:=True)
            End Sub

            Private Sub OnCopyJsonClick(sender As Object, e As EventArgs)
                Dim idx = _draftListBox.SelectedIndex
                If idx < 0 OrElse idx >= _drafts.Count Then Return
                Try
                    Clipboard.SetText(_drafts(idx).ToDraftJson(indented:=True))
                    _copyStatusLabel.ForeColor = AppTheme.SageText
                    _copyStatusLabel.Text = "JSON draft berhasil disalin ke clipboard."
                    _copyStatusLabel.Visible = True
                Catch
                    _copyStatusLabel.ForeColor = AppTheme.ErrorText
                    _copyStatusLabel.Text = "Clipboard sedang sibuk. Coba salin ulang."
                    _copyStatusLabel.Visible = True
                End Try
            End Sub

            Private Sub OnRemoveSelectedClick(sender As Object, e As EventArgs)
                Dim idx = _draftListBox.SelectedIndex
                If idx >= 0 AndAlso DrinkDraftHandoff.RemovePendingAt(idx) Then
                    ReloadDrafts()
                End If
            End Sub

            Private Sub OnClearAllClick(sender As Object, e As EventArgs)
                DrinkDraftHandoff.ClearPending()
                ReloadDrafts()
            End Sub

            Protected Overrides Sub Dispose(disposing As Boolean)
                If disposing Then
                    _monoFont.Dispose()
                End If
                MyBase.Dispose(disposing)
            End Sub
        End Class
    End Class
End Namespace
