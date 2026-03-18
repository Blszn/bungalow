# 📋 Hafta 2 – Data Katmanı (Persistence) - EF Core & Repository

**Tarih:** Mart 2026  
**Proje:** Bungalov Rezervasyon Sistemi  
**Hedef:** Veritabanı ile konuşacak motoru inşa etmek.

---

## 🎯 Bu Hafta Yapılanlar

### 1. DbContext (AppDbContext)
Entity Framework Core ile veritabanı bağlantısı kuran `AppDbContext` sınıfı oluşturuldu:

```csharp
// Bungalov.DataAccess/Baglam/AppDbContext.cs
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Bungalow> Bungalows { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<BungalowImage> BungalowImages { get; set; }
}
```

**Connection String** ayarı `appsettings.json` dosyasında yapılandırıldı ve PostgreSQL (Supabase) veritabanına bağlanıldı.

### 2. Generic Repository Pattern
Tüm tablolar için ortak CRUD metotlarını sağlayan bir generic repository oluşturuldu:

```csharp
// Bungalov.Core/Interfaces/IGenericRepository.cs
public interface IGenericRepository<T> where T : class
{
    Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetByFilterAsync(Expression<Func<T, bool>> filter, ...);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
```

Implementasyon sınıfı `GenericRepository<T>` olarak yazıldı. **Eager Loading (Include)** desteği ile ilişkili veriler de sorguya dahil edilebiliyor.

### 3. Unit of Work Pattern
Transaction yönetimi için `IUnitOfWork` arayüzü ve `UnitOfWork` sınıfı oluşturuldu:

```csharp
// Bungalov.Core/Interfaces/IUnitOfWork.cs
public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> GetRepository<T>() where T : class;
    Task<int> SaveAsync();
}
```

Bu pattern sayesinde birden fazla veritabanı işlemi tek bir transaction altında yönetilebiliyor.

### 4. EF Core Migrations
İlk veritabanı göçü (migration) yapılarak PostgreSQL üzerinde tablolar oluşturuldu:

| Migration Adı | Tarih | İçerik |
|---|---|---|
| `InitialPostgres` | 2026-03-15 | Bungalows, Categories, Reservations tablolarının oluşturulması |
| `AddBungalowImages` | 2026-03-15 | BungalowImages tablosunun eklenmesi |

---

## 📁 Oluşturulan Dosyalar

| Dosya | Yol |
|---|---|
| `AppDbContext.cs` | `Bungalov.DataAccess/Baglam/` |
| `GenericRepository.cs` | `Bungalov.DataAccess/Repositories/` |
| `UnitOfWork.cs` | `Bungalov.DataAccess/Repositories/` |
| `IGenericRepository.cs` | `Bungalov.Core/Interfaces/` |
| `IUnitOfWork.cs` | `Bungalov.Core/Interfaces/` |
| Migration dosyaları | `Bungalov.DataAccess/Migrations/` |

---

## 🛠️ Kullanılan NuGet Paketleri

| Paket | Katman |
|---|---|
| `Microsoft.EntityFrameworkCore` | DataAccess |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | DataAccess |
| `Microsoft.EntityFrameworkCore.Tools` | DataAccess |

---

## ✅ Hafta Sonu Durumu
- [x] AppDbContext oluşturulması ve Connection String ayarları
- [x] Generic Repository (Add, Update, Delete, GetById, GetAll + Include desteği)
- [x] Unit of Work (Transaction yönetimi)
- [x] Migrations — PostgreSQL (Supabase) üzerinde tabloların oluşması
