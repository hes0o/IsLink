# Setup and deployment

🇹🇷 [Türkçe](setup.tr.md)

## Requirements

- .NET 8 SDK
- Node.js 18+ and npm
- PostgreSQL 14+ (local, Docker, or Neon)
- MongoDB (local, Docker, or Atlas), for messages and notifications
- A Groq API key, for LinkerAI (https://console.groq.com)

## Configuration

The API reads standard ASP.NET Core configuration: `appsettings.json`, then `appsettings.{Environment}.json`, user secrets (Development only), then environment variables. In environment variables, `:` becomes `__`.

| Key | Env var | Notes |
| --- | --- | --- |
| `ConnectionStrings:PostgreSQL` | `ConnectionStrings__PostgreSQL` | Npgsql connection string |
| `ConnectionStrings:MongoDB` | `ConnectionStrings__MongoDB` | Defaults to `mongodb://localhost:27017`; database name is `islink` |
| `JwtSettings:Secret` | `JwtSettings__Secret` | At least 32 characters |
| `JwtSettings:Issuer` / `Audience` | `JwtSettings__Issuer` / `JwtSettings__Audience` | `IsLink.API` / `IsLink.Client` |
| `JwtSettings:ExpirationInDays` | `JwtSettings__ExpirationInDays` | `7` |
| `Groq:ApiKey` | `Groq__ApiKey` | LinkerAI |
| — | `SKIP_DB_INIT` | `true` skips migrations and seeding at startup |
| — | `ASPNETCORE_URLS` | e.g. `http://localhost:5001` |
| — | `ASPNETCORE_ENVIRONMENT` | `Development`, `Production`, or `Test` (in-memory DB, no seeding) |

**Don't commit secrets.** Keep real values in user secrets locally and in the host's environment settings in production, and leave placeholders in `appsettings.json`.

```bash
cd islink-backend/IsLink.API
dotnet user-secrets init          # adds a UserSecretsId to the .csproj, once
dotnet user-secrets set "ConnectionStrings:PostgreSQL" "Host=localhost;Database=islink;Username=postgres;Password=postgres"
dotnet user-secrets set "ConnectionStrings:MongoDB" "mongodb://localhost:27017"
dotnet user-secrets set "JwtSettings:Secret" "<at least 32 random characters>"
dotnet user-secrets set "Groq:ApiKey" "<your Groq key>"
```

Frontend: `VITE_API_URL` (default `http://localhost:5001/api`) in `islink-frontend/.env.local`.

## Local databases with Docker

```bash
docker run -d --name islink-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=islink -p 5432:5432 postgres:16
docker run -d --name islink-mongo -p 27017:27017 mongo:7
```

## Database initialization

At startup, unless `ASPNETCORE_ENVIRONMENT=Test` or `SKIP_DB_INIT=true`, `Program.cs`:

1. runs `Database.MigrateAsync()`, then
2. runs `DbSeeder.SeedAsync()`, which **deletes every review, order, transaction, gig, user skill, user language, user and category**, then inserts demo categories, users and gigs.

Failures in either step are logged and don't stop the API.

> [!CAUTION]
> Because of step 2, restarting or redeploying the API wipes real data. Set `SKIP_DB_INIT=true` on any environment whose data you want to keep, and run migrations yourself:
>
> ```bash
> dotnet tool install --global dotnet-ef
> dotnet ef database update --project islink-backend/IsLink.API
> ```

The demo users' emails and password are in `Data/DbSeeder.cs`.

## Run locally

```bash
# API
cd islink-backend/IsLink.API
dotnet run --urls http://localhost:5001        # Swagger: http://localhost:5001/swagger

# Frontend
cd islink-frontend
npm install
npm run dev                                    # http://localhost:5173
```

`islink-backend/start-with-groq.sh` starts the API on port 5001 with `SKIP_DB_INIT=true`, using the Homebrew `dotnet@8` path on macOS.

## Tests

```bash
cd islink-backend
dotnet test
```

`IsLink.API.Tests` boots the API with `WebApplicationFactory` in the `Test` environment (in-memory database) and swaps in `FakeLinkerAIService`, so the LinkerAI endpoint tests need no network. `RealLinkerAISmokeTests` is skipped unless `RUN_REAL_LINKERAI_SMOKE` is set; it still looks for a Gemini key from before the switch to Groq.

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

The image is a multi-stage build (SDK to compile, ASP.NET runtime to run) listening on port 8080.

## Deployment

**Backend on Render:** create a Web Service from the repo with root directory `islink-backend/IsLink.API` and the Docker runtime. Add the environment variables above in the Render dashboard, including `SKIP_DB_INIT=true` once real users exist.

**Frontend on Vercel:** import the repo with root directory `islink-frontend` (Vite preset, build `npm run build`, output `dist`). Set `VITE_API_URL` to `https://<your-render-service>.onrender.com/api`. `vercel.json` already rewrites all paths to the SPA.

**Databases:** Neon for PostgreSQL (use `SSL Mode=Require`), MongoDB Atlas for MongoDB. Allow Render's outbound IPs in Atlas network access.

## Troubleshooting

| Symptom | Fix |
| --- | --- |
| `Groq API key not configured` from LinkerAI | Set `Groq:ApiKey`. Check with `GET /api/linkerai/status`. |
| Frontend can't reach the API | Port mismatch: the frontend defaults to 5001, while `dotnet run` without `--urls` may pick another port. |
| CORS error from a custom domain | Add the domain to the `AllowFrontend` policy in `Program.cs`. |
| `⚠️ Database initialization warning` in logs | Migration or seed failed; check the PostgreSQL connection string. The API keeps running. |
| All my data vanished after a deploy | See [Database initialization](#database-initialization). |
