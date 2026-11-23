
Option Strict On
Option Explicit On

Imports System.Text.Json.Serialization
Imports Supabase.Postgrest.Attributes
Imports Supabase.Postgrest.Models

Namespace Models
    <Table("order_status_history")>
    Public Class BrewOrderStatusHistory
        Inherits BaseModel

        <PrimaryKey("id", False)>
        <JsonPropertyName("id")>
        <JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
        Public Property Id As String

        <Column("order_id")>
        <JsonPropertyName("order_id")>
        Public Property OrderId As String

        <Column("previous_status")>
        <JsonPropertyName("previous_status")>
        Public Property PreviousStatus As String

        <Column("new_status")>
        <JsonPropertyName("new_status")>
        Public Property NewStatus As String = String.Empty

        <Column("changed_by")>
        <JsonPropertyName("changed_by")>
        Public Property ChangedBy As String

        <Column("changed_at", JsonIgnoreCondition.WhenWritingNull, True, True)>
        <JsonPropertyName("changed_at")>
        <JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
        Public Property ChangedAt As DateTimeOffset?
    End Class
End Namespace
