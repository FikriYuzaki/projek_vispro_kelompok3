Option Strict On
Option Explicit On

Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports CuppaClub.Core

Namespace Models
    Public Enum FriendsOrderErrorKind
        Unknown = 0
        Unconfigured = 1
        Unauthenticated = 2
        Unauthorized = 3
        InvalidInput = 4
        InvalidRoomCode = 5
        RoomLocked = 6
        RoomCancelled = 7
        RoomSubmitted = 8
        RoomUnavailable = 9
        RpcNotDeployed = 10
        RlsBlocked = 11
        NetworkError = 12
        CodeCollision = 13
    End Enum

    Public NotInheritable Class FriendsOrderException
        Inherits InvalidOperationException

        Public ReadOnly Property ErrorKind As FriendsOrderErrorKind

        Public Sub New(errorKind As FriendsOrderErrorKind, message As String, Optional innerException As Exception = Nothing)
            MyBase.New(message, innerException)
            Me.ErrorKind = errorKind
        End Sub
    End Class

    Public NotInheritable Class JoinRoomResult
        <JsonPropertyName("room_id")>
        Public Property RoomId As String = String.Empty

        <JsonPropertyName("title")>
        Public Property Title As String = String.Empty

        <JsonPropertyName("status")>
        Public Property Status As String = String.Empty

        <JsonPropertyName("joined_at")>
        Public Property JoinedAt As DateTimeOffset?
    End Class

    ' Adapter payload converting Habil's ConfiguredDrinkDraft into the shared order_items contract.
    Public NotInheritable Class RoomOrderItemPayload
        Public Property MenuItemId As String = String.Empty
        Public Property MemberUserId As String = String.Empty
        Public Property ItemNameSnapshot As String = String.Empty
        Public Property UnitBasePrice As Decimal
        Public Property OptionsSnapshotJson As String = "[]"
        Public Property OptionsExtraPrice As Decimal
        Public Property Quantity As Integer = 1
        Public Property LineTotal As Decimal
        Public Property Notes As String = String.Empty

        Public Function ToOptionsSnapshotJsonElement() As JsonElement
            Dim raw = If(String.IsNullOrWhiteSpace(OptionsSnapshotJson), "[]", OptionsSnapshotJson)
            Using doc = JsonDocument.Parse(raw)
                Return doc.RootElement.Clone()
            End Using
        End Function

        Public Function ToBrewOrderItem(orderId As String) As BrewOrderItem
            If String.IsNullOrWhiteSpace(orderId) Then
                Throw New ArgumentException("Order ID wajib diisi.", NameOf(orderId))
            End If

            Return New BrewOrderItem With {
                .OrderId = orderId.Trim(),
                .MenuItemId = MenuItemId,
                .MemberUserId = MemberUserId,
                .ItemNameSnapshot = ItemNameSnapshot,
                .UnitBasePrice = UnitBasePrice,
                .OptionsSnapshot = ToOptionsSnapshotJsonElement(),
                .OptionsExtraPrice = OptionsExtraPrice,
                .Quantity = Quantity,
                .LineTotal = LineTotal,
                .Notes = Notes
            }
        End Function

        Public Shared Function FromConfiguredDrinkDraft(
            draft As ConfiguredDrinkDraft,
            memberUserId As String
        ) As RoomOrderItemPayload
            If draft Is Nothing Then
                Throw New FriendsOrderException(FriendsOrderErrorKind.InvalidInput, "Draft minuman tidak boleh kosong.")
            End If

            Dim cleanMemberId = If(memberUserId, String.Empty).Trim()
            If String.IsNullOrWhiteSpace(cleanMemberId) Then
                Throw New FriendsOrderException(FriendsOrderErrorKind.Unauthenticated, "Masuk ke akun terlebih dahulu sebelum menambahkan item ke room.")
            End If

            Dim draftOwnerId = If(draft.OwnerUserId, String.Empty).Trim()
            If Not String.IsNullOrEmpty(draftOwnerId) AndAlso
               Not String.Equals(draftOwnerId, cleanMemberId, StringComparison.OrdinalIgnoreCase) Then
                Throw New FriendsOrderException(
                    FriendsOrderErrorKind.Unauthorized,
                    "Draft minuman ini milik akun lain dan tidak dapat ditambahkan atas nama akun yang sedang aktif.")
            End If

            Dim cleanMenuItemId = If(draft.MenuItemId, String.Empty).Trim()
            If String.IsNullOrWhiteSpace(cleanMenuItemId) Then
                Throw New FriendsOrderException(FriendsOrderErrorKind.InvalidInput, "Item menu pada draft tidak valid.")
            End If

            Dim cleanName = If(draft.ItemNameSnapshot, String.Empty).Trim()
            If String.IsNullOrWhiteSpace(cleanName) Then
                Throw New FriendsOrderException(FriendsOrderErrorKind.InvalidInput, "Nama item pada draft tidak boleh kosong.")
            End If

            If draft.Quantity <= 0 OrElse draft.Quantity > 99 Then
                Throw New FriendsOrderException(FriendsOrderErrorKind.InvalidInput, "Jumlah item harus antara 1 sampai 99.")
            End If

            If draft.UnitBasePrice < 0D OrElse draft.OptionsExtraPrice < 0D Then
                Throw New FriendsOrderException(FriendsOrderErrorKind.InvalidInput, "Harga dasar dan harga opsi tidak boleh negatif.")
            End If

            Dim cleanNotes = If(draft.Notes, String.Empty).Trim()
            If cleanNotes.Length > 200 Then
                cleanNotes = cleanNotes.Substring(0, 200)
            End If

            Dim computedLineTotal = (draft.UnitBasePrice + draft.OptionsExtraPrice) * draft.Quantity

            Return New RoomOrderItemPayload With {
                .MenuItemId = cleanMenuItemId,
                .MemberUserId = cleanMemberId,
                .ItemNameSnapshot = cleanName,
                .UnitBasePrice = draft.UnitBasePrice,
                .OptionsSnapshotJson = draft.ToOptionsSnapshotJson(False),
                .OptionsExtraPrice = draft.OptionsExtraPrice,
                .Quantity = draft.Quantity,
                .LineTotal = computedLineTotal,
                .Notes = cleanNotes
            }
        End Function

        Public Shared Function FromMenuItem(
            menuItem As MenuItem,
            memberUserId As String,
            quantity As Integer,
            Optional notes As String = ""
        ) As RoomOrderItemPayload
            If menuItem Is Nothing Then
                Throw New FriendsOrderException(FriendsOrderErrorKind.InvalidInput, "Pilih menu terlebih dahulu.")
            End If
            If Not menuItem.IsAvailable Then
                Throw New FriendsOrderException(FriendsOrderErrorKind.InvalidInput, $"Menu '{menuItem.Name}' sedang habis.")
            End If

            Dim draft As New ConfiguredDrinkDraft With {
                .OwnerUserId = memberUserId,
                .MenuItemId = menuItem.Id,
                .ItemNameSnapshot = menuItem.Name,
                .CategorySnapshot = menuItem.Category,
                .UnitBasePrice = menuItem.BasePrice,
                .OptionsSnapshot = New List(Of DrinkOptionSnapshotItem)(),
                .OptionsExtraPrice = 0D,
                .Quantity = quantity,
                .Notes = If(notes, String.Empty).Trim()
            }
            Return FromConfiguredDrinkDraft(draft, memberUserId)
        End Function

        Public Shared Function FormatOrderItemOptions(optionsElement As JsonElement) As String
            If optionsElement.ValueKind = JsonValueKind.Undefined OrElse optionsElement.ValueKind = JsonValueKind.Null Then
                Return String.Empty
            End If

            Try
                If optionsElement.ValueKind = JsonValueKind.Array Then
                    Dim parts As New List(Of String)()
                    For Each entry In optionsElement.EnumerateArray()
                        If entry.ValueKind <> JsonValueKind.Object Then Continue For
                        Dim optName As String = String.Empty
                        Dim optGroup As String = String.Empty
                        Dim extraPrice As Decimal = 0D

                        Dim prop As JsonElement
                        If entry.TryGetProperty("option_name", prop) AndAlso prop.ValueKind = JsonValueKind.String Then
                            optName = If(prop.GetString(), String.Empty).Trim()
                        End If
                        If entry.TryGetProperty("option_group", prop) AndAlso prop.ValueKind = JsonValueKind.String Then
                            optGroup = MenuOption.GetGroupDisplayName(If(prop.GetString(), String.Empty))
                        End If
                        If entry.TryGetProperty("extra_price", prop) AndAlso prop.ValueKind = JsonValueKind.Number Then
                            prop.TryGetDecimal(extraPrice)
                        End If

                        If Not String.IsNullOrEmpty(optName) Then
                            Dim label = If(String.IsNullOrEmpty(optGroup), optName, $"{optGroup}: {optName}")
                            If extraPrice > 0D Then
                                label &= $" (+{AppTheme.FormatRupiah(extraPrice)})"
                            End If
                            parts.Add(label)
                        End If
                    Next
                    Return String.Join(", ", parts)
                End If
            Catch
                ' Keep display resilient even if another module writes an alternate JSON shape.
            End Try

            Return String.Empty
        End Function
    End Class

    Public NotInheritable Class RoomMemberOrderSummary
        Public Property MemberUserId As String = String.Empty
        Public Property DisplayName As String = "Anggota"
        Public Property IsHost As Boolean
        Public Property JoinedAt As DateTimeOffset?
        Public Property Items As New List(Of BrewOrderItem)()

        Public ReadOnly Property ItemCount As Integer
            Get
                If Items Is Nothing Then Return 0
                Return Items.Sum(Function(i) Math.Max(0, i.Quantity))
            End Get
        End Property

        Public ReadOnly Property Subtotal As Decimal
            Get
                If Items Is Nothing Then Return 0D
                Return Items.Sum(Function(i) Math.Max(0D, i.LineTotal))
            End Get
        End Property
    End Class

    Public NotInheritable Class RoomOverviewSnapshot
        Public Property Room As OrderRoom
        Public Property Members As New List(Of RoomMember)()
        Public Property ParentOrder As BrewOrder
        Public Property Items As New List(Of BrewOrderItem)()
        Public Property MemberSummaries As New List(Of RoomMemberOrderSummary)()
        Public Property HostDisplayName As String = "Host"
        Public Property IsCurrentUserHost As Boolean
        Public Property IsCurrentUserMember As Boolean
        Public Property HostMemberTriggerVerified As Boolean = True

        Public ReadOnly Property GroupTotal As Decimal
            Get
                If MemberSummaries Is Nothing Then Return 0D
                Return MemberSummaries.Sum(Function(s) s.Subtotal)
            End Get
        End Property

        Public ReadOnly Property TotalItemCount As Integer
            Get
                If MemberSummaries Is Nothing Then Return 0
                Return MemberSummaries.Sum(Function(s) s.ItemCount)
            End Get
        End Property

        Public ReadOnly Property CanAddItems As Boolean
            Get
                Return Room IsNot Nothing AndAlso Room.IsOpen AndAlso (IsCurrentUserHost OrElse IsCurrentUserMember)
            End Get
        End Property

        Public ReadOnly Property CanLockRoom As Boolean
            Get
                Return Room IsNot Nothing AndAlso Room.IsOpen AndAlso IsCurrentUserHost
            End Get
        End Property
    End Class
End Namespace

