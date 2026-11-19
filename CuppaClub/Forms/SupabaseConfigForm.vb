Option Strict On
Option Explicit On

Imports CuppaClub.Core
Imports CuppaClub.Services

Namespace Forms
    Public Class SupabaseConfigForm
        Inherits Form

        Private ReadOnly _sourceValueLabel As New Label()
        Private ReadOnly _urlTextBox As New TextBox()
        Private ReadOnly _keyTextBox As New TextBox()
        Private ReadOnly _maskKeyCheckBox As New CheckBox()
        Private ReadOnly _statusLabel As New Label()
        Private ReadOnly _saveButton As New Button()
        Private ReadOnly _cancelButton As New Button()

        Public Sub New()
            Text = "Koneksi Supabase - CuppaClub"
            FormBorderStyle = FormBorderStyle.FixedDialog
            MaximizeBox = False
            MinimizeBox = False
            StartPosition = FormStartPosition.CenterParent
            ClientSize = New Size(540, 434)
            BackColor = AppTheme.CanvasCream
            ForeColor = AppTheme.TextPrimary
            Font = AppTheme.FontBody
            KeyPreview = True

            BuildUi()
            LoadCurrentSettings()
        End Sub

        Private Sub BuildUi()
            Dim container As New Panel With {
                .Dock = DockStyle.Fill,
                .Padding = New Padding(24)
            }

            Dim titleLabel As New Label With {
                .Text = "Pengaturan Koneksi Supabase",
                .Font = AppTheme.FontPageTitle,
                .ForeColor = AppTheme.TextPrimary,
                .Location = New Point(24, 16),
                .AutoSize = True
            }

            Dim subtitleLabel As New Label With {
                .Text = "Masukkan Project URL dan Publishable / Anon Key. Konfigurasi disimpan secara lokal di komputer ini dan tidak pernah di-commit ke Git.",
                .Font = AppTheme.FontCaption,
                .ForeColor = AppTheme.TextSecondary,
                .Location = New Point(24, 56),
                .MaximumSize = New Size(492, 0),
                .AutoSize = True
            }

            Dim sourceTitle As New Label With {
                .Text = "Sumber aktif:",
                .Font = AppTheme.FontCaptionBold,
                .ForeColor = AppTheme.TextSecondary,
                .Location = New Point(24, 102),
                .AutoSize = True
            }

            _sourceValueLabel.Location = New Point(24 + sourceTitle.PreferredWidth + 8, 102)
            _sourceValueLabel.Font = AppTheme.FontCaption
            _sourceValueLabel.ForeColor = AppTheme.TextPrimary
            _sourceValueLabel.MaximumSize = New Size(492 - sourceTitle.PreferredWidth - 8, 0)
            _sourceValueLabel.AutoSize = True

            Dim urlLabel As New Label With {
                .Text = "Project URL Supabase",
                .Font = AppTheme.FontBodyBold,
                .ForeColor = AppTheme.TextPrimary,
                .Location = New Point(24, 136),
                .AutoSize = True
            }

            _urlTextBox.Location = New Point(24, 158)
            _urlTextBox.Size = New Size(492, 28)
            _urlTextBox.BackColor = AppTheme.SurfaceMilk
            _urlTextBox.ForeColor = AppTheme.TextPrimary
            _urlTextBox.PlaceholderText = "https://project-ref.supabase.co"
            _urlTextBox.TabIndex = 0

            Dim keyLabel As New Label With {
                .Text = "Publishable / Anon Key",
                .Font = AppTheme.FontBodyBold,
                .ForeColor = AppTheme.TextPrimary,
                .Location = New Point(24, 198),
                .AutoSize = True
            }

            _keyTextBox.Location = New Point(24, 220)
            _keyTextBox.Size = New Size(492, 28)
            _keyTextBox.BackColor = AppTheme.SurfaceMilk
            _keyTextBox.ForeColor = AppTheme.TextPrimary
            _keyTextBox.UseSystemPasswordChar = True
            _keyTextBox.PlaceholderText = "sb_publishable_... atau eyJ..."
            _keyTextBox.TabIndex = 1

            _maskKeyCheckBox.Text = "Tampilkan key"
            _maskKeyCheckBox.Location = New Point(24, 254)
            _maskKeyCheckBox.AutoSize = True
            _maskKeyCheckBox.Cursor = Cursors.Hand
            _maskKeyCheckBox.TabIndex = 2
            AddHandler _maskKeyCheckBox.CheckedChanged, Sub()
                                                            _keyTextBox.UseSystemPasswordChar = Not _maskKeyCheckBox.Checked
                                                        End Sub

            Dim securityNote As New Label With {
                .Text = "Keamanan: Jangan masukkan service_role atau sb_secret_ key di aplikasi desktop. Aplikasi otomatis menolak kunci tersebut.",
                .Font = AppTheme.FontCaption,
                .ForeColor = AppTheme.WarningText,
                .BackColor = AppTheme.WarningTint,
                .Location = New Point(24, 286),
                .Size = New Size(492, 42),
                .Padding = New Padding(10, 6, 10, 6)
            }

            _statusLabel.Location = New Point(24, 336)
            _statusLabel.Size = New Size(492, 34)
            _statusLabel.Font = AppTheme.FontCaptionBold
            _statusLabel.Visible = False

            _saveButton.Text = "Uji & Simpan"
            _saveButton.Size = New Size(AppTheme.MeasureButtonWidth(_saveButton.Text, AppTheme.FontBodyBold, 136, 28), 34)
            _saveButton.Location = New Point(516 - _saveButton.Width, 380)
            _saveButton.TabIndex = 3
            AppTheme.StylePrimaryButton(_saveButton)
            AddHandler _saveButton.Click, AddressOf OnSaveClick

            _cancelButton.Text = "Batal"
            _cancelButton.Size = New Size(AppTheme.MeasureButtonWidth(_cancelButton.Text, AppTheme.FontBodyBold, 100, 28), 34)
            _cancelButton.Location = New Point(_saveButton.Left - _cancelButton.Width - 8, 380)
            _cancelButton.DialogResult = DialogResult.Cancel
            _cancelButton.TabIndex = 4
            AppTheme.StyleSecondaryButton(_cancelButton)

            AcceptButton = _saveButton
            CancelButton = _cancelButton

            container.Controls.AddRange({
                titleLabel,
                subtitleLabel,
                sourceTitle,
                _sourceValueLabel,
                urlLabel,
                _urlTextBox,
                keyLabel,
                _keyTextBox,
                _maskKeyCheckBox,
                securityNote,
                _statusLabel,
                _cancelButton,
                _saveButton
            })
            Controls.Add(container)
        End Sub

        Private Sub LoadCurrentSettings()
            Dim current = AppConfig.Load()
            _sourceValueLabel.Text = current.ConfigSource
            _urlTextBox.Text = current.SupabaseUrl
            _keyTextBox.Text = current.SupabaseAnonKey
        End Sub

        Private Async Sub OnSaveClick(sender As Object, e As EventArgs)
            SetBusyState(True)
            ShowStatus("Menguji koneksi ke Supabase...", AppTheme.TaupeTint, AppTheme.TaupeText)

            Dim result = Await SupabaseClientProvider.TestAndSaveAsync(_urlTextBox.Text, _keyTextBox.Text)
            If IsDisposed Then Return

            SetBusyState(False)
            If result.Success Then
                ShowStatus(result.Message, AppTheme.SageTint, AppTheme.SageText)
                DialogResult = DialogResult.OK
                Close()
            Else
                ShowStatus(result.Message, AppTheme.ErrorTint, AppTheme.ErrorText)
            End If
        End Sub

        Private Sub SetBusyState(isBusy As Boolean)
            _urlTextBox.Enabled = Not isBusy
            _keyTextBox.Enabled = Not isBusy
            _maskKeyCheckBox.Enabled = Not isBusy
            _saveButton.Enabled = Not isBusy
            _cancelButton.Enabled = Not isBusy
            _saveButton.Text = If(isBusy, "Menguji...", "Uji & Simpan")
        End Sub

        Private Sub ShowStatus(message As String, backColor As Color, foreColor As Color)
            _statusLabel.Text = message
            _statusLabel.BackColor = backColor
            _statusLabel.ForeColor = foreColor
            _statusLabel.Padding = New Padding(8, 4, 8, 4)
            _statusLabel.Visible = True
        End Sub
    End Class
End Namespace
