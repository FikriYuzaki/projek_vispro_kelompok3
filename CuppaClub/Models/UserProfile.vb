Option Strict On
Option Explicit On

Imports System.Text.Json.Serialization
Imports Supabase.Postgrest.Attributes
Imports Supabase.Postgrest.Models

Namespace Models
    <Table("profiles")>
    Public Class UserProfile
        Inherits BaseModel

        <PrimaryKey("id", False)>
        <JsonPropertyName("id")>
        Public Property Id As String = String.Empty

        <Column("display_name")>
        <JsonPropertyName("display_name")>
        Public Property DisplayName As String = String.Empty

        <Column("email")>
        <JsonPropertyName("email")>
        Public Property Email As String = String.Empty

        <Column("role")>
        <JsonPropertyName("role")>
        Public Property Role As String = "customer"

        <Column("created_at", JsonIgnoreCondition.WhenWritingNull, True, True)>
        <JsonPropertyName("created_at")>
        <JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
        Public Property CreatedAt As DateTimeOffset?

        <JsonIgnore>
        Public ReadOnly Property IsStaff As Boolean
            Get
                Return String.Equals(Role, "staff", StringComparison.OrdinalIgnoreCase) OrElse
                       String.Equals(Role, "admin", StringComparison.OrdinalIgnoreCase)
            End Get
        End Property

        <JsonIgnore>
        Public ReadOnly Property RoleLabel As String
            Get
                If String.Equals(Role, "admin", StringComparison.OrdinalIgnoreCase) Then
                    Return "Admin"
                End If
                If String.Equals(Role, "staff", StringComparison.OrdinalIgnoreCase) Then
                    Return "Staf"
                End If
                Return "Pelanggan"
            End Get
        End Property

        <JsonIgnore>
        Public ReadOnly Property DisplayLabel As String
            Get
                If Not String.IsNullOrWhiteSpace(DisplayName) Then
                    Return DisplayName.Trim()
                End If
                If Not String.IsNullOrWhiteSpace(Email) Then
                    Return Email.Trim()
                End If
                Return "Pengguna"
            End Get
        End Property
    End Class
End Namespace
