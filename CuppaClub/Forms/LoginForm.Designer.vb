Option Strict On
Option Explicit On

Namespace Forms
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class LoginForm
        Inherits System.Windows.Forms.Form

        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing Then
                    If components IsNot Nothing Then
                        components.Dispose()
                    End If
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        Private components As System.ComponentModel.IContainer

        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Me._titleLabel = New System.Windows.Forms.Label()
            Me._subtitleLabel = New System.Windows.Forms.Label()
            Me._signInModeBtn = New System.Windows.Forms.Button()
            Me._registerModeBtn = New System.Windows.Forms.Button()
            Me._configBanner = New System.Windows.Forms.Panel()
            Me._configMessageLabel = New System.Windows.Forms.Label()
            Me._openConfigBtn = New System.Windows.Forms.Button()
            Me._nameLabel = New System.Windows.Forms.Label()
            Me._nameTextBox = New System.Windows.Forms.TextBox()
            Me._emailLabel = New System.Windows.Forms.Label()
            Me._emailTextBox = New System.Windows.Forms.TextBox()
            Me._passwordLabel = New System.Windows.Forms.Label()
            Me._passwordTextBox = New System.Windows.Forms.TextBox()
            Me._showPasswordCheckBox = New System.Windows.Forms.CheckBox()
            Me._feedbackLabel = New System.Windows.Forms.Label()
            Me._cancelButton = New System.Windows.Forms.Button()
            Me._submitButton = New System.Windows.Forms.Button()
            Me._configBanner.SuspendLayout()
            Me.SuspendLayout()
            '
            '_titleLabel
            '
            Me._titleLabel.AutoSize = True
            Me._titleLabel.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._titleLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._titleLabel.Location = New System.Drawing.Point(24, 16)
            Me._titleLabel.Name = "_titleLabel"
            Me._titleLabel.Size = New System.Drawing.Size(218, 30)
            Me._titleLabel.TabIndex = 0
            Me._titleLabel.Text = "Masuk ke CuppaClub"
            '
            '_subtitleLabel
            '
            Me._subtitleLabel.AutoSize = True
            Me._subtitleLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._subtitleLabel.Location = New System.Drawing.Point(24, 56)
            Me._subtitleLabel.MaximumSize = New System.Drawing.Size(412, 0)
            Me._subtitleLabel.Name = "_subtitleLabel"
            Me._subtitleLabel.Size = New System.Drawing.Size(374, 15)
            Me._subtitleLabel.TabIndex = 1
            Me._subtitleLabel.Text = "Gunakan akun pelanggan atau akun staf yang terdaftar di Supabase Auth."
            '
            '_signInModeBtn
            '
            Me._signInModeBtn.BackColor = System.Drawing.Color.FromArgb(158, 61, 80)
            Me._signInModeBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._signInModeBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(158, 61, 80)
            Me._signInModeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._signInModeBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._signInModeBtn.ForeColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._signInModeBtn.Location = New System.Drawing.Point(24, 102)
            Me._signInModeBtn.Name = "_signInModeBtn"
            Me._signInModeBtn.Size = New System.Drawing.Size(116, 32)
            Me._signInModeBtn.TabIndex = 0
            Me._signInModeBtn.Text = "Masuk"
            Me._signInModeBtn.UseVisualStyleBackColor = False
            '
            '_registerModeBtn
            '
            Me._registerModeBtn.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._registerModeBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._registerModeBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 123, 101)
            Me._registerModeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._registerModeBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._registerModeBtn.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._registerModeBtn.Location = New System.Drawing.Point(148, 102)
            Me._registerModeBtn.Name = "_registerModeBtn"
            Me._registerModeBtn.Size = New System.Drawing.Size(128, 32)
            Me._registerModeBtn.TabIndex = 1
            Me._registerModeBtn.Text = "Daftar Akun"
            Me._registerModeBtn.UseVisualStyleBackColor = False
            '
            '_configBanner
            '
            Me._configBanner.BackColor = System.Drawing.Color.FromArgb(253, 243, 227)
            Me._configBanner.Controls.Add(Me._configMessageLabel)
            Me._configBanner.Controls.Add(Me._openConfigBtn)
            Me._configBanner.Location = New System.Drawing.Point(24, 144)
            Me._configBanner.Name = "_configBanner"
            Me._configBanner.Padding = New System.Windows.Forms.Padding(10, 8, 10, 8)
            Me._configBanner.Size = New System.Drawing.Size(412, 62)
            Me._configBanner.TabIndex = 2
            '
            '_configMessageLabel
            '
            Me._configMessageLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._configMessageLabel.ForeColor = System.Drawing.Color.FromArgb(122, 70, 14)
            Me._configMessageLabel.Location = New System.Drawing.Point(10, 8)
            Me._configMessageLabel.Name = "_configMessageLabel"
            Me._configMessageLabel.Size = New System.Drawing.Size(260, 44)
            Me._configMessageLabel.TabIndex = 0
            Me._configMessageLabel.Text = "Supabase belum dikonfigurasi. Atur koneksi terlebih dahulu."
            '
            '_openConfigBtn
            '
            Me._openConfigBtn.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._openConfigBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._openConfigBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 123, 101)
            Me._openConfigBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._openConfigBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._openConfigBtn.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._openConfigBtn.Location = New System.Drawing.Point(276, 16)
            Me._openConfigBtn.Name = "_openConfigBtn"
            Me._openConfigBtn.Size = New System.Drawing.Size(126, 30)
            Me._openConfigBtn.TabIndex = 2
            Me._openConfigBtn.Text = "Atur Koneksi"
            Me._openConfigBtn.UseVisualStyleBackColor = False
            '
            '_nameLabel
            '
            Me._nameLabel.AutoSize = True
            Me._nameLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._nameLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._nameLabel.Location = New System.Drawing.Point(24, 218)
            Me._nameLabel.Name = "_nameLabel"
            Me._nameLabel.Size = New System.Drawing.Size(100, 17)
            Me._nameLabel.TabIndex = 3
            Me._nameLabel.Text = "Nama Lengkap"
            '
            '_nameTextBox
            '
            Me._nameTextBox.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._nameTextBox.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._nameTextBox.Location = New System.Drawing.Point(24, 240)
            Me._nameTextBox.Name = "_nameTextBox"
            Me._nameTextBox.PlaceholderText = "Nama Anda"
            Me._nameTextBox.Size = New System.Drawing.Size(412, 24)
            Me._nameTextBox.TabIndex = 3
            '
            '_emailLabel
            '
            Me._emailLabel.AutoSize = True
            Me._emailLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._emailLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._emailLabel.Location = New System.Drawing.Point(24, 280)
            Me._emailLabel.Name = "_emailLabel"
            Me._emailLabel.Size = New System.Drawing.Size(42, 17)
            Me._emailLabel.TabIndex = 4
            Me._emailLabel.Text = "Email"
            '
            '_emailTextBox
            '
            Me._emailTextBox.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._emailTextBox.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._emailTextBox.Location = New System.Drawing.Point(24, 302)
            Me._emailTextBox.Name = "_emailTextBox"
            Me._emailTextBox.PlaceholderText = "nama@email.com"
            Me._emailTextBox.Size = New System.Drawing.Size(412, 24)
            Me._emailTextBox.TabIndex = 4
            '
            '_passwordLabel
            '
            Me._passwordLabel.AutoSize = True
            Me._passwordLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._passwordLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._passwordLabel.Location = New System.Drawing.Point(24, 342)
            Me._passwordLabel.Name = "_passwordLabel"
            Me._passwordLabel.Size = New System.Drawing.Size(74, 17)
            Me._passwordLabel.TabIndex = 5
            Me._passwordLabel.Text = "Kata Sandi"
            '
            '_passwordTextBox
            '
            Me._passwordTextBox.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._passwordTextBox.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._passwordTextBox.Location = New System.Drawing.Point(24, 364)
            Me._passwordTextBox.Name = "_passwordTextBox"
            Me._passwordTextBox.PlaceholderText = "Minimal 6 karakter"
            Me._passwordTextBox.Size = New System.Drawing.Size(412, 24)
            Me._passwordTextBox.TabIndex = 5
            Me._passwordTextBox.UseSystemPasswordChar = True
            '
            '_showPasswordCheckBox
            '
            Me._showPasswordCheckBox.AutoSize = True
            Me._showPasswordCheckBox.Cursor = System.Windows.Forms.Cursors.Hand
            Me._showPasswordCheckBox.Location = New System.Drawing.Point(24, 398)
            Me._showPasswordCheckBox.Name = "_showPasswordCheckBox"
            Me._showPasswordCheckBox.Size = New System.Drawing.Size(148, 21)
            Me._showPasswordCheckBox.TabIndex = 6
            Me._showPasswordCheckBox.Text = "Tampilkan kata sandi"
            '
            '_feedbackLabel
            '
            Me._feedbackLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._feedbackLabel.Location = New System.Drawing.Point(24, 428)
            Me._feedbackLabel.Name = "_feedbackLabel"
            Me._feedbackLabel.Padding = New System.Windows.Forms.Padding(10, 6, 10, 6)
            Me._feedbackLabel.Size = New System.Drawing.Size(412, 42)
            Me._feedbackLabel.TabIndex = 7
            Me._feedbackLabel.Visible = False
            '
            '_cancelButton
            '
            Me._cancelButton.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._cancelButton.Cursor = System.Windows.Forms.Cursors.Hand
            Me._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me._cancelButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 123, 101)
            Me._cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._cancelButton.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._cancelButton.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._cancelButton.Location = New System.Drawing.Point(196, 480)
            Me._cancelButton.Name = "_cancelButton"
            Me._cancelButton.Size = New System.Drawing.Size(96, 36)
            Me._cancelButton.TabIndex = 8
            Me._cancelButton.Text = "Tutup"
            Me._cancelButton.UseVisualStyleBackColor = False
            '
            '_submitButton
            '
            Me._submitButton.BackColor = System.Drawing.Color.FromArgb(107, 68, 46)
            Me._submitButton.Cursor = System.Windows.Forms.Cursors.Hand
            Me._submitButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(107, 68, 46)
            Me._submitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._submitButton.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._submitButton.ForeColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._submitButton.Location = New System.Drawing.Point(300, 480)
            Me._submitButton.Name = "_submitButton"
            Me._submitButton.Size = New System.Drawing.Size(136, 36)
            Me._submitButton.TabIndex = 7
            Me._submitButton.Text = "Masuk"
            Me._submitButton.UseVisualStyleBackColor = False
            '
            'LoginForm
            '
            Me.AcceptButton = Me._submitButton
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me.CancelButton = Me._cancelButton
            Me.ClientSize = New System.Drawing.Size(460, 538)
            Me.Controls.Add(Me._titleLabel)
            Me.Controls.Add(Me._subtitleLabel)
            Me.Controls.Add(Me._signInModeBtn)
            Me.Controls.Add(Me._registerModeBtn)
            Me.Controls.Add(Me._configBanner)
            Me.Controls.Add(Me._nameLabel)
            Me.Controls.Add(Me._nameTextBox)
            Me.Controls.Add(Me._emailLabel)
            Me.Controls.Add(Me._emailTextBox)
            Me.Controls.Add(Me._passwordLabel)
            Me.Controls.Add(Me._passwordTextBox)
            Me.Controls.Add(Me._showPasswordCheckBox)
            Me.Controls.Add(Me._feedbackLabel)
            Me.Controls.Add(Me._cancelButton)
            Me.Controls.Add(Me._submitButton)
            Me.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.KeyPreview = True
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "LoginForm"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Akun CuppaClub"
            Me._configBanner.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub

        Friend WithEvents _titleLabel As System.Windows.Forms.Label
        Friend WithEvents _subtitleLabel As System.Windows.Forms.Label
        Friend WithEvents _signInModeBtn As System.Windows.Forms.Button
        Friend WithEvents _registerModeBtn As System.Windows.Forms.Button
        Friend WithEvents _configBanner As System.Windows.Forms.Panel
        Friend WithEvents _configMessageLabel As System.Windows.Forms.Label
        Friend WithEvents _openConfigBtn As System.Windows.Forms.Button
        Friend WithEvents _nameLabel As System.Windows.Forms.Label
        Friend WithEvents _nameTextBox As System.Windows.Forms.TextBox
        Friend WithEvents _emailLabel As System.Windows.Forms.Label
        Friend WithEvents _emailTextBox As System.Windows.Forms.TextBox
        Friend WithEvents _passwordLabel As System.Windows.Forms.Label
        Friend WithEvents _passwordTextBox As System.Windows.Forms.TextBox
        Friend WithEvents _showPasswordCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents _feedbackLabel As System.Windows.Forms.Label
        Friend WithEvents _cancelButton As System.Windows.Forms.Button
        Friend WithEvents _submitButton As System.Windows.Forms.Button
    End Class
End Namespace
