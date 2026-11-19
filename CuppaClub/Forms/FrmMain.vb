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

    Private _activeFeature As FeatureDefinition
    Private _activeView As UserControl
    Private _runtimeInitialized As Boolean

    Public Sub New()
        InitializeComponent()

        If LicenseManager.UsageMode = LicenseUsageMode.Designtime OrElse DesignMode Then
            Return
        End If

        _runtimeInitialized = True
        WireRuntimeBehavior()

        AddHandler AppSession.SessionChanged, AddressOf OnSessionChanged
        AddHandler SupabaseClientProvider.ConfigurationChanged, AddressOf OnConfigurationChanged

        RefreshAccountUi()

        If FeatureRegistry.Features.Count > 0 Then
            SelectFeature(FeatureRegistry.Features(0))
        End If
    End Sub

    Private Sub WireRuntimeBehavior()
        AddHandler _sidebarPanel.Paint, AddressOf OnSidebarPaint
        AddHandler _brandPanel.Paint, AddressOf OnBrandPanelPaint
        AddHandler _bottomUtilityPanel.Paint, Sub(s, e)
                                                   Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                                       e.Graphics.DrawLine(pen, 14, 0, _bottomUtilityPanel.Width - 15, 0)
                                                   End Using
                                               End Sub
        AddHandler _accountCard.Paint, Sub(s, e)
                                            Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                                e.Graphics.DrawRectangle(pen, 0, 0, _accountCard.Width - 1, _accountCard.Height - 1)
                                            End Using
                                        End Sub
        AddHandler _headerPanel.Paint, Sub(s, e)
                                            Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                                                e.Graphics.DrawLine(pen, 0, _headerPanel.Height - 1, _headerPanel.Width, _headerPanel.Height - 1)
                                            End Using
                                        End Sub
        AddHandler _headerPanel.Resize, Sub()
                                             _headerConnectionBtn.Location = New Point(_headerPanel.ClientSize.Width - _headerConnectionBtn.Width - 24, 22)
                                         End Sub

        AddHandler _sidebarConnBtn.Click, AddressOf OnOpenConfigClick
        AddHandler _headerConnectionBtn.Click, AddressOf OnOpenConfigClick
        AddHandler _accountActionBtn.Click, AddressOf OnAccountActionClick

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

        RefreshConnectionUi()
    End Sub

    Private Sub RefreshConnectionUi()
        Dim configured = SupabaseClientProvider.IsConfigured
        Dim showConnectionControls = (Not configured) OrElse AppSession.IsStaff

        _sidebarConnBtn.Visible = showConnectionControls
        _headerConnectionBtn.Visible = showConnectionControls

        If showConnectionControls Then
            _bottomUtilityPanel.Height = 174
            _accountCard.Location = New Point(14, 54)
        Else
            _bottomUtilityPanel.Height = 130
            _accountCard.Location = New Point(14, 12)
        End If

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

    Private Sub CleanupRuntimeResources()
        If _runtimeInitialized Then
            RemoveHandler AppSession.SessionChanged, AddressOf OnSessionChanged
            RemoveHandler SupabaseClientProvider.ConfigurationChanged, AddressOf OnConfigurationChanged
            _runtimeInitialized = False
        End If
        If _activeView IsNot Nothing Then
            _activeView.Dispose()
            _activeView = Nothing
        End If
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
