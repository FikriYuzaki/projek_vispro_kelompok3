Option Strict On
Option Explicit On

Imports CuppaClub.Features

Namespace Core
    Public Enum FeatureStatus
        ComingSoon
        Available
        Disabled
    End Enum

    Public NotInheritable Class FeatureDefinition
        Public Property Id As String = String.Empty
        Public Property DisplayName As String = String.Empty
        Public Property Owner As String = String.Empty
        Public Property Summary As String = String.Empty
        Public Property PlannedItems As String() = Array.Empty(Of String)()
        Public Property Status As FeatureStatus = FeatureStatus.ComingSoon
        Public Property CreateView As Func(Of UserControl)
    End Class

    Public NotInheritable Class FeatureRegistry
        Private Sub New()
        End Sub

        Private Shared ReadOnly FeatureList As New List(Of FeatureDefinition) From {
            New FeatureDefinition With {
                .Id = "menu-gallery",
                .DisplayName = "Café Menu Gallery",
                .Owner = "Fikri",
                .Status = FeatureStatus.Available,
                .CreateView = Function() New MenuGalleryControl(),
                .Summary = "Katalog kopi, non-kopi, pastry, dan dessert dengan foto produk, pencarian, filter kategori, dan pengelolaan ketersediaan oleh staf.",
                .PlannedItems = {
                    "Katalog foto minuman & pastry dari Supabase",
                    "Pencarian dan filter kategori serta status tersedia",
                    "Pengelolaan menu & ketersediaan khusus staf"
                }
            },
            New FeatureDefinition With {
                .Id = "drink-creator",
                .DisplayName = "My Drink Creator",
                .Owner = "Habil",
                .Status = FeatureStatus.Available,
                .CreateView = Function() New DrinkCreatorControl(),
                .Summary = "Kustomisasi minuman berdasarkan opsi menu terstruktur (ukuran, gula, es, susu, topping, extra espresso) dengan perhitungan harga otomatis.",
                .PlannedItems = {
                    "Pemilihan opsi sesuai definisi item menu",
                    "Perhitungan harga dasar + tambahan opsi",
                    "Ringkasan konfigurasi tersimpan sebagai detail pesanan"
                }
            },
            New FeatureDefinition With {
                .Id = "friends-order",
                .DisplayName = "Friends Order Room",
                .Owner = "Nisa",
                .Status = FeatureStatus.ComingSoon,
                .Summary = "Room pemesanan kelompok dengan kode bergabung, keranjang bersama per anggota, aturan penguncian room, dan sinkronisasi langsung.",
                .PlannedItems = {
                    "Pembuatan room dan bergabung via kode room",
                    "Keranjang kelompok dan subtotal per anggota",
                    "Kontrol kunci room sebelum pesanan dikirim"
                }
            },
            New FeatureDefinition With {
                .Id = "brew-tracker",
                .DisplayName = "Live Brew Tracker",
                .Owner = "Nita",
                .Status = FeatureStatus.ComingSoon,
                .Summary = "Pemantauan progres pesanan secara langsung dari Waiting, Preparing, Ready, hingga Completed lengkap dengan riwayat perubahan status.",
                .PlannedItems = {
                    "Daftar pesanan aktif dan detail item",
                    "Validasi transisi status berurutan untuk staf",
                    "Pencatatan waktu dan pelaku setiap perubahan status"
                }
            },
            New FeatureDefinition With {
                .Id = "sweet-rewards",
                .DisplayName = "Sweet Rewards Club",
                .Owner = "Visel",
                .Status = FeatureStatus.ComingSoon,
                .Summary = "Saldo poin loyalitas, katalog reward yang dapat ditukar, validasi saldo/stok, dan ledger riwayat poin yang dapat diaudit.",
                .PlannedItems = {
                    "Saldo poin pelanggan dari transaksi yang memenuhi syarat",
                    "Katalog reward dan penukaran dengan validasi saldo",
                    "Riwayat ledger poin masuk dan keluar"
                }
            }
        }

        Public Shared ReadOnly Property Features As IReadOnlyList(Of FeatureDefinition)
            Get
                Return FeatureList
            End Get
        End Property

        Public Shared Function StatusLabel(status As FeatureStatus) As String
            Select Case status
                Case FeatureStatus.Available
                    Return "Tersedia"
                Case FeatureStatus.Disabled
                    Return "Nonaktif"
                Case Else
                    Return "Coming Soon"
            End Select
        End Function

        Public Shared Function CreateControl(feature As FeatureDefinition) As UserControl
            If feature Is Nothing Then
                Throw New ArgumentNullException(NameOf(feature))
            End If

            If feature.Status = FeatureStatus.Available AndAlso feature.CreateView IsNot Nothing Then
                Return feature.CreateView.Invoke()
            End If

            Return New ComingSoonControl(feature)
        End Function
    End Class
End Namespace
