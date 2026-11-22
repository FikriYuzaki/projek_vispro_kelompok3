Option Strict On
Option Explicit On

Imports System.Linq
Imports Supabase.Postgrest
Imports Supabase.Postgrest.Models

Namespace Services
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

    End Class
End Namespace