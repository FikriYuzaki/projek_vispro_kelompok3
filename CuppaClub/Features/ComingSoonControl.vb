Option Strict On
Option Explicit On

Imports CuppaClub.Core

Namespace Features
    Public Class ComingSoonControl
        Inherits UserControl

        Private ReadOnly _feature As FeatureDefinition
        Private ReadOnly _cardPanel As New Panel()

        Public Sub New(feature As FeatureDefinition)
            If feature Is Nothing Then
                Throw New ArgumentNullException(NameOf(feature))
            End If

            _feature = feature
            DoubleBuffered = True
            BackColor = AppTheme.CanvasCream
            AutoScroll = True
            Padding = New Padding(AppTheme.SpaceLg)

            BuildLayout()
            AddHandler Resize, AddressOf OnContainerResize
        End Sub

        Private Sub BuildLayout()
            _cardPanel.Width = 620
            _cardPanel.BackColor = AppTheme.SurfaceMilk
            _cardPanel.Padding = New Padding(28, 26, 28, 26)
            AddHandler _cardPanel.Paint, AddressOf OnCardPaint

            Dim layout As New FlowLayoutPanel With {
                .Dock = DockStyle.Fill,
                .FlowDirection = FlowDirection.TopDown,
                .WrapContents = False,
                .AutoSize = True,
                .BackColor = Color.Transparent
            }

            Dim statusLabel As New Label()
            Dim statusText = FeatureRegistry.StatusLabel(_feature.Status)
            If _feature.Status = FeatureStatus.Disabled Then
                AppTheme.ApplyStatusTag(statusLabel, "Status: " & statusText, AppTheme.WarningTint, AppTheme.WarningText)
            Else
                AppTheme.ApplyStatusTag(statusLabel, "Status: " & statusText, AppTheme.TaupeTint, AppTheme.TaupeText)
            End If
            statusLabel.Margin = New Padding(0, 0, 0, 12)

            Dim titleLabel As New Label With {
                .Text = _feature.DisplayName,
                .Font = AppTheme.FontPageTitle,
                .ForeColor = AppTheme.TextPrimary,
                .AutoSize = True,
                .MaximumSize = New Size(556, 0),
                .Margin = New Padding(0, 0, 0, 4)
            }

            Dim ownerLabel As New Label With {
                .Text = "Penanggung jawab modul: " & _feature.Owner,
                .Font = AppTheme.FontBodyBold,
                .ForeColor = AppTheme.AccentStrawberryText,
                .AutoSize = True,
                .Margin = New Padding(0, 0, 0, 14)
            }

            Dim summaryLabel As New Label With {
                .Text = _feature.Summary,
                .Font = AppTheme.FontBody,
                .ForeColor = AppTheme.TextSecondary,
                .AutoSize = True,
                .MaximumSize = New Size(556, 0),
                .Margin = New Padding(0, 0, 0, 18)
            }

            Dim divider As New Panel With {
                .Width = 556,
                .Height = 1,
                .BackColor = AppTheme.DividerAlmond,
                .Margin = New Padding(0, 0, 0, 16)
            }

            Dim scopeHeader As New Label With {
                .Text = "Ruang lingkup yang sedang disiapkan",
                .Font = AppTheme.FontSectionTitle,
                .ForeColor = AppTheme.TextPrimary,
                .AutoSize = True,
                .Margin = New Padding(0, 0, 0, 8)
            }

            layout.Controls.Add(statusLabel)
            layout.Controls.Add(titleLabel)
            layout.Controls.Add(ownerLabel)
            layout.Controls.Add(summaryLabel)
            layout.Controls.Add(divider)
            layout.Controls.Add(scopeHeader)

            For Each item In _feature.PlannedItems
                Dim bullet As New Label With {
                    .Text = "- " & item,
                    .Font = AppTheme.FontBody,
                    .ForeColor = AppTheme.TextPrimary,
                    .AutoSize = True,
                    .MaximumSize = New Size(556, 0),
                    .Margin = New Padding(4, 0, 0, 6)
                }
                layout.Controls.Add(bullet)
            Next

            Dim honestyNote As New Label With {
                .Text = "Modul ini sengaja ditampilkan sebagai Coming Soon pada Feature Registry hingga implementasi nyata selesai, diuji, dan diintegrasikan ke solusi utama.",
                .Font = AppTheme.FontCaption,
                .ForeColor = AppTheme.TextSecondary,
                .BackColor = AppTheme.SidebarOat,
                .Padding = New Padding(12, 10, 12, 10),
                .AutoSize = True,
                .MaximumSize = New Size(556, 0),
                .Margin = New Padding(0, 16, 0, 0)
            }
            layout.Controls.Add(honestyNote)

            _cardPanel.Controls.Add(layout)
            _cardPanel.Height = layout.PreferredSize.Height + _cardPanel.Padding.Vertical + 4
            Controls.Add(_cardPanel)
            CenterCard()
        End Sub

        Private Sub OnContainerResize(sender As Object, e As EventArgs)
            CenterCard()
        End Sub

        Private Sub CenterCard()
            Dim availableWidth = Math.Max(320, ClientSize.Width - AppTheme.SpaceLg * 2)
            Dim targetWidth = Math.Min(620, availableWidth)
            _cardPanel.Width = targetWidth
            Dim x = Math.Max(AppTheme.SpaceLg, (ClientSize.Width - _cardPanel.Width) \ 2)
            Dim y = Math.Max(AppTheme.SpaceLg, (ClientSize.Height - _cardPanel.Height) \ 2)
            _cardPanel.Location = New Point(x, y)
        End Sub

        Private Sub OnCardPaint(sender As Object, e As PaintEventArgs)
            Dim rect = New Rectangle(0, 0, _cardPanel.Width - 1, _cardPanel.Height - 1)
            Using pen As New Pen(AppTheme.DividerAlmond, 1.0F)
                e.Graphics.DrawRectangle(pen, rect)
            End Using
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing Then
                RemoveHandler Resize, AddressOf OnContainerResize
                RemoveHandler _cardPanel.Paint, AddressOf OnCardPaint
            End If
            MyBase.Dispose(disposing)
        End Sub
    End Class
End Namespace
