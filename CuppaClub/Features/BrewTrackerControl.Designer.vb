Option Strict On
Option Explicit On

Namespace Features
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class BrewTrackerControl
        Inherits System.Windows.Forms.UserControl

        Private components As System.ComponentModel.IContainer

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

        Private _headerPanel As Panel
        Private _titleLabel As Label
        Private _subtitleLabel As Label
        Private _refreshButton As Button
        Private _lastUpdatedLabel As Label

        Private _statePanel As Panel
        Private _stateLabel As Label

        Private _workspacePanel As Panel
        Private _staffSplitContainer As SplitContainer
        Private _queuePanel As Panel
        Private _queueTitleLabel As Label
        Private _queueSubtitleLabel As Label
        Private _queueFlowPanel As FlowLayoutPanel

        Private _detailPanel As Panel
        Private _detailTitleLabel As Label
        Private _detailContentPanel As Panel

        Private _customerPanel As Panel
        Private _customerContentPanel As Panel

        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()

            Me.components = New System.ComponentModel.Container()

            Me._headerPanel = New Panel()
            Me._titleLabel = New Label()
            Me._subtitleLabel = New Label()
            Me._refreshButton = New Button()
            Me._lastUpdatedLabel = New Label()

            Me._statePanel = New Panel()
            Me._stateLabel = New Label()

            Me._workspacePanel = New Panel()
            Me._staffSplitContainer = New SplitContainer()

            Me._queuePanel = New Panel()
            Me._queueTitleLabel = New Label()
            Me._queueSubtitleLabel = New Label()
            Me._queueFlowPanel = New FlowLayoutPanel()

            Me._detailPanel = New Panel()
            Me._detailTitleLabel = New Label()
            Me._detailContentPanel = New Panel()

            Me._customerPanel = New Panel()
            Me._customerContentPanel = New Panel()

            CType(Me._staffSplitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
            Me._staffSplitContainer.Panel1.SuspendLayout()
            Me._staffSplitContainer.Panel2.SuspendLayout()
            Me._staffSplitContainer.SuspendLayout()

            Me._headerPanel.SuspendLayout()
            Me._statePanel.SuspendLayout()
            Me._queuePanel.SuspendLayout()
            Me._detailPanel.SuspendLayout()
            Me._customerPanel.SuspendLayout()
            Me.SuspendLayout()

            '
            ' BrewTrackerControl
            '
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)
            Me.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Name = "BrewTrackerControl"
            Me.Size = New System.Drawing.Size(932, 620)

            '
            ' Header Panel
            '
            Me._headerPanel.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._headerPanel.Dock = System.Windows.Forms.DockStyle.Top
            Me._headerPanel.Height = 86
            Me._headerPanel.Padding = New Padding(20, 14, 20, 10)
            Me._headerPanel.Controls.Add(Me._titleLabel)
            Me._headerPanel.Controls.Add(Me._subtitleLabel)
            Me._headerPanel.Controls.Add(Me._refreshButton)
            Me._headerPanel.Controls.Add(Me._lastUpdatedLabel)

            '
            ' Title
            '
            Me._titleLabel.AutoSize = True
            Me._titleLabel.Font = New System.Drawing.Font(
                "Segoe UI", 16.0!, FontStyle.Bold)
            Me._titleLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._titleLabel.Location = New Point(20, 12)
            Me._titleLabel.Text = "Live Brew Tracker"

            '
            ' Subtitle
            '
            Me._subtitleLabel.AutoSize = True
            Me._subtitleLabel.Font = New System.Drawing.Font(
                "Segoe UI", 9.5!)
            Me._subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._subtitleLabel.Location = New Point(22, 52)
            Me._subtitleLabel.Text = "Pantau pesanan dan progress penyajian"

            '
            ' Refresh Button
            '
            Me._refreshButton.Anchor =
                AnchorStyles.Top Or AnchorStyles.Right
            Me._refreshButton.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._refreshButton.FlatStyle = FlatStyle.Flat
            Me._refreshButton.FlatAppearance.BorderColor =
                System.Drawing.Color.FromArgb(150, 123, 101)
            Me._refreshButton.Font = New System.Drawing.Font(
                "Segoe UI", 9.5!, FontStyle.Bold)
            Me._refreshButton.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._refreshButton.Location = New Point(780, 15)
            Me._refreshButton.Size = New Size(112, 32)
            Me._refreshButton.Text = "Muat Ulang"
            Me._refreshButton.Cursor = Cursors.Hand
            Me._refreshButton.UseVisualStyleBackColor = False

            '
            ' Last Updated
            '
            Me._lastUpdatedLabel.Anchor =
                AnchorStyles.Top Or AnchorStyles.Right
            Me._lastUpdatedLabel.AutoSize = True
            Me._lastUpdatedLabel.Font = New System.Drawing.Font(
                "Segoe UI", 8.5!)
            Me._lastUpdatedLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._lastUpdatedLabel.Location = New Point(780, 54)
            Me._lastUpdatedLabel.Text = "Belum diperbarui"

            '
            ' State Panel
            '
            Me._statePanel.BackColor = System.Drawing.Color.FromArgb(243, 232, 220)
            Me._statePanel.Dock = System.Windows.Forms.DockStyle.Top
            Me._statePanel.Height = 38
            Me._statePanel.Padding = New Padding(20, 0, 20, 0)
            Me._statePanel.Controls.Add(Me._stateLabel)

            '
            ' State Label
            '
            Me._stateLabel.Dock = DockStyle.Fill
            Me._stateLabel.Font = New System.Drawing.Font(
                "Segoe UI", 9.0!, FontStyle.Regular)
            Me._stateLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._stateLabel.TextAlign = ContentAlignment.MiddleLeft
            Me._stateLabel.Text = ""

            '
            ' Workspace Panel
            '
            Me._workspacePanel.Dock = DockStyle.Fill
            Me._workspacePanel.Padding = New Padding(12)
            Me._workspacePanel.BackColor = System.Drawing.Color.FromArgb(250, 244, 236)

            '
            ' Staff Split Container
            '
            Me._staffSplitContainer.Dock = DockStyle.Fill
            Me._staffSplitContainer.BackColor = System.Drawing.Color.FromArgb(226, 211, 195)
            Me._staffSplitContainer.BorderStyle = BorderStyle.None
            Me._staffSplitContainer.FixedPanel = FixedPanel.Panel1
            Me._staffSplitContainer.IsSplitterFixed = False
            Me._staffSplitContainer.SplitterWidth = 6
	    Me._staffSplitContainer.Panel1MinSize = 0
            Me._staffSplitContainer.Panel2MinSize = 0
            Me._staffSplitContainer.SplitterDistance = 100

            '
            ' Queue Panel
            '
            Me._queuePanel.Dock = DockStyle.Fill
            Me._queuePanel.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._queuePanel.Padding = New Padding(12)

            Me._queuePanel.Controls.Add(Me._queueFlowPanel)
            Me._queuePanel.Controls.Add(Me._queueSubtitleLabel)
            Me._queuePanel.Controls.Add(Me._queueTitleLabel)

            '
            ' Queue Title
            '
            Me._queueTitleLabel.Dock = DockStyle.Top
            Me._queueTitleLabel.Height = 30
            Me._queueTitleLabel.Font = New System.Drawing.Font(
                "Segoe UI", 11.5!, FontStyle.Bold)
            Me._queueTitleLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._queueTitleLabel.Text = "Antrean Pesanan"
            Me._queueTitleLabel.TextAlign = ContentAlignment.MiddleLeft

            '
            ' Queue Subtitle
            '
            Me._queueSubtitleLabel.Dock = DockStyle.Top
            Me._queueSubtitleLabel.Height = 24
            Me._queueSubtitleLabel.Font = New System.Drawing.Font(
                "Segoe UI", 8.5!)
            Me._queueSubtitleLabel.ForeColor = System.Drawing.Color.FromArgb(110, 86, 71)
            Me._queueSubtitleLabel.Text = "Pilih pesanan untuk melihat detail"
            Me._queueSubtitleLabel.TextAlign = ContentAlignment.MiddleLeft

            '
            ' Queue Flow Panel
            '
            Me._queueFlowPanel.Dock = DockStyle.Fill
            Me._queueFlowPanel.FlowDirection = FlowDirection.TopDown
            Me._queueFlowPanel.WrapContents = False
            Me._queueFlowPanel.AutoScroll = True
            Me._queueFlowPanel.Padding = New Padding(2, 8, 2, 2)
            Me._queueFlowPanel.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)

            '
            ' Detail Panel
            '
            Me._detailPanel.Dock = DockStyle.Fill
            Me._detailPanel.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._detailPanel.Padding = New Padding(16)

            Me._detailPanel.Controls.Add(Me._detailContentPanel)
            Me._detailPanel.Controls.Add(Me._detailTitleLabel)

            '
            ' Detail Title
            '
            Me._detailTitleLabel.Dock = DockStyle.Top
            Me._detailTitleLabel.Height = 36
            Me._detailTitleLabel.Font = New System.Drawing.Font(
                "Segoe UI", 11.5!, FontStyle.Bold)
            Me._detailTitleLabel.ForeColor = System.Drawing.Color.FromArgb(59, 38, 28)
            Me._detailTitleLabel.Text = "Detail Pesanan"
            Me._detailTitleLabel.TextAlign = ContentAlignment.MiddleLeft

            '
            ' Detail Content
            '
            Me._detailContentPanel.Dock = DockStyle.Fill
            Me._detailContentPanel.AutoScroll = True
            Me._detailContentPanel.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)

            '
            ' Customer Panel
            '
            Me._customerPanel.Dock = DockStyle.Fill
            Me._customerPanel.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)
            Me._customerPanel.Padding = New Padding(20)

            Me._customerPanel.Controls.Add(Me._customerContentPanel)

            '
            ' Customer Content
            '
            Me._customerContentPanel.Dock = DockStyle.Fill
            Me._customerContentPanel.AutoScroll = True
            Me._customerContentPanel.BackColor = System.Drawing.Color.FromArgb(255, 253, 249)

            '
            ' Add split container panels
            '
            Me._staffSplitContainer.Panel1.Controls.Add(Me._queuePanel)
            Me._staffSplitContainer.Panel2.Controls.Add(Me._detailPanel)

            '
            ' Add workspace views
            '
            Me._workspacePanel.Controls.Add(Me._customerPanel)
            Me._workspacePanel.Controls.Add(Me._staffSplitContainer)

            '
            ' Add root controls
            '
            Me.Controls.Add(Me._workspacePanel)
            Me.Controls.Add(Me._statePanel)
            Me.Controls.Add(Me._headerPanel)

            '
            ' Resume Layout
            '
            Me._headerPanel.ResumeLayout(False)
            Me._headerPanel.PerformLayout()

            Me._statePanel.ResumeLayout(False)

            Me._queuePanel.ResumeLayout(False)
            Me._detailPanel.ResumeLayout(False)
            Me._customerPanel.ResumeLayout(False)

            Me._staffSplitContainer.Panel1.ResumeLayout(False)
            Me._staffSplitContainer.Panel2.ResumeLayout(False)
            CType(Me._staffSplitContainer, System.ComponentModel.ISupportInitialize).EndInit()
            Me._staffSplitContainer.ResumeLayout(False)

            Me.ResumeLayout(False)

        End Sub

    End Class
End Namespace