# IsLink Project Documentation

## 1. Executive Summary
**IsLink** is a modern freelance marketplace platform connecting Freelancers (Sellers) with Clients (Buyers). It differentiates itself with **LinkerAI**, an intelligent AI assistant that helps users define their project requirements and matches them with the perfect services.

The platform relies on a **Hybrid Database Architecture**, leveraging the strengths of both Relational (PostgreSQL) and NoSQL (MongoDB) databases to ensure data integrity for transactions while maintaining high performance for real-time communications.

---

## 2. Technology Stack

### Frontend (Client-Side)
*   **Framework**: [React 18](https://react.dev/)
*   **Build Tool**: [Vite](https://vitejs.dev/) (Fast HMR & Bundling)
*   **Language**: JavaScript (ES6+)
*   **Styling**: Vanilla CSS3 with Custom Design Variables (Zero-Dependency)
*   **Routing**: React Router DOM v6
*   **State Management**: React Context API (AuthContext)
*   **HTTP Client**: Fetch API (Abstraction Layer)

### Backend (Server-Side)
*   **Framework**: [.NET 8 / ASP.NET Core Web API](https://dotnet.microsoft.com/)
*   **Language**: C#
*   **ORM**: Entity Framework Core (EF Core)
*   **Authentication**: JWT (JSON Web Tokens)
*   **API Documentation**: Swagger / OpenAPI

### Infrastructure & Database
*   **Primary Database**: [PostgreSQL (Neon)](https://neon.tech/) - *Users, Orders, Gigs*
*   **Messaging Database**: [MongoDB](https://www.mongodb.com/) - *Real-time Chat Logs*
*   **Hosting**: Render (Backend), Vercel (Frontend)

---

## 3. System Architecture

### 3.1. Hybrid Database Strategy
IsLink uses a polyglot persistence pattern to optimize for different data types:

| Component | Database | Justification |
| :--- | :--- | :--- |
| **Transactional Data** | **PostgreSQL** | Requires ACID compliance. Used for Financials (Orders), User Accounts, and relational Service data (Categories -> Gigs). |
| **Communication** | **MongoDB** | Requires high write throughput and flexibility. Chat messages are unstructured, high-volume, and time-series based. |

### 3.2. Frontend Architecture
The frontend follows a **Service-Based Architecture** to ensure clean separation of concerns:
*   **`src/pages`**: View logic (UI).
*   **`src/components`**: Reusable UI blocks (Header, Footer, Cards).
*   **`src/services/api.js`**: Centralized API abstraction layer. All HTTP calls are defined here, decoupling UI from backend endpoints.
*   **`src/context`**: Global state (User Session) accessible anywhere in the app.

---

## 4. Key Features

### 🤖 LinkerAI
An AI-powered concierge that guides users.
*   **Function**: Users describe their needs in natural language (e.g., "I need a logo for a coffee shop").
*   **Logic**: The AI analyzes the intent and recommends specific Categories or Gigs.
*   **UX**: Features a ChatGPT-like streaming interface with "Starter Chips" for quick engagement.

### 🛒 Marketplace & Gigs
*   **Search & Discovery**: Advanced filtering by category, price, and rating.
*   **Gig Details**: Comprehensive service pages with pricing tiers, seller info, and reviews.
*   **Order System**: Full lifecycle management (Placed -> In Progress -> Delivered -> Completed).

### 💬 Real-Time Messaging
*   **Inbox**: A centralized hub for all conversations.
*   **Context Aware**: Chats are linked to specific Gigs ("Related Gig" tag), giving sellers context immediately.
*   **Hybrid Backend**: Messages stored in MongoDB for speed, User profiles fetched from Postgres for accuracy.

---

## 5. Database Schema Overview

### PostgreSQL Entities (Core)
*   **User**: `Id`, `Username`, `Email`, `PasswordHash`, `Role`, `Balance`
*   **Gig**: `Id`, `Title`, `Description`, `Price`, `SellerId`, `CategoryId`
*   **Order**: `Id`, `Status` (Pending/Process/Done), `Amount`, `BuyerId`, `SellerId`
*   **Review**: `Id`, `Rating` (1-5), `Comment`, `OrderId`

### MongoDB Collections (Chat)
*   **Conversations**: `_id`, `Participants[]`, `LastMessage`, `UpdatedAt`
*   **Messages**: `_id`, `ConversationId`, `SenderId`, `Content`, `Timestamp`

---

## 6. Design System
IsLink implements a custom design system focused on **Visual Hierarchy** and **Modern Aesthetics**.

*   **Colors**:
    *   Primary: `Emerald Green (#1dbf73)` - Representative of growth and money.
    *   Neutral: `Slate Gray (#0f172a)` - Professional, high-contrast text.
*   **Typography**:
    *   Headings: *Plus Jakarta Sans* (Geometric, Modern)
    *   Body: *Inter* (Clean, Readable)
*   **Components**:
    *   Glassmorphism Headers
    *   Soft Shadow Cards
    *   Fluid Responsive Layouts (Mobile -> Desktop)

---

## 7. Security Measures
*   **JWT Authentication**: Stateless, secure token-based auth.
*   **Password Hashing**: BCrypt/Argon2 hashing for user passwords.
*   **CORS Policy**: Strict Cross-Origin policies allowing only trusted domains (Vercel Frontend).
*   **Environment Variables**: Sensitive keys (DB Strings, API Keys) managed via `.env` and Cloud Configs.
