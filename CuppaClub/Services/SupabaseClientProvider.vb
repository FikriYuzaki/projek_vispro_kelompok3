
Option Strict On
Option Explicit On

Imports System.Net
Imports System.Net.Http
Imports System.Threading
Imports CuppaClub.Core

Namespace Services
    Public NotInheritable Class SupabaseNotConfiguredException
        Inherits InvalidOperationException

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
    End Class

    Public NotInheritable Class ConnectionTestResult
        Public Property Success As Boolean
        Public Property Message As String = String.Empty
        Public Property SavedPath As String = String.Empty
    End Class

    Public NotInheritable Class SupabaseClientProvider
        Private Sub New()
        End Sub

        Private Shared ReadOnly InitLock As New SemaphoreSlim(1, 1)

        Private Shared ReadOnly PingClient As New HttpClient With {
            .Timeout = TimeSpan.FromSeconds(8)
        }

        Private Shared _client As Supabase.Client
        Private Shared _cachedUrl As String = String.Empty
        Private Shared _cachedKey As String = String.Empty

        Public Shared Event ConfigurationChanged As EventHandler

        Public Shared ReadOnly Property CurrentSettings As SupabaseSettings
            Get
                Return AppConfig.Load()
            End Get
        End Property

        Public Shared ReadOnly Property IsConfigured As Boolean
            Get
                Return AppConfig.Load().IsConfigured
            End Get
        End Property

        Public Shared Async Function GetClientAsync() As Task(Of Supabase.Client)
            Dim settings = AppConfig.Load()

            If Not settings.IsConfigured Then
                Throw New SupabaseNotConfiguredException(
                    "Koneksi Supabase belum diatur. Buka Koneksi Supabase untuk mengisi URL dan anon/publishable key.")
            End If

            Await InitLock.WaitAsync().ConfigureAwait(False)

            Try
                Dim needsRebuild = _client Is Nothing OrElse
                    Not String.Equals(
                        _cachedUrl,
                        settings.SupabaseUrl,
                        StringComparison.OrdinalIgnoreCase) OrElse
                    Not String.Equals(
                        _cachedKey,
                        settings.SupabaseAnonKey,
                        StringComparison.Ordinal)

                If needsRebuild Then
                    Dim options As New Supabase.SupabaseOptions With {
                        .AutoRefreshToken = True,
                        .AutoConnectRealtime = False
                    }

                    Dim client As New Supabase.Client(
                        settings.SupabaseUrl,
                        settings.SupabaseAnonKey,
                        options)

                    Await client.InitializeAsync().ConfigureAwait(False)

                    _client = client
                    _cachedUrl = settings.SupabaseUrl
                    _cachedKey = settings.SupabaseAnonKey
                End If

                Return _client

            Finally
                InitLock.Release()
            End Try
        End Function

        Public Shared Async Function TestAndSaveAsync(
            rawUrl As String,
            rawKey As String
        ) As Task(Of ConnectionTestResult)

            Dim validation = AppConfig.ValidateSettings(rawUrl, rawKey)

            If Not validation.IsValid Then
                Return New ConnectionTestResult With {
                    .Success = False,
                    .Message = validation.ErrorMessage
                }
            End If

            Try
                ' Gunakan Auth Health endpoint.
                ' Endpoint ini kompatibel dengan sb_publishable_...
                ' Berbeda dengan GET /rest/v1/ yang membutuhkan secret key.

                Dim healthUrl = validation.NormalizedUrl & "/auth/v1/health"

                Using request As New HttpRequestMessage(
                    HttpMethod.Get,
                    healthUrl)

                    request.Headers.Add(
                        "apikey",
                        validation.NormalizedKey)

                    Using response = Await PingClient.SendAsync(
                        request).ConfigureAwait(False)

                        If response.StatusCode = HttpStatusCode.Unauthorized OrElse
                           response.StatusCode = HttpStatusCode.Forbidden Then

                            Return New ConnectionTestResult With {
                                .Success = False,
                                .Message =
                                    $"Server Supabase merespons, tetapi key ditolak (HTTP {CInt(response.StatusCode)}). Periksa Project URL dan publishable/anon key."
                            }
                        End If

                        If Not response.IsSuccessStatusCode Then
                            Return New ConnectionTestResult With {
                                .Success = False,
                                .Message =
                                    $"Supabase Auth Health mengembalikan HTTP {CInt(response.StatusCode)}. Periksa URL proyek dan status layanan Auth."
                            }
                        End If
                    End Using
                End Using

                ' Simpan konfigurasi hanya jika test berhasil.
                Dim savedPath = AppConfig.SaveLocalSettings(
                    validation.NormalizedUrl,
                    validation.NormalizedKey)

                Await ResetClientAsync().ConfigureAwait(False)

                RaiseEvent ConfigurationChanged(
                    Nothing,
                    EventArgs.Empty)

                Return New ConnectionTestResult With {
                    .Success = True,
                    .SavedPath = savedPath,
                    .Message =
                        "Koneksi Supabase terverifikasi dan konfigurasi lokal berhasil disimpan."
                }

            Catch ex As TaskCanceledException
                Return New ConnectionTestResult With {
                    .Success = False,
                    .Message =
                        "Koneksi timeout ke Supabase. Periksa jaringan internet atau URL proyek."
                }

            Catch ex As HttpRequestException
                Return New ConnectionTestResult With {
                    .Success = False,
                    .Message =
                        "Tidak dapat menjangkau server Supabase: " & ex.Message
                }

            Catch ex As Exception
                Return New ConnectionTestResult With {
                    .Success = False,
                    .Message =
                        "Gagal menyimpan konfigurasi Supabase: " & ex.Message
                }
            End Try
        End Function

        Public Shared Async Function ResetClientAsync() As Task
            Await InitLock.WaitAsync().ConfigureAwait(False)

            Try
                _client = Nothing
                _cachedUrl = String.Empty
                _cachedKey = String.Empty

            Finally
                InitLock.Release()
            End Try
        End Function
    End Class
End Namespace
