# Kurulum ve deploy

🇬🇧 [English](setup.md)

## Gereksinimler

- .NET 8 SDK
- Node.js 18+ ve npm
- PostgreSQL 14+ (yerel, Docker veya Neon)
- MongoDB (yerel, Docker veya Atlas), mesajlar ve bildirimler için
- LinkerAI için Groq API anahtarı (https://console.groq.com)

## Yapılandırma

API standart ASP.NET Core yapılandırmasını okur: `appsettings.json`, sonra `appsettings.{Environment}.json`, user secrets (yalnızca Development), sonra ortam değişkenleri. Ortam değişkenlerinde `:` yerine `__` kullanılır.

| Anahtar | Ortam değişkeni | Not |
| --- | --- | --- |
| `ConnectionStrings:PostgreSQL` | `ConnectionStrings__PostgreSQL` | Npgsql bağlantı dizesi |
| `ConnectionStrings:MongoDB` | `ConnectionStrings__MongoDB` | Varsayılan `mongodb://localhost:27017`; veritabanı adı `islink` |
| `JwtSettings:Secret` | `JwtSettings__Secret` | En az 32 karakter |
| `JwtSettings:Issuer` / `Audience` | `JwtSettings__Issuer` / `JwtSettings__Audience` | `IsLink.API` / `IsLink.Client` |
| `JwtSettings:ExpirationInDays` | `JwtSettings__ExpirationInDays` | `7` |
| `Groq:ApiKey` | `Groq__ApiKey` | LinkerAI |
| — | `SKIP_DB_INIT` | `true` başlangıçta migration ve seed'i atlar |
| — | `ASPNETCORE_URLS` | ör. `http://localhost:5001` |
| — | `ASPNETCORE_ENVIRONMENT` | `Development`, `Production` veya `Test` (in-memory DB, seed yok) |

**Gizli bilgileri commit'lemeyin.** Gerçek değerleri yerelde user secrets'ta, production'da barındırıcının ortam ayarlarında tutun; `appsettings.json` içinde yer tutucu bırakın.

```bash
cd islink-backend/IsLink.API
dotnet user-secrets init          # .csproj'a bir kez UserSecretsId ekler
dotnet user-secrets set "ConnectionStrings:PostgreSQL" "Host=localhost;Database=islink;Username=postgres;Password=postgres"
dotnet user-secrets set "ConnectionStrings:MongoDB" "mongodb://localhost:27017"
dotnet user-secrets set "JwtSettings:Secret" "<en az 32 rastgele karakter>"
dotnet user-secrets set "Groq:ApiKey" "<Groq anahtarınız>"
```

Frontend: `islink-frontend/.env.local` içinde `VITE_API_URL` (varsayılan `http://localhost:5001/api`).

## Docker ile yerel veritabanları

```bash
docker run -d --name islink-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=islink -p 5432:5432 postgres:16
docker run -d --name islink-mongo -p 27017:27017 mongo:7
```

## Veritabanı başlatma

`ASPNETCORE_ENVIRONMENT=Test` veya `SKIP_DB_INIT=true` olmadıkça `Program.cs` başlangıçta:

1. `Database.MigrateAsync()` çalıştırır, sonra
2. `DbSeeder.SeedAsync()` çalıştırır; bu **tüm değerlendirmeleri, siparişleri, işlemleri, gig'leri, kullanıcı yeteneklerini, kullanıcı dillerini, kullanıcıları ve kategorileri siler**, ardından demo kategori, kullanıcı ve gig'leri ekler.

İki adımdaki hatalar da loglanır ve API'yi durdurmaz.

> [!CAUTION]
> 2. adım yüzünden API'yi yeniden başlatmak veya yeniden deploy etmek gerçek veriyi siler. Verisini korumak istediğiniz her ortamda `SKIP_DB_INIT=true` ayarlayın ve migration'ları kendiniz çalıştırın:
>
> ```bash
> dotnet tool install --global dotnet-ef
> dotnet ef database update --project islink-backend/IsLink.API
> ```

Demo kullanıcıların e-posta ve şifresi `Data/DbSeeder.cs` içinde.

## Yerelde çalıştırma

```bash
# API
cd islink-backend/IsLink.API
dotnet run --urls http://localhost:5001        # Swagger: http://localhost:5001/swagger

# Frontend
cd islink-frontend
npm install
npm run dev                                    # http://localhost:5173
```

`islink-backend/start-with-groq.sh`, macOS'taki Homebrew `dotnet@8` yolunu kullanarak API'yi 5001 portunda `SKIP_DB_INIT=true` ile başlatır.

## Testler

```bash
cd islink-backend
dotnet test
```

`IsLink.API.Tests`, API'yi `Test` ortamında (in-memory veritabanı) `WebApplicationFactory` ile başlatır ve `FakeLinkerAIService` kullanır; bu yüzden LinkerAI endpoint testleri ağ gerektirmez. `RealLinkerAISmokeTests`, `RUN_REAL_LINKERAI_SMOKE` ayarlanmadıkça atlanır; Groq'a geçişten önceki Gemini anahtarını aramaya devam eder.

## Docker

```bash
cd islink-backend/IsLink.API
docker build -t islink-api .
docker run -p 8080:8080 \
  -e ConnectionStrings__PostgreSQL="…" \
  -e ConnectionStrings__MongoDB="…" \
  -e JwtSettings__Secret="…" \
  -e Groq__ApiKey="…" \
  -e SKIP_DB_INIT=true \
  islink-api
```

İmaj çok aşamalı bir build'dir (derleme için SDK, çalıştırma için ASP.NET runtime) ve 8080 portunu dinler.

## Deploy

**Render'da backend:** Depodan kök dizini `islink-backend/IsLink.API` ve runtime'ı Docker olan bir Web Service oluşturun. Yukarıdaki ortam değişkenlerini Render panelinden ekleyin; gerçek kullanıcılar olduğunda `SKIP_DB_INIT=true` dahil.

**Vercel'de frontend:** Depoyu kök dizin `islink-frontend` ile içe aktarın (Vite preset, build `npm run build`, output `dist`). `VITE_API_URL` değerini `https://<render-servisiniz>.onrender.com/api` yapın. `vercel.json` tüm yolları zaten SPA'ya yönlendiriyor.

**Veritabanları:** PostgreSQL için Neon (`SSL Mode=Require` kullanın), MongoDB için MongoDB Atlas. Atlas ağ erişiminde Render'ın çıkış IP'lerine izin verin.

## Sorun giderme

| Belirti | Çözüm |
| --- | --- |
| LinkerAI'den `Groq API key not configured` | `Groq:ApiKey` ayarlayın. `GET /api/linkerai/status` ile kontrol edin. |
| Frontend API'ye ulaşamıyor | Port uyuşmazlığı: frontend varsayılan olarak 5001 kullanır, `--urls` olmadan `dotnet run` başka bir port seçebilir. |
| Özel alan adından CORS hatası | Alan adını `Program.cs` içindeki `AllowFrontend` politikasına ekleyin. |
| Loglarda `⚠️ Database initialization warning` | Migration veya seed başarısız; PostgreSQL bağlantı dizesini kontrol edin. API çalışmaya devam eder. |
| Deploy sonrası tüm verim kayboldu | Bkz. [Veritabanı başlatma](#veritabanı-başlatma). |
