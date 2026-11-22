
Option Strict On
Option Explicit On

Imports System.Text.Json.Serialization
Imports Supabase.Postgrest.Attributes
Imports Supabase.Postgrest.Models

Namespace Models
    <Table("orders")>
    Public Class BrewOrder
        Inherits BaseModel

        <PrimaryKey("id", False)>
        <JsonPropertyName("id")>
        <JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
        Public Property Id As String

        <Column("order_number")>
        <JsonPropertyName("order_number")>
        Public Property OrderNumber As String = String.Empty

        <Column("customer_id")>
        <JsonPropertyName("customer_id")>
        Public Property CustomerId As String

        <Column("room_id")>
        <JsonPropertyName("room_id")>
        Public Property RoomId As String

        <Column("status")>
        <JsonPropertyName("status")>
        Public Property Status As String = "Waiting"

        <Column("payment_method")>
        <JsonPropertyName("payment_method")>
        Public Property PaymentMethod As String = String.Empty

        <Column("payment_status")>
        <JsonPropertyName("payment_status")>
        Public Property PaymentStatus As String = "pending"

        <Column("subtotal")>
        <JsonPropertyName("subtotal")>
        Public Property Subtotal As Decimal

        <Column("discount_amount")>
        <JsonPropertyName("discount_amount")>
        Public Property DiscountAmount As Decimal

        <Column("total_amount")>
        <JsonPropertyName("total_amount")>
        Public Property TotalAmount As Decimal

        <Column("created_at", JsonIgnoreCondition.WhenWritingNull, True, True)>
        <JsonPropertyName("created_at")>
        <JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
        Public Property CreatedAt As DateTimeOffset?

        <Column("updated_at", JsonIgnoreCondition.WhenWritingNull, True, True)>
        <JsonPropertyName("updated_at")>
        <JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
        Public Property UpdatedAt As DateTimeOffset?
    End Class
End Namespace
