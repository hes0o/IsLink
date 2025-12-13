# IsLink Backend API (.NET 8)

ASP.NET Core Web API backend for the IsLink freelance marketplace platform.

## 🛠️ Tech Stack

- **Framework:** ASP.NET Core 8.0
- **ORM:** Entity Framework Core 8.0
- **Databases:**
  - PostgreSQL (Neon) - Relational data
  - MongoDB (Atlas) - Messages/Notifications
- **Authentication:** JWT Bearer Tokens
- **Documentation:** Swagger/OpenAPI

## 📁 Project Structure

```
IsLink.API/
├── Configuration/       # App configuration classes
├── Controllers/         # API endpoints
│   ├── AuthController.cs
│   ├── UsersController.cs
│   ├── GigsController.cs
│   ├── CategoriesController.cs
│   ├── OrdersController.cs
│   ├── ReviewsController.cs
│   ├── MessagesController.cs
│   └── NotificationsController.cs
├── Data/
│   └── ApplicationDbContext.cs  # EF Core DbContext
├── Middleware/
│   └── ExceptionMiddleware.cs
├── Models/
│   ├── DTOs/            # Data Transfer Objects
│   ├── Entities/        # EF Core Entities (PostgreSQL)
│   └── MongoDB/         # MongoDB Document Models
├── Services/            # Business Logic
├── Program.cs           # Entry point
├── appsettings.json     # Configuration
└── IsLink.API.csproj    # Project file
```

## 📊 Database Schema

### PostgreSQL Tables (15 tables)
- `users` - User accounts
- `user_skills` - Many-to-many user skills
- `user_languages` - User language proficiencies
- `categories` - Service categories (self-referencing)
- `gigs` - Service listings
- `gig_packages` - Pricing packages (basic/standard/premium)
- `package_features` - Package feature list
- `gig_images` - Gig gallery
- `gig_tags` - Searchable tags
- `orders` - Transactions
- `order_deliveries` - Delivery submissions
- `delivery_attachments` - Delivery files
- `reviews` - Customer reviews
- `favorites` - Saved gigs
- `transactions` - Payment history

### MongoDB Collections
- `conversations` - Chat threads
- `messages` - Individual messages
- `notifications` - User notifications
- `chathistories` - AI chatbot conversations

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- PostgreSQL (or Neon account)
- MongoDB (or Atlas account)

### 1. Clone and Navigate
```bash
cd islink-backend/IsLink.API
```

### 2. Configure Database Connections

Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=your-host;Database=islink;Username=user;Password=pass;SSL Mode=Require",
    "MongoDB": "mongodb+srv://user:pass@cluster.mongodb.net/islink"
  },
  "JwtSettings": {
    "Secret": "your-secret-key-at-least-32-characters-long",
    "Issuer": "IsLink.API",
    "Audience": "IsLink.Client",
    "ExpirationInDays": 7
  }
}
```

### 3. Run Migrations
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Run the API
```bash
dotnet run
```

The API will be available at:
- **API:** http://localhost:5000
- **Swagger:** http://localhost:5000/swagger

## 📡 API Endpoints

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login |
| POST | `/api/auth/logout` | Logout |
| GET | `/api/auth/me` | Get current user |

### Users
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users/sellers` | List sellers |
| GET | `/api/users/{username}` | Get user profile |
| PUT | `/api/users/profile` | Update profile |

### Gigs
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/gigs` | List gigs (with filters) |
| GET | `/api/gigs/{slug}` | Get gig details |
| POST | `/api/gigs` | Create gig |
| GET | `/api/gigs/seller/{username}` | Get seller's gigs |

### Categories
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/categories` | List all categories |
| GET | `/api/categories/{slug}` | Get category |

### Orders
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/orders` | Create order |
| GET | `/api/orders` | List user's orders |
| GET | `/api/orders/{id}` | Get order details |
| PATCH | `/api/orders/{id}/status` | Update order status |

### Reviews
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/reviews/gig/{gigId}` | Get gig reviews |
| GET | `/api/reviews/seller/{username}` | Get seller reviews |
| POST | `/api/reviews` | Create review |
| POST | `/api/reviews/{id}/response` | Add seller response |

### Messages
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/messages/conversations` | List conversations |
| POST | `/api/messages/conversations` | Start conversation |
| GET | `/api/messages/conversations/{id}` | Get messages |
| POST | `/api/messages/conversations/{id}` | Send message |

### Notifications
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/notifications` | Get notifications |
| GET | `/api/notifications/unread-count` | Get unread count |
| POST | `/api/notifications/mark-all-read` | Mark all as read |

## 💰 Currency

All prices are in **Syrian Lira (SYP)**.

## 🔐 Authentication

Use JWT Bearer token in Authorization header:
```
Authorization: Bearer <your-token>
```

## 📝 License

MIT
