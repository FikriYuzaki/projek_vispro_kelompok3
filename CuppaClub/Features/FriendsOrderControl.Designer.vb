Option Strict On
Option Explicit On

Namespace Features
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class FriendsOrderControl
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
            Me._toolbarPanel = New System.Windows.Forms.Panel()
            Me._createRoomToggleBtn = New System.Windows.Forms.Button()
            Me._joinRoomToggleBtn = New System.Windows.Forms.Button()
            Me._refreshBtn = New System.Windows.Forms.Button()
            Me._realtimeStatusTag = New System.Windows.Forms.Label()
            Me._actionDrawerPanel = New System.Windows.Forms.Panel()
            Me._createPanel = New System.Windows.Forms.Panel()
            Me._createHeadingLabel = New System.Windows.Forms.Label()
            Me._createTitleLabel = New System.Windows.Forms.Label()
            Me._createTitleTextBox = New System.Windows.Forms.TextBox()
            Me._submitCreateBtn = New System.Windows.Forms.Button()
            Me._cancelCreateBtn = New System.Windows.Forms.Button()
            Me._copyCreatedCodeBtn = New System.Windows.Forms.Button()
            Me._createFeedbackLabel = New System.Windows.Forms.Label()
            Me._joinPanel = New System.Windows.Forms.Panel()
            Me._joinHeadingLabel = New System.Windows.Forms.Label()
            Me._joinCodeLabel = New System.Windows.Forms.Label()
            Me._joinCodeTextBox = New System.Windows.Forms.TextBox()
            Me._submitJoinBtn = New System.Windows.Forms.Button()
            Me._cancelJoinBtn = New System.Windows.Forms.Button()
            Me._joinFeedbackLabel = New System.Windows.Forms.Label()
            Me._bannerPanel = New System.Windows.Forms.Panel()
            Me._bannerMessageLabel = New System.Windows.Forms.Label()
            Me._bannerDismissBtn = New System.Windows.Forms.Button()
            Me._workspaceHost = New System.Windows.Forms.Panel()
            Me._roomsPane = New System.Windows.Forms.Panel()
            Me._roomsHeaderPanel = New System.Windows.Forms.Panel()
            Me._roomsHeadingLabel = New System.Windows.Forms.Label()
            Me._roomsCountLabel = New System.Windows.Forms.Label()
            Me._roomsSubLabel = New System.Windows.Forms.Label()
            Me._roomsListPanel = New System.Windows.Forms.FlowLayoutPanel()
            Me._overviewPane = New System.Windows.Forms.Panel()
            Me._overviewScrollPanel = New System.Windows.Forms.Panel()
            Me._roomHeaderCard = New System.Windows.Forms.Panel()
            Me._overviewTitleLabel = New System.Windows.Forms.Label()
            Me._overviewStatusTag = New System.Windows.Forms.Label()
            Me._overviewRoleTag = New System.Windows.Forms.Label()
            Me._overviewCodeLabel = New System.Windows.Forms.Label()
            Me._copyCodeBtn = New System.Windows.Forms.Button()
            Me._overviewHostLabel = New System.Windows.Forms.Label()
            Me._overviewMemberCountLabel = New System.Windows.Forms.Label()
            Me._overviewOrderNumberLabel = New System.Windows.Forms.Label()
            Me._lockRoomBtn = New System.Windows.Forms.Button()
            Me._lockedNoticeLabel = New System.Windows.Forms.Label()
            Me._addItemsCard = New System.Windows.Forms.Panel()
            Me._addItemsHeadingLabel = New System.Windows.Forms.Label()
            Me._handoffInfoLabel = New System.Windows.Forms.Label()
            Me._importDraftsBtn = New System.Windows.Forms.Button()
            Me._quickMenuLabel = New System.Windows.Forms.Label()
            Me._quickMenuCombo = New System.Windows.Forms.ComboBox()
            Me._quickQtyLabel = New System.Windows.Forms.Label()
            Me._quickQtyUpDown = New System.Windows.Forms.NumericUpDown()
            Me._quickNotesTextBox = New System.Windows.Forms.TextBox()
            Me._quickAddBtn = New System.Windows.Forms.Button()
            Me._membersSectionLabel = New System.Windows.Forms.Label()
            Me._membersItemsPanel = New System.Windows.Forms.FlowLayoutPanel()
            Me._groupTotalBar = New System.Windows.Forms.Panel()
            Me._groupSummaryMetaLabel = New System.Windows.Forms.Label()
            Me._groupTotalCaptionLabel = New System.Windows.Forms.Label()
            Me._groupTotalValueLabel = New System.Windows.Forms.Label()
            Me._toolbarPanel.SuspendLayout()
            Me._actionDrawerPanel.SuspendLayout()
            Me._createPanel.SuspendLayout()
            Me._joinPanel.SuspendLayout()
            Me._bannerPanel.SuspendLayout()
            Me._workspaceHost.SuspendLayout()
            Me._roomsPane.SuspendLayout()
            Me._roomsHeaderPanel.SuspendLayout()
            Me._overviewPane.SuspendLayout()
            Me._overviewScrollPanel.SuspendLayout()
            Me._roomHeaderCard.SuspendLayout()
            Me._addItemsCard.SuspendLayout()
            CType(Me._quickQtyUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
            Me._groupTotalBar.SuspendLayout()
            Me.SuspendLayout()
            '
            '_toolbarPanel
            '
            Me._toolbarPanel.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._toolbarPanel.Controls.Add(Me._createRoomToggleBtn)
            Me._toolbarPanel.Controls.Add(Me._joinRoomToggleBtn)
            Me._toolbarPanel.Controls.Add(Me._refreshBtn)
            Me._toolbarPanel.Controls.Add(Me._realtimeStatusTag)
            Me._toolbarPanel.Dock = System.Windows.Forms.DockStyle.Top
            Me._toolbarPanel.Location = New System.Drawing.Point(0, 0)
            Me._toolbarPanel.Name = "_toolbarPanel"
            Me._toolbarPanel.Size = New System.Drawing.Size(932, 56)
            Me._toolbarPanel.TabIndex = 0
            '
            '_createRoomToggleBtn
            '
            Me._createRoomToggleBtn.BackColor = System.Drawing.Color.FromArgb(107, 68, 46)
            Me._createRoomToggleBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._createRoomToggleBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(107, 68, 46)
            Me._createRoomToggleBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._createRoomToggleBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._createRoomToggleBtn.ForeColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._createRoomToggleBtn.Location = New System.Drawing.Point(20, 12)
            Me._createRoomToggleBtn.Name = "_createRoomToggleBtn"
            Me._createRoomToggleBtn.Size = New System.Drawing.Size(128, 32)
            Me._createRoomToggleBtn.TabIndex = 0
            Me._createRoomToggleBtn.Text = "+ Buat Room"
            Me._createRoomToggleBtn.UseVisualStyleBackColor = False
            '
            '_joinRoomToggleBtn
            '
            Me._joinRoomToggleBtn.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._joinRoomToggleBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._joinRoomToggleBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 123, 101)
            Me._joinRoomToggleBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._joinRoomToggleBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._joinRoomToggleBtn.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._joinRoomToggleBtn.Location = New System.Drawing.Point(158, 12)
            Me._joinRoomToggleBtn.Name = "_joinRoomToggleBtn"
            Me._joinRoomToggleBtn.Size = New System.Drawing.Size(144, 32)
            Me._joinRoomToggleBtn.TabIndex = 1
            Me._joinRoomToggleBtn.Text = "Gabung Kode"
            Me._joinRoomToggleBtn.UseVisualStyleBackColor = False
            '
            '_refreshBtn
            '
            Me._refreshBtn.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._refreshBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._refreshBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 123, 101)
            Me._refreshBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._refreshBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._refreshBtn.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._refreshBtn.Location = New System.Drawing.Point(312, 12)
            Me._refreshBtn.Name = "_refreshBtn"
            Me._refreshBtn.Size = New System.Drawing.Size(116, 32)
            Me._refreshBtn.TabIndex = 2
            Me._refreshBtn.Text = "Muat Ulang"
            Me._refreshBtn.UseVisualStyleBackColor = False
            '
            '_realtimeStatusTag
            '
            Me._realtimeStatusTag.AutoSize = True
            Me._realtimeStatusTag.BackColor = System.Drawing.Color.FromArgb(239, 231, 223)
            Me._realtimeStatusTag.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._realtimeStatusTag.ForeColor = System.Drawing.Color.FromArgb(92, 78, 67)
            Me._realtimeStatusTag.Location = New System.Drawing.Point(760, 18)
            Me._realtimeStatusTag.Name = "_realtimeStatusTag"
            Me._realtimeStatusTag.Padding = New System.Windows.Forms.Padding(8, 3, 8, 3)
            Me._realtimeStatusTag.Size = New System.Drawing.Size(152, 21)
            Me._realtimeStatusTag.TabIndex = 3
            Me._realtimeStatusTag.Text = "Realtime: Siap"
            '
            '_actionDrawerPanel
            '
            Me._actionDrawerPanel.BackColor = System.Drawing.Color.FromArgb(243, 232, 220)
            Me._actionDrawerPanel.Controls.Add(Me._createPanel)
            Me._actionDrawerPanel.Controls.Add(Me._joinPanel)
            Me._actionDrawerPanel.Dock = System.Windows.Forms.DockStyle.Top
            Me._actionDrawerPanel.Location = New System.Drawing.Point(0, 56)
            Me._actionDrawerPanel.Name = "_actionDrawerPanel"
            Me._actionDrawerPanel.Size = New System.Drawing.Size(932, 112)
            Me._actionDrawerPanel.TabIndex = 1
            Me._actionDrawerPanel.Visible = False
            '
            '_createPanel
            '
            Me._createPanel.BackColor = System.Drawing.Color.Transparent
            Me._createPanel.Controls.Add(Me._createHeadingLabel)
            Me._createPanel.Controls.Add(Me._createTitleLabel)
            Me._createPanel.Controls.Add(Me._createTitleTextBox)
            Me._createPanel.Controls.Add(Me._submitCreateBtn)
            Me._createPanel.Controls.Add(Me._cancelCreateBtn)
            Me._createPanel.Controls.Add(Me._copyCreatedCodeBtn)
            Me._createPanel.Controls.Add(Me._createFeedbackLabel)
            Me._createPanel.Dock = System.Windows.Forms.DockStyle.Fill
            Me._createPanel.Location = New System.Drawing.Point(0, 0)
            Me._createPanel.Name = "_createPanel"
            Me._createPanel.Size = New System.Drawing.Size(932, 112)
            Me._createPanel.TabIndex = 0
            Me._createPanel.Visible = False
            '
            '_createHeadingLabel
            '
            Me._createHeadingLabel.AutoSize = True
            Me._createHeadingLabel.Font = New System.Drawing.Font("Segoe UI", 10.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._createHeadingLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._createHeadingLabel.Location = New System.Drawing.Point(20, 10)
            Me._createHeadingLabel.Name = "_createHeadingLabel"
            Me._createHeadingLabel.Size = New System.Drawing.Size(218, 19)
            Me._createHeadingLabel.TabIndex = 0
            Me._createHeadingLabel.Text = "Buat Friends Order Room Baru"
            '
            '_createTitleLabel
            '
            Me._createTitleLabel.AutoSize = True
            Me._createTitleLabel.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._createTitleLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._createTitleLabel.Location = New System.Drawing.Point(20, 42)
            Me._createTitleLabel.Name = "_createTitleLabel"
            Me._createTitleLabel.Size = New System.Drawing.Size(76, 15)
            Me._createTitleLabel.TabIndex = 1
            Me._createTitleLabel.Text = "Judul Room"
            '
            '_createTitleTextBox
            '
            Me._createTitleTextBox.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._createTitleTextBox.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._createTitleTextBox.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._createTitleTextBox.Location = New System.Drawing.Point(108, 38)
            Me._createTitleTextBox.MaxLength = 80
            Me._createTitleTextBox.Name = "_createTitleTextBox"
            Me._createTitleTextBox.PlaceholderText = "Contoh: Ngopi Sore Tim VisPro"
            Me._createTitleTextBox.Size = New System.Drawing.Size(340, 24)
            Me._createTitleTextBox.TabIndex = 2
            '
            '_submitCreateBtn
            '
            Me._submitCreateBtn.Location = New System.Drawing.Point(460, 35)
            Me._submitCreateBtn.Name = "_submitCreateBtn"
            Me._submitCreateBtn.Size = New System.Drawing.Size(124, 30)
            Me._submitCreateBtn.TabIndex = 3
            Me._submitCreateBtn.Text = "Buat Room"
            '
            '_cancelCreateBtn
            '
            Me._cancelCreateBtn.Location = New System.Drawing.Point(592, 35)
            Me._cancelCreateBtn.Name = "_cancelCreateBtn"
            Me._cancelCreateBtn.Size = New System.Drawing.Size(84, 30)
            Me._cancelCreateBtn.TabIndex = 4
            Me._cancelCreateBtn.Text = "Tutup"
            '
            '_copyCreatedCodeBtn
            '
            Me._copyCreatedCodeBtn.Location = New System.Drawing.Point(684, 35)
            Me._copyCreatedCodeBtn.Name = "_copyCreatedCodeBtn"
            Me._copyCreatedCodeBtn.Size = New System.Drawing.Size(120, 30)
            Me._copyCreatedCodeBtn.TabIndex = 5
            Me._copyCreatedCodeBtn.Text = "Salin Kode"
            Me._copyCreatedCodeBtn.Visible = False
            '
            '_createFeedbackLabel
            '
            Me._createFeedbackLabel.AutoSize = True
            Me._createFeedbackLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._createFeedbackLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._createFeedbackLabel.Location = New System.Drawing.Point(20, 76)
            Me._createFeedbackLabel.Name = "_createFeedbackLabel"
            Me._createFeedbackLabel.Size = New System.Drawing.Size(420, 15)
            Me._createFeedbackLabel.TabIndex = 6
            Me._createFeedbackLabel.Text = "Kode undangan acak 8 karakter dibuat otomatis dan Anda langsung menjadi host."
            '
            '_joinPanel
            '
            Me._joinPanel.BackColor = System.Drawing.Color.Transparent
            Me._joinPanel.Controls.Add(Me._joinHeadingLabel)
            Me._joinPanel.Controls.Add(Me._joinCodeLabel)
            Me._joinPanel.Controls.Add(Me._joinCodeTextBox)
            Me._joinPanel.Controls.Add(Me._submitJoinBtn)
            Me._joinPanel.Controls.Add(Me._cancelJoinBtn)
            Me._joinPanel.Controls.Add(Me._joinFeedbackLabel)
            Me._joinPanel.Dock = System.Windows.Forms.DockStyle.Fill
            Me._joinPanel.Location = New System.Drawing.Point(0, 0)
            Me._joinPanel.Name = "_joinPanel"
            Me._joinPanel.Size = New System.Drawing.Size(932, 112)
            Me._joinPanel.TabIndex = 1
            Me._joinPanel.Visible = False
            '
            '_joinHeadingLabel
            '
            Me._joinHeadingLabel.AutoSize = True
            Me._joinHeadingLabel.Font = New System.Drawing.Font("Segoe UI", 10.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._joinHeadingLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._joinHeadingLabel.Location = New System.Drawing.Point(20, 10)
            Me._joinHeadingLabel.Name = "_joinHeadingLabel"
            Me._joinHeadingLabel.Size = New System.Drawing.Size(222, 19)
            Me._joinHeadingLabel.TabIndex = 0
            Me._joinHeadingLabel.Text = "Gabung Room dengan Kode Undangan"
            '
            '_joinCodeLabel
            '
            Me._joinCodeLabel.AutoSize = True
            Me._joinCodeLabel.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._joinCodeLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._joinCodeLabel.Location = New System.Drawing.Point(20, 42)
            Me._joinCodeLabel.Name = "_joinCodeLabel"
            Me._joinCodeLabel.Size = New System.Drawing.Size(74, 15)
            Me._joinCodeLabel.TabIndex = 1
            Me._joinCodeLabel.Text = "Kode Room"
            '
            '_joinCodeTextBox
            '
            Me._joinCodeTextBox.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._joinCodeTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
            Me._joinCodeTextBox.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._joinCodeTextBox.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._joinCodeTextBox.Location = New System.Drawing.Point(108, 38)
            Me._joinCodeTextBox.MaxLength = 16
            Me._joinCodeTextBox.Name = "_joinCodeTextBox"
            Me._joinCodeTextBox.PlaceholderText = "Masukkan 4-16 karakter kode..."
            Me._joinCodeTextBox.Size = New System.Drawing.Size(240, 24)
            Me._joinCodeTextBox.TabIndex = 2
            '
            '_submitJoinBtn
            '
            Me._submitJoinBtn.Location = New System.Drawing.Point(360, 35)
            Me._submitJoinBtn.Name = "_submitJoinBtn"
            Me._submitJoinBtn.Size = New System.Drawing.Size(132, 30)
            Me._submitJoinBtn.TabIndex = 3
            Me._submitJoinBtn.Text = "Gabung Room"
            '
            '_cancelJoinBtn
            '
            Me._cancelJoinBtn.Location = New System.Drawing.Point(500, 35)
            Me._cancelJoinBtn.Name = "_cancelJoinBtn"
            Me._cancelJoinBtn.Size = New System.Drawing.Size(84, 30)
            Me._cancelJoinBtn.TabIndex = 4
            Me._cancelJoinBtn.Text = "Tutup"
            '
            '_joinFeedbackLabel
            '
            Me._joinFeedbackLabel.AutoSize = True
            Me._joinFeedbackLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._joinFeedbackLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._joinFeedbackLabel.Location = New System.Drawing.Point(20, 76)
            Me._joinFeedbackLabel.Name = "_joinFeedbackLabel"
            Me._joinFeedbackLabel.Size = New System.Drawing.Size(440, 15)
            Me._joinFeedbackLabel.TabIndex = 5
            Me._joinFeedbackLabel.Text = "Masukkan kode dari host. Validasi keanggotaan diproses melalui RPC server."
            '
            '_bannerPanel
            '
            Me._bannerPanel.BackColor = System.Drawing.Color.FromArgb(234, 242, 236)
            Me._bannerPanel.Controls.Add(Me._bannerMessageLabel)
            Me._bannerPanel.Controls.Add(Me._bannerDismissBtn)
            Me._bannerPanel.Dock = System.Windows.Forms.DockStyle.Top
            Me._bannerPanel.Location = New System.Drawing.Point(0, 168)
            Me._bannerPanel.Name = "_bannerPanel"
            Me._bannerPanel.Size = New System.Drawing.Size(932, 40)
            Me._bannerPanel.TabIndex = 2
            Me._bannerPanel.Visible = False
            '
            '_bannerMessageLabel
            '
            Me._bannerMessageLabel.AutoEllipsis = True
            Me._bannerMessageLabel.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._bannerMessageLabel.ForeColor = System.Drawing.Color.FromArgb(47, 79, 56)
            Me._bannerMessageLabel.Location = New System.Drawing.Point(20, 10)
            Me._bannerMessageLabel.Name = "_bannerMessageLabel"
            Me._bannerMessageLabel.Size = New System.Drawing.Size(810, 20)
            Me._bannerMessageLabel.TabIndex = 0
            Me._bannerMessageLabel.Text = ""
            '
            '_bannerDismissBtn
            '
            Me._bannerDismissBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me._bannerDismissBtn.Location = New System.Drawing.Point(842, 7)
            Me._bannerDismissBtn.Name = "_bannerDismissBtn"
            Me._bannerDismissBtn.Size = New System.Drawing.Size(70, 26)
            Me._bannerDismissBtn.TabIndex = 1
            Me._bannerDismissBtn.Text = "Tutup"
            '
            '_workspaceHost
            '
            Me._workspaceHost.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me._workspaceHost.Controls.Add(Me._overviewPane)
            Me._workspaceHost.Controls.Add(Me._roomsPane)
            Me._workspaceHost.Dock = System.Windows.Forms.DockStyle.Fill
            Me._workspaceHost.Location = New System.Drawing.Point(0, 208)
            Me._workspaceHost.Name = "_workspaceHost"
            Me._workspaceHost.Size = New System.Drawing.Size(932, 488)
            Me._workspaceHost.TabIndex = 3
            '
            '_roomsPane
            '
            Me._roomsPane.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._roomsPane.Controls.Add(Me._roomsListPanel)
            Me._roomsPane.Controls.Add(Me._roomsHeaderPanel)
            Me._roomsPane.Dock = System.Windows.Forms.DockStyle.Left
            Me._roomsPane.Location = New System.Drawing.Point(0, 0)
            Me._roomsPane.Name = "_roomsPane"
            Me._roomsPane.Size = New System.Drawing.Size(296, 488)
            Me._roomsPane.TabIndex = 0
            '
            '_roomsHeaderPanel
            '
            Me._roomsHeaderPanel.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._roomsHeaderPanel.Controls.Add(Me._roomsHeadingLabel)
            Me._roomsHeaderPanel.Controls.Add(Me._roomsCountLabel)
            Me._roomsHeaderPanel.Controls.Add(Me._roomsSubLabel)
            Me._roomsHeaderPanel.Dock = System.Windows.Forms.DockStyle.Top
            Me._roomsHeaderPanel.Location = New System.Drawing.Point(0, 0)
            Me._roomsHeaderPanel.Name = "_roomsHeaderPanel"
            Me._roomsHeaderPanel.Size = New System.Drawing.Size(296, 58)
            Me._roomsHeaderPanel.TabIndex = 0
            '
            '_roomsHeadingLabel
            '
            Me._roomsHeadingLabel.AutoSize = True
            Me._roomsHeadingLabel.Font = New System.Drawing.Font("Segoe UI", 11.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._roomsHeadingLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._roomsHeadingLabel.Location = New System.Drawing.Point(16, 12)
            Me._roomsHeadingLabel.Name = "_roomsHeadingLabel"
            Me._roomsHeadingLabel.Size = New System.Drawing.Size(88, 21)
            Me._roomsHeadingLabel.TabIndex = 0
            Me._roomsHeadingLabel.Text = "Room Saya"
            '
            '_roomsCountLabel
            '
            Me._roomsCountLabel.AutoSize = True
            Me._roomsCountLabel.BackColor = System.Drawing.Color.FromArgb(239, 231, 223)
            Me._roomsCountLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._roomsCountLabel.ForeColor = System.Drawing.Color.FromArgb(92, 78, 67)
            Me._roomsCountLabel.Location = New System.Drawing.Point(222, 13)
            Me._roomsCountLabel.Name = "_roomsCountLabel"
            Me._roomsCountLabel.Padding = New System.Windows.Forms.Padding(6, 2, 6, 2)
            Me._roomsCountLabel.Size = New System.Drawing.Size(56, 19)
            Me._roomsCountLabel.TabIndex = 1
            Me._roomsCountLabel.Text = "0 room"
            '
            '_roomsSubLabel
            '
            Me._roomsSubLabel.AutoSize = True
            Me._roomsSubLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._roomsSubLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._roomsSubLabel.Location = New System.Drawing.Point(16, 36)
            Me._roomsSubLabel.Name = "_roomsSubLabel"
            Me._roomsSubLabel.Size = New System.Drawing.Size(190, 15)
            Me._roomsSubLabel.TabIndex = 2
            Me._roomsSubLabel.Text = "Room yang Anda buat atau ikuti"
            '
            '_roomsListPanel
            '
            Me._roomsListPanel.AutoScroll = True
            Me._roomsListPanel.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._roomsListPanel.Dock = System.Windows.Forms.DockStyle.Fill
            Me._roomsListPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
            Me._roomsListPanel.Location = New System.Drawing.Point(0, 58)
            Me._roomsListPanel.Name = "_roomsListPanel"
            Me._roomsListPanel.Padding = New System.Windows.Forms.Padding(12, 12, 12, 12)
            Me._roomsListPanel.Size = New System.Drawing.Size(296, 430)
            Me._roomsListPanel.TabIndex = 1
            Me._roomsListPanel.WrapContents = False
            '
            '_overviewPane
            '
            Me._overviewPane.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me._overviewPane.Controls.Add(Me._overviewScrollPanel)
            Me._overviewPane.Controls.Add(Me._groupTotalBar)
            Me._overviewPane.Dock = System.Windows.Forms.DockStyle.Fill
            Me._overviewPane.Location = New System.Drawing.Point(296, 0)
            Me._overviewPane.Name = "_overviewPane"
            Me._overviewPane.Size = New System.Drawing.Size(636, 488)
            Me._overviewPane.TabIndex = 1
            '
            '_overviewScrollPanel
            '
            Me._overviewScrollPanel.AutoScroll = True
            Me._overviewScrollPanel.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me._overviewScrollPanel.Controls.Add(Me._roomHeaderCard)
            Me._overviewScrollPanel.Controls.Add(Me._addItemsCard)
            Me._overviewScrollPanel.Controls.Add(Me._membersSectionLabel)
            Me._overviewScrollPanel.Controls.Add(Me._membersItemsPanel)
            Me._overviewScrollPanel.Dock = System.Windows.Forms.DockStyle.Fill
            Me._overviewScrollPanel.Location = New System.Drawing.Point(0, 0)
            Me._overviewScrollPanel.Name = "_overviewScrollPanel"
            Me._overviewScrollPanel.Padding = New System.Windows.Forms.Padding(16)
            Me._overviewScrollPanel.Size = New System.Drawing.Size(636, 432)
            Me._overviewScrollPanel.TabIndex = 0
            '
            '_roomHeaderCard
            '
            Me._roomHeaderCard.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._roomHeaderCard.Controls.Add(Me._overviewTitleLabel)
            Me._roomHeaderCard.Controls.Add(Me._overviewStatusTag)
            Me._roomHeaderCard.Controls.Add(Me._overviewRoleTag)
            Me._roomHeaderCard.Controls.Add(Me._overviewCodeLabel)
            Me._roomHeaderCard.Controls.Add(Me._copyCodeBtn)
            Me._roomHeaderCard.Controls.Add(Me._overviewHostLabel)
            Me._roomHeaderCard.Controls.Add(Me._overviewMemberCountLabel)
            Me._roomHeaderCard.Controls.Add(Me._overviewOrderNumberLabel)
            Me._roomHeaderCard.Controls.Add(Me._lockRoomBtn)
            Me._roomHeaderCard.Controls.Add(Me._lockedNoticeLabel)
            Me._roomHeaderCard.Location = New System.Drawing.Point(16, 16)
            Me._roomHeaderCard.Name = "_roomHeaderCard"
            Me._roomHeaderCard.Size = New System.Drawing.Size(584, 146)
            Me._roomHeaderCard.TabIndex = 0
            '
            '_overviewTitleLabel
            '
            Me._overviewTitleLabel.AutoEllipsis = True
            Me._overviewTitleLabel.Font = New System.Drawing.Font("Segoe UI", 13.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._overviewTitleLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._overviewTitleLabel.Location = New System.Drawing.Point(16, 14)
            Me._overviewTitleLabel.Name = "_overviewTitleLabel"
            Me._overviewTitleLabel.Size = New System.Drawing.Size(370, 26)
            Me._overviewTitleLabel.TabIndex = 0
            Me._overviewTitleLabel.Text = "Pilih room di sebelah kiri"
            '
            '_overviewStatusTag
            '
            Me._overviewStatusTag.AutoSize = True
            Me._overviewStatusTag.BackColor = System.Drawing.Color.FromArgb(234, 242, 236)
            Me._overviewStatusTag.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._overviewStatusTag.ForeColor = System.Drawing.Color.FromArgb(47, 79, 56)
            Me._overviewStatusTag.Location = New System.Drawing.Point(398, 16)
            Me._overviewStatusTag.Name = "_overviewStatusTag"
            Me._overviewStatusTag.Padding = New System.Windows.Forms.Padding(8, 3, 8, 3)
            Me._overviewStatusTag.Size = New System.Drawing.Size(66, 21)
            Me._overviewStatusTag.TabIndex = 1
            Me._overviewStatusTag.Text = "Terbuka"
            '
            '_overviewRoleTag
            '
            Me._overviewRoleTag.AutoSize = True
            Me._overviewRoleTag.BackColor = System.Drawing.Color.FromArgb(251, 236, 239)
            Me._overviewRoleTag.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._overviewRoleTag.ForeColor = System.Drawing.Color.FromArgb(143, 53, 71)
            Me._overviewRoleTag.Location = New System.Drawing.Point(476, 16)
            Me._overviewRoleTag.Name = "_overviewRoleTag"
            Me._overviewRoleTag.Padding = New System.Windows.Forms.Padding(8, 3, 8, 3)
            Me._overviewRoleTag.Size = New System.Drawing.Size(74, 21)
            Me._overviewRoleTag.TabIndex = 2
            Me._overviewRoleTag.Text = "Host"
            '
            '_overviewCodeLabel
            '
            Me._overviewCodeLabel.AutoSize = True
            Me._overviewCodeLabel.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._overviewCodeLabel.ForeColor = System.Drawing.Color.FromArgb(107, 68, 46)
            Me._overviewCodeLabel.Location = New System.Drawing.Point(16, 48)
            Me._overviewCodeLabel.Name = "_overviewCodeLabel"
            Me._overviewCodeLabel.Size = New System.Drawing.Size(176, 19)
            Me._overviewCodeLabel.TabIndex = 3
            Me._overviewCodeLabel.Text = "Kode Undangan: -"
            '
            '_copyCodeBtn
            '
            Me._copyCodeBtn.Location = New System.Drawing.Point(214, 43)
            Me._copyCodeBtn.Name = "_copyCodeBtn"
            Me._copyCodeBtn.Size = New System.Drawing.Size(96, 28)
            Me._copyCodeBtn.TabIndex = 4
            Me._copyCodeBtn.Text = "Salin Kode"
            '
            '_overviewHostLabel
            '
            Me._overviewHostLabel.AutoSize = True
            Me._overviewHostLabel.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._overviewHostLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._overviewHostLabel.Location = New System.Drawing.Point(16, 76)
            Me._overviewHostLabel.Name = "_overviewHostLabel"
            Me._overviewHostLabel.Size = New System.Drawing.Size(120, 15)
            Me._overviewHostLabel.TabIndex = 5
            Me._overviewHostLabel.Text = "Host: -"
            '
            '_overviewMemberCountLabel
            '
            Me._overviewMemberCountLabel.AutoSize = True
            Me._overviewMemberCountLabel.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._overviewMemberCountLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._overviewMemberCountLabel.Location = New System.Drawing.Point(220, 76)
            Me._overviewMemberCountLabel.Name = "_overviewMemberCountLabel"
            Me._overviewMemberCountLabel.Size = New System.Drawing.Size(100, 15)
            Me._overviewMemberCountLabel.TabIndex = 6
            Me._overviewMemberCountLabel.Text = "Anggota: 0"
            '
            '_overviewOrderNumberLabel
            '
            Me._overviewOrderNumberLabel.AutoSize = True
            Me._overviewOrderNumberLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._overviewOrderNumberLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._overviewOrderNumberLabel.Location = New System.Drawing.Point(16, 98)
            Me._overviewOrderNumberLabel.Name = "_overviewOrderNumberLabel"
            Me._overviewOrderNumberLabel.Size = New System.Drawing.Size(220, 15)
            Me._overviewOrderNumberLabel.TabIndex = 7
            Me._overviewOrderNumberLabel.Text = "Order grup: Belum dibuat"
            '
            '_lockRoomBtn
            '
            Me._lockRoomBtn.Location = New System.Drawing.Point(436, 43)
            Me._lockRoomBtn.Name = "_lockRoomBtn"
            Me._lockRoomBtn.Size = New System.Drawing.Size(132, 30)
            Me._lockRoomBtn.TabIndex = 8
            Me._lockRoomBtn.Text = "Kunci Room"
            '
            '_lockedNoticeLabel
            '
            Me._lockedNoticeLabel.AutoEllipsis = True
            Me._lockedNoticeLabel.BackColor = System.Drawing.Color.FromArgb(253, 243, 227)
            Me._lockedNoticeLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._lockedNoticeLabel.ForeColor = System.Drawing.Color.FromArgb(122, 70, 14)
            Me._lockedNoticeLabel.Location = New System.Drawing.Point(16, 118)
            Me._lockedNoticeLabel.Name = "_lockedNoticeLabel"
            Me._lockedNoticeLabel.Padding = New System.Windows.Forms.Padding(8, 3, 8, 3)
            Me._lockedNoticeLabel.Size = New System.Drawing.Size(552, 22)
            Me._lockedNoticeLabel.TabIndex = 9
            Me._lockedNoticeLabel.Text = "Room dikunci oleh host. Anggota baru dan penambahan item dihentikan."
            Me._lockedNoticeLabel.Visible = False
            '
            '_addItemsCard
            '
            Me._addItemsCard.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._addItemsCard.Controls.Add(Me._addItemsHeadingLabel)
            Me._addItemsCard.Controls.Add(Me._handoffInfoLabel)
            Me._addItemsCard.Controls.Add(Me._importDraftsBtn)
            Me._addItemsCard.Controls.Add(Me._quickMenuLabel)
            Me._addItemsCard.Controls.Add(Me._quickMenuCombo)
            Me._addItemsCard.Controls.Add(Me._quickQtyLabel)
            Me._addItemsCard.Controls.Add(Me._quickQtyUpDown)
            Me._addItemsCard.Controls.Add(Me._quickNotesTextBox)
            Me._addItemsCard.Controls.Add(Me._quickAddBtn)
            Me._addItemsCard.Location = New System.Drawing.Point(16, 174)
            Me._addItemsCard.Name = "_addItemsCard"
            Me._addItemsCard.Size = New System.Drawing.Size(584, 146)
            Me._addItemsCard.TabIndex = 1
            '
            '_addItemsHeadingLabel
            '
            Me._addItemsHeadingLabel.AutoSize = True
            Me._addItemsHeadingLabel.Font = New System.Drawing.Font("Segoe UI", 10.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._addItemsHeadingLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._addItemsHeadingLabel.Location = New System.Drawing.Point(16, 12)
            Me._addItemsHeadingLabel.Name = "_addItemsHeadingLabel"
            Me._addItemsHeadingLabel.Size = New System.Drawing.Size(214, 19)
            Me._addItemsHeadingLabel.TabIndex = 0
            Me._addItemsHeadingLabel.Text = "Tambahkan Pesanan Anda"
            '
            '_handoffInfoLabel
            '
            Me._handoffInfoLabel.AutoEllipsis = True
            Me._handoffInfoLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._handoffInfoLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._handoffInfoLabel.Location = New System.Drawing.Point(16, 40)
            Me._handoffInfoLabel.Name = "_handoffInfoLabel"
            Me._handoffInfoLabel.Size = New System.Drawing.Size(320, 20)
            Me._handoffInfoLabel.TabIndex = 1
            Me._handoffInfoLabel.Text = "Draft My Drink Creator: 0 item menunggu"
            '
            '_importDraftsBtn
            '
            Me._importDraftsBtn.Location = New System.Drawing.Point(346, 34)
            Me._importDraftsBtn.Name = "_importDraftsBtn"
            Me._importDraftsBtn.Size = New System.Drawing.Size(222, 30)
            Me._importDraftsBtn.TabIndex = 2
            Me._importDraftsBtn.Text = "Masukkan Draft Drink Creator"
            '
            '_quickMenuLabel
            '
            Me._quickMenuLabel.AutoSize = True
            Me._quickMenuLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._quickMenuLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._quickMenuLabel.Location = New System.Drawing.Point(16, 78)
            Me._quickMenuLabel.Name = "_quickMenuLabel"
            Me._quickMenuLabel.Size = New System.Drawing.Size(92, 15)
            Me._quickMenuLabel.TabIndex = 3
            Me._quickMenuLabel.Text = "Tambah Cepat"
            '
            '_quickMenuCombo
            '
            Me._quickMenuCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me._quickMenuCombo.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._quickMenuCombo.FormattingEnabled = True
            Me._quickMenuCombo.Location = New System.Drawing.Point(16, 98)
            Me._quickMenuCombo.Name = "_quickMenuCombo"
            Me._quickMenuCombo.Size = New System.Drawing.Size(210, 23)
            Me._quickMenuCombo.TabIndex = 4
            '
            '_quickQtyLabel
            '
            Me._quickQtyLabel.AutoSize = True
            Me._quickQtyLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._quickQtyLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._quickQtyLabel.Location = New System.Drawing.Point(234, 78)
            Me._quickQtyLabel.Name = "_quickQtyLabel"
            Me._quickQtyLabel.Size = New System.Drawing.Size(28, 15)
            Me._quickQtyLabel.TabIndex = 5
            Me._quickQtyLabel.Text = "Qty"
            '
            '_quickQtyUpDown
            '
            Me._quickQtyUpDown.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._quickQtyUpDown.Location = New System.Drawing.Point(234, 98)
            Me._quickQtyUpDown.Maximum = New Decimal(New Integer() {99, 0, 0, 0})
            Me._quickQtyUpDown.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me._quickQtyUpDown.Name = "_quickQtyUpDown"
            Me._quickQtyUpDown.Size = New System.Drawing.Size(54, 23)
            Me._quickQtyUpDown.TabIndex = 6
            Me._quickQtyUpDown.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            '_quickNotesTextBox
            '
            Me._quickNotesTextBox.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me._quickNotesTextBox.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._quickNotesTextBox.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._quickNotesTextBox.Location = New System.Drawing.Point(296, 98)
            Me._quickNotesTextBox.MaxLength = 200
            Me._quickNotesTextBox.Name = "_quickNotesTextBox"
            Me._quickNotesTextBox.PlaceholderText = "Catatan (opsional)..."
            Me._quickNotesTextBox.Size = New System.Drawing.Size(148, 23)
            Me._quickNotesTextBox.TabIndex = 7
            '
            '_quickAddBtn
            '
            Me._quickAddBtn.Location = New System.Drawing.Point(452, 95)
            Me._quickAddBtn.Name = "_quickAddBtn"
            Me._quickAddBtn.Size = New System.Drawing.Size(116, 29)
            Me._quickAddBtn.TabIndex = 8
            Me._quickAddBtn.Text = "Tambah Item"
            '
            '_membersSectionLabel
            '
            Me._membersSectionLabel.AutoSize = True
            Me._membersSectionLabel.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._membersSectionLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._membersSectionLabel.Location = New System.Drawing.Point(16, 334)
            Me._membersSectionLabel.Name = "_membersSectionLabel"
            Me._membersSectionLabel.Size = New System.Drawing.Size(262, 20)
            Me._membersSectionLabel.TabIndex = 2
            Me._membersSectionLabel.Text = "Anggota & Pesanan per Anggota"
            Me._membersSectionLabel.UseMnemonic = False
            '
            '_membersItemsPanel
            '
            Me._membersItemsPanel.AutoSize = True
            Me._membersItemsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me._membersItemsPanel.BackColor = System.Drawing.Color.Transparent
            Me._membersItemsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
            Me._membersItemsPanel.Location = New System.Drawing.Point(16, 362)
            Me._membersItemsPanel.Name = "_membersItemsPanel"
            Me._membersItemsPanel.Size = New System.Drawing.Size(584, 56)
            Me._membersItemsPanel.TabIndex = 3
            Me._membersItemsPanel.WrapContents = False
            '
            '_groupTotalBar
            '
            Me._groupTotalBar.BackColor = System.Drawing.Color.FromArgb(243, 232, 220)
            Me._groupTotalBar.Controls.Add(Me._groupSummaryMetaLabel)
            Me._groupTotalBar.Controls.Add(Me._groupTotalCaptionLabel)
            Me._groupTotalBar.Controls.Add(Me._groupTotalValueLabel)
            Me._groupTotalBar.Dock = System.Windows.Forms.DockStyle.Bottom
            Me._groupTotalBar.Location = New System.Drawing.Point(0, 432)
            Me._groupTotalBar.Name = "_groupTotalBar"
            Me._groupTotalBar.Size = New System.Drawing.Size(636, 56)
            Me._groupTotalBar.TabIndex = 1
            '
            '_groupSummaryMetaLabel
            '
            Me._groupSummaryMetaLabel.AutoSize = True
            Me._groupSummaryMetaLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._groupSummaryMetaLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._groupSummaryMetaLabel.Location = New System.Drawing.Point(20, 19)
            Me._groupSummaryMetaLabel.Name = "_groupSummaryMetaLabel"
            Me._groupSummaryMetaLabel.Size = New System.Drawing.Size(148, 17)
            Me._groupSummaryMetaLabel.TabIndex = 0
            Me._groupSummaryMetaLabel.Text = "0 anggota · 0 item"
            '
            '_groupTotalCaptionLabel
            '
            Me._groupTotalCaptionLabel.AutoSize = True
            Me._groupTotalCaptionLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._groupTotalCaptionLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._groupTotalCaptionLabel.Location = New System.Drawing.Point(390, 19)
            Me._groupTotalCaptionLabel.Name = "_groupTotalCaptionLabel"
            Me._groupTotalCaptionLabel.Size = New System.Drawing.Size(78, 17)
            Me._groupTotalCaptionLabel.TabIndex = 1
            Me._groupTotalCaptionLabel.Text = "Total Grup:"
            '
            '_groupTotalValueLabel
            '
            Me._groupTotalValueLabel.AutoSize = True
            Me._groupTotalValueLabel.Font = New System.Drawing.Font("Segoe UI", 13.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._groupTotalValueLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._groupTotalValueLabel.Location = New System.Drawing.Point(476, 14)
            Me._groupTotalValueLabel.Name = "_groupTotalValueLabel"
            Me._groupTotalValueLabel.Size = New System.Drawing.Size(120, 25)
            Me._groupTotalValueLabel.TabIndex = 2
            Me._groupTotalValueLabel.Text = "Rp 0"
            '
            'FriendsOrderControl
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me.Controls.Add(Me._workspaceHost)
            Me.Controls.Add(Me._bannerPanel)
            Me.Controls.Add(Me._actionDrawerPanel)
            Me.Controls.Add(Me._toolbarPanel)
            Me.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.Name = "FriendsOrderControl"
            Me.Size = New System.Drawing.Size(932, 696)
            Me._toolbarPanel.ResumeLayout(False)
            Me._toolbarPanel.PerformLayout()
            Me._actionDrawerPanel.ResumeLayout(False)
            Me._createPanel.ResumeLayout(False)
            Me._createPanel.PerformLayout()
            Me._joinPanel.ResumeLayout(False)
            Me._joinPanel.PerformLayout()
            Me._bannerPanel.ResumeLayout(False)
            Me._workspaceHost.ResumeLayout(False)
            Me._roomsPane.ResumeLayout(False)
            Me._roomsHeaderPanel.ResumeLayout(False)
            Me._roomsHeaderPanel.PerformLayout()
            Me._overviewPane.ResumeLayout(False)
            Me._overviewScrollPanel.ResumeLayout(False)
            Me._overviewScrollPanel.PerformLayout()
            Me._roomHeaderCard.ResumeLayout(False)
            Me._roomHeaderCard.PerformLayout()
            Me._addItemsCard.ResumeLayout(False)
            Me._addItemsCard.PerformLayout()
            CType(Me._quickQtyUpDown, System.ComponentModel.ISupportInitialize).EndInit()
            Me._groupTotalBar.ResumeLayout(False)
            Me._groupTotalBar.PerformLayout()
            Me.ResumeLayout(False)
        End Sub

        Friend WithEvents _toolbarPanel As System.Windows.Forms.Panel
        Friend WithEvents _createRoomToggleBtn As System.Windows.Forms.Button
        Friend WithEvents _joinRoomToggleBtn As System.Windows.Forms.Button
        Friend WithEvents _refreshBtn As System.Windows.Forms.Button
        Friend WithEvents _realtimeStatusTag As System.Windows.Forms.Label
        Friend WithEvents _actionDrawerPanel As System.Windows.Forms.Panel
        Friend WithEvents _createPanel As System.Windows.Forms.Panel
        Friend WithEvents _createHeadingLabel As System.Windows.Forms.Label
        Friend WithEvents _createTitleLabel As System.Windows.Forms.Label
        Friend WithEvents _createTitleTextBox As System.Windows.Forms.TextBox
        Friend WithEvents _submitCreateBtn As System.Windows.Forms.Button
        Friend WithEvents _cancelCreateBtn As System.Windows.Forms.Button
        Friend WithEvents _copyCreatedCodeBtn As System.Windows.Forms.Button
        Friend WithEvents _createFeedbackLabel As System.Windows.Forms.Label
        Friend WithEvents _joinPanel As System.Windows.Forms.Panel
        Friend WithEvents _joinHeadingLabel As System.Windows.Forms.Label
        Friend WithEvents _joinCodeLabel As System.Windows.Forms.Label
        Friend WithEvents _joinCodeTextBox As System.Windows.Forms.TextBox
        Friend WithEvents _submitJoinBtn As System.Windows.Forms.Button
        Friend WithEvents _cancelJoinBtn As System.Windows.Forms.Button
        Friend WithEvents _joinFeedbackLabel As System.Windows.Forms.Label
        Friend WithEvents _bannerPanel As System.Windows.Forms.Panel
        Friend WithEvents _bannerMessageLabel As System.Windows.Forms.Label
        Friend WithEvents _bannerDismissBtn As System.Windows.Forms.Button
        Friend WithEvents _workspaceHost As System.Windows.Forms.Panel
        Friend WithEvents _roomsPane As System.Windows.Forms.Panel
        Friend WithEvents _roomsHeaderPanel As System.Windows.Forms.Panel
        Friend WithEvents _roomsHeadingLabel As System.Windows.Forms.Label
        Friend WithEvents _roomsCountLabel As System.Windows.Forms.Label
        Friend WithEvents _roomsSubLabel As System.Windows.Forms.Label
        Friend WithEvents _roomsListPanel As System.Windows.Forms.FlowLayoutPanel
        Friend WithEvents _overviewPane As System.Windows.Forms.Panel
        Friend WithEvents _overviewScrollPanel As System.Windows.Forms.Panel
        Friend WithEvents _roomHeaderCard As System.Windows.Forms.Panel
        Friend WithEvents _overviewTitleLabel As System.Windows.Forms.Label
        Friend WithEvents _overviewStatusTag As System.Windows.Forms.Label
        Friend WithEvents _overviewRoleTag As System.Windows.Forms.Label
        Friend WithEvents _overviewCodeLabel As System.Windows.Forms.Label
        Friend WithEvents _copyCodeBtn As System.Windows.Forms.Button
        Friend WithEvents _overviewHostLabel As System.Windows.Forms.Label
        Friend WithEvents _overviewMemberCountLabel As System.Windows.Forms.Label
        Friend WithEvents _overviewOrderNumberLabel As System.Windows.Forms.Label
        Friend WithEvents _lockRoomBtn As System.Windows.Forms.Button
        Friend WithEvents _lockedNoticeLabel As System.Windows.Forms.Label
        Friend WithEvents _addItemsCard As System.Windows.Forms.Panel
        Friend WithEvents _addItemsHeadingLabel As System.Windows.Forms.Label
        Friend WithEvents _handoffInfoLabel As System.Windows.Forms.Label
        Friend WithEvents _importDraftsBtn As System.Windows.Forms.Button
        Friend WithEvents _quickMenuLabel As System.Windows.Forms.Label
        Friend WithEvents _quickMenuCombo As System.Windows.Forms.ComboBox
        Friend WithEvents _quickQtyLabel As System.Windows.Forms.Label
        Friend WithEvents _quickQtyUpDown As System.Windows.Forms.NumericUpDown
        Friend WithEvents _quickNotesTextBox As System.Windows.Forms.TextBox
        Friend WithEvents _quickAddBtn As System.Windows.Forms.Button
        Friend WithEvents _membersSectionLabel As System.Windows.Forms.Label
        Friend WithEvents _membersItemsPanel As System.Windows.Forms.FlowLayoutPanel
        Friend WithEvents _groupTotalBar As System.Windows.Forms.Panel
        Friend WithEvents _groupSummaryMetaLabel As System.Windows.Forms.Label
        Friend WithEvents _groupTotalCaptionLabel As System.Windows.Forms.Label
        Friend WithEvents _groupTotalValueLabel As System.Windows.Forms.Label
    End Class
End Namespace
