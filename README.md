# 🔗 IsLink

> Connecting talent, empowering dreams. A freelance marketplace where clients find and hire freelancers, with **LinkerAI**, an assistant that turns a project idea into a shortlist of matching services.

🇹🇷 [Türkçe README](README.tr.md) · 📚 [Documentation](docs/)

---

## Features

- **LinkerAI matchmaker.** A chat assistant (Groq, `llama-3.3-70b-versatile`) asks about the project until it knows the project type, budget and deadline, then recommends gigs that fit the budget. Chat history is saved per user.
- **Marketplace.** Gigs with categories, tags, image galleries and Basic / Standard / Premium packages; search and filters by category, price and delivery time; seller profiles with skills, languages and ratings.
- **Orders.** Buyers order a gig package, sellers deliver, buyers accept or ask for a revision: `pending → in_progress → delivered → completed`, with `revision_requested` and `cancelled` branches.
- **Reviews.** Buyers rate completed orders (1–5) and sellers can reply.
- **Messaging and notifications.** Inbox with conversations that can be tied to a gig, plus in-app notifications, stored in MongoDB.
- **Accounts.** Register as buyer or seller, JWT sign-in, BCrypt password hashing.

## Tech stack

| Layer | Technology |
| --- | --- |
| Frontend | React 18, Vite 6, React Router 7, Context API, plain CSS. Hosted on Vercel. |
| Backend | .NET 8, ASP.NET Core Web API, EF Core 8, AutoMapper, Swagger. Hosted on Render (Docker). |
| Databases | PostgreSQL (Neon) for users, gigs, orders, reviews and AI chat history; MongoDB (Atlas) for messages and notifications |
| AI | Groq API (OpenAI-compatible chat completions) |
| Auth | JWT bearer tokens, BCrypt.Net |
| Tests | xUnit + `WebApplicationFactory` with an in-memory database |

## Architecture

```
React SPA (Vercel)  ──fetch──►  ASP.NET Core API (Render, Docker)
                                   │          │            │
                                   ▼          ▼            ▼
                           PostgreSQL     MongoDB      Groq API
                           (EF Core)      (driver)     (LinkerAI)
```

Details, including the data model and LinkerAI flow: [docs/architecture.md](docs/architecture.md).

## Quickstart

You need the .NET 8 SDK, Node.js 18+, a PostgreSQL database, and optionally MongoDB and a Groq API key.

### Backend

```bash
git clone https://github.com/hes0o/IsLink.git
cd IsLink/islink-backend/IsLink.API

dotnet user-secrets set "ConnectionStrings:PostgreSQL" "Host=localhost;Database=islink;Username=postgres;Password=postgres"
dotnet user-secrets set "ConnectionStrings:MongoDB" "mongodb://localhost:27017"
dotnet user-secrets set "JwtSettings:Secret" "<at least 32 random characters>"
dotnet user-secrets set "Groq:ApiKey" "<your Groq key>"

dotnet run --urls http://localhost:5001
```

Swagger: http://localhost:5001/swagger

On startup the API applies EF Core migrations and, only if the database is empty, adds demo categories, users and gigs. Existing data is never modified. Set `SKIP_DB_INIT=true` to skip both.

### Frontend

```bash
cd IsLink/islink-frontend
npm install
npm run dev            # http://localhost:5173
```

The frontend calls `http://localhost:5001/api` unless `VITE_API_URL` is set.

Full setup, configuration keys, Docker and deployment: [docs/setup.md](docs/setup.md).

## Documentation

| Document | Contents |
| --- | --- |
| [docs/architecture.md](docs/architecture.md) | Components, hybrid database, data model, LinkerAI, order lifecycle, security |
| [docs/api.md](docs/api.md) | Every endpoint, auth, request examples |
| [docs/setup.md](docs/setup.md) | Configuration, local run, tests, Docker, Render and Vercel deployment |
| [PROJECT_DOCUMENTATION.md](PROJECT_DOCUMENTATION.md) | Original project write-up |

## Project structure

```
islink-backend/
├── IsLink.API/
│   ├── Controllers/     Auth, Users, Gigs, Categories, Orders, Reviews, Messages, Notifications, LinkerAI
│   ├── Services/        business logic, one interface + class per area
│   ├── Data/            EF Core DbContext and seeders
│   ├── Models/          Entities (PostgreSQL), MongoDB documents, DTOs
│   ├── Migrations/      EF Core migrations
│   ├── Middleware/      global exception handler
│   └── Dockerfile
└── IsLink.API.Tests/    LinkerAI endpoint tests
islink-frontend/
├── src/pages/           Home, Gigs, GigDetails, CreateGig, Dashboard, OrderDetails, Messages, Profile, LinkerAI, Auth
├── src/components/      layout, gig cards, error boundary
├── src/services/api.js  all HTTP calls
└── src/context/         AuthContext
```
