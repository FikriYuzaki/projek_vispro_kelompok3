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

        <JsonPropertyName("owner_user_id")>
        Public Property OwnerUserId As String = String.Empty

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
                .OwnerUserId = OwnerUserId,
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

    ' Per-account in-memory handoff bridge partitioned by authenticated Supabase User ID.
    Public NotInheritable Class DrinkDraftHandoff
        Private Sub New()
        End Sub

        Private Shared ReadOnly SyncRoot As New Object()
        Private Shared ReadOnly PendingByUser As New Dictionary(Of String, List(Of ConfiguredDrinkDraft))(StringComparer.OrdinalIgnoreCase)
        Private Shared ReadOnly LatestByUser As New Dictionary(Of String, ConfiguredDrinkDraft)(StringComparer.OrdinalIgnoreCase)

        Public Shared Event DraftSubmitted As EventHandler(Of DrinkDraftSubmittedEventArgs)
        Public Shared Event PendingDraftsChanged As EventHandler

        Shared Sub New()
            AddHandler AppSession.SessionChanged, AddressOf OnSessionChanged
        End Sub

        Private Shared Sub OnSessionChanged(sender As Object, e As EventArgs)
            RaiseEvent PendingDraftsChanged(Nothing, EventArgs.Empty)
        End Sub

        Public Shared ReadOnly Property CurrentOwnerUserId As String
            Get
                Dim user = AppSession.CurrentUser
                If user Is Nothing OrElse String.IsNullOrWhiteSpace(user.Id) Then
                    Return String.Empty
                End If
                Return user.Id.Trim()
            End Get
        End Property

        Public Shared ReadOnly Property HasAuthenticatedOwner As Boolean
            Get
                Return Not String.IsNullOrEmpty(CurrentOwnerUserId)
            End Get
        End Property

        Public Shared ReadOnly Property LatestDraft As ConfiguredDrinkDraft
            Get
                Dim ownerId = CurrentOwnerUserId
                If String.IsNullOrEmpty(ownerId) Then
                    Return Nothing
                End If

                SyncLock SyncRoot
                    Dim latest As ConfiguredDrinkDraft = Nothing
                    If LatestByUser.TryGetValue(ownerId, latest) AndAlso latest IsNot Nothing Then
                        Return latest.Clone()
                    End If
                    Return Nothing
                End SyncLock
            End Get
        End Property

        Public Shared ReadOnly Property PendingCount As Integer
            Get
                Dim ownerId = CurrentOwnerUserId
                If String.IsNullOrEmpty(ownerId) Then
                    Return 0
                End If

                SyncLock SyncRoot
                    Dim list As List(Of ConfiguredDrinkDraft) = Nothing
                    If PendingByUser.TryGetValue(ownerId, list) AndAlso list IsNot Nothing Then
                        Return list.Count
                    End If
                    Return 0
                End SyncLock
            End Get
        End Property

        Public Shared Sub Publish(draft As ConfiguredDrinkDraft)
            If draft Is Nothing Then
                Throw New ArgumentNullException(NameOf(draft))
            End If

            Dim ownerId = CurrentOwnerUserId
            If String.IsNullOrEmpty(ownerId) Then
                Throw New InvalidOperationException("Masuk ke akun terlebih dahulu sebelum menyimpan draft pesanan.")
            End If

            Dim requestedOwner = If(draft.OwnerUserId, String.Empty).Trim()
            If Not String.IsNullOrEmpty(requestedOwner) AndAlso
               Not String.Equals(requestedOwner, ownerId, StringComparison.OrdinalIgnoreCase) Then
                Throw New InvalidOperationException("Draft tidak sesuai dengan akun yang sedang aktif.")
            End If

            Dim snapshot = draft.Clone()
            snapshot.OwnerUserId = ownerId

            SyncLock SyncRoot
                Dim list As List(Of ConfiguredDrinkDraft) = Nothing
                If Not PendingByUser.TryGetValue(ownerId, list) Then
                    list = New List(Of ConfiguredDrinkDraft)()
                    PendingByUser(ownerId) = list
                End If
                list.Add(snapshot)
                LatestByUser(ownerId) = snapshot
            End SyncLock

            RaiseEvent DraftSubmitted(Nothing, New DrinkDraftSubmittedEventArgs(snapshot))
            RaiseEvent PendingDraftsChanged(Nothing, EventArgs.Empty)
        End Sub

        Public Shared Function GetPendingDrafts() As List(Of ConfiguredDrinkDraft)
            Dim ownerId = CurrentOwnerUserId
            If String.IsNullOrEmpty(ownerId) Then
                Return New List(Of ConfiguredDrinkDraft)()
            End If

            SyncLock SyncRoot
                Dim list As List(Of ConfiguredDrinkDraft) = Nothing
                If Not PendingByUser.TryGetValue(ownerId, list) OrElse list Is Nothing Then
                    Return New List(Of ConfiguredDrinkDraft)()
                End If
                Return list.Select(Function(d) d.Clone()).ToList()
            End SyncLock
        End Function

        Public Shared Function ConsumePendingDrafts() As List(Of ConfiguredDrinkDraft)
            Dim ownerId = CurrentOwnerUserId
            If String.IsNullOrEmpty(ownerId) Then
                Return New List(Of ConfiguredDrinkDraft)()
            End If

            Dim items As List(Of ConfiguredDrinkDraft)
            Dim hadItems As Boolean
            SyncLock SyncRoot
                Dim list As List(Of ConfiguredDrinkDraft) = Nothing
                If PendingByUser.TryGetValue(ownerId, list) AndAlso list IsNot Nothing AndAlso list.Count > 0 Then
                    hadItems = True
                    items = list.Select(Function(d) d.Clone()).ToList()
                    PendingByUser.Remove(ownerId)
                Else
                    items = New List(Of ConfiguredDrinkDraft)()
                End If
                If LatestByUser.Remove(ownerId) Then
                    hadItems = True
                End If
            End SyncLock

            If hadItems Then
                RaiseEvent PendingDraftsChanged(Nothing, EventArgs.Empty)
            End If
            Return items
        End Function

        Public Shared Function RemovePendingAt(index As Integer) As Boolean
            Dim ownerId = CurrentOwnerUserId
            If String.IsNullOrEmpty(ownerId) Then
                Return False
            End If

            Dim removed As Boolean
            SyncLock SyncRoot
                Dim list As List(Of ConfiguredDrinkDraft) = Nothing
                If Not PendingByUser.TryGetValue(ownerId, list) OrElse list Is Nothing Then
                    Return False
                End If
                If index < 0 OrElse index >= list.Count Then
                    Return False
                End If
                list.RemoveAt(index)
                If list.Count > 0 Then
                    LatestByUser(ownerId) = list(list.Count - 1)
                Else
                    PendingByUser.Remove(ownerId)
                    LatestByUser.Remove(ownerId)
                End If
                removed = True
            End SyncLock

            If removed Then
                RaiseEvent PendingDraftsChanged(Nothing, EventArgs.Empty)
            End If
            Return True
        End Function

        Public Shared Sub ClearPending()
            Dim ownerId = CurrentOwnerUserId
            If String.IsNullOrEmpty(ownerId) Then
                Return
            End If

            Dim hadItems As Boolean
            SyncLock SyncRoot
                Dim removedList = PendingByUser.Remove(ownerId)
                Dim removedLatest = LatestByUser.Remove(ownerId)
                hadItems = removedList OrElse removedLatest
            End SyncLock

            If hadItems Then
                RaiseEvent PendingDraftsChanged(Nothing, EventArgs.Empty)
            End If
        End Sub
    End Class
End Namespace
