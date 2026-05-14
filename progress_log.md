# Proje İlerleme ve Geliştirme Günlüğü (Log)

## [2026-03-15] 1. ile 4. Hafta Arası Temel Yapı ve Veritabanı Kurulumu
- **Veritabanı Geçişi:** Proje SQLite'tan PostgreSQL (Supabase) altyapısına taşındı. `appsettings.json` ve `Program.cs` güncellendi, `Npgsql.EntityFrameworkCore.PostgreSQL` paketi eklendi.
- **Migration Yönetimi:** Supabase üzerinde zaten var olan Categories vb. tabloların silinmemesi için sahte bir `InitialCreate` kaydı EF Core Migration geçmişine eklendi.
- **Katmanlı Mimari (Core & DataAccess):** Generic Repository (`IGenericRepository`, `GenericRepository`) ve Transaction yönetimi için `UnitOfWork` sınıfları oluşturuldu. Ortak alanlar için `BaseEntity` kullanıldı.
- **Business Katmanı (Servisler):** `IBungalowService`, `ICategoryService` ve `IReservationService` arayüzleri oluşturuldu. Bu arayüzleri uygulayan ve `UnitOfWork` üzerinden veritabanı ile konuşan servis sınıfları yazıldı.
- **FluentValidation Kullanımı:** `BungalowValidator` ve `CategoryValidator` sınıfları oluşturularak iş kuralları (örneğin fiyatı 0'dan büyük olmalı) eklendi. `FluentValidation.AspNetCore` paketi kurularak bu validatörler `Program.cs` içine eklendi.
- **Bağımlılık Enjeksiyonu (DI):** Servisler, Unit of Work, Repository ve Veritabanı bağlantı konfigürasyonları WebUI katmanında `Program.cs` içerisine eklendi. Veritabanı iletişiminin ve bağımlılıkların hatasız derlendiği (0 hata ile build) onaylandı.
- **Migration Hata Çözümleri:** Supabase geçişi sırasında yaşanan özellik türü uyumsuzlukları giderildi. `BaseEntity` içerisindeki `CreatedDate` özelliği `DateTime.UtcNow` olarak güncellendi. SQLite'a özel bırakılmış Decimal - Double veri tipi dönüşümü `AppDbContext` içerisinden kaldırılarak PostgreSQL `numeric` tipine uygun hale getirildi. Mevcut EF Core veritabanı şeması tamamen temizlenip baştan `InitialPostgres` migration'ı ile Supabase'e uygulandı.

## [2026-03-15] Bungalov Yönetim Arayüzü (UI) Eklendi
- **BungalowController:** Bungalovları listeleme (`Index`), ekleme (`Create` GET/POST) ve silme (`Delete` POST) işlemleri için `BungalowController` oluşturuldu. `IBungalowService` ve `ICategoryService` kullanılarak veritabanı işlemleri bağlandı. Daha sonra `Edit` (Düzenleme) özelliği eklendi ve hem ekleme hem de düzenleme için **çoklu resim yükleme** (multi-image upload) desteği sağlandı. Resimler `wwwroot/images/bungalows` klasörüne kaydedilmektedir.
- **Kullanıcı Arayüzü (Views):** MVC projesi için temel görünümler eklendi. Bootstrap 5 ve Bootstrap Icons kullanılarak modern, mobil uyumlu ve şık bir tasarım (card yapıları, formlar) hazırlandı. `Views/Shared/_Layout.cshtml` ana şablon olarak ayarlandı, `Views/Bungalow/Index.cshtml` ve form sayfaları (Create/Edit) kodlandı. Listeleme sayfasında bungalovların ilk resminin kapak resmi olarak gösterilmesi sağlandı.
- **Doğrulama (Validation):** İstemci tarafı doğrulaması için `_ValidationScriptsPartial` görünümü eklendi ve FluentValidation ile entegre çalışacak şekilde formlara `asp-validation` etiketleri yerleştirildi. Artık sistem üzerinden manuel olarak Bungalov eklenip, düzenlenip, silinebiliyor.
- **Kategori Yönetimi ve Resim Özelliği:** Kategorileri yönetebilmek için `CategoryController` ve ilgili MVC görünümleri (Index, Create, Edit) eklendi. Kategorilere kapak resmi (image) yükleyebilme özelliği oluşturuldu (resimler `wwwroot/images/categories` içerisine kaydediliyor). `BungalowImage` isminde yeni bir varlık sınıfı (Entity) ve PostgreSQL vertabanı tablosu EF Core Migrations ile sisteme dahil edildi. Layout içerisindeki navigasyon menüsüne "Kategoriler" linki eklendi.

## [2026-03-15] Resim Gösterim Düzeltmeleri ve Yeni Özellikler
- **Eager Loading Eklendi:** `GenericRepository` ve `IGenericRepository`'ye `Include` desteği eklendi. `BungalowService` içinde tüm sorgu metotları artık Category ve Images ilişkili verilerini otomatik olarak yüklüyor.
- **Model Binding Hatası Düzeltildi:** Bungalov oluşturma formundaki dosya yükleme input'unun adı (`name="images"`) Bungalow entity'sindeki `Images` koleksiyonuyla çakışıyordu. `newImages` olarak yeniden adlandırılarak hata giderildi.
- **Bootstrap Carousel (Slider):** Bungalov listeleme sayfasında birden fazla resmi olan bungalovlarda önceki/sonraki okları ve alt göstergeler ile bir slider eklendi. Tek resim varsa düz resim gösterilir.
- **Tekil Resim Silme:** Bungalov düzenleme sayfasında her resmin üzerinde küçük kırmızı "X" silme butonu eklendi. Resimler hem veritabanından hem de fiziksel dosya sisteminden silinir.

## [2026-03-15] Proje Temizliği
- **Silinen Dosyalar:** `WeatherForecast.cs`, `WeatherForecastController.cs` (kullanılmayan .NET şablon dosyaları) ve `DevTestController.cs` (artık gerekli olmayan test controller) silindi.
- **launchSettings.json Temizlendi:** Swagger referansları kaldırıldı, MVC uygulamasına uygun hale getirildi.
- **Haftalık Rapor Sistemi Kuruldu:** `Raporlar/` klasörüne `Hafta5_Rapor.md` oluşturuldu. Her hafta yeni rapor aynı klasöre eklenecek.

## [2026-03-25] Dinamik Listeleme ve Gelişmiş Filtreleme (6. Hafta)
- **Dinamik Filtreleme:** `BungalowController` üzerinde `search`, `categoryId`, `minCapacity`, `hasJacuzzi` ve `hasPool` parametrelerine göre çalışan gelişmiş bir filtreleme sistemi kuruldu. Veriler veritabanı seviyesinde filtrelenmektedir.
- **ViewComponents:** Kategorileri yan menüde listelemek için `CategoryList` ViewComponent'i oluşturuldu. Bu sayede kategoriler modüler ve tekrar kullanılabilir bir yapıya kavuştu.
- **Arayüz (UI) Revizyonu:** `Index.cshtml` sayfası iki kolonlu (Sidebar + Content) bir yapıya dönüştürüldü. Sol menüye filtreler ve kategoriler, üst kısma ise arama çubuğu eklendi. Bootstrap Switch ve modern kart yapıları ile görsel kalite artırıldı.
- **Otomasyon ve Rotalama:** Projeyi tek tıkla başlatmak için `baslat.bat` dosyası eklendi. Uygulamanın varsayılan açılış sayfası (Home -> Bungalow) olarak güncellenerek 404 hatası giderildi.
- **Raporlama:** 6. hafta ilerleme raporu (`Hafta6_Rapor.md`) oluşturuldu ve dökümante edildi.

## [2026-04-08] Dinamik Özellik Yönetimi ve Rezervasyon Motoru (7. Hafta)
- **Dinamik Amenity Sistemi:** Sabit boolean özellikler kaldırılarak `Amenity` entity'si ve Many-to-Many ilişki kuruldu. Admin panelinden dinamik özellik yönetimi (CRUD) sağlandı.
- **Rezervasyon Altyapısı:** `IReservationService` çakışma kontrolü ve dolu tarih listeleme özellikleriyle güncellendi.
- **Takvim Entegrasyonu:** FullCalendar.js kullanılarak bungalov detay sayfasında müsaitlik takvimi oluşturuldu.
- **Rezervasyon Akışı:** Tarih seçimi, anlık fiyat hesaplama, AJAX ile kayıt, e-posta bildirimi (simülasyon) ve SweetAlert2 ile görsel onay mekanizması kuruldu.
- **UI Geliştirmeleri:** Bungalov kartlarında özellik isimlerinin gösterilmesi ve sidebar üzerinde özellik bazlı filtreleme yapılması sağlandı. Detay Sayfası tasarlandı.
- **Raporlama:** 7. hafta ilerleme raporu (`Hafta7_Rapor.md`) oluşturuldu.

## [2026-05-07] Identity, Profil Yönetimi ve Rivora Markalaması (8. Hafta)
- **Identity Entegrasyonu:** ASP.NET Core Identity kütüphanesi projeye dahil edildi. `AppUser` sınıfı ile genişletilmiş kullanıcı modeli (Ad, Soyad, Adres) oluşturuldu.
- **Giriş/Kayıt Sistemi:** `AccountController` üzerinden kullanıcı kayıt, giriş ve çıkış işlemleri tamamlandı. Role tabanlı yetkilendirme (Admin/Customer) eklendi.
- **Profil Paneli:** Kullanıcıların kendi bilgilerini güncelleyebileceği ve rezervasyonlarını takip edebileceği `ProfileController` ve ilgili görünümler oluşturuldu.
- **Rivora Rebranding:** Proje ismi ve tüm arayüz metinleri "Bungalov"dan "Rivora"ya dönüştürüldü. Kurumsal kimlik çalışmaları uygulandı.
- **Admin Dashboard:** Yönetici için özet istatistikler (toplam kazanç, rezervasyon sayısı vb.) içeren dashboard sayfası `AdminController` ile hayata geçirildi.
- **Güvenlik İyileştirmesi:** Rezervasyon formlarından ve e-postalardan T.C. Kimlik No gösterimi kaldırılarak veri güvenliği artırıldı.
- **Raporlama:** 8. hafta ilerleme raporu (`Hafta8_Rapor.md`) oluşturuldu.

## [2026-05-07] Güvenlik ve Global Error Handling (9. Hafta)
- **Serilog Entegrasyonu:** `Serilog.AspNetCore` ve `Serilog.Sinks.File` paketleri kurularak `Program.cs` üzerinde yapılandırıldı. Hatalar artık `Logs/` klasörüne kaydediliyor.
- **Hata Yönetimi:** `ErrorController` ve özel hata görünümleri (`Index`, `NotFound`) oluşturuldu. `UseExceptionHandler` ve `UseStatusCodePagesWithReExecute` middleware'leri eklendi.
- **CSRF Koruması:** Tüm POST action'larına `[ValidateAntiForgeryToken]` eklendi. AJAX (fetch) istekleri CSRF token desteğiyle güncellendi.
## [2026-05-14] Değerlendirme Sistemi ve Canlı Destek (10. Hafta)
- **Değerlendirme ve Yorum Sistemi:** Kullanıcıların konaklama sonrası bungalovları 1-5 yıldız arası puanlayabileceği ve yorum yapabileceği `Review` sistemi geliştirildi.
- **Canlı Destek (SignalR):** Müşteri ve Admin arasında gerçek zamanlı iletişim sağlayan `ChatHub` oluşturuldu.
- **Gelişmiş Mesajlaşma:** Yazıyor göstergesi (typing indicator), okunmamış mesaj bildirimleri ve mesajların veritabanında kalıcı olarak saklanması özellikleri eklendi.
- **İstatistik Güncelleme:** Bungalov detay sayfasında ortalama puan ve toplam yorum sayısı dinamik olarak gösterilmeye başlandı.
- **Raporlama:** 10. hafta ilerleme raporu (`Hafta10_Rapor.md`) oluşturuldu.

