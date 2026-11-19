Option Strict On
Option Explicit On

Imports System.Net.Http
Imports CuppaClub.Core
Imports CuppaClub.Models
Imports Supabase.Gotrue
Imports Supabase.Gotrue.Exceptions
Imports Supabase.Postgrest.Exceptions

Namespace Services
    Public NotInheritable Class AuthResult
        Public Property Success As Boolean
        Public Property Profile As UserProfile
        Public Property RequiresEmailConfirmation As Boolean
        Public Property Message As String = String.Empty
    End Class

    Public NotInheritable Class AuthService
        Public Async Function SignInAsync(email As String, password As String) As Task(Of AuthResult)
            Dim cleanEmail = If(email, String.Empty).Trim()
            If String.IsNullOrWhiteSpace(cleanEmail) OrElse String.IsNullOrEmpty(password) Then
                Return Fail("Email dan kata sandi wajib diisi.")
            End If

            Try
                Dim client = Await SupabaseClientProvider.GetClientAsync()
                Dim session = Await client.Auth.SignIn(cleanEmail, password)
                If session Is Nothing OrElse session.User Is Nothing OrElse String.IsNullOrWhiteSpace(session.User.Id) Then
                    Return Fail("Login tidak mengembalikan sesi pengguna yang valid.")
                End If

                Dim profile = Await LoadOrFallbackProfileAsync(client, session.User, cleanEmail, String.Empty)
                AppSession.SetUser(profile)

                Return New AuthResult With {
                    .Success = True,
                    .Profile = profile,
                    .Message = "Berhasil masuk sebagai " & profile.DisplayLabel & "."
                }
            Catch ex As Exception
                Return Fail(FormatException(ex, "Gagal masuk ke akun."))
            End Try
        End Function

        Public Async Function SignUpAsync(displayName As String, email As String, password As String) As Task(Of AuthResult)
            Dim cleanName = If(displayName, String.Empty).Trim()
            Dim cleanEmail = If(email, String.Empty).Trim()

            If cleanName.Length < 2 Then
                Return Fail("Nama lengkap minimal 2 karakter.")
            End If
            If String.IsNullOrWhiteSpace(cleanEmail) OrElse Not cleanEmail.Contains("@"c) Then
                Return Fail("Masukkan alamat email yang valid.")
            End If
            If String.IsNullOrEmpty(password) OrElse password.Length < 6 Then
                Return Fail("Kata sandi minimal 6 karakter.")
            End If

            Try
                Dim client = Await SupabaseClientProvider.GetClientAsync()
                Dim options As New SignUpOptions With {
                    .Data = New Dictionary(Of String, Object) From {
                        {"display_name", cleanName}
                    }
                }
                Dim signUpState = Await client.Auth.SignUpAsync(
                    Supabase.Gotrue.Constants.SignUpType.Email,
                    cleanEmail,
                    password,
                    options)
                If signUpState Is Nothing OrElse signUpState.User Is Nothing Then
                    Return Fail("Pendaftaran tidak mengembalikan data akun.")
                End If

                Dim session = signUpState.Session
                If session Is Nothing OrElse String.IsNullOrWhiteSpace(session.AccessToken) Then
                    Return New AuthResult With {
                        .Success = True,
                        .RequiresEmailConfirmation = True,
                        .Message = "Akun dibuat. Periksa email Anda untuk tautan konfirmasi sebelum masuk."
                    }
                End If

                Dim profile = Await LoadOrFallbackProfileAsync(client, signUpState.User, cleanEmail, cleanName)
                AppSession.SetUser(profile)

                Return New AuthResult With {
                    .Success = True,
                    .Profile = profile,
                    .Message = "Pendaftaran berhasil. Selamat datang, " & profile.DisplayLabel & "."
                }
            Catch ex As Exception
                Return Fail(FormatException(ex, "Gagal mendaftarkan akun baru."))
            End Try
        End Function

        Public Async Function SignOutAsync() As Task
            Try
                If SupabaseClientProvider.IsConfigured Then
                    Dim client = Await SupabaseClientProvider.GetClientAsync()
                    Await client.Auth.SignOut()
                End If
            Catch
                ' Clear local session even when network sign-out fails so user is not trapped.
            Finally
                AppSession.Clear()
            End Try
        End Function

        Private Shared Async Function LoadOrFallbackProfileAsync(
            client As Supabase.Client,
            user As User,
            fallbackEmail As String,
            fallbackName As String
        ) As Task(Of UserProfile)
            Try
                ' Use column-name Filter instead of VB LINQ predicates, which emit VB CompareString nodes.
                Dim response = Await client.From(Of UserProfile)().
                    Filter("id", Supabase.Postgrest.Constants.Operator.Equals, user.Id).
                    Get()

                If response IsNot Nothing AndAlso response.Models IsNot Nothing AndAlso response.Models.Count > 0 Then
                    Dim loaded = response.Models(0)
                    If String.IsNullOrWhiteSpace(loaded.Email) Then
                        loaded.Email = If(user.Email, fallbackEmail)
                    End If
                    Return loaded
                End If
            Catch
                ' Fall back to customer profile if profiles table is missing or inaccessible.
            End Try

            Dim resolvedName = fallbackName
            If String.IsNullOrWhiteSpace(resolvedName) AndAlso user.UserMetadata IsNot Nothing Then
                Dim metaValue As Object = Nothing
                If user.UserMetadata.TryGetValue("display_name", metaValue) AndAlso metaValue IsNot Nothing Then
                    resolvedName = metaValue.ToString()
                End If
            End If
            If String.IsNullOrWhiteSpace(resolvedName) Then
                Dim addr = If(user.Email, fallbackEmail)
                Dim atIndex = addr.IndexOf("@"c)
                resolvedName = If(atIndex > 0, addr.Substring(0, atIndex), "Pelanggan")
            End If

            Return New UserProfile With {
                .Id = user.Id,
                .DisplayName = resolvedName,
                .Email = If(user.Email, fallbackEmail),
                .Role = "customer"
            }
        End Function

        Public Shared Function FormatException(ex As Exception, defaultMessage As String) As String
            If TypeOf ex Is SupabaseNotConfiguredException Then
                Return ex.Message
            End If

            If TypeOf ex Is GotrueException Then
                Dim msg = ex.Message
                If msg.IndexOf("Invalid login credentials", StringComparison.OrdinalIgnoreCase) >= 0 Then
                    Return "Email atau kata sandi salah."
                End If
                If msg.IndexOf("Email not confirmed", StringComparison.OrdinalIgnoreCase) >= 0 Then
                    Return "Email akun ini belum dikonfirmasi. Periksa inbox email Anda."
                End If
                If msg.IndexOf("User already registered", StringComparison.OrdinalIgnoreCase) >= 0 Then
                    Return "Email sudah terdaftar. Silakan masuk dengan akun yang ada."
                End If
                If msg.IndexOf("Password should be at least", StringComparison.OrdinalIgnoreCase) >= 0 Then
                    Return "Kata sandi tidak memenuhi panjang minimum Supabase Auth."
                End If
                Return defaultMessage & " (" & msg & ")"
            End If

            If TypeOf ex Is PostgrestException Then
                Return "Kesalahan akses database Supabase: " & ex.Message
            End If

            If TypeOf ex Is TaskCanceledException OrElse TypeOf ex Is TimeoutException Then
                Return "Permintaan ke Supabase timeout. Periksa koneksi internet Anda lalu coba lagi."
            End If

            If TypeOf ex Is HttpRequestException Then
                Return "Gagal menghubungi server Supabase: " & ex.Message
            End If

            Return defaultMessage & " (" & ex.Message & ")"
        End Function

        Private Shared Function Fail(message As String) As AuthResult
            Return New AuthResult With {
                .Success = False,
                .Message = message
            }
        End Function
    End Class
End Namespace
