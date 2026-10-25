# API reference

🇹🇷 [Türkçe](api.tr.md)

- Base URL (local): `http://localhost:5001/api`
- Swagger UI: `/swagger` (always on)
- Health: `GET /api/health` · Root: `GET /`

## Authentication

Register or log in to get a JWT, then send it on protected routes:

```http
Authorization: Bearer <token>
```

Tokens are valid for `JwtSettings:ExpirationInDays` (7 by default). 🔒 marks routes that need a token.

## Auth

| Method | Endpoint | | Description |
| --- | --- | --- | --- |
| POST | `/api/auth/register` | | Create an account |
| POST | `/api/auth/login` | | Log in |
| POST | `/api/auth/logout` | 🔒 | Log out (marks the user offline) |
| GET | `/api/auth/me` | 🔒 | Current user |

```json
// POST /api/auth/register
{ "email": "jane@example.com", "username": "jane", "password": "…", "fullName": "Jane Doe", "accountType": "seller" }

// POST /api/auth/login
{ "email": "jane@example.com", "password": "…" }
```

`accountType` is `buyer` (default) or `seller`.

## Users

| Method | Endpoint | | Description |
| --- | --- | --- | --- |
| GET | `/api/users/sellers` | | List sellers. Query: `skill`, `country`, `minRating`, `limit` (20), `offset` |
| GET | `/api/users/{username}` | | Public profile |
| PUT | `/api/users/profile` | 🔒 | Update my profile |

## Categories

| Method | Endpoint | Description |
| --- | --- | --- |
| GET | `/api/categories` | All categories |
| GET | `/api/categories/{slug}` | One category |

## Gigs

| Method | Endpoint | | Description |
| --- | --- | --- | --- |
| GET | `/api/gigs` | | Search gigs |
| GET | `/api/gigs/{slug}` | | Gig detail with packages, images, tags and seller |
| GET | `/api/gigs/seller/{username}` | | A seller's gigs |
| POST | `/api/gigs` | 🔒 | Create a gig |

`GET /api/gigs` query: `category` (slug), `search`, `minPrice`, `maxPrice`, `deliveryTime` (days), `sortBy` (default `recommended`), `limit` (20), `offset`.

`POST /api/gigs` body: `title`, `description`, `categoryId`, `images[]`, `tags[]`, and `packages` with `basic`, `standard` and `premium` entries (name, price, delivery days, features).

## Orders 🔒

| Method | Endpoint | Description |
| --- | --- | --- |
| POST | `/api/orders` | Order a gig package: `{"gigId": "…", "packageType": "basic", "requirements": "…"}` |
| GET | `/api/orders` | My orders. Query: `role` (`buyer` default, or `seller`), `status`, `limit`, `offset` |
| GET | `/api/orders/{id}` | One order, if I'm its buyer or seller |
| PATCH | `/api/orders/{id}/status` | `{"status": "in_progress"}` |

You can't order your own gig. Allowed status changes:

| From | To |
| --- | --- |
| `pending` | `in_progress`, `cancelled` |
| `in_progress` | `delivered`, `cancelled` |
| `delivered` | `completed`, `revision_requested` |
| `revision_requested` | `in_progress` |

## Reviews

| Method | Endpoint | | Description |
| --- | --- | --- | --- |
| GET | `/api/reviews/gig/{gigId}` | | Reviews of a gig. Query: `limit` (10), `offset` |
| GET | `/api/reviews/seller/{username}` | | Reviews of a seller |
| POST | `/api/reviews` | 🔒 | `{"orderId": "…", "rating": 5, "comment": "…"}` (rating 1–5) |
| POST | `/api/reviews/{id}/response` | 🔒 | Seller reply: `{"response": "…"}` |

## Messages 🔒 (MongoDB)

| Method | Endpoint | Description |
| --- | --- | --- |
| GET | `/api/messages/conversations` | My conversations |
| POST | `/api/messages/conversations` | Get or create a conversation: `{"participantId": "…", "relatedGig": "…", "relatedOrder": "…"}` |
| GET | `/api/messages/conversations/{conversationId}` | Messages. Query: `limit` (50), `before` (timestamp, for paging back) |
| POST | `/api/messages/conversations/{conversationId}` | Send: `{"content": "…", "messageType": "text"}` (attachments and custom offers are also supported) |

## Notifications 🔒 (MongoDB)

| Method | Endpoint | Description |
| --- | --- | --- |
| GET | `/api/notifications` | Query: `limit` (20), `skip` |
| GET | `/api/notifications/unread-count` | Unread count |
| POST | `/api/notifications/mark-all-read` | Mark everything read |

## LinkerAI 🔒

| Method | Endpoint | Description |
| --- | --- | --- |
| POST | `/api/linkerai/start` | Start a session: `{"initialMessage": "I need a logo"}` → greeting and `sessionId` |
| POST | `/api/linkerai/chat` | Send a message: `{"sessionId": "…", "message": "Budget $200, 5 days"}` |
| GET | `/api/linkerai/recommendations/{sessionId}` | Recommendations for a finished session |
| GET | `/api/linkerai/active` | My active session |
| GET | `/api/linkerai/sessions` | My past sessions |
| GET | `/api/linkerai/session/{sessionId}` | One session with its messages |
| GET | `/api/linkerai/status` | Whether a Groq key is configured |

Chat response:

```json
{
  "success": true,
  "sessionId": "…",
  "message": "Great. Any style preferences for the logo?",
  "isComplete": false,
  "missingInfo": ["deadline"],
  "recommendations": null
}
```

Once the project type, budget and deadline are known, `isComplete` turns `true` and `recommendations` holds the matched gigs. If Groq fails, `success` is `false` and `message` carries the error.

## Errors

Validation and "not found" cases return `400` / `404` from the controllers. Unhandled exceptions are caught by `ExceptionMiddleware` and returned as JSON with status `500`. Missing or invalid tokens give `401`.
