Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports CuppaClub.Core
Imports CuppaClub.Services

Namespace Forms
    Public Class SupabaseConfigForm
        Inherits Form

        Public Sub New()
            InitializeComponent()

            If LicenseManager.UsageMode = LicenseUsageMode.Designtime OrElse DesignMode Then
                Return
            End If

            WireRuntimeBehavior()
            ApplyButtonSizing()
            LoadCurrentSettings()
        End Sub

        Private Sub WireRuntimeBehavior()
            _sourceValueLabel.Location = New Point(24 + _sourceTitleLabel.PreferredWidth + 8, 102)
            _sourceValueLabel.MaximumSize = New Size(492 - _sourceTitleLabel.PreferredWidth - 8, 0)

            AddHandler _maskKeyCheckBox.CheckedChanged, Sub()
                                                            _keyTextBox.UseSystemPasswordChar = Not _maskKeyCheckBox.Checked
                                                        End Sub
            AddHandler _saveButton.Click, AddressOf OnSaveClick
        End Sub

        Private Sub ApplyButtonSizing()
            _saveButton.Width = AppTheme.MeasureButtonWidth(_saveButton.Text, AppTheme.FontBodyBold, 136, 28)
            _saveButton.Location = New Point(516 - _saveButton.Width, 380)
            _cancelButton.Width = AppTheme.MeasureButtonWidth(_cancelButton.Text, AppTheme.FontBodyBold, 100, 28)
            _cancelButton.Location = New Point(_saveButton.Left - _cancelButton.Width - 8, 380)

            AppTheme.StylePrimaryButton(_saveButton)
            AppTheme.StyleSecondaryButton(_cancelButton)
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
