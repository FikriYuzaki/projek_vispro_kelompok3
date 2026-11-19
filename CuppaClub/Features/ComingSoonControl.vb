Option Strict On
Option Explicit On

Imports System.ComponentModel
Imports CuppaClub.Core

Namespace Features
    Public Class ComingSoonControl
        Inherits UserControl

        Private _feature As FeatureDefinition
        Private _runtimeInitialized As Boolean

        Public Sub New()
            InitializeComponent()

            If LicenseManager.UsageMode = LicenseUsageMode.Designtime OrElse DesignMode Then
                Return
            End If

            _runtimeInitialized = True
            AddHandler _cardPanel.Paint, AddressOf OnCardPaint
            AddHandler Resize, AddressOf OnContainerResize
            CenterCard()
        End Sub

        Public Sub New(feature As FeatureDefinition)
            Me.New()
            If feature Is Nothing Then
                Throw New ArgumentNullException(NameOf(feature))
            End If

            _feature = feature
            ApplyFeature(feature)
        End Sub

        Private Sub ApplyFeature(feature As FeatureDefinition)
            Dim statusText = FeatureRegistry.StatusLabel(feature.Status)
            If feature.Status = FeatureStatus.Disabled Then
                AppTheme.ApplyStatusTag(_statusLabel, "Status: " & statusText, AppTheme.WarningTint, AppTheme.WarningText)
            Else
                AppTheme.ApplyStatusTag(_statusLabel, "Status: " & statusText, AppTheme.TaupeTint, AppTheme.TaupeText)
            End If

            _titleLabel.Text = feature.DisplayName
            _ownerLabel.Text = "Penanggung jawab modul: " & feature.Owner
            _summaryLabel.Text = feature.Summary

            Dim lines = feature.PlannedItems.Select(Function(item) "- " & item)
            _plannedListLabel.Text = String.Join(Environment.NewLine, lines)

            _cardPanel.Height = _layoutFlow.PreferredSize.Height + _cardPanel.Padding.Vertical + 6
            CenterCard()
        End Sub

        Private Sub OnContainerResize(sender As Object, e As EventArgs)
            CenterCard()
        End Sub

        Private Sub CenterCard()
            Dim availableWidth = Math.Max(320, ClientSize.Width - AppTheme.SpaceLg * 2)
            Dim targetWidth = Math.Min(620, availableWidth)
            _cardPanel.Width = targetWidth
            _cardPanel.Height = _layoutFlow.PreferredSize.Height + _cardPanel.Padding.Vertical + 6
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

        Private Sub CleanupRuntimeResources()
            If _runtimeInitialized Then
                RemoveHandler Resize, AddressOf OnContainerResize
                RemoveHandler _cardPanel.Paint, AddressOf OnCardPaint
                _runtimeInitialized = False
            End If
        End Sub
    End Class
End Namespace
