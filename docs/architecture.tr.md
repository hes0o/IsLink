# Mimari

🇬🇧 [English](architecture.md)

## Bileşenler

```
┌─────────────────────────────┐        ┌──────────────────────────────────────────┐
│ islink-frontend (Vercel)    │  HTTP  │ islink-backend / IsLink.API (Render)     │
│ React 18 + Vite SPA         │ ─────► │ ASP.NET Core 8, JWT, Swagger             │
│ services/api.js (fetch)     │  JSON  │ Controller → Servis → EF Core / Mongo    │
└─────────────────────────────┘        └──────┬──────────────┬──────────────┬─────┘
                                              ▼              ▼              ▼
                                       PostgreSQL       MongoDB         Groq API
                                       (Neon)           (Atlas)         llama-3.3-70b
```

Backend tek bir ASP.NET Core projesidir. Her alanın bir controller'ı, bir servis arayüzü ve bir servis sınıfı vardır (`Services/I*.cs` + `Services/*.cs`); `Program.cs` içinde scoped servis olarak kaydedilir.

## Hibrit veritabanı

| Depo | İçerik | Neden |
| --- | --- | --- |
| EF Core ile PostgreSQL (`Data/ApplicationDbContext.cs`) | Kullanıcılar, yetenekler, diller, kategoriler, gig'ler, paketler, özellikler, görseller, etiketler, siparişler, teslimatlar, değerlendirmeler, favoriler, işlemler, LinkerAI sohbet oturumları ve mesajları | Foreign key ve transaction gerektiren ilişkisel veri |
| Resmi sürücüyle MongoDB, `islink` veritabanı | Konuşmalar, mesajlar, bildirimler (`Models/MongoDB/`) | Yüksek hacimli, çoğunlukla ekleme yapılan esnek dokümanlar |

`MoveMessagingToPostgres` adlı bir migration var, ancak `MessageService` ve `NotificationService` hâlâ MongoDB'ye okuyup yazıyor.

MongoDB bağlantı dizesi yoksa API yine de açılır ve `mongodb://localhost:27017` varsayılanını kullanır.

## Veri modeli (PostgreSQL)

| Entity | Önemli alanlar |
| --- | --- |
| `User` | `Username`, `Email`, `PasswordHash` (BCrypt), `FullName`, `Role` (`buyer` / `seller` / `admin`), `Rating`, `ReviewCount`, `CompletedOrders`, `Balance`, yetenekler, diller |
| `Category` | `Name`, `Slug`, `Icon` |
| `Gig` | `Title`, `Slug`, `Description`, `SellerId`, `CategoryId`, görseller, etiketler, paketler |
| `GigPackage` | `PackageType` (`basic` / `standard` / `premium`), `Name`, `Price`, `DeliveryDays`, özellikler |
| `Order` | `GigId`, `BuyerId`, `SellerId`, paket, fiyat, `Requirements`, `Status`, `DeliveredAt`, teslimatlar |
| `Review` | `OrderId`, `Rating` (1–5), `Comment`, satıcı yanıtı |
| `Transaction`, `Favorite` | Tanımlı ve migration'ı var; henüz hiçbir endpoint kullanmıyor |
| `ChatSession`, `ChatMessage` | Kullanıcı başına LinkerAI konuşmaları ve kaydedilen öneriler |

Fiyatlar decimal olarak saklanır (entity'lerdeki yorumlar Suriye Lirası'ndan bahseder).

## Sipariş yaşam döngüsü

```
pending ──► in_progress ──► delivered ──► completed
   │             │              │
   ▼             ▼              ▼
cancelled    cancelled    revision_requested ──► in_progress
```

`OrderService` içinde uygulanır. Alıcı kendi gig'ini sipariş edemez ve bir siparişi yalnızca alıcısı veya satıcısı görebilir. `delivered`'a geçiş `DeliveredAt`'i, `completed`'a geçiş `CompletedAt`'i işaretler ve satıcının `CompletedOrders` sayısını artırır. Her değişiklik karşı tarafa bildirim gönderir.

## LinkerAI

`Services/LinkerAIService.cs`:

1. **Başlat.** PostgreSQL'de, modele proje yöneticisi gibi davranıp proje türünü, bütçeyi ve teslim süresini toplamasını söyleyen bir sistem prompt'uyla oturum açılır.
2. **Sohbet.** Her kullanıcı mesajı geçmişle birlikte Groq'un OpenAI uyumlu endpoint'ine (`https://api.groq.com/openai/v1/chat/completions`, model `llama-3.3-70b-versatile`) gönderilir. Yanıt kaydedilir.
3. **Gereksinim çıkarma.** Proje türü, bütçe (`$200` veya `200$` gibi tutarlar) ve gün cinsinden teslim süresi, konuşma metninden kurallar ve düzenli ifadelerle çıkarılır. Eksik olanlar `missingInfo` içinde döner.
4. **Öneriler.** Konuşma tamamlanınca gig'ler çekilir, çıkarılan gereksinimlere göre filtrelenir ve toplam bütçeye uyan bir hizmet seti döndürülüp oturuma kaydedilir.

Groq anahtarı `Groq:ApiKey` üzerinden okunur (status endpoint'i `Groq__ApiKey` ve `GROQ_API_KEY` ortam değişkenlerine de bakar). Anahtar yoksa sohbet çağrıları hata mesajı döner.

## Güvenlik

- **Şifreler:** BCrypt (`BCrypt.Net-Next`).
- **Kimlik doğrulama:** Issuer, audience, süre ve imza açısından sıfır saat toleransıyla doğrulanan HS256 JWT. Secret, issuer, audience ve süre `JwtSettings`'ten gelir.
- **CORS:** `localhost` içeren, `.vercel.app` veya `.onrender.com` ile biten her origin'e credentials ile izin verir. Bu yalnızca sizinkiler değil, tüm Vercel ve Render uygulamaları demektir; production için kendi alan adlarınıza daraltın.
- **Swagger** production dahil her zaman açıktır.
- **Hatalar:** `ExceptionMiddleware` yakalanmayan istisnaları JSON `500` yanıtına çevirir.
- **Gizli bilgiler:** Bağlantı dizelerini ve API anahtarlarını `appsettings*.json` dışında tutun; bkz. [setup.tr.md](setup.tr.md#yapılandırma).

## Frontend

| Yol | İçerik |
| --- | --- |
| `src/App.jsx` | Route'lar: `/`, `/gigs`, `/gig/:slug`, `/profile/:username`, `/linkerai`, `/gigs/create`, `/dashboard`, `/orders/:orderId`, `/messages`, `/messages/:conversationId`, `/auth/login`, `/auth/register` |
| `src/services/api.js` | Tüm API çağrıları, `fetch` üzerine kurulu, JWT'yi ekler |
| `src/context/AuthContext.jsx` | Giriş yapmış kullanıcı ve token |
| `src/pages/` | Kendi CSS'iyle sayfa başına bir klasör |
| `src/styles/variables.css` | Tasarım değişkenleri |
| `vercel.json` | İstemci tarafı routing için tüm yolları `/`'e yönlendirir |
