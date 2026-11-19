Option Strict On
Option Explicit On

Imports CuppaClub.Core
Imports CuppaClub.Services

Namespace Forms
    Public Class LoginForm
        Inherits Form

        Private ReadOnly _authService As New AuthService()
        Private _isRegisterMode As Boolean
        Private _showConfigBanner As Boolean

        Private ReadOnly _signInModeBtn As New Button()
        Private ReadOnly _registerModeBtn As New Button()
        Private ReadOnly _configBanner As New Panel()
        Private ReadOnly _configMessageLabel As New Label()
        Private ReadOnly _openConfigBtn As New Button()

        Private ReadOnly _nameLabel As New Label()
        Private ReadOnly _nameTextBox As New TextBox()
        Private ReadOnly _emailLabel As New Label()
        Private ReadOnly _emailTextBox As New TextBox()
        Private ReadOnly _passwordLabel As New Label()
        Private ReadOnly _passwordTextBox As New TextBox()
        Private ReadOnly _showPasswordCheckBox As New CheckBox()
        Private ReadOnly _feedbackLabel As New Label()
        Private ReadOnly _submitButton As New Button()
        Private ReadOnly _cancelButton As New Button()

        Public Sub New()
            Text = "Akun CuppaClub"
            FormBorderStyle = FormBorderStyle.FixedDialog
            MaximizeBox = False
            MinimizeBox = False
            StartPosition = FormStartPosition.CenterParent
            ClientSize = New Size(460, 490)
            BackColor = AppTheme.CanvasCream
            ForeColor = AppTheme.TextPrimary
            Font = AppTheme.FontBody
            KeyPreview = True

            BuildUi()
            UpdateModeUi()
            RefreshConfigBanner()
        End Sub

        Private Sub BuildUi()
            Dim titleLabel As New Label With {
                .Text = "Masuk ke CuppaClub",
                .Font = AppTheme.FontPageTitle,
                .ForeColor = AppTheme.TextPrimary,
                .Location = New Point(24, 16),
                .AutoSize = True
            }

            Dim subtitleLabel As New Label With {
                .Text = "Gunakan akun pelanggan atau akun staf yang terdaftar di Supabase Auth.",
                .Font = AppTheme.FontCaption,
                .ForeColor = AppTheme.TextSecondary,
                .Location = New Point(24, 56),
                .MaximumSize = New Size(412, 0),
                .AutoSize = True
            }

            _signInModeBtn.Text = "Masuk"
            _signInModeBtn.Location = New Point(24, 102)
            _signInModeBtn.Size = New Size(AppTheme.MeasureButtonWidth(_signInModeBtn.Text, AppTheme.FontBodyBold, 116, 28), 32)
            _signInModeBtn.TabIndex = 0
            AddHandler _signInModeBtn.Click, Sub()
                                                 _isRegisterMode = False
                                                 ClearFeedback()
                                                 UpdateModeUi()
                                             End Sub

            _registerModeBtn.Text = "Daftar Akun"
            _registerModeBtn.Location = New Point(_signInModeBtn.Right + 8, 102)
            _registerModeBtn.Size = New Size(AppTheme.MeasureButtonWidth(_registerModeBtn.Text, AppTheme.FontBodyBold, 128, 28), 32)
            _registerModeBtn.TabIndex = 1
            AddHandler _registerModeBtn.Click, Sub()
                                                   _isRegisterMode = True
                                                   ClearFeedback()
                                                   UpdateModeUi()
                                               End Sub

            _configBanner.Location = New Point(24, 144)
            _configBanner.Size = New Size(412, 62)
            _configBanner.BackColor = AppTheme.WarningTint
            _configBanner.Padding = New Padding(10, 8, 10, 8)

            _openConfigBtn.Text = "Atur Koneksi"
            _openConfigBtn.Size = New Size(AppTheme.MeasureButtonWidth(_openConfigBtn.Text, AppTheme.FontBodyBold, 126, 26), 30)
            _openConfigBtn.Location = New Point(_configBanner.Width - _openConfigBtn.Width - 10, 16)
            _openConfigBtn.TabIndex = 2
            AppTheme.StyleSecondaryButton(_openConfigBtn)
            AddHandler _openConfigBtn.Click, AddressOf OnOpenConfigClick

            _configMessageLabel.Text = "Supabase belum dikonfigurasi. Atur koneksi terlebih dahulu."
            _configMessageLabel.Font = AppTheme.FontCaptionBold
            _configMessageLabel.ForeColor = AppTheme.WarningText
            _configMessageLabel.Location = New Point(10, 8)
            _configMessageLabel.Size = New Size(_openConfigBtn.Left - 16, 44)

            _configBanner.Controls.Add(_configMessageLabel)
            _configBanner.Controls.Add(_openConfigBtn)

            _nameLabel.Text = "Nama Lengkap"
            _nameLabel.Font = AppTheme.FontBodyBold
            _nameLabel.ForeColor = AppTheme.TextPrimary
            _nameLabel.Location = New Point(24, 202)
            _nameLabel.AutoSize = True

            _nameTextBox.Location = New Point(24, 224)
            _nameTextBox.Size = New Size(412, 28)
            _nameTextBox.BackColor = AppTheme.SurfaceMilk
            _nameTextBox.ForeColor = AppTheme.TextPrimary
            _nameTextBox.PlaceholderText = "Nama Anda"
            _nameTextBox.TabIndex = 3

            _emailLabel.Text = "Email"
            _emailLabel.Font = AppTheme.FontBodyBold
            _emailLabel.ForeColor = AppTheme.TextPrimary
            _emailLabel.AutoSize = True

            _emailTextBox.Size = New Size(412, 28)
            _emailTextBox.BackColor = AppTheme.SurfaceMilk
            _emailTextBox.ForeColor = AppTheme.TextPrimary
            _emailTextBox.PlaceholderText = "nama@email.com"
            _emailTextBox.TabIndex = 4

            _passwordLabel.Text = "Kata Sandi"
            _passwordLabel.Font = AppTheme.FontBodyBold
            _passwordLabel.ForeColor = AppTheme.TextPrimary
            _passwordLabel.AutoSize = True

            _passwordTextBox.Size = New Size(412, 28)
            _passwordTextBox.BackColor = AppTheme.SurfaceMilk
            _passwordTextBox.ForeColor = AppTheme.TextPrimary
            _passwordTextBox.UseSystemPasswordChar = True
            _passwordTextBox.PlaceholderText = "Minimal 6 karakter"
            _passwordTextBox.TabIndex = 5

            _showPasswordCheckBox.Text = "Tampilkan kata sandi"
            _showPasswordCheckBox.AutoSize = True
            _showPasswordCheckBox.Cursor = Cursors.Hand
            _showPasswordCheckBox.TabIndex = 6
            AddHandler _showPasswordCheckBox.CheckedChanged, Sub()
                                                                 _passwordTextBox.UseSystemPasswordChar = Not _showPasswordCheckBox.Checked
                                                             End Sub

            _feedbackLabel.Size = New Size(412, 42)
            _feedbackLabel.Font = AppTheme.FontCaptionBold
            _feedbackLabel.Padding = New Padding(10, 6, 10, 6)
            _feedbackLabel.Visible = False

            _cancelButton.Text = "Tutup"
            _cancelButton.Size = New Size(AppTheme.MeasureButtonWidth(_cancelButton.Text, AppTheme.FontBodyBold, 96, 28), 36)
            _cancelButton.DialogResult = DialogResult.Cancel
            _cancelButton.TabIndex = 8
            AppTheme.StyleSecondaryButton(_cancelButton)

            _submitButton.Size = New Size(AppTheme.MeasureButtonWidth("Buat Akun", AppTheme.FontBodyBold, 136, 28), 36)
            _submitButton.TabIndex = 7
            AppTheme.StylePrimaryButton(_submitButton)
            AddHandler _submitButton.Click, AddressOf OnSubmitClick

            AcceptButton = _submitButton
            CancelButton = _cancelButton

            Controls.AddRange({
                titleLabel,
                subtitleLabel,
                _signInModeBtn,
                _registerModeBtn,
                _configBanner,
                _nameLabel,
                _nameTextBox,
                _emailLabel,
                _emailTextBox,
                _passwordLabel,
                _passwordTextBox,
                _showPasswordCheckBox,
                _feedbackLabel,
                _cancelButton,
                _submitButton
            })
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
