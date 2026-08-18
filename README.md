# 🔗 IsLink

> Connecting Talent, Empowering Dreams Seamlessly.

---

## 1. Overview
IsLink is an all-in-one freelance marketplace platform connecting Freelancers (Sellers) with Clients (Buyers)[cite: 2, 3]. It bridges freelancers and clients across diverse service categories, streamlining interactions and transactions. The platform differentiates itself with **LinkerAI**, an intelligent AI assistant that helps users define their project requirements and matches them with the perfect services[cite: 2]. 

## 2. Key Features
*   **🤖 LinkerAI (Intelligent Matchmaker):** Leverages the Groq API (Llama-3.3-70b-versatile model) for intelligent project recommendations and requirement extraction[cite: 2, 3].
*   **🛒 Marketplace & Gigs:** Provides comprehensive service pages with pricing tiers, seller info, reviews, and full lifecycle order management (Placed -> In Progress -> Delivered -> Completed)[cite: 2].
*   **💬 Real-Time Messaging & Chat:** Ensures seamless communication with a centralized inbox linked to specific gigs, utilizing MongoDB for speed and PostgreSQL for user profile accuracy[cite: 2, 3].
*   **🔒 Secure Transactions:** Facilitates trustworthy transactions and transparent feedback, protected by stateless JWT authentication and BCrypt/Argon2 password hashing[cite: 2, 3].

## 3. Tech Stack & Architecture
IsLink features a modular architecture that supports scalable development with a clear separation of frontend and backend components.

*   **Frontend (Client-Side):** React 18, Vite, JavaScript (ES6+), Vanilla CSS3 (Zero-Dependency), React Router DOM v6, and Context API[cite: 2, 3]. Hosted on Vercel[cite: 2].
*   **Backend (Server-Side):** .NET 8 / ASP.NET Core Web API, C#, Entity Framework Core (EF Core), Swagger / OpenAPI, and NuGet[cite: 2, 3]. Hosted on Render via Docker[cite: 2].
*   **Hybrid Database Strategy:**
    *   **PostgreSQL (Neon Tech):** Used for ACID-compliant transactional data such as financials (Orders), user accounts, and relational service data[cite: 2].
    *   **MongoDB (MongoDB Atlas):** Used for flexible, high-volume, unstructured, and time-series based chat communications[cite: 2].
*   **Infrastructure:** Docker containerization utilizing a multi-stage build strategy, which uses a heavy SDK image to compile C# code and copies only the compiled files to a lightweight runtime image[cite: 2, 3].

## 4. Getting Started

### Prerequisites
This project requires the following dependencies[cite: 3]:
*   **Programming Language:** CSharp[cite: 3]
*   **Package Managers:** Npm, Nuget[cite: 3]
*   **Container Runtime:** Docker[cite: 3]

### Installation & Usage
Clone the repository and navigate to the project directory[cite: 3]:
```bash
git clone [https://github.com/hes0o/IsLink](https://github.com/hes0o/IsLink)
cd IsLink
