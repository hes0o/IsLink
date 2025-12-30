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
*   **AI Provider**: Groq API (Llama 3 Model)

### Infrastructure & Deployment
*   **Containerization**: [Docker](https://www.docker.com/) (Standardized Runtime Environment)
*   **Hosting**:
    *   **Backend**: Render (deployed via Docker)
    *   **Frontend**: Vercel (Static Site Hosting)
*   **Databases**:
    *   **PostgreSQL**: Neon Tech (Serverless SQL)
    *   **MongoDB**: MongoDB Atlas (Cloud NoSQL)

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

### 🤖 LinkerAI (Intelligent Matchmaker)
An AI-powered concierge that guides users to the right services.
*   **Model**: **Llama-3.3-70b-versatile** (via Groq API).
*   **Why Groq?**: Chosen for its ultra-low latency, making the chat feel instant and conversational.
*   **Logic**:
    1.  **System Prompt**: Instructs the AI to act as a project manager.
    2.  **Requirement Extraction**: Analyzes user text for "Budget", "Deadline", and "Project Type".
    3.  **Semantic Search**: Matches extracted keywords against the Gig database.

### 🛒 Marketplace & Gigs
*   **Search & Discovery**: Advanced filtering by category, price, and rating.
*   **Gig Details**: Comprehensive service pages with pricing tiers, seller info, and reviews.
*   **Order System**: Full lifecycle management (Placed -> In Progress -> Delivered -> Completed).

### 💬 Real-Time Messaging & Chat
*   **Inbox**: A centralized hub for all conversations.
*   **Context Aware**: Chats are linked to specific Gigs ("Related Gig" tag), giving sellers context immediately.
*   **Hybrid Backend**: Messages stored in MongoDB for speed, User profiles fetched from Postgres for accuracy.

---

## 5. Deployment & DevOps

### Docker Integration
The backend is packaged using **Docker** to ensure consistency across environments.
*   **Purpose**: "Build once, run anywhere." It bundles the .NET code, libraries, and runtime into a single "Create" (Image).
*   **Dockerfile Strategy**: Uses a "Multi-Stage Build":
    1.  **Build Stage**: Uses the heavy SDK image to compile the C# code.
    2.  **Runtime Stage**: Copies *only* the compiled files to a lightweight Runtime image, keeping the final deployment small and secure.

---

## 6. Database Schema Overview

### PostgreSQL Entities (Core)
*   **User**: `Id`, `Username`, `Email`, `PasswordHash`, `Role`, `Balance`
*   **Gig**: `Id`, `Title`, `Description`, `Price`, `SellerId`, `CategoryId`
*   **Order**: `Id`, `Status` (Pending/Process/Done), `Amount`, `BuyerId`, `SellerId`
*   **Review**: `Id`, `Rating` (1-5), `Comment`, `OrderId`

### MongoDB Collections (Chat)
*   **Conversations**: `_id`, `Participants[]`, `LastMessage`, `UpdatedAt`
*   **Messages**: `_id`, `ConversationId`, `SenderId`, `Content`, `Timestamp`

---

## 7. Security Measures
*   **JWT Authentication**: Stateless, secure token-based auth.
*   **Password Hashing**: BCrypt/Argon2 hashing for user passwords.
*   **CORS Policy**: Strict Cross-Origin policies allowing only trusted domains (Vercel Frontend).
*   **Environment Variables**: Sensitive keys (DB Strings, API Keys) managed via `.env` and Cloud Configs.
