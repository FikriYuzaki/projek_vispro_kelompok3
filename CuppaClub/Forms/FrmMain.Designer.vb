Option Strict On
Option Explicit On

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmMain
    Inherits System.Windows.Forms.Form

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
        Me._sidebarPanel = New System.Windows.Forms.Panel()
        Me._navHost = New System.Windows.Forms.FlowLayoutPanel()
        Me._navSectionLabel = New System.Windows.Forms.Label()
        Me._brandPanel = New System.Windows.Forms.Panel()
        Me._brandTitleLabel = New System.Windows.Forms.Label()
        Me._brandSubtitleLabel = New System.Windows.Forms.Label()
        Me._bottomUtilityPanel = New System.Windows.Forms.Panel()
        Me._sidebarConnBtn = New System.Windows.Forms.Button()
        Me._accountCard = New System.Windows.Forms.Panel()
        Me._accountNameLabel = New System.Windows.Forms.Label()
        Me._accountRoleLabel = New System.Windows.Forms.Label()
        Me._accountActionBtn = New System.Windows.Forms.Button()
        Me._rightWorkspace = New System.Windows.Forms.Panel()
        Me._contentHost = New System.Windows.Forms.Panel()
        Me._headerPanel = New System.Windows.Forms.Panel()
        Me._headerTitleLabel = New System.Windows.Forms.Label()
        Me._headerSubtitleLabel = New System.Windows.Forms.Label()
        Me._headerConnectionBtn = New System.Windows.Forms.Button()
        Me._sidebarPanel.SuspendLayout()
        Me._brandPanel.SuspendLayout()
        Me._bottomUtilityPanel.SuspendLayout()
        Me._accountCard.SuspendLayout()
        Me._rightWorkspace.SuspendLayout()
        Me._headerPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        '_sidebarPanel
        '
        Me._sidebarPanel.BackColor = System.Drawing.Color.FromArgb(243, 232, 220)
        Me._sidebarPanel.Controls.Add(Me._navHost)
        Me._sidebarPanel.Controls.Add(Me._navSectionLabel)
        Me._sidebarPanel.Controls.Add(Me._brandPanel)
        Me._sidebarPanel.Controls.Add(Me._bottomUtilityPanel)
        Me._sidebarPanel.Dock = System.Windows.Forms.DockStyle.Left
        Me._sidebarPanel.Location = New System.Drawing.Point(0, 0)
        Me._sidebarPanel.Name = "_sidebarPanel"
        Me._sidebarPanel.Size = New System.Drawing.Size(268, 760)
        Me._sidebarPanel.TabIndex = 0
        '
        '_brandPanel
        '
        Me._brandPanel.BackColor = System.Drawing.Color.Transparent
        Me._brandPanel.Controls.Add(Me._brandTitleLabel)
        Me._brandPanel.Controls.Add(Me._brandSubtitleLabel)
        Me._brandPanel.Dock = System.Windows.Forms.DockStyle.Top
        Me._brandPanel.Location = New System.Drawing.Point(0, 0)
        Me._brandPanel.Name = "_brandPanel"
        Me._brandPanel.Padding = New System.Windows.Forms.Padding(18, 18, 16, 12)
        Me._brandPanel.Size = New System.Drawing.Size(268, 84)
        Me._brandPanel.TabIndex = 0
        '
        '_brandTitleLabel
        '
        Me._brandTitleLabel.AutoSize = True
        Me._brandTitleLabel.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me._brandTitleLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
        Me._brandTitleLabel.Location = New System.Drawing.Point(64, 16)
        Me._brandTitleLabel.Name = "_brandTitleLabel"
        Me._brandTitleLabel.Size = New System.Drawing.Size(112, 25)
        Me._brandTitleLabel.TabIndex = 0
        Me._brandTitleLabel.Text = "CuppaClub"
        '
        '_brandSubtitleLabel
        '
        Me._brandSubtitleLabel.AutoSize = True
        Me._brandSubtitleLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me._brandSubtitleLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
        Me._brandSubtitleLabel.Location = New System.Drawing.Point(64, 50)
        Me._brandSubtitleLabel.Name = "_brandSubtitleLabel"
        Me._brandSubtitleLabel.Size = New System.Drawing.Size(114, 15)
        Me._brandSubtitleLabel.TabIndex = 1
        Me._brandSubtitleLabel.Text = "Sweet Cafe Desktop"
        '
        '_navSectionLabel
        '
        Me._navSectionLabel.Dock = System.Windows.Forms.DockStyle.Top
        Me._navSectionLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me._navSectionLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
        Me._navSectionLabel.Location = New System.Drawing.Point(0, 84)
        Me._navSectionLabel.Name = "_navSectionLabel"
        Me._navSectionLabel.Padding = New System.Windows.Forms.Padding(18, 10, 12, 6)
        Me._navSectionLabel.Size = New System.Drawing.Size(268, 32)
        Me._navSectionLabel.TabIndex = 1
        Me._navSectionLabel.Text = "Fitur Utama"
        '
        '_navHost
        '
        Me._navHost.AutoScroll = True
        Me._navHost.BackColor = System.Drawing.Color.Transparent
        Me._navHost.Dock = System.Windows.Forms.DockStyle.Fill
        Me._navHost.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me._navHost.Location = New System.Drawing.Point(0, 116)
        Me._navHost.Name = "_navHost"
        Me._navHost.Padding = New System.Windows.Forms.Padding(12, 4, 12, 12)
        Me._navHost.Size = New System.Drawing.Size(268, 470)
        Me._navHost.TabIndex = 2
        Me._navHost.WrapContents = False
        '
        '_bottomUtilityPanel
        '
        Me._bottomUtilityPanel.BackColor = System.Drawing.Color.Transparent
        Me._bottomUtilityPanel.Controls.Add(Me._sidebarConnBtn)
        Me._bottomUtilityPanel.Controls.Add(Me._accountCard)
        Me._bottomUtilityPanel.Dock = System.Windows.Forms.DockStyle.Bottom
        Me._bottomUtilityPanel.Location = New System.Drawing.Point(0, 586)
        Me._bottomUtilityPanel.Name = "_bottomUtilityPanel"
        Me._bottomUtilityPanel.Padding = New System.Windows.Forms.Padding(14, 12, 14, 14)
        Me._bottomUtilityPanel.Size = New System.Drawing.Size(268, 174)
        Me._bottomUtilityPanel.TabIndex = 3
        '
        '_sidebarConnBtn
        '
        Me._sidebarConnBtn.BackColor = System.Drawing.Color.FromArgb(234, 242, 236)
        Me._sidebarConnBtn.Cursor = System.Windows.Forms.Cursors.Hand
        Me._sidebarConnBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(74, 107, 83)
        Me._sidebarConnBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me._sidebarConnBtn.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me._sidebarConnBtn.ForeColor = System.Drawing.Color.FromArgb(47, 79, 56)
        Me._sidebarConnBtn.Location = New System.Drawing.Point(14, 12)
        Me._sidebarConnBtn.Name = "_sidebarConnBtn"
        Me._sidebarConnBtn.Padding = New System.Windows.Forms.Padding(10, 0, 10, 0)
        Me._sidebarConnBtn.Size = New System.Drawing.Size(240, 34)
        Me._sidebarConnBtn.TabIndex = 10
        Me._sidebarConnBtn.Text = "Supabase: Terhubung"
        Me._sidebarConnBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me._sidebarConnBtn.UseVisualStyleBackColor = False
        '
        '_accountCard
        '
        Me._accountCard.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
        Me._accountCard.Controls.Add(Me._accountNameLabel)
        Me._accountCard.Controls.Add(Me._accountRoleLabel)
        Me._accountCard.Controls.Add(Me._accountActionBtn)
        Me._accountCard.Location = New System.Drawing.Point(14, 54)
        Me._accountCard.Name = "_accountCard"
        Me._accountCard.Padding = New System.Windows.Forms.Padding(12, 10, 12, 10)
        Me._accountCard.Size = New System.Drawing.Size(240, 104)
        Me._accountCard.TabIndex = 1
        '
        '_accountNameLabel
        '
        Me._accountNameLabel.AutoEllipsis = True
        Me._accountNameLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me._accountNameLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
        Me._accountNameLabel.Location = New System.Drawing.Point(12, 10)
        Me._accountNameLabel.Name = "_accountNameLabel"
        Me._accountNameLabel.Size = New System.Drawing.Size(216, 24)
        Me._accountNameLabel.TabIndex = 0
        Me._accountNameLabel.Text = "Tamu (Belum Masuk)"
        '
        '_accountRoleLabel
        '
        Me._accountRoleLabel.AutoEllipsis = True
        Me._accountRoleLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me._accountRoleLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
        Me._accountRoleLabel.Location = New System.Drawing.Point(12, 34)
        Me._accountRoleLabel.Name = "_accountRoleLabel"
        Me._accountRoleLabel.Size = New System.Drawing.Size(216, 20)
        Me._accountRoleLabel.TabIndex = 1
        Me._accountRoleLabel.Text = "Masuk untuk akun atau staf"
        '
        '_accountActionBtn
        '
        Me._accountActionBtn.BackColor = System.Drawing.Color.FromArgb(107, 68, 46)
        Me._accountActionBtn.Cursor = System.Windows.Forms.Cursors.Hand
        Me._accountActionBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(107, 68, 46)
        Me._accountActionBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me._accountActionBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me._accountActionBtn.ForeColor = System.Drawing.Color.FromArgb(255, 253, 249)
        Me._accountActionBtn.Location = New System.Drawing.Point(12, 60)
        Me._accountActionBtn.Name = "_accountActionBtn"
        Me._accountActionBtn.Size = New System.Drawing.Size(216, 32)
        Me._accountActionBtn.TabIndex = 11
        Me._accountActionBtn.Text = "Masuk / Daftar"
        Me._accountActionBtn.UseVisualStyleBackColor = False
        '
        '_rightWorkspace
        '
        Me._rightWorkspace.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
        Me._rightWorkspace.Controls.Add(Me._contentHost)
        Me._rightWorkspace.Controls.Add(Me._headerPanel)
        Me._rightWorkspace.Dock = System.Windows.Forms.DockStyle.Fill
        Me._rightWorkspace.Location = New System.Drawing.Point(268, 0)
        Me._rightWorkspace.Name = "_rightWorkspace"
        Me._rightWorkspace.Size = New System.Drawing.Size(932, 760)
        Me._rightWorkspace.TabIndex = 1
        '
        '_headerPanel
        '
        Me._headerPanel.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
        Me._headerPanel.Controls.Add(Me._headerTitleLabel)
        Me._headerPanel.Controls.Add(Me._headerSubtitleLabel)
        Me._headerPanel.Controls.Add(Me._headerConnectionBtn)
        Me._headerPanel.Dock = System.Windows.Forms.DockStyle.Top
        Me._headerPanel.Location = New System.Drawing.Point(0, 0)
        Me._headerPanel.Name = "_headerPanel"
        Me._headerPanel.Padding = New System.Windows.Forms.Padding(24, 10, 24, 10)
        Me._headerPanel.Size = New System.Drawing.Size(932, 76)
        Me._headerPanel.TabIndex = 0
        '
        '_headerTitleLabel
        '
        Me._headerTitleLabel.AutoSize = True
        Me._headerTitleLabel.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me._headerTitleLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
        Me._headerTitleLabel.Location = New System.Drawing.Point(24, 8)
        Me._headerTitleLabel.Name = "_headerTitleLabel"
        Me._headerTitleLabel.Size = New System.Drawing.Size(196, 30)
        Me._headerTitleLabel.TabIndex = 0
        Me._headerTitleLabel.Text = "Café Menu Gallery"
        '
        '_headerSubtitleLabel
        '
        Me._headerSubtitleLabel.AutoSize = True
        Me._headerSubtitleLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me._headerSubtitleLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
        Me._headerSubtitleLabel.Location = New System.Drawing.Point(25, 46)
        Me._headerSubtitleLabel.Name = "_headerSubtitleLabel"
        Me._headerSubtitleLabel.Size = New System.Drawing.Size(222, 15)
        Me._headerSubtitleLabel.TabIndex = 1
        Me._headerSubtitleLabel.Text = "Penanggung jawab: Fikri  |  Status: Tersedia"
        '
        '_headerConnectionBtn
        '
        Me._headerConnectionBtn.BackColor = System.Drawing.Color.FromArgb(234, 242, 236)
        Me._headerConnectionBtn.Cursor = System.Windows.Forms.Cursors.Hand
        Me._headerConnectionBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(74, 107, 83)
        Me._headerConnectionBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me._headerConnectionBtn.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me._headerConnectionBtn.ForeColor = System.Drawing.Color.FromArgb(47, 79, 56)
        Me._headerConnectionBtn.Location = New System.Drawing.Point(692, 22)
        Me._headerConnectionBtn.Name = "_headerConnectionBtn"
        Me._headerConnectionBtn.Size = New System.Drawing.Size(216, 32)
        Me._headerConnectionBtn.TabIndex = 12
        Me._headerConnectionBtn.Text = "Supabase: Terhubung"
        Me._headerConnectionBtn.UseVisualStyleBackColor = False
        '
        '_contentHost
        '
        Me._contentHost.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
        Me._contentHost.Dock = System.Windows.Forms.DockStyle.Fill
        Me._contentHost.Location = New System.Drawing.Point(0, 76)
        Me._contentHost.Name = "_contentHost"
        Me._contentHost.Size = New System.Drawing.Size(932, 684)
        Me._contentHost.TabIndex = 1
        '
        'FrmMain
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
        Me.ClientSize = New System.Drawing.Size(1200, 760)
        Me.Controls.Add(Me._rightWorkspace)
        Me.Controls.Add(Me._sidebarPanel)
        Me.DoubleBuffered = True
        Me.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
        Me.MinimumSize = New System.Drawing.Size(1024, 680)
        Me.Name = "FrmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "CuppaClub - Desktop Cafe App"
        Me._sidebarPanel.ResumeLayout(False)
        Me._brandPanel.ResumeLayout(False)
        Me._brandPanel.PerformLayout()
        Me._bottomUtilityPanel.ResumeLayout(False)
        Me._accountCard.ResumeLayout(False)
        Me._rightWorkspace.ResumeLayout(False)
        Me._headerPanel.ResumeLayout(False)
        Me._headerPanel.PerformLayout()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents _sidebarPanel As System.Windows.Forms.Panel
    Friend WithEvents _brandPanel As System.Windows.Forms.Panel
    Friend WithEvents _brandTitleLabel As System.Windows.Forms.Label
    Friend WithEvents _brandSubtitleLabel As System.Windows.Forms.Label
    Friend WithEvents _navSectionLabel As System.Windows.Forms.Label
    Friend WithEvents _navHost As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents _bottomUtilityPanel As System.Windows.Forms.Panel
    Friend WithEvents _sidebarConnBtn As System.Windows.Forms.Button
    Friend WithEvents _accountCard As System.Windows.Forms.Panel
    Friend WithEvents _accountNameLabel As System.Windows.Forms.Label
    Friend WithEvents _accountRoleLabel As System.Windows.Forms.Label
    Friend WithEvents _accountActionBtn As System.Windows.Forms.Button
    Friend WithEvents _rightWorkspace As System.Windows.Forms.Panel
    Friend WithEvents _headerPanel As System.Windows.Forms.Panel
    Friend WithEvents _headerTitleLabel As System.Windows.Forms.Label
    Friend WithEvents _headerSubtitleLabel As System.Windows.Forms.Label
    Friend WithEvents _headerConnectionBtn As System.Windows.Forms.Button
    Friend WithEvents _contentHost As System.Windows.Forms.Panel
End Class
