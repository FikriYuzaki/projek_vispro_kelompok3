Option Strict On
Option Explicit On

Imports CuppaClub.Models
Imports Supabase.Postgrest

Namespace Services
    Public NotInheritable Class DrinkMenuLoadResult
        Public Property TotalMenuCount As Integer
        Public Property AvailableItems As New List(Of MenuItem)()
    End Class

    Public NotInheritable Class DrinkOptionGroupBucket
        Public Property GroupKey As String = String.Empty
        Public Property DisplayName As String = String.Empty
        Public Property AllowMultiple As Boolean
        Public Property Options As New List(Of MenuOption)()
    End Class

    Public NotInheritable Class DrinkPriceBreakdown
        Public Property UnitBasePrice As Decimal
        Public Property OptionsExtraPrice As Decimal
        Public Property Quantity As Integer

        Public ReadOnly Property UnitTotal As Decimal
            Get
                Return UnitBasePrice + OptionsExtraPrice
            End Get
        End Property

        Public ReadOnly Property LineTotal As Decimal
            Get
                If Quantity <= 0 Then
                    Return 0D
                End If
                Return UnitTotal * Quantity
            End Get
        End Property
    End Class

    Public NotInheritable Class DrinkDraftValidationResult
        Public Property IsValid As Boolean
        Public Property ErrorMessage As String = String.Empty
        Public Property RefreshSuggested As Boolean
        Public Property Draft As ConfiguredDrinkDraft

        Public Shared Function Success(draft As ConfiguredDrinkDraft) As DrinkDraftValidationResult
            Return New DrinkDraftValidationResult With {
                .IsValid = True,
                .Draft = draft
            }
        End Function

        Public Shared Function Fail(message As String, Optional refreshSuggested As Boolean = False) As DrinkDraftValidationResult
            Return New DrinkDraftValidationResult With {
                .IsValid = False,
                .ErrorMessage = message,
                .RefreshSuggested = refreshSuggested
            }
        End Function
    End Class

    Public NotInheritable Class DrinkOptionService
        Public Const MaxNotesLength As Integer = 200
        Public Const MaxQuantity As Integer = 99

        Private ReadOnly _menuService As New MenuService()

        Public Async Function GetAvailableMenuItemsAsync() As Task(Of DrinkMenuLoadResult)
            Dim allItems = Await _menuService.GetMenuItemsAsync()
            Dim available As New List(Of MenuItem)()

            For Each item In allItems
                If item Is Nothing Then Continue For
                If Not item.IsAvailable Then Continue For
                If String.IsNullOrWhiteSpace(item.Id) OrElse String.IsNullOrWhiteSpace(item.Name) Then Continue For
                If item.BasePrice < 0D Then Continue For
                available.Add(item)
            Next

            Return New DrinkMenuLoadResult With {
                .TotalMenuCount = allItems.Count,
                .AvailableItems = available
            }
        End Function

        Public Async Function GetOptionsForMenuItemAsync(menuItemId As String) As Task(Of List(Of MenuOption))
            Dim cleanItemId = If(menuItemId, String.Empty).Trim()
            If String.IsNullOrWhiteSpace(cleanItemId) Then
                Return New List(Of MenuOption)()
            End If

            Dim client = Await SupabaseClientProvider.GetClientAsync()
            ' Use column-name Filter instead of VB LINQ predicates, which emit VB CompareString nodes.
            Dim response = Await client.From(Of MenuOption)().
                Filter("menu_item_id", Constants.Operator.Equals, cleanItemId).
                Filter("is_available", Constants.Operator.Equals, "true").
                Order("option_group", Constants.Ordering.Ascending).
                Order("extra_price", Constants.Ordering.Ascending).
                Order("option_name", Constants.Ordering.Ascending).
                Get()

            If response Is Nothing OrElse response.Models Is Nothing Then
                Return New List(Of MenuOption)()
            End If

            Return SanitizeAvailableOptions(response.Models, cleanItemId)
        End Function

        Public Async Function ValidateAndBuildDraftAsync(
            menuItemId As String,
            selectedOptionIds As IEnumerable(Of String),
            quantity As Integer,
            notes As String
        ) As Task(Of DrinkDraftValidationResult)
            Dim cleanItemId = If(menuItemId, String.Empty).Trim()
            If String.IsNullOrWhiteSpace(cleanItemId) Then
                Return DrinkDraftValidationResult.Fail("Pilih minuman terlebih dahulu sebelum menyiapkan draft pesanan.")
            End If

            If quantity <= 0 Then
                Return DrinkDraftValidationResult.Fail("Kuantitas minimal 1 unit.")
            End If

            If quantity > MaxQuantity Then
                Return DrinkDraftValidationResult.Fail($"Kuantitas maksimal {MaxQuantity} unit per item draft.")
            End If

            Dim freshItem = Await GetMenuItemByIdAsync(cleanItemId)
            If freshItem Is Nothing Then
                Return DrinkDraftValidationResult.Fail(
                    "Item menu ini tidak lagi ditemukan di database. Daftar menu perlu dimuat ulang.",
                    refreshSuggested:=True)
            End If

            If Not freshItem.IsAvailable Then
                Return DrinkDraftValidationResult.Fail(
                    $"Item ""{freshItem.Name}"" saat ini sudah habis dan tidak dapat dipilih.",
                    refreshSuggested:=True)
            End If

            Dim freshOptions As List(Of MenuOption)
            If freshItem.SupportsCustomization Then
                freshOptions = Await GetOptionsForMenuItemAsync(cleanItemId)
            Else
                freshOptions = New List(Of MenuOption)()
            End If

            Return ValidateSelection(freshItem, freshOptions, selectedOptionIds, quantity, notes)
        End Function

        Private Async Function GetMenuItemByIdAsync(menuItemId As String) As Task(Of MenuItem)
            Dim client = Await SupabaseClientProvider.GetClientAsync()
            Dim response = Await client.From(Of MenuItem)().
                Filter("id", Constants.Operator.Equals, menuItemId).
                Get()

            If response Is Nothing OrElse response.Models Is Nothing OrElse response.Models.Count = 0 Then
                Return Nothing
            End If

            Return response.Models(0)
        End Function

        Public Shared Function SanitizeAvailableOptions(
            options As IEnumerable(Of MenuOption),
            expectedMenuItemId As String
        ) As List(Of MenuOption)
            Dim result As New List(Of MenuOption)()
            If options Is Nothing Then
                Return result
            End If

            Dim cleanExpectedId = If(expectedMenuItemId, String.Empty).Trim()
            For Each opt In options
                If opt Is Nothing Then Continue For
                If Not opt.IsAvailable Then Continue For
                If String.IsNullOrWhiteSpace(opt.Id) Then Continue For
                If String.IsNullOrWhiteSpace(opt.OptionName) Then Continue For
                If Not String.IsNullOrEmpty(cleanExpectedId) AndAlso
                   Not String.Equals(If(opt.MenuItemId, String.Empty).Trim(), cleanExpectedId, StringComparison.OrdinalIgnoreCase) Then
                    Continue For
                End If
                If Not MenuOption.IsValidOptionGroup(opt.OptionGroup) Then Continue For
                If opt.ExtraPrice < 0D Then Continue For

                opt.OptionGroup = MenuOption.NormalizeGroupKey(opt.OptionGroup)
                opt.OptionName = opt.OptionName.Trim()
                result.Add(opt)
            Next

            Return result.
                OrderBy(Function(o) MenuOption.GetGroupOrder(o.OptionGroup)).
                ThenBy(Function(o) o.ExtraPrice).
                ThenBy(Function(o) o.OptionName, StringComparer.OrdinalIgnoreCase).
                ToList()
        End Function

        Public Shared Function GroupOptions(options As IEnumerable(Of MenuOption)) As List(Of DrinkOptionGroupBucket)
            Dim buckets As New Dictionary(Of String, DrinkOptionGroupBucket)(StringComparer.Ordinal)

            If options IsNot Nothing Then
                For Each opt In options
                    If opt Is Nothing OrElse Not opt.IsAvailable Then Continue For
                    Dim groupKey = MenuOption.NormalizeGroupKey(opt.OptionGroup)
                    If Not MenuOption.IsValidOptionGroup(groupKey) Then Continue For

                    Dim bucket As DrinkOptionGroupBucket = Nothing
                    If Not buckets.TryGetValue(groupKey, bucket) Then
                        bucket = New DrinkOptionGroupBucket With {
                            .GroupKey = groupKey,
                            .DisplayName = MenuOption.GetGroupDisplayName(groupKey),
                            .AllowMultiple = MenuOption.IsMultiSelectGroup(groupKey)
                        }
                        buckets(groupKey) = bucket
                    End If

                    bucket.Options.Add(opt)
                Next
            End If

            Return buckets.Values.
                OrderBy(Function(b) MenuOption.GetGroupOrder(b.GroupKey)).
                ToList()
        End Function

        Public Shared Function GetDefaultOptionIds(options As IEnumerable(Of MenuOption)) As HashSet(Of String)
            Dim selected As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Dim buckets = GroupOptions(options)

            For Each bucket In buckets
                Dim defaultOptions = bucket.Options.Where(Function(o) o.IsDefault AndAlso Not String.IsNullOrWhiteSpace(o.Id)).ToList()
                If defaultOptions.Count = 0 Then Continue For

                If bucket.AllowMultiple Then
                    For Each opt In defaultOptions
                        selected.Add(opt.Id.Trim())
                    Next
                Else
                    selected.Add(defaultOptions(0).Id.Trim())
                End If
            Next

            Return selected
        End Function

        Public Shared Function CalculatePricing(
            basePrice As Decimal,
            selectedOptions As IEnumerable(Of MenuOption),
            quantity As Integer
        ) As DrinkPriceBreakdown
            If basePrice < 0D Then
                Throw New ArgumentOutOfRangeException(NameOf(basePrice), "Harga dasar tidak boleh negatif.")
            End If
            If quantity <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(quantity), "Kuantitas harus lebih dari nol.")
            End If

            Dim extraTotal As Decimal = 0D
            If selectedOptions IsNot Nothing Then
                For Each opt In selectedOptions
                    If opt Is Nothing Then Continue For
                    If opt.ExtraPrice < 0D Then
                        Throw New ArgumentOutOfRangeException(NameOf(selectedOptions), "Tambahan harga opsi tidak boleh negatif.")
                    End If
                    extraTotal += opt.ExtraPrice
                Next
            End If

            Return New DrinkPriceBreakdown With {
                .UnitBasePrice = basePrice,
                .OptionsExtraPrice = extraTotal,
                .Quantity = quantity
            }
        End Function

        Public Shared Function ValidateSelection(
            item As MenuItem,
            availableOptions As IEnumerable(Of MenuOption),
            selectedOptionIds As IEnumerable(Of String),
            quantity As Integer,
            notes As String
        ) As DrinkDraftValidationResult
            If item Is Nothing OrElse String.IsNullOrWhiteSpace(item.Id) Then
                Return DrinkDraftValidationResult.Fail("Item menu belum dipilih atau tidak valid.")
            End If

            If Not item.IsAvailable Then
                Return DrinkDraftValidationResult.Fail(
                    $"Item ""{item.Name}"" sedang habis dan tidak dapat dimasukkan ke draft.",
                    refreshSuggested:=True)
            End If

            If item.BasePrice < 0D Then
                Return DrinkDraftValidationResult.Fail("Harga dasar item tidak valid (bernilai negatif).")
            End If

            If quantity <= 0 Then
                Return DrinkDraftValidationResult.Fail("Kuantitas harus lebih besar dari 0.")
            End If

            If quantity > MaxQuantity Then
                Return DrinkDraftValidationResult.Fail($"Kuantitas maksimal {MaxQuantity} unit per item draft.")
            End If

            Dim normalizedIds As New List(Of String)()
            Dim seenIds As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            If selectedOptionIds IsNot Nothing Then
                For Each rawId In selectedOptionIds
                    Dim cleanId = If(rawId, String.Empty).Trim()
                    If String.IsNullOrEmpty(cleanId) Then Continue For
                    If seenIds.Add(cleanId) Then
                        normalizedIds.Add(cleanId)
                    End If
                Next
            End If

            If Not item.SupportsCustomization Then
                If normalizedIds.Count > 0 Then
                    Return DrinkDraftValidationResult.Fail(
                        $"Item ""{item.Name}"" tidak mendukung kustomisasi opsi.",
                        refreshSuggested:=True)
                End If

                Dim plainDraft As New ConfiguredDrinkDraft With {
                    .MenuItemId = item.Id.Trim(),
                    .ItemNameSnapshot = If(item.Name, String.Empty).Trim(),
                    .CategorySnapshot = If(item.Category, String.Empty).Trim(),
                    .UnitBasePrice = item.BasePrice,
                    .OptionsSnapshot = New List(Of DrinkOptionSnapshotItem)(),
                    .OptionsExtraPrice = 0D,
                    .Quantity = quantity,
                    .Notes = NormalizeNotes(notes),
                    .CreatedAtUtc = DateTimeOffset.UtcNow
                }
                Return DrinkDraftValidationResult.Success(plainDraft)
            End If

            Dim optionLookup As New Dictionary(Of String, MenuOption)(StringComparer.OrdinalIgnoreCase)
            If availableOptions IsNot Nothing Then
                For Each opt In availableOptions
                    If opt Is Nothing OrElse String.IsNullOrWhiteSpace(opt.Id) Then Continue For
                    optionLookup(opt.Id.Trim()) = opt
                Next
            End If

            Dim resolvedOptions As New List(Of MenuOption)()
            Dim singleSelectGroupsUsed As New HashSet(Of String)(StringComparer.Ordinal)

            For Each optionId In normalizedIds
                Dim matched As MenuOption = Nothing
                If Not optionLookup.TryGetValue(optionId, matched) Then
                    Return DrinkDraftValidationResult.Fail(
                        "Salah satu opsi yang dipilih sudah tidak tersedia di database. Muat ulang opsi untuk memperbarui pilihan.",
                        refreshSuggested:=True)
                End If

                If Not matched.IsAvailable Then
                    Return DrinkDraftValidationResult.Fail(
                        $"Opsi ""{matched.OptionName}"" sedang tidak tersedia.",
                        refreshSuggested:=True)
                End If

                If Not String.Equals(If(matched.MenuItemId, String.Empty).Trim(), item.Id.Trim(), StringComparison.OrdinalIgnoreCase) Then
                    Return DrinkDraftValidationResult.Fail(
                        $"Opsi ""{matched.OptionName}"" tidak sesuai dengan item yang dipilih.",
                        refreshSuggested:=True)
                End If

                Dim groupKey = MenuOption.NormalizeGroupKey(matched.OptionGroup)
                If Not MenuOption.IsValidOptionGroup(groupKey) Then
                    Return DrinkDraftValidationResult.Fail($"Kelompok opsi ""{matched.OptionGroup}"" tidak dikenali.")
                End If

                If matched.ExtraPrice < 0D Then
                    Return DrinkDraftValidationResult.Fail($"Harga tambahan untuk opsi ""{matched.OptionName}"" tidak valid.")
                End If

                If Not MenuOption.IsMultiSelectGroup(groupKey) Then
                    If Not singleSelectGroupsUsed.Add(groupKey) Then
                        Return DrinkDraftValidationResult.Fail(
                            $"Kelompok {MenuOption.GetGroupDisplayName(groupKey)} hanya boleh memilih satu opsi.")
                    End If
                End If

                resolvedOptions.Add(matched)
            Next

            Dim orderedOptions = resolvedOptions.
                OrderBy(Function(o) MenuOption.GetGroupOrder(o.OptionGroup)).
                ThenBy(Function(o) o.ExtraPrice).
                ThenBy(Function(o) o.OptionName, StringComparer.OrdinalIgnoreCase).
                ToList()

            Dim pricing = CalculatePricing(item.BasePrice, orderedOptions, quantity)
            Dim snapshots = orderedOptions.Select(
                Function(o) New DrinkOptionSnapshotItem With {
                    .OptionGroup = MenuOption.NormalizeGroupKey(o.OptionGroup),
                    .OptionId = o.Id.Trim(),
                    .OptionName = o.OptionName.Trim(),
                    .ExtraPrice = o.ExtraPrice
                }).ToList()

            Dim draft As New ConfiguredDrinkDraft With {
                .MenuItemId = item.Id.Trim(),
                .ItemNameSnapshot = If(item.Name, String.Empty).Trim(),
                .CategorySnapshot = If(item.Category, String.Empty).Trim(),
                .UnitBasePrice = pricing.UnitBasePrice,
                .OptionsSnapshot = snapshots,
                .OptionsExtraPrice = pricing.OptionsExtraPrice,
                .Quantity = pricing.Quantity,
                .Notes = NormalizeNotes(notes),
                .CreatedAtUtc = DateTimeOffset.UtcNow
            }

            Return DrinkDraftValidationResult.Success(draft)
        End Function

        Private Shared Function NormalizeNotes(notes As String) As String
            Dim trimmed = If(notes, String.Empty).Trim()
            If trimmed.Length > MaxNotesLength Then
                Return trimmed.Substring(0, MaxNotesLength)
            End If
            Return trimmed
        End Function
    End Class
End Namespace
