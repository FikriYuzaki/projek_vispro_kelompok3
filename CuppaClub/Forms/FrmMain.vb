Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports CuppaClub.Core
Imports CuppaClub.Forms
Imports CuppaClub.Services

Public Class FrmMain
    Inherits Form

    Private ReadOnly _authService As New AuthService()
    Private ReadOnly _navItems As New List(Of SidebarNavItem)()

    Private ReadOnly _sidebarPanel As New Panel()
    Private ReadOnly _navHost As New FlowLayoutPanel()
    Private ReadOnly _headerPanel As New Panel()
    Private ReadOnly _headerTitleLabel As New Label()
    Private ReadOnly _headerSubtitleLabel As New Label()
    Private ReadOnly _headerConnectionBtn As New Button()
    Private ReadOnly _contentHost As New Panel()

    Private ReadOnly _sidebarConnBtn As New Button()
    Private ReadOnly _accountNameLabel As New Label()
    Private ReadOnly _accountRoleLabel As New Label()
    Private ReadOnly _accountActionBtn As New Button()

    Private _activeFeature As FeatureDefinition
    Private _activeView As UserControl

    Public Sub New()
        Text = "CuppaClub - Desktop Cafe App"
        ClientSize = New Size(1200, 760)
        MinimumSize = New Size(1024, 680)
        StartPosition = FormStartPosition.CenterScreen
        BackColor = AppTheme.CanvasCream
        ForeColor = AppTheme.TextPrimary
        Font = AppTheme.FontBody
        DoubleBuffered = True

        BuildShell()

        AddHandler AppSession.SessionChanged, AddressOf OnSessionChanged
        AddHandler SupabaseClientProvider.ConfigurationChanged, AddressOf OnConfigurationChanged

        RefreshAccountUi()
        RefreshConnectionUi()

        If FeatureRegistry.Features.Count > 0 Then
            SelectFeature(FeatureRegistry.Features(0))
        End If
    End Sub

    Private Sub BuildShell()
        _sidebarPanel.Dock = DockStyle.Left
        _sidebarPanel.Width = 268
        _sidebarPanel.BackColor = AppTheme.SidebarOat
        AddHandler _sidebarPanel.Paint, AddressOf OnSidebarPaint

        Dim brandPanel As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 84,
            .BackColor = Color.Transparent,
            .Padding = New Padding(18, 18, 16, 12)
        }
        AddHandler brandPanel.Paint, AddressOf OnBrandPanelPaint

        Dim brandTitle As New Label With {
            .Name = "BrandTitle",
            .Text = "CuppaClub",
            .Font = AppTheme.FontBrand,
            .ForeColor = AppTheme.TextPrimary,
            .Location = New Point(64, 16),
            .AutoSize = True
        }

        Dim brandSubtitle As New Label With {
            .Name = "BrandSubtitle",
            .Text = "Sweet Cafe Desktop",
            .Font = AppTheme.FontCaption,
            .ForeColor = AppTheme.TextSecondary,
            .Location = New Point(64, 50),
            .AutoSize = True
        }

        brandPanel.Controls.Add(brandTitle)
        brandPanel.Controls.Add(brandSubtitle)

        Dim navSectionLabel As New Label With {
            .Dock = DockStyle.Top,
            .Text = "Fitur Utama",
            .Font = AppTheme.FontCaptionBold,
            .ForeColor = AppTheme.TextSecondary,
            .Padding = New Padding(18, 10, 12, 6),
            .Height = 32
        }

        _navHost.Dock = DockStyle.Fill
        _navHost.FlowDirection = FlowDirection.TopDown
        _navHost.WrapContents = False
        _navHost.AutoScroll = True
        _navHost.Padding = New Padding(12, 4, 12, 12)
        _navHost.BackColor = Color.Transparent

        Dim tabIdx = 0
        For Each feature In FeatureRegistry.Features
            Dim navItem As New SidebarNavItem(feature) With {
                .Width = 242,
                .Height = 60,
                .Margin = New Padding(0, 0, 0, 6),
                .TabIndex = tabIdx
            }
            tabIdx += 1
            Dim captured = feature
            AddHandler navItem.Activated, Sub() SelectFeature(captured)
            _navItems.Add(navItem)
            _navHost.Controls.Add(navItem)
        Next

        Dim bottomUtilityPanel As New Panel With {
            .Dock = DockStyle.Bottom,
            .Height = 174,
            .BackColor = Color.Transparent,
            .Padding = New Padding(14, 12, 14, 14)
        }
        AddHandler bottomUtilityPanel.Paint, Sub(s, e)
                                                 Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                                     e.Graphics.DrawLine(pen, 14, 0, bottomUtilityPanel.Width - 15, 0)
                                                 End Using
                                             End Sub

        _sidebarConnBtn.Location = New Point(14, 12)
        _sidebarConnBtn.Size = New Size(240, 34)
        _sidebarConnBtn.FlatStyle = FlatStyle.Flat
        _sidebarConnBtn.Font = AppTheme.FontCaptionBold
        _sidebarConnBtn.Cursor = Cursors.Hand
        _sidebarConnBtn.TextAlign = ContentAlignment.MiddleLeft
        _sidebarConnBtn.Padding = New Padding(10, 0, 10, 0)
        _sidebarConnBtn.TabIndex = 10
        AddHandler _sidebarConnBtn.Click, AddressOf OnOpenConfigClick

        Dim accountCard As New Panel With {
            .Location = New Point(14, 54),
            .Size = New Size(240, 104),
            .BackColor = AppTheme.SurfaceMilk,
            .Padding = New Padding(12, 10, 12, 10)
        }
        AddHandler accountCard.Paint, Sub(s, e)
                                          Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                              e.Graphics.DrawRectangle(pen, 0, 0, accountCard.Width - 1, accountCard.Height - 1)
                                          End Using
                                      End Sub

        _accountNameLabel.Location = New Point(12, 10)
        _accountNameLabel.Size = New Size(216, 24)
        _accountNameLabel.Font = AppTheme.FontBodyBold
        _accountNameLabel.ForeColor = AppTheme.TextPrimary
        _accountNameLabel.AutoEllipsis = True

        _accountRoleLabel.Location = New Point(12, 34)
        _accountRoleLabel.Size = New Size(216, 20)
        _accountRoleLabel.Font = AppTheme.FontCaption
        _accountRoleLabel.ForeColor = AppTheme.TextSecondary
        _accountRoleLabel.AutoEllipsis = True

        _accountActionBtn.Location = New Point(12, 60)
        _accountActionBtn.Size = New Size(216, 32)
        _accountActionBtn.TabIndex = 11
        AddHandler _accountActionBtn.Click, AddressOf OnAccountActionClick

        accountCard.Controls.Add(_accountNameLabel)
        accountCard.Controls.Add(_accountRoleLabel)
        accountCard.Controls.Add(_accountActionBtn)

        bottomUtilityPanel.Controls.Add(_sidebarConnBtn)
        bottomUtilityPanel.Controls.Add(accountCard)

        _sidebarPanel.Controls.Add(_navHost)
        _sidebarPanel.Controls.Add(navSectionLabel)
        _sidebarPanel.Controls.Add(brandPanel)
        _sidebarPanel.Controls.Add(bottomUtilityPanel)

        Dim rightWorkspace As New Panel With {
            .Dock = DockStyle.Fill,
            .BackColor = AppTheme.CanvasCream
        }

        _headerPanel.Dock = DockStyle.Top
        _headerPanel.Height = 76
        _headerPanel.BackColor = AppTheme.SurfaceMilk
        _headerPanel.Padding = New Padding(24, 10, 24, 10)
        AddHandler _headerPanel.Paint, Sub(s, e)
                                           Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                               e.Graphics.DrawLine(pen, 0, _headerPanel.Height - 1, _headerPanel.Width, _headerPanel.Height - 1)
                                           End Using
                                       End Sub

        _headerTitleLabel.Location = New Point(24, 8)
        _headerTitleLabel.Font = AppTheme.FontPageTitle
        _headerTitleLabel.ForeColor = AppTheme.TextPrimary
        _headerTitleLabel.AutoSize = True

        _headerSubtitleLabel.Location = New Point(25, 46)
        _headerSubtitleLabel.Font = AppTheme.FontCaption
        _headerSubtitleLabel.ForeColor = AppTheme.TextSecondary
        _headerSubtitleLabel.AutoSize = True

        _headerConnectionBtn.Size = New Size(216, 32)
        _headerConnectionBtn.Location = New Point(_headerPanel.Width - _headerConnectionBtn.Width - 24, 22)
        _headerConnectionBtn.FlatStyle = FlatStyle.Flat
        _headerConnectionBtn.Font = AppTheme.FontCaptionBold
        _headerConnectionBtn.Cursor = Cursors.Hand
        _headerConnectionBtn.TabIndex = 12
        AddHandler _headerConnectionBtn.Click, AddressOf OnOpenConfigClick
        AddHandler _headerPanel.Resize, Sub()
                                            _headerConnectionBtn.Location = New Point(_headerPanel.ClientSize.Width - _headerConnectionBtn.Width - 24, 22)
                                        End Sub

        _headerPanel.Controls.Add(_headerTitleLabel)
        _headerPanel.Controls.Add(_headerSubtitleLabel)
        _headerPanel.Controls.Add(_headerConnectionBtn)

        _contentHost.Dock = DockStyle.Fill
        _contentHost.BackColor = AppTheme.CanvasCream

        rightWorkspace.Controls.Add(_contentHost)
        rightWorkspace.Controls.Add(_headerPanel)

        Controls.Add(rightWorkspace)
        Controls.Add(_sidebarPanel)
    End Sub

    Private Sub SelectFeature(feature As FeatureDefinition)
        If feature Is Nothing Then Return
        _activeFeature = feature

        For Each navItem In _navItems
            navItem.IsSelected = String.Equals(navItem.Feature.Id, feature.Id, StringComparison.Ordinal)
        Next

        _headerTitleLabel.Text = feature.DisplayName
        _headerSubtitleLabel.Text = $"Penanggung jawab: {feature.Owner}  |  Status: {FeatureRegistry.StatusLabel(feature.Status)}"

        Dim nextView = FeatureRegistry.CreateControl(feature)
        nextView.Dock = DockStyle.Fill

        Dim previousView = _activeView
        _activeView = nextView

        _contentHost.SuspendLayout()
        Try
            _contentHost.Controls.Clear()
            _contentHost.Controls.Add(nextView)
        Finally
            _contentHost.ResumeLayout(True)
        End Try

        If previousView IsNot Nothing Then
            previousView.Dispose()
        End If
    End Sub

    Private Sub OnOpenConfigClick(sender As Object, e As EventArgs)
        Using dlg As New SupabaseConfigForm()
            dlg.ShowDialog(Me)
        End Using
        RefreshConnectionUi()
    End Sub

    Private Async Sub OnAccountActionClick(sender As Object, e As EventArgs)
        If AppSession.IsAuthenticated Then
            _accountActionBtn.Enabled = False
            _accountActionBtn.Text = "Keluar..."
            Await _authService.SignOutAsync()
            If IsDisposed Then Return
            _accountActionBtn.Enabled = True
            RefreshAccountUi()
        Else
            Using dlg As New LoginForm()
                dlg.ShowDialog(Me)
            End Using
            RefreshAccountUi()
        End If
    End Sub

    Private Sub OnSessionChanged(sender As Object, e As EventArgs)
        If IsDisposed Then Return
        If InvokeRequired Then
            BeginInvoke(New Action(AddressOf RefreshAccountUi))
        Else
            RefreshAccountUi()
        End If
    End Sub

    Private Sub OnConfigurationChanged(sender As Object, e As EventArgs)
        If IsDisposed Then Return
        If InvokeRequired Then
            BeginInvoke(New Action(AddressOf RefreshConnectionUi))
        Else
            RefreshConnectionUi()
        End If
    End Sub

    Private Sub RefreshAccountUi()
        If AppSession.IsAuthenticated Then
            Dim user = AppSession.CurrentUser
            _accountNameLabel.Text = user.DisplayLabel
            _accountRoleLabel.Text = $"Peran: {user.RoleLabel}"
            _accountActionBtn.Text = "Keluar"
            AppTheme.StyleSecondaryButton(_accountActionBtn)
        Else
            _accountNameLabel.Text = "Tamu (Belum Masuk)"
            _accountRoleLabel.Text = "Masuk untuk akun atau staf"
            _accountActionBtn.Text = "Masuk / Daftar"
            AppTheme.StylePrimaryButton(_accountActionBtn)
        End If
    End Sub

    Private Sub RefreshConnectionUi()
        Dim configured = SupabaseClientProvider.IsConfigured
        If configured Then
            _sidebarConnBtn.Text = "Supabase: Terhubung"
            _sidebarConnBtn.BackColor = AppTheme.SageTint
            _sidebarConnBtn.ForeColor = AppTheme.SageText
            _sidebarConnBtn.FlatAppearance.BorderSize = 1
            _sidebarConnBtn.FlatAppearance.BorderColor = AppTheme.SageSolid

            _headerConnectionBtn.Text = "Supabase: Terhubung"
            _headerConnectionBtn.BackColor = AppTheme.SageTint
            _headerConnectionBtn.ForeColor = AppTheme.SageText
            _headerConnectionBtn.FlatAppearance.BorderSize = 1
            _headerConnectionBtn.FlatAppearance.BorderColor = AppTheme.SageSolid
        Else
            _sidebarConnBtn.Text = "Supabase: Belum Diatur"
            _sidebarConnBtn.BackColor = AppTheme.WarningTint
            _sidebarConnBtn.ForeColor = AppTheme.WarningText
            _sidebarConnBtn.FlatAppearance.BorderSize = 1
            _sidebarConnBtn.FlatAppearance.BorderColor = AppTheme.ControlBorder

            _headerConnectionBtn.Text = "Atur Koneksi Supabase"
            _headerConnectionBtn.BackColor = AppTheme.WarningTint
            _headerConnectionBtn.ForeColor = AppTheme.WarningText
            _headerConnectionBtn.FlatAppearance.BorderSize = 1
            _headerConnectionBtn.FlatAppearance.BorderColor = AppTheme.ControlBorder
        End If

        _headerConnectionBtn.Width = AppTheme.MeasureButtonWidth(_headerConnectionBtn.Text, AppTheme.FontCaptionBold, 210, 28)
        _headerConnectionBtn.Location = New Point(_headerPanel.ClientSize.Width - _headerConnectionBtn.Width - 24, 22)
    End Sub

    Private Sub OnSidebarPaint(sender As Object, e As PaintEventArgs)
        Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
            e.Graphics.DrawLine(pen, _sidebarPanel.Width - 1, 0, _sidebarPanel.Width - 1, _sidebarPanel.Height)
        End Using
    End Sub

    Private Sub OnBrandPanelPaint(sender As Object, e As PaintEventArgs)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

        Dim badgeRect As New Rectangle(18, 20, 38, 38)
        Using path = AppTheme.CreateRoundedRectPath(badgeRect, 8)
            Using fillBrush As New SolidBrush(AppTheme.PrimaryMocha)
                e.Graphics.FillPath(fillBrush, path)
            End Using
        End Using

        ' Draw minimal cafe cup mark inside brand badge without relying on external logo assets.
        Using cupPen As New Pen(AppTheme.SurfaceMilk, 2.0F)
            cupPen.StartCap = LineCap.Round
            cupPen.EndCap = LineCap.Round
            e.Graphics.DrawRectangle(cupPen, 28, 34, 15, 13)
            e.Graphics.DrawArc(cupPen, 41, 36, 6, 8, -90, 180)
            e.Graphics.DrawLine(cupPen, 26, 50, 46, 50)
        End Using
        Using steamPen As New Pen(AppTheme.AccentStrawberryTint, 1.8F)
            steamPen.StartCap = LineCap.Round
            steamPen.EndCap = LineCap.Round
            e.Graphics.DrawLine(steamPen, 33, 27, 33, 30)
            e.Graphics.DrawLine(steamPen, 38, 26, 38, 30)
        End Using
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            RemoveHandler AppSession.SessionChanged, AddressOf OnSessionChanged
            RemoveHandler SupabaseClientProvider.ConfigurationChanged, AddressOf OnConfigurationChanged
            If _activeView IsNot Nothing Then
                _activeView.Dispose()
                _activeView = Nothing
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    Private NotInheritable Class SidebarNavItem
        Inherits Control

        Private _isSelected As Boolean
        Private _isHovered As Boolean

        Public ReadOnly Property Feature As FeatureDefinition
        Public Event Activated As EventHandler

        Public Sub New(feature As FeatureDefinition)
            Me.Feature = feature
            DoubleBuffered = True
            Cursor = Cursors.Hand
            TabStop = True
            SetStyle(ControlStyles.Selectable Or ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer, True)
            AccessibleName = $"{feature.DisplayName} ({FeatureRegistry.StatusLabel(feature.Status)})"
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
            Dim bounds As New Rectangle(0, 0, Width - 1, Height - 1)

            Dim bg = AppTheme.SidebarOat
            If _isSelected Then
                bg = AppTheme.SidebarActive
            ElseIf _isHovered Then
                bg = AppTheme.SidebarHover
            End If

            Using path = AppTheme.CreateRoundedRectPath(bounds, 6)
                Using brush As New SolidBrush(bg)
                    e.Graphics.FillPath(brush, path)
                End Using

                If Focused Then
                    Using focusPen As New Pen(AppTheme.FocusMocha, 2.0F)
                        e.Graphics.DrawPath(focusPen, path)
                    End Using
                ElseIf _isSelected Then
                    Using borderPen As New Pen(AppTheme.ControlBorder, 1.0F)
                        e.Graphics.DrawPath(borderPen, path)
                    End Using
                End If
            End Using

            If _isSelected Then
                Using activeBar As New SolidBrush(AppTheme.PrimaryMocha)
                    e.Graphics.FillRectangle(activeBar, 0, 8, 4, Height - 16)
                End Using
            End If

            Dim titleColor = AppTheme.TextPrimary
            Dim titleFont = If(_isSelected, AppTheme.FontBodyBold, AppTheme.FontBody)
            Dim titleRect As New Rectangle(14, 8, Width - 22, 24)
            TextRenderer.DrawText(
                e.Graphics,
                Feature.DisplayName,
                titleFont,
                titleRect,
                titleColor,
                TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)

            Dim statusText = FeatureRegistry.StatusLabel(Feature.Status)
            Dim statusBg = AppTheme.TaupeTint
            Dim statusFg = AppTheme.TaupeText
            If Feature.Status = FeatureStatus.Available Then
                statusBg = AppTheme.SageTint
                statusFg = AppTheme.SageText
            ElseIf Feature.Status = FeatureStatus.Disabled Then
                statusBg = AppTheme.WarningTint
                statusFg = AppTheme.WarningText
            End If

            Dim badgeWidth = TextRenderer.MeasureText(statusText, AppTheme.FontCaptionBold, Size.Empty, TextFormatFlags.SingleLine).Width + 14
            Dim badgeRect As New Rectangle(14, 34, badgeWidth, 20)
            Using badgePath = AppTheme.CreateRoundedRectPath(badgeRect, 4)
                Using badgeBrush As New SolidBrush(statusBg)
                    e.Graphics.FillPath(badgeBrush, badgePath)
                End Using
            End Using
            TextRenderer.DrawText(
                e.Graphics,
                statusText,
                AppTheme.FontCaptionBold,
                badgeRect,
                statusFg,
                TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)

            Dim ownerRect As New Rectangle(badgeRect.Right + 8, 34, Width - badgeRect.Right - 14, 20)
            TextRenderer.DrawText(
                e.Graphics,
                Feature.Owner,
                AppTheme.FontCaption,
                ownerRect,
                AppTheme.TextSecondary,
                TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
        End Sub
    End Class
End Class
