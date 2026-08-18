# 🔗 IsLink

> Connecting Talent, Empowering Dreams Seamlessly.

---

## 1. Overview
IsLink is an all-in-one freelance marketplace platform connecting Freelancers (Sellers) with Clients (Buyers). It bridges freelancers and clients across diverse service categories, streamlining interactions and transactions. The platform differentiates itself with **LinkerAI**, an intelligent AI assistant that helps users define their project requirements and matches them with the perfect services. 

## 2. Key Features
*   **🤖 LinkerAI (Intelligent Matchmaker):** Leverages the Groq API (Llama-3.3-70b-versatile model) for intelligent project recommendations and requirement extraction.
*   **🛒 Marketplace & Gigs:** Provides comprehensive service pages with pricing tiers, seller info, reviews, and full lifecycle order management (Placed -> In Progress -> Delivered -> Completed).
*   **💬 Real-Time Messaging & Chat:** Ensures seamless communication with a centralized inbox linked to specific gigs, utilizing MongoDB for speed and PostgreSQL for user profile accuracy.
*   **🔒 Secure Transactions:** Facilitates trustworthy transactions and transparent feedback, protected by stateless JWT authentication and BCrypt/Argon2 password hashing.

## 3. Tech Stack & Architecture
IsLink features a modular architecture that supports scalable development with a clear separation of frontend and backend components.

*   **Frontend (Client-Side):** React 18, Vite, JavaScript (ES6+), Vanilla CSS3 (Zero-Dependency), React Router DOM v6, and Context API. Hosted on Vercel.
*   **Backend (Server-Side):** .NET 8 / ASP.NET Core Web API, C#, Entity Framework Core (EF Core), Swagger / OpenAPI, and NuGet. Hosted on Render via Docker.
*   **Hybrid Database Strategy:**
    *   **PostgreSQL (Neon Tech):** Used for ACID-compliant transactional data such as financials (Orders), user accounts, and relational service data.
    *   **MongoDB (MongoDB Atlas):** Used for flexible, high-volume, unstructured, and time-series based chat communications.
*   **Infrastructure:** Docker containerization utilizing a multi-stage build strategy, which uses a heavy SDK image to compile C# code and copies only the compiled files to a lightweight runtime image.

## 4. Getting Started

### Prerequisites
This project requires the following dependencies:
*   **Programming Language:** CSharp
*   **Package Managers:** Npm, Nuget
*   **Container Runtime:** Docker

### Installation & Usage
Clone the repository and navigate to the project directory:
```bash
git clone [https://github.com/hes0o/IsLink](https://github.com/hes0o/IsLink)
cd IsLink
