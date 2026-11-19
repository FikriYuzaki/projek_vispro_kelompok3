Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports CuppaClub.Core
Imports CuppaClub.Models
Imports CuppaClub.Services

Namespace Forms
    Public Class MenuItemEditorForm
        Inherits Form

        Private ReadOnly _menuService As New MenuService()
        Private _editingId As String

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SavedItem As MenuItem

        Public Sub New()
            InitializeComponent()

            If LicenseManager.UsageMode = LicenseUsageMode.Designtime OrElse DesignMode Then
                Return
            End If

            WireRuntimeBehavior()
            ApplyButtonSizing()
        End Sub

        Public Sub New(existingItem As MenuItem)
            Me.New()
            If existingItem IsNot Nothing Then
                ConfigureEditMode(existingItem)
            End If
        End Sub

        Private Sub WireRuntimeBehavior()
            _categoryCombo.Items.Clear()
            _categoryCombo.Items.AddRange(MenuItem.Categories)
            If _categoryCombo.Items.Count > 0 Then
                _categoryCombo.SelectedIndex = 0
            End If

            AddHandler _uploadPhotoBtn.Click, AddressOf OnUploadPhotoClick
            AddHandler _saveButton.Click, AddressOf OnSaveClick
        End Sub

        Private Sub ApplyButtonSizing()
            _uploadPhotoBtn.Width = AppTheme.MeasureButtonWidth(_uploadPhotoBtn.Text, AppTheme.FontBodyBold, 140, 28)
            _uploadPhotoBtn.Location = New Point(496 - _uploadPhotoBtn.Width, 355)
            _imageUrlTextBox.Width = _uploadPhotoBtn.Left - 32

            _saveButton.Width = AppTheme.MeasureButtonWidth(_saveButton.Text, AppTheme.FontBodyBold, 164, 28)
            _saveButton.Location = New Point(496 - _saveButton.Width, 508)
            _cancelButton.Width = AppTheme.MeasureButtonWidth(_cancelButton.Text, AppTheme.FontBodyBold, 100, 28)
            _cancelButton.Location = New Point(_saveButton.Left - _cancelButton.Width - 8, 508)

            AppTheme.StyleSecondaryButton(_uploadPhotoBtn)
            AppTheme.StyleSecondaryButton(_cancelButton)
            AppTheme.StylePrimaryButton(_saveButton)
        End Sub

        Private Sub ConfigureEditMode(existingItem As MenuItem)
            _editingId = existingItem.Id
            Text = "Edit Item Menu - CuppaClub"
            _titleLabel.Text = "Perbarui Detail Menu"
            _saveButton.Text = "Simpan Perubahan"
            ApplyButtonSizing()
            PopulateFields(existingItem)
        End Sub

        Private Sub PopulateFields(item As MenuItem)
            _nameTextBox.Text = item.Name
            Dim catIdx = Array.FindIndex(MenuItem.Categories, Function(c) String.Equals(c, item.Category, StringComparison.OrdinalIgnoreCase))
            _categoryCombo.SelectedIndex = If(catIdx >= 0, catIdx, 0)
            _priceNumeric.Value = Math.Max(_priceNumeric.Minimum, Math.Min(_priceNumeric.Maximum, item.BasePrice))
            _descriptionTextBox.Text = item.Description
            _imageUrlTextBox.Text = item.ImageUrl
            _availableCheckBox.Checked = item.IsAvailable
            _customizableCheckBox.Checked = item.SupportsCustomization
        End Sub

        Private Async Sub OnUploadPhotoClick(sender As Object, e As EventArgs)
            Using dialog As New OpenFileDialog()
                dialog.Title = "Pilih Foto Produk CuppaClub"
                dialog.Filter = "Gambar (*.jpg;*.jpeg;*.png;*.webp)|*.jpg;*.jpeg;*.png;*.webp"
                dialog.Multiselect = False

                If dialog.ShowDialog(Me) <> DialogResult.OK Then
                    Return
                End If

                SetBusyState(True, "Mengunggah foto ke Supabase Storage...")
                Try
                    Dim publicUrl = Await _menuService.UploadMenuPhotoAsync(dialog.FileName)
                    If IsDisposed Then Return
                    _imageUrlTextBox.Text = publicUrl
                    SetBusyState(False, String.Empty)
                    ShowStatus("Foto berhasil diunggah ke bucket menu-photos.", AppTheme.SageTint, AppTheme.SageText)
                Catch ex As Exception
                    If IsDisposed Then Return
                    SetBusyState(False, String.Empty)
                    ShowStatus(AuthService.FormatException(ex, "Gagal mengunggah foto."), AppTheme.ErrorTint, AppTheme.ErrorText)
                End Try
            End Using
        End Sub

        Private Async Sub OnSaveClick(sender As Object, e As EventArgs)
            If String.IsNullOrWhiteSpace(_nameTextBox.Text) Then
                ShowStatus("Nama menu wajib diisi.", AppTheme.ErrorTint, AppTheme.ErrorText)
                _nameTextBox.Focus()
                Return
            End If

            Dim imageUrl = _imageUrlTextBox.Text.Trim()
            If Not String.IsNullOrEmpty(imageUrl) Then
                Dim parsedUri As Uri = Nothing
                If Not Uri.TryCreate(imageUrl, UriKind.Absolute, parsedUri) OrElse
                   (parsedUri.Scheme <> Uri.UriSchemeHttps AndAlso parsedUri.Scheme <> Uri.UriSchemeHttp) Then
                    ShowStatus("URL foto harus berupa tautan http/https yang valid atau kosong.", AppTheme.ErrorTint, AppTheme.ErrorText)
                    _imageUrlTextBox.Focus()
                    Return
                End If
            End If

            Dim model As New MenuItem With {
                .Id = _editingId,
                .Name = _nameTextBox.Text.Trim(),
                .Category = If(_categoryCombo.SelectedItem, "Coffee").ToString(),
                .BasePrice = _priceNumeric.Value,
                .Description = _descriptionTextBox.Text.Trim(),
                .ImageUrl = imageUrl,
                .IsAvailable = _availableCheckBox.Checked,
                .SupportsCustomization = _customizableCheckBox.Checked
            }

            SetBusyState(True, "Menyimpan perubahan ke Supabase...")
            Try
                If String.IsNullOrEmpty(_editingId) Then
                    SavedItem = Await _menuService.CreateMenuItemAsync(model)
                Else
                    SavedItem = Await _menuService.UpdateMenuItemAsync(model)
                End If

                If IsDisposed Then Return
                DialogResult = DialogResult.OK
                Close()
            Catch ex As Exception
                If IsDisposed Then Return
                SetBusyState(False, String.Empty)
                ShowStatus(AuthService.FormatException(ex, "Gagal menyimpan menu."), AppTheme.ErrorTint, AppTheme.ErrorText)
            End Try
        End Sub

        Private Sub SetBusyState(isBusy As Boolean, statusMessage As String)
            _nameTextBox.Enabled = Not isBusy
            _categoryCombo.Enabled = Not isBusy
            _priceNumeric.Enabled = Not isBusy
            _descriptionTextBox.Enabled = Not isBusy
            _imageUrlTextBox.Enabled = Not isBusy
            _uploadPhotoBtn.Enabled = Not isBusy
            _availableCheckBox.Enabled = Not isBusy
            _customizableCheckBox.Enabled = Not isBusy
            _cancelButton.Enabled = Not isBusy
            _saveButton.Enabled = Not isBusy
            _saveButton.Text = If(isBusy, "Menyimpan...", If(String.IsNullOrEmpty(_editingId), "Simpan Menu", "Simpan Perubahan"))

            If isBusy AndAlso Not String.IsNullOrWhiteSpace(statusMessage) Then
                ShowStatus(statusMessage, AppTheme.TaupeTint, AppTheme.TaupeText)
            End If
        End Sub

        Private Sub ShowStatus(message As String, backColor As Color, foreColor As Color)
            _statusLabel.Text = message
            _statusLabel.BackColor = backColor
            _statusLabel.ForeColor = foreColor
            _statusLabel.Visible = True
        End Sub
    End Class
End Namespace
