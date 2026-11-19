Option Strict On
Option Explicit On

Imports System.IO
Imports CuppaClub.Models
Imports Supabase.Postgrest

Namespace Services
    Public NotInheritable Class MenuService
        Private Const MenuBucketName As String = "menu-photos"
        Private Const MaxPhotoBytes As Long = 2L * 1024L * 1024L

        Private Shared ReadOnly AllowedExtensions As HashSet(Of String) = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
            ".jpg", ".jpeg", ".png", ".webp"
        }

        Public Async Function GetMenuItemsAsync() As Task(Of List(Of MenuItem))
            Dim client = Await SupabaseClientProvider.GetClientAsync()
            Dim response = Await client.From(Of MenuItem)().
                Order("category", Constants.Ordering.Ascending).
                Order("name", Constants.Ordering.Ascending).
                Get()

            If response Is Nothing OrElse response.Models Is Nothing Then
                Return New List(Of MenuItem)()
            End If

            Return response.Models
        End Function

        Public Async Function CreateMenuItemAsync(item As MenuItem) As Task(Of MenuItem)
            ValidateItem(item, requireId:=False)
            Dim client = Await SupabaseClientProvider.GetClientAsync()

            Dim payload As New MenuItem With {
                .Id = Nothing,
                .Name = item.Name.Trim(),
                .Category = item.Category.Trim(),
                .Description = If(item.Description, String.Empty).Trim(),
                .BasePrice = item.BasePrice,
                .ImageUrl = If(item.ImageUrl, String.Empty).Trim(),
                .IsAvailable = item.IsAvailable,
                .SupportsCustomization = item.SupportsCustomization
            }

            Dim response = Await client.From(Of MenuItem)().Insert(payload)
            If response IsNot Nothing AndAlso response.Models IsNot Nothing AndAlso response.Models.Count > 0 Then
                Return response.Models(0)
            End If

            Return payload
        End Function

        Public Async Function UpdateMenuItemAsync(item As MenuItem) As Task(Of MenuItem)
            ValidateItem(item, requireId:=True)
            Dim client = Await SupabaseClientProvider.GetClientAsync()

            Dim payload As New MenuItem With {
                .Id = item.Id.Trim(),
                .Name = item.Name.Trim(),
                .Category = item.Category.Trim(),
                .Description = If(item.Description, String.Empty).Trim(),
                .BasePrice = item.BasePrice,
                .ImageUrl = If(item.ImageUrl, String.Empty).Trim(),
                .IsAvailable = item.IsAvailable,
                .SupportsCustomization = item.SupportsCustomization
            }

            Dim response = Await client.From(Of MenuItem)().
                Filter("id", Constants.Operator.Equals, payload.Id).
                Update(payload)

            If response IsNot Nothing AndAlso response.Models IsNot Nothing AndAlso response.Models.Count > 0 Then
                Return response.Models(0)
            End If

            Return payload
        End Function

        Public Async Function SetAvailabilityAsync(id As String, isAvailable As Boolean) As Task
            If String.IsNullOrWhiteSpace(id) Then
                Throw New ArgumentException("ID item menu tidak valid.", NameOf(id))
            End If

            Dim client = Await SupabaseClientProvider.GetClientAsync()
            Await client.From(Of MenuItem)().
                Filter("id", Constants.Operator.Equals, id.Trim()).
                Set(Function(x) CObj(x.IsAvailable), isAvailable).
                Update()
        End Function

        Public Async Function UploadMenuPhotoAsync(localFilePath As String) As Task(Of String)
            If String.IsNullOrWhiteSpace(localFilePath) OrElse Not File.Exists(localFilePath) Then
                Throw New FileNotFoundException("File gambar tidak ditemukan.", localFilePath)
            End If

            Dim info As New FileInfo(localFilePath)
            If Not AllowedExtensions.Contains(info.Extension) Then
                Throw New InvalidOperationException("Format foto harus JPG, PNG, atau WEBP.")
            End If
            If info.Length = 0 OrElse info.Length > MaxPhotoBytes Then
                Throw New InvalidOperationException("Ukuran foto maksimal 2 MB dan tidak boleh kosong.")
            End If

            Dim client = Await SupabaseClientProvider.GetClientAsync()
            Dim bytes = Await File.ReadAllBytesAsync(localFilePath)
            Dim storagePath = $"menu/{Guid.NewGuid():N}{info.Extension.ToLowerInvariant()}"
            Dim fileOptions As New Supabase.Storage.FileOptions With {
                .ContentType = ResolveContentType(info.Extension),
                .Upsert = True
            }

            Dim bucket = client.Storage.From(MenuBucketName)
            Await bucket.Upload(bytes, storagePath, fileOptions)
            Return bucket.GetPublicUrl(storagePath, Nothing)
        End Function

        Public Shared Function FilterMenuItems(
            items As IEnumerable(Of MenuItem),
            searchQuery As String,
            selectedCategory As String,
            showUnavailable As Boolean
        ) As List(Of MenuItem)
            If items Is Nothing Then
                Return New List(Of MenuItem)()
            End If

            Dim query = If(searchQuery, String.Empty).Trim()
            Dim category = If(selectedCategory, "Semua").Trim()
            Dim allCategories = String.IsNullOrEmpty(category) OrElse
                                String.Equals(category, "Semua", StringComparison.OrdinalIgnoreCase)

            Dim filtered As New List(Of MenuItem)()
            For Each item In items
                If item Is Nothing Then Continue For
                If Not showUnavailable AndAlso Not item.IsAvailable Then Continue For

                If Not allCategories AndAlso Not String.Equals(item.Category, category, StringComparison.OrdinalIgnoreCase) Then
                    Continue For
                End If

                If Not String.IsNullOrEmpty(query) Then
                    Dim inName = Not String.IsNullOrEmpty(item.Name) AndAlso
                                 item.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0
                    Dim inDesc = Not String.IsNullOrEmpty(item.Description) AndAlso
                                 item.Description.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0
                    If Not inName AndAlso Not inDesc Then
                        Continue For
                    End If
                End If

                filtered.Add(item)
            Next

            Return filtered
        End Function

        Private Shared Sub ValidateItem(item As MenuItem, requireId As Boolean)
            If item Is Nothing Then
                Throw New ArgumentNullException(NameOf(item))
            End If
            If requireId AndAlso String.IsNullOrWhiteSpace(item.Id) Then
                Throw New ArgumentException("ID item menu wajib diisi untuk pembaruan.", NameOf(item))
            End If
            If String.IsNullOrWhiteSpace(item.Name) Then
                Throw New ArgumentException("Nama item menu wajib diisi.", NameOf(item))
            End If
            If Not MenuItem.Categories.Any(Function(c) String.Equals(c, item.Category, StringComparison.OrdinalIgnoreCase)) Then
                Throw New ArgumentException("Kategori menu tidak valid.", NameOf(item))
            End If
            If item.BasePrice < 0D Then
                Throw New ArgumentException("Harga dasar tidak boleh negatif.", NameOf(item))
            End If
        End Sub

        Private Shared Function ResolveContentType(extension As String) As String
            Select Case extension.ToLowerInvariant()
                Case ".png"
                    Return "image/png"
                Case ".webp"
                    Return "image/webp"
                Case Else
                    Return "image/jpeg"
            End Select
        End Function
    End Class
End Namespace
