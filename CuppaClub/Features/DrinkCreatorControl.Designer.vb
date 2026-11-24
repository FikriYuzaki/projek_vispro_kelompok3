Option Strict On
Option Explicit On

Namespace Features
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class DrinkCreatorControl
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
            Me._reloadBtn = New System.Windows.Forms.Button()
            Me._categoryLabel = New System.Windows.Forms.Label()
            Me._catAllBtn = New System.Windows.Forms.Button()
            Me._catCoffeeBtn = New System.Windows.Forms.Button()
            Me._catNonCoffeeBtn = New System.Windows.Forms.Button()
            Me._catPastryBtn = New System.Windows.Forms.Button()
            Me._catDessertBtn = New System.Windows.Forms.Button()
            Me._availableCountLabel = New System.Windows.Forms.Label()
            Me._workspaceHost = New System.Windows.Forms.Panel()
            Me._builderPane = New System.Windows.Forms.Panel()
            Me._builderBodyPanel = New System.Windows.Forms.Panel()
            Me._builderFlow = New System.Windows.Forms.FlowLayoutPanel()
            Me._optionsHostPanel = New System.Windows.Forms.FlowLayoutPanel()
            Me._orderMetaCard = New System.Windows.Forms.Panel()
            Me._quantitySectionLabel = New System.Windows.Forms.Label()
            Me._qtyMinusBtn = New System.Windows.Forms.Button()
            Me._quantityInput = New System.Windows.Forms.NumericUpDown()
            Me._qtyPlusBtn = New System.Windows.Forms.Button()
            Me._quantityHintLabel = New System.Windows.Forms.Label()
            Me._notesLabel = New System.Windows.Forms.Label()
            Me._notesTextBox = New System.Windows.Forms.TextBox()
            Me._selectedHeaderPanel = New System.Windows.Forms.Panel()
            Me._selectedPhotoBox = New System.Windows.Forms.PictureBox()
            Me._selectedCategoryLabel = New System.Windows.Forms.Label()
            Me._selectedAvailabilityTag = New System.Windows.Forms.Label()
            Me._selectedCustomTag = New System.Windows.Forms.Label()
            Me._selectedTitleLabel = New System.Windows.Forms.Label()
            Me._selectedDescriptionLabel = New System.Windows.Forms.Label()
            Me._selectedBasePriceLabel = New System.Windows.Forms.Label()
            Me._summaryPanel = New System.Windows.Forms.Panel()
            Me._summaryTitleLabel = New System.Windows.Forms.Label()
            Me._summaryOptionsLabel = New System.Windows.Forms.Label()
            Me._basePriceCaptionLabel = New System.Windows.Forms.Label()
            Me._basePriceValueLabel = New System.Windows.Forms.Label()
            Me._extraPriceCaptionLabel = New System.Windows.Forms.Label()
            Me._extraPriceValueLabel = New System.Windows.Forms.Label()
            Me._unitTotalCaptionLabel = New System.Windows.Forms.Label()
            Me._unitTotalValueLabel = New System.Windows.Forms.Label()
            Me._subtotalCaptionLabel = New System.Windows.Forms.Label()
            Me._subtotalValueLabel = New System.Windows.Forms.Label()
            Me._feedbackBanner = New System.Windows.Forms.Label()
            Me._actionRowPanel = New System.Windows.Forms.Panel()
            Me._handoffBtn = New System.Windows.Forms.Button()
            Me._resetOptionsBtn = New System.Windows.Forms.Button()
            Me._viewDraftsBtn = New System.Windows.Forms.Button()
            Me._menuListPane = New System.Windows.Forms.Panel()
            Me._drinkCardsPanel = New System.Windows.Forms.FlowLayoutPanel()
            Me._menuListHeaderPanel = New System.Windows.Forms.Panel()
            Me._menuListHeaderLabel = New System.Windows.Forms.Label()
            Me._toolbarPanel.SuspendLayout()
            Me._workspaceHost.SuspendLayout()
            Me._builderPane.SuspendLayout()
            Me._builderBodyPanel.SuspendLayout()
            Me._builderFlow.SuspendLayout()
            Me._orderMetaCard.SuspendLayout()
            CType(Me._quantityInput, System.ComponentModel.ISupportInitialize).BeginInit()
            Me._selectedHeaderPanel.SuspendLayout()
            CType(Me._selectedPhotoBox, System.ComponentModel.ISupportInitialize).BeginInit()
            Me._summaryPanel.SuspendLayout()
            Me._actionRowPanel.SuspendLayout()
            Me._menuListPane.SuspendLayout()
            Me._menuListHeaderPanel.SuspendLayout()
            Me.SuspendLayout()
            '
            '_toolbarPanel
            '
            Me._toolbarPanel.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._toolbarPanel.Controls.Add(Me._searchLabel)
            Me._toolbarPanel.Controls.Add(Me._searchTextBox)
            Me._toolbarPanel.Controls.Add(Me._clearSearchBtn)
            Me._toolbarPanel.Controls.Add(Me._reloadBtn)
            Me._toolbarPanel.Controls.Add(Me._categoryLabel)
            Me._toolbarPanel.Controls.Add(Me._catAllBtn)
            Me._toolbarPanel.Controls.Add(Me._catCoffeeBtn)
            Me._toolbarPanel.Controls.Add(Me._catNonCoffeeBtn)
            Me._toolbarPanel.Controls.Add(Me._catPastryBtn)
            Me._toolbarPanel.Controls.Add(Me._catDessertBtn)
            Me._toolbarPanel.Controls.Add(Me._availableCountLabel)
            Me._toolbarPanel.Dock = System.Windows.Forms.DockStyle.Top
            Me._toolbarPanel.Location = New System.Drawing.Point(0, 0)
            Me._toolbarPanel.Name = "_toolbarPanel"
            Me._toolbarPanel.Size = New System.Drawing.Size(932, 98)
            Me._toolbarPanel.TabIndex = 0
            '
            '_searchLabel
            '
            Me._searchLabel.AutoSize = True
            Me._searchLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._searchLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._searchLabel.Location = New System.Drawing.Point(20, 16)
            Me._searchLabel.Name = "_searchLabel"
            Me._searchLabel.Size = New System.Drawing.Size(90, 17)
            Me._searchLabel.TabIndex = 0
            Me._searchLabel.Text = "Cari minuman"
            '
            '_searchTextBox
            '
            Me._searchTextBox.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me._searchTextBox.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._searchTextBox.Location = New System.Drawing.Point(122, 13)
            Me._searchTextBox.Name = "_searchTextBox"
            Me._searchTextBox.PlaceholderText = "Nama minuman atau deskripsi..."
            Me._searchTextBox.Size = New System.Drawing.Size(270, 24)
            Me._searchTextBox.TabIndex = 1
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
            Me._clearSearchBtn.Location = New System.Drawing.Point(400, 11)
            Me._clearSearchBtn.Name = "_clearSearchBtn"
            Me._clearSearchBtn.Size = New System.Drawing.Size(96, 30)
            Me._clearSearchBtn.TabIndex = 2
            Me._clearSearchBtn.Text = "Bersihkan"
            Me._clearSearchBtn.UseVisualStyleBackColor = False
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
            Me._reloadBtn.Size = New System.Drawing.Size(114, 30)
            Me._reloadBtn.TabIndex = 3
            Me._reloadBtn.Text = "Muat Ulang"
            Me._reloadBtn.UseVisualStyleBackColor = False
            '
            '_categoryLabel
            '
            Me._categoryLabel.AutoSize = True
            Me._categoryLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._categoryLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._categoryLabel.Location = New System.Drawing.Point(20, 58)
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
            Me._catAllBtn.Location = New System.Drawing.Point(122, 53)
            Me._catAllBtn.Name = "_catAllBtn"
            Me._catAllBtn.Size = New System.Drawing.Size(76, 30)
            Me._catAllBtn.TabIndex = 5
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
            Me._catCoffeeBtn.Location = New System.Drawing.Point(204, 53)
            Me._catCoffeeBtn.Name = "_catCoffeeBtn"
            Me._catCoffeeBtn.Size = New System.Drawing.Size(74, 30)
            Me._catCoffeeBtn.TabIndex = 6
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
            Me._catNonCoffeeBtn.Location = New System.Drawing.Point(284, 53)
            Me._catNonCoffeeBtn.Name = "_catNonCoffeeBtn"
            Me._catNonCoffeeBtn.Size = New System.Drawing.Size(110, 30)
            Me._catNonCoffeeBtn.TabIndex = 7
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
            Me._catPastryBtn.Location = New System.Drawing.Point(400, 53)
            Me._catPastryBtn.Name = "_catPastryBtn"
            Me._catPastryBtn.Size = New System.Drawing.Size(74, 30)
            Me._catPastryBtn.TabIndex = 8
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
            Me._catDessertBtn.Location = New System.Drawing.Point(480, 53)
            Me._catDessertBtn.Name = "_catDessertBtn"
            Me._catDessertBtn.Size = New System.Drawing.Size(80, 30)
            Me._catDessertBtn.TabIndex = 9
            Me._catDessertBtn.Text = "Dessert"
            Me._catDessertBtn.UseVisualStyleBackColor = False
            '
            '_availableCountLabel
            '
            Me._availableCountLabel.AutoSize = True
            Me._availableCountLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._availableCountLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._availableCountLabel.Location = New System.Drawing.Point(752, 60)
            Me._availableCountLabel.Name = "_availableCountLabel"
            Me._availableCountLabel.Size = New System.Drawing.Size(160, 15)
            Me._availableCountLabel.TabIndex = 10
            Me._availableCountLabel.Text = "Menampilkan item tersedia"
            '
            '_workspaceHost
            '
            Me._workspaceHost.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me._workspaceHost.Controls.Add(Me._builderPane)
            Me._workspaceHost.Controls.Add(Me._menuListPane)
            Me._workspaceHost.Dock = System.Windows.Forms.DockStyle.Fill
            Me._workspaceHost.Location = New System.Drawing.Point(0, 98)
            Me._workspaceHost.Name = "_workspaceHost"
            Me._workspaceHost.Size = New System.Drawing.Size(932, 586)
            Me._workspaceHost.TabIndex = 1
            '
            '_menuListPane
            '
            Me._menuListPane.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me._menuListPane.Controls.Add(Me._drinkCardsPanel)
            Me._menuListPane.Controls.Add(Me._menuListHeaderPanel)
            Me._menuListPane.Dock = System.Windows.Forms.DockStyle.Left
            Me._menuListPane.Location = New System.Drawing.Point(0, 0)
            Me._menuListPane.Name = "_menuListPane"
            Me._menuListPane.Size = New System.Drawing.Size(348, 586)
            Me._menuListPane.TabIndex = 0
            '
            '_menuListHeaderPanel
            '
            Me._menuListHeaderPanel.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me._menuListHeaderPanel.Controls.Add(Me._menuListHeaderLabel)
            Me._menuListHeaderPanel.Dock = System.Windows.Forms.DockStyle.Top
            Me._menuListHeaderPanel.Location = New System.Drawing.Point(0, 0)
            Me._menuListHeaderPanel.Name = "_menuListHeaderPanel"
            Me._menuListHeaderPanel.Padding = New System.Windows.Forms.Padding(18, 10, 12, 4)
            Me._menuListHeaderPanel.Size = New System.Drawing.Size(348, 34)
            Me._menuListHeaderPanel.TabIndex = 0
            '
            '_menuListHeaderLabel
            '
            Me._menuListHeaderLabel.AutoSize = True
            Me._menuListHeaderLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._menuListHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._menuListHeaderLabel.Location = New System.Drawing.Point(18, 10)
            Me._menuListHeaderLabel.Name = "_menuListHeaderLabel"
            Me._menuListHeaderLabel.Size = New System.Drawing.Size(218, 15)
            Me._menuListHeaderLabel.TabIndex = 0
            Me._menuListHeaderLabel.Text = "Pilih minuman untuk atur komposisi"
            '
            '_drinkCardsPanel
            '
            Me._drinkCardsPanel.AutoScroll = True
            Me._drinkCardsPanel.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me._drinkCardsPanel.Dock = System.Windows.Forms.DockStyle.Fill
            Me._drinkCardsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
            Me._drinkCardsPanel.Location = New System.Drawing.Point(0, 34)
            Me._drinkCardsPanel.Name = "_drinkCardsPanel"
            Me._drinkCardsPanel.Padding = New System.Windows.Forms.Padding(16, 4, 12, 16)
            Me._drinkCardsPanel.Size = New System.Drawing.Size(348, 552)
            Me._drinkCardsPanel.TabIndex = 1
            Me._drinkCardsPanel.WrapContents = False
            '
            '_builderPane
            '
            Me._builderPane.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._builderPane.Controls.Add(Me._builderBodyPanel)
            Me._builderPane.Controls.Add(Me._selectedHeaderPanel)
            Me._builderPane.Controls.Add(Me._summaryPanel)
            Me._builderPane.Dock = System.Windows.Forms.DockStyle.Fill
            Me._builderPane.Location = New System.Drawing.Point(348, 0)
            Me._builderPane.Name = "_builderPane"
            Me._builderPane.Size = New System.Drawing.Size(584, 586)
            Me._builderPane.TabIndex = 1
            '
            '_selectedHeaderPanel
            '
            Me._selectedHeaderPanel.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._selectedHeaderPanel.Controls.Add(Me._selectedPhotoBox)
            Me._selectedHeaderPanel.Controls.Add(Me._selectedCategoryLabel)
            Me._selectedHeaderPanel.Controls.Add(Me._selectedAvailabilityTag)
            Me._selectedHeaderPanel.Controls.Add(Me._selectedCustomTag)
            Me._selectedHeaderPanel.Controls.Add(Me._selectedTitleLabel)
            Me._selectedHeaderPanel.Controls.Add(Me._selectedDescriptionLabel)
            Me._selectedHeaderPanel.Controls.Add(Me._selectedBasePriceLabel)
            Me._selectedHeaderPanel.Dock = System.Windows.Forms.DockStyle.Top
            Me._selectedHeaderPanel.Location = New System.Drawing.Point(0, 0)
            Me._selectedHeaderPanel.Name = "_selectedHeaderPanel"
            Me._selectedHeaderPanel.Size = New System.Drawing.Size(584, 114)
            Me._selectedHeaderPanel.TabIndex = 0
            '
            '_selectedPhotoBox
            '
            Me._selectedPhotoBox.BackColor = System.Drawing.Color.FromArgb(243, 232, 220)
            Me._selectedPhotoBox.Location = New System.Drawing.Point(20, 14)
            Me._selectedPhotoBox.Name = "_selectedPhotoBox"
            Me._selectedPhotoBox.Size = New System.Drawing.Size(86, 86)
            Me._selectedPhotoBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me._selectedPhotoBox.TabIndex = 0
            Me._selectedPhotoBox.TabStop = False
            '
            '_selectedCategoryLabel
            '
            Me._selectedCategoryLabel.AutoSize = True
            Me._selectedCategoryLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._selectedCategoryLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._selectedCategoryLabel.Location = New System.Drawing.Point(120, 16)
            Me._selectedCategoryLabel.Name = "_selectedCategoryLabel"
            Me._selectedCategoryLabel.Size = New System.Drawing.Size(44, 15)
            Me._selectedCategoryLabel.TabIndex = 1
            Me._selectedCategoryLabel.Text = "Coffee"
            '
            '_selectedAvailabilityTag
            '
            Me._selectedAvailabilityTag.AutoSize = True
            Me._selectedAvailabilityTag.BackColor = System.Drawing.Color.FromArgb(234, 242, 236)
            Me._selectedAvailabilityTag.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._selectedAvailabilityTag.ForeColor = System.Drawing.Color.FromArgb(47, 79, 56)
            Me._selectedAvailabilityTag.Location = New System.Drawing.Point(174, 13)
            Me._selectedAvailabilityTag.Name = "_selectedAvailabilityTag"
            Me._selectedAvailabilityTag.Padding = New System.Windows.Forms.Padding(8, 2, 8, 2)
            Me._selectedAvailabilityTag.Size = New System.Drawing.Size(66, 19)
            Me._selectedAvailabilityTag.TabIndex = 2
            Me._selectedAvailabilityTag.Text = "Tersedia"
            '
            '_selectedCustomTag
            '
            Me._selectedCustomTag.AutoSize = True
            Me._selectedCustomTag.BackColor = System.Drawing.Color.FromArgb(251, 236, 239)
            Me._selectedCustomTag.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._selectedCustomTag.ForeColor = System.Drawing.Color.FromArgb(143, 53, 71)
            Me._selectedCustomTag.Location = New System.Drawing.Point(248, 13)
            Me._selectedCustomTag.Name = "_selectedCustomTag"
            Me._selectedCustomTag.Padding = New System.Windows.Forms.Padding(8, 2, 8, 2)
            Me._selectedCustomTag.Size = New System.Drawing.Size(96, 19)
            Me._selectedCustomTag.TabIndex = 3
            Me._selectedCustomTag.Text = "Bisa Dikustom"
            '
            '_selectedTitleLabel
            '
            Me._selectedTitleLabel.AutoEllipsis = True
            Me._selectedTitleLabel.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._selectedTitleLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._selectedTitleLabel.Location = New System.Drawing.Point(120, 36)
            Me._selectedTitleLabel.Name = "_selectedTitleLabel"
            Me._selectedTitleLabel.Size = New System.Drawing.Size(444, 24)
            Me._selectedTitleLabel.TabIndex = 4
            Me._selectedTitleLabel.Text = "Pilih minuman dari daftar"
            '
            '_selectedDescriptionLabel
            '
            Me._selectedDescriptionLabel.AutoEllipsis = True
            Me._selectedDescriptionLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._selectedDescriptionLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._selectedDescriptionLabel.Location = New System.Drawing.Point(120, 61)
            Me._selectedDescriptionLabel.Name = "_selectedDescriptionLabel"
            Me._selectedDescriptionLabel.Size = New System.Drawing.Size(444, 18)
            Me._selectedDescriptionLabel.TabIndex = 5
            Me._selectedDescriptionLabel.Text = "Opsi kustomisasi akan dimuat berdasarkan item yang dipilih."
            '
            '_selectedBasePriceLabel
            '
            Me._selectedBasePriceLabel.AutoSize = True
            Me._selectedBasePriceLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._selectedBasePriceLabel.ForeColor = System.Drawing.Color.FromArgb(107, 68, 46)
            Me._selectedBasePriceLabel.Location = New System.Drawing.Point(120, 83)
            Me._selectedBasePriceLabel.Name = "_selectedBasePriceLabel"
            Me._selectedBasePriceLabel.Size = New System.Drawing.Size(134, 17)
            Me._selectedBasePriceLabel.TabIndex = 6
            Me._selectedBasePriceLabel.Text = "Harga dasar: Rp 0"
            '
            '_builderBodyPanel
            '
            Me._builderBodyPanel.AutoScroll = True
            Me._builderBodyPanel.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._builderBodyPanel.Controls.Add(Me._builderFlow)
            Me._builderBodyPanel.Dock = System.Windows.Forms.DockStyle.Fill
            Me._builderBodyPanel.Location = New System.Drawing.Point(0, 114)
            Me._builderBodyPanel.Name = "_builderBodyPanel"
            Me._builderBodyPanel.Padding = New System.Windows.Forms.Padding(20, 12, 20, 16)
            Me._builderBodyPanel.Size = New System.Drawing.Size(584, 244)
            Me._builderBodyPanel.TabIndex = 1
            '
            '_builderFlow
            '
            Me._builderFlow.AutoSize = True
            Me._builderFlow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me._builderFlow.BackColor = System.Drawing.Color.Transparent
            Me._builderFlow.Controls.Add(Me._optionsHostPanel)
            Me._builderFlow.Controls.Add(Me._orderMetaCard)
            Me._builderFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
            Me._builderFlow.Location = New System.Drawing.Point(20, 12)
            Me._builderFlow.Name = "_builderFlow"
            Me._builderFlow.Size = New System.Drawing.Size(526, 210)
            Me._builderFlow.TabIndex = 0
            Me._builderFlow.WrapContents = False
            '
            '_optionsHostPanel
            '
            Me._optionsHostPanel.AutoSize = True
            Me._optionsHostPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me._optionsHostPanel.BackColor = System.Drawing.Color.Transparent
            Me._optionsHostPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
            Me._optionsHostPanel.Location = New System.Drawing.Point(0, 0)
            Me._optionsHostPanel.Margin = New System.Windows.Forms.Padding(0, 0, 0, 12)
            Me._optionsHostPanel.Name = "_optionsHostPanel"
            Me._optionsHostPanel.Size = New System.Drawing.Size(526, 36)
            Me._optionsHostPanel.TabIndex = 0
            Me._optionsHostPanel.WrapContents = False
            '
            '_orderMetaCard
            '
            Me._orderMetaCard.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me._orderMetaCard.Controls.Add(Me._quantitySectionLabel)
            Me._orderMetaCard.Controls.Add(Me._qtyMinusBtn)
            Me._orderMetaCard.Controls.Add(Me._quantityInput)
            Me._orderMetaCard.Controls.Add(Me._qtyPlusBtn)
            Me._orderMetaCard.Controls.Add(Me._quantityHintLabel)
            Me._orderMetaCard.Controls.Add(Me._notesLabel)
            Me._orderMetaCard.Controls.Add(Me._notesTextBox)
            Me._orderMetaCard.Location = New System.Drawing.Point(0, 48)
            Me._orderMetaCard.Margin = New System.Windows.Forms.Padding(0, 0, 0, 12)
            Me._orderMetaCard.Name = "_orderMetaCard"
            Me._orderMetaCard.Size = New System.Drawing.Size(526, 154)
            Me._orderMetaCard.TabIndex = 1
            '
            '_quantitySectionLabel
            '
            Me._quantitySectionLabel.AutoSize = True
            Me._quantitySectionLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._quantitySectionLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._quantitySectionLabel.Location = New System.Drawing.Point(14, 18)
            Me._quantitySectionLabel.Name = "_quantitySectionLabel"
            Me._quantitySectionLabel.Size = New System.Drawing.Size(66, 17)
            Me._quantitySectionLabel.TabIndex = 0
            Me._quantitySectionLabel.Text = "Kuantitas"
            '
            '_qtyMinusBtn
            '
            Me._qtyMinusBtn.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._qtyMinusBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._qtyMinusBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 123, 101)
            Me._qtyMinusBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._qtyMinusBtn.Font = New System.Drawing.Font("Segoe UI", 10.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._qtyMinusBtn.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._qtyMinusBtn.Location = New System.Drawing.Point(96, 12)
            Me._qtyMinusBtn.Name = "_qtyMinusBtn"
            Me._qtyMinusBtn.Size = New System.Drawing.Size(34, 30)
            Me._qtyMinusBtn.TabIndex = 1
            Me._qtyMinusBtn.Text = "-"
            Me._qtyMinusBtn.UseVisualStyleBackColor = False
            '
            '_quantityInput
            '
            Me._quantityInput.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._quantityInput.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._quantityInput.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._quantityInput.Location = New System.Drawing.Point(136, 15)
            Me._quantityInput.Maximum = New Decimal(New Integer() {99, 0, 0, 0})
            Me._quantityInput.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me._quantityInput.Name = "_quantityInput"
            Me._quantityInput.Size = New System.Drawing.Size(64, 24)
            Me._quantityInput.TabIndex = 2
            Me._quantityInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
            Me._quantityInput.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            '_qtyPlusBtn
            '
            Me._qtyPlusBtn.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._qtyPlusBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._qtyPlusBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 123, 101)
            Me._qtyPlusBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._qtyPlusBtn.Font = New System.Drawing.Font("Segoe UI", 10.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._qtyPlusBtn.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._qtyPlusBtn.Location = New System.Drawing.Point(206, 12)
            Me._qtyPlusBtn.Name = "_qtyPlusBtn"
            Me._qtyPlusBtn.Size = New System.Drawing.Size(34, 30)
            Me._qtyPlusBtn.TabIndex = 3
            Me._qtyPlusBtn.Text = "+"
            Me._qtyPlusBtn.UseVisualStyleBackColor = False
            '
            '_quantityHintLabel
            '
            Me._quantityHintLabel.AutoSize = True
            Me._quantityHintLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._quantityHintLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._quantityHintLabel.Location = New System.Drawing.Point(252, 19)
            Me._quantityHintLabel.Name = "_quantityHintLabel"
            Me._quantityHintLabel.Size = New System.Drawing.Size(124, 15)
            Me._quantityHintLabel.TabIndex = 4
            Me._quantityHintLabel.Text = "Minimal 1, maksimal 99"
            '
            '_notesLabel
            '
            Me._notesLabel.AutoSize = True
            Me._notesLabel.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._notesLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._notesLabel.Location = New System.Drawing.Point(14, 54)
            Me._notesLabel.Name = "_notesLabel"
            Me._notesLabel.Size = New System.Drawing.Size(174, 17)
            Me._notesLabel.TabIndex = 5
            Me._notesLabel.Text = "Catatan pesanan (opsional)"
            '
            '_notesTextBox
            '
            Me._notesTextBox.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._notesTextBox.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._notesTextBox.Location = New System.Drawing.Point(14, 76)
            Me._notesTextBox.MaxLength = 200
            Me._notesTextBox.Multiline = True
            Me._notesTextBox.Name = "_notesTextBox"
            Me._notesTextBox.PlaceholderText = "Contoh: kurang manis, pisah es, tanpa sedotan..."
            Me._notesTextBox.Size = New System.Drawing.Size(498, 62)
            Me._notesTextBox.TabIndex = 6
            '
            '_summaryPanel
            '
            Me._summaryPanel.BackColor = System.Drawing.Color.FromArgb(243, 232, 220)
            Me._summaryPanel.Controls.Add(Me._summaryTitleLabel)
            Me._summaryPanel.Controls.Add(Me._summaryOptionsLabel)
            Me._summaryPanel.Controls.Add(Me._basePriceCaptionLabel)
            Me._summaryPanel.Controls.Add(Me._basePriceValueLabel)
            Me._summaryPanel.Controls.Add(Me._extraPriceCaptionLabel)
            Me._summaryPanel.Controls.Add(Me._extraPriceValueLabel)
            Me._summaryPanel.Controls.Add(Me._unitTotalCaptionLabel)
            Me._summaryPanel.Controls.Add(Me._unitTotalValueLabel)
            Me._summaryPanel.Controls.Add(Me._subtotalCaptionLabel)
            Me._summaryPanel.Controls.Add(Me._subtotalValueLabel)
            Me._summaryPanel.Controls.Add(Me._feedbackBanner)
            Me._summaryPanel.Controls.Add(Me._actionRowPanel)
            Me._summaryPanel.Dock = System.Windows.Forms.DockStyle.Bottom
            Me._summaryPanel.Location = New System.Drawing.Point(0, 378)
            Me._summaryPanel.Name = "_summaryPanel"
            Me._summaryPanel.Size = New System.Drawing.Size(584, 208)
            Me._summaryPanel.TabIndex = 2
            '
            '_summaryTitleLabel
            '
            Me._summaryTitleLabel.AutoSize = True
            Me._summaryTitleLabel.Font = New System.Drawing.Font("Segoe UI", 10.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._summaryTitleLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._summaryTitleLabel.Location = New System.Drawing.Point(20, 12)
            Me._summaryTitleLabel.Name = "_summaryTitleLabel"
            Me._summaryTitleLabel.Size = New System.Drawing.Size(134, 19)
            Me._summaryTitleLabel.TabIndex = 0
            Me._summaryTitleLabel.Text = "Ringkasan Pilihan"
            '
            '_summaryOptionsLabel
            '
            Me._summaryOptionsLabel.AutoEllipsis = True
            Me._summaryOptionsLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._summaryOptionsLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._summaryOptionsLabel.Location = New System.Drawing.Point(20, 34)
            Me._summaryOptionsLabel.Name = "_summaryOptionsLabel"
            Me._summaryOptionsLabel.Size = New System.Drawing.Size(544, 30)
            Me._summaryOptionsLabel.TabIndex = 1
            Me._summaryOptionsLabel.Text = "Belum ada item dipilih."
            '
            '_basePriceCaptionLabel
            '
            Me._basePriceCaptionLabel.AutoSize = True
            Me._basePriceCaptionLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._basePriceCaptionLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._basePriceCaptionLabel.Location = New System.Drawing.Point(20, 68)
            Me._basePriceCaptionLabel.Name = "_basePriceCaptionLabel"
            Me._basePriceCaptionLabel.Size = New System.Drawing.Size(69, 15)
            Me._basePriceCaptionLabel.TabIndex = 2
            Me._basePriceCaptionLabel.Text = "Harga dasar"
            '
            '_basePriceValueLabel
            '
            Me._basePriceValueLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._basePriceValueLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._basePriceValueLabel.Location = New System.Drawing.Point(414, 68)
            Me._basePriceValueLabel.Name = "_basePriceValueLabel"
            Me._basePriceValueLabel.Size = New System.Drawing.Size(150, 16)
            Me._basePriceValueLabel.TabIndex = 3
            Me._basePriceValueLabel.Text = "Rp 0"
            Me._basePriceValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            '_extraPriceCaptionLabel
            '
            Me._extraPriceCaptionLabel.AutoSize = True
            Me._extraPriceCaptionLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me._extraPriceCaptionLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._extraPriceCaptionLabel.Location = New System.Drawing.Point(20, 87)
            Me._extraPriceCaptionLabel.Name = "_extraPriceCaptionLabel"
            Me._extraPriceCaptionLabel.Size = New System.Drawing.Size(83, 15)
            Me._extraPriceCaptionLabel.TabIndex = 4
            Me._extraPriceCaptionLabel.Text = "Tambahan opsi"
            '
            '_extraPriceValueLabel
            '
            Me._extraPriceValueLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._extraPriceValueLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._extraPriceValueLabel.Location = New System.Drawing.Point(414, 87)
            Me._extraPriceValueLabel.Name = "_extraPriceValueLabel"
            Me._extraPriceValueLabel.Size = New System.Drawing.Size(150, 16)
            Me._extraPriceValueLabel.TabIndex = 5
            Me._extraPriceValueLabel.Text = "Rp 0"
            Me._extraPriceValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            '_unitTotalCaptionLabel
            '
            Me._unitTotalCaptionLabel.AutoSize = True
            Me._unitTotalCaptionLabel.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._unitTotalCaptionLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._unitTotalCaptionLabel.Location = New System.Drawing.Point(20, 106)
            Me._unitTotalCaptionLabel.Name = "_unitTotalCaptionLabel"
            Me._unitTotalCaptionLabel.Size = New System.Drawing.Size(78, 15)
            Me._unitTotalCaptionLabel.TabIndex = 6
            Me._unitTotalCaptionLabel.Text = "Total per unit"
            '
            '_unitTotalValueLabel
            '
            Me._unitTotalValueLabel.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._unitTotalValueLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._unitTotalValueLabel.Location = New System.Drawing.Point(414, 105)
            Me._unitTotalValueLabel.Name = "_unitTotalValueLabel"
            Me._unitTotalValueLabel.Size = New System.Drawing.Size(150, 18)
            Me._unitTotalValueLabel.TabIndex = 7
            Me._unitTotalValueLabel.Text = "Rp 0"
            Me._unitTotalValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            '_subtotalCaptionLabel
            '
            Me._subtotalCaptionLabel.AutoSize = True
            Me._subtotalCaptionLabel.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._subtotalCaptionLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._subtotalCaptionLabel.Location = New System.Drawing.Point(20, 134)
            Me._subtotalCaptionLabel.Name = "_subtotalCaptionLabel"
            Me._subtotalCaptionLabel.Size = New System.Drawing.Size(89, 19)
            Me._subtotalCaptionLabel.TabIndex = 8
            Me._subtotalCaptionLabel.Text = "Subtotal (1x)"
            '
            '_subtotalValueLabel
            '
            Me._subtotalValueLabel.Font = New System.Drawing.Font("Segoe UI", 13.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._subtotalValueLabel.ForeColor = System.Drawing.Color.FromArgb(107, 68, 46)
            Me._subtotalValueLabel.Location = New System.Drawing.Point(364, 130)
            Me._subtotalValueLabel.Name = "_subtotalValueLabel"
            Me._subtotalValueLabel.Size = New System.Drawing.Size(200, 26)
            Me._subtotalValueLabel.TabIndex = 9
            Me._subtotalValueLabel.Text = "Rp 0"
            Me._subtotalValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            '_feedbackBanner
            '
            Me._feedbackBanner.AutoEllipsis = True
            Me._feedbackBanner.Font = New System.Drawing.Font("Segoe UI", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._feedbackBanner.Location = New System.Drawing.Point(20, 160)
            Me._feedbackBanner.Name = "_feedbackBanner"
            Me._feedbackBanner.Padding = New System.Windows.Forms.Padding(8, 3, 8, 3)
            Me._feedbackBanner.Size = New System.Drawing.Size(544, 24)
            Me._feedbackBanner.TabIndex = 10
            Me._feedbackBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me._feedbackBanner.Visible = False
            '
            '_actionRowPanel
            '
            Me._actionRowPanel.BackColor = System.Drawing.Color.Transparent
            Me._actionRowPanel.Controls.Add(Me._handoffBtn)
            Me._actionRowPanel.Controls.Add(Me._resetOptionsBtn)
            Me._actionRowPanel.Controls.Add(Me._viewDraftsBtn)
            Me._actionRowPanel.Location = New System.Drawing.Point(20, 162)
            Me._actionRowPanel.Name = "_actionRowPanel"
            Me._actionRowPanel.Size = New System.Drawing.Size(544, 36)
            Me._actionRowPanel.TabIndex = 11
            '
            '_handoffBtn
            '
            Me._handoffBtn.BackColor = System.Drawing.Color.FromArgb(107, 68, 46)
            Me._handoffBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._handoffBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(107, 68, 46)
            Me._handoffBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._handoffBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._handoffBtn.ForeColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._handoffBtn.Location = New System.Drawing.Point(0, 0)
            Me._handoffBtn.Name = "_handoffBtn"
            Me._handoffBtn.Size = New System.Drawing.Size(196, 34)
            Me._handoffBtn.TabIndex = 0
            Me._handoffBtn.Text = "Siapkan Draft Pesanan"
            Me._handoffBtn.UseVisualStyleBackColor = False
            '
            '_resetOptionsBtn
            '
            Me._resetOptionsBtn.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._resetOptionsBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._resetOptionsBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 123, 101)
            Me._resetOptionsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._resetOptionsBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._resetOptionsBtn.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._resetOptionsBtn.Location = New System.Drawing.Point(204, 0)
            Me._resetOptionsBtn.Name = "_resetOptionsBtn"
            Me._resetOptionsBtn.Size = New System.Drawing.Size(118, 34)
            Me._resetOptionsBtn.TabIndex = 1
            Me._resetOptionsBtn.Text = "Reset Pilihan"
            Me._resetOptionsBtn.UseVisualStyleBackColor = False
            '
            '_viewDraftsBtn
            '
            Me._viewDraftsBtn.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._viewDraftsBtn.Cursor = System.Windows.Forms.Cursors.Hand
            Me._viewDraftsBtn.Enabled = False
            Me._viewDraftsBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(150, 123, 101)
            Me._viewDraftsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me._viewDraftsBtn.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
            Me._viewDraftsBtn.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._viewDraftsBtn.Location = New System.Drawing.Point(330, 0)
            Me._viewDraftsBtn.Name = "_viewDraftsBtn"
            Me._viewDraftsBtn.Size = New System.Drawing.Size(134, 34)
            Me._viewDraftsBtn.TabIndex = 2
            Me._viewDraftsBtn.Text = "Lihat Draft (0)"
            Me._viewDraftsBtn.UseVisualStyleBackColor = False
            '
            'DrinkCreatorControl
            '
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me.Controls.Add(Me._workspaceHost)
            Me.Controls.Add(Me._toolbarPanel)
            Me.DoubleBuffered = True
            Me.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
            Me.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me.Name = "DrinkCreatorControl"
            Me.Size = New System.Drawing.Size(932, 684)
            Me._toolbarPanel.ResumeLayout(False)
            Me._toolbarPanel.PerformLayout()
            Me._workspaceHost.ResumeLayout(False)
            Me._builderPane.ResumeLayout(False)
            Me._builderBodyPanel.ResumeLayout(False)
            Me._builderBodyPanel.PerformLayout()
            Me._builderFlow.ResumeLayout(False)
            Me._builderFlow.PerformLayout()
            Me._orderMetaCard.ResumeLayout(False)
            Me._orderMetaCard.PerformLayout()
            CType(Me._quantityInput, System.ComponentModel.ISupportInitialize).EndInit()
            Me._selectedHeaderPanel.ResumeLayout(False)
            Me._selectedHeaderPanel.PerformLayout()
            CType(Me._selectedPhotoBox, System.ComponentModel.ISupportInitialize).EndInit()
            Me._summaryPanel.ResumeLayout(False)
            Me._summaryPanel.PerformLayout()
            Me._actionRowPanel.ResumeLayout(False)
            Me._menuListPane.ResumeLayout(False)
            Me._menuListHeaderPanel.ResumeLayout(False)
            Me._menuListHeaderPanel.PerformLayout()
            Me.ResumeLayout(False)
        End Sub

        Friend WithEvents _toolbarPanel As System.Windows.Forms.Panel
        Friend WithEvents _searchLabel As System.Windows.Forms.Label
        Friend WithEvents _searchTextBox As System.Windows.Forms.TextBox
        Friend WithEvents _clearSearchBtn As System.Windows.Forms.Button
        Friend WithEvents _reloadBtn As System.Windows.Forms.Button
        Friend WithEvents _categoryLabel As System.Windows.Forms.Label
        Friend WithEvents _catAllBtn As System.Windows.Forms.Button
        Friend WithEvents _catCoffeeBtn As System.Windows.Forms.Button
        Friend WithEvents _catNonCoffeeBtn As System.Windows.Forms.Button
        Friend WithEvents _catPastryBtn As System.Windows.Forms.Button
        Friend WithEvents _catDessertBtn As System.Windows.Forms.Button
        Friend WithEvents _availableCountLabel As System.Windows.Forms.Label
        Friend WithEvents _workspaceHost As System.Windows.Forms.Panel
        Friend WithEvents _menuListPane As System.Windows.Forms.Panel
        Friend WithEvents _menuListHeaderPanel As System.Windows.Forms.Panel
        Friend WithEvents _menuListHeaderLabel As System.Windows.Forms.Label
        Friend WithEvents _drinkCardsPanel As System.Windows.Forms.FlowLayoutPanel
        Friend WithEvents _builderPane As System.Windows.Forms.Panel
        Friend WithEvents _selectedHeaderPanel As System.Windows.Forms.Panel
        Friend WithEvents _selectedPhotoBox As System.Windows.Forms.PictureBox
        Friend WithEvents _selectedCategoryLabel As System.Windows.Forms.Label
        Friend WithEvents _selectedAvailabilityTag As System.Windows.Forms.Label
        Friend WithEvents _selectedCustomTag As System.Windows.Forms.Label
        Friend WithEvents _selectedTitleLabel As System.Windows.Forms.Label
        Friend WithEvents _selectedDescriptionLabel As System.Windows.Forms.Label
        Friend WithEvents _selectedBasePriceLabel As System.Windows.Forms.Label
        Friend WithEvents _builderBodyPanel As System.Windows.Forms.Panel
        Friend WithEvents _builderFlow As System.Windows.Forms.FlowLayoutPanel
        Friend WithEvents _optionsHostPanel As System.Windows.Forms.FlowLayoutPanel
        Friend WithEvents _orderMetaCard As System.Windows.Forms.Panel
        Friend WithEvents _quantitySectionLabel As System.Windows.Forms.Label
        Friend WithEvents _qtyMinusBtn As System.Windows.Forms.Button
        Friend WithEvents _quantityInput As System.Windows.Forms.NumericUpDown
        Friend WithEvents _qtyPlusBtn As System.Windows.Forms.Button
        Friend WithEvents _quantityHintLabel As System.Windows.Forms.Label
        Friend WithEvents _notesLabel As System.Windows.Forms.Label
        Friend WithEvents _notesTextBox As System.Windows.Forms.TextBox
        Friend WithEvents _summaryPanel As System.Windows.Forms.Panel
        Friend WithEvents _summaryTitleLabel As System.Windows.Forms.Label
        Friend WithEvents _summaryOptionsLabel As System.Windows.Forms.Label
        Friend WithEvents _basePriceCaptionLabel As System.Windows.Forms.Label
        Friend WithEvents _basePriceValueLabel As System.Windows.Forms.Label
        Friend WithEvents _extraPriceCaptionLabel As System.Windows.Forms.Label
        Friend WithEvents _extraPriceValueLabel As System.Windows.Forms.Label
        Friend WithEvents _unitTotalCaptionLabel As System.Windows.Forms.Label
        Friend WithEvents _unitTotalValueLabel As System.Windows.Forms.Label
        Friend WithEvents _subtotalCaptionLabel As System.Windows.Forms.Label
        Friend WithEvents _subtotalValueLabel As System.Windows.Forms.Label
        Friend WithEvents _feedbackBanner As System.Windows.Forms.Label
        Friend WithEvents _actionRowPanel As System.Windows.Forms.Panel
        Friend WithEvents _handoffBtn As System.Windows.Forms.Button
        Friend WithEvents _resetOptionsBtn As System.Windows.Forms.Button
        Friend WithEvents _viewDraftsBtn As System.Windows.Forms.Button
    End Class
End Namespace
