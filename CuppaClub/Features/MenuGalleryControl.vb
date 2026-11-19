Option Strict On
Option Explicit On

Imports System.Collections.Concurrent
Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Net.Http
Imports CuppaClub.Core
Imports CuppaClub.Forms
Imports CuppaClub.Models
Imports CuppaClub.Services

Namespace Features
    Public Class MenuGalleryControl
        Inherits UserControl

        Private Enum GalleryViewState
            Unconfigured
            Loading
            ErrorState
            EmptyDatabase
            EmptyFilter
            Ready
        End Enum

        Private Shared ReadOnly ImageHttpClient As New HttpClient With {
            .Timeout = TimeSpan.FromSeconds(8),
            .MaxResponseContentBufferSize = 4L * 1024L * 1024L
        }
        Private Shared ReadOnly ImageBytesCache As New ConcurrentDictionary(Of String, Byte())(StringComparer.OrdinalIgnoreCase)

        Private ReadOnly _menuService As New MenuService()
        Private ReadOnly _categoryButtons As New Dictionary(Of String, Button)(StringComparer.OrdinalIgnoreCase)

        Private _allItems As New List(Of Models.MenuItem)()
        Private _filteredItems As New List(Of Models.MenuItem)()
        Private _selectedCategory As String = "Semua"
        Private _selectedItemId As String = String.Empty
        Private _loadVersion As Integer

        Private ReadOnly _toolbarPanel As New Panel()
        Private ReadOnly _searchLabel As New Label()
        Private ReadOnly _categoryLabel As New Label()
        Private ReadOnly _searchTextBox As New TextBox()
        Private ReadOnly _clearSearchBtn As New Button()
        Private ReadOnly _showUnavailableCheck As New CheckBox()
        Private ReadOnly _reloadBtn As New Button()
        Private ReadOnly _addMenuBtn As New Button()
        Private ReadOnly _resultCountLabel As New Label()

        Private ReadOnly _workspaceHost As New Panel()
        Private ReadOnly _catalogPane As New Panel()
        Private ReadOnly _countStripPanel As New Panel()
        Private ReadOnly _cardsPanel As New BufferedFlowLayoutPanel()
        Private ReadOnly _detailPanel As New Panel()
        Private ReadOnly _detailPhotoBox As New PictureBox()
        Private ReadOnly _detailCategoryLabel As New Label()
        Private ReadOnly _detailAvailabilityTag As New Label()
        Private ReadOnly _detailTitleLabel As New Label()
        Private ReadOnly _detailPriceLabel As New Label()
        Private ReadOnly _detailDescriptionLabel As New Label()
        Private ReadOnly _detailCustomNoteLabel As New Label()
        Private ReadOnly _staffActionsPanel As New Panel()
        Private ReadOnly _editMenuBtn As New Button()
        Private ReadOnly _toggleAvailabilityBtn As New Button()
        Private ReadOnly _detailStatusBanner As New Label()

        Private ReadOnly _stateOverlay As New Panel()
        Private ReadOnly _stateCard As New Panel()
        Private ReadOnly _stateBadge As New Label()
        Private ReadOnly _stateTitle As New Label()
        Private ReadOnly _stateMessage As New Label()
        Private ReadOnly _stateProgress As New ProgressBar()
        Private ReadOnly _stateActionBtn As New Button()
        Private _currentOverlayState As GalleryViewState = GalleryViewState.Loading

        Public Sub New()
            DoubleBuffered = True
            BackColor = AppTheme.CanvasCream
            ForeColor = AppTheme.TextPrimary
            Font = AppTheme.FontBody

            BuildUi()

            AddHandler AppSession.SessionChanged, AddressOf OnSessionChanged
            AddHandler SupabaseClientProvider.ConfigurationChanged, AddressOf OnConfigurationChanged

            UpdateStaffControlsVisibility()
            LoadMenuAsync()
        End Sub

        Private Sub BuildUi()
            BuildToolbar()
            BuildWorkspace()
            BuildStateOverlay()

            Controls.Add(_workspaceHost)
            Controls.Add(_toolbarPanel)
        End Sub

        Private Sub BuildToolbar()
            _toolbarPanel.Dock = DockStyle.Top
            _toolbarPanel.Height = 104
            _toolbarPanel.BackColor = AppTheme.SurfaceMilk
            AddHandler _toolbarPanel.Paint, Sub(s, e)
                                                Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                                    e.Graphics.DrawLine(pen, 0, _toolbarPanel.Height - 1, _toolbarPanel.Width, _toolbarPanel.Height - 1)
                                                End Using
                                            End Sub
            AddHandler _toolbarPanel.Resize, Sub() LayoutToolbar()

            _searchLabel.Text = "Cari menu"
            _searchLabel.Font = AppTheme.FontBodyBold
            _searchLabel.ForeColor = AppTheme.TextPrimary
            _searchLabel.Location = New Point(20, 16)
            _searchLabel.AutoSize = True

            _categoryLabel.Text = "Kategori"
            _categoryLabel.Font = AppTheme.FontBodyBold
            _categoryLabel.ForeColor = AppTheme.TextPrimary
            _categoryLabel.Location = New Point(20, 61)
            _categoryLabel.AutoSize = True

            _searchTextBox.Height = 28
            _searchTextBox.BackColor = AppTheme.CanvasCream
            _searchTextBox.ForeColor = AppTheme.TextPrimary
            _searchTextBox.PlaceholderText = "Nama kopi, pastry, atau bahan..."
            _searchTextBox.TabIndex = 0
            AddHandler _searchTextBox.TextChanged, AddressOf OnSearchTextChanged

            _clearSearchBtn.Text = "Bersihkan"
            _clearSearchBtn.Height = 30
            _clearSearchBtn.Width = AppTheme.MeasureButtonWidth(_clearSearchBtn.Text, AppTheme.FontBodyBold, 98, 28)
            _clearSearchBtn.Enabled = False
            _clearSearchBtn.TabIndex = 1
            AppTheme.StyleSecondaryButton(_clearSearchBtn)
            AddHandler _clearSearchBtn.Click, Sub()
                                                  _searchTextBox.Clear()
                                                  _searchTextBox.Focus()
                                              End Sub

            _showUnavailableCheck.Text = "Tampilkan habis"
            _showUnavailableCheck.AutoSize = True
            _showUnavailableCheck.Cursor = Cursors.Hand
            _showUnavailableCheck.TabIndex = 7
            AddHandler _showUnavailableCheck.CheckedChanged, Sub() ApplyFiltersAndRender()

            _addMenuBtn.Text = "+ Tambah Menu"
            _addMenuBtn.Height = 32
            _addMenuBtn.Width = AppTheme.MeasureButtonWidth(_addMenuBtn.Text, AppTheme.FontBodyBold, 142, 28)
            _addMenuBtn.TabIndex = 2
            AppTheme.StylePrimaryButton(_addMenuBtn)
            AddHandler _addMenuBtn.Click, AddressOf OnAddMenuClick

            _reloadBtn.Text = "Muat Ulang"
            _reloadBtn.Height = 32
            _reloadBtn.Width = AppTheme.MeasureButtonWidth(_reloadBtn.Text, AppTheme.FontBodyBold, 114, 28)
            _reloadBtn.TabIndex = 3
            AppTheme.StyleSecondaryButton(_reloadBtn)
            AddHandler _reloadBtn.Click, Sub() LoadMenuAsync()

            Dim categoryNames As New List(Of String) From {"Semua"}
            categoryNames.AddRange(Models.MenuItem.Categories)

            Dim catTabIdx = 4
            For Each catName In categoryNames
                Dim btn As New Button With {
                    .Text = catName,
                    .Height = 32,
                    .Width = AppTheme.MeasureButtonWidth(catName, AppTheme.FontBodyBold, 72, 24),
                    .TabIndex = catTabIdx
                }
                catTabIdx += 1
                Dim captured = catName
                AddHandler btn.Click, Sub()
                                          _selectedCategory = captured
                                          UpdateCategoryButtonStyles()
                                          ApplyFiltersAndRender()
                                      End Sub
                _categoryButtons(catName) = btn
                _toolbarPanel.Controls.Add(btn)
            Next

            _toolbarPanel.Controls.AddRange({
                _searchLabel,
                _searchTextBox,
                _clearSearchBtn,
                _showUnavailableCheck,
                _addMenuBtn,
                _reloadBtn,
                _categoryLabel
            })

            UpdateCategoryButtonStyles()
            LayoutToolbar()
        End Sub

        Private Sub LayoutToolbar()
            Dim w = _toolbarPanel.ClientSize.Width
            If w <= 0 Then Return

            Dim leftColumnX = 20 + Math.Max(_searchLabel.PreferredWidth, _categoryLabel.PreferredWidth) + 12

            _reloadBtn.Location = New Point(w - _reloadBtn.Width - 20, 11)
            Dim row1RightEdge = _reloadBtn.Left - 8
            If _addMenuBtn.Visible Then
                _addMenuBtn.Location = New Point(_reloadBtn.Left - _addMenuBtn.Width - 8, 11)
                row1RightEdge = _addMenuBtn.Left - 12
            End If

            Dim searchWidth = Math.Max(140, Math.Min(340, row1RightEdge - leftColumnX - _clearSearchBtn.Width - 16))
            _searchTextBox.Location = New Point(leftColumnX, 13)
            _searchTextBox.Width = searchWidth
            _clearSearchBtn.Location = New Point(_searchTextBox.Right + 8, 12)

            Dim xPos = leftColumnX
            For Each catName In _categoryButtons.Keys
                Dim btn = _categoryButtons(catName)
                btn.Location = New Point(xPos, 56)
                xPos += btn.Width + 6
            Next

            Dim checkWidth = _showUnavailableCheck.PreferredSize.Width
            _showUnavailableCheck.Location = New Point(Math.Max(xPos + 8, w - checkWidth - 20), 61)
        End Sub

        Private Sub BuildWorkspace()
            _workspaceHost.Dock = DockStyle.Fill
            _workspaceHost.BackColor = AppTheme.CanvasCream

            _catalogPane.Dock = DockStyle.Fill
            _catalogPane.BackColor = AppTheme.CanvasCream

            _countStripPanel.Dock = DockStyle.Top
            _countStripPanel.Height = 30
            _countStripPanel.BackColor = AppTheme.CanvasCream
            _countStripPanel.Padding = New Padding(20, 8, 12, 0)
            _countStripPanel.Visible = False

            _resultCountLabel.Location = New Point(20, 8)
            _resultCountLabel.AutoSize = True
            _resultCountLabel.Font = AppTheme.FontCaptionBold
            _resultCountLabel.ForeColor = AppTheme.TextSecondary
            _countStripPanel.Controls.Add(_resultCountLabel)

            _cardsPanel.Dock = DockStyle.Fill
            _cardsPanel.BackColor = AppTheme.CanvasCream
            _cardsPanel.AutoScroll = True
            _cardsPanel.WrapContents = True
            _cardsPanel.FlowDirection = FlowDirection.LeftToRight
            _cardsPanel.Padding = New Padding(20, 10, 10, 20)

            _catalogPane.Controls.Add(_cardsPanel)
            _catalogPane.Controls.Add(_countStripPanel)

            _detailPanel.Dock = DockStyle.Right
            _detailPanel.Width = 356
            _detailPanel.BackColor = AppTheme.SurfaceMilk
            _detailPanel.AutoScroll = True
            _detailPanel.Padding = New Padding(20)
            AddHandler _detailPanel.Paint, Sub(s, e)
                                               Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                                   e.Graphics.DrawLine(pen, 0, 0, 0, _detailPanel.Height)
                                               End Using
                                           End Sub

            Dim detailLayout As New FlowLayoutPanel With {
                .Dock = DockStyle.Top,
                .FlowDirection = FlowDirection.TopDown,
                .WrapContents = False,
                .AutoSize = True,
                .Width = 296,
                .BackColor = Color.Transparent
            }

            _detailPhotoBox.Size = New Size(296, 182)
            _detailPhotoBox.SizeMode = PictureBoxSizeMode.Zoom
            _detailPhotoBox.BackColor = AppTheme.SidebarOat
            _detailPhotoBox.Margin = New Padding(0, 0, 0, 12)
            AddHandler _detailPhotoBox.Paint, AddressOf OnDetailPhotoPaint

            Dim metaRow As New Panel With {
                .Width = 296,
                .Height = 28,
                .Margin = New Padding(0, 0, 0, 6)
            }

            _detailCategoryLabel.Location = New Point(0, 4)
            _detailCategoryLabel.Font = AppTheme.FontCaptionBold
            _detailCategoryLabel.ForeColor = AppTheme.TextSecondary
            _detailCategoryLabel.AutoSize = True

            _detailAvailabilityTag.Location = New Point(176, 2)
            _detailAvailabilityTag.AutoSize = True

            metaRow.Controls.Add(_detailCategoryLabel)
            metaRow.Controls.Add(_detailAvailabilityTag)

            _detailTitleLabel.Font = AppTheme.FontSectionTitle
            _detailTitleLabel.ForeColor = AppTheme.TextPrimary
            _detailTitleLabel.MaximumSize = New Size(296, 0)
            _detailTitleLabel.AutoSize = True
            _detailTitleLabel.Margin = New Padding(0, 0, 0, 4)

            _detailPriceLabel.Font = New Font("Segoe UI", 13.0F, FontStyle.Bold, GraphicsUnit.Point)
            _detailPriceLabel.ForeColor = AppTheme.PrimaryMocha
            _detailPriceLabel.AutoSize = True
            _detailPriceLabel.Margin = New Padding(0, 0, 0, 12)

            _detailDescriptionLabel.Font = AppTheme.FontBody
            _detailDescriptionLabel.ForeColor = AppTheme.TextSecondary
            _detailDescriptionLabel.MaximumSize = New Size(296, 0)
            _detailDescriptionLabel.AutoSize = True
            _detailDescriptionLabel.Margin = New Padding(0, 0, 0, 14)

            _detailCustomNoteLabel.Font = AppTheme.FontCaption
            _detailCustomNoteLabel.MaximumSize = New Size(296, 0)
            _detailCustomNoteLabel.AutoSize = True
            _detailCustomNoteLabel.Padding = New Padding(10, 8, 10, 8)
            _detailCustomNoteLabel.Margin = New Padding(0, 0, 0, 14)

            _staffActionsPanel.Width = 296
            _staffActionsPanel.Height = 46
            _staffActionsPanel.Margin = New Padding(0, 0, 0, 8)

            _editMenuBtn.Text = "Edit Menu"
            _editMenuBtn.Location = New Point(0, 4)
            _editMenuBtn.Size = New Size(AppTheme.MeasureButtonWidth(_editMenuBtn.Text, AppTheme.FontBodyBold, 114, 24), 34)
            AppTheme.StylePrimaryButton(_editMenuBtn)
            AddHandler _editMenuBtn.Click, AddressOf OnEditSelectedClick

            _toggleAvailabilityBtn.Text = "Tandai Habis"
            _toggleAvailabilityBtn.Location = New Point(_editMenuBtn.Right + 8, 4)
            _toggleAvailabilityBtn.Size = New Size(296 - _toggleAvailabilityBtn.Left, 34)
            AppTheme.StyleSecondaryButton(_toggleAvailabilityBtn)
            AddHandler _toggleAvailabilityBtn.Click, AddressOf OnToggleAvailabilityClick

            _staffActionsPanel.Controls.Add(_editMenuBtn)
            _staffActionsPanel.Controls.Add(_toggleAvailabilityBtn)

            _detailStatusBanner.Font = AppTheme.FontCaptionBold
            _detailStatusBanner.MaximumSize = New Size(296, 0)
            _detailStatusBanner.AutoSize = True
            _detailStatusBanner.Padding = New Padding(10, 6, 10, 6)
            _detailStatusBanner.Visible = False

            detailLayout.Controls.AddRange({
                _detailPhotoBox,
                metaRow,
                _detailTitleLabel,
                _detailPriceLabel,
                _detailDescriptionLabel,
                _detailCustomNoteLabel,
                _staffActionsPanel,
                _detailStatusBanner
            })

            _detailPanel.Controls.Add(detailLayout)

            _workspaceHost.Controls.Add(_catalogPane)
            _workspaceHost.Controls.Add(_detailPanel)
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

            _stateActionBtn.Size = New Size(190, 36)
            AppTheme.StylePrimaryButton(_stateActionBtn)
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

        Public Async Sub LoadMenuAsync()
            _loadVersion += 1
            Dim activeVersion = _loadVersion

            If Not SupabaseClientProvider.IsConfigured Then
                _allItems = New List(Of Models.MenuItem)()
                _filteredItems = New List(Of Models.MenuItem)()
                ShowStateOverlay(GalleryViewState.Unconfigured, String.Empty)
                Return
            End If

            ShowStateOverlay(GalleryViewState.Loading, String.Empty)
            _reloadBtn.Enabled = False

            Try
                Dim loaded = Await _menuService.GetMenuItemsAsync()
                If IsDisposed OrElse Not IsHandleCreated OrElse activeVersion <> _loadVersion Then
                    Return
                End If

                _allItems = loaded
                _reloadBtn.Enabled = True
                ApplyFiltersAndRender()
            Catch ex As SupabaseNotConfiguredException
                If IsDisposed OrElse Not IsHandleCreated OrElse activeVersion <> _loadVersion Then
                    Return
                End If
                _reloadBtn.Enabled = True
                ShowStateOverlay(GalleryViewState.Unconfigured, ex.Message)
            Catch ex As Exception
                If IsDisposed OrElse Not IsHandleCreated OrElse activeVersion <> _loadVersion Then
                    Return
                End If
                _reloadBtn.Enabled = True
                ShowStateOverlay(GalleryViewState.ErrorState, AuthService.FormatException(ex, "Tidak dapat memuat daftar menu."))
            End Try
        End Sub

        Private Sub ApplyFiltersAndRender()
            If Not SupabaseClientProvider.IsConfigured Then
                ShowStateOverlay(GalleryViewState.Unconfigured, String.Empty)
                Return
            End If

            If _allItems.Count = 0 Then
                _filteredItems = New List(Of Models.MenuItem)()
                _countStripPanel.Visible = False
                ShowStateOverlay(GalleryViewState.EmptyDatabase, String.Empty)
                Return
            End If

            _filteredItems = MenuService.FilterMenuItems(
                _allItems,
                _searchTextBox.Text,
                _selectedCategory,
                _showUnavailableCheck.Checked)

            If _filteredItems.Count = 0 Then
                _countStripPanel.Visible = False
                ShowStateOverlay(GalleryViewState.EmptyFilter, String.Empty)
                Return
            End If

            _stateOverlay.Visible = False
            _catalogPane.Visible = True
            _detailPanel.Visible = True
            _resultCountLabel.Text = $"Menampilkan {_filteredItems.Count} dari {_allItems.Count} menu"
            _countStripPanel.Visible = True

            RenderCards()
        End Sub

        Private Sub RenderCards()
            Dim selected = _filteredItems.FirstOrDefault(Function(x) String.Equals(x.Id, _selectedItemId, StringComparison.Ordinal))
            If selected Is Nothing AndAlso _filteredItems.Count > 0 Then
                selected = _filteredItems(0)
                _selectedItemId = If(selected.Id, String.Empty)
            End If

            _cardsPanel.SuspendLayout()
            Try
                For Each ctrl As Control In _cardsPanel.Controls
                    ctrl.Dispose()
                Next
                _cardsPanel.Controls.Clear()

                Dim tabIdx = 20
                For Each item In _filteredItems
                    Dim card As New MenuCardControl(item) With {
                        .Size = New Size(244, 280),
                        .Margin = New Padding(0, 0, 14, 14),
                        .TabIndex = tabIdx,
                        .IsSelected = String.Equals(item.Id, _selectedItemId, StringComparison.Ordinal)
                    }
                    tabIdx += 1
                    Dim captured = item
                    AddHandler card.Activated, Sub() SelectMenuItem(captured)
                    _cardsPanel.Controls.Add(card)
                    LoadCardPhotoAsync(card, item.ImageUrl, _loadVersion)
                Next
            Finally
                _cardsPanel.ResumeLayout(True)
            End Try

            UpdateDetailPanel(selected)
        End Sub

        Private Sub SelectMenuItem(item As Models.MenuItem)
            If item Is Nothing Then Return
            _selectedItemId = If(item.Id, String.Empty)
            _detailStatusBanner.Visible = False

            For Each ctrl As Control In _cardsPanel.Controls
                Dim card = TryCast(ctrl, MenuCardControl)
                If card IsNot Nothing Then
                    card.IsSelected = String.Equals(card.Item.Id, _selectedItemId, StringComparison.Ordinal)
                End If
            Next

            UpdateDetailPanel(item)
        End Sub

        Private Sub UpdateDetailPanel(item As Models.MenuItem)
            If item Is Nothing Then
                _detailCategoryLabel.Text = String.Empty
                _detailAvailabilityTag.Visible = False
                _detailTitleLabel.Text = "Pilih item menu"
                _detailPriceLabel.Text = String.Empty
                _detailDescriptionLabel.Text = "Klik salah satu kartu menu di sebelah kiri untuk melihat detail."
                _detailCustomNoteLabel.Visible = False
                _staffActionsPanel.Visible = False
                SetDetailPhoto(Nothing)
                Return
            End If

            _detailCategoryLabel.Text = item.Category
            _detailAvailabilityTag.Visible = True
            If item.IsAvailable Then
                AppTheme.ApplyStatusTag(_detailAvailabilityTag, "Tersedia", AppTheme.SageTint, AppTheme.SageText)
            Else
                AppTheme.ApplyStatusTag(_detailAvailabilityTag, "Habis", AppTheme.ErrorTint, AppTheme.ErrorText)
            End If
            _detailAvailabilityTag.Location = New Point(Math.Max(120, 296 - _detailAvailabilityTag.PreferredWidth), 2)

            _detailTitleLabel.Text = item.Name
            _detailPriceLabel.Text = AppTheme.FormatRupiah(item.BasePrice)
            _detailDescriptionLabel.Text = If(String.IsNullOrWhiteSpace(item.Description), "Belum ada deskripsi untuk item ini.", item.Description)

            _detailCustomNoteLabel.Visible = True
            If item.SupportsCustomization Then
                _detailCustomNoteLabel.Text = "Mendukung kustomisasi ukuran, gula, es, susu, atau topping saat modul My Drink Creator aktif."
                _detailCustomNoteLabel.BackColor = AppTheme.AccentStrawberryTint
                _detailCustomNoteLabel.ForeColor = AppTheme.AccentStrawberryText
            Else
                _detailCustomNoteLabel.Text = "Disajikan sesuai resep standar cafe tanpa opsi kustomisasi tambahan."
                _detailCustomNoteLabel.BackColor = AppTheme.SidebarOat
                _detailCustomNoteLabel.ForeColor = AppTheme.TextSecondary
            End If

            _staffActionsPanel.Visible = AppSession.IsStaff
            If AppSession.IsStaff Then
                _toggleAvailabilityBtn.Text = If(item.IsAvailable, "Tandai Habis", "Tandai Tersedia")
            End If

            LoadDetailPhotoAsync(item.ImageUrl, _loadVersion, _selectedItemId)
        End Sub

        Private Async Sub LoadCardPhotoAsync(card As MenuCardControl, imageUrl As String, loadVersion As Integer)
            Dim bytes = Await FetchImageBytesAsync(imageUrl)
            If IsDisposed OrElse Not IsHandleCreated OrElse card.IsDisposed OrElse loadVersion <> _loadVersion Then
                Return
            End If

            If bytes Is Nothing OrElse bytes.Length = 0 Then
                Return
            End If

            Try
                Using ms As New MemoryStream(bytes)
                    Using decoded = Image.FromStream(ms)
                        card.SetPhoto(New Bitmap(decoded))
                    End Using
                End Using
            Catch
                ' Keep warm framed fallback if remote image bytes are not valid bitmap data.
            End Try
        End Sub

        Private Async Sub LoadDetailPhotoAsync(imageUrl As String, loadVersion As Integer, expectedItemId As String)
            SetDetailPhoto(Nothing)
            Dim bytes = Await FetchImageBytesAsync(imageUrl)
            If IsDisposed OrElse Not IsHandleCreated OrElse loadVersion <> _loadVersion Then
                Return
            End If
            If Not String.Equals(_selectedItemId, expectedItemId, StringComparison.Ordinal) Then
                Return
            End If

            If bytes Is Nothing OrElse bytes.Length = 0 Then
                Return
            End If

            Try
                Using ms As New MemoryStream(bytes)
                    Using decoded = Image.FromStream(ms)
                        SetDetailPhoto(New Bitmap(decoded))
                    End Using
                End Using
            Catch
                SetDetailPhoto(Nothing)
            End Try
        End Sub

        Private Shared Async Function FetchImageBytesAsync(imageUrl As String) As Task(Of Byte())
            Dim cleanUrl = If(imageUrl, String.Empty).Trim()
            If String.IsNullOrEmpty(cleanUrl) Then
                Return Nothing
            End If

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

        Private Sub SetDetailPhoto(nextImage As Image)
            Dim previous = _detailPhotoBox.Image
            _detailPhotoBox.Image = nextImage
            If previous IsNot Nothing Then
                previous.Dispose()
            End If
        End Sub

        Private Sub OnDetailPhotoPaint(sender As Object, e As PaintEventArgs)
            Using borderPen As New Pen(AppTheme.DividerAlmond, 1.0F)
                e.Graphics.DrawRectangle(borderPen, 0, 0, _detailPhotoBox.Width - 1, _detailPhotoBox.Height - 1)
            End Using

            If _detailPhotoBox.Image Is Nothing Then
                TextRenderer.DrawText(
                    e.Graphics,
                    "Foto belum tersedia",
                    AppTheme.FontCaption,
                    New Rectangle(0, 0, _detailPhotoBox.Width, _detailPhotoBox.Height),
                    AppTheme.TextSecondary,
                    TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
            End If
        End Sub

        Private Sub ShowStateOverlay(state As GalleryViewState, detailError As String)
            _currentOverlayState = state
            _catalogPane.Visible = False
            _detailPanel.Visible = False
            _countStripPanel.Visible = False
            _stateOverlay.Visible = True
            _stateProgress.Visible = (state = GalleryViewState.Loading)
            _stateActionBtn.Visible = True

            Select Case state
                Case GalleryViewState.Unconfigured
                    AppTheme.ApplyStatusTag(_stateBadge, "Belum Dikonfigurasi", AppTheme.WarningTint, AppTheme.WarningText)
                    _stateTitle.Text = "Koneksi Supabase Belum Diatur"
                    _stateMessage.Text = "Hubungkan aplikasi ke proyek Supabase Anda (URL & Publishable/Anon Key) agar katalog menu nyata dapat dimuat."
                    _stateActionBtn.Text = "Atur Koneksi Supabase"
                    AppTheme.StylePrimaryButton(_stateActionBtn)

                Case GalleryViewState.Loading
                    AppTheme.ApplyStatusTag(_stateBadge, "Memuat Data", AppTheme.TaupeTint, AppTheme.TaupeText)
                    _stateTitle.Text = "Memuat Katalog Menu"
                    _stateMessage.Text = "Mengambil daftar kopi, non-kopi, pastry, dan dessert terbaru dari Supabase..."
                    _stateActionBtn.Visible = False

                Case GalleryViewState.ErrorState
                    AppTheme.ApplyStatusTag(_stateBadge, "Gagal Memuat", AppTheme.ErrorTint, AppTheme.ErrorText)
                    _stateTitle.Text = "Katalog Menu Tidak Dapat Dimuat"
                    _stateMessage.Text = If(String.IsNullOrWhiteSpace(detailError), "Terjadi kesalahan saat menghubungi Supabase.", detailError)
                    _stateActionBtn.Text = "Coba Lagi"
                    AppTheme.StylePrimaryButton(_stateActionBtn)

                Case GalleryViewState.EmptyDatabase
                    AppTheme.ApplyStatusTag(_stateBadge, "Data Kosong", AppTheme.TaupeTint, AppTheme.TaupeText)
                    _stateTitle.Text = "Belum Ada Item Menu di Supabase"
                    _stateMessage.Text = If(AppSession.IsStaff,
                        "Tabel menu_items masih kosong. Tambahkan item menu pertama langsung dari tombol di bawah atau jalankan supabase/seed_menu.sql.",
                        "Tabel menu_items masih kosong. Hubungi staf cafe atau jalankan supabase/seed_menu.sql di SQL Editor Supabase.")
                    _stateActionBtn.Text = If(AppSession.IsStaff, "+ Tambah Menu", "Muat Ulang")
                    AppTheme.StylePrimaryButton(_stateActionBtn)

                Case GalleryViewState.EmptyFilter
                    AppTheme.ApplyStatusTag(_stateBadge, "Hasil Pencarian Kosong", AppTheme.TaupeTint, AppTheme.TaupeText)
                    _stateTitle.Text = "Tidak Ada Menu yang Cocok"
                    _stateMessage.Text = "Tidak ada produk yang sesuai dengan kata kunci, kategori, atau filter ketersediaan saat ini."
                    _stateActionBtn.Text = "Reset Filter"
                    AppTheme.StyleSecondaryButton(_stateActionBtn)
            End Select

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
                Case GalleryViewState.Unconfigured
                    Using dlg As New SupabaseConfigForm()
                        dlg.ShowDialog(FindForm())
                    End Using
                Case GalleryViewState.ErrorState
                    LoadMenuAsync()
                Case GalleryViewState.EmptyDatabase
                    If AppSession.IsStaff Then
                        OnAddMenuClick(sender, e)
                    Else
                        LoadMenuAsync()
                    End If
                Case GalleryViewState.EmptyFilter
                    _searchTextBox.Clear()
                    _selectedCategory = "Semua"
                    _showUnavailableCheck.Checked = False
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

        Private Sub OnAddMenuClick(sender As Object, e As EventArgs)
            If Not AppSession.IsStaff Then Return

            Using dlg As New MenuItemEditorForm()
                If dlg.ShowDialog(FindForm()) = DialogResult.OK Then
                    If dlg.SavedItem IsNot Nothing AndAlso Not String.IsNullOrEmpty(dlg.SavedItem.Id) Then
                        _selectedItemId = dlg.SavedItem.Id
                    End If
                    LoadMenuAsync()
                End If
            End Using
        End Sub

        Private Sub OnEditSelectedClick(sender As Object, e As EventArgs)
            If Not AppSession.IsStaff Then Return
            Dim selected = _filteredItems.FirstOrDefault(Function(x) String.Equals(x.Id, _selectedItemId, StringComparison.Ordinal))
            If selected Is Nothing Then Return

            Using dlg As New MenuItemEditorForm(selected)
                If dlg.ShowDialog(FindForm()) = DialogResult.OK Then
                    _selectedItemId = If(dlg.SavedItem?.Id, selected.Id)
                    LoadMenuAsync()
                End If
            End Using
        End Sub

        Private Async Sub OnToggleAvailabilityClick(sender As Object, e As EventArgs)
            If Not AppSession.IsStaff Then Return
            Dim selected = _filteredItems.FirstOrDefault(Function(x) String.Equals(x.Id, _selectedItemId, StringComparison.Ordinal))
            If selected Is Nothing OrElse String.IsNullOrWhiteSpace(selected.Id) Then Return

            Dim targetAvailability = Not selected.IsAvailable
            _toggleAvailabilityBtn.Enabled = False
            _editMenuBtn.Enabled = False
            _toggleAvailabilityBtn.Text = "Menyimpan..."
            _detailStatusBanner.Visible = False

            Try
                Await _menuService.SetAvailabilityAsync(selected.Id, targetAvailability)
                If IsDisposed OrElse Not IsHandleCreated Then Return

                selected.IsAvailable = targetAvailability
                _toggleAvailabilityBtn.Enabled = True
                _editMenuBtn.Enabled = True
                _detailStatusBanner.Text = If(targetAvailability, "Status diubah ke Tersedia.", "Status diubah ke Habis.")
                _detailStatusBanner.BackColor = AppTheme.SageTint
                _detailStatusBanner.ForeColor = AppTheme.SageText
                _detailStatusBanner.Visible = True

                ApplyFiltersAndRender()
            Catch ex As Exception
                If IsDisposed OrElse Not IsHandleCreated Then Return
                _toggleAvailabilityBtn.Enabled = True
                _editMenuBtn.Enabled = True
                _toggleAvailabilityBtn.Text = If(selected.IsAvailable, "Tandai Habis", "Tandai Tersedia")
                _detailStatusBanner.Text = AuthService.FormatException(ex, "Gagal memperbarui ketersediaan.")
                _detailStatusBanner.BackColor = AppTheme.ErrorTint
                _detailStatusBanner.ForeColor = AppTheme.ErrorText
                _detailStatusBanner.Visible = True
            End Try
        End Sub

        Private Sub OnSessionChanged(sender As Object, e As EventArgs)
            If IsDisposed Then Return
            If InvokeRequired Then
                BeginInvoke(New Action(AddressOf HandleSessionChangedUi))
            Else
                HandleSessionChangedUi()
            End If
        End Sub

        Private Sub HandleSessionChangedUi()
            UpdateStaffControlsVisibility()
            If _stateOverlay.Visible AndAlso _currentOverlayState = GalleryViewState.EmptyDatabase Then
                ShowStateOverlay(GalleryViewState.EmptyDatabase, String.Empty)
            End If
        End Sub

        Private Sub OnConfigurationChanged(sender As Object, e As EventArgs)
            If IsDisposed Then Return
            If InvokeRequired Then
                BeginInvoke(New Action(AddressOf LoadMenuAsync))
            Else
                LoadMenuAsync()
            End If
        End Sub

        Private Sub UpdateStaffControlsVisibility()
            _addMenuBtn.Visible = AppSession.IsStaff
            LayoutToolbar()
            Dim selected = _filteredItems.FirstOrDefault(Function(x) String.Equals(x.Id, _selectedItemId, StringComparison.Ordinal))
            _staffActionsPanel.Visible = AppSession.IsStaff AndAlso selected IsNot Nothing
            If AppSession.IsStaff AndAlso selected IsNot Nothing Then
                _toggleAvailabilityBtn.Text = If(selected.IsAvailable, "Tandai Habis", "Tandai Tersedia")
            End If
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing Then
                RemoveHandler AppSession.SessionChanged, AddressOf OnSessionChanged
                RemoveHandler SupabaseClientProvider.ConfigurationChanged, AddressOf OnConfigurationChanged
                SetDetailPhoto(Nothing)
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private NotInheritable Class BufferedFlowLayoutPanel
            Inherits FlowLayoutPanel

            Public Sub New()
                DoubleBuffered = True
                ResizeRedraw = True
            End Sub
        End Class

        Private NotInheritable Class MenuCardControl
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
                Dim availText = If(item.IsAvailable, "Tersedia", "Habis")
                AccessibleName = $"{item.Name}, {item.Category}, {AppTheme.FormatRupiah(item.BasePrice)}, {availText}"
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
                Dim cardBg = If(_isHovered AndAlso Not _isSelected, ColorTranslator.FromHtml("#FCF7F0"), AppTheme.SurfaceMilk)

                Using cardPath = AppTheme.CreateRoundedRectPath(cardBounds, 8)
                    Using bgBrush As New SolidBrush(cardBg)
                        e.Graphics.FillPath(bgBrush, cardPath)
                    End Using

                    Dim borderColor = AppTheme.DividerAlmond
                    Dim borderWidth = 1.0F
                    If Focused OrElse _isSelected Then
                        borderColor = AppTheme.FocusMocha
                        borderWidth = 2.0F
                    ElseIf _isHovered Then
                        borderColor = AppTheme.ControlBorder
                    End If

                    Using borderPen As New Pen(borderColor, borderWidth)
                        e.Graphics.DrawPath(borderPen, cardPath)
                    End Using
                End Using

                Dim photoRect As New Rectangle(8, 8, Width - 16, 134)
                Using photoPath = AppTheme.CreateRoundedRectPath(photoRect, 6)
                    Using photoBg As New SolidBrush(AppTheme.SidebarOat)
                        e.Graphics.FillPath(photoBg, photoPath)
                    End Using

                    If _photo IsNot Nothing Then
                        Dim oldClip = e.Graphics.Clip
                        e.Graphics.SetClip(photoPath)
                        DrawAspectFillImage(e.Graphics, _photo, photoRect)
                        e.Graphics.Clip = oldClip
                    Else
                        TextRenderer.DrawText(
                            e.Graphics,
                            "Foto belum tersedia",
                            AppTheme.FontCaption,
                            photoRect,
                            AppTheme.TextSecondary,
                            TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
                    End If
                End Using

                Dim customW = 0
                If Item.SupportsCustomization Then
                    Dim customText = "Bisa Dikustom"
                    customW = TextRenderer.MeasureText(customText, AppTheme.FontCaptionBold, Size.Empty, TextFormatFlags.SingleLine).Width + 14
                    Dim customRect As New Rectangle(Width - customW - 10, 149, customW, 22)
                    Using customPath = AppTheme.CreateRoundedRectPath(customRect, 4)
                        Using customBrush As New SolidBrush(AppTheme.AccentStrawberryTint)
                            e.Graphics.FillPath(customBrush, customPath)
                        End Using
                    End Using
                    TextRenderer.DrawText(
                        e.Graphics,
                        customText,
                        AppTheme.FontCaptionBold,
                        customRect,
                        AppTheme.AccentStrawberryText,
                        TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
                End If

                Dim categoryWidth = If(customW > 0, Width - customW - 26, Width - 20)
                Dim categoryRect As New Rectangle(10, 150, categoryWidth, 22)
                TextRenderer.DrawText(
                    e.Graphics,
                    Item.Category,
                    AppTheme.FontCaptionBold,
                    categoryRect,
                    AppTheme.TextSecondary,
                    TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)

                Dim nameRect As New Rectangle(10, 176, Width - 20, 56)
                TextRenderer.DrawText(
                    e.Graphics,
                    Item.Name,
                    AppTheme.FontCardTitle,
                    nameRect,
                    AppTheme.TextPrimary,
                    TextFormatFlags.Left Or TextFormatFlags.Top Or TextFormatFlags.WordBreak Or TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPrefix)

                Dim availText = If(Item.IsAvailable, "Tersedia", "Habis")
                Dim availBg = If(Item.IsAvailable, AppTheme.SageTint, AppTheme.ErrorTint)
                Dim availFg = If(Item.IsAvailable, AppTheme.SageText, AppTheme.ErrorText)
                Dim availW = TextRenderer.MeasureText(availText, AppTheme.FontCaptionBold, Size.Empty, TextFormatFlags.SingleLine).Width + 16
                Dim availRect As New Rectangle(Width - availW - 10, 242, availW, 24)

                Dim priceRect As New Rectangle(10, 240, Width - availW - 24, 28)
                TextRenderer.DrawText(
                    e.Graphics,
                    AppTheme.FormatRupiah(Item.BasePrice),
                    AppTheme.FontBodyBold,
                    priceRect,
                    AppTheme.PrimaryMocha,
                    TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)

                Using availPath = AppTheme.CreateRoundedRectPath(availRect, 4)
                    Using availBrush As New SolidBrush(availBg)
                        e.Graphics.FillPath(availBrush, availPath)
                    End Using
                End Using
                TextRenderer.DrawText(
                    e.Graphics,
                    availText,
                    AppTheme.FontCaptionBold,
                    availRect,
                    availFg,
                    TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
            End Sub

            Private Shared Sub DrawAspectFillImage(g As Graphics, img As Image, target As Rectangle)
                If img.Width <= 0 OrElse img.Height <= 0 Then Return
                Dim scale = Math.Max(CSng(target.Width) / img.Width, CSng(target.Height) / img.Height)
                Dim drawW = img.Width * scale
                Dim drawH = img.Height * scale
                Dim drawX = target.X + (target.Width - drawW) / 2.0F
                Dim drawY = target.Y + (target.Height - drawH) / 2.0F
                g.DrawImage(img, drawX, drawY, drawW, drawH)
            End Sub

            Protected Overrides Sub Dispose(disposing As Boolean)
                If disposing AndAlso _photo IsNot Nothing Then
                    _photo.Dispose()
                    _photo = Nothing
                End If
                MyBase.Dispose(disposing)
            End Sub
        End Class
    End Class
End Namespace
