# 📋 Hafta 4 – Business Logic & FluentValidation (İş Kuralları)

**Tarih:** Mart 2026  
**Proje:** Bungalov Rezervasyon Sistemi  
**Hedef:** Sistemin kurallarını ve doğruluğunu kontrol etmek.

---

## 🎯 Bu Hafta Yapılanlar

### 1. Service Layer (İş Katmanı Servisleri)
Repository'leri çağıran ve iş kurallarını işleten servis sınıfları oluşturuldu:

| Arayüz | Servis Sınıfı | Açıklama |
|---|---|---|
| `IBungalowService` | `BungalowService` | Bungalov CRUD + filtreleme işlemleri |
| `ICategoryService` | `CategoryService` | Kategori CRUD işlemleri |
| `IReservationService` | `ReservationService` | Rezervasyon CRUD işlemleri |

**Örnek servis metotları (IBungalowService):**
```csharp
Task<List<Bungalow>> GetAllBungalowsAsync();
Task<Bungalow?> GetBungalowByIdAsync(int id);
Task<List<Bungalow>> GetBungalowsByFilterAsync(Expression<Func<Bungalow, bool>> filter);
Task AddBungalowAsync(Bungalow bungalow);
Task UpdateBungalowAsync(Bungalow bungalow);
Task DeleteBungalowAsync(int id);
```

Servisler, `IUnitOfWork` üzerinden Generic Repository'yi çağırarak veritabanı işlemlerini gerçekleştirir.

### 2. FluentValidation (İş Kuralı Doğrulamaları)

**BungalowValidator:**
```csharp
RuleFor(x => x.Name).NotEmpty()
    .WithMessage("Bungalov adı boş olamaz.");
RuleFor(x => x.PricePerNight).GreaterThan(0)
    .WithMessage("Bungalov fiyatı 0'dan büyük olmalıdır.");
RuleFor(x => x.Capacity).GreaterThanOrEqualTo(1)
    .WithMessage("Kapasite en az 1 kişi olmalıdır.");
RuleFor(x => x.CategoryId).NotEmpty()
    .WithMessage("Kategori seçimi zorunludur.");
```

**CategoryValidator:**
```csharp
RuleFor(x => x.CategoryName).NotEmpty()
    .WithMessage("Kategori adı boş olamaz.");
```

### 3. Dependency Injection (Bağımlılık Enjeksiyonu)
Tüm servisler `Program.cs` dosyasına Scoped olarak kaydedildi:

```csharp
// Repository & UnitOfWork
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Business Servisleri
builder.Services.AddScoped<IBungalowService, BungalowService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IReservationService, ReservationService>();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<BungalowValidator>();
```

---

## 📁 Oluşturulan Dosyalar

| Dosya | Yol |
|---|---|
| `IBungalowService.cs` | `Bungalov.Business/Interfaces/` |
| `ICategoryService.cs` | `Bungalov.Business/Interfaces/` |
| `IReservationService.cs` | `Bungalov.Business/Interfaces/` |
| `BungalowService.cs` | `Bungalov.Business/Services/` |
| `CategoryService.cs` | `Bungalov.Business/Services/` |
| `ReservationService.cs` | `Bungalov.Business/Services/` |
| `BungalowValidator.cs` | `Bungalov.Business/Validators/` |
| `CategoryValidator.cs` | `Bungalov.Business/Validators/` |

---

## 🛠️ Kullanılan NuGet Paketleri

| Paket | Katman |
|---|---|
| `FluentValidation` | Business |
| `FluentValidation.AspNetCore` | WebUI |

---

## 🔄 Mimari Akış

```
Controller (WebUI)
    ↓ çağırır
Service (Business)          ← FluentValidation ile doğrulanır
    ↓ çağırır
UnitOfWork (DataAccess)
    ↓ çağırır
GenericRepository (DataAccess)
    ↓
AppDbContext → PostgreSQL (Supabase)
```

---

## ✅ Hafta Sonu Durumu
- [x] IBungalowService, ICategoryService, IReservationService arayüzleri
- [x] BungalowService, CategoryService, ReservationService implementasyonları
- [x] BungalowValidator (Fiyat > 0, Kapasite ≥ 1, Ad boş olamaz)
- [x] CategoryValidator (Ad boş olamaz)
- [x] Dependency Injection yapılandırması (Program.cs)
- [x] Proje 0 hata ile derleniyor ✅
