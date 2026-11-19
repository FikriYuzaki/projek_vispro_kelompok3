Option Strict On
Option Explicit On

Namespace Features
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class ComingSoonControl
        Inherits System.Windows.Forms.UserControl

        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing Then
                    CleanupRuntimeResources()
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
            Me._cardPanel = New System.Windows.Forms.Panel()
            Me._layoutFlow = New System.Windows.Forms.FlowLayoutPanel()
            Me._statusLabel = New System.Windows.Forms.Label()
            Me._titleLabel = New System.Windows.Forms.Label()
            Me._ownerLabel = New System.Windows.Forms.Label()
            Me._summaryLabel = New System.Windows.Forms.Label()
            Me._dividerPanel = New System.Windows.Forms.Panel()
            Me._scopeHeaderLabel = New System.Windows.Forms.Label()
            Me._plannedListLabel = New System.Windows.Forms.Label()
            Me._honestyNoteLabel = New System.Windows.Forms.Label()
            Me._cardPanel.SuspendLayout()
            Me._layoutFlow.SuspendLayout()
            Me.SuspendLayout()
            '
            '_cardPanel
            '
            Me._cardPanel.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._cardPanel.Controls.Add(Me._layoutFlow)
            Me._cardPanel.Location = New System.Drawing.Point(40, 32)
            Me._cardPanel.Name = "_cardPanel"
            Me._cardPanel.Padding = New System.Windows.Forms.Padding(28, 26, 28, 26)
            Me._cardPanel.Size = New System.Drawing.Size(620, 420)
            Me._cardPanel.TabIndex = 0
            '
            '_layoutFlow
            '
            Me._layoutFlow.AutoSize = True
            Me._layoutFlow.BackColor = System.Drawing.Color.Transparent
            Me._layoutFlow.Controls.Add(Me._statusLabel)
            Me._layoutFlow.Controls.Add(Me._titleLabel)
            Me._layoutFlow.Controls.Add(Me._ownerLabel)
            Me._layoutFlow.Controls.Add(Me._summaryLabel)
            Me._layoutFlow.Controls.Add(Me._dividerPanel)
            Me._layoutFlow.Controls.Add(Me._scopeHeaderLabel)
            Me._layoutFlow.Controls.Add(Me._plannedListLabel)
            Me._layoutFlow.Controls.Add(Me._honestyNoteLabel)
            Me._layoutFlow.Dock = System.Windows.Forms.DockStyle.Fill
            Me._layoutFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
            Me._layoutFlow.Location = New System.Drawing.Point(28, 26)
            Me._layoutFlow.Name = "_layoutFlow"
            Me._layoutFlow.Size = New System.Drawing.Size(564, 368)
            Me._layoutFlow.TabIndex = 0
            Me._layoutFlow.WrapContents = False
            '
            '_statusLabel
            '
            Me._statusLabel.AutoSize = True
            Me._statusLabel.BackColor = System.Drawing.Color.FromArgb(239, 231, 223)
            Me._statusLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._statusLabel.ForeColor = System.Drawing.Color.FromArgb(92, 78, 67)
            Me._statusLabel.Location = New System.Drawing.Point(0, 0)
            Me._statusLabel.Margin = New System.Windows.Forms.Padding(0, 0, 0, 12)
            Me._statusLabel.Name = "_statusLabel"
            Me._statusLabel.Padding = New System.Windows.Forms.Padding(8, 3, 8, 3)
            Me._statusLabel.Size = New System.Drawing.Size(128, 21)
            Me._statusLabel.TabIndex = 0
            Me._statusLabel.Text = "Status: Coming Soon"
            '
            '_titleLabel
            '
            Me._titleLabel.AutoSize = True
            Me._titleLabel.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._titleLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._titleLabel.Location = New System.Drawing.Point(0, 33)
            Me._titleLabel.Margin = New System.Windows.Forms.Padding(0, 0, 0, 4)
            Me._titleLabel.MaximumSize = New System.Drawing.Size(556, 0)
            Me._titleLabel.Name = "_titleLabel"
            Me._titleLabel.Size = New System.Drawing.Size(166, 30)
            Me._titleLabel.TabIndex = 1
            Me._titleLabel.Text = "Modul CuppaClub"
            '
            '_ownerLabel
            '
            Me._ownerLabel.AutoSize = True
            Me._ownerLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._ownerLabel.ForeColor = System.Drawing.Color.FromArgb(143, 53, 71)
            Me._ownerLabel.Location = New System.Drawing.Point(0, 67)
            Me._ownerLabel.Margin = New System.Windows.Forms.Padding(0, 0, 0, 14)
            Me._ownerLabel.Name = "_ownerLabel"
            Me._ownerLabel.Size = New System.Drawing.Size(218, 17)
            Me._ownerLabel.TabIndex = 2
            Me._ownerLabel.Text = "Penanggung jawab modul: Anggota Tim"
            '
            '_summaryLabel
            '
            Me._summaryLabel.AutoSize = True
            Me._summaryLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._summaryLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._summaryLabel.Location = New System.Drawing.Point(0, 98)
            Me._summaryLabel.Margin = New System.Windows.Forms.Padding(0, 0, 0, 18)
            Me._summaryLabel.MaximumSize = New System.Drawing.Size(556, 0)
            Me._summaryLabel.Name = "_summaryLabel"
            Me._summaryLabel.Size = New System.Drawing.Size(340, 17)
            Me._summaryLabel.TabIndex = 3
            Me._summaryLabel.Text = "Ringkasan fitur akan ditampilkan saat modul dipilih."
            '
            '_dividerPanel
            '
            Me._dividerPanel.BackColor = System.Drawing.Color.FromArgb(226, 211, 195)
            Me._dividerPanel.Location = New System.Drawing.Point(0, 133)
            Me._dividerPanel.Margin = New System.Windows.Forms.Padding(0, 0, 0, 16)
            Me._dividerPanel.Name = "_dividerPanel"
            Me._dividerPanel.Size = New System.Drawing.Size(556, 1)
            Me._dividerPanel.TabIndex = 4
            '
            '_scopeHeaderLabel
            '
            Me._scopeHeaderLabel.AutoSize = True
            Me._scopeHeaderLabel.Font = New System.Drawing.Font("Segoe UI", 11.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._scopeHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._scopeHeaderLabel.Location = New System.Drawing.Point(0, 150)
            Me._scopeHeaderLabel.Margin = New System.Windows.Forms.Padding(0, 0, 0, 8)
            Me._scopeHeaderLabel.Name = "_scopeHeaderLabel"
            Me._scopeHeaderLabel.Size = New System.Drawing.Size(272, 21)
            Me._scopeHeaderLabel.TabIndex = 5
            Me._scopeHeaderLabel.Text = "Ruang lingkup yang sedang disiapkan"
            '
            '_plannedListLabel
            '
            Me._plannedListLabel.AutoSize = True
            Me._plannedListLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._plannedListLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._plannedListLabel.Location = New System.Drawing.Point(4, 179)
            Me._plannedListLabel.Margin = New System.Windows.Forms.Padding(4, 0, 0, 6)
            Me._plannedListLabel.MaximumSize = New System.Drawing.Size(556, 0)
            Me._plannedListLabel.Name = "_plannedListLabel"
            Me._plannedListLabel.Size = New System.Drawing.Size(260, 17)
            Me._plannedListLabel.TabIndex = 6
            Me._plannedListLabel.Text = "- Rencana modul dari Feature Registry"
            '
            '_honestyNoteLabel
            '
            Me._honestyNoteLabel.AutoSize = True
            Me._honestyNoteLabel.BackColor = System.Drawing.Color.FromArgb(243, 232, 220)
            Me._honestyNoteLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._honestyNoteLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._honestyNoteLabel.Location = New System.Drawing.Point(0, 218)
            Me._honestyNoteLabel.Margin = New System.Windows.Forms.Padding(0, 16, 0, 0)
            Me._honestyNoteLabel.MaximumSize = New System.Drawing.Size(556, 0)
            Me._honestyNoteLabel.Name = "_honestyNoteLabel"
            Me._honestyNoteLabel.Padding = New System.Windows.Forms.Padding(12, 10, 12, 10)
            Me._honestyNoteLabel.Size = New System.Drawing.Size(556, 52)
            Me._honestyNoteLabel.TabIndex = 7
            Me._honestyNoteLabel.Text = "Modul ini sengaja ditampilkan sebagai Coming Soon pada Feature Registry hingga implementasi nyata selesai, diuji, dan diintegrasikan ke solusi utama."
            '
            'ComingSoonControl
            '
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.AutoScroll = True
            Me.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me.Controls.Add(Me._cardPanel)
            Me.DoubleBuffered = True
            Me.Name = "ComingSoonControl"
            Me.Padding = New System.Windows.Forms.Padding(24)
            Me.Size = New System.Drawing.Size(880, 560)
            Me._cardPanel.ResumeLayout(False)
            Me._cardPanel.PerformLayout()
            Me._layoutFlow.ResumeLayout(False)
            Me._layoutFlow.PerformLayout()
            Me.ResumeLayout(False)
        End Sub

        Friend WithEvents _cardPanel As System.Windows.Forms.Panel
        Friend WithEvents _layoutFlow As System.Windows.Forms.FlowLayoutPanel
        Friend WithEvents _statusLabel As System.Windows.Forms.Label
        Friend WithEvents _titleLabel As System.Windows.Forms.Label
        Friend WithEvents _ownerLabel As System.Windows.Forms.Label
        Friend WithEvents _summaryLabel As System.Windows.Forms.Label
        Friend WithEvents _dividerPanel As System.Windows.Forms.Panel
        Friend WithEvents _scopeHeaderLabel As System.Windows.Forms.Label
        Friend WithEvents _plannedListLabel As System.Windows.Forms.Label
        Friend WithEvents _honestyNoteLabel As System.Windows.Forms.Label
    End Class
End Namespace
