Option Strict On
Option Explicit On

Imports System.Text.Json.Serialization
Imports Supabase.Postgrest.Attributes
Imports Supabase.Postgrest.Models

Namespace Models
    <Table("order_rooms")>
    Public Class OrderRoom
        Inherits BaseModel

        <PrimaryKey("id", False)>
        <JsonPropertyName("id")>
        <JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
        Public Property Id As String

        <Column("room_code")>
        <JsonPropertyName("room_code")>
        Public Property RoomCode As String = String.Empty

        <Column("title")>
        <JsonPropertyName("title")>
        Public Property Title As String = String.Empty

        <Column("host_user_id")>
        <JsonPropertyName("host_user_id")>
        Public Property HostUserId As String = String.Empty

        <Column("status")>
        <JsonPropertyName("status")>
        Public Property Status As String = "open"

        <Column("created_at", JsonIgnoreCondition.WhenWritingNull, True, True)>
        <JsonPropertyName("created_at")>
        <JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
        Public Property CreatedAt As DateTimeOffset?

        <Column("locked_at", JsonIgnoreCondition.WhenWritingNull)>
        <JsonPropertyName("locked_at")>
        <JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
        Public Property LockedAt As DateTimeOffset?

        <Column("submitted_at", JsonIgnoreCondition.WhenWritingNull)>
        <JsonPropertyName("submitted_at")>
        <JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
        Public Property SubmittedAt As DateTimeOffset?

        <JsonIgnore>
        Public ReadOnly Property IsOpen As Boolean
            Get
                Return String.Equals(Status, "open", StringComparison.OrdinalIgnoreCase)
            End Get
        End Property

        <JsonIgnore>
        Public ReadOnly Property IsLocked As Boolean
            Get
                Return String.Equals(Status, "locked", StringComparison.OrdinalIgnoreCase)
            End Get
        End Property

        <JsonIgnore>
        Public ReadOnly Property IsSubmitted As Boolean
            Get
                Return String.Equals(Status, "submitted", StringComparison.OrdinalIgnoreCase)
            End Get
        End Property

        <JsonIgnore>
        Public ReadOnly Property IsCancelled As Boolean
            Get
                Return String.Equals(Status, "cancelled", StringComparison.OrdinalIgnoreCase)
            End Get
        End Property

        <JsonIgnore>
        Public ReadOnly Property StatusDisplayLabel As String
            Get
                Return GetStatusDisplayLabel(Status)
            End Get
        End Property

        Public Function IsHost(userId As String) As Boolean
            If String.IsNullOrWhiteSpace(userId) OrElse String.IsNullOrWhiteSpace(HostUserId) Then
                Return False
            End If
            Return String.Equals(HostUserId.Trim(), userId.Trim(), StringComparison.OrdinalIgnoreCase)
        End Function

        Public Shared Function GetStatusDisplayLabel(statusValue As String) As String
            Select Case If(statusValue, String.Empty).Trim().ToLowerInvariant()
                Case "open"
                    Return "Terbuka"
                Case "locked"
                    Return "Terkunci"
                Case "submitted"
                    Return "Terkirim"
                Case "cancelled"
                    Return "Dibatalkan"
                Case Else
                    Return If(String.IsNullOrWhiteSpace(statusValue), "Tidak diketahui", statusValue)
            End Select
        End Function
    End Class
End Namespace

