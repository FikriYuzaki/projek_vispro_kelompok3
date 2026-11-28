Option Strict On
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Linq
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Globalization
Imports System.Text.Json
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports CuppaClub.Core
Imports CuppaClub.Models
Imports CuppaClub.Services

Namespace Features

    Public Class BrewTrackerControl

        Private Const FallbackPollIntervalMs As Integer = 25000
        Private Const RealtimeReconnectIntervalMs As Integer = 15000
        Private Const RealtimeDebounceIntervalMs As Integer = 350

        Private _orders As New List(Of BrewOrder)()
        Private _selectedOrderId As String = String.Empty
        Private _customerDetailOrderId As String = String.Empty

        Private _isLoading As Boolean = False
        Private _pendingBackgroundRefresh As Boolean = False
        Private _isDisposed As Boolean = False
        Private _hasLoaded As Boolean = False
        Private _initialLiveSetupPending As Boolean = False
        Private _wasHiddenSinceLoad As Boolean = False
        Private _hasPendingRefreshWhileHidden As Boolean = False
        Private _runtimeInitialized As Boolean = False
        Private _cleanupDone As Boolean = False

        Private _loadVersion As Integer = 0
        Private _detailVersion As Integer = 0

        Private _realtimeSubscription As BrewOrdersRealtimeSubscription
        Private _realtimeConnectInFlight As Boolean = False
        Private _realtimeGeneration As Integer = 0
        Private _realtimeState As BrewRealtimeConnectionState = BrewRealtimeConnectionState.Disconnected
        Private _hadRealtimeDisruption As Boolean = False
        Private _realtimeUnavailableOnServer As Boolean = False
        Private _lastUpdatedAt As DateTime?

        Private _fallbackPollTimer As Timer
        Private _reconnectTimer As Timer
        Private _realtimeDebounceTimer As Timer

        Private ReadOnly _canvas As Color = Color.FromArgb(250, 244, 236)
        Private ReadOnly _surface As Color = Color.FromArgb(255, 253, 249)
        Private ReadOnly _border As Color = Color.FromArgb(226, 211, 195)
        Private ReadOnly _primary As Color = Color.FromArgb(59, 38, 28)
        Private ReadOnly _secondary As Color = Color.FromArgb(110, 86, 71)
        Private ReadOnly _mocha As Color = Color.FromArgb(107, 68, 46)
        Private ReadOnly _strawberry As Color = Color.FromArgb(184, 84, 103)
        Private ReadOnly _strawberryTint As Color = Color.FromArgb(251, 236, 239)
        Private ReadOnly _sageTint As Color = Color.FromArgb(235, 243, 233)
        Private ReadOnly _sageText As Color = Color.FromArgb(66, 112, 72)
        Private ReadOnly _warningTint As Color = Color.FromArgb(250, 241, 221)
        Private ReadOnly _warningText As Color = Color.FromArgb(135, 101, 43)
        Private ReadOnly _errorTint As Color = Color.FromArgb(250, 232, 230)
        Private ReadOnly _errorText As Color = Color.FromArgb(153, 59, 52)
        Private ReadOnly _taupeTint As Color = Color.FromArgb(240, 234, 228)

        Public Sub New()

            InitializeComponent()

            If LicenseManager.UsageMode = LicenseUsageMode.Designtime OrElse DesignMode Then Return

            _runtimeInitialized = True
            Me.BackColor = _canvas

            _fallbackPollTimer = New Timer With {
                .Interval = FallbackPollIntervalMs,
                .Enabled = False
            }
            _reconnectTimer = New Timer With {
                .Interval = RealtimeReconnectIntervalMs,
                .Enabled = False
            }
            _realtimeDebounceTimer = New Timer With {
                .Interval = RealtimeDebounceIntervalMs,
                .Enabled = False
            }

            AddHandler _refreshButton.Click, AddressOf RefreshButton_Click
            AddHandler _headerPanel.Resize, AddressOf HeaderPanel_Resize
            AddHandler _fallbackPollTimer.Tick, AddressOf FallbackPollTimer_Tick
            AddHandler _reconnectTimer.Tick, AddressOf ReconnectTimer_Tick
            AddHandler _realtimeDebounceTimer.Tick, AddressOf RealtimeDebounceTimer_Tick
            AddHandler AppSession.SessionChanged, AddressOf SessionChanged
            AddHandler SupabaseClientProvider.ConfigurationChanged, AddressOf ConfigurationChanged
            AddHandler Me.VisibleChanged, AddressOf Control_VisibleChanged
            AddHandler Me.Disposed, AddressOf Control_Disposed
            AddHandler Me.Load, AddressOf Control_Load

            ApplyViewMode()
            UpdateLastUpdatedLabel()

        End Sub

        ' =========================================================
        ' LIFECYCLE & LIVE UPDATES
        ' =========================================================

        Private Async Sub Control_Load(
            sender As Object,
            e As EventArgs
        )

            If _isDisposed OrElse LicenseManager.UsageMode = LicenseUsageMode.Designtime OrElse DesignMode Then Return

            _hasLoaded = True
            _initialLiveSetupPending = True
            PositionHeaderStatusLabel()

            Try
                Await RefreshDataAsync(False)

                If Not _isDisposed AndAlso Me.Visible Then
                    Await EnsureRealtimeSubscriptionAsync()
                End If
            Catch ex As Exception
                If Not _isDisposed Then
                    ShowState(
                        "Tracker gagal dimuat. Silakan coba Refresh.",
                        False,
                        _errorTint,
                        _errorText
                    )
                End If
            Finally
                _initialLiveSetupPending = False
            End Try

        End Sub

        Private Sub SessionChanged(
            sender As Object,
            e As EventArgs
        )

            If _isDisposed Then Return

            _realtimeGeneration += 1
            TeardownRealtimeSubscription()
            StopLiveUpdateTimers()
            _realtimeState = BrewRealtimeConnectionState.Disconnected
            _hadRealtimeDisruption = False
            _realtimeUnavailableOnServer = False

            ApplyViewMode()

            _orders.Clear()
            _selectedOrderId = String.Empty
            _customerDetailOrderId = String.Empty
            _loadVersion += 1
            _detailVersion += 1
            UpdateLastUpdatedLabel()

            If Me.IsHandleCreated Then
                BeginInvoke(
                    New Action(
                        Async Sub()
                            If _isDisposed Then Return
                            If _isLoading Then
                                _pendingBackgroundRefresh = True
                            Else
                                Await RefreshDataAsync(False)
                            End If
                            If Not _isDisposed AndAlso Me.Visible Then
                                Await EnsureRealtimeSubscriptionAsync()
                            End If
                        End Sub
                    )
                )
            End If

        End Sub

        Private Sub ConfigurationChanged(
            sender As Object,
            e As EventArgs
        )

            If _isDisposed Then Return

            _realtimeGeneration += 1
            TeardownRealtimeSubscription()
            StopLiveUpdateTimers()
            _realtimeState = BrewRealtimeConnectionState.Disconnected
            _hadRealtimeDisruption = False
            _realtimeUnavailableOnServer = False

            If Me.IsHandleCreated Then
                BeginInvoke(
                    New Action(
                        Async Sub()
                            If _isDisposed Then Return
                            Await RefreshDataAsync(False)
                            If Not _isDisposed AndAlso Me.Visible Then
                                Await EnsureRealtimeSubscriptionAsync()
                            End If
                        End Sub
                    )
                )
            End If

        End Sub

        Private Async Sub Control_VisibleChanged(
            sender As Object,
            e As EventArgs
        )

            If _isDisposed Then Return

            If Not Me.Visible Then
                If _hasLoaded Then
                    _wasHiddenSinceLoad = True
                End If
                StopLiveUpdateTimers()
                UpdateLastUpdatedLabel()
                Return
            End If

            PositionHeaderStatusLabel()

            If _hasLoaded AndAlso Not _initialLiveSetupPending Then
                Dim shouldRefreshOnResume As Boolean = _wasHiddenSinceLoad OrElse _hasPendingRefreshWhileHidden
                _wasHiddenSinceLoad = False
                _hasPendingRefreshWhileHidden = False

                If _realtimeState <> BrewRealtimeConnectionState.Connected AndAlso
                   SupabaseClientProvider.IsConfigured Then
                    StartFallbackPollingIfNeeded()
                    If Not _realtimeUnavailableOnServer Then
                        Await EnsureRealtimeSubscriptionAsync()
                    End If
                Else
                    UpdateLastUpdatedLabel()
                End If

                If shouldRefreshOnResume AndAlso Not _isDisposed AndAlso Me.Visible Then
                    If _realtimeDebounceTimer IsNot Nothing Then
                        _realtimeDebounceTimer.Stop()
                    End If
                    Await RefreshDataAsync(True)
                End If
            End If

        End Sub

        Private Sub HeaderPanel_Resize(
            sender As Object,
            e As EventArgs
        )

            PositionHeaderStatusLabel()

        End Sub

        Private Sub Control_Disposed(
            sender As Object,
            e As EventArgs
        )

            CleanupRuntimeResources()

        End Sub

        Private Sub CleanupRuntimeResources()

            If _cleanupDone Then Return
            _cleanupDone = True
            _isDisposed = True
            _pendingBackgroundRefresh = False
            _wasHiddenSinceLoad = False
            _hasPendingRefreshWhileHidden = False

            _realtimeGeneration += 1
            _loadVersion += 1
            _detailVersion += 1

            If Not _runtimeInitialized Then Return

            RemoveHandler AppSession.SessionChanged, AddressOf SessionChanged
            RemoveHandler SupabaseClientProvider.ConfigurationChanged, AddressOf ConfigurationChanged
            RemoveHandler Me.VisibleChanged, AddressOf Control_VisibleChanged
            RemoveHandler Me.Disposed, AddressOf Control_Disposed
            RemoveHandler Me.Load, AddressOf Control_Load

            If _refreshButton IsNot Nothing Then
                RemoveHandler _refreshButton.Click, AddressOf RefreshButton_Click
            End If

            If _headerPanel IsNot Nothing Then
                RemoveHandler _headerPanel.Resize, AddressOf HeaderPanel_Resize
            End If

            DisposeTimer(
                _fallbackPollTimer,
                AddressOf FallbackPollTimer_Tick
            )
            DisposeTimer(
                _reconnectTimer,
                AddressOf ReconnectTimer_Tick
            )
            DisposeTimer(
                _realtimeDebounceTimer,
                AddressOf RealtimeDebounceTimer_Tick
            )

            TeardownRealtimeSubscription()

        End Sub

        Private Sub DisposeTimer(
            ByRef targetTimer As Timer,
            handler As EventHandler
        )

            If targetTimer Is Nothing Then Return

            Try
                targetTimer.Stop()
                RemoveHandler targetTimer.Tick, handler
                targetTimer.Dispose()
            Catch
            Finally
                targetTimer = Nothing
            End Try

        End Sub

        Private Sub StopLiveUpdateTimers()

            If _fallbackPollTimer IsNot Nothing Then
                _fallbackPollTimer.Stop()
            End If
            If _reconnectTimer IsNot Nothing Then
                _reconnectTimer.Stop()
            End If
            If _realtimeDebounceTimer IsNot Nothing Then
                _realtimeDebounceTimer.Stop()
            End If

        End Sub

        Private Sub TeardownRealtimeSubscription()

            Dim existing As BrewOrdersRealtimeSubscription = _realtimeSubscription
            _realtimeSubscription = Nothing

            If existing IsNot Nothing Then
                Try
                    existing.Dispose()
                Catch
                End Try
            End If

        End Sub

        Private Sub StartFallbackPollingIfNeeded()

            If _isDisposed OrElse Not Me.Visible Then Return
            If Not SupabaseClientProvider.IsConfigured Then Return
            If _realtimeState = BrewRealtimeConnectionState.Connected Then Return

            If _fallbackPollTimer IsNot Nothing AndAlso Not _fallbackPollTimer.Enabled Then
                _fallbackPollTimer.Start()
            End If

            If _reconnectTimer IsNot Nothing Then
                If _realtimeUnavailableOnServer Then
                    _reconnectTimer.Stop()
                ElseIf Not _reconnectTimer.Enabled AndAlso Not _realtimeConnectInFlight Then
                    _reconnectTimer.Start()
                End If
            End If

            UpdateLastUpdatedLabel()

        End Sub

        Private Async Function EnsureRealtimeSubscriptionAsync() As Task

            If _isDisposed OrElse Not Me.Visible Then Return
            If _realtimeConnectInFlight Then Return

            If _realtimeSubscription IsNot Nothing AndAlso
               Not _realtimeSubscription.IsDisposed AndAlso
               _realtimeSubscription.IsJoined Then
                _realtimeState = BrewRealtimeConnectionState.Connected
                StopFallbackAndReconnectTimers()
                UpdateLastUpdatedLabel()
                Return
            End If

            If Not SupabaseClientProvider.IsConfigured Then
                _realtimeState = BrewRealtimeConnectionState.Disconnected
                StopLiveUpdateTimers()
                UpdateLastUpdatedLabel()
                Return
            End If

            If _realtimeUnavailableOnServer Then
                StartFallbackPollingIfNeeded()
                Return
            End If

            TeardownRealtimeSubscription()

            Dim generation As Integer = _realtimeGeneration
            Dim subscription As BrewOrdersRealtimeSubscription = Nothing
            Dim subscribeError As Exception = Nothing
            _realtimeConnectInFlight = True
            _realtimeState = BrewRealtimeConnectionState.Connecting
            UpdateLastUpdatedLabel()

            Try
                subscription =
                    Await BrewTrackerService.SubscribeOrdersAsync(
                        AddressOf HandleRealtimeOrderChanged,
                        AddressOf HandleRealtimeStateChanged
                    )
            Catch ex As Exception
                subscribeError = ex
            End Try

            Await EnsureUiThreadAsync()

            _realtimeConnectInFlight = False

            If _isDisposed OrElse Me.InvokeRequired OrElse generation <> _realtimeGeneration Then
                If subscription IsNot Nothing Then
                    Try
                        subscription.Dispose()
                    Catch
                    End Try
                End If
                Return
            End If

            If subscribeError IsNot Nothing Then
                Dim errMsg As String = If(subscribeError.Message, String.Empty)
                If errMsg.IndexOf("Unable to subscribe to changes", StringComparison.OrdinalIgnoreCase) >= 0 OrElse
                   errMsg.IndexOf("Realtime is enabled", StringComparison.OrdinalIgnoreCase) >= 0 Then
                    _realtimeUnavailableOnServer = True
                End If

                _realtimeState = BrewRealtimeConnectionState.[Error]
                _hadRealtimeDisruption = True
                UpdateLastUpdatedLabel()
                StartFallbackPollingIfNeeded()
                Return
            End If

            _realtimeSubscription = subscription
            _realtimeState = BrewRealtimeConnectionState.Connected
            StopFallbackAndReconnectTimers()
            UpdateLastUpdatedLabel()

            If _hadRealtimeDisruption Then
                _hadRealtimeDisruption = False
                ScheduleRealtimeRefresh()
            End If

        End Function

        Private Sub StopFallbackAndReconnectTimers()

            If _fallbackPollTimer IsNot Nothing Then
                _fallbackPollTimer.Stop()
            End If
            If _reconnectTimer IsNot Nothing Then
                _reconnectTimer.Stop()
            End If

        End Sub

        Private Sub HandleRealtimeOrderChanged(
            orderId As String,
            newStatus As String
        )

            MarshalToUi(
                Sub()
                    If _isDisposed Then Return
                    ScheduleRealtimeRefresh()
                End Sub
            )

        End Sub

        Private Sub HandleRealtimeStateChanged(
            state As BrewRealtimeConnectionState,
            detail As String
        )

            MarshalToUi(
                Sub()
                    If _isDisposed Then Return

                    _realtimeState = state

                    Select Case state
                        Case BrewRealtimeConnectionState.Connected
                            StopFallbackAndReconnectTimers()
                            If _hadRealtimeDisruption Then
                                _hadRealtimeDisruption = False
                                ScheduleRealtimeRefresh()
                            End If
                        Case BrewRealtimeConnectionState.Reconnecting,
                             BrewRealtimeConnectionState.Disconnected,
                             BrewRealtimeConnectionState.[Error]
                            _hadRealtimeDisruption = True
                            StartFallbackPollingIfNeeded()
                    End Select

                    UpdateLastUpdatedLabel()
                End Sub
            )

        End Sub

        Private Sub MarshalToUi(action As Action)

            If _isDisposed OrElse Not Me.IsHandleCreated Then Return

            Try
                Me.BeginInvoke(action)
            Catch ex As ObjectDisposedException
            Catch ex As InvalidOperationException
            End Try

        End Sub

        Private Function EnsureUiThreadAsync() As Task

            If _isDisposed OrElse Not Me.IsHandleCreated OrElse Not Me.InvokeRequired Then
                Return Task.CompletedTask
            End If

            ' Omit RunContinuationsAsynchronously so TrySetResult resumes inline on the UI thread.
            Dim tcs As New TaskCompletionSource(Of Boolean)()
            Try
                Me.BeginInvoke(
                    New Action(
                        Sub()
                            tcs.TrySetResult(True)
                        End Sub
                    )
                )
            Catch ex As ObjectDisposedException
                tcs.TrySetResult(False)
            Catch ex As InvalidOperationException
                tcs.TrySetResult(False)
            End Try

            Return tcs.Task

        End Function

        Private Sub ScheduleRealtimeRefresh()

            If _isDisposed OrElse _realtimeDebounceTimer Is Nothing Then Return

            If Not Me.Visible Then
                _hasPendingRefreshWhileHidden = True
                If _realtimeDebounceTimer.Enabled Then
                    _realtimeDebounceTimer.Stop()
                End If
                Return
            End If

            _realtimeDebounceTimer.Stop()
            _realtimeDebounceTimer.Start()

        End Sub

        Private Async Sub RealtimeDebounceTimer_Tick(
            sender As Object,
            e As EventArgs
        )

            If _realtimeDebounceTimer IsNot Nothing Then
                _realtimeDebounceTimer.Stop()
            End If

            If _isDisposed OrElse Not Me.Visible Then Return

            Await RefreshDataAsync(True)

        End Sub

        Private Async Sub FallbackPollTimer_Tick(
            sender As Object,
            e As EventArgs
        )

            If _isDisposed OrElse Not Me.Visible Then Return
            If _realtimeState = BrewRealtimeConnectionState.Connected Then
                StopFallbackAndReconnectTimers()
                UpdateLastUpdatedLabel()
                Return
            End If

            Await RefreshDataAsync(True)

        End Sub

        Private Async Sub ReconnectTimer_Tick(
            sender As Object,
            e As EventArgs
        )

            If _reconnectTimer IsNot Nothing Then
                _reconnectTimer.Stop()
            End If

            If _isDisposed OrElse Not Me.Visible Then Return
            If _realtimeState = BrewRealtimeConnectionState.Connected Then Return

            Await EnsureRealtimeSubscriptionAsync()

        End Sub

        Private Sub UpdateLastUpdatedLabel()

            If _isDisposed OrElse _lastUpdatedLabel Is Nothing Then Return

            Dim modeText As String
            If _realtimeState = BrewRealtimeConnectionState.Connected Then
                modeText = "Live"
            ElseIf _realtimeState = BrewRealtimeConnectionState.Connecting OrElse
                   _realtimeState = BrewRealtimeConnectionState.Reconnecting Then
                modeText = "Menghubungkan"
            ElseIf _fallbackPollTimer IsNot Nothing AndAlso _fallbackPollTimer.Enabled Then
                modeText = "Auto 25d"
            Else
                modeText = "Manual"
            End If

            Dim timeText As String = If(
                _lastUpdatedAt.HasValue,
                _lastUpdatedAt.Value.ToString("HH:mm", CultureInfo.InvariantCulture),
                "Belum diperbarui"
            )

            _lastUpdatedLabel.Text = modeText & " | " & timeText
            PositionHeaderStatusLabel()

        End Sub

        Private Sub PositionHeaderStatusLabel()

            If _isDisposed OrElse _headerPanel Is Nothing OrElse _lastUpdatedLabel Is Nothing OrElse _subtitleLabel Is Nothing Then Return

            Dim targetLeft As Integer = _headerPanel.ClientSize.Width - 20 - _lastUpdatedLabel.Width
            Dim minLeft As Integer = _subtitleLabel.Right + 16
            _lastUpdatedLabel.Left = Math.Max(minLeft, targetLeft)

        End Sub

        Private Sub ApplyViewMode()

            If _isDisposed Then Return

            Dim staffMode As Boolean = AppSession.IsStaff

            _subtitleLabel.Text =
                If(
                    staffMode,
                    "Monitor antrean pesanan dan update proses pembuatan.",
                    "Pantau pesanan kamu sampai siap dinikmati."
                )

            _staffSplitContainer.Visible = staffMode
            _customerPanel.Visible = Not staffMode

            If staffMode Then
                _staffSplitContainer.Dock = DockStyle.Fill
            Else
                _customerPanel.Dock = DockStyle.Fill
            End If

        End Sub

        ' =========================================================
        ' DATA LOADING
        ' =========================================================

        Private Async Function RefreshDataAsync(Optional isBackground As Boolean = False) As Task

            If _isDisposed Then Return

            If _isLoading Then
                If isBackground Then
                    _pendingBackgroundRefresh = True
                End If
                Return
            End If

            _isLoading = True
            _pendingBackgroundRefresh = False
            _loadVersion += 1

            Dim currentLoadVersion As Integer = _loadVersion
            Dim loadResolved As Boolean = False

            Try

                If Not isBackground OrElse _orders.Count = 0 Then
                    _refreshButton.Enabled = False
                    _refreshButton.Text = "Refreshing..."

                    ShowState(
                        "Mengambil data pesanan...",
                        True,
                        _warningTint,
                        _warningText
                    )
                End If

                Dim result As List(Of BrewOrder) = Nothing
                Dim fetchError As Exception = Nothing

                Try
                    result = Await BrewTrackerService.GetOrdersAsync()
                Catch ex As Exception
                    fetchError = ex
                    System.Diagnostics.Debug.WriteLine("[BrewTracker] GetOrdersAsync gagal: " & ex.ToString())
                End Try

                Await EnsureUiThreadAsync()

                If _isDisposed Then
                    Return
                End If

                If Me.InvokeRequired OrElse currentLoadVersion <> _loadVersion Then
                    Return
                End If

                If fetchError IsNot Nothing Then
                    ShowState(
                        "Data pesanan gagal dimuat. Coba refresh kembali.",
                        False,
                        _errorTint,
                        _errorText
                    )

                    If Not isBackground OrElse _orders.Count = 0 Then
                        If AppSession.IsStaff Then

                            ShowPanelMessage(
                                _queueFlowPanel,
                                "Gagal memuat antrean",
                                "Silakan tekan tombol Refresh."
                            )

                            ShowPanelMessage(
                                _detailContentPanel,
                                "Data belum tersedia",
                                "Silakan refresh untuk mencoba kembali."
                            )

                        Else

                            ShowPanelMessage(
                                _customerContentPanel,
                                "Data belum tersedia",
                                "Silakan refresh untuk mencoba kembali."
                            )

                        End If
                    End If

                    loadResolved = True
                    Return
                End If

                _orders = If(
                    result,
                    New List(Of BrewOrder)()
                )

                _orders = _orders.
                    OrderByDescending(
                        Function(order)
                            If order.CreatedAt.HasValue Then
                                Return order.CreatedAt.Value
                            End If

                            Return DateTimeOffset.MinValue
                        End Function
                    ).
                    ToList()

                _lastUpdatedAt = DateTime.Now
                UpdateLastUpdatedLabel()

                HideState()
                loadResolved = True

                If AppSession.IsStaff Then

                    RenderStaffQueue()

                    Dim selectedOrder As BrewOrder = Nothing

                    If Not String.IsNullOrWhiteSpace(_selectedOrderId) Then

                        selectedOrder =
                            _orders.FirstOrDefault(
                                Function(order)
                                    Return order.Id = _selectedOrderId
                                End Function
                            )

                    End If

                    If selectedOrder Is Nothing AndAlso _orders.Count > 0 Then
                        selectedOrder = _orders.First()
                    End If

                    If selectedOrder IsNot Nothing Then

                        _selectedOrderId = selectedOrder.Id

                        Await RenderOrderDetailAsync(
                            selectedOrder,
                            _detailContentPanel,
                            False
                        )

                    Else

                        _selectedOrderId = String.Empty

                        ShowPanelMessage(
                            _detailContentPanel,
                            "Belum ada pesanan",
                            "Pesanan yang masuk akan muncul di sini."
                        )

                    End If

                Else

                    _selectedOrderId = String.Empty

                    Dim activeCustomerOrder As BrewOrder = Nothing
                    If Not String.IsNullOrWhiteSpace(_customerDetailOrderId) Then
                        activeCustomerOrder = _orders.FirstOrDefault(
                            Function(order)
                                Return order.Id = _customerDetailOrderId
                            End Function
                        )
                    End If

                    If activeCustomerOrder IsNot Nothing Then
                        Await RenderOrderDetailAsync(
                            activeCustomerOrder,
                            _customerContentPanel,
                            True
                        )
                    Else
                        _customerDetailOrderId = String.Empty
                        RenderCustomerOrders()
                    End If

                End If

                If _orders.Count = 0 Then

                    ShowState(
                        If(
                            AppSession.IsStaff,
                            "Belum ada pesanan yang masuk.",
                            "Kamu belum memiliki pesanan."
                        ),
                        False,
                        _taupeTint,
                        _secondary
                    )

                End If

            Catch ex As Exception
                ' Pastikan recovery di Finally menangani error tak terduga.
                System.Diagnostics.Debug.WriteLine("[BrewTracker] RefreshDataAsync error tak terduga: " & ex.ToString())
                loadResolved = False
            Finally

                If Not _isDisposed Then
                    If Not Me.InvokeRequired Then
                        _isLoading = False
                        _refreshButton.Enabled = True
                        _refreshButton.Text = "Refresh"

                        If _pendingBackgroundRefresh Then
                            _pendingBackgroundRefresh = False
                            MarshalToUi(
                                Async Sub()
                                    Await RefreshDataAsync(True)
                                End Sub
                            )
                        End If
                    Else
                        MarshalToUi(
                            Async Sub()
                                If _isDisposed Then Return
                                _isLoading = False
                                _refreshButton.Enabled = True
                                _refreshButton.Text = "Refresh"
                                If _pendingBackgroundRefresh Then
                                    _pendingBackgroundRefresh = False
                                    Await RefreshDataAsync(True)
                                End If
                            End Sub
                        )
                    End If
                End If

                ' Jaring pengaman: banner "Mengambil data pesanan..." tidak boleh tertinggal.
                If Not _isDisposed AndAlso Not loadResolved Then
                    Dim unresolvedVersion As Integer = currentLoadVersion
                    MarshalToUi(
                        Async Sub()
                            Try
                                Await RecoverUnresolvedLoadState(unresolvedVersion)
                            Catch ex As Exception
                                System.Diagnostics.Debug.WriteLine("[BrewTracker] RecoverUnresolvedLoadState gagal: " & ex.ToString())
                            End Try
                        End Sub
                    )
                End If

            End Try

        End Function

        Private Async Function RecoverUnresolvedLoadState(failedLoadVersion As Integer) As Task

            If _isDisposed Then Return

            If failedLoadVersion = _loadVersion Then
                ShowState(
                    "Data pesanan belum dapat ditampilkan. Tekan Refresh untuk mencoba lagi.",
                    False,
                    _errorTint,
                    _errorText
                )
                Return
            End If

            ' Hasil refresh ini sudah usang. Pastikan ada refresh pengganti yang menyelesaikan banner.
            If Not _isLoading AndAlso Not _pendingBackgroundRefresh Then
                Await RefreshDataAsync(False)
            End If

        End Function

        Private Async Sub RefreshButton_Click(
            sender As Object,
            e As EventArgs
        )

            ' Manual Refresh never creates a duplicate Realtime subscription.
            Await RefreshDataAsync(False)

        End Sub

        ' =========================================================
        ' CUSTOMER VIEW
        ' =========================================================

        Private Sub RenderCustomerOrders()

            If _isDisposed Then Return

            _customerContentPanel.Controls.Clear()

            If _orders.Count = 0 Then

                ShowPanelMessage(
                    _customerContentPanel,
                    "Belum ada pesanan",
                    "Pesanan kamu akan muncul di sini setelah checkout."
                )

                Return

            End If

            Dim listPanel As New FlowLayoutPanel With {
                .Dock = DockStyle.Fill,
                .FlowDirection = FlowDirection.TopDown,
                .WrapContents = False,
                .AutoScroll = True,
                .Padding = New Padding(4, 4, 8, 12),
                .BackColor = _canvas
            }

            _customerContentPanel.Controls.Add(listPanel)

            For Each order As BrewOrder In _orders

                Dim card As Panel = CreateOrderCard(order, False)

                card.Width = Math.Max(
                    400,
                    _customerContentPanel.ClientSize.Width - 35
                )

                card.Margin = New Padding(0, 0, 0, 10)

                Dim capturedOrder As BrewOrder = order

                AddHandler card.Click,
                    Async Sub(sender As Object, e As EventArgs)

                        _customerDetailOrderId = capturedOrder.Id
                        Await RenderOrderDetailAsync(
                            capturedOrder,
                            _customerContentPanel,
                            True
                        )

                    End Sub

                For Each child As Control In card.Controls

                    AddHandler child.Click,
                        Async Sub(sender As Object, e As EventArgs)

                            _customerDetailOrderId = capturedOrder.Id
                            Await RenderOrderDetailAsync(
                                capturedOrder,
                                _customerContentPanel,
                                True
                            )

                        End Sub

                Next

                listPanel.Controls.Add(card)

            Next

        End Sub

        ' =========================================================
        ' STAFF QUEUE
        ' =========================================================

        Private Sub RenderStaffQueue()

            If _isDisposed Then Return

            _queueFlowPanel.Controls.Clear()

            If _orders.Count = 0 Then

                ShowPanelMessage(
                    _queueFlowPanel,
                    "Antrean kosong",
                    "Belum ada pesanan yang perlu diproses."
                )

                Return

            End If

            For Each order As BrewOrder In _orders

                Dim card As Panel = CreateOrderCard(order, True)

                card.Width = Math.Max(
                    260,
                    _queueFlowPanel.ClientSize.Width - 30
                )

                card.Margin = New Padding(0, 0, 0, 8)

                AddHandler card.Click,
                    Async Sub(sender As Object, e As EventArgs)

                        _selectedOrderId = order.Id

                        Await RenderOrderDetailAsync(
                            order,
                            _detailContentPanel,
                            False
                        )

                    End Sub

                For Each child As Control In card.Controls

                    AddHandler child.Click,
                        Async Sub(sender As Object, e As EventArgs)

                            _selectedOrderId = order.Id

                            Await RenderOrderDetailAsync(
                                order,
                                _detailContentPanel,
                                False
                            )

                        End Sub

                Next

                _queueFlowPanel.Controls.Add(card)

            Next

        End Sub

        ' =========================================================
        ' ORDER CARD
        ' =========================================================

        Private Function CreateOrderCard(
            order As BrewOrder,
            staffMode As Boolean
        ) As Panel

            Dim card As New Panel With {
                .Height = If(staffMode, 112, 126),
                .BackColor = _surface,
                .BorderStyle = BorderStyle.None,
                .Cursor = Cursors.Hand
            }

            card.Padding = New Padding(12)

            Dim orderNumber As New Label With {
                .AutoSize = True,
                .Text = If(
                    String.IsNullOrWhiteSpace(order.OrderNumber),
                    "Order #" & ShortId(order.Id),
                    order.OrderNumber
                ),
                .Font = New Font("Segoe UI", 10, FontStyle.Bold),
                .ForeColor = _primary,
                .Location = New Point(12, 10),
                .Cursor = Cursors.Hand
            }

            Dim statusLabel As Label = CreateStatusLabel(order.Status)

            statusLabel.Location = New Point(12, 38)

            Dim totalLabel As New Label With {
                .AutoSize = True,
                .Text = FormatCurrency(order.TotalAmount),
                .Font = New Font("Segoe UI", 9, FontStyle.Bold),
                .ForeColor = _mocha,
                .Location = New Point(12, 68),
                .Cursor = Cursors.Hand
            }

            Dim dateLabel As New Label With {
                .AutoSize = True,
                .Text = FormatDate(order.CreatedAt),
                .Font = New Font("Segoe UI", 8.5F),
                .ForeColor = _secondary,
                .Location = New Point(12, 91),
                .Cursor = Cursors.Hand
            }

            card.Controls.Add(orderNumber)
            card.Controls.Add(statusLabel)
            card.Controls.Add(totalLabel)
            card.Controls.Add(dateLabel)

            If staffMode Then

                Dim customerLabel As New Label With {
                    .AutoSize = True,
                    .Text = "Customer: " & ShortId(order.CustomerId),
                    .Font = New Font("Segoe UI", 8.5F),
                    .ForeColor = _secondary,
                    .Location = New Point(145, 38),
                    .Cursor = Cursors.Hand
                }

                card.Controls.Add(customerLabel)

            End If

            AddHandler card.MouseEnter,
                Sub(sender As Object, e As EventArgs)
                    card.BackColor = Color.FromArgb(250, 247, 242)
                End Sub

            AddHandler card.MouseLeave,
                Sub(sender As Object, e As EventArgs)
                    card.BackColor = _surface
                End Sub

            Return card

        End Function

        ' =========================================================
        ' ORDER DETAIL
        ' =========================================================

        Private Async Function RenderOrderDetailAsync(
            order As BrewOrder,
            targetPanel As Panel,
            customerMode As Boolean
        ) As Task

            If _isDisposed OrElse order Is Nothing Then Return

            _detailVersion += 1

            Dim currentDetailVersion As Integer = _detailVersion

            targetPanel.Controls.Clear()

            Dim detailLayout As New FlowLayoutPanel With {
                .Dock = DockStyle.Fill,
                .FlowDirection = FlowDirection.TopDown,
                .WrapContents = False,
                .AutoScroll = True,
                .Padding = New Padding(12),
                .BackColor = _canvas
            }

            targetPanel.Controls.Add(detailLayout)

            AddHandler detailLayout.SizeChanged,
                Sub(sender As Object, e As EventArgs)
                    FitDetailCardWidths(detailLayout)
                End Sub

            If customerMode Then

                Dim backButton As Button = CreateButton(
                    "←  Kembali ke pesanan",
                    False
                )

                backButton.Margin = New Padding(0, 0, 0, 12)

                AddHandler backButton.Click,
                    Sub(sender As Object, e As EventArgs)
                        _detailVersion += 1
                        _customerDetailOrderId = String.Empty
                        RenderCustomerOrders()
                    End Sub

                detailLayout.Controls.Add(backButton)

            End If

            AddDetailHeader(detailLayout, order, customerMode)

            Dim loadingLabel As New Label With {
                .AutoSize = True,
                .Text = "Memuat item dan riwayat pesanan...",
                .ForeColor = _secondary,
                .Font = New Font("Segoe UI", 9),
                .Margin = New Padding(0, 12, 0, 12)
            }

            detailLayout.Controls.Add(loadingLabel)
            FitDetailCardWidths(detailLayout)

            Dim items As List(Of BrewOrderItem) = Nothing
            Dim history As List(Of BrewOrderStatusHistory) = Nothing
            Dim detailError As Exception = Nothing

            Try
                items = Await BrewTrackerService.GetOrderItemsAsync(order.Id)
                If Not _isDisposed AndAlso currentDetailVersion = _detailVersion Then
                    history = Await BrewTrackerService.GetOrderHistoryAsync(order.Id)
                End If
            Catch ex As Exception
                detailError = ex
            End Try

            Await EnsureUiThreadAsync()

            If _isDisposed OrElse Me.InvokeRequired OrElse currentDetailVersion <> _detailVersion Then
                Return
            End If

            detailLayout.Controls.Remove(loadingLabel)

            If detailError IsNot Nothing Then
                AddSectionTitle(
                    detailLayout,
                    "Detail pesanan"
                )

                AddMessageCard(
                    detailLayout,
                    "Detail gagal dimuat",
                    "Item atau riwayat pesanan gagal diambil. Coba refresh."
                )
                FitDetailCardWidths(detailLayout)
                Return
            End If

            AddItemsSection(detailLayout, items)
            AddPaymentSection(detailLayout, order)
            AddHistorySection(detailLayout, history)

            If AppSession.IsStaff Then

                AddStaffActionSection(
                    detailLayout,
                    order
                )

            End If

            FitDetailCardWidths(detailLayout)

        End Function

        Private Sub FitDetailCardWidths(layout As FlowLayoutPanel)
            If layout Is Nothing OrElse layout.IsDisposed Then Return

            Dim available As Integer =
                layout.ClientSize.Width -
                layout.Padding.Horizontal -
                SystemInformation.VerticalScrollBarWidth

            If available < 260 Then available = 260

            For Each child As Control In layout.Controls
                If TypeOf child Is Panel Then
                    child.Width = available
                End If
            Next
        End Sub

        Private Sub AddDetailHeader(
            target As FlowLayoutPanel,
            order As BrewOrder,
            customerMode As Boolean
        )

            Dim headerCard As New Panel With {
                .BackColor = _surface,
                .Height = 142,
                .Padding = New Padding(14),
                .Margin = New Padding(0, 0, 0, 12)
            }

            Dim orderNumber As New Label With {
                .AutoSize = True,
                .Text = If(
                    String.IsNullOrWhiteSpace(order.OrderNumber),
                    "Order #" & ShortId(order.Id),
                    order.OrderNumber
                ),
                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                .ForeColor = _primary,
                .Location = New Point(14, 12)
            }

            Dim status As Label = CreateStatusLabel(order.Status)

            status.Location = New Point(14, 44)

            Dim created As New Label With {
                .AutoSize = True,
                .Text = "Dibuat " & FormatDate(order.CreatedAt),
                .Font = New Font("Segoe UI", 9),
                .ForeColor = _secondary,
                .Location = New Point(14, 76)
            }

            Dim total As New Label With {
                .AutoSize = True,
                .Text = "Total " & FormatCurrency(order.TotalAmount),
                .Font = New Font("Segoe UI", 10, FontStyle.Bold),
                .ForeColor = _mocha,
                .Location = New Point(14, 101)
            }

            headerCard.Controls.Add(orderNumber)
            headerCard.Controls.Add(status)
            headerCard.Controls.Add(created)
            headerCard.Controls.Add(total)

            target.Controls.Add(headerCard)

        End Sub

        ' =========================================================
        ' ITEMS
        ' =========================================================

        Private Sub AddItemsSection(
            target As FlowLayoutPanel,
            items As List(Of BrewOrderItem)
        )

            AddSectionTitle(
                target,
                "Item pesanan"
            )

            If items Is Nothing OrElse items.Count = 0 Then

                AddMessageCard(
                    target,
                    "Tidak ada item",
                    "Detail item tidak ditemukan untuk order ini."
                )

                Return

            End If

            For Each item As BrewOrderItem In items

                AddItemCard(target, item)

            Next

        End Sub

        Private Sub AddItemCard(
            target As FlowLayoutPanel,
            item As BrewOrderItem
        )

            Dim itemCard As New Panel With {
                .BackColor = _surface,
                .Height = 164,
                .Padding = New Padding(12),
                .Margin = New Padding(0, 0, 0, 8)
            }

            Dim itemName As String =
                If(
                    String.IsNullOrWhiteSpace(item.ItemNameSnapshot),
                    "Menu",
                    item.ItemNameSnapshot
                )

            Dim nameLabel As New Label With {
                .AutoSize = True,
                .Text = itemName,
                .Font = New Font("Segoe UI", 10, FontStyle.Bold),
                .ForeColor = _primary,
                .Location = New Point(12, 10)
            }

            Dim quantityLabel As New Label With {
                .AutoSize = True,
                .Text = "Qty: " & item.Quantity.ToString(),
                .Font = New Font("Segoe UI", 9),
                .ForeColor = _secondary,
                .Location = New Point(12, 38)
            }

            Dim priceLabel As New Label With {
                .AutoSize = True,
                .Text =
                    FormatCurrency(item.UnitBasePrice) &
                    " / item",
                .Font = New Font("Segoe UI", 9),
                .ForeColor = _secondary,
                .Location = New Point(12, 61)
            }

            Dim optionsText As String =
                GetOptionsText(item.OptionsSnapshot)

            Dim optionsLabel As New Label With {
                .AutoSize = False,
                .Text = If(
                    String.IsNullOrWhiteSpace(optionsText),
                    "Opsi: -",
                    "Opsi: " & optionsText
                ),
                .Font = New Font("Segoe UI", 8.5F),
                .ForeColor = _secondary,
                .Location = New Point(12, 84),
                .MaximumSize = New Size(700, 0),
                .Width = 700,
                .Height = 28
            }

            Dim notesText As String =
                If(
                    String.IsNullOrWhiteSpace(item.Notes),
                    "Catatan: -",
                    "Catatan: " & item.Notes
                )

            Dim notesLabel As New Label With {
                .AutoSize = False,
                .Text = notesText,
                .Font = New Font("Segoe UI", 8.5F),
                .ForeColor = _secondary,
                .Location = New Point(12, 112),
                .MaximumSize = New Size(700, 0),
                .Width = 700,
                .Height = 28
            }

            Dim lineTotal As New Label With {
                .AutoSize = True,
                .Text = FormatCurrency(item.LineTotal),
                .Font = New Font("Segoe UI", 9, FontStyle.Bold),
                .ForeColor = _mocha,
                .Location = New Point(12, 142)
            }

            itemCard.Controls.Add(nameLabel)
            itemCard.Controls.Add(quantityLabel)
            itemCard.Controls.Add(priceLabel)
            itemCard.Controls.Add(optionsLabel)
            itemCard.Controls.Add(notesLabel)
            itemCard.Controls.Add(lineTotal)

            target.Controls.Add(itemCard)

        End Sub

        Private Function GetOptionsText(
            options As JsonElement
        ) As String

            Try

                If options.ValueKind = JsonValueKind.Undefined OrElse
                   options.ValueKind = JsonValueKind.Null Then

                    Return String.Empty

                End If

                If options.ValueKind = JsonValueKind.String Then
                    Return options.GetString()
                End If

                If options.ValueKind = JsonValueKind.Object Then

                    Dim parts As New List(Of String)

                    For Each propertyItem As JsonProperty In options.EnumerateObject()

                        Dim value As String =
                            propertyItem.Value.ToString()

                        If Not String.IsNullOrWhiteSpace(value) Then

                            parts.Add(
                                propertyItem.Name & ": " & value
                            )

                        End If

                    Next

                    Return String.Join(", ", parts)

                End If

                If options.ValueKind = JsonValueKind.Array Then

                    Dim parts As New List(Of String)

                    For Each element As JsonElement In options.EnumerateArray()

                        Dim value As String = element.ToString()

                        If Not String.IsNullOrWhiteSpace(value) Then
                            parts.Add(value)
                        End If

                    Next

                    Return String.Join(", ", parts)

                End If

                Return options.ToString()

            Catch

                Return String.Empty

            End Try

        End Function

        ' =========================================================
        ' PAYMENT
        ' =========================================================

        Private Sub AddPaymentSection(
            target As FlowLayoutPanel,
            order As BrewOrder
        )

            AddSectionTitle(
                target,
                "Ringkasan pembayaran"
            )

            Dim paymentCard As New Panel With {
                .BackColor = _surface,
                .Height = 125,
                .Padding = New Padding(12),
                .Margin = New Padding(0, 0, 0, 10)
            }

            AddPaymentRow(
                paymentCard,
                "Subtotal",
                FormatCurrency(order.Subtotal),
                12
            )

            AddPaymentRow(
                paymentCard,
                "Diskon",
                FormatCurrency(order.DiscountAmount),
                38
            )

            AddPaymentRow(
                paymentCard,
                "Metode pembayaran",
                If(
                    String.IsNullOrWhiteSpace(order.PaymentMethod),
                    "-",
                    order.PaymentMethod
                ),
                64
            )

            AddPaymentRow(
                paymentCard,
                "Status pembayaran",
                If(
                    String.IsNullOrWhiteSpace(order.PaymentStatus),
                    "-",
                    order.PaymentStatus
                ),
                90
            )

            target.Controls.Add(paymentCard)

        End Sub

        Private Sub AddPaymentRow(
            panel As Panel,
            labelText As String,
            valueText As String,
            top As Integer
        )

            Dim label As New Label With {
                .AutoSize = True,
                .Text = labelText,
                .Font = New Font("Segoe UI", 8.5F),
                .ForeColor = _secondary,
                .Location = New Point(12, top)
            }

            Dim value As New Label With {
                .AutoSize = True,
                .Text = valueText,
                .Font = New Font("Segoe UI", 8.5F, FontStyle.Bold),
                .ForeColor = _primary,
                .Location = New Point(190, top)
            }

            panel.Controls.Add(label)
            panel.Controls.Add(value)

        End Sub

        ' =========================================================
        ' HISTORY
        ' =========================================================

        Private Sub AddHistorySection(
            target As FlowLayoutPanel,
            history As List(Of BrewOrderStatusHistory)
        )

            AddSectionTitle(
                target,
                "Riwayat status"
            )

            If history Is Nothing OrElse history.Count = 0 Then

                AddMessageCard(
                    target,
                    "Riwayat belum tersedia",
                    "Belum ada perubahan status yang tercatat."
                )

                Return

            End If

            Dim orderedHistory As List(Of BrewOrderStatusHistory) =
                history.
                OrderBy(
                    Function(entry)
                        If entry.ChangedAt.HasValue Then
                            Return entry.ChangedAt.Value
                        End If

                        Return DateTimeOffset.MinValue
                    End Function
                ).
                ToList()

            For Each entry As BrewOrderStatusHistory In orderedHistory

                Dim historyCard As New Panel With {
                    .BackColor = _surface,
                    .Height = 82,
                    .Padding = New Padding(12),
                    .Margin = New Padding(0, 0, 0, 8)
                }

                Dim transitionText As String =
                    If(
                        String.IsNullOrWhiteSpace(entry.PreviousStatus),
                        "Pesanan dibuat → " & DisplayStatus(entry.NewStatus),
                        DisplayStatus(entry.PreviousStatus) &
                        " → " &
                        DisplayStatus(entry.NewStatus)
                    )

                Dim transitionLabel As New Label With {
                    .AutoSize = True,
                    .Text = transitionText,
                    .Font = New Font("Segoe UI", 9, FontStyle.Bold),
                    .ForeColor = _primary,
                    .Location = New Point(12, 10)
                }

                Dim dateLabel As New Label With {
                    .AutoSize = True,
                    .Text = FormatDate(entry.ChangedAt),
                    .Font = New Font("Segoe UI", 8.5F),
                    .ForeColor = _secondary,
                    .Location = New Point(12, 35)
                }

                Dim actorLabel As New Label With {
                    .AutoSize = True,
                    .Text = "Actor: " & ShortId(entry.ChangedBy),
                    .Font = New Font("Segoe UI", 8.5F),
                    .ForeColor = _secondary,
                    .Location = New Point(12, 56)
                }

                historyCard.Controls.Add(transitionLabel)
                historyCard.Controls.Add(dateLabel)
                historyCard.Controls.Add(actorLabel)

                target.Controls.Add(historyCard)

            Next

        End Sub

        ' =========================================================
        ' STAFF ACTION
        ' =========================================================

        Private Sub AddStaffActionSection(
            target As FlowLayoutPanel,
            order As BrewOrder
        )

            AddSectionTitle(
                target,
                "Aksi staff"
            )

            Dim nextStatus As String =
                GetNextStatus(order.Status)

            If String.IsNullOrWhiteSpace(nextStatus) Then

                AddMessageCard(
                    target,
                    "Pesanan selesai",
                    "Order ini sudah berada di tahap Completed."
                )

                Return

            End If

            Dim actionCard As New Panel With {
                .BackColor = _surface,
                .Height = 108,
                .Padding = New Padding(12),
                .Margin = New Padding(0, 0, 0, 10)
            }

            Dim description As New Label With {
                .AutoSize = True,
                .Text =
                    "Status saat ini: " &
                    DisplayStatus(order.Status) &
                    Environment.NewLine &
                    "Tahap berikutnya: " &
                    DisplayStatus(nextStatus),
                .Font = New Font("Segoe UI", 9),
                .ForeColor = _primary,
                .Location = New Point(12, 10)
            }

            Dim updateButton As Button =
                CreateButton(
                    "Update ke " & DisplayStatus(nextStatus),
                    True
                )

            updateButton.Location = New Point(12, 55)
            updateButton.Width = 210
            updateButton.Height = 34

            AddHandler updateButton.Click,
                Async Sub(sender As Object, e As EventArgs)

                    Await UpdateStatusAsync(
                        order,
                        nextStatus,
                        updateButton
                    )

                End Sub

            actionCard.Controls.Add(description)
            actionCard.Controls.Add(updateButton)

            target.Controls.Add(actionCard)

        End Sub

        Private Async Function UpdateStatusAsync(
            order As BrewOrder,
            nextStatus As String,
            actionButton As Button
        ) As Task

            If _isDisposed Then Return

            If Not AppSession.IsStaff Then

                MessageBox.Show(
                    "Hanya staff yang dapat mengubah status pesanan.",
                    "Akses ditolak",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )

                Return

            End If

            If Not BrewTrackerService.IsValidStatusTransition(
                order.Status,
                nextStatus
            ) Then

                MessageBox.Show(
                    "Perubahan status tidak valid.",
                    "Status tidak valid",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )

                Return

            End If

            actionButton.Enabled = False
            actionButton.Text = "Updating..."

            Dim updateError As Exception = Nothing
            Try
                Await BrewTrackerService.UpdateOrderStatusAsync(
                    order.Id,
                    nextStatus
                )
            Catch ex As Exception
                updateError = ex
            End Try

            Await EnsureUiThreadAsync()

            If _isDisposed OrElse Me.InvokeRequired Then Return

            If updateError IsNot Nothing Then
                MessageBox.Show(
                    DescribeUpdateError(updateError),
                    "Gagal update",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )

                actionButton.Enabled = True
                actionButton.Text = "Update ke " & DisplayStatus(nextStatus)

                _selectedOrderId = order.Id

                Await RefreshDataAsync(False)
                Return
            End If

            MessageBox.Show(
                "Status pesanan berhasil diperbarui.",
                "Berhasil",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            )

            _selectedOrderId = order.Id

            Await RefreshDataAsync(False)

        End Function

        Private Shared Function DescribeUpdateError(ex As Exception) As String

            If TypeOf ex Is TimeoutException Then
                Return "Server tidak merespons. Periksa koneksi lalu coba lagi. Data akan dimuat ulang."
            End If

            If TypeOf ex Is InvalidOperationException Then
                Return ex.Message & " Data akan dimuat ulang."
            End If

            If ex.Message.IndexOf("Invalid order status transition", StringComparison.OrdinalIgnoreCase) >= 0 Then
                Return "Status order sudah berubah di sisi lain, jadi perubahan ini ditolak. Data akan dimuat ulang."
            End If

            Return "Status gagal diperbarui. Pastikan koneksi aktif lalu coba lagi. Data akan dimuat ulang."

        End Function

        ' =========================================================
        ' STATUS HELPERS
        ' =========================================================

        Private Function GetNextStatus(
            currentStatus As String
        ) As String

            Select Case NormalizeStatus(currentStatus)

                Case "waiting"
                    Return "Preparing"

                Case "preparing"
                    Return "Ready"

                Case "ready"
                    Return "Completed"

                Case Else
                    Return String.Empty

            End Select

        End Function

        Private Function NormalizeStatus(
            status As String
        ) As String

            If String.IsNullOrWhiteSpace(status) Then
                Return String.Empty
            End If

            Return status.Trim().ToLowerInvariant()

        End Function

        Private Function DisplayStatus(
            status As String
        ) As String

            Select Case NormalizeStatus(status)

                Case "waiting"
                    Return "Waiting"

                Case "preparing"
                    Return "Preparing"

                Case "ready"
                    Return "Ready"

                Case "completed"
                    Return "Completed"

                Case Else
                    Return If(
                        String.IsNullOrWhiteSpace(status),
                        "Unknown",
                        status
                    )

            End Select

        End Function

        Private Function CreateStatusLabel(
            status As String
        ) As Label

            Dim normalized As String =
                NormalizeStatus(status)

            Dim background As Color = _taupeTint
            Dim foreground As Color = _secondary

            Select Case normalized

                Case "waiting"

                    background = _warningTint
                    foreground = _warningText

                Case "preparing"

                    background = _strawberryTint
                    foreground = _strawberry

                Case "ready"

                    background = _sageTint
                    foreground = _sageText

                Case "completed"

                    background = _sageTint
                    foreground = _sageText

            End Select

            Return New Label With {
                .AutoSize = True,
                .Text = DisplayStatus(status),
                .Font = New Font("Segoe UI", 8.5F, FontStyle.Bold),
                .ForeColor = foreground,
                .BackColor = background,
                .Padding = New Padding(7, 4, 7, 4)
            }

        End Function

        ' =========================================================
        ' GENERAL UI HELPERS
        ' =========================================================

        Private Sub AddSectionTitle(
            target As FlowLayoutPanel,
            titleText As String
        )

            Dim titleLabel As New Label With {
                .AutoSize = True,
                .Text = titleText,
                .Font = New Font("Segoe UI", 10, FontStyle.Bold),
                .ForeColor = _primary,
                .Margin = New Padding(0, 10, 0, 8)
            }

            target.Controls.Add(titleLabel)

        End Sub

        Private Sub AddMessageCard(
            target As FlowLayoutPanel,
            titleText As String,
            messageText As String
        )

            Dim card As New Panel With {
                .BackColor = _surface,
                .Height = 88,
                .Padding = New Padding(12),
                .Margin = New Padding(0, 0, 0, 10)
            }

            Dim titleLabel As New Label With {
                .AutoSize = True,
                .Text = titleText,
                .Font = New Font("Segoe UI", 9, FontStyle.Bold),
                .ForeColor = _primary,
                .Location = New Point(12, 12)
            }

            Dim messageLabel As New Label With {
                .AutoSize = True,
                .Text = messageText,
                .Font = New Font("Segoe UI", 8.5F),
                .ForeColor = _secondary,
                .Location = New Point(12, 38),
                .MaximumSize = New Size(700, 0)
            }

            card.Controls.Add(titleLabel)
            card.Controls.Add(messageLabel)

            target.Controls.Add(card)

        End Sub

        Private Sub ShowPanelMessage(
            target As Control,
            titleText As String,
            messageText As String
        )

            target.Controls.Clear()

            Dim container As New Panel With {
                .Dock = DockStyle.Fill,
                .BackColor = _canvas
            }

            Dim titleLabel As New Label With {
                .AutoSize = True,
                .Text = titleText,
                .Font = New Font("Segoe UI", 11, FontStyle.Bold),
                .ForeColor = _primary,
                .Location = New Point(20, 24)
            }

            Dim messageLabel As New Label With {
                .AutoSize = True,
                .Text = messageText,
                .Font = New Font("Segoe UI", 9),
                .ForeColor = _secondary,
                .Location = New Point(20, 58),
                .MaximumSize = New Size(600, 0)
            }

            container.Controls.Add(titleLabel)
            container.Controls.Add(messageLabel)

            target.Controls.Add(container)

        End Sub

        Private Sub ShowState(
            message As String,
            isLoading As Boolean,
            background As Color,
            foreground As Color
        )

            _statePanel.Visible = True
            _statePanel.BackColor = background
            _stateLabel.ForeColor = foreground
            _stateLabel.Text = message

        End Sub

        Private Sub HideState()

            _statePanel.Visible = False

        End Sub

        Private Function CreateButton(
            text As String,
            primaryButton As Boolean
        ) As Button

            Dim button As New Button With {
                .Text = text,
                .FlatStyle = FlatStyle.Flat,
                .Font = New Font("Segoe UI", 9, FontStyle.Bold),
                .Cursor = Cursors.Hand,
                .BackColor = If(primaryButton, _mocha, _surface),
                .ForeColor = If(primaryButton, Color.White, _primary),
                .Height = 34,
                .Width = 150,
                .UseVisualStyleBackColor = False
            }

            button.FlatAppearance.BorderSize = 1

            button.FlatAppearance.BorderColor =
                If(primaryButton, _mocha, _border)

            button.FlatAppearance.MouseOverBackColor =
                If(
                    primaryButton,
                    Color.FromArgb(85, 53, 35),
                    Color.FromArgb(250, 247, 242)
                )

            Return button

        End Function

        ' =========================================================
        ' FORMATTING HELPERS
        ' =========================================================

        Private Function FormatCurrency(
            amount As Decimal
        ) As String

            Return "Rp " &
                amount.ToString(
                    "N0",
                    CultureInfo.GetCultureInfo("id-ID")
                )

        End Function

        Private Function FormatDate(
            value As DateTimeOffset?
        ) As String

            If Not value.HasValue Then
                Return "-"
            End If

            Return value.Value.
                ToLocalTime().
                ToString(
                    "dd MMM yyyy, HH:mm",
                    CultureInfo.GetCultureInfo("id-ID")
                )

        End Function

        Private Function ShortId(
            value As String
        ) As String

            If String.IsNullOrWhiteSpace(value) Then
                Return "-"
            End If

            If value.Length <= 8 Then
                Return value
            End If

            Return value.Substring(0, 8)

        End Function

    End Class

End Namespace