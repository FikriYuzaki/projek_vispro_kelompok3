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

        Private ReadOnly _service As BrewTrackerService

        Private _orders As New List(Of BrewOrder)()
        Private _selectedOrderId As String = String.Empty

        Private _isLoading As Boolean = False
        Private _isDisposed As Boolean = False

        Private _loadVersion As Integer = 0
        Private _detailVersion As Integer = 0

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

            If LicenseManager.UsageMode = LicenseUsageMode.Designtime Then Return

            _service = New BrewTrackerService()

            Me.BackColor = _canvas

            AddHandler _refreshButton.Click, AddressOf RefreshButton_Click
            AddHandler AppSession.SessionChanged, AddressOf SessionChanged
            AddHandler Me.Disposed, AddressOf Control_Disposed

            ApplyViewMode()

            AddHandler Me.Load, AddressOf Control_Load

        End Sub

        ' =========================================================
        ' LIFECYCLE
        ' =========================================================

        Private Async Sub Control_Load(
            sender As Object,
            e As EventArgs
        )

            Await RefreshDataAsync()

        End Sub

        Private Sub SessionChanged(
            sender As Object,
            e As EventArgs
        )

            If _isDisposed Then Return

            ApplyViewMode()

            _orders.Clear()
            _selectedOrderId = String.Empty

            If Me.IsHandleCreated Then
                BeginInvoke(
                    New Action(
                        Async Function()
                            Await RefreshDataAsync()
                        End Function
                    )
                )
            End If

        End Sub

        Private Sub Control_Disposed(
            sender As Object,
            e As EventArgs
        )

            _isDisposed = True

            RemoveHandler AppSession.SessionChanged, AddressOf SessionChanged
            RemoveHandler _refreshButton.Click, AddressOf RefreshButton_Click

            _loadVersion += 1
            _detailVersion += 1

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

        Private Async Function RefreshDataAsync() As Task

            If _isDisposed OrElse _isLoading Then Return

            _isLoading = True
            _loadVersion += 1

            Dim currentLoadVersion As Integer = _loadVersion

            _refreshButton.Enabled = False
            _refreshButton.Text = "Refreshing..."

            ShowState(
                "Mengambil data pesanan...",
                True,
                _warningTint,
                _warningText
            )

            Try

                Dim result As List(Of BrewOrder) =
                    Await _service.GetOrdersAsync()

                If _isDisposed OrElse currentLoadVersion <> _loadVersion Then
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

                _lastUpdatedLabel.Text =
                    "Last updated " &
                    DateTime.Now.ToString(
                        "HH:mm",
                        CultureInfo.InvariantCulture
                    )

                HideState()

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
                    RenderCustomerOrders()

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

                If _isDisposed OrElse currentLoadVersion <> _loadVersion Then
                    Return
                End If

                ShowState(
                    "Data pesanan gagal dimuat. Coba refresh kembali.",
                    False,
                    _errorTint,
                    _errorText
                )

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

            Finally

                If Not _isDisposed AndAlso currentLoadVersion = _loadVersion Then

                    _isLoading = False

                    _refreshButton.Enabled = True
                    _refreshButton.Text = "Refresh"

                End If

            End Try

        End Function

        Private Async Sub RefreshButton_Click(
            sender As Object,
            e As EventArgs
        )

            Await RefreshDataAsync()

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

                AddHandler card.Click,
                    Async Sub(sender As Object, e As EventArgs)

                        Await RenderOrderDetailAsync(
                            order,
                            _customerContentPanel,
                            True
                        )

                    End Sub

                For Each child As Control In card.Controls

                    AddHandler child.Click,
                        Async Sub(sender As Object, e As EventArgs)

                            Await RenderOrderDetailAsync(
                                order,
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

            If customerMode Then

                Dim backButton As Button = CreateButton(
                    "←  Kembali ke pesanan",
                    False
                )

                backButton.Margin = New Padding(0, 0, 0, 12)

                AddHandler backButton.Click,
                    Sub(sender As Object, e As EventArgs)
                        _detailVersion += 1
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

            Try

                Dim items As List(Of BrewOrderItem) =
                    Await _service.GetOrderItemsAsync(order.Id)

                If _isDisposed OrElse currentDetailVersion <> _detailVersion Then
                    Return
                End If

                Dim history As List(Of BrewOrderStatusHistory) =
                    Await _service.GetOrderHistoryAsync(order.Id)

                If _isDisposed OrElse currentDetailVersion <> _detailVersion Then
                    Return
                End If

                detailLayout.Controls.Remove(loadingLabel)

                AddItemsSection(detailLayout, items)
                AddPaymentSection(detailLayout, order)
                AddHistorySection(detailLayout, history)

                If AppSession.IsStaff Then

                    AddStaffActionSection(
                        detailLayout,
                        order
                    )

                End If

            Catch ex As Exception

                If _isDisposed OrElse currentDetailVersion <> _detailVersion Then
                    Return
                End If

                detailLayout.Controls.Remove(loadingLabel)

                AddSectionTitle(
                    detailLayout,
                    "Detail pesanan"
                )

                AddMessageCard(
                    detailLayout,
                    "Detail gagal dimuat",
                    "Item atau riwayat pesanan gagal diambil. Coba refresh."
                )

            End Try

        End Function

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
                .Height = 150,
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
                .Location = New Point(12, 132)
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

            If Not _service.IsValidStatusTransition(
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

            Try

                Await _service.UpdateOrderStatusAsync(
                    order.Id,
                    nextStatus
                )

                If _isDisposed Then Return

                MessageBox.Show(
                    "Status pesanan berhasil diperbarui.",
                    "Berhasil",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                )

                _selectedOrderId = order.Id

                Await RefreshDataAsync()

            Catch ex As Exception

                If _isDisposed Then Return

                MessageBox.Show(
                    "Status gagal diperbarui. Pastikan koneksi aktif dan status masih valid.",
                    "Gagal update",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )

                actionButton.Enabled = True
                actionButton.Text = "Update ke " & DisplayStatus(nextStatus)

            End Try

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