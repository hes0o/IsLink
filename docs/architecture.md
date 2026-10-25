# Architecture

🇹🇷 [Türkçe](architecture.tr.md)

## Components

```
┌─────────────────────────────┐        ┌──────────────────────────────────────────┐
│ islink-frontend (Vercel)    │  HTTP  │ islink-backend / IsLink.API (Render)     │
│ React 18 + Vite SPA         │ ─────► │ ASP.NET Core 8, JWT, Swagger             │
│ services/api.js (fetch)     │  JSON  │ Controllers → Services → EF Core / Mongo │
└─────────────────────────────┘        └──────┬──────────────┬──────────────┬─────┘
                                              ▼              ▼              ▼
                                       PostgreSQL       MongoDB         Groq API
                                       (Neon)           (Atlas)         llama-3.3-70b
```

The backend is a single ASP.NET Core project. Each area has a controller, a service interface and a service class (`Services/I*.cs` + `Services/*.cs`), registered as scoped services in `Program.cs`.

## Hybrid database

| Store | Holds | Why |
| --- | --- | --- |
| PostgreSQL via EF Core (`Data/ApplicationDbContext.cs`) | Users, skills, languages, categories, gigs, packages, features, images, tags, orders, deliveries, reviews, favorites, transactions, LinkerAI chat sessions and messages | Relational data with foreign keys and transactions |
| MongoDB via the official driver, database `islink` | Conversations, messages, notifications (`Models/MongoDB/`) | High-volume, append-heavy, flexible documents |

A migration named `MoveMessagingToPostgres` exists, but `MessageService` and `NotificationService` still read and write MongoDB.

If no MongoDB connection string is configured the API still boots, defaulting to `mongodb://localhost:27017`.

## Data model (PostgreSQL)

| Entity | Key fields |
| --- | --- |
| `User` | `Username`, `Email`, `PasswordHash` (BCrypt), `FullName`, `Role` (`buyer` / `seller` / `admin`), `Rating`, `ReviewCount`, `CompletedOrders`, `Balance`, skills, languages |
| `Category` | `Name`, `Slug`, `Icon` |
| `Gig` | `Title`, `Slug`, `Description`, `SellerId`, `CategoryId`, images, tags, packages |
| `GigPackage` | `PackageType` (`basic` / `standard` / `premium`), `Name`, `Price`, `DeliveryDays`, features |
| `Order` | `GigId`, `BuyerId`, `SellerId`, package, price, `Requirements`, `Status`, `DeliveredAt`, deliveries |
| `Review` | `OrderId`, `Rating` (1–5), `Comment`, seller response |
| `Transaction`, `Favorite` | Defined and migrated; not used by any endpoint yet |
| `ChatSession`, `ChatMessage` | LinkerAI conversations per user, with saved recommendations |

Prices are stored as decimals (comments in the entities mention Syrian Lira).

## Order lifecycle

```
pending ──► in_progress ──► delivered ──► completed
   │             │              │
   ▼             ▼              ▼
cancelled    cancelled    revision_requested ──► in_progress
```

Enforced in `OrderService`. A buyer can't order their own gig, and only the buyer or seller of an order can see it. Moving to `delivered` stamps `DeliveredAt`; moving to `completed` stamps `CompletedAt` and increments the seller's `CompletedOrders`. Each change sends a notification to the other party.

## LinkerAI

`Services/LinkerAIService.cs`:

1. **Start.** A session is created in PostgreSQL with a system prompt that tells the model to act as a project manager and gather the project type, budget and deadline.
2. **Chat.** Each user message plus the history is sent to Groq's OpenAI-compatible endpoint (`https://api.groq.com/openai/v1/chat/completions`, model `llama-3.3-70b-versatile`). The reply is saved.
3. **Requirement extraction.** Project type, budget (amounts like `$200` or `200$`) and deadline in days are pulled from the conversation text with rules and regular expressions. Anything missing is returned in `missingInfo`.
4. **Recommendations.** When the conversation is complete, gigs are fetched, filtered against the extracted requirements, and a set of services that fits the total budget is returned and stored on the session.

The Groq key is read from `Groq:ApiKey` (`Groq__ApiKey` or `GROQ_API_KEY` env vars are also checked by the status endpoint). Without it, chat calls return an error message.

## Security

- **Passwords:** BCrypt (`BCrypt.Net-Next`).
- **Auth:** HS256 JWT validated for issuer, audience, lifetime and signature with zero clock skew. Secret, issuer, audience and lifetime come from `JwtSettings`.
- **CORS:** allows any origin containing `localhost`, ending in `.vercel.app` or ending in `.onrender.com`, with credentials. That includes every Vercel and Render app, not just yours; narrow it to your own domains for production.
- **Swagger** is always enabled, including in production.
- **Errors:** `ExceptionMiddleware` turns unhandled exceptions into JSON `500` responses.
- **Secrets:** keep connection strings and API keys out of `appsettings*.json`; see [setup.md](setup.md#configuration).

## Frontend

| Path | Contents |
| --- | --- |
| `src/App.jsx` | Routes: `/`, `/gigs`, `/gig/:slug`, `/profile/:username`, `/linkerai`, `/gigs/create`, `/dashboard`, `/orders/:orderId`, `/messages`, `/messages/:conversationId`, `/auth/login`, `/auth/register` |
| `src/services/api.js` | Every API call, built on `fetch`, adds the JWT |
| `src/context/AuthContext.jsx` | Signed-in user and token |
| `src/pages/` | One folder per page with its own CSS |
| `src/styles/variables.css` | Design tokens |
| `vercel.json` | Rewrites every path to `/` for client-side routing |
