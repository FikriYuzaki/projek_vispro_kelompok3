Option Strict On
Option Explicit On

Imports System.Text.Json.Serialization
Imports Supabase.Postgrest.Attributes
Imports Supabase.Postgrest.Models

Namespace Models
    <Table("menu_items")>
    Public Class MenuItem
        Inherits BaseModel

        Public Shared ReadOnly Categories As String() = {
            "Coffee",
            "Non-Coffee",
            "Pastry",
            "Dessert"
        }

        <PrimaryKey("id", False)>
        <JsonPropertyName("id")>
        <JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
        Public Property Id As String

        <Column("name")>
        <JsonPropertyName("name")>
        Public Property Name As String = String.Empty

        <Column("category")>
        <JsonPropertyName("category")>
        Public Property Category As String = "Coffee"

        <Column("description")>
        <JsonPropertyName("description")>
        Public Property Description As String = String.Empty

        <Column("base_price")>
        <JsonPropertyName("base_price")>
        Public Property BasePrice As Decimal

        <Column("image_url")>
        <JsonPropertyName("image_url")>
        Public Property ImageUrl As String = String.Empty

        <Column("is_available")>
        <JsonPropertyName("is_available")>
        Public Property IsAvailable As Boolean = True

        <Column("supports_customization")>
        <JsonPropertyName("supports_customization")>
        Public Property SupportsCustomization As Boolean

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
