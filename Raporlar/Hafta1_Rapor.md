# 📋 Hafta 1 – Core Yapı ve Entity (Varlık) Tasarımı

**Tarih:** Mart 2026  
**Proje:** Bungalov Rezervasyon Sistemi  
**Hedef:** Projenin kalbini ve veritabanı şemasını oluşturmak.

---

## 🎯 Bu Hafta Yapılanlar

### 1. Solution Kurulumu (4 Katmanlı Mimari)
Proje **4 katmanlı mimari** ile kuruldu ve her katman bağımsız bir class library olarak yapılandırıldı:

| Katman | Proje Adı | Rolü |
|---|---|---|
| **Core** | `Bungalov.Core` | Entity sınıfları ve arayüzler |
| **DataAccess** | `Bungalov.DataAccess` | Veritabanı erişim katmanı |
| **Business** | `Bungalov.Business` | İş kuralları ve servisler |
| **WebUI** | `Bungalov.WebUI` | MVC Web uygulaması |

Solution dosyası: `Bungalov.slnx`

### 2. BaseEntity Tasarımı
Tüm tablolarda ortak kullanılacak alanlar `BaseEntity` abstract sınıfında tanımlandı:

```csharp
// Bungalov.Core/Varliklar/BaseEntity.cs
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool Status { get; set; } = true;     // Aktif/Pasif
}
```

### 3. Bungalow Entity
Bungalov bilgilerini tutan ana varlık sınıfı oluşturuldu:

```csharp
// Bungalov.Core/Varliklar/Bungalow.cs
public class Bungalow : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public string Location { get; set; }
    public bool HasJacuzzi { get; set; }
    public bool HasPool { get; set; }
    public bool IsWifiAvailable { get; set; }

    // İlişkiler
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public ICollection<Reservation> Reservations { get; set; }
    public ICollection<BungalowImage> Images { get; set; }
}
```

### 4. Category Entity
Bungalovların gruplandığı kategori sınıfı oluşturuldu:

```csharp
// Bungalov.Core/Varliklar/Category.cs
public class Category : BaseEntity
{
    public string CategoryName { get; set; }      // Örn: Lüks Bungalovlar, Dağ Evleri
    public string Description { get; set; }
    public string ImageUrl { get; set; }          // Kategori kapak resmi

    public ICollection<Bungalow> Bungalows { get; set; }
}
```

### 5. Reservation Entity
Rezervasyon bilgilerini tutan sınıf oluşturuldu:

```csharp
// Bungalov.Core/Varliklar/Reservation.cs
public class Reservation : BaseEntity
{
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public int AppUserId { get; set; }
    public int BungalowId { get; set; }
    public Bungalow Bungalow { get; set; }
}
```

---

## 📁 Oluşturulan Dosyalar

| Dosya | Yol |
|---|---|
| `BaseEntity.cs` | `Bungalov.Core/Varliklar/` |
| `Bungalow.cs` | `Bungalov.Core/Varliklar/` |
| `Category.cs` | `Bungalov.Core/Varliklar/` |
| `Reservation.cs` | `Bungalov.Core/Varliklar/` |

---

## 🔗 Entity İlişki Diyagramı

```
Category (1) ──────── (N) Bungalow (1) ──────── (N) Reservation
                                    │
                                    └──── (N) BungalowImage
```

- **Category → Bungalow**: Bir kategoriye birden fazla bungalov ait olabilir (1:N).
- **Bungalow → Reservation**: Bir bungalov birden fazla rezervasyon alabilir (1:N).
- **Bungalow → BungalowImage**: Bir bungalova birden fazla resim yüklenebilir (1:N).

---

## ✅ Hafta Sonu Durumu
- [x] Solution kurulumu (4 katman)
- [x] BaseEntity tasarımı
- [x] Bungalow Entity tasarımı
- [x] Category Entity tasarımı
- [x] Reservation Entity tasarımı
