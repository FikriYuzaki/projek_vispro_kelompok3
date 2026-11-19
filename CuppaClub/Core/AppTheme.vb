Option Strict On
Option Explicit On

Imports System.Drawing.Drawing2D
Imports System.Globalization

Namespace Core
    Public NotInheritable Class AppTheme
        Private Sub New()
        End Sub

        Public Shared ReadOnly CanvasCream As Color = ColorTranslator.FromHtml("#FAF4EC")
        Public Shared ReadOnly SurfaceMilk As Color = ColorTranslator.FromHtml("#FFFDF9")
        Public Shared ReadOnly SidebarOat As Color = ColorTranslator.FromHtml("#F3E8DC")
        Public Shared ReadOnly SidebarHover As Color = ColorTranslator.FromHtml("#EAE0D3")
        Public Shared ReadOnly SidebarActive As Color = ColorTranslator.FromHtml("#E6D6C6")

        Public Shared ReadOnly DividerAlmond As Color = ColorTranslator.FromHtml("#E2D3C3")
        Public Shared ReadOnly ControlBorder As Color = ColorTranslator.FromHtml("#967B65")
        Public Shared ReadOnly FocusMocha As Color = ColorTranslator.FromHtml("#6B442E")

        Public Shared ReadOnly TextPrimary As Color = ColorTranslator.FromHtml("#3B261C")
        Public Shared ReadOnly TextSecondary As Color = ColorTranslator.FromHtml("#6E5647")

        Public Shared ReadOnly PrimaryMocha As Color = ColorTranslator.FromHtml("#6B442E")
        Public Shared ReadOnly PrimaryMochaHover As Color = ColorTranslator.FromHtml("#553523")

        Public Shared ReadOnly AccentStrawberry As Color = ColorTranslator.FromHtml("#B85467")
        Public Shared ReadOnly AccentStrawberryDark As Color = ColorTranslator.FromHtml("#9E3D50")
        Public Shared ReadOnly AccentStrawberryTint As Color = ColorTranslator.FromHtml("#FBECEF")
        Public Shared ReadOnly AccentStrawberryText As Color = ColorTranslator.FromHtml("#8F3547")

        Public Shared ReadOnly SageSolid As Color = ColorTranslator.FromHtml("#4A6B53")
        Public Shared ReadOnly SageTint As Color = ColorTranslator.FromHtml("#EAF2EC")
        Public Shared ReadOnly SageText As Color = ColorTranslator.FromHtml("#2F4F38")

        Public Shared ReadOnly TaupeTint As Color = ColorTranslator.FromHtml("#EFE7DF")
        Public Shared ReadOnly TaupeText As Color = ColorTranslator.FromHtml("#5C4E43")

        Public Shared ReadOnly WarningTint As Color = ColorTranslator.FromHtml("#FDF3E3")
        Public Shared ReadOnly WarningText As Color = ColorTranslator.FromHtml("#7A460E")

        Public Shared ReadOnly ErrorSolid As Color = ColorTranslator.FromHtml("#A63A43")
        Public Shared ReadOnly ErrorTint As Color = ColorTranslator.FromHtml("#FCEBEC")
        Public Shared ReadOnly ErrorText As Color = ColorTranslator.FromHtml("#852931")

        Public Shared ReadOnly FontBrand As New Font("Segoe UI", 14.0F, FontStyle.Bold, GraphicsUnit.Point)
        Public Shared ReadOnly FontPageTitle As New Font("Segoe UI", 16.0F, FontStyle.Bold, GraphicsUnit.Point)
        Public Shared ReadOnly FontSectionTitle As New Font("Segoe UI", 11.5F, FontStyle.Bold, GraphicsUnit.Point)
        Public Shared ReadOnly FontCardTitle As New Font("Segoe UI", 10.5F, FontStyle.Bold, GraphicsUnit.Point)
        Public Shared ReadOnly FontBody As New Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point)
        Public Shared ReadOnly FontBodyBold As New Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point)
        Public Shared ReadOnly FontCaption As New Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point)
        Public Shared ReadOnly FontCaptionBold As New Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point)

        Public Const SpaceSm As Integer = 8
        Public Const SpaceMd As Integer = 16
        Public Const SpaceLg As Integer = 24

        Private Shared ReadOnly IdCulture As CultureInfo = CultureInfo.GetCultureInfo("id-ID")

        Public Shared Function FormatRupiah(amount As Decimal) As String
            Return "Rp " & amount.ToString("#,##0", IdCulture)
        End Function

        Public Shared Sub StylePrimaryButton(button As Button)
            button.FlatStyle = FlatStyle.Flat
            button.BackColor = PrimaryMocha
            button.ForeColor = SurfaceMilk
            button.Font = FontBodyBold
            button.Cursor = Cursors.Hand
            button.FlatAppearance.BorderSize = 1
            button.FlatAppearance.BorderColor = PrimaryMocha
            button.FlatAppearance.MouseOverBackColor = PrimaryMochaHover
            button.FlatAppearance.MouseDownBackColor = TextPrimary
        End Sub

        Public Shared Sub StyleAccentButton(button As Button)
            button.FlatStyle = FlatStyle.Flat
            button.BackColor = AccentStrawberryDark
            button.ForeColor = SurfaceMilk
            button.Font = FontBodyBold
            button.Cursor = Cursors.Hand
            button.FlatAppearance.BorderSize = 1
            button.FlatAppearance.BorderColor = AccentStrawberryDark
            button.FlatAppearance.MouseOverBackColor = AccentStrawberryText
            button.FlatAppearance.MouseDownBackColor = AccentStrawberryText
        End Sub

        Public Shared Sub StyleSecondaryButton(button As Button)
            button.FlatStyle = FlatStyle.Flat
            button.BackColor = SurfaceMilk
            button.ForeColor = TextPrimary
            button.Font = FontBodyBold
            button.Cursor = Cursors.Hand
            button.FlatAppearance.BorderSize = 1
            button.FlatAppearance.BorderColor = ControlBorder
            button.FlatAppearance.MouseOverBackColor = SidebarOat
            button.FlatAppearance.MouseDownBackColor = SidebarActive
        End Sub

        Public Shared Sub StyleDangerButton(button As Button)
            button.FlatStyle = FlatStyle.Flat
            button.BackColor = ErrorTint
            button.ForeColor = ErrorText
            button.Font = FontBodyBold
            button.Cursor = Cursors.Hand
            button.FlatAppearance.BorderSize = 1
            button.FlatAppearance.BorderColor = ErrorSolid
            button.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#F7D7DA")
            button.FlatAppearance.MouseDownBackColor = ColorTranslator.FromHtml("#F2C4C8")
        End Sub

        Public Shared Sub ApplyStatusTag(label As Label, text As String, backColor As Color, foreColor As Color)
            label.Text = text
            label.BackColor = backColor
            label.ForeColor = foreColor
            label.Font = FontCaptionBold
            label.Padding = New Padding(8, 3, 8, 3)
            label.AutoSize = True
        End Sub

        Public Shared Function CreateRoundedRectPath(bounds As Rectangle, radius As Integer) As GraphicsPath
            Dim path As New GraphicsPath()
            Dim safeRadius = Math.Max(1, Math.Min(radius, Math.Min(bounds.Width, bounds.Height) \ 2))
            Dim diameter = safeRadius * 2

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90)
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90)
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90)
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90)
            path.CloseFigure()
            Return path
        End Function
    End Class
End Namespace
