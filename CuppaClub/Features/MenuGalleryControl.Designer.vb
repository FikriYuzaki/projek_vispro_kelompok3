Option Strict On
Option Explicit On

Namespace Features
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class MenuGalleryControl
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
            Me._searchLabel = New System.Windows.Forms.Label()
            Me._searchTextBox = New System.Windows.Forms.TextBox()
            Me._clearSearchBtn = New System.Windows.Forms.Button()
            Me._addMenuBtn = New System.Windows.Forms.Button()
            Me._reloadBtn = New System.Windows.Forms.Button()
            Me._categoryLabel = New System.Windows.Forms.Label()
            Me._catAllBtn = New System.Windows.Forms.Button()
            Me._catCoffeeBtn = New System.Windows.Forms.Button()
            Me._catNonCoffeeBtn = New System.Windows.Forms.Button()
            Me._catPastryBtn = New System.Windows.Forms.Button()
            Me._catDessertBtn = New System.Windows.Forms.Button()
            Me._showUnavailableCheck = New System.Windows.Forms.CheckBox()
            Me._workspaceHost = New System.Windows.Forms.Panel()
            Me._catalogPane = New System.Windows.Forms.Panel()
            Me._cardsPanel = New System.Windows.Forms.FlowLayoutPanel()
            Me._countStripPanel = New System.Windows.Forms.Panel()
            Me._resultCountLabel = New System.Windows.Forms.Label()
            Me._detailPanel = New System.Windows.Forms.Panel()
            Me._detailLayout = New System.Windows.Forms.FlowLayoutPanel()
            Me._detailPhotoBox = New System.Windows.Forms.PictureBox()
            Me._metaRow = New System.Windows.Forms.Panel()
            Me._detailCategoryLabel = New System.Windows.Forms.Label()
            Me._detailAvailabilityTag = New System.Windows.Forms.Label()
            Me._detailTitleLabel = New System.Windows.Forms.Label()
            Me._detailPriceLabel = New System.Windows.Forms.Label()
            Me._detailDescriptionLabel = New System.Windows.Forms.Label()
            Me._detailCustomNoteLabel = New System.Windows.Forms.Label()
            Me._staffActionsPanel = New System.Windows.Forms.Panel()
            Me._editMenuBtn = New System.Windows.Forms.Button()
            Me._toggleAvailabilityBtn = New System.Windows.Forms.Button()
            Me._detailStatusBanner = New System.Windows.Forms.Label()
            Me._toolbarPanel.SuspendLayout()
            Me._workspaceHost.SuspendLayout()
            Me._catalogPane.SuspendLayout()
            Me._countStripPanel.SuspendLayout()
            Me._detailPanel.SuspendLayout()
            Me._detailLayout.SuspendLayout()
            CType(Me._detailPhotoBox, System.ComponentModel.ISupportInitialize).BeginInit()
            Me._metaRow.SuspendLayout()
            Me._staffActionsPanel.SuspendLayout()
            Me.SuspendLayout()
            '
            '_toolbarPanel
            '
            Me._toolbarPanel.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._toolbarPanel.Controls.Add(Me._searchLabel)
            Me._toolbarPanel.Controls.Add(Me._searchTextBox)
            Me._toolbarPanel.Controls.Add(Me._clearSearchBtn)
            Me._toolbarPanel.Controls.Add(Me._addMenuBtn)
            Me._toolbarPanel.Controls.Add(Me._reloadBtn)
            Me._toolbarPanel.Controls.Add(Me._categoryLabel)
            Me._toolbarPanel.Controls.Add(Me._catAllBtn)
            Me._toolbarPanel.Controls.Add(Me._catCoffeeBtn)
            Me._toolbarPanel.Controls.Add(Me._catNonCoffeeBtn)
            Me._toolbarPanel.Controls.Add(Me._catPastryBtn)
            Me._toolbarPanel.Controls.Add(Me._catDessertBtn)
            Me._toolbarPanel.Controls.Add(Me._showUnavailableCheck)
            Me._toolbarPanel.Dock = System.Windows.Forms.DockStyle.Top
            Me._toolbarPanel.Location = New System.Drawing.Point(0, 0)
            Me._toolbarPanel.Name = "_toolbarPanel"
            Me._toolbarPanel.Size = New System.Drawing.Size(932, 104)
            Me._toolbarPanel.TabIndex = 0
            '
            '_searchLabel
            '
            Me._searchLabel.AutoSize = True
            Me._searchLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._searchLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._searchLabel.Location = New System.Drawing.Point(20, 16)
            Me._searchLabel.Name = "_searchLabel"
            Me._searchLabel.Size = New System.Drawing.Size(71, 17)
            Me._searchLabel.TabIndex = 0
            Me._searchLabel.Text = "Cari menu"
            '
            '_searchTextBox
            '
            Me._searchTextBox.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me._searchTextBox.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._searchTextBox.Location = New System.Drawing.Point(104, 13)
            Me._searchTextBox.Name = "_searchTextBox"
            Me._searchTextBox.PlaceholderText = "Nama kopi, pastry, atau bahan..."
            Me._searchTextBox.Size = New System.Drawing.Size(260, 24)
            Me._searchTextBox.TabIndex = 0
            '
            '_clearSearchBtn
            '
            Me._clearSearchBtn.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._clearSearchBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._clearSearchBtn.Enabled = False
            Me._clearSearchBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 123, 101)
            Me._clearSearchBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._clearSearchBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._clearSearchBtn.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._clearSearchBtn.Location = New System.Drawing.Point(372, 12)
            Me._clearSearchBtn.Name = "_clearSearchBtn"
            Me._clearSearchBtn.Size = New System.Drawing.Size(98, 30)
            Me._clearSearchBtn.TabIndex = 1
            Me._clearSearchBtn.Text = "Bersihkan"
            Me._clearSearchBtn.UseVisualStyleBackColor = False
            '
            '_addMenuBtn
            '
            Me._addMenuBtn.BackColor = System.Drawing.Color.FromArgb(107, 68, 46)
            Me._addMenuBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._addMenuBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(107, 68, 46)
            Me._addMenuBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._addMenuBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._addMenuBtn.ForeColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._addMenuBtn.Location = New System.Drawing.Point(648, 11)
            Me._addMenuBtn.Name = "_addMenuBtn"
            Me._addMenuBtn.Size = New System.Drawing.Size(142, 32)
            Me._addMenuBtn.TabIndex = 2
            Me._addMenuBtn.Text = "+ Tambah Menu"
            Me._addMenuBtn.UseVisualStyleBackColor = False
            '
            '_reloadBtn
            '
            Me._reloadBtn.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._reloadBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._reloadBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 123, 101)
            Me._reloadBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._reloadBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._reloadBtn.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._reloadBtn.Location = New System.Drawing.Point(798, 11)
            Me._reloadBtn.Name = "_reloadBtn"
            Me._reloadBtn.Size = New System.Drawing.Size(114, 32)
            Me._reloadBtn.TabIndex = 3
            Me._reloadBtn.Text = "Muat Ulang"
            Me._reloadBtn.UseVisualStyleBackColor = False
            '
            '_categoryLabel
            '
            Me._categoryLabel.AutoSize = True
            Me._categoryLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._categoryLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._categoryLabel.Location = New System.Drawing.Point(20, 61)
            Me._categoryLabel.Name = "_categoryLabel"
            Me._categoryLabel.Size = New System.Drawing.Size(61, 17)
            Me._categoryLabel.TabIndex = 4
            Me._categoryLabel.Text = "Kategori"
            '
            '_catAllBtn
            '
            Me._catAllBtn.BackColor = System.Drawing.Color.FromArgb(158, 61, 80)
            Me._catAllBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._catAllBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(158, 61, 80)
            Me._catAllBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._catAllBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._catAllBtn.ForeColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._catAllBtn.Location = New System.Drawing.Point(104, 56)
            Me._catAllBtn.Name = "_catAllBtn"
            Me._catAllBtn.Size = New System.Drawing.Size(76, 32)
            Me._catAllBtn.TabIndex = 4
            Me._catAllBtn.Text = "Semua"
            Me._catAllBtn.UseVisualStyleBackColor = False
            '
            '_catCoffeeBtn
            '
            Me._catCoffeeBtn.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._catCoffeeBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._catCoffeeBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 123, 101)
            Me._catCoffeeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._catCoffeeBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._catCoffeeBtn.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._catCoffeeBtn.Location = New System.Drawing.Point(186, 56)
            Me._catCoffeeBtn.Name = "_catCoffeeBtn"
            Me._catCoffeeBtn.Size = New System.Drawing.Size(74, 32)
            Me._catCoffeeBtn.TabIndex = 5
            Me._catCoffeeBtn.Text = "Coffee"
            Me._catCoffeeBtn.UseVisualStyleBackColor = False
            '
            '_catNonCoffeeBtn
            '
            Me._catNonCoffeeBtn.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._catNonCoffeeBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._catNonCoffeeBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 123, 101)
            Me._catNonCoffeeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._catNonCoffeeBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._catNonCoffeeBtn.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._catNonCoffeeBtn.Location = New System.Drawing.Point(266, 56)
            Me._catNonCoffeeBtn.Name = "_catNonCoffeeBtn"
            Me._catNonCoffeeBtn.Size = New System.Drawing.Size(110, 32)
            Me._catNonCoffeeBtn.TabIndex = 6
            Me._catNonCoffeeBtn.Text = "Non-Coffee"
            Me._catNonCoffeeBtn.UseVisualStyleBackColor = False
            '
            '_catPastryBtn
            '
            Me._catPastryBtn.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._catPastryBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._catPastryBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 123, 101)
            Me._catPastryBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._catPastryBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._catPastryBtn.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._catPastryBtn.Location = New System.Drawing.Point(382, 56)
            Me._catPastryBtn.Name = "_catPastryBtn"
            Me._catPastryBtn.Size = New System.Drawing.Size(74, 32)
            Me._catPastryBtn.TabIndex = 7
            Me._catPastryBtn.Text = "Pastry"
            Me._catPastryBtn.UseVisualStyleBackColor = False
            '
            '_catDessertBtn
            '
            Me._catDessertBtn.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._catDessertBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._catDessertBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 123, 101)
            Me._catDessertBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._catDessertBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._catDessertBtn.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._catDessertBtn.Location = New System.Drawing.Point(462, 56)
            Me._catDessertBtn.Name = "_catDessertBtn"
            Me._catDessertBtn.Size = New System.Drawing.Size(80, 32)
            Me._catDessertBtn.TabIndex = 8
            Me._catDessertBtn.Text = "Dessert"
            Me._catDessertBtn.UseVisualStyleBackColor = False
            '
            '_showUnavailableCheck
            '
            Me._showUnavailableCheck.AutoSize = True
            Me._showUnavailableCheck.Cursor = System.Windows.Forms.Cursors.Hand
            Me._showUnavailableCheck.Location = New System.Drawing.Point(790, 61)
            Me._showUnavailableCheck.Name = "_showUnavailableCheck"
            Me._showUnavailableCheck.Size = New System.Drawing.Size(122, 21)
            Me._showUnavailableCheck.TabIndex = 9
            Me._showUnavailableCheck.Text = "Tampilkan habis"
            '
            '_workspaceHost
            '
            Me._workspaceHost.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me._workspaceHost.Controls.Add(Me._catalogPane)
            Me._workspaceHost.Controls.Add(Me._detailPanel)
            Me._workspaceHost.Dock = System.Windows.Forms.DockStyle.Fill
            Me._workspaceHost.Location = New System.Drawing.Point(0, 104)
            Me._workspaceHost.Name = "_workspaceHost"
            Me._workspaceHost.Size = New System.Drawing.Size(932, 580)
            Me._workspaceHost.TabIndex = 1
            '
            '_catalogPane
            '
            Me._catalogPane.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me._catalogPane.Controls.Add(Me._cardsPanel)
            Me._catalogPane.Controls.Add(Me._countStripPanel)
            Me._catalogPane.Dock = System.Windows.Forms.DockStyle.Fill
            Me._catalogPane.Location = New System.Drawing.Point(0, 0)
            Me._catalogPane.Name = "_catalogPane"
            Me._catalogPane.Size = New System.Drawing.Size(576, 580)
            Me._catalogPane.TabIndex = 0
            '
            '_countStripPanel
            '
            Me._countStripPanel.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me._countStripPanel.Controls.Add(Me._resultCountLabel)
            Me._countStripPanel.Dock = System.Windows.Forms.DockStyle.Top
            Me._countStripPanel.Location = New System.Drawing.Point(0, 0)
            Me._countStripPanel.Name = "_countStripPanel"
            Me._countStripPanel.Padding = New System.Windows.Forms.Padding(20, 8, 12, 0)
            Me._countStripPanel.Size = New System.Drawing.Size(576, 30)
            Me._countStripPanel.TabIndex = 0
            '
            '_resultCountLabel
            '
            Me._resultCountLabel.AutoSize = True
            Me._resultCountLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._resultCountLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._resultCountLabel.Location = New System.Drawing.Point(20, 8)
            Me._resultCountLabel.Name = "_resultCountLabel"
            Me._resultCountLabel.Size = New System.Drawing.Size(156, 15)
            Me._resultCountLabel.TabIndex = 0
            Me._resultCountLabel.Text = "Menampilkan daftar menu"
            '
            '_cardsPanel
            '
            Me._cardsPanel.AutoScroll = True
            Me._cardsPanel.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me._cardsPanel.Dock = System.Windows.Forms.DockStyle.Fill
            Me._cardsPanel.Location = New System.Drawing.Point(0, 30)
            Me._cardsPanel.Name = "_cardsPanel"
            Me._cardsPanel.Padding = New System.Windows.Forms.Padding(20, 10, 10, 20)
            Me._cardsPanel.Size = New System.Drawing.Size(576, 550)
            Me._cardsPanel.TabIndex = 1
            '
            '_detailPanel
            '
            Me._detailPanel.AutoScroll = True
            Me._detailPanel.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._detailPanel.Controls.Add(Me._detailLayout)
            Me._detailPanel.Dock = System.Windows.Forms.DockStyle.Right
            Me._detailPanel.Location = New System.Drawing.Point(576, 0)
            Me._detailPanel.Name = "_detailPanel"
            Me._detailPanel.Padding = New System.Windows.Forms.Padding(20)
            Me._detailPanel.Size = New System.Drawing.Size(356, 580)
            Me._detailPanel.TabIndex = 1
            '
            '_detailLayout
            '
            Me._detailLayout.AutoSize = True
            Me._detailLayout.BackColor = System.Drawing.Color.Transparent
            Me._detailLayout.Controls.Add(Me._detailPhotoBox)
            Me._detailLayout.Controls.Add(Me._metaRow)
            Me._detailLayout.Controls.Add(Me._detailTitleLabel)
            Me._detailLayout.Controls.Add(Me._detailPriceLabel)
            Me._detailLayout.Controls.Add(Me._detailDescriptionLabel)
            Me._detailLayout.Controls.Add(Me._detailCustomNoteLabel)
            Me._detailLayout.Controls.Add(Me._staffActionsPanel)
            Me._detailLayout.Controls.Add(Me._detailStatusBanner)
            Me._detailLayout.Dock = System.Windows.Forms.DockStyle.Top
            Me._detailLayout.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
            Me._detailLayout.Location = New System.Drawing.Point(20, 20)
            Me._detailLayout.Name = "_detailLayout"
            Me._detailLayout.Size = New System.Drawing.Size(296, 460)
            Me._detailLayout.TabIndex = 0
            Me._detailLayout.WrapContents = False
            '
            '_detailPhotoBox
            '
            Me._detailPhotoBox.BackColor = System.Drawing.Color.FromArgb(243, 232, 220)
            Me._detailPhotoBox.Location = New System.Drawing.Point(0, 0)
            Me._detailPhotoBox.Margin = New System.Windows.Forms.Padding(0, 0, 0, 12)
            Me._detailPhotoBox.Name = "_detailPhotoBox"
            Me._detailPhotoBox.Size = New System.Drawing.Size(296, 182)
            Me._detailPhotoBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me._detailPhotoBox.TabIndex = 0
            Me._detailPhotoBox.TabStop = False
            '
            '_metaRow
            '
            Me._metaRow.Controls.Add(Me._detailCategoryLabel)
            Me._metaRow.Controls.Add(Me._detailAvailabilityTag)
            Me._metaRow.Location = New System.Drawing.Point(0, 194)
            Me._metaRow.Margin = New System.Windows.Forms.Padding(0, 0, 0, 6)
            Me._metaRow.Name = "_metaRow"
            Me._metaRow.Size = New System.Drawing.Size(296, 28)
            Me._metaRow.TabIndex = 1
            '
            '_detailCategoryLabel
            '
            Me._detailCategoryLabel.AutoSize = True
            Me._detailCategoryLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._detailCategoryLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._detailCategoryLabel.Location = New System.Drawing.Point(0, 4)
            Me._detailCategoryLabel.Name = "_detailCategoryLabel"
            Me._detailCategoryLabel.Size = New System.Drawing.Size(44, 15)
            Me._detailCategoryLabel.TabIndex = 0
            Me._detailCategoryLabel.Text = "Coffee"
            '
            '_detailAvailabilityTag
            '
            Me._detailAvailabilityTag.AutoSize = True
            Me._detailAvailabilityTag.BackColor = System.Drawing.Color.FromArgb(234, 242, 236)
            Me._detailAvailabilityTag.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._detailAvailabilityTag.ForeColor = System.Drawing.Color.FromArgb(47, 79, 56)
            Me._detailAvailabilityTag.Location = New System.Drawing.Point(224, 2)
            Me._detailAvailabilityTag.Name = "_detailAvailabilityTag"
            Me._detailAvailabilityTag.Padding = New System.Windows.Forms.Padding(8, 3, 8, 3)
            Me._detailAvailabilityTag.Size = New System.Drawing.Size(66, 21)
            Me._detailAvailabilityTag.TabIndex = 1
            Me._detailAvailabilityTag.Text = "Tersedia"
            '
            '_detailTitleLabel
            '
            Me._detailTitleLabel.AutoSize = True
            Me._detailTitleLabel.Font = New System.Drawing.Font("Segoe UI", 11.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._detailTitleLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._detailTitleLabel.Location = New System.Drawing.Point(0, 228)
            Me._detailTitleLabel.Margin = New System.Windows.Forms.Padding(0, 0, 0, 4)
            Me._detailTitleLabel.MaximumSize = New System.Drawing.Size(296, 0)
            Me._detailTitleLabel.Name = "_detailTitleLabel"
            Me._detailTitleLabel.Size = New System.Drawing.Size(142, 21)
            Me._detailTitleLabel.TabIndex = 2
            Me._detailTitleLabel.Text = "Caramel Oat Latte"
            '
            '_detailPriceLabel
            '
            Me._detailPriceLabel.AutoSize = True
            Me._detailPriceLabel.Font = New System.Drawing.Font("Segoe UI", 13.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._detailPriceLabel.ForeColor = System.Drawing.Color.FromArgb(107, 68, 46)
            Me._detailPriceLabel.Location = New System.Drawing.Point(0, 253)
            Me._detailPriceLabel.Margin = New System.Windows.Forms.Padding(0, 0, 0, 12)
            Me._detailPriceLabel.Name = "_detailPriceLabel"
            Me._detailPriceLabel.Size = New System.Drawing.Size(92, 25)
            Me._detailPriceLabel.TabIndex = 3
            Me._detailPriceLabel.Text = "Rp 34.000"
            '
            '_detailDescriptionLabel
            '
            Me._detailDescriptionLabel.AutoSize = True
            Me._detailDescriptionLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._detailDescriptionLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._detailDescriptionLabel.Location = New System.Drawing.Point(0, 290)
            Me._detailDescriptionLabel.Margin = New System.Windows.Forms.Padding(0, 0, 0, 14)
            Me._detailDescriptionLabel.MaximumSize = New System.Drawing.Size(296, 0)
            Me._detailDescriptionLabel.Name = "_detailDescriptionLabel"
            Me._detailDescriptionLabel.Size = New System.Drawing.Size(242, 17)
            Me._detailDescriptionLabel.TabIndex = 4
            Me._detailDescriptionLabel.Text = "Deskripsi produk tampil di panel detail."
            '
            '_detailCustomNoteLabel
            '
            Me._detailCustomNoteLabel.AutoSize = True
            Me._detailCustomNoteLabel.BackColor = System.Drawing.Color.FromArgb(251, 236, 239)
            Me._detailCustomNoteLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._detailCustomNoteLabel.ForeColor = System.Drawing.Color.FromArgb(143, 53, 71)
            Me._detailCustomNoteLabel.Location = New System.Drawing.Point(0, 321)
            Me._detailCustomNoteLabel.Margin = New System.Windows.Forms.Padding(0, 0, 0, 14)
            Me._detailCustomNoteLabel.MaximumSize = New System.Drawing.Size(296, 0)
            Me._detailCustomNoteLabel.Name = "_detailCustomNoteLabel"
            Me._detailCustomNoteLabel.Padding = New System.Windows.Forms.Padding(10, 8, 10, 8)
            Me._detailCustomNoteLabel.Size = New System.Drawing.Size(296, 46)
            Me._detailCustomNoteLabel.TabIndex = 5
            Me._detailCustomNoteLabel.Text = "Mendukung kustomisasi di My Drink Creator."
            '
            '_staffActionsPanel
            '
            Me._staffActionsPanel.Controls.Add(Me._editMenuBtn)
            Me._staffActionsPanel.Controls.Add(Me._toggleAvailabilityBtn)
            Me._staffActionsPanel.Location = New System.Drawing.Point(0, 381)
            Me._staffActionsPanel.Margin = New System.Windows.Forms.Padding(0, 0, 0, 8)
            Me._staffActionsPanel.Name = "_staffActionsPanel"
            Me._staffActionsPanel.Size = New System.Drawing.Size(296, 46)
            Me._staffActionsPanel.TabIndex = 6
            '
            '_editMenuBtn
            '
            Me._editMenuBtn.BackColor = System.Drawing.Color.FromArgb(107, 68, 46)
            Me._editMenuBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._editMenuBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(107, 68, 46)
            Me._editMenuBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._editMenuBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._editMenuBtn.ForeColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._editMenuBtn.Location = New System.Drawing.Point(0, 4)
            Me._editMenuBtn.Name = "_editMenuBtn"
            Me._editMenuBtn.Size = New System.Drawing.Size(120, 34)
            Me._editMenuBtn.TabIndex = 0
            Me._editMenuBtn.Text = "Edit Menu"
            Me._editMenuBtn.UseVisualStyleBackColor = False
            '
            '_toggleAvailabilityBtn
            '
            Me._toggleAvailabilityBtn.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._toggleAvailabilityBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._toggleAvailabilityBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 123, 101)
            Me._toggleAvailabilityBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._toggleAvailabilityBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._toggleAvailabilityBtn.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._toggleAvailabilityBtn.Location = New System.Drawing.Point(128, 4)
            Me._toggleAvailabilityBtn.Name = "_toggleAvailabilityBtn"
            Me._toggleAvailabilityBtn.Size = New System.Drawing.Size(168, 34)
            Me._toggleAvailabilityBtn.TabIndex = 1
            Me._toggleAvailabilityBtn.Text = "Tandai Habis"
            Me._toggleAvailabilityBtn.UseVisualStyleBackColor = False
            '
            '_detailStatusBanner
            '
            Me._detailStatusBanner.AutoSize = True
            Me._detailStatusBanner.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._detailStatusBanner.Location = New System.Drawing.Point(0, 435)
            Me._detailStatusBanner.MaximumSize = New System.Drawing.Size(296, 0)
            Me._detailStatusBanner.Name = "_detailStatusBanner"
            Me._detailStatusBanner.Padding = New System.Windows.Forms.Padding(10, 6, 10, 6)
            Me._detailStatusBanner.Size = New System.Drawing.Size(20, 27)
            Me._detailStatusBanner.TabIndex = 7
            Me._detailStatusBanner.Visible = False
            '
            'MenuGalleryControl
            '
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me.Controls.Add(Me._workspaceHost)
            Me.Controls.Add(Me._toolbarPanel)
            Me.DoubleBuffered = True
            Me.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me.Name = "MenuGalleryControl"
            Me.Size = New System.Drawing.Size(932, 684)
            Me._toolbarPanel.ResumeLayout(False)
            Me._toolbarPanel.PerformLayout()
            Me._workspaceHost.ResumeLayout(False)
            Me._catalogPane.ResumeLayout(False)
            Me._countStripPanel.ResumeLayout(False)
            Me._countStripPanel.PerformLayout()
            Me._detailPanel.ResumeLayout(False)
            Me._detailPanel.PerformLayout()
            Me._detailLayout.ResumeLayout(False)
            Me._detailLayout.PerformLayout()
            CType(Me._detailPhotoBox, System.ComponentModel.ISupportInitialize).EndInit()
            Me._metaRow.ResumeLayout(False)
            Me._metaRow.PerformLayout()
            Me._staffActionsPanel.ResumeLayout(False)
            Me.ResumeLayout(False)
        End Sub

        Friend WithEvents _toolbarPanel As System.Windows.Forms.Panel
        Friend WithEvents _searchLabel As System.Windows.Forms.Label
        Friend WithEvents _searchTextBox As System.Windows.Forms.TextBox
        Friend WithEvents _clearSearchBtn As System.Windows.Forms.Button
        Friend WithEvents _addMenuBtn As System.Windows.Forms.Button
        Friend WithEvents _reloadBtn As System.Windows.Forms.Button
        Friend WithEvents _categoryLabel As System.Windows.Forms.Label
        Friend WithEvents _catAllBtn As System.Windows.Forms.Button
        Friend WithEvents _catCoffeeBtn As System.Windows.Forms.Button
        Friend WithEvents _catNonCoffeeBtn As System.Windows.Forms.Button
        Friend WithEvents _catPastryBtn As System.Windows.Forms.Button
        Friend WithEvents _catDessertBtn As System.Windows.Forms.Button
        Friend WithEvents _showUnavailableCheck As System.Windows.Forms.CheckBox
        Friend WithEvents _workspaceHost As System.Windows.Forms.Panel
        Friend WithEvents _catalogPane As System.Windows.Forms.Panel
        Friend WithEvents _countStripPanel As System.Windows.Forms.Panel
        Friend WithEvents _resultCountLabel As System.Windows.Forms.Label
        Friend WithEvents _cardsPanel As System.Windows.Forms.FlowLayoutPanel
        Friend WithEvents _detailPanel As System.Windows.Forms.Panel
        Friend WithEvents _detailLayout As System.Windows.Forms.FlowLayoutPanel
        Friend WithEvents _detailPhotoBox As System.Windows.Forms.PictureBox
        Friend WithEvents _metaRow As System.Windows.Forms.Panel
        Friend WithEvents _detailCategoryLabel As System.Windows.Forms.Label
        Friend WithEvents _detailAvailabilityTag As System.Windows.Forms.Label
        Friend WithEvents _detailTitleLabel As System.Windows.Forms.Label
        Friend WithEvents _detailPriceLabel As System.Windows.Forms.Label
        Friend WithEvents _detailDescriptionLabel As System.Windows.Forms.Label
        Friend WithEvents _detailCustomNoteLabel As System.Windows.Forms.Label
        Friend WithEvents _staffActionsPanel As System.Windows.Forms.Panel
        Friend WithEvents _editMenuBtn As System.Windows.Forms.Button
        Friend WithEvents _toggleAvailabilityBtn As System.Windows.Forms.Button
        Friend WithEvents _detailStatusBanner As System.Windows.Forms.Label
    End Class
End Namespace
