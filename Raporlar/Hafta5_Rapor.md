# 📋 Hafta 5 – Haftalık İlerleme Raporu

**Tarih:** 15 Mart 2026  
**Proje:** Bungalov Rezervasyon Sistemi  

---

## 🎯 Bu Hafta Yapılanlar

### 1. Veritabanı Geçişi (PostgreSQL / Supabase)
- Proje **SQLite'tan PostgreSQL (Supabase)** altyapısına başarıyla taşındı.
- `appsettings.json` ve `Program.cs` güncellendi, `Npgsql.EntityFrameworkCore.PostgreSQL` paketi eklendi.
- `InitialPostgres` adıyla yeni bir EF Core Migration oluşturulup Supabase'e (AWS EU-Central) uygulandı.
- `BaseEntity` içerisindeki `CreatedDate` özelliği `DateTime.UtcNow` olarak güncellendi.

### 2. Katmanlı Mimari ve Altyapı Düzenlemesi
- **Generic Repository** (`IGenericRepository`, `GenericRepository`) ve **Unit of Work** pattern'ı uygulandı.
- Generic Repository'ye **Eager Loading (Include)** desteği eklendi — ilişkili veriler (Resimler, Kategoriler) artık otomatik olarak yükleniyor.
- Business katmanında servis arayüzleri (`IBungalowService`, `ICategoryService`, `IReservationService`) ve bunları uygulayan sınıflar oluşturuldu.
- **FluentValidation** ile iş kuralı doğrulamaları (`BungalowValidator`, `CategoryValidator`) eklendi.
- Tüm bağımlılıklar **Dependency Injection** ile `Program.cs` içinde yapılandırıldı.

### 3. Bungalov Yönetim Paneli (CRUD)
- `BungalowController` oluşturuldu:
  - **Listeleme** (Index) — Tüm bungalovları kartlar halinde gösterir.
  - **Ekleme** (Create) — Yeni bungalov ekler, çoklu resim yükleme destekler.
  - **Düzenleme** (Edit) — Mevcut bungalov bilgilerini günceller, yeni resim ekler.
  - **Silme** (Delete) — Bungalovu veritabanından kaldırır.
  - **Tekil Resim Silme** (DeleteImage) — Düzenleme sırasında istenen görseller tek tek silinebilir.
- **Çoklu Resim Yükleme:** `BungalowImage` varlık sınıfı ile her bungalova birden fazla resim yüklenebiliyor. Resimler `wwwroot/images/bungalows/` klasörüne kaydedilir.
- **Bootstrap Carousel (Slider):** Listeleme sayfasında birden fazla resmi olan bungalovlarda oklu slider ile resimler arasında gezilebiliyor.

### 4. Kategori Yönetim Paneli (CRUD)
- `CategoryController` oluşturuldu:
  - **Listeleme** (Index), **Ekleme** (Create), **Düzenleme** (Edit), **Silme** (Delete).
  - Tekli resim yükleme destekli (kapak resmi). Resimler `wwwroot/images/categories/` klasörüne kaydedilir.
- Layout navigasyon menüsüne **"Kategoriler"** linki eklendi.

### 5. Kullanıcı Arayüzü (UI)
- **Bootstrap 5** ve **Bootstrap Icons** ile modern, mobil uyumlu ve şık bir tasarım hazırlandı.
- Form doğrulamaları (`asp-validation`) ile hem istemci hem sunucu tarafında veri kontrolü sağlanıyor.
- Bungalov kartları: kapak resmi, kategori badge'i, fiyat, kapasite, konum, özellikler (Jakuzi/Havuz/WiFi) bilgilerini gösteriyor.

---

## 🗑️ Temizlenen Dosyalar
- `WeatherForecastController.cs` — .NET şablon API controller'ı (kullanılmıyor)
- `DevTestController.cs` — Geliştirme amaçlı test controller'ı (artık gerek yok)
- `launchSettings.json` içindeki `swagger` referansları temizlendi (MVC uygulamasına uygun hale getirildi)

---

## 🏗️ Proje Mimarisi (Güncel Durum)

```
Bungalov/
├── Bungalov.Core/            # Varlık sınıfları (Entity), Arayüzler
│   ├── Varliklar/            # Bungalow, Category, Reservation, BungalowImage, BaseEntity
│   └── Interfaces/           # IGenericRepository, IUnitOfWork
├── Bungalov.DataAccess/      # Veritabanı İşlemleri
│   ├── Baglam/               # AppDbContext (PostgreSQL)
│   ├── Repositories/         # GenericRepository, UnitOfWork
│   └── Migrations/           # EF Core Migration dosyaları
├── Bungalov.Business/        # İş Katmanı
│   ├── Interfaces/           # IBungalowService, ICategoryService, IReservationService
│   ├── Services/             # BungalowService, CategoryService, ReservationService
│   └── Validators/           # BungalowValidator, CategoryValidator
├── Bungalov.WebUI/           # MVC Web Uygulaması
│   ├── Controllers/          # BungalowController, CategoryController
│   ├── Views/                # Razor Views (Bungalow, Category, Shared)
│   └── wwwroot/              # Statik dosyalar (CSS, JS, yüklenen resimler)
├── Raporlar/                 # Haftalık Raporlar
│   └── Hafta5_Rapor.md       # Bu rapor
└── progress_log.md           # Genel ilerleme günlüğü
```

---

## 🛠️ Kullanılan Teknolojiler
| Teknoloji | Sürüm/Detay |
|---|---|
| .NET | 8.0 |
| Entity Framework Core | 8.0.x |
| PostgreSQL | Supabase |
| FluentValidation | ASP.NET Core entegrasyonu ile |
| Bootstrap | 5.x + Bootstrap Icons |
| Mimari Desen | Repository + Unit of Work + MVC |



