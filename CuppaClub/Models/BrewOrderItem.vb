Option Strict On
Option Explicit On

Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports Supabase.Postgrest.Attributes
Imports Supabase.Postgrest.Models

Namespace Models
    <Table("order_items")>
    Public Class BrewOrderItem
        Inherits BaseModel

        <PrimaryKey("id", False)>
        <JsonPropertyName("id")>
        <JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
        Public Property Id As String

        <Column("order_id")>
        <JsonPropertyName("order_id")>
        Public Property OrderId As String

        <Column("menu_item_id")>
        <JsonPropertyName("menu_item_id")>
        Public Property MenuItemId As String

        <Column("member_user_id")>
        <JsonPropertyName("member_user_id")>
        Public Property MemberUserId As String

        <Column("item_name_snapshot")>
        <JsonPropertyName("item_name_snapshot")>
        Public Property ItemNameSnapshot As String = String.Empty

        <Column("unit_base_price")>
        <JsonPropertyName("unit_base_price")>
        Public Property UnitBasePrice As Decimal

        <Column("options_snapshot")>
        <JsonPropertyName("options_snapshot")>
        Public Property OptionsSnapshot As JsonElement

        <Column("options_extra_price")>
        <JsonPropertyName("options_extra_price")>
        Public Property OptionsExtraPrice As Decimal

        <Column("quantity")>
        <JsonPropertyName("quantity")>
        Public Property Quantity As Integer

        <Column("line_total")>
        <JsonPropertyName("line_total")>
        Public Property LineTotal As Decimal

        <Column("notes")>
        <JsonPropertyName("notes")>
        Public Property Notes As String = String.Empty

        <Column("created_at", JsonIgnoreCondition.WhenWritingNull, True, True)>
        <JsonPropertyName("created_at")>
        <JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
        Public Property CreatedAt As DateTimeOffset?
    End Class
End Namespace