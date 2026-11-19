Option Strict On
Option Explicit On

Imports System.IO
Imports System.Text
Imports System.Text.Json

Namespace Core
    Public NotInheritable Class SupabaseSettings
        Public Property SupabaseUrl As String = String.Empty
        Public Property SupabaseAnonKey As String = String.Empty
        Public Property ConfigSource As String = "Belum dikonfigurasi"

        Public ReadOnly Property IsConfigured As Boolean
            Get
                Return Not String.IsNullOrWhiteSpace(SupabaseUrl) AndAlso Not String.IsNullOrWhiteSpace(SupabaseAnonKey)
            End Get
        End Property
    End Class

    Public NotInheritable Class SettingsValidationResult
        Public Property IsValid As Boolean
        Public Property ErrorMessage As String = String.Empty
        Public Property NormalizedUrl As String = String.Empty
        Public Property NormalizedKey As String = String.Empty
    End Class

    Public NotInheritable Class AppConfig
        Private Sub New()
        End Sub

        Private Class ConfigPayload
            Public Property SupabaseUrl As String = String.Empty
            Public Property SupabaseAnonKey As String = String.Empty
        End Class

        Public Shared Function GetLocalUserConfigPath() As String
            Dim localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            Return Path.Combine(localAppData, "CuppaClub", "supabase.json")
        End Function

        Public Shared Function FindWorkspaceLocalConfigPath() As String
            Dim current = New DirectoryInfo(AppContext.BaseDirectory)
            For i = 0 To 6
                If current Is Nothing Then Exit For
                Dim candidate = Path.Combine(current.FullName, "appsettings.local.json")
                If File.Exists(candidate) Then
                    Return candidate
                End If
                current = current.Parent
            Next
            Return String.Empty
        End Function

        Public Shared Function Load() As SupabaseSettings
            Dim userConfigPath = GetLocalUserConfigPath()
            Dim fromUserFile = TryReadJsonConfig(userConfigPath, userConfigPath)
            If fromUserFile IsNot Nothing Then
                Return fromUserFile
            End If

            Dim workspaceConfigPath = FindWorkspaceLocalConfigPath()
            If Not String.IsNullOrEmpty(workspaceConfigPath) Then
                Dim fromWorkspace = TryReadJsonConfig(workspaceConfigPath, "appsettings.local.json")
                If fromWorkspace IsNot Nothing Then
                    Return fromWorkspace
                End If
            End If

            Dim envUrl = FirstNonEmpty(
                Environment.GetEnvironmentVariable("CUPPACLUB_SUPABASE_URL"),
                Environment.GetEnvironmentVariable("SUPABASE_URL"))
            Dim envKey = FirstNonEmpty(
                Environment.GetEnvironmentVariable("CUPPACLUB_SUPABASE_ANON_KEY"),
                Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY"))

            If Not String.IsNullOrWhiteSpace(envUrl) AndAlso Not String.IsNullOrWhiteSpace(envKey) Then
                Dim validation = ValidateSettings(envUrl, envKey)
                If validation.IsValid Then
                    Return New SupabaseSettings With {
                        .SupabaseUrl = validation.NormalizedUrl,
                        .SupabaseAnonKey = validation.NormalizedKey,
                        .ConfigSource = "Environment Variable"
                    }
                End If
            End If

            Return New SupabaseSettings()
        End Function

        Public Shared Function ValidateSettings(rawUrl As String, rawKey As String) As SettingsValidationResult
            Dim url = If(rawUrl, String.Empty).Trim().TrimEnd("/"c)
            Dim key = If(rawKey, String.Empty).Trim()

            If String.IsNullOrWhiteSpace(url) Then
                Return Invalid("URL proyek Supabase wajib diisi (contoh: https://xxxx.supabase.co).")
            End If

            Dim parsedUri As Uri = Nothing
            If Not Uri.TryCreate(url, UriKind.Absolute, parsedUri) Then
                Return Invalid("Format URL Supabase tidak valid.")
            End If

            Dim isHttps = String.Equals(parsedUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
            Dim isLoopbackHttp = String.Equals(parsedUri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) AndAlso parsedUri.IsLoopback
            If Not isHttps AndAlso Not isLoopbackHttp Then
                Return Invalid("URL Supabase harus menggunakan HTTPS (kecuali localhost).")
            End If

            If String.IsNullOrWhiteSpace(key) Then
                Return Invalid("Publishable / Anon Key Supabase wajib diisi.")
            End If

            If key.Any(Function(c) Char.IsWhiteSpace(c)) Then
                Return Invalid("Key Supabase tidak boleh mengandung spasi atau baris baru.")
            End If

            ' Reject privileged keys so desktop clients never ship with RLS-bypass credentials.
            If key.StartsWith("sb_secret_", StringComparison.OrdinalIgnoreCase) Then
                Return Invalid("Kunci sb_secret_ (service_role) ditolak. Gunakan publishable/anon key.")
            End If

            If IsServiceRoleJwt(key) Then
                Return Invalid("Kunci JWT service_role ditolak. Gunakan anon / publishable key.")
            End If

            Return New SettingsValidationResult With {
                .IsValid = True,
                .NormalizedUrl = url,
                .NormalizedKey = key
            }
        End Function

        Public Shared Function SaveLocalSettings(rawUrl As String, rawKey As String) As String
            Dim validation = ValidateSettings(rawUrl, rawKey)
            If Not validation.IsValid Then
                Throw New ArgumentException(validation.ErrorMessage)
            End If

            Dim targetPath = GetLocalUserConfigPath()
            Dim folder = Path.GetDirectoryName(targetPath)
            If Not String.IsNullOrEmpty(folder) Then
                Directory.CreateDirectory(folder)
            End If

            Dim payload = New ConfigPayload With {
                .SupabaseUrl = validation.NormalizedUrl,
                .SupabaseAnonKey = validation.NormalizedKey
            }
            Dim json = JsonSerializer.Serialize(payload, New JsonSerializerOptions With {.WriteIndented = True})
            File.WriteAllText(targetPath, json, Encoding.UTF8)
            Return targetPath
        End Function

        Private Shared Function TryReadJsonConfig(filePath As String, sourceLabel As String) As SupabaseSettings
            If String.IsNullOrWhiteSpace(filePath) OrElse Not File.Exists(filePath) Then
                Return Nothing
            End If

            Try
                Dim json = File.ReadAllText(filePath, Encoding.UTF8)
                Dim payload = JsonSerializer.Deserialize(Of ConfigPayload)(json, New JsonSerializerOptions With {
                    .PropertyNameCaseInsensitive = True
                })
                If payload Is Nothing Then
                    Return Nothing
                End If

                Dim validation = ValidateSettings(payload.SupabaseUrl, payload.SupabaseAnonKey)
                If Not validation.IsValid Then
                    Return Nothing
                End If

                Return New SupabaseSettings With {
                    .SupabaseUrl = validation.NormalizedUrl,
                    .SupabaseAnonKey = validation.NormalizedKey,
                    .ConfigSource = sourceLabel
                }
            Catch
                Return Nothing
            End Try
        End Function

        Private Shared Function IsServiceRoleJwt(token As String) As Boolean
            Dim parts = token.Split("."c)
            If parts.Length <> 3 Then
                Return False
            End If

            Try
                Dim payloadSegment = parts(1).Replace("-"c, "+"c).Replace("_"c, "/"c)
                Dim padding = payloadSegment.Length Mod 4
                If padding > 0 Then
                    payloadSegment &= New String("="c, 4 - padding)
                End If

                Dim rawBytes = Convert.FromBase64String(payloadSegment)
                Using doc = JsonDocument.Parse(rawBytes)
                    Dim roleProp As JsonElement
                    If doc.RootElement.TryGetProperty("role", roleProp) AndAlso roleProp.ValueKind = JsonValueKind.String Then
                        Return String.Equals(roleProp.GetString(), "service_role", StringComparison.OrdinalIgnoreCase)
                    End If
                End Using
            Catch
                Return False
            End Try

            Return False
        End Function

        Private Shared Function FirstNonEmpty(first As String, second As String) As String
            If Not String.IsNullOrWhiteSpace(first) Then
                Return first
            End If
            Return If(second, String.Empty)
        End Function

        Private Shared Function Invalid(message As String) As SettingsValidationResult
            Return New SettingsValidationResult With {
                .IsValid = False,
                .ErrorMessage = message
            }
        End Function
    End Class
End Namespace
