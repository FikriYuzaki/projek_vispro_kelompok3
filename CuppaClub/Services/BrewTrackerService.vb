Option Strict On
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks
Imports Supabase.Postgrest
Imports Supabase.Postgrest.Models
Imports Supabase.Realtime.Exceptions
Imports Supabase.Realtime.Interfaces
Imports Supabase.Realtime.PostgresChanges
Imports RealtimeConstants = Supabase.Realtime.Constants

Namespace Services
    Public Enum BrewRealtimeConnectionState
        Disconnected = 0
        Connecting = 1
        Connected = 2
        Reconnecting = 3
        [Error] = 4
    End Enum

    Public NotInheritable Class BrewOrdersRealtimeSubscription
        Implements IDisposable

        Private ReadOnly _syncRoot As New Object()
        Private _realtimeClient As IRealtimeClient(Of Supabase.Realtime.RealtimeSocket, Supabase.Realtime.RealtimeChannel)
        Private _channel As Supabase.Realtime.RealtimeChannel
        Private _postgresHandler As IRealtimeChannel.PostgresChangesHandler
        Private _channelStateHandler As IRealtimeChannel.StateChangedHandler
        Private _channelErrorHandler As IRealtimeChannel.ErrorEventHandler
        Private _socketStateHandler As IRealtimeClient(Of Supabase.Realtime.RealtimeSocket, Supabase.Realtime.RealtimeChannel).SocketStateEventHandler
        Private _disposed As Boolean = False

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

        Public ReadOnly Property ChannelTopic As String
            Get
                SyncLock _syncRoot
                    If _disposed OrElse _channel Is Nothing Then
                        Return String.Empty
                    End If
                    Return _channel.Topic
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
            ' Do not call _realtimeClient.Disconnect() because the shared client may be used by other modules.
        End Sub
    End Class

    Public Class BrewTrackerService

        ' 1. Mengambil semua order yang dapat diakses user
        Public Shared Async Function GetOrdersAsync() As Task(Of List(Of Models.BrewOrder))
            Dim client = Await SupabaseClientProvider.GetClientAsync()

            Dim response = Await client.From(Of Models.BrewOrder)().Get()

            Return response.Models
        End Function


        ' 2. Mengambil item/menu berdasarkan ID order
        Public Shared Async Function GetOrderItemsAsync(orderId As String) As Task(Of List(Of Models.BrewOrderItem))
            Dim client = Await SupabaseClientProvider.GetClientAsync()

            Dim response = Await client.From(Of Models.BrewOrderItem)().
                Filter("order_id", Constants.Operator.Equals, orderId).
                Get()

            Return response.Models
        End Function


        ' 3. Mengambil riwayat perubahan status order
        Public Shared Async Function GetOrderHistoryAsync(orderId As String) As Task(Of List(Of Models.BrewOrderStatusHistory))
            Dim client = Await SupabaseClientProvider.GetClientAsync()

            Dim response = Await client.From(Of Models.BrewOrderStatusHistory)().
                Filter("order_id", Constants.Operator.Equals, orderId).
                Order("changed_at", Constants.Ordering.Descending).
                Get()

            Return response.Models
        End Function


        ' 4. Memvalidasi perpindahan status order
        Public Shared Function IsValidStatusTransition(currentStatus As String, newStatus As String) As Boolean

            Select Case currentStatus

                Case "Waiting"
                    Return newStatus = "Preparing"

                Case "Preparing"
                    Return newStatus = "Ready"

                Case "Ready"
                    Return newStatus = "Completed"

                Case "Completed"
                    Return False

                Case Else
                    Return False

            End Select

        End Function


        ' 5. Mengubah status order
        Public Shared Async Function UpdateOrderStatusAsync(orderId As String, newStatus As String) As Task

            ' Ambil order untuk mengecek status saat ini
            Dim orders = Await GetOrdersAsync()

            Dim currentOrder = orders.FirstOrDefault(
                Function(orderItem) orderItem.Id = orderId
            )

            If currentOrder Is Nothing Then
                Throw New InvalidOperationException(
                    "Order tidak ditemukan atau tidak dapat diakses."
                )
            End If


            ' Validasi perpindahan status
            If Not IsValidStatusTransition(currentOrder.Status, newStatus) Then
                Throw New InvalidOperationException(
                    "Perubahan status order tidak valid."
                )
            End If


            ' Update status ke Supabase
            Dim client = Await SupabaseClientProvider.GetClientAsync()

            Await client.From(Of Models.BrewOrder)().
                Filter("id", Constants.Operator.Equals, orderId).
                Set(Function(orderItem) CObj(orderItem.Status), newStatus).
                Update()

        End Function

        ''' <summary>
        ''' Opens an explicit Realtime subscription for public.orders using the shared Supabase client.
        ''' </summary>
        Public Shared Async Function SubscribeOrdersAsync(
            onOrderChanged As Action(Of String, String),
            onStateChanged As Action(Of BrewRealtimeConnectionState, String)
        ) As Task(Of BrewOrdersRealtimeSubscription)
            Dim client = Await SupabaseClientProvider.GetClientAsync().ConfigureAwait(False)
            Dim realtime = client.Realtime

            ' Ensure RLS evaluates under the active user session token before connecting.
            If client.Auth IsNot Nothing AndAlso
               client.Auth.CurrentSession IsNot Nothing AndAlso
               Not String.IsNullOrWhiteSpace(client.Auth.CurrentSession.AccessToken) Then
                realtime.SetAuth(client.Auth.CurrentSession.AccessToken)
            End If

            onStateChanged?.Invoke(BrewRealtimeConnectionState.Connecting, "Menghubungkan ke pembaruan realtime...")

            If realtime.ConnectionState <> RealtimeConstants.ConnectionState.Open Then
                Await realtime.ConnectAsync().ConfigureAwait(False)
            End If

            Dim topic As String = "brew-tracker-orders-" & Guid.NewGuid().ToString("N")
            Dim channel As Supabase.Realtime.RealtimeChannel = realtime.Channel(topic)
            Dim subscriptionRef As BrewOrdersRealtimeSubscription = Nothing

            Dim pgHandler As IRealtimeChannel.PostgresChangesHandler =
                Sub(sender As IRealtimeChannel, change As PostgresChangesResponse)
                    If subscriptionRef IsNot Nothing AndAlso subscriptionRef.IsDisposed Then Return

                    Dim orderId As String = String.Empty
                    Dim status As String = String.Empty
                    Try
                        If change IsNot Nothing Then
                            Dim changedModel As Models.BrewOrder = change.Model(Of Models.BrewOrder)()
                            If changedModel Is Nothing Then
                                changedModel = change.OldModel(Of Models.BrewOrder)()
                            End If
                            If changedModel IsNot Nothing Then
                                orderId = If(changedModel.Id, String.Empty)
                                status = If(changedModel.Status, String.Empty)
                            End If
                        End If
                    Catch
                        ' Payload parsing can vary on DELETE/partial updates; caller still refreshes full order state.
                    End Try

                    onOrderChanged?.Invoke(orderId, status)
                End Sub

            Dim chStateHandler As IRealtimeChannel.StateChangedHandler =
                Sub(sender As IRealtimeChannel, state As RealtimeConstants.ChannelState)
                    If subscriptionRef IsNot Nothing AndAlso subscriptionRef.IsDisposed Then Return

                    Select Case state
                        Case RealtimeConstants.ChannelState.Joined
                            onStateChanged?.Invoke(BrewRealtimeConnectionState.Connected, "Terhubung ke pembaruan realtime.")
                        Case RealtimeConstants.ChannelState.Joining
                            onStateChanged?.Invoke(BrewRealtimeConnectionState.Connecting, "Menghubungkan ulang channel pesanan...")
                        Case RealtimeConstants.ChannelState.Errored
                            onStateChanged?.Invoke(BrewRealtimeConnectionState.[Error], "Channel realtime mengalami gangguan.")
                        Case RealtimeConstants.ChannelState.Closed, RealtimeConstants.ChannelState.Leaving
                            onStateChanged?.Invoke(BrewRealtimeConnectionState.Disconnected, "Koneksi realtime terputus.")
                    End Select
                End Sub

            Dim chErrorHandler As IRealtimeChannel.ErrorEventHandler =
                Sub(sender As IRealtimeChannel, ex As RealtimeException)
                    If subscriptionRef IsNot Nothing AndAlso subscriptionRef.IsDisposed Then Return
                    Dim msg As String = If(ex IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(ex.Message), ex.Message, "Channel error.")
                    onStateChanged?.Invoke(BrewRealtimeConnectionState.[Error], msg)
                End Sub

            Dim sockHandler As IRealtimeClient(Of Supabase.Realtime.RealtimeSocket, Supabase.Realtime.RealtimeChannel).SocketStateEventHandler =
                Sub(sender As IRealtimeClient(Of Supabase.Realtime.RealtimeSocket, Supabase.Realtime.RealtimeChannel), state As RealtimeConstants.SocketState)
                    If subscriptionRef IsNot Nothing AndAlso subscriptionRef.IsDisposed Then Return

                    Select Case state
                        Case RealtimeConstants.SocketState.Open
                            Dim mapped = If(channel.IsJoined, BrewRealtimeConnectionState.Connected, BrewRealtimeConnectionState.Reconnecting)
                            Dim detail = If(channel.IsJoined, "Terhubung ke pembaruan realtime.", "Socket realtime terhubung, menyambungkan channel...")
                            onStateChanged?.Invoke(mapped, detail)
                        Case RealtimeConstants.SocketState.Reconnect
                            onStateChanged?.Invoke(BrewRealtimeConnectionState.Reconnecting, "Menyambung ulang koneksi realtime...")
                        Case RealtimeConstants.SocketState.Close
                            onStateChanged?.Invoke(BrewRealtimeConnectionState.Disconnected, "Koneksi socket realtime terputus.")
                        Case RealtimeConstants.SocketState.[Error]
                            onStateChanged?.Invoke(BrewRealtimeConnectionState.[Error], "Socket realtime mengalami error.")
                    End Select
                End Sub

            subscriptionRef = New BrewOrdersRealtimeSubscription(
                realtime,
                channel,
                pgHandler,
                chStateHandler,
                chErrorHandler,
                sockHandler
            )

            Try
                Dim filter As New PostgresChangesFilter With {
                    .Schema = "public",
                    .Table = "orders"
                }
                channel.OnPostgresChange(pgHandler, PostgresChangesOptions.ListenType.All, filter)
                channel.AddStateChangedHandler(chStateHandler)
                channel.AddErrorHandler(chErrorHandler)
                realtime.AddStateChangedHandler(sockHandler)

                Await channel.Subscribe(10000).ConfigureAwait(False)
                onStateChanged?.Invoke(BrewRealtimeConnectionState.Connected, "Terhubung ke pembaruan realtime.")
                Return subscriptionRef
            Catch
                subscriptionRef.Dispose()
                Throw
            End Try
        End Function

    End Class
End Namespace
