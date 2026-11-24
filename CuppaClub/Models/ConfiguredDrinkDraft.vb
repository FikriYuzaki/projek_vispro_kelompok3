Option Strict On
Option Explicit On

Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports CuppaClub.Core

Namespace Models
    Public NotInheritable Class DrinkOptionSnapshotItem
        <JsonPropertyName("option_group")>
        Public Property OptionGroup As String = String.Empty

        <JsonPropertyName("option_id")>
        Public Property OptionId As String = String.Empty

        <JsonPropertyName("option_name")>
        Public Property OptionName As String = String.Empty

        <JsonPropertyName("extra_price")>
        Public Property ExtraPrice As Decimal

        Public Function Clone() As DrinkOptionSnapshotItem
            Return New DrinkOptionSnapshotItem With {
                .OptionGroup = OptionGroup,
                .OptionId = OptionId,
                .OptionName = OptionName,
                .ExtraPrice = ExtraPrice
            }
        End Function
    End Class

    Public NotInheritable Class ConfiguredDrinkDraft
        Private Shared ReadOnly JsonOptions As New JsonSerializerOptions With {
            .WriteIndented = False
        }

        Private Shared ReadOnly PrettyJsonOptions As New JsonSerializerOptions With {
            .WriteIndented = True
        }

        <JsonPropertyName("menu_item_id")>
        Public Property MenuItemId As String = String.Empty

        <JsonPropertyName("item_name_snapshot")>
        Public Property ItemNameSnapshot As String = String.Empty

        <JsonPropertyName("category_snapshot")>
        Public Property CategorySnapshot As String = String.Empty

        <JsonPropertyName("unit_base_price")>
        Public Property UnitBasePrice As Decimal

        <JsonPropertyName("options_snapshot")>
        Public Property OptionsSnapshot As New List(Of DrinkOptionSnapshotItem)()

        <JsonPropertyName("options_extra_price")>
        Public Property OptionsExtraPrice As Decimal

        <JsonPropertyName("quantity")>
        Public Property Quantity As Integer = 1

        <JsonPropertyName("notes")>
        Public Property Notes As String = String.Empty

        <JsonPropertyName("unit_total")>
        Public ReadOnly Property UnitTotal As Decimal
            Get
                Return Math.Max(0D, UnitBasePrice) + Math.Max(0D, OptionsExtraPrice)
            End Get
        End Property

        <JsonPropertyName("line_total")>
        Public ReadOnly Property LineTotal As Decimal
            Get
                If Quantity <= 0 Then
                    Return 0D
                End If
                Return UnitTotal * Quantity
            End Get
        End Property

        <JsonPropertyName("created_at_utc")>
        Public Property CreatedAtUtc As DateTimeOffset = DateTimeOffset.UtcNow

        Public Function ToOptionsSnapshotJson(Optional indented As Boolean = False) As String
            Dim optionsToSerialize = If(OptionsSnapshot, New List(Of DrinkOptionSnapshotItem)())
            Return JsonSerializer.Serialize(optionsToSerialize, If(indented, PrettyJsonOptions, JsonOptions))
        End Function

        Public Function ToOptionsSnapshotJsonElement() As JsonElement
            Using doc = JsonDocument.Parse(ToOptionsSnapshotJson(False))
                Return doc.RootElement.Clone()
            End Using
        End Function

        Public Function ToDraftJson(Optional indented As Boolean = True) As String
            Return JsonSerializer.Serialize(Me, If(indented, PrettyJsonOptions, JsonOptions))
        End Function

        Public Function FormatSelectedOptionsSummary() As String
            If OptionsSnapshot Is Nothing OrElse OptionsSnapshot.Count = 0 Then
                Return "Tanpa opsi tambahan"
            End If

            Dim parts As New List(Of String)()
            For Each opt In OptionsSnapshot
                If opt Is Nothing Then Continue For
                Dim groupLabel = MenuOption.GetGroupDisplayName(opt.OptionGroup)
                If opt.ExtraPrice > 0D Then
                    parts.Add($"{groupLabel}: {opt.OptionName} (+{AppTheme.FormatRupiah(opt.ExtraPrice)})")
                Else
                    parts.Add($"{groupLabel}: {opt.OptionName}")
                End If
            Next

            If parts.Count = 0 Then
                Return "Tanpa opsi tambahan"
            End If

            Return String.Join(", ", parts)
        End Function

        Public Function Clone() As ConfiguredDrinkDraft
            Dim clonedOptions As New List(Of DrinkOptionSnapshotItem)()
            If OptionsSnapshot IsNot Nothing Then
                For Each opt In OptionsSnapshot
                    If opt IsNot Nothing Then
                        clonedOptions.Add(opt.Clone())
                    End If
                Next
            End If

            Return New ConfiguredDrinkDraft With {
                .MenuItemId = MenuItemId,
                .ItemNameSnapshot = ItemNameSnapshot,
                .CategorySnapshot = CategorySnapshot,
                .UnitBasePrice = UnitBasePrice,
                .OptionsSnapshot = clonedOptions,
                .OptionsExtraPrice = OptionsExtraPrice,
                .Quantity = Quantity,
                .Notes = Notes,
                .CreatedAtUtc = CreatedAtUtc
            }
        End Function
    End Class

    Public NotInheritable Class DrinkDraftSubmittedEventArgs
        Inherits EventArgs

        Public ReadOnly Property Draft As ConfiguredDrinkDraft

        Public Sub New(draft As ConfiguredDrinkDraft)
            If draft Is Nothing Then
                Throw New ArgumentNullException(NameOf(draft))
            End If
            Me.Draft = draft.Clone()
        End Sub
    End Class

    ' In-memory handoff bridge so Friends Order Room or the shell can read prepared drafts without writing orders directly.
    Public NotInheritable Class DrinkDraftHandoff
        Private Sub New()
        End Sub

        Private Shared ReadOnly SyncRoot As New Object()
        Private Shared ReadOnly PendingList As New List(Of ConfiguredDrinkDraft)()
        Private Shared _latestDraft As ConfiguredDrinkDraft

        Public Shared Event DraftSubmitted As EventHandler(Of DrinkDraftSubmittedEventArgs)

        Public Shared ReadOnly Property LatestDraft As ConfiguredDrinkDraft
            Get
                SyncLock SyncRoot
                    Return If(_latestDraft IsNot Nothing, _latestDraft.Clone(), Nothing)
                End SyncLock
            End Get
        End Property

        Public Shared ReadOnly Property PendingCount As Integer
            Get
                SyncLock SyncRoot
                    Return PendingList.Count
                End SyncLock
            End Get
        End Property

        Public Shared Sub Publish(draft As ConfiguredDrinkDraft)
            If draft Is Nothing Then
                Throw New ArgumentNullException(NameOf(draft))
            End If

            Dim snapshot = draft.Clone()
            SyncLock SyncRoot
                _latestDraft = snapshot
                PendingList.Add(snapshot)
            End SyncLock

            RaiseEvent DraftSubmitted(Nothing, New DrinkDraftSubmittedEventArgs(snapshot))
        End Sub

        Public Shared Function GetPendingDrafts() As List(Of ConfiguredDrinkDraft)
            SyncLock SyncRoot
                Return PendingList.Select(Function(d) d.Clone()).ToList()
            End SyncLock
        End Function

        Public Shared Function ConsumePendingDrafts() As List(Of ConfiguredDrinkDraft)
            SyncLock SyncRoot
                Dim items = PendingList.Select(Function(d) d.Clone()).ToList()
                PendingList.Clear()
                Return items
            End SyncLock
        End Function

        Public Shared Function RemovePendingAt(index As Integer) As Boolean
            SyncLock SyncRoot
                If index < 0 OrElse index >= PendingList.Count Then
                    Return False
                End If
                PendingList.RemoveAt(index)
                _latestDraft = If(PendingList.Count > 0, PendingList(PendingList.Count - 1), Nothing)
                Return True
            End SyncLock
        End Function

        Public Shared Sub ClearPending()
            SyncLock SyncRoot
                PendingList.Clear()
                _latestDraft = Nothing
            End SyncLock
        End Sub
    End Class
End Namespace
