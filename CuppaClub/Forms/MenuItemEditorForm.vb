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
        Private ReadOnly _editingId As String

        Private ReadOnly _nameTextBox As New TextBox()
        Private ReadOnly _categoryCombo As New ComboBox()
        Private ReadOnly _priceNumeric As New NumericUpDown()
        Private ReadOnly _descriptionTextBox As New TextBox()
        Private ReadOnly _imageUrlTextBox As New TextBox()
        Private ReadOnly _uploadPhotoBtn As New Button()
        Private ReadOnly _availableCheckBox As New CheckBox()
        Private ReadOnly _customizableCheckBox As New CheckBox()
        Private ReadOnly _statusLabel As New Label()
        Private ReadOnly _saveButton As New Button()
        Private ReadOnly _cancelButton As New Button()

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SavedItem As MenuItem

        Public Sub New(Optional existingItem As MenuItem = Nothing)
            _editingId = If(existingItem IsNot Nothing, existingItem.Id, Nothing)

            Text = If(existingItem Is Nothing, "Tambah Item Menu - CuppaClub", "Edit Item Menu - CuppaClub")
            FormBorderStyle = FormBorderStyle.FixedDialog
            MaximizeBox = False
            MinimizeBox = False
            StartPosition = FormStartPosition.CenterParent
            ClientSize = New Size(520, 564)
            BackColor = AppTheme.CanvasCream
            ForeColor = AppTheme.TextPrimary
            Font = AppTheme.FontBody
            KeyPreview = True

            BuildUi(existingItem Is Nothing)
            If existingItem IsNot Nothing Then
                PopulateFields(existingItem)
            End If
        End Sub

        Private Sub BuildUi(isNew As Boolean)
            Dim titleLabel As New Label With {
                .Text = If(isNew, "Tambah Item Menu Baru", "Perbarui Detail Menu"),
                .Font = AppTheme.FontPageTitle,
                .ForeColor = AppTheme.TextPrimary,
                .Location = New Point(24, 16),
                .AutoSize = True
            }

            Dim subtitleLabel As New Label With {
                .Text = "Perubahan langsung disimpan ke tabel menu_items di Supabase sesuai izin staf.",
                .Font = AppTheme.FontCaption,
                .ForeColor = AppTheme.TextSecondary,
                .Location = New Point(24, 56),
                .MaximumSize = New Size(472, 0),
                .AutoSize = True
            }

            Dim nameLabel As New Label With {
                .Text = "Nama Menu",
                .Font = AppTheme.FontBodyBold,
                .Location = New Point(24, 102),
                .AutoSize = True
            }

            _nameTextBox.Location = New Point(24, 124)
            _nameTextBox.Size = New Size(472, 28)
            _nameTextBox.BackColor = AppTheme.SurfaceMilk
            _nameTextBox.PlaceholderText = "Contoh: Caramel Oat Latte"
            _nameTextBox.TabIndex = 0

            Dim categoryLabel As New Label With {
                .Text = "Kategori",
                .Font = AppTheme.FontBodyBold,
                .Location = New Point(24, 164),
                .AutoSize = True
            }

            _categoryCombo.Location = New Point(24, 186)
            _categoryCombo.Size = New Size(224, 28)
            _categoryCombo.DropDownStyle = ComboBoxStyle.DropDownList
            _categoryCombo.BackColor = AppTheme.SurfaceMilk
            _categoryCombo.Items.AddRange(MenuItem.Categories)
            _categoryCombo.SelectedIndex = 0
            _categoryCombo.TabIndex = 1

            Dim priceLabel As New Label With {
                .Text = "Harga Dasar (Rp)",
                .Font = AppTheme.FontBodyBold,
                .Location = New Point(268, 164),
                .AutoSize = True
            }

            _priceNumeric.Location = New Point(268, 186)
            _priceNumeric.Size = New Size(228, 28)
            _priceNumeric.BackColor = AppTheme.SurfaceMilk
            _priceNumeric.Minimum = 0D
            _priceNumeric.Maximum = 10000000D
            _priceNumeric.Increment = 1000D
            _priceNumeric.ThousandsSeparator = True
            _priceNumeric.Value = 25000D
            _priceNumeric.TabIndex = 2

            Dim descLabel As New Label With {
                .Text = "Deskripsi",
                .Font = AppTheme.FontBodyBold,
                .Location = New Point(24, 226),
                .AutoSize = True
            }

            _descriptionTextBox.Location = New Point(24, 248)
            _descriptionTextBox.Size = New Size(472, 74)
            _descriptionTextBox.Multiline = True
            _descriptionTextBox.AcceptsReturn = False
            _descriptionTextBox.ScrollBars = ScrollBars.Vertical
            _descriptionTextBox.BackColor = AppTheme.SurfaceMilk
            _descriptionTextBox.PlaceholderText = "Deskripsi singkat rasa, penyajian, atau bahan utama..."
            _descriptionTextBox.TabIndex = 3

            Dim imageLabel As New Label With {
                .Text = "Foto Produk (URL atau Unggah ke Supabase Storage)",
                .Font = AppTheme.FontBodyBold,
                .Location = New Point(24, 334),
                .AutoSize = True
            }

            _imageUrlTextBox.Location = New Point(24, 356)
            _imageUrlTextBox.Size = New Size(336, 28)
            _imageUrlTextBox.BackColor = AppTheme.SurfaceMilk
            _imageUrlTextBox.PlaceholderText = "https://... (opsional)"
            _imageUrlTextBox.TabIndex = 4

            _uploadPhotoBtn.Text = "Unggah Foto..."
            _uploadPhotoBtn.Location = New Point(368, 355)
            _uploadPhotoBtn.Size = New Size(128, 30)
            _uploadPhotoBtn.TabIndex = 5
            AppTheme.StyleSecondaryButton(_uploadPhotoBtn)
            AddHandler _uploadPhotoBtn.Click, AddressOf OnUploadPhotoClick

            _availableCheckBox.Text = "Tersedia untuk dipesan"
            _availableCheckBox.Location = New Point(24, 398)
            _availableCheckBox.AutoSize = True
            _availableCheckBox.Checked = True
            _availableCheckBox.Cursor = Cursors.Hand
            _availableCheckBox.TabIndex = 6

            _customizableCheckBox.Text = "Mendukung kustomisasi (My Drink Creator)"
            _customizableCheckBox.Location = New Point(24, 426)
            _customizableCheckBox.AutoSize = True
            _customizableCheckBox.Cursor = Cursors.Hand
            _customizableCheckBox.TabIndex = 7

            _statusLabel.Location = New Point(24, 460)
            _statusLabel.Size = New Size(472, 38)
            _statusLabel.Font = AppTheme.FontCaptionBold
            _statusLabel.Padding = New Padding(10, 6, 10, 6)
            _statusLabel.Visible = False

            _cancelButton.Text = "Batal"
            _cancelButton.Location = New Point(268, 508)
            _cancelButton.Size = New Size(104, 36)
            _cancelButton.DialogResult = DialogResult.Cancel
            _cancelButton.TabIndex = 9
            AppTheme.StyleSecondaryButton(_cancelButton)

            _saveButton.Text = If(isNew, "Simpan Menu", "Simpan Perubahan")
            _saveButton.Location = New Point(380, 508)
            _saveButton.Size = New Size(116, 36)
            _saveButton.TabIndex = 8
            AppTheme.StylePrimaryButton(_saveButton)
            AddHandler _saveButton.Click, AddressOf OnSaveClick

            AcceptButton = _saveButton
            CancelButton = _cancelButton

            Controls.AddRange({
                titleLabel,
                subtitleLabel,
                nameLabel,
                _nameTextBox,
                categoryLabel,
                _categoryCombo,
                priceLabel,
                _priceNumeric,
                descLabel,
                _descriptionTextBox,
                imageLabel,
                _imageUrlTextBox,
                _uploadPhotoBtn,
                _availableCheckBox,
                _customizableCheckBox,
                _statusLabel,
                _cancelButton,
                _saveButton
            })
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
