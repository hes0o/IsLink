# API referansı

🇬🇧 [English](api.md)

- Temel adres (yerel): `http://localhost:5001/api`
- Swagger UI: `/swagger` (her zaman açık)
- Sağlık: `GET /api/health` · Kök: `GET /`

## Kimlik doğrulama

JWT almak için kayıt olun veya giriş yapın, ardından korumalı route'larda gönderin:

```http
Authorization: Bearer <token>
```

Token'lar `JwtSettings:ExpirationInDays` (varsayılan 7) gün geçerlidir. 🔒 token gerektiren route'ları gösterir.

## Auth

| Metot | Endpoint | | Açıklama |
| --- | --- | --- | --- |
| POST | `/api/auth/register` | | Hesap oluştur |
| POST | `/api/auth/login` | | Giriş yap |
| POST | `/api/auth/logout` | 🔒 | Çıkış (kullanıcıyı çevrimdışı işaretler) |
| GET | `/api/auth/me` | 🔒 | Oturum sahibi |

```json
// POST /api/auth/register
{ "email": "jane@example.com", "username": "jane", "password": "…", "fullName": "Jane Doe", "accountType": "seller" }

// POST /api/auth/login
{ "email": "jane@example.com", "password": "…" }
```

`accountType`: `buyer` (varsayılan) veya `seller`.

## Kullanıcılar

| Metot | Endpoint | | Açıklama |
| --- | --- | --- | --- |
| GET | `/api/users/sellers` | | Satıcıları listele. Query: `skill`, `country`, `minRating`, `limit` (20), `offset` |
| GET | `/api/users/{username}` | | Herkese açık profil |
| PUT | `/api/users/profile` | 🔒 | Profilimi güncelle |

## Kategoriler

| Metot | Endpoint | Açıklama |
| --- | --- | --- |
| GET | `/api/categories` | Tüm kategoriler |
| GET | `/api/categories/{slug}` | Tek kategori |

## Gig'ler

| Metot | Endpoint | | Açıklama |
| --- | --- | --- | --- |
| GET | `/api/gigs` | | Gig ara |
| GET | `/api/gigs/{slug}` | | Paketler, görseller, etiketler ve satıcıyla gig detayı |
| GET | `/api/gigs/seller/{username}` | | Bir satıcının gig'leri |
| POST | `/api/gigs` | 🔒 | Gig oluştur |

`GET /api/gigs` query: `category` (slug), `search`, `minPrice`, `maxPrice`, `deliveryTime` (gün), `sortBy` (varsayılan `recommended`), `limit` (20), `offset`.

`POST /api/gigs` gövdesi: `title`, `description`, `categoryId`, `images[]`, `tags[]` ve `basic`, `standard`, `premium` girişleri olan `packages` (ad, fiyat, teslim günü, özellikler).

## Siparişler 🔒

| Metot | Endpoint | Açıklama |
| --- | --- | --- |
| POST | `/api/orders` | Gig paketi sipariş et: `{"gigId": "…", "packageType": "basic", "requirements": "…"}` |
| GET | `/api/orders` | Siparişlerim. Query: `role` (varsayılan `buyer` veya `seller`), `status`, `limit`, `offset` |
| GET | `/api/orders/{id}` | Alıcısı veya satıcısı olduğum tek sipariş |
| PATCH | `/api/orders/{id}/status` | `{"status": "in_progress"}` |

Kendi gig'inizi sipariş edemezsiniz. İzin verilen durum geçişleri:

| Kaynak | Hedef |
| --- | --- |
| `pending` | `in_progress`, `cancelled` |
| `in_progress` | `delivered`, `cancelled` |
| `delivered` | `completed`, `revision_requested` |
| `revision_requested` | `in_progress` |

## Değerlendirmeler

| Metot | Endpoint | | Açıklama |
| --- | --- | --- | --- |
| GET | `/api/reviews/gig/{gigId}` | | Bir gig'in değerlendirmeleri. Query: `limit` (10), `offset` |
| GET | `/api/reviews/seller/{username}` | | Bir satıcının değerlendirmeleri |
| POST | `/api/reviews` | 🔒 | `{"orderId": "…", "rating": 5, "comment": "…"}` (puan 1–5) |
| POST | `/api/reviews/{id}/response` | 🔒 | Satıcı yanıtı: `{"response": "…"}` |

## Mesajlar 🔒 (MongoDB)

| Metot | Endpoint | Açıklama |
| --- | --- | --- |
| GET | `/api/messages/conversations` | Konuşmalarım |
| POST | `/api/messages/conversations` | Konuşmayı getir veya oluştur: `{"participantId": "…", "relatedGig": "…", "relatedOrder": "…"}` |
| GET | `/api/messages/conversations/{conversationId}` | Mesajlar. Query: `limit` (50), `before` (geriye sayfalama için zaman damgası) |
| POST | `/api/messages/conversations/{conversationId}` | Gönder: `{"content": "…", "messageType": "text"}` (ekler ve özel teklifler de desteklenir) |

## Bildirimler 🔒 (MongoDB)

| Metot | Endpoint | Açıklama |
| --- | --- | --- |
| GET | `/api/notifications` | Query: `limit` (20), `skip` |
| GET | `/api/notifications/unread-count` | Okunmamış sayısı |
| POST | `/api/notifications/mark-all-read` | Tümünü okundu yap |

## LinkerAI 🔒

| Metot | Endpoint | Açıklama |
| --- | --- | --- |
| POST | `/api/linkerai/start` | Oturum başlat: `{"initialMessage": "Logo lazım"}` → karşılama ve `sessionId` |
| POST | `/api/linkerai/chat` | Mesaj gönder: `{"sessionId": "…", "message": "Bütçe $200, 5 gün"}` |
| GET | `/api/linkerai/recommendations/{sessionId}` | Tamamlanmış oturumun önerileri |
| GET | `/api/linkerai/active` | Aktif oturumum |
| GET | `/api/linkerai/sessions` | Geçmiş oturumlarım |
| GET | `/api/linkerai/session/{sessionId}` | Mesajlarıyla tek oturum |
| GET | `/api/linkerai/status` | Groq anahtarı yapılandırılmış mı |

Sohbet yanıtı:

```json
{
  "success": true,
  "sessionId": "…",
  "message": "Harika. Logo için tercih ettiğiniz bir stil var mı?",
  "isComplete": false,
  "missingInfo": ["deadline"],
  "recommendations": null
}
```

Proje türü, bütçe ve teslim süresi belli olunca `isComplete` `true` olur ve `recommendations` eşleşen gig'leri içerir. Groq hata verirse `success` `false` olur ve `message` hatayı taşır.

## Hatalar

Doğrulama ve "bulunamadı" durumları controller'lardan `400` / `404` döner. Yakalanmayan istisnalar `ExceptionMiddleware` tarafından `500` durumlu JSON olarak döner. Token eksik veya geçersizse `401`.
