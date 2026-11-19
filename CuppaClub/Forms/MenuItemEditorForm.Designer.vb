Option Strict On
Option Explicit On

Namespace Forms
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class MenuItemEditorForm
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
            Me._nameLabel = New System.Windows.Forms.Label()
            Me._nameTextBox = New System.Windows.Forms.TextBox()
            Me._categoryLabel = New System.Windows.Forms.Label()
            Me._categoryCombo = New System.Windows.Forms.ComboBox()
            Me._priceLabel = New System.Windows.Forms.Label()
            Me._priceNumeric = New System.Windows.Forms.NumericUpDown()
            Me._descLabel = New System.Windows.Forms.Label()
            Me._descriptionTextBox = New System.Windows.Forms.TextBox()
            Me._imageLabel = New System.Windows.Forms.Label()
            Me._imageUrlTextBox = New System.Windows.Forms.TextBox()
            Me._uploadPhotoBtn = New System.Windows.Forms.Button()
            Me._availableCheckBox = New System.Windows.Forms.CheckBox()
            Me._customizableCheckBox = New System.Windows.Forms.CheckBox()
            Me._statusLabel = New System.Windows.Forms.Label()
            Me._cancelButton = New System.Windows.Forms.Button()
            Me._saveButton = New System.Windows.Forms.Button()
            CType(Me._priceNumeric, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            '_titleLabel
            '
            Me._titleLabel.AutoSize = True
            Me._titleLabel.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._titleLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._titleLabel.Location = New System.Drawing.Point(24, 16)
            Me._titleLabel.Name = "_titleLabel"
            Me._titleLabel.Size = New System.Drawing.Size(252, 30)
            Me._titleLabel.TabIndex = 0
            Me._titleLabel.Text = "Tambah Item Menu Baru"
            '
            '_subtitleLabel
            '
            Me._subtitleLabel.AutoSize = True
            Me._subtitleLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._subtitleLabel.Location = New System.Drawing.Point(24, 56)
            Me._subtitleLabel.MaximumSize = New System.Drawing.Size(472, 0)
            Me._subtitleLabel.Name = "_subtitleLabel"
            Me._subtitleLabel.Size = New System.Drawing.Size(420, 15)
            Me._subtitleLabel.TabIndex = 1
            Me._subtitleLabel.Text = "Perubahan langsung disimpan ke tabel menu_items di Supabase sesuai izin staf."
            '
            '_nameLabel
            '
            Me._nameLabel.AutoSize = True
            Me._nameLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._nameLabel.Location = New System.Drawing.Point(24, 102)
            Me._nameLabel.Name = "_nameLabel"
            Me._nameLabel.Size = New System.Drawing.Size(86, 17)
            Me._nameLabel.TabIndex = 2
            Me._nameLabel.Text = "Nama Menu"
            '
            '_nameTextBox
            '
            Me._nameTextBox.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._nameTextBox.Location = New System.Drawing.Point(24, 124)
            Me._nameTextBox.Name = "_nameTextBox"
            Me._nameTextBox.PlaceholderText = "Contoh: Caramel Oat Latte"
            Me._nameTextBox.Size = New System.Drawing.Size(472, 24)
            Me._nameTextBox.TabIndex = 0
            '
            '_categoryLabel
            '
            Me._categoryLabel.AutoSize = True
            Me._categoryLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._categoryLabel.Location = New System.Drawing.Point(24, 164)
            Me._categoryLabel.Name = "_categoryLabel"
            Me._categoryLabel.Size = New System.Drawing.Size(61, 17)
            Me._categoryLabel.TabIndex = 3
            Me._categoryLabel.Text = "Kategori"
            '
            '_categoryCombo
            '
            Me._categoryCombo.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._categoryCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me._categoryCombo.FormattingEnabled = True
            Me._categoryCombo.Items.AddRange(New Object() {"Coffee", "Non-Coffee", "Pastry", "Dessert"})
            Me._categoryCombo.Location = New System.Drawing.Point(24, 186)
            Me._categoryCombo.Name = "_categoryCombo"
            Me._categoryCombo.Size = New System.Drawing.Size(224, 25)
            Me._categoryCombo.TabIndex = 1
            '
            '_priceLabel
            '
            Me._priceLabel.AutoSize = True
            Me._priceLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._priceLabel.Location = New System.Drawing.Point(268, 164)
            Me._priceLabel.Name = "_priceLabel"
            Me._priceLabel.Size = New System.Drawing.Size(114, 17)
            Me._priceLabel.TabIndex = 4
            Me._priceLabel.Text = "Harga Dasar (Rp)"
            '
            '_priceNumeric
            '
            Me._priceNumeric.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._priceNumeric.Increment = New Decimal(New Integer() {1000, 0, 0, 0})
            Me._priceNumeric.Location = New System.Drawing.Point(268, 186)
            Me._priceNumeric.Maximum = New Decimal(New Integer() {10000000, 0, 0, 0})
            Me._priceNumeric.Name = "_priceNumeric"
            Me._priceNumeric.Size = New System.Drawing.Size(228, 24)
            Me._priceNumeric.TabIndex = 2
            Me._priceNumeric.ThousandsSeparator = True
            Me._priceNumeric.Value = New Decimal(New Integer() {25000, 0, 0, 0})
            '
            '_descLabel
            '
            Me._descLabel.AutoSize = True
            Me._descLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._descLabel.Location = New System.Drawing.Point(24, 226)
            Me._descLabel.Name = "_descLabel"
            Me._descLabel.Size = New System.Drawing.Size(66, 17)
            Me._descLabel.TabIndex = 5
            Me._descLabel.Text = "Deskripsi"
            '
            '_descriptionTextBox
            '
            Me._descriptionTextBox.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._descriptionTextBox.Location = New System.Drawing.Point(24, 248)
            Me._descriptionTextBox.Multiline = True
            Me._descriptionTextBox.Name = "_descriptionTextBox"
            Me._descriptionTextBox.PlaceholderText = "Deskripsi singkat rasa, penyajian, atau bahan utama..."
            Me._descriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            Me._descriptionTextBox.Size = New System.Drawing.Size(472, 74)
            Me._descriptionTextBox.TabIndex = 3
            '
            '_imageLabel
            '
            Me._imageLabel.AutoSize = True
            Me._imageLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._imageLabel.Location = New System.Drawing.Point(24, 334)
            Me._imageLabel.Name = "_imageLabel"
            Me._imageLabel.Size = New System.Drawing.Size(324, 17)
            Me._imageLabel.TabIndex = 6
            Me._imageLabel.Text = "Foto Produk (URL atau Unggah ke Supabase Storage)"
            '
            '_imageUrlTextBox
            '
            Me._imageUrlTextBox.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._imageUrlTextBox.Location = New System.Drawing.Point(24, 356)
            Me._imageUrlTextBox.Name = "_imageUrlTextBox"
            Me._imageUrlTextBox.PlaceholderText = "https://... (opsional)"
            Me._imageUrlTextBox.Size = New System.Drawing.Size(324, 24)
            Me._imageUrlTextBox.TabIndex = 4
            '
            '_uploadPhotoBtn
            '
            Me._uploadPhotoBtn.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._uploadPhotoBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._uploadPhotoBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 123, 101)
            Me._uploadPhotoBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._uploadPhotoBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._uploadPhotoBtn.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._uploadPhotoBtn.Location = New System.Drawing.Point(356, 355)
            Me._uploadPhotoBtn.Name = "_uploadPhotoBtn"
            Me._uploadPhotoBtn.Size = New System.Drawing.Size(140, 30)
            Me._uploadPhotoBtn.TabIndex = 5
            Me._uploadPhotoBtn.Text = "Unggah Foto..."
            Me._uploadPhotoBtn.UseVisualStyleBackColor = False
            '
            '_availableCheckBox
            '
            Me._availableCheckBox.AutoSize = True
            Me._availableCheckBox.Checked = True
            Me._availableCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
            Me._availableCheckBox.Cursor = System.Windows.Forms.Cursors.Hand
            Me._availableCheckBox.Location = New System.Drawing.Point(24, 398)
            Me._availableCheckBox.Name = "_availableCheckBox"
            Me._availableCheckBox.Size = New System.Drawing.Size(168, 21)
            Me._availableCheckBox.TabIndex = 6
            Me._availableCheckBox.Text = "Tersedia untuk dipesan"
            '
            '_customizableCheckBox
            '
            Me._customizableCheckBox.AutoSize = True
            Me._customizableCheckBox.Cursor = System.Windows.Forms.Cursors.Hand
            Me._customizableCheckBox.Location = New System.Drawing.Point(24, 426)
            Me._customizableCheckBox.Name = "_customizableCheckBox"
            Me._customizableCheckBox.Size = New System.Drawing.Size(270, 21)
            Me._customizableCheckBox.TabIndex = 7
            Me._customizableCheckBox.Text = "Mendukung kustomisasi (My Drink Creator)"
            '
            '_statusLabel
            '
            Me._statusLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._statusLabel.Location = New System.Drawing.Point(24, 460)
            Me._statusLabel.Name = "_statusLabel"
            Me._statusLabel.Padding = New System.Windows.Forms.Padding(10, 6, 10, 6)
            Me._statusLabel.Size = New System.Drawing.Size(472, 38)
            Me._statusLabel.TabIndex = 8
            Me._statusLabel.Visible = False
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
            Me._cancelButton.Location = New System.Drawing.Point(224, 508)
            Me._cancelButton.Name = "_cancelButton"
            Me._cancelButton.Size = New System.Drawing.Size(100, 36)
            Me._cancelButton.TabIndex = 9
            Me._cancelButton.Text = "Batal"
            Me._cancelButton.UseVisualStyleBackColor = False
            '
            '_saveButton
            '
            Me._saveButton.BackColor = System.Drawing.Color.FromArgb(107, 68, 46)
            Me._saveButton.Cursor = System.Windows.Forms.Cursors.Hand
            Me._saveButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(107, 68, 46)
            Me._saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._saveButton.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._saveButton.ForeColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._saveButton.Location = New System.Drawing.Point(332, 508)
            Me._saveButton.Name = "_saveButton"
            Me._saveButton.Size = New System.Drawing.Size(164, 36)
            Me._saveButton.TabIndex = 8
            Me._saveButton.Text = "Simpan Menu"
            Me._saveButton.UseVisualStyleBackColor = False
            '
            'MenuItemEditorForm
            '
            Me.AcceptButton = Me._saveButton
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me.CancelButton = Me._cancelButton
            Me.ClientSize = New System.Drawing.Size(520, 564)
            Me.Controls.Add(Me._titleLabel)
            Me.Controls.Add(Me._subtitleLabel)
            Me.Controls.Add(Me._nameLabel)
            Me.Controls.Add(Me._nameTextBox)
            Me.Controls.Add(Me._categoryLabel)
            Me.Controls.Add(Me._categoryCombo)
            Me.Controls.Add(Me._priceLabel)
            Me.Controls.Add(Me._priceNumeric)
            Me.Controls.Add(Me._descLabel)
            Me.Controls.Add(Me._descriptionTextBox)
            Me.Controls.Add(Me._imageLabel)
            Me.Controls.Add(Me._imageUrlTextBox)
            Me.Controls.Add(Me._uploadPhotoBtn)
            Me.Controls.Add(Me._availableCheckBox)
            Me.Controls.Add(Me._customizableCheckBox)
            Me.Controls.Add(Me._statusLabel)
            Me.Controls.Add(Me._cancelButton)
            Me.Controls.Add(Me._saveButton)
            Me.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.KeyPreview = True
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "MenuItemEditorForm"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Tambah Item Menu - CuppaClub"
            CType(Me._priceNumeric, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()
        End Sub

        Friend WithEvents _titleLabel As System.Windows.Forms.Label
        Friend WithEvents _subtitleLabel As System.Windows.Forms.Label
        Friend WithEvents _nameLabel As System.Windows.Forms.Label
        Friend WithEvents _nameTextBox As System.Windows.Forms.TextBox
        Friend WithEvents _categoryLabel As System.Windows.Forms.Label
        Friend WithEvents _categoryCombo As System.Windows.Forms.ComboBox
        Friend WithEvents _priceLabel As System.Windows.Forms.Label
        Friend WithEvents _priceNumeric As System.Windows.Forms.NumericUpDown
        Friend WithEvents _descLabel As System.Windows.Forms.Label
        Friend WithEvents _descriptionTextBox As System.Windows.Forms.TextBox
        Friend WithEvents _imageLabel As System.Windows.Forms.Label
        Friend WithEvents _imageUrlTextBox As System.Windows.Forms.TextBox
        Friend WithEvents _uploadPhotoBtn As System.Windows.Forms.Button
        Friend WithEvents _availableCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents _customizableCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents _statusLabel As System.Windows.Forms.Label
        Friend WithEvents _cancelButton As System.Windows.Forms.Button
        Friend WithEvents _saveButton As System.Windows.Forms.Button
    End Class
End Namespace
