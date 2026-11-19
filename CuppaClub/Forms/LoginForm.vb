Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports CuppaClub.Core
Imports CuppaClub.Services

Namespace Forms
    Public Class LoginForm
        Inherits Form

        Private ReadOnly _authService As New AuthService()
        Private _isRegisterMode As Boolean
        Private _showConfigBanner As Boolean

        Public Sub New()
            InitializeComponent()

            If LicenseManager.UsageMode = LicenseUsageMode.Designtime OrElse DesignMode Then
                Return
            End If

            WireRuntimeBehavior()
            ApplyButtonSizing()
            UpdateModeUi()
            RefreshConfigBanner()
        End Sub

        Private Sub WireRuntimeBehavior()
            AddHandler _signInModeBtn.Click, Sub()
                                                 _isRegisterMode = False
                                                 ClearFeedback()
                                                 UpdateModeUi()
                                             End Sub
            AddHandler _registerModeBtn.Click, Sub()
                                                   _isRegisterMode = True
                                                   ClearFeedback()
                                                   UpdateModeUi()
                                               End Sub
            AddHandler _openConfigBtn.Click, AddressOf OnOpenConfigClick
            AddHandler _showPasswordCheckBox.CheckedChanged, Sub()
                                                                 _passwordTextBox.UseSystemPasswordChar = Not _showPasswordCheckBox.Checked
                                                             End Sub
            AddHandler _submitButton.Click, AddressOf OnSubmitClick
        End Sub

        Private Sub ApplyButtonSizing()
            _signInModeBtn.Width = AppTheme.MeasureButtonWidth(_signInModeBtn.Text, AppTheme.FontBodyBold, 116, 28)
            _registerModeBtn.Width = AppTheme.MeasureButtonWidth(_registerModeBtn.Text, AppTheme.FontBodyBold, 128, 28)
            _registerModeBtn.Location = New Point(_signInModeBtn.Right + 8, _registerModeBtn.Top)

            _openConfigBtn.Width = AppTheme.MeasureButtonWidth(_openConfigBtn.Text, AppTheme.FontBodyBold, 126, 26)
            _openConfigBtn.Location = New Point(_configBanner.Width - _openConfigBtn.Width - 10, 16)
            _configMessageLabel.Width = _openConfigBtn.Left - 16

            _cancelButton.Width = AppTheme.MeasureButtonWidth(_cancelButton.Text, AppTheme.FontBodyBold, 96, 28)
            AppTheme.StyleSecondaryButton(_cancelButton)
            AppTheme.StyleSecondaryButton(_openConfigBtn)
            AppTheme.StylePrimaryButton(_submitButton)
        End Sub

        Private Sub UpdateModeUi()
            If _isRegisterMode Then
                AppTheme.StyleSecondaryButton(_signInModeBtn)
                AppTheme.StyleAccentButton(_registerModeBtn)
                _submitButton.Text = "Buat Akun"
            Else
                AppTheme.StyleAccentButton(_signInModeBtn)
                AppTheme.StyleSecondaryButton(_registerModeBtn)
                _submitButton.Text = "Masuk"
            End If

            _nameLabel.Visible = _isRegisterMode
            _nameTextBox.Visible = _isRegisterMode

            Dim topOffset = If(_showConfigBanner, 218, 146)
            If _isRegisterMode Then
                _nameLabel.Location = New Point(24, topOffset)
                _nameTextBox.Location = New Point(24, topOffset + 22)
                topOffset += 62
            End If

            _emailLabel.Location = New Point(24, topOffset)
            _emailTextBox.Location = New Point(24, topOffset + 22)
            _passwordLabel.Location = New Point(24, topOffset + 62)
            _passwordTextBox.Location = New Point(24, topOffset + 84)
            _showPasswordCheckBox.Location = New Point(24, topOffset + 118)
            _feedbackLabel.Location = New Point(24, topOffset + 148)

            Dim buttonsY = Math.Max(428, topOffset + 200)
            _submitButton.Width = AppTheme.MeasureButtonWidth(_submitButton.Text, AppTheme.FontBodyBold, 136, 28)
            _submitButton.Location = New Point(436 - _submitButton.Width, buttonsY)
            _cancelButton.Location = New Point(_submitButton.Left - _cancelButton.Width - 8, buttonsY)
            ClientSize = New Size(460, buttonsY + 58)
        End Sub

        Private Sub RefreshConfigBanner()
            Dim configured = SupabaseClientProvider.IsConfigured
            _showConfigBanner = Not configured
            _configBanner.Visible = _showConfigBanner
            _submitButton.Enabled = configured
            UpdateModeUi()
        End Sub

        Private Sub OnOpenConfigClick(sender As Object, e As EventArgs)
            Using dlg As New SupabaseConfigForm()
                dlg.ShowDialog(Me)
            End Using
            RefreshConfigBanner()
        End Sub

        Private Async Sub OnSubmitClick(sender As Object, e As EventArgs)
            If Not SupabaseClientProvider.IsConfigured Then
                ShowFeedback("Atur koneksi Supabase terlebih dahulu sebelum masuk.", AppTheme.WarningTint, AppTheme.WarningText)
                Return
            End If

            SetBusyState(True)
            ShowFeedback("Memproses autentikasi...", AppTheme.TaupeTint, AppTheme.TaupeText)

            Dim result As AuthResult
            If _isRegisterMode Then
                result = Await _authService.SignUpAsync(_nameTextBox.Text, _emailTextBox.Text, _passwordTextBox.Text)
            Else
                result = Await _authService.SignInAsync(_emailTextBox.Text, _passwordTextBox.Text)
            End If

            If IsDisposed Then Return
            SetBusyState(False)

            If result.Success Then
                If result.RequiresEmailConfirmation Then
                    ShowFeedback(result.Message, AppTheme.SageTint, AppTheme.SageText)
                    _isRegisterMode = False
                    UpdateModeUi()
                Else
                    DialogResult = DialogResult.OK
                    Close()
                End If
            Else
                ShowFeedback(result.Message, AppTheme.ErrorTint, AppTheme.ErrorText)
            End If
        End Sub

        Private Sub SetBusyState(isBusy As Boolean)
            _signInModeBtn.Enabled = Not isBusy
            _registerModeBtn.Enabled = Not isBusy
            _nameTextBox.Enabled = Not isBusy
            _emailTextBox.Enabled = Not isBusy
            _passwordTextBox.Enabled = Not isBusy
            _showPasswordCheckBox.Enabled = Not isBusy
            _cancelButton.Enabled = Not isBusy
            _submitButton.Enabled = Not isBusy AndAlso SupabaseClientProvider.IsConfigured
            _submitButton.Text = If(isBusy, "Memproses...", If(_isRegisterMode, "Buat Akun", "Masuk"))
        End Sub

        Private Sub ShowFeedback(message As String, backColor As Color, foreColor As Color)
            _feedbackLabel.Text = message
            _feedbackLabel.BackColor = backColor
            _feedbackLabel.ForeColor = foreColor
            _feedbackLabel.Visible = True
        End Sub

        Private Sub ClearFeedback()
            _feedbackLabel.Visible = False
            _feedbackLabel.Text = String.Empty
        End Sub
    End Class
End Namespace
