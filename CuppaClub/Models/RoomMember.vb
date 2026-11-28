Option Strict On
Option Explicit On

Imports System.Text.Json.Serialization
Imports Supabase.Postgrest.Attributes
Imports Supabase.Postgrest.Models

Namespace Models
    <Table("room_members")>
    Public Class RoomMember
        Inherits BaseModel

        <PrimaryKey("id", False)>
        <JsonPropertyName("id")>
        <JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
        Public Property Id As String

        <Column("room_id")>
        <JsonPropertyName("room_id")>
        Public Property RoomId As String = String.Empty

        <Column("user_id")>
        <JsonPropertyName("user_id")>
        Public Property UserId As String = String.Empty

        <Column("display_name")>
        <JsonPropertyName("display_name")>
        Public Property DisplayName As String = String.Empty

        <Column("joined_at", JsonIgnoreCondition.WhenWritingNull, True, True)>
        <JsonPropertyName("joined_at")>
        <JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
        Public Property JoinedAt As DateTimeOffset?

        <JsonIgnore>
        Public ReadOnly Property DisplayLabel As String
            Get
                If Not String.IsNullOrWhiteSpace(DisplayName) Then
                    Return DisplayName.Trim()
                End If
                Return "Anggota"
            End Get
        End Property
    End Class
End Namespace

