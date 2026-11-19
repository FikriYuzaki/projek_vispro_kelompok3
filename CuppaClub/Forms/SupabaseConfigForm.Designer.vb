Option Strict On
Option Explicit On

Namespace Forms
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class SupabaseConfigForm
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
            Me._containerPanel = New System.Windows.Forms.Panel()
            Me._titleLabel = New System.Windows.Forms.Label()
            Me._subtitleLabel = New System.Windows.Forms.Label()
            Me._sourceTitleLabel = New System.Windows.Forms.Label()
            Me._sourceValueLabel = New System.Windows.Forms.Label()
            Me._urlLabel = New System.Windows.Forms.Label()
            Me._urlTextBox = New System.Windows.Forms.TextBox()
            Me._keyLabel = New System.Windows.Forms.Label()
            Me._keyTextBox = New System.Windows.Forms.TextBox()
            Me._maskKeyCheckBox = New System.Windows.Forms.CheckBox()
            Me._securityNoteLabel = New System.Windows.Forms.Label()
            Me._statusLabel = New System.Windows.Forms.Label()
            Me._cancelButton = New System.Windows.Forms.Button()
            Me._saveButton = New System.Windows.Forms.Button()
            Me._containerPanel.SuspendLayout()
            Me.SuspendLayout()
            '
            '_containerPanel
            '
            Me._containerPanel.Controls.Add(Me._titleLabel)
            Me._containerPanel.Controls.Add(Me._subtitleLabel)
            Me._containerPanel.Controls.Add(Me._sourceTitleLabel)
            Me._containerPanel.Controls.Add(Me._sourceValueLabel)
            Me._containerPanel.Controls.Add(Me._urlLabel)
            Me._containerPanel.Controls.Add(Me._urlTextBox)
            Me._containerPanel.Controls.Add(Me._keyLabel)
            Me._containerPanel.Controls.Add(Me._keyTextBox)
            Me._containerPanel.Controls.Add(Me._maskKeyCheckBox)
            Me._containerPanel.Controls.Add(Me._securityNoteLabel)
            Me._containerPanel.Controls.Add(Me._statusLabel)
            Me._containerPanel.Controls.Add(Me._cancelButton)
            Me._containerPanel.Controls.Add(Me._saveButton)
            Me._containerPanel.Dock = System.Windows.Forms.DockStyle.Fill
            Me._containerPanel.Location = New System.Drawing.Point(0, 0)
            Me._containerPanel.Name = "_containerPanel"
            Me._containerPanel.Padding = New System.Windows.Forms.Padding(24)
            Me._containerPanel.Size = New System.Drawing.Size(540, 434)
            Me._containerPanel.TabIndex = 0
            '
            '_titleLabel
            '
            Me._titleLabel.AutoSize = True
            Me._titleLabel.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._titleLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._titleLabel.Location = New System.Drawing.Point(24, 16)
            Me._titleLabel.Name = "_titleLabel"
            Me._titleLabel.Size = New System.Drawing.Size(304, 30)
            Me._titleLabel.TabIndex = 0
            Me._titleLabel.Text = "Pengaturan Koneksi Supabase"
            '
            '_subtitleLabel
            '
            Me._subtitleLabel.AutoSize = True
            Me._subtitleLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._subtitleLabel.Location = New System.Drawing.Point(24, 56)
            Me._subtitleLabel.MaximumSize = New System.Drawing.Size(492, 0)
            Me._subtitleLabel.Name = "_subtitleLabel"
            Me._subtitleLabel.Size = New System.Drawing.Size(486, 30)
            Me._subtitleLabel.TabIndex = 1
            Me._subtitleLabel.Text = "Masukkan Project URL dan Publishable / Anon Key. Konfigurasi disimpan secara lokal di komputer ini dan tidak pernah di-commit ke Git."
            '
            '_sourceTitleLabel
            '
            Me._sourceTitleLabel.AutoSize = True
            Me._sourceTitleLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._sourceTitleLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._sourceTitleLabel.Location = New System.Drawing.Point(24, 102)
            Me._sourceTitleLabel.Name = "_sourceTitleLabel"
            Me._sourceTitleLabel.Size = New System.Drawing.Size(80, 15)
            Me._sourceTitleLabel.TabIndex = 2
            Me._sourceTitleLabel.Text = "Sumber aktif:"
            '
            '_sourceValueLabel
            '
            Me._sourceValueLabel.AutoSize = True
            Me._sourceValueLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._sourceValueLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._sourceValueLabel.Location = New System.Drawing.Point(116, 102)
            Me._sourceValueLabel.MaximumSize = New System.Drawing.Size(400, 0)
            Me._sourceValueLabel.Name = "_sourceValueLabel"
            Me._sourceValueLabel.Size = New System.Drawing.Size(124, 15)
            Me._sourceValueLabel.TabIndex = 3
            Me._sourceValueLabel.Text = "Belum dikonfigurasi"
            '
            '_urlLabel
            '
            Me._urlLabel.AutoSize = True
            Me._urlLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._urlLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._urlLabel.Location = New System.Drawing.Point(24, 136)
            Me._urlLabel.Name = "_urlLabel"
            Me._urlLabel.Size = New System.Drawing.Size(142, 17)
            Me._urlLabel.TabIndex = 4
            Me._urlLabel.Text = "Project URL Supabase"
            '
            '_urlTextBox
            '
            Me._urlTextBox.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._urlTextBox.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._urlTextBox.Location = New System.Drawing.Point(24, 158)
            Me._urlTextBox.Name = "_urlTextBox"
            Me._urlTextBox.PlaceholderText = "https://project-ref.supabase.co"
            Me._urlTextBox.Size = New System.Drawing.Size(492, 24)
            Me._urlTextBox.TabIndex = 0
            '
            '_keyLabel
            '
            Me._keyLabel.AutoSize = True
            Me._keyLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._keyLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._keyLabel.Location = New System.Drawing.Point(24, 198)
            Me._keyLabel.Name = "_keyLabel"
            Me._keyLabel.Size = New System.Drawing.Size(152, 17)
            Me._keyLabel.TabIndex = 5
            Me._keyLabel.Text = "Publishable / Anon Key"
            '
            '_keyTextBox
            '
            Me._keyTextBox.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._keyTextBox.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._keyTextBox.Location = New System.Drawing.Point(24, 220)
            Me._keyTextBox.Name = "_keyTextBox"
            Me._keyTextBox.PlaceholderText = "sb_publishable_... atau eyJ..."
            Me._keyTextBox.Size = New System.Drawing.Size(492, 24)
            Me._keyTextBox.TabIndex = 1
            Me._keyTextBox.UseSystemPasswordChar = True
            '
            '_maskKeyCheckBox
            '
            Me._maskKeyCheckBox.AutoSize = True
            Me._maskKeyCheckBox.Cursor = System.Windows.Forms.Cursors.Hand
            Me._maskKeyCheckBox.Location = New System.Drawing.Point(24, 254)
            Me._maskKeyCheckBox.Name = "_maskKeyCheckBox"
            Me._maskKeyCheckBox.Size = New System.Drawing.Size(110, 21)
            Me._maskKeyCheckBox.TabIndex = 2
            Me._maskKeyCheckBox.Text = "Tampilkan key"
            '
            '_securityNoteLabel
            '
            Me._securityNoteLabel.BackColor = System.Drawing.Color.FromArgb(253, 243, 227)
            Me._securityNoteLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._securityNoteLabel.ForeColor = System.Drawing.Color.FromArgb(122, 70, 14)
            Me._securityNoteLabel.Location = New System.Drawing.Point(24, 286)
            Me._securityNoteLabel.Name = "_securityNoteLabel"
            Me._securityNoteLabel.Padding = New System.Windows.Forms.Padding(10, 6, 10, 6)
            Me._securityNoteLabel.Size = New System.Drawing.Size(492, 42)
            Me._securityNoteLabel.TabIndex = 6
            Me._securityNoteLabel.Text = "Keamanan: Jangan masukkan service_role atau sb_secret_ key di aplikasi desktop. Aplikasi otomatis menolak kunci tersebut."
            '
            '_statusLabel
            '
            Me._statusLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._statusLabel.Location = New System.Drawing.Point(24, 336)
            Me._statusLabel.Name = "_statusLabel"
            Me._statusLabel.Size = New System.Drawing.Size(492, 34)
            Me._statusLabel.TabIndex = 7
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
            Me._cancelButton.Location = New System.Drawing.Point(272, 380)
            Me._cancelButton.Name = "_cancelButton"
            Me._cancelButton.Size = New System.Drawing.Size(100, 34)
            Me._cancelButton.TabIndex = 4
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
            Me._saveButton.Location = New System.Drawing.Point(380, 380)
            Me._saveButton.Name = "_saveButton"
            Me._saveButton.Size = New System.Drawing.Size(136, 34)
            Me._saveButton.TabIndex = 3
            Me._saveButton.Text = "Uji & Simpan"
            Me._saveButton.UseVisualStyleBackColor = False
            '
            'SupabaseConfigForm
            '
            Me.AcceptButton = Me._saveButton
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me.CancelButton = Me._cancelButton
            Me.ClientSize = New System.Drawing.Size(540, 434)
            Me.Controls.Add(Me._containerPanel)
            Me.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.KeyPreview = True
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "SupabaseConfigForm"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Koneksi Supabase - CuppaClub"
            Me._containerPanel.ResumeLayout(False)
            Me._containerPanel.PerformLayout()
            Me.ResumeLayout(False)
        End Sub

        Friend WithEvents _containerPanel As System.Windows.Forms.Panel
        Friend WithEvents _titleLabel As System.Windows.Forms.Label
        Friend WithEvents _subtitleLabel As System.Windows.Forms.Label
        Friend WithEvents _sourceTitleLabel As System.Windows.Forms.Label
        Friend WithEvents _sourceValueLabel As System.Windows.Forms.Label
        Friend WithEvents _urlLabel As System.Windows.Forms.Label
        Friend WithEvents _urlTextBox As System.Windows.Forms.TextBox
        Friend WithEvents _keyLabel As System.Windows.Forms.Label
        Friend WithEvents _keyTextBox As System.Windows.Forms.TextBox
        Friend WithEvents _maskKeyCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents _securityNoteLabel As System.Windows.Forms.Label
        Friend WithEvents _statusLabel As System.Windows.Forms.Label
        Friend WithEvents _cancelButton As System.Windows.Forms.Button
        Friend WithEvents _saveButton As System.Windows.Forms.Button
    End Class
End Namespace
