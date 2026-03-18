# 📋 Hafta 3 – Admin Paneli CRUD (İçerik Yönetimi)

**Tarih:** Mart 2026  
**Proje:** Bungalov Rezervasyon Sistemi  
**Hedef:** Sisteme veri girişini sağlayacak yönetim panelini kurmak.

---

## 🎯 Bu Hafta Yapılanlar

### 1. Bungalov Yönetimi (CRUD)
`BungalowController` oluşturularak tam CRUD işlemleri sağlandı:

| İşlem | HTTP | Action | Açıklama |
|---|---|---|---|
| Listeleme | GET | `Index` | Tüm bungalovları kartlar halinde listeler |
| Ekleme Formu | GET | `Create` | Yeni bungalov ekleme formu |
| Kaydetme | POST | `Create` | Formdaki veriyi veritabanına kaydeder |
| Düzenleme Formu | GET | `Edit` | Mevcut bungalov bilgilerini düzenleme formu |
| Güncelleme | POST | `Edit` | Güncellenmiş veriyi kaydeder |
| Silme | POST | `Delete` | Bungalovu siler |s
| Resim Silme | POST | `DeleteImage` | Tekil resim silme |

### 2. Kategori Yönetimi (CRUD)
`CategoryController` ile kategorilerin tam yönetimi oluşturuldu:

- **Listeleme (Index):** Tüm kategorileri kapak resimleriyle birlikte gösterir.
- **Ekleme (Create):** Yeni kategori ve kapak resmi yükleme.
- **Düzenleme (Edit):** Mevcut kategori bilgiler ve resmi güncelleme.
- **Silme (Delete):** Kategoriyi kaldırma.

Örnek kategoriler: *Dere Evi, Yayla Evleri, Lüks Bungalovlar* vb.

### 3. Image Upload (Resim Yükleme)
İki farklı resim yükleme sistemi kuruldu:

**Bungalov Görselleri (Çoklu):**
- `IFormFile` ile birden fazla resim aynı anda yüklenebilir.
- Her resim benzersiz GUID ismi ile `wwwroot/images/bungalows/` klasörüne kaydedilir.
- Resim URL'leri `BungalowImages` tablosuna yazılır.
- Düzenle sayfasında her resmin üzerinde kırmızı "X" butonu ile tekil silme yapılabilir.

**Kategori Görseli (Tekli):**
- Tek bir kapak resmi yüklenir.
- `wwwroot/images/categories/` klasörüne kaydedilir.
- URL, `Categories` tablosundaki `ImageUrl` alanına yazılır.

### 4. Kullanıcı Arayüzü (Views)

- **`_Layout.cshtml`:** Bootstrap 5 + Bootstrap Icons ile ana şablon. Üst menüde Bungalovlar ve Kategoriler linkleri.
- **Bungalov Index:** Kartlı listeleme. Bootstrap Carousel (Slider) ile birden fazla resim gezme.
- **Bungalov Create/Edit:** Form floating input'lar, çoklu dosya yükleme, kategori dropdown, özellik switch'leri.
- **Kategori Index/Create/Edit:** Kategori listeleme, ekleme ve düzenleme formları.

---

## 📁 Oluşturulan Dosyalar

| Dosya | Yol |
|---|---|
| `BungalowController.cs` | `Bungalov.WebUI/Controllers/` |
| `CategoryController.cs` | `Bungalov.WebUI/Controllers/` |
| `Index.cshtml` | `Bungalov.WebUI/Views/Bungalow/` |
| `Create.cshtml` | `Bungalov.WebUI/Views/Bungalow/` |
| `Edit.cshtml` | `Bungalov.WebUI/Views/Bungalow/` |
| `Index.cshtml` | `Bungalov.WebUI/Views/Category/` |
| `Create.cshtml` | `Bungalov.WebUI/Views/Category/` |
| `Edit.cshtml` | `Bungalov.WebUI/Views/Category/` |
| `_Layout.cshtml` | `Bungalov.WebUI/Views/Shared/` |
| `BungalowImage.cs` | `Bungalov.Core/Varliklar/` |

---

## ✅ Hafta Sonu Durumu
- [x] Bungalov listeleme, ekleme, silme, güncelleme ekranları
- [x] Kategori yönetimi (CRUD)
- [x] Çoklu Image Upload (Bungalov)
- [x] Tekli Image Upload (Kategori)
- [x] Bootstrap Carousel ile resim slider'ı
- [x] Tekil resim silme özelliği
