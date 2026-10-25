# 🔗 IsLink

> Yetenekleri birbirine bağlar, hayalleri güçlendirir. Müşterilerin freelancer bulup iş verdiği bir freelance pazar yeri; proje fikrini uygun hizmetlerden oluşan bir kısa listeye çeviren asistan **LinkerAI** ile.

🇬🇧 [English README](README.md) · 📚 [Dokümantasyon](docs/)

---

## Özellikler

- **LinkerAI eşleştirici.** Bir sohbet asistanı (Groq, `llama-3.3-70b-versatile`) proje türünü, bütçeyi ve teslim süresini öğrenene kadar soru sorar, ardından bütçeye uyan gig'leri önerir. Sohbet geçmişi kullanıcı bazında saklanır.
- **Pazar yeri.** Kategori, etiket, görsel galerisi ve Basic / Standard / Premium paketleri olan gig'ler; kategori, fiyat ve teslim süresine göre arama ve filtre; yetenek, dil ve puan içeren satıcı profilleri.
- **Siparişler.** Alıcı bir gig paketi sipariş eder, satıcı teslim eder, alıcı onaylar veya revizyon ister: `pending → in_progress → delivered → completed`, ayrıca `revision_requested` ve `cancelled` dalları.
- **Değerlendirmeler.** Alıcılar tamamlanan siparişleri 1–5 arası puanlar, satıcılar yanıt verebilir.
- **Mesajlaşma ve bildirimler.** Bir gig'e bağlanabilen konuşmalar içeren gelen kutusu ve uygulama içi bildirimler; MongoDB'de saklanır.
- **Hesaplar.** Alıcı veya satıcı olarak kayıt, JWT ile giriş, BCrypt şifre hash'leme.

## Teknolojiler

| Katman | Teknoloji |
| --- | --- |
| Frontend | React 18, Vite 6, React Router 7, Context API, saf CSS. Vercel'de barındırılır. |
| Backend | .NET 8, ASP.NET Core Web API, EF Core 8, AutoMapper, Swagger. Render'da (Docker) barındırılır. |
| Veritabanları | Kullanıcılar, gig'ler, siparişler, değerlendirmeler ve AI sohbet geçmişi için PostgreSQL (Neon); mesajlar ve bildirimler için MongoDB (Atlas) |
| Yapay zeka | Groq API (OpenAI uyumlu chat completions) |
| Kimlik doğrulama | JWT bearer token, BCrypt.Net |
| Test | xUnit + in-memory veritabanlı `WebApplicationFactory` |

## Mimari

```
React SPA (Vercel)  ──fetch──►  ASP.NET Core API (Render, Docker)
                                   │          │            │
                                   ▼          ▼            ▼
                           PostgreSQL     MongoDB      Groq API
                           (EF Core)      (driver)     (LinkerAI)
```

Veri modeli ve LinkerAI akışı dahil ayrıntılar: [docs/architecture.tr.md](docs/architecture.tr.md).

## Hızlı başlangıç

.NET 8 SDK, Node.js 18+, bir PostgreSQL veritabanı ve isteğe bağlı olarak MongoDB ile bir Groq API anahtarı gerekir.

### Backend

```bash
git clone https://github.com/hes0o/IsLink.git
cd IsLink/islink-backend/IsLink.API

dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:PostgreSQL" "Host=localhost;Database=islink;Username=postgres;Password=postgres"
dotnet user-secrets set "ConnectionStrings:MongoDB" "mongodb://localhost:27017"
dotnet user-secrets set "JwtSettings:Secret" "<en az 32 rastgele karakter>"
dotnet user-secrets set "Groq:ApiKey" "<Groq anahtarınız>"

dotnet run --urls http://localhost:5001
```

Swagger: http://localhost:5001/swagger

> [!WARNING]
> `SKIP_DB_INIT=true` ayarlanmadıkça API her başlangıçta migration'ları ve ardından `DbSeeder`'ı çalıştırır; bu da **tüm kullanıcıları, gig'leri, siparişleri, değerlendirmeleri, işlemleri ve kategorileri siler** ve demo verisini yeniden oluşturur. API'yi yalnızca silinebilir bir veritabanına bağlayın ve gerçek verinin olduğu her ortamda `SKIP_DB_INIT=true` ayarlayın. Bkz. [docs/setup.tr.md](docs/setup.tr.md#veritabanı-başlatma).

### Frontend

```bash
cd IsLink/islink-frontend
npm install
npm run dev            # http://localhost:5173
```

`VITE_API_URL` ayarlanmadıkça frontend `http://localhost:5001/api` adresini çağırır.

Tam kurulum, yapılandırma anahtarları, Docker ve deploy: [docs/setup.tr.md](docs/setup.tr.md).

## Dokümantasyon

| Doküman | İçerik |
| --- | --- |
| [docs/architecture.tr.md](docs/architecture.tr.md) | Bileşenler, hibrit veritabanı, veri modeli, LinkerAI, sipariş yaşam döngüsü, güvenlik |
| [docs/api.tr.md](docs/api.tr.md) | Tüm endpoint'ler, kimlik doğrulama, istek örnekleri |
| [docs/setup.tr.md](docs/setup.tr.md) | Yapılandırma, yerel çalıştırma, testler, Docker, Render ve Vercel deploy |
| [PROJECT_DOCUMENTATION.md](PROJECT_DOCUMENTATION.md) | Orijinal proje raporu (İngilizce) |

## Proje yapısı

```
islink-backend/
├── IsLink.API/
│   ├── Controllers/     Auth, Users, Gigs, Categories, Orders, Reviews, Messages, Notifications, LinkerAI
│   ├── Services/        iş mantığı, her alan için bir arayüz + sınıf
│   ├── Data/            EF Core DbContext ve seed'ler
│   ├── Models/          Entity'ler (PostgreSQL), MongoDB dokümanları, DTO'lar
│   ├── Migrations/      EF Core migration'ları
│   ├── Middleware/      genel hata yakalayıcı
│   └── Dockerfile
└── IsLink.API.Tests/    LinkerAI endpoint testleri
islink-frontend/
├── src/pages/           Home, Gigs, GigDetails, CreateGig, Dashboard, OrderDetails, Messages, Profile, LinkerAI, Auth
├── src/components/      layout, gig kartları, hata sınırı
├── src/services/api.js  tüm HTTP çağrıları
└── src/context/         AuthContext
```
