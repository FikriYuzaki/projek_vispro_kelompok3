Option Strict On
Option Explicit On

Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Text.Json
Imports CuppaClub.Core
Imports CuppaClub.Models
Imports Supabase.Postgrest
Imports Supabase.Postgrest.Exceptions
Imports Supabase.Realtime.Exceptions
Imports Supabase.Realtime.Interfaces
Imports Supabase.Realtime.PostgresChanges
Imports RealtimeConstants = Supabase.Realtime.Constants

Namespace Services
    Public Enum FriendsRoomRealtimeState
        Disconnected = 0
        Connecting = 1
        Connected = 2
        Reconnecting = 3
        [Error] = 4
        PublicationUnavailable = 5
    End Enum

    Public NotInheritable Class FriendsRoomRealtimeSubscription
        Implements IDisposable

        Private ReadOnly _syncRoot As New Object()
        Private _realtimeClient As IRealtimeClient(Of Supabase.Realtime.RealtimeSocket, Supabase.Realtime.RealtimeChannel)
        Private _channel As Supabase.Realtime.RealtimeChannel
        Private _postgresHandler As IRealtimeChannel.PostgresChangesHandler
        Private _channelStateHandler As IRealtimeChannel.StateChangedHandler
        Private _channelErrorHandler As IRealtimeChannel.ErrorEventHandler
        Private _socketStateHandler As IRealtimeClient(Of Supabase.Realtime.RealtimeSocket, Supabase.Realtime.RealtimeChannel).SocketStateEventHandler
        Private _disposed As Boolean

        Friend Sub New(
            realtimeClient As IRealtimeClient(Of Supabase.Realtime.RealtimeSocket, Supabase.Realtime.RealtimeChannel),
            channel As Supabase.Realtime.RealtimeChannel,
            postgresHandler As IRealtimeChannel.PostgresChangesHandler,
            channelStateHandler As IRealtimeChannel.StateChangedHandler,
            channelErrorHandler As IRealtimeChannel.ErrorEventHandler,
            socketStateHandler As IRealtimeClient(Of Supabase.Realtime.RealtimeSocket, Supabase.Realtime.RealtimeChannel).SocketStateEventHandler
        )
            _realtimeClient = realtimeClient
            _channel = channel
            _postgresHandler = postgresHandler
            _channelStateHandler = channelStateHandler
            _channelErrorHandler = channelErrorHandler
            _socketStateHandler = socketStateHandler
        End Sub

        Public ReadOnly Property IsDisposed As Boolean
            Get
                SyncLock _syncRoot
                    Return _disposed
                End SyncLock
            End Get
        End Property

        Public ReadOnly Property IsJoined As Boolean
            Get
                SyncLock _syncRoot
                    Return Not _disposed AndAlso _channel IsNot Nothing AndAlso _channel.IsJoined
                End SyncLock
            End Get
        End Property

        Public Sub Dispose() Implements IDisposable.Dispose
            Dim clientToClean As IRealtimeClient(Of Supabase.Realtime.RealtimeSocket, Supabase.Realtime.RealtimeChannel)
            Dim channelToClean As Supabase.Realtime.RealtimeChannel
            Dim pgHandler As IRealtimeChannel.PostgresChangesHandler
            Dim chStateHandler As IRealtimeChannel.StateChangedHandler
            Dim chErrorHandler As IRealtimeChannel.ErrorEventHandler
            Dim sockHandler As IRealtimeClient(Of Supabase.Realtime.RealtimeSocket, Supabase.Realtime.RealtimeChannel).SocketStateEventHandler

            SyncLock _syncRoot
                If _disposed Then Return
                _disposed = True

                clientToClean = _realtimeClient
                channelToClean = _channel
                pgHandler = _postgresHandler
                chStateHandler = _channelStateHandler
                chErrorHandler = _channelErrorHandler
                sockHandler = _socketStateHandler

                _realtimeClient = Nothing
                _channel = Nothing
                _postgresHandler = Nothing
                _channelStateHandler = Nothing
                _channelErrorHandler = Nothing
                _socketStateHandler = Nothing
            End SyncLock

            If clientToClean IsNot Nothing AndAlso sockHandler IsNot Nothing Then
                Try
                    clientToClean.RemoveStateChangedHandler(sockHandler)
                Catch
                End Try
            End If

            If channelToClean IsNot Nothing Then
                If pgHandler IsNot Nothing Then
                    Try
                        channelToClean.RemovePostgresChangeHandler(PostgresChangesOptions.ListenType.All, pgHandler)
                    Catch
                    End Try
                End If
                If chStateHandler IsNot Nothing Then
                    Try
                        channelToClean.RemoveStateChangedHandler(chStateHandler)
                    Catch
                    End Try
                End If
                If chErrorHandler IsNot Nothing Then
                    Try
                        channelToClean.RemoveErrorHandler(chErrorHandler)
                    Catch
                    End Try
                End If
                Try
                    channelToClean.Unsubscribe()
                Catch
                End Try
                If clientToClean IsNot Nothing Then
                    Try
                        clientToClean.Remove(channelToClean)
                    Catch
                    End Try
                End If
            End If
        End Sub
    End Class

    Public Interface IFriendsOrderDataSource
        ReadOnly Property IsConfigured As Boolean
        Function GetMembershipsByUserAsync(userId As String) As Task(Of List(Of RoomMember))
        Function GetRoomsByHostAsync(hostUserId As String) As Task(Of List(Of OrderRoom))
        Function GetRoomsByIdsAsync(roomIds As IEnumerable(Of String)) As Task(Of List(Of OrderRoom))
        Function GetRoomByIdAsync(roomId As String) As Task(Of OrderRoom)
        Function InsertRoomAsync(room As OrderRoom) As Task(Of OrderRoom)
        Function JoinRoomByCodeRpcAsync(normalizedCode As String) As Task(Of JoinRoomResult)
        Function GetRoomMembersAsync(roomId As String) As Task(Of List(Of RoomMember))
        Function GetParentOrderByRoomAsync(roomId As String) As Task(Of BrewOrder)
        Function InsertParentOrderAsync(orderModel As BrewOrder) As Task(Of BrewOrder)
        Function GetOrderItemsAsync(orderId As String) As Task(Of List(Of BrewOrderItem))
        Function InsertOrderItemAsync(item As BrewOrderItem) As Task(Of BrewOrderItem)
        Function LockRoomAsync(roomId As String, lockedAtUtc As DateTimeOffset) As Task(Of OrderRoom)
        Function GetAvailableMenuItemsAsync() As Task(Of List(Of MenuItem))
        Function SubscribeRoomAsync(
            roomId As String,
            parentOrderId As String,
            onChanged As Action(Of String),
            onStateChanged As Action(Of FriendsRoomRealtimeState, String)
        ) As Task(Of FriendsRoomRealtimeSubscription)
    End Interface

    Friend NotInheritable Class SupabaseFriendsOrderDataSource
        Implements IFriendsOrderDataSource

        Private Shared ReadOnly RpcJsonOptions As New JsonSerializerOptions With {
            .PropertyNameCaseInsensitive = True
        }

        Public ReadOnly Property IsConfigured As Boolean Implements IFriendsOrderDataSource.IsConfigured
            Get
                Return SupabaseClientProvider.IsConfigured
            End Get
        End Property

        Public Async Function GetMembershipsByUserAsync(userId As String) As Task(Of List(Of RoomMember)) Implements IFriendsOrderDataSource.GetMembershipsByUserAsync
            Dim client = Await SupabaseClientProvider.GetClientAsync().ConfigureAwait(False)
            Dim response = Await client.From(Of RoomMember)().
                Filter("user_id", Constants.Operator.Equals, userId).
                Order("joined_at", Constants.Ordering.Descending).
                Get().ConfigureAwait(False)

            If response Is Nothing OrElse response.Models Is Nothing Then
                Return New List(Of RoomMember)()
            End If
            Return response.Models
        End Function

        Public Async Function GetRoomsByHostAsync(hostUserId As String) As Task(Of List(Of OrderRoom)) Implements IFriendsOrderDataSource.GetRoomsByHostAsync
            Dim client = Await SupabaseClientProvider.GetClientAsync().ConfigureAwait(False)
            Dim response = Await client.From(Of OrderRoom)().
                Filter("host_user_id", Constants.Operator.Equals, hostUserId).
                Order("created_at", Constants.Ordering.Descending).
                Get().ConfigureAwait(False)

            If response Is Nothing OrElse response.Models Is Nothing Then
                Return New List(Of OrderRoom)()
            End If
            Return response.Models
        End Function

        Public Async Function GetRoomsByIdsAsync(roomIds As IEnumerable(Of String)) As Task(Of List(Of OrderRoom)) Implements IFriendsOrderDataSource.GetRoomsByIdsAsync
            Dim distinctIds = roomIds.
                Where(Function(id) Not String.IsNullOrWhiteSpace(id)).
                Select(Function(id) id.Trim()).
                Distinct(StringComparer.OrdinalIgnoreCase).
                ToList()

            If distinctIds.Count = 0 Then
                Return New List(Of OrderRoom)()
            End If

            Dim client = Await SupabaseClientProvider.GetClientAsync().ConfigureAwait(False)
            Dim response = Await client.From(Of OrderRoom)().
                Filter("id", Constants.Operator.In, distinctIds).
                Order("created_at", Constants.Ordering.Descending).
                Get().ConfigureAwait(False)

            If response Is Nothing OrElse response.Models Is Nothing Then
                Return New List(Of OrderRoom)()
            End If
            Return response.Models
        End Function

        Public Async Function GetRoomByIdAsync(roomId As String) As Task(Of OrderRoom) Implements IFriendsOrderDataSource.GetRoomByIdAsync
            Dim client = Await SupabaseClientProvider.GetClientAsync().ConfigureAwait(False)
            Dim response = Await client.From(Of OrderRoom)().
                Filter("id", Constants.Operator.Equals, roomId).
                Get().ConfigureAwait(False)

            If response Is Nothing OrElse response.Models Is Nothing OrElse response.Models.Count = 0 Then
                Return Nothing
            End If
            Return response.Models(0)
        End Function

        Public Async Function InsertRoomAsync(room As OrderRoom) As Task(Of OrderRoom) Implements IFriendsOrderDataSource.InsertRoomAsync
            Dim client = Await SupabaseClientProvider.GetClientAsync().ConfigureAwait(False)
            Dim response = Await client.From(Of OrderRoom)().Insert(room).ConfigureAwait(False)

            If response IsNot Nothing AndAlso response.Models IsNot Nothing AndAlso response.Models.Count > 0 Then
                Return response.Models(0)
            End If

            Throw New FriendsOrderException(FriendsOrderErrorKind.Unknown, "Supabase tidak mengembalikan data room yang baru dibuat.")
        End Function

        Public Async Function JoinRoomByCodeRpcAsync(normalizedCode As String) As Task(Of JoinRoomResult) Implements IFriendsOrderDataSource.JoinRoomByCodeRpcAsync
            Dim client = Await SupabaseClientProvider.GetClientAsync().ConfigureAwait(False)
            Dim parameters As New Dictionary(Of String, Object) From {
                {"p_room_code", normalizedCode}
            }

            Dim response = Await client.Rpc("join_room_by_code", parameters).ConfigureAwait(False)
            Dim content = If(response?.Content, String.Empty).Trim()

            If String.IsNullOrWhiteSpace(content) OrElse content = "[]" OrElse content = "null" Then
                Throw New FriendsOrderException(FriendsOrderErrorKind.InvalidRoomCode, "Kode room tidak ditemukan atau tidak valid.")
            End If

            If content.StartsWith("[", StringComparison.Ordinal) Then
                Dim rows = JsonSerializer.Deserialize(Of List(Of JoinRoomResult))(content, RpcJsonOptions)
                If rows IsNot Nothing AndAlso rows.Count > 0 AndAlso Not String.IsNullOrWhiteSpace(rows(0).RoomId) Then
                    Return rows(0)
                End If
                Throw New FriendsOrderException(FriendsOrderErrorKind.InvalidRoomCode, "Kode room tidak ditemukan atau tidak valid.")
            End If

            Dim singleRow = JsonSerializer.Deserialize(Of JoinRoomResult)(content, RpcJsonOptions)
            If singleRow IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(singleRow.RoomId) Then
                Return singleRow
            End If

            Throw New FriendsOrderException(FriendsOrderErrorKind.InvalidRoomCode, "Respons RPC join_room_by_code tidak mengembalikan data room yang valid.")
        End Function

        Public Async Function GetRoomMembersAsync(roomId As String) As Task(Of List(Of RoomMember)) Implements IFriendsOrderDataSource.GetRoomMembersAsync
            Dim client = Await SupabaseClientProvider.GetClientAsync().ConfigureAwait(False)
            Dim response = Await client.From(Of RoomMember)().
                Filter("room_id", Constants.Operator.Equals, roomId).
                Order("joined_at", Constants.Ordering.Ascending).
                Get().ConfigureAwait(False)

            If response Is Nothing OrElse response.Models Is Nothing Then
                Return New List(Of RoomMember)()
            End If
            Return response.Models
        End Function

        Public Async Function GetParentOrderByRoomAsync(roomId As String) As Task(Of BrewOrder) Implements IFriendsOrderDataSource.GetParentOrderByRoomAsync
            Dim client = Await SupabaseClientProvider.GetClientAsync().ConfigureAwait(False)
            Dim response = Await client.From(Of BrewOrder)().
                Filter("room_id", Constants.Operator.Equals, roomId).
                Order("created_at", Constants.Ordering.Ascending).
                Get().ConfigureAwait(False)

            If response Is Nothing OrElse response.Models Is Nothing OrElse response.Models.Count = 0 Then
                Return Nothing
            End If
            Return response.Models(0)
        End Function

        Public Async Function InsertParentOrderAsync(orderModel As BrewOrder) As Task(Of BrewOrder) Implements IFriendsOrderDataSource.InsertParentOrderAsync
            Dim client = Await SupabaseClientProvider.GetClientAsync().ConfigureAwait(False)
            Dim response = Await client.From(Of BrewOrder)().Insert(orderModel).ConfigureAwait(False)

            If response IsNot Nothing AndAlso response.Models IsNot Nothing AndAlso response.Models.Count > 0 Then
                Return response.Models(0)
            End If

            Throw New FriendsOrderException(FriendsOrderErrorKind.Unknown, "Supabase tidak mengembalikan order induk grup yang dibuat.")
        End Function

        Public Async Function GetOrderItemsAsync(orderId As String) As Task(Of List(Of BrewOrderItem)) Implements IFriendsOrderDataSource.GetOrderItemsAsync
            Dim client = Await SupabaseClientProvider.GetClientAsync().ConfigureAwait(False)
            Dim response = Await client.From(Of BrewOrderItem)().
                Filter("order_id", Constants.Operator.Equals, orderId).
                Order("created_at", Constants.Ordering.Ascending).
                Get().ConfigureAwait(False)

            If response Is Nothing OrElse response.Models Is Nothing Then
                Return New List(Of BrewOrderItem)()
            End If
            Return response.Models
        End Function

        Public Async Function InsertOrderItemAsync(item As BrewOrderItem) As Task(Of BrewOrderItem) Implements IFriendsOrderDataSource.InsertOrderItemAsync
            Dim client = Await SupabaseClientProvider.GetClientAsync().ConfigureAwait(False)
            Dim response = Await client.From(Of BrewOrderItem)().Insert(item).ConfigureAwait(False)

            If response IsNot Nothing AndAlso response.Models IsNot Nothing AndAlso response.Models.Count > 0 Then
                Return response.Models(0)
            End If

            Throw New FriendsOrderException(FriendsOrderErrorKind.Unknown, "Supabase tidak mengembalikan item pesanan yang disimpan.")
        End Function

        Public Async Function LockRoomAsync(roomId As String, lockedAtUtc As DateTimeOffset) As Task(Of OrderRoom) Implements IFriendsOrderDataSource.LockRoomAsync
            Dim client = Await SupabaseClientProvider.GetClientAsync().ConfigureAwait(False)
            ' Conditional update: only lock when status is still 'open' to prevent race conditions.
            Dim response = Await client.From(Of OrderRoom)().
                Filter("id", Constants.Operator.Equals, roomId).
                Filter("status", Constants.Operator.Equals, "open").
                Set(Function(r) CObj(r.Status), "locked").
                Set(Function(r) CObj(r.LockedAt), lockedAtUtc).
                Update().ConfigureAwait(False)

            If response IsNot Nothing AndAlso response.Models IsNot Nothing AndAlso response.Models.Count > 0 Then
                Return response.Models(0)
            End If

            Dim refreshed = Await GetRoomByIdAsync(roomId).ConfigureAwait(False)
            If refreshed IsNot Nothing Then
                If refreshed.IsLocked Then
                    Throw New FriendsOrderException(
                        FriendsOrderErrorKind.RoomLocked,
                        "Room sudah terkunci sebelumnya.")
                End If
                If refreshed.IsCancelled Then
                    Throw New FriendsOrderException(
                        FriendsOrderErrorKind.RoomCancelled,
                        "Room sudah dibatalkan sehingga tidak dapat dikunci.")
                End If
                If refreshed.IsSubmitted Then
                    Throw New FriendsOrderException(
                        FriendsOrderErrorKind.RoomSubmitted,
                        "Pesanan room sudah dikirim sehingga tidak dapat dikunci.")
                End If
            End If

            Throw New FriendsOrderException(
                FriendsOrderErrorKind.RlsBlocked,
                "Room tidak dapat dikunci. Hanya host room yang memiliki izin mengunci room yang masih terbuka.")
        End Function

        Public Async Function GetAvailableMenuItemsAsync() As Task(Of List(Of MenuItem)) Implements IFriendsOrderDataSource.GetAvailableMenuItemsAsync
            Dim client = Await SupabaseClientProvider.GetClientAsync().ConfigureAwait(False)
            Dim response = Await client.From(Of MenuItem)().
                Filter("is_available", Constants.Operator.Equals, "true").
                Order("category", Constants.Ordering.Ascending).
                Order("name", Constants.Ordering.Ascending).
                Get().ConfigureAwait(False)

            If response Is Nothing OrElse response.Models Is Nothing Then
                Return New List(Of MenuItem)()
            End If
            Return response.Models
        End Function

        Public Async Function SubscribeRoomAsync(
            roomId As String,
            parentOrderId As String,
            onChanged As Action(Of String),
            onStateChanged As Action(Of FriendsRoomRealtimeState, String)
        ) As Task(Of FriendsRoomRealtimeSubscription) Implements IFriendsOrderDataSource.SubscribeRoomAsync
            Dim client = Await SupabaseClientProvider.GetClientAsync().ConfigureAwait(False)
            Dim realtime = client.Realtime

            If client.Auth IsNot Nothing AndAlso
               client.Auth.CurrentSession IsNot Nothing AndAlso
               Not String.IsNullOrWhiteSpace(client.Auth.CurrentSession.AccessToken) Then
                realtime.SetAuth(client.Auth.CurrentSession.AccessToken)
            End If

            onStateChanged?.Invoke(FriendsRoomRealtimeState.Connecting, "Menghubungkan pembaruan realtime room...")

            If realtime.ConnectionState <> RealtimeConstants.ConnectionState.Open Then
                Await realtime.ConnectAsync().ConfigureAwait(False)
            End If

            Dim topic As String = $"friends-room-{roomId}-{Guid.NewGuid():N}"
            Dim channel As Supabase.Realtime.RealtimeChannel = realtime.Channel(topic)
            Dim subscriptionRef As FriendsRoomRealtimeSubscription = Nothing

            Dim pgHandler As IRealtimeChannel.PostgresChangesHandler =
                Sub(sender As IRealtimeChannel, change As PostgresChangesResponse)
                    If subscriptionRef IsNot Nothing AndAlso subscriptionRef.IsDisposed Then Return
                    Dim tableName = If(change?.Payload?.Data?.Table, "room")
                    onChanged?.Invoke(tableName)
                End Sub

            Dim chStateHandler As IRealtimeChannel.StateChangedHandler =
                Sub(sender As IRealtimeChannel, state As RealtimeConstants.ChannelState)
                    If subscriptionRef IsNot Nothing AndAlso subscriptionRef.IsDisposed Then Return
                    Select Case state
                        Case RealtimeConstants.ChannelState.Joined
                            onStateChanged?.Invoke(FriendsRoomRealtimeState.Connected, "Realtime aktif.")
                        Case RealtimeConstants.ChannelState.Joining
                            onStateChanged?.Invoke(FriendsRoomRealtimeState.Connecting, "Menyambungkan channel room...")
                        Case RealtimeConstants.ChannelState.Errored
                            onStateChanged?.Invoke(FriendsRoomRealtimeState.PublicationUnavailable, "Realtime tidak tersedia (publikasi tabel belum aktif atau channel error). Gunakan Muat Ulang.")
                        Case RealtimeConstants.ChannelState.Closed, RealtimeConstants.ChannelState.Leaving
                            onStateChanged?.Invoke(FriendsRoomRealtimeState.Disconnected, "Realtime terputus. Gunakan Muat Ulang.")
                    End Select
                End Sub

            Dim chErrorHandler As IRealtimeChannel.ErrorEventHandler =
                Sub(sender As IRealtimeChannel, ex As RealtimeException)
                    If subscriptionRef IsNot Nothing AndAlso subscriptionRef.IsDisposed Then Return
                    Dim msg = If(ex IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(ex.Message), ex.Message, "Gangguan channel realtime.")
                    onStateChanged?.Invoke(FriendsRoomRealtimeState.[Error], msg)
                End Sub

            Dim sockHandler As IRealtimeClient(Of Supabase.Realtime.RealtimeSocket, Supabase.Realtime.RealtimeChannel).SocketStateEventHandler =
                Sub(sender As IRealtimeClient(Of Supabase.Realtime.RealtimeSocket, Supabase.Realtime.RealtimeChannel), state As RealtimeConstants.SocketState)
                    If subscriptionRef IsNot Nothing AndAlso subscriptionRef.IsDisposed Then Return
                    Select Case state
                        Case RealtimeConstants.SocketState.Open
                            Dim mapped = If(channel.IsJoined, FriendsRoomRealtimeState.Connected, FriendsRoomRealtimeState.Reconnecting)
                            Dim detail = If(channel.IsJoined, "Realtime aktif.", "Socket terhubung, menyambungkan ulang channel...")
                            onStateChanged?.Invoke(mapped, detail)
                        Case RealtimeConstants.SocketState.Reconnect
                            onStateChanged?.Invoke(FriendsRoomRealtimeState.Reconnecting, "Menyambung ulang realtime...")
                        Case RealtimeConstants.SocketState.Close
                            onStateChanged?.Invoke(FriendsRoomRealtimeState.Disconnected, "Koneksi realtime terputus. Gunakan Muat Ulang.")
                        Case RealtimeConstants.SocketState.[Error]
                            onStateChanged?.Invoke(FriendsRoomRealtimeState.[Error], "Socket realtime mengalami error. Gunakan Muat Ulang.")
                    End Select
                End Sub

            subscriptionRef = New FriendsRoomRealtimeSubscription(
                realtime,
                channel,
                pgHandler,
                chStateHandler,
                chErrorHandler,
                sockHandler)

            Try
                channel.OnPostgresChange(pgHandler, PostgresChangesOptions.ListenType.All, New PostgresChangesFilter With {
                    .Schema = "public",
                    .Table = "order_rooms",
                    .Filter = $"id=eq.{roomId}"
                })
                channel.OnPostgresChange(pgHandler, PostgresChangesOptions.ListenType.All, New PostgresChangesFilter With {
                    .Schema = "public",
                    .Table = "room_members",
                    .Filter = $"room_id=eq.{roomId}"
                })
                If Not String.IsNullOrWhiteSpace(parentOrderId) Then
                    channel.OnPostgresChange(pgHandler, PostgresChangesOptions.ListenType.All, New PostgresChangesFilter With {
                        .Schema = "public",
                        .Table = "order_items",
                        .Filter = $"order_id=eq.{parentOrderId}"
                    })
                Else
                    channel.OnPostgresChange(pgHandler, PostgresChangesOptions.ListenType.All, New PostgresChangesFilter With {
                        .Schema = "public",
                        .Table = "orders",
                        .Filter = $"room_id=eq.{roomId}"
                    })
                End If

                channel.AddStateChangedHandler(chStateHandler)
                channel.AddErrorHandler(chErrorHandler)
                realtime.AddStateChangedHandler(sockHandler)

                Await channel.Subscribe(10000).ConfigureAwait(False)
                onStateChanged?.Invoke(FriendsRoomRealtimeState.Connected, "Realtime aktif.")
                Return subscriptionRef
            Catch
                subscriptionRef.Dispose()
                Throw
            End Try
        End Function
    End Class

    Public NotInheritable Class FriendsOrderService
        Public Const MinCodeLength As Integer = 4
        Public Const MaxCodeLength As Integer = 16
        Public Const DefaultGeneratedCodeLength As Integer = 8
        Public Const MaxCodeGenerationRetries As Integer = 5
        Public Const MinTitleLength As Integer = 2
        Public Const MaxTitleLength As Integer = 80

        Private Const CodeAlphabet As String = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"

        Private ReadOnly _dataSource As IFriendsOrderDataSource
        Private ReadOnly _currentUserProvider As Func(Of UserProfile)

        Public Sub New()
            Me.New(New SupabaseFriendsOrderDataSource(), Function() AppSession.CurrentUser)
        End Sub

        Public Sub New(dataSource As IFriendsOrderDataSource, Optional currentUserProvider As Func(Of UserProfile) = Nothing)
            If dataSource Is Nothing Then
                Throw New ArgumentNullException(NameOf(dataSource))
            End If
            _dataSource = dataSource
            _currentUserProvider = If(currentUserProvider, Function() AppSession.CurrentUser)
        End Sub

        Public ReadOnly Property IsConfigured As Boolean
            Get
                Return _dataSource.IsConfigured
            End Get
        End Property

        Public Shared Function GenerateRoomCode(Optional length As Integer = DefaultGeneratedCodeLength) As String
            Dim safeLen = Math.Max(MinCodeLength, Math.Min(MaxCodeLength, length))
            Dim chars(safeLen - 1) As Char
            Dim buffer(safeLen - 1) As Byte
            RandomNumberGenerator.Fill(buffer)

            For i = 0 To safeLen - 1
                chars(i) = CodeAlphabet(buffer(i) Mod CodeAlphabet.Length)
            Next

            Return New String(chars)
        End Function

        Public Shared Function NormalizeRoomCode(rawCode As String) As String
            Return If(rawCode, String.Empty).Trim().ToUpperInvariant()
        End Function

        Public Shared Function ValidateRoomTitle(rawTitle As String) As String
            Dim clean = If(rawTitle, String.Empty).Trim()
            If clean.Length < MinTitleLength Then
                Throw New FriendsOrderException(FriendsOrderErrorKind.InvalidInput, $"Judul room minimal {MinTitleLength} karakter.")
            End If
            If clean.Length > MaxTitleLength Then
                Throw New FriendsOrderException(FriendsOrderErrorKind.InvalidInput, $"Judul room maksimal {MaxTitleLength} karakter.")
            End If
            Return clean
        End Function

        Public Shared Function ValidateRoomCode(rawCode As String) As String
            Dim normalized = NormalizeRoomCode(rawCode)
            If normalized.Length < MinCodeLength OrElse normalized.Length > MaxCodeLength Then
                Throw New FriendsOrderException(
                    FriendsOrderErrorKind.InvalidInput,
                    $"Kode room harus terdiri dari {MinCodeLength} sampai {MaxCodeLength} karakter.")
            End If
            For Each ch In normalized
                If Not (Char.IsLetterOrDigit(ch) OrElse ch = "-"c OrElse ch = "_"c) Then
                    Throw New FriendsOrderException(
                        FriendsOrderErrorKind.InvalidInput,
                        "Kode room hanya boleh berisi huruf, angka, tanda hubung (-), atau garis bawah (_).")
                End If
            Next
            Return normalized
        End Function

        Public Async Function GetMyRoomsAsync() As Task(Of List(Of OrderRoom))
            EnsureConfigured()
            Dim user = RequireAuthenticatedUser()

            Try
                Dim memberships = Await _dataSource.GetMembershipsByUserAsync(user.Id).ConfigureAwait(False)
                Dim hostedRooms = Await _dataSource.GetRoomsByHostAsync(user.Id).ConfigureAwait(False)

                Dim roomMap As New Dictionary(Of String, OrderRoom)(StringComparer.OrdinalIgnoreCase)
                For Each hosted In hostedRooms
                    If hosted IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(hosted.Id) Then
                        roomMap(hosted.Id) = hosted
                    End If
                Next

                Dim missingMemberRoomIds = memberships.
                    Where(Function(m) m IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(m.RoomId) AndAlso Not roomMap.ContainsKey(m.RoomId)).
                    Select(Function(m) m.RoomId).
                    Distinct(StringComparer.OrdinalIgnoreCase).
                    ToList()

                If missingMemberRoomIds.Count > 0 Then
                    Dim joinedRooms = Await _dataSource.GetRoomsByIdsAsync(missingMemberRoomIds).ConfigureAwait(False)
                    For Each joined In joinedRooms
                        If joined IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(joined.Id) Then
                            roomMap(joined.Id) = joined
                        End If
                    Next
                End If

                Return roomMap.Values.
                    OrderByDescending(Function(r) If(r.CreatedAt.HasValue, r.CreatedAt.Value, DateTimeOffset.MinValue)).
                    ToList()
            Catch ex As FriendsOrderException
                Throw
            Catch ex As Exception
                Throw MapException(ex, "Gagal memuat daftar room Anda.")
            End Try
        End Function

        Public Async Function CreateRoomAsync(rawTitle As String, Optional preferredCode As String = Nothing) As Task(Of OrderRoom)
            EnsureConfigured()
            Dim user = RequireAuthenticatedUser()
            Dim title = ValidateRoomTitle(rawTitle)

            Dim useManualCode = Not String.IsNullOrWhiteSpace(preferredCode)
            Dim maxAttempts = If(useManualCode, 1, MaxCodeGenerationRetries)

            For attempt = 1 To maxAttempts
                Dim code = If(useManualCode, ValidateRoomCode(preferredCode), GenerateRoomCode())
                Dim candidate As New OrderRoom With {
                    .Title = title,
                    .RoomCode = code,
                    .HostUserId = user.Id,
                    .Status = "open"
                }

                Try
                    Dim created = Await _dataSource.InsertRoomAsync(candidate).ConfigureAwait(False)
                    Return created
                Catch ex As FriendsOrderException
                    If ex.ErrorKind = FriendsOrderErrorKind.CodeCollision AndAlso Not useManualCode AndAlso attempt < maxAttempts Then
                        Continue For
                    End If
                    Throw
                Catch ex As Exception
                    Dim mapped = MapException(ex, "Gagal membuat room baru.")
                    If mapped.ErrorKind = FriendsOrderErrorKind.CodeCollision AndAlso Not useManualCode AndAlso attempt < maxAttempts Then
                        Continue For
                    End If
                    If mapped.ErrorKind = FriendsOrderErrorKind.CodeCollision AndAlso useManualCode Then
                        Throw New FriendsOrderException(
                            FriendsOrderErrorKind.CodeCollision,
                            $"Kode room '{code}' sudah digunakan. Kosongkan kode agar dibuat otomatis atau gunakan kode lain.",
                            ex)
                    End If
                    Throw mapped
                End Try
            Next

            Throw New FriendsOrderException(
                FriendsOrderErrorKind.CodeCollision,
                "Gagal menghasilkan kode room unik setelah beberapa percobaan. Silakan coba lagi.")
        End Function

        Public Async Function JoinRoomByCodeAsync(rawRoomCode As String) As Task(Of JoinRoomResult)
            EnsureConfigured()
            RequireAuthenticatedUser()
            Dim normalizedCode = ValidateRoomCode(rawRoomCode)

            Try
                Return Await _dataSource.JoinRoomByCodeRpcAsync(normalizedCode).ConfigureAwait(False)
            Catch ex As FriendsOrderException
                Throw
            Catch ex As Exception
                Throw MapException(ex, "Gagal bergabung ke room.")
            End Try
        End Function

        Public Async Function GetRoomOverviewAsync(roomId As String) As Task(Of RoomOverviewSnapshot)
            EnsureConfigured()
            Dim user = RequireAuthenticatedUser()

            Dim cleanRoomId = If(roomId, String.Empty).Trim()
            If String.IsNullOrWhiteSpace(cleanRoomId) Then
                Throw New FriendsOrderException(FriendsOrderErrorKind.InvalidInput, "ID room tidak valid.")
            End If

            Try
                Dim room = Await _dataSource.GetRoomByIdAsync(cleanRoomId).ConfigureAwait(False)
                If room Is Nothing Then
                    Throw New FriendsOrderException(
                        FriendsOrderErrorKind.Unauthorized,
                        "Room tidak ditemukan atau Anda tidak memiliki akses ke room ini.")
                End If

                Dim members = Await _dataSource.GetRoomMembersAsync(cleanRoomId).ConfigureAwait(False)
                Dim parentOrder = Await _dataSource.GetParentOrderByRoomAsync(cleanRoomId).ConfigureAwait(False)
                Dim items As List(Of BrewOrderItem)
                If parentOrder IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(parentOrder.Id) Then
                    items = Await _dataSource.GetOrderItemsAsync(parentOrder.Id).ConfigureAwait(False)
                Else
                    items = New List(Of BrewOrderItem)()
                End If

                Return BuildOverviewSnapshot(room, members, parentOrder, items, user.Id)
            Catch ex As FriendsOrderException
                Throw
            Catch ex As Exception
                Throw MapException(ex, "Gagal memuat ringkasan room.")
            End Try
        End Function

        Public Async Function AddDraftToRoomAsync(roomId As String, draft As ConfiguredDrinkDraft) As Task(Of BrewOrderItem)
            EnsureConfigured()
            Dim user = RequireAuthenticatedUser()
            Dim payload = RoomOrderItemPayload.FromConfiguredDrinkDraft(draft, user.Id)
            Return Await AddOrderItemPayloadAsync(roomId, payload, user).ConfigureAwait(False)
        End Function

        Public Async Function AddMenuItemToRoomAsync(
            roomId As String,
            menuItem As MenuItem,
            quantity As Integer,
            Optional notes As String = ""
        ) As Task(Of BrewOrderItem)
            EnsureConfigured()
            Dim user = RequireAuthenticatedUser()
            Dim payload = RoomOrderItemPayload.FromMenuItem(menuItem, user.Id, quantity, notes)
            Return Await AddOrderItemPayloadAsync(roomId, payload, user).ConfigureAwait(False)
        End Function

        Private Async Function AddOrderItemPayloadAsync(
            roomId As String,
            payload As RoomOrderItemPayload,
            user As UserProfile
        ) As Task(Of BrewOrderItem)
            Dim cleanRoomId = If(roomId, String.Empty).Trim()
            If String.IsNullOrWhiteSpace(cleanRoomId) Then
                Throw New FriendsOrderException(FriendsOrderErrorKind.InvalidInput, "Pilih room terlebih dahulu sebelum menambahkan item.")
            End If

            Try
                Dim room = Await _dataSource.GetRoomByIdAsync(cleanRoomId).ConfigureAwait(False)
                If room Is Nothing Then
                    Throw New FriendsOrderException(
                        FriendsOrderErrorKind.Unauthorized,
                        "Room tidak ditemukan atau Anda bukan anggota room ini.")
                End If

                EnsureRoomOpenForMutations(room)

                Dim members = Await _dataSource.GetRoomMembersAsync(cleanRoomId).ConfigureAwait(False)
                Dim isMember = room.IsHost(user.Id) OrElse
                               members.Any(Function(m) String.Equals(m.UserId, user.Id, StringComparison.OrdinalIgnoreCase))
                If Not isMember Then
                    Throw New FriendsOrderException(
                        FriendsOrderErrorKind.Unauthorized,
                        "Hanya anggota room yang dapat menambahkan item ke pesanan grup.")
                End If

                Dim parentOrder = Await EnsureParentOrderAsync(room, user.Id).ConfigureAwait(False)
                If Not String.Equals(parentOrder.Status, "Waiting", StringComparison.OrdinalIgnoreCase) Then
                    Throw New FriendsOrderException(
                        FriendsOrderErrorKind.RoomSubmitted,
                        $"Order induk grup sudah berstatus '{parentOrder.Status}' dan tidak dapat menerima item baru.")
                End If

                Dim itemModel = payload.ToBrewOrderItem(parentOrder.Id)
                Return Await _dataSource.InsertOrderItemAsync(itemModel).ConfigureAwait(False)
            Catch ex As FriendsOrderException
                Throw
            Catch ex As Exception
                Throw MapException(ex, "Gagal menambahkan item ke pesanan grup.")
            End Try
        End Function

        Public Async Function LockRoomAsync(roomId As String) As Task(Of OrderRoom)
            EnsureConfigured()
            Dim user = RequireAuthenticatedUser()

            Dim cleanRoomId = If(roomId, String.Empty).Trim()
            If String.IsNullOrWhiteSpace(cleanRoomId) Then
                Throw New FriendsOrderException(FriendsOrderErrorKind.InvalidInput, "ID room tidak valid.")
            End If

            Try
                Dim room = Await _dataSource.GetRoomByIdAsync(cleanRoomId).ConfigureAwait(False)
                If room Is Nothing Then
                    Throw New FriendsOrderException(
                        FriendsOrderErrorKind.Unauthorized,
                        "Room tidak ditemukan atau Anda tidak memiliki akses ke room ini.")
                End If

                If Not room.IsHost(user.Id) Then
                    Throw New FriendsOrderException(
                        FriendsOrderErrorKind.Unauthorized,
                        "Hanya host room yang dapat mengunci room ini.")
                End If

                EnsureRoomOpenForMutations(room)

                Return Await _dataSource.LockRoomAsync(cleanRoomId, DateTimeOffset.UtcNow).ConfigureAwait(False)
            Catch ex As FriendsOrderException
                Throw
            Catch ex As Exception
                Throw MapException(ex, "Gagal mengunci room.")
            End Try
        End Function

        Public Async Function GetAvailableMenuItemsAsync() As Task(Of List(Of MenuItem))
            EnsureConfigured()
            Try
                Return Await _dataSource.GetAvailableMenuItemsAsync().ConfigureAwait(False)
            Catch ex As FriendsOrderException
                Throw
            Catch ex As Exception
                Throw MapException(ex, "Gagal memuat daftar menu tersedia.")
            End Try
        End Function

        Public Function SubscribeRoomUpdatesAsync(
            roomId As String,
            parentOrderId As String,
            onChanged As Action(Of String),
            onStateChanged As Action(Of FriendsRoomRealtimeState, String)
        ) As Task(Of FriendsRoomRealtimeSubscription)
            EnsureConfigured()
            RequireAuthenticatedUser()
            Return _dataSource.SubscribeRoomAsync(roomId, parentOrderId, onChanged, onStateChanged)
        End Function

        Public Shared Function BuildOverviewSnapshot(
            room As OrderRoom,
            members As List(Of RoomMember),
            parentOrder As BrewOrder,
            items As List(Of BrewOrderItem),
            currentUserId As String
        ) As RoomOverviewSnapshot
            Dim safeMembers = If(members, New List(Of RoomMember)())
            Dim safeItems = If(items, New List(Of BrewOrderItem)())

            Dim isHost = room IsNot Nothing AndAlso room.IsHost(currentUserId)
            Dim isMember = isHost OrElse safeMembers.Any(Function(m) String.Equals(m.UserId, currentUserId, StringComparison.OrdinalIgnoreCase))

            Dim hostMember = safeMembers.FirstOrDefault(Function(m) room IsNot Nothing AndAlso room.IsHost(m.UserId))
            Dim hostDisplayName = If(hostMember IsNot Nothing, hostMember.DisplayLabel, "Host")

            Dim summaryByUser As New Dictionary(Of String, RoomMemberOrderSummary)(StringComparer.OrdinalIgnoreCase)
            Dim orderedSummaries As New List(Of RoomMemberOrderSummary)()

            For Each member In safeMembers
                If member Is Nothing OrElse String.IsNullOrWhiteSpace(member.UserId) Then Continue For
                If summaryByUser.ContainsKey(member.UserId) Then Continue For

                Dim summary As New RoomMemberOrderSummary With {
                    .MemberUserId = member.UserId,
                    .DisplayName = member.DisplayLabel,
                    .IsHost = room IsNot Nothing AndAlso room.IsHost(member.UserId),
                    .JoinedAt = member.JoinedAt
                }
                summaryByUser(member.UserId) = summary
                orderedSummaries.Add(summary)
            Next

            For Each item In safeItems
                If item Is Nothing Then Continue For
                Dim ownerKey = If(String.IsNullOrWhiteSpace(item.MemberUserId), "__unassigned__", item.MemberUserId.Trim())
                Dim bucket As RoomMemberOrderSummary = Nothing
                If Not summaryByUser.TryGetValue(ownerKey, bucket) Then
                    bucket = New RoomMemberOrderSummary With {
                        .MemberUserId = If(ownerKey = "__unassigned__", String.Empty, ownerKey),
                        .DisplayName = If(ownerKey = "__unassigned__", "Item Grup (Tanpa Anggota)", "Anggota Lain"),
                        .IsHost = room IsNot Nothing AndAlso room.IsHost(ownerKey)
                    }
                    summaryByUser(ownerKey) = bucket
                    orderedSummaries.Add(bucket)
                End If
                bucket.Items.Add(item)
            Next

            ' Sort host first, then by join time.
            orderedSummaries = orderedSummaries.
                OrderByDescending(Function(s) s.IsHost).
                ThenBy(Function(s) If(s.JoinedAt.HasValue, s.JoinedAt.Value, DateTimeOffset.MaxValue)).
                ThenBy(Function(s) s.DisplayName, StringComparer.OrdinalIgnoreCase).
                ToList()

            Return New RoomOverviewSnapshot With {
                .Room = room,
                .Members = safeMembers,
                .ParentOrder = parentOrder,
                .Items = safeItems,
                .MemberSummaries = orderedSummaries,
                .HostDisplayName = hostDisplayName,
                .IsCurrentUserHost = isHost,
                .IsCurrentUserMember = isMember,
                .HostMemberTriggerVerified = hostMember IsNot Nothing
            }
        End Function

        Private Async Function EnsureParentOrderAsync(room As OrderRoom, currentUserId As String) As Task(Of BrewOrder)
            Dim existing = Await _dataSource.GetParentOrderByRoomAsync(room.Id).ConfigureAwait(False)
            If existing IsNot Nothing Then
                Return existing
            End If

            Dim newOrder As New BrewOrder With {
                .OrderNumber = GenerateGroupOrderNumber(room.RoomCode),
                .CustomerId = currentUserId,
                .RoomId = room.Id,
                .Status = "Waiting",
                .PaymentMethod = "cashier_sim",
                .PaymentStatus = "pending",
                .Subtotal = 0D,
                .DiscountAmount = 0D,
                .TotalAmount = 0D
            }

            Dim conflictDetected = False
            Try
                Return Await _dataSource.InsertParentOrderAsync(newOrder).ConfigureAwait(False)
            Catch ex As FriendsOrderException When ex.ErrorKind = FriendsOrderErrorKind.ParentOrderConflict OrElse ex.ErrorKind = FriendsOrderErrorKind.CodeCollision
                conflictDetected = True
            Catch ex As Exception
                Dim mapped = MapException(ex, "Gagal membuat order induk grup.")
                If mapped.ErrorKind = FriendsOrderErrorKind.ParentOrderConflict OrElse mapped.ErrorKind = FriendsOrderErrorKind.CodeCollision Then
                    conflictDetected = True
                Else
                    Throw mapped
                End If
            End Try

            ' Another concurrent member created the parent order first (SQLSTATE 23505 on uq_orders_single_parent_per_room).
            ' Re-fetch and reuse the winning parent order so each room strictly has one parent order.
            If conflictDetected Then
                Dim winner = Await _dataSource.GetParentOrderByRoomAsync(room.Id).ConfigureAwait(False)
                If winner IsNot Nothing Then
                    Return winner
                End If
            End If

            Throw New FriendsOrderException(
                FriendsOrderErrorKind.ParentOrderConflict,
                "Order induk grup sedang dibuat oleh anggota lain. Silakan coba lagi.")
        End Function

        Private Shared Function GenerateGroupOrderNumber(roomCode As String) As String
            Dim prefix = NormalizeRoomCode(roomCode)
            If prefix.Length > 6 Then
                prefix = prefix.Substring(0, 6)
            End If
            If String.IsNullOrEmpty(prefix) Then
                prefix = "ROOM"
            End If
            Dim suffix = GenerateRoomCode(4)
            Return $"GRP-{prefix}-{DateTimeOffset.UtcNow:yyMMddHHmm}-{suffix}"
        End Function

        Private Shared Sub EnsureRoomOpenForMutations(room As OrderRoom)
            If room.IsOpen Then Return

            If room.IsLocked Then
                Throw New FriendsOrderException(
                    FriendsOrderErrorKind.RoomLocked,
                    "Room sudah dikunci oleh host. Tidak dapat menambahkan item atau mengubah room.")
            End If
            If room.IsSubmitted Then
                Throw New FriendsOrderException(
                    FriendsOrderErrorKind.RoomSubmitted,
                    "Pesanan room ini sudah dikirim dan tidak dapat diubah lagi.")
            End If
            If room.IsCancelled Then
                Throw New FriendsOrderException(
                    FriendsOrderErrorKind.RoomCancelled,
                    "Room ini sudah dibatalkan dan tidak dapat digunakan lagi.")
            End If

            Throw New FriendsOrderException(
                FriendsOrderErrorKind.RoomUnavailable,
                $"Room berstatus '{room.StatusDisplayLabel}' dan tidak menerima perubahan.")
        End Sub

        Private Sub EnsureConfigured()
            If Not _dataSource.IsConfigured Then
                Throw New FriendsOrderException(
                    FriendsOrderErrorKind.Unconfigured,
                    "Koneksi Supabase belum diatur. Buka Koneksi Supabase untuk mengisi URL dan publishable/anon key.")
            End If
        End Sub

        Private Function RequireAuthenticatedUser() As UserProfile
            Dim user = _currentUserProvider()
            If user Is Nothing OrElse String.IsNullOrWhiteSpace(user.Id) Then
                Throw New FriendsOrderException(
                    FriendsOrderErrorKind.Unauthenticated,
                    "Silakan masuk ke akun Anda terlebih dahulu untuk menggunakan Friends Order Room.")
            End If
            Return user
        End Function

        Public Shared Function MapException(ex As Exception, fallbackMessage As String) As FriendsOrderException
            If TypeOf ex Is FriendsOrderException Then
                Return DirectCast(ex, FriendsOrderException)
            End If

            If TypeOf ex Is SupabaseNotConfiguredException Then
                Return New FriendsOrderException(FriendsOrderErrorKind.Unconfigured, ex.Message, ex)
            End If

            If TypeOf ex Is TaskCanceledException OrElse TypeOf ex Is TimeoutException Then
                Return New FriendsOrderException(
                    FriendsOrderErrorKind.NetworkError,
                    "Permintaan ke Supabase timeout. Periksa koneksi internet lalu coba lagi.",
                    ex)
            End If

            If TypeOf ex Is HttpRequestException Then
                Return New FriendsOrderException(
                    FriendsOrderErrorKind.NetworkError,
                    "Gagal menghubungi server Supabase. Periksa koneksi internet Anda lalu coba lagi.",
                    ex)
            End If

            Dim raw = If(ex?.Message, String.Empty)
            Dim content = String.Empty
            If TypeOf ex Is PostgrestException Then
                content = If(DirectCast(ex, PostgrestException).Content, String.Empty)
            End If
            Dim combined = (raw & " " & content).Trim()

            If ContainsAny(combined, "PGRST202", "42883", "could not find the function", "join_room_by_code") AndAlso
               ContainsAny(combined, "not found", "does not exist", "PGRST202", "42883", "schema cache") Then
                Return New FriendsOrderException(
                    FriendsOrderErrorKind.RpcNotDeployed,
                    "RPC 'join_room_by_code' belum tersedia di Supabase bersama. Informasikan Fikri untuk menerapkan migration 20261119_secure_order_rooms_rls_and_rpc.sql.",
                    ex)
            End If

            If ContainsAny(combined, "Room is locked", "no longer accepting members") Then
                Return New FriendsOrderException(
                    FriendsOrderErrorKind.RoomLocked,
                    "Room sudah dikunci oleh host dan tidak menerima anggota baru.",
                    ex)
            End If

            If ContainsAny(combined, "cancelled", "dibatalkan") Then
                Return New FriendsOrderException(
                    FriendsOrderErrorKind.RoomCancelled,
                    "Room sudah dibatalkan dan tidak dapat dimasuki.",
                    ex)
            End If

            If ContainsAny(combined, "submitted", "sudah dikirim") Then
                Return New FriendsOrderException(
                    FriendsOrderErrorKind.RoomSubmitted,
                    "Pesanan room ini sudah dikirim dan tidak menerima anggota baru.",
                    ex)
            End If

            If ContainsAny(combined, "Room is no longer available") Then
                Return New FriendsOrderException(
                    FriendsOrderErrorKind.RoomUnavailable,
                    "Room sudah tidak aktif (dibatalkan atau sudah dikirim) dan tidak menerima anggota baru.",
                    ex)
            End If

            If ContainsAny(combined, "Invalid or unavailable room code") Then
                Return New FriendsOrderException(
                    FriendsOrderErrorKind.InvalidRoomCode,
                    "Kode room tidak ditemukan atau tidak valid. Periksa kembali kode undangan dari host.",
                    ex)
            End If

            If ContainsAny(combined, "uq_orders_single_parent_per_room", "orders_room_id") AndAlso
               ContainsAny(combined, "23505", "duplicate key", "unique constraint") Then
                Return New FriendsOrderException(
                    FriendsOrderErrorKind.ParentOrderConflict,
                    "Order induk untuk room ini sudah dibuat oleh anggota lain.",
                    ex)
            End If

            If ContainsAny(combined, "23505", "duplicate key", "order_rooms_room_code_key") Then
                Return New FriendsOrderException(
                    FriendsOrderErrorKind.CodeCollision,
                    "Kode room sudah digunakan. Silakan coba kode lain.",
                    ex)
            End If

            If ContainsAny(combined,
                           "currently unavailable",
                           "does not belong to the selected menu item",
                           "is no longer available",
                           "does not support customization",
                           "Invalid item quantity",
                           "Invalid option_id",
                           "Duplicate option_id",
                           "Multiple options selected",
                           "Menu item not found",
                           "Menu option") Then
                Return New FriendsOrderException(
                    FriendsOrderErrorKind.PricingValidationFailed,
                    "Validasi menu/opsi ditolak oleh server: " & If(String.IsNullOrWhiteSpace(raw), "periksa ketersediaan menu dan opsi lalu muat ulang.", raw),
                    ex)
            End If

            If ContainsAny(combined, "42501", "Authentication required", "permission denied", "row-level security", "JWT") Then
                Return New FriendsOrderException(
                    FriendsOrderErrorKind.RlsBlocked,
                    "Akses ditolak oleh kebijakan keamanan (RLS/session). Pastikan Anda sudah login dan memiliki hak akses pada room ini.",
                    ex)
            End If

            Return New FriendsOrderException(FriendsOrderErrorKind.Unknown, fallbackMessage, ex)
        End Function

        Private Shared Function ContainsAny(source As String, ParamArray needles As String()) As Boolean
            If String.IsNullOrEmpty(source) Then Return False
            For Each needle In needles
                If source.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0 Then
                    Return True
                End If
            Next
            Return False
        End Function
    End Class
End Namespace

