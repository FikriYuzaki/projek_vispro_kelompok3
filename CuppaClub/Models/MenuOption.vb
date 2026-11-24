Option Strict On
Option Explicit On

Imports System.Text.Json.Serialization
Imports Supabase.Postgrest.Attributes
Imports Supabase.Postgrest.Models

Namespace Models
    <Table("menu_options")>
    Public Class MenuOption
        Inherits BaseModel

        Public Shared ReadOnly ValidOptionGroups As String() = {
            "size",
            "sugar",
            "ice",
            "milk",
            "topping",
            "espresso"
        }

        Private Shared ReadOnly MultiSelectGroups As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
            "topping",
            "espresso"
        }

        <PrimaryKey("id", False)>
        <JsonPropertyName("id")>
        <JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
        Public Property Id As String

        <Column("menu_item_id")>
        <JsonPropertyName("menu_item_id")>
        Public Property MenuItemId As String = String.Empty

        <Column("option_group")>
        <JsonPropertyName("option_group")>
        Public Property OptionGroup As String = String.Empty

        <Column("option_name")>
        <JsonPropertyName("option_name")>
        Public Property OptionName As String = String.Empty

        <Column("extra_price")>
        <JsonPropertyName("extra_price")>
        Public Property ExtraPrice As Decimal

        <Column("is_default")>
        <JsonPropertyName("is_default")>
        Public Property IsDefault As Boolean

        <Column("is_available")>
        <JsonPropertyName("is_available")>
        Public Property IsAvailable As Boolean = True

        <Column("created_at", JsonIgnoreCondition.WhenWritingNull, True, True)>
        <JsonPropertyName("created_at")>
        <JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
        Public Property CreatedAt As DateTimeOffset?

        Public Shared Function NormalizeGroupKey(groupName As String) As String
            Return If(groupName, String.Empty).Trim().ToLowerInvariant()
        End Function

        Public Shared Function IsValidOptionGroup(groupName As String) As Boolean
            Dim normalized = NormalizeGroupKey(groupName)
            If String.IsNullOrEmpty(normalized) Then
                Return False
            End If

            For Each validGroup In ValidOptionGroups
                If String.Equals(validGroup, normalized, StringComparison.Ordinal) Then
                    Return True
                End If
            Next

            Return False
        End Function

        Public Shared Function IsMultiSelectGroup(groupName As String) As Boolean
            Dim normalized = NormalizeGroupKey(groupName)
            Return MultiSelectGroups.Contains(normalized)
        End Function

        Public Shared Function GetGroupOrder(groupName As String) As Integer
            Dim normalized = NormalizeGroupKey(groupName)
            For i = 0 To ValidOptionGroups.Length - 1
                If String.Equals(ValidOptionGroups(i), normalized, StringComparison.Ordinal) Then
                    Return i
                End If
            Next
            Return ValidOptionGroups.Length
        End Function

        Public Shared Function GetGroupDisplayName(groupName As String) As String
            Select Case NormalizeGroupKey(groupName)
                Case "size"
                    Return "Ukuran"
                Case "sugar"
                    Return "Tingkat Gula"
                Case "ice"
                    Return "Tingkat Es"
                Case "milk"
                    Return "Jenis Susu"
                Case "topping"
                    Return "Topping"
                Case "espresso"
                    Return "Extra Espresso"
                Case Else
                    Return If(String.IsNullOrWhiteSpace(groupName), "Opsi Lainnya", groupName.Trim())
            End Select
        End Function
    End Class
End Namespace
