# 🍽️ Restaurant Management System

A full-stack restaurant management application built with React, ASP.NET Core, and SQLServer. Features include dishes and category management, user authentication, shopping cart, order management.

## Features

### User Features
- 🔐 **User Authentication** - Register, login with JWT tokens stored in HTTP-only cookies
- 🔑 **Google OAuth** - Quick login with Google account
- 📧 **Email Verification** - Verify email address during registration
- 🔒 **Password Reset** - Forgot password functionality
- 🍕 **Browse Dishes** - View dishes by categories with images and prices
- 🔍 **Search & Filter** - Find dishes quickly
- 🛒 **Shopping Cart** - Add/remove items, update quantities
- 📦 **Order Management** - Place orders, view order history and track order status
- 👤 **User Profile** - Update profile information and profile picture

### Admin Features
- 🍔 **Dish Management** - Create, update, and delete dishes 
- 📑 **Category Management** - Create and soft-delete categories
- 📋 **Order Management** - View and manage all customer orders

## Tech Stack

### Frontend
- **React 19** – UI library
- **Vite** – Frontend build tool and dev server
- **Redux Toolkit & React Redux** - State management
- **React Router DOM v7** – Client-side routing
- **use-debounce** – Optimized debounced search
- **Tailwind CSS** - Styling
- **Axios** - HTTP client
- **React Hook Form** - Form validation
- **React Icons** - Icon library
- **React Google OAuth** - Google authentication
- **React Hot Toast** – User notifications

### Backend
- **ASP.NET Core 8** - Web API framework
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **JWT Authentication** – Token-based authentication
- **Role-based Authorization** – Access control (Admin/User roles)
- **Google OAuth** – External authentication support
- **SMTP (Mailtrap for development)** – Email sending (account verification & reset password)
- **Swagger** – API documentation

### DevOps
- **Docker** - Containerization
- **Docker Compose** - Multi-container orchestration

## Prerequisites

Before you begin, ensure you have the following installed:

- [Node.js](https://nodejs.org/) (v18 or higher)
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (Express or Developer Edition)
- [Git](https://git-scm.com/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (optional, for containerization)

## Getting Started
### Option 1: Run Without Docker (Recommended for Development)

#### 1. Clone the Repository

```bash
git clone https://github.com/Adityaa134/Restaurant-Project.git
cd Restaurant-Project
```

#### 2. Backend Setup


🔧 Configure appsettings.json (Local Only)

**Database Configuration - Choose One:**

**Option A: LocalDB (Recommended - Comes with Visual Studio)**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=RestaurantDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "jwt-secret-key",
    "Issuer": "https://localhost:7219",
    "Audience": "http://localhost:5173"
  },
  "SMTPConfig": {
    "Username": "your-username",
    "SenderDisplayName": "RestaurantApp",
    "SenderAddress": "no-reply@restaurant.com",
    "Port": "port-number",
    "Password": "your-password",
    "Host": "host"
  },
  "AllowedOrigins": {
    "OriginName": "http://localhost:5173"
  }
}
```

**Option B: SQL Server Express/Developer**

Requires [SQL Server installation](https://www.microsoft.com/sql-server/sql-server-downloads).
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RestaurantDB;User Id=sa;Password=YourStrongPassword123;Encrypt=False;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "jwt-secret-key",
    "Issuer": "https://localhost:7219",
    "Audience": "http://localhost:5173"
  },
  "SMTPConfig": {
    "Username": "your-username",
    "SenderDisplayName": "RestaurantApp",
    "SenderAddress": "no-reply@restaurant.com",
    "Port": "port-number",
    "Password": "your-password",
    "Host": "host"
  },
  "AllowedOrigins": {
    "OriginName": "http://localhost:5173"
  }
}
```

**⚠️ Notes:**
- Choose either Option A or Option B based on your SQL Server setup
- Replace placeholder values (passwords, SMTP credentials) with your actual values
- **Do not commit secrets in appsettings.json**
- In production, use Azure Key Vault or environment variables for secret management

✅ Uploading images to `wwwroot/images` works correctly in local mode

##### Run Migrations

```bash
cd src/Restaurent.WebAPI
dotnet restore
dotnet ef database update
dotnet run
```

#### Access the application
- Backend API: `https://localhost:7219`
- Swagger: `https://localhost:7219/swagger`

#### 3. Frontend Setup

##### Install Dependencies

```bash
cd src/frontend/RestaurentFrontend
npm install
```

### Google OAuth Setup (Required for Google Login)

This project uses Google OAuth for authentication.

To obtain a Google Client ID:

1. Go to https://console.cloud.google.com/
2. Create a new project (or select an existing one)
3. Navigate to **APIs & Services → Credentials**
4. Create an **OAuth Client ID**
5. Choose **Web Application**
6. Add the following:
   - Authorized JavaScript origin: `http://localhost:5173`
   - Authorized redirect URI: `http://localhost:5173`
7. Copy the generated **Client ID**

Create a .env file inside `src/frontend/RestaurantFrontend` and add:
```env
VITE_CLIENT_ID=your_google_client_id_here
```

##### Run Frontend Development Server

```bash
cd src/frontend/RestaurentFrontend
npm run dev
```

Frontend will run on `http://localhost:5173`

**✅ You can now access the application at `http://localhost:5173`**

### Option 2: Run With Docker

**⚠️ Note:** When running with Docker, you won't have direct access to `wwwroot/Images` folder for uploaded images. Use Option 1 for development if you need to manage uploaded files.

#### Prerequisites
- Docker Desktop installed and running

#### 1. Clone the Repository
```bash
git clone https://github.com/Adityaa134/Restaurant-Project.git
cd Restaurant-Project
```

#### 2. Backend Setup


📁 Environment Variables Setup

Create a .env file in the same folder as docker-compose.yml.

**⚠️ Note:** Docker requires SQL Server connection string. Do not use LocalDB connection string - it will cause connection errors in containers.

Example .env
```
# Database
CONNECTIONSTRINGS__DEFAULTCONNECTION=Server=sqlserver;Database=RestaurantDB;User Id=sa;Password=YourStrongPassword123;Encrypt=False;TrustServerCertificate=True
SA_PASSWORD=YourStrongPassword123

# JWT
JWT__KEY=this is a secret key for a jwt token
JWT__ISSUER=https://localhost:7219
JWT__AUDIENCE=http://localhost:5173

# SMTP
SMTPCONFIG__USERNAME=your_smtp_username
SMTPCONFIG__SENDERDISPLAYNAME=Restaurant App
SMTPCONFIG__SENDERADDRESS=no-reply@restaurant.com
SMTPCONFIG__PORT=587
SMTPCONFIG__PASSWORD=your_smtp_password
SMTPCONFIG__HOST=smtp.yourprovider.com

# CORS
ALLOWEDORIGINS__ORIGINNAME=http://localhost:5173
```

🔁 How configuration works


appsettings.json contains empty values:

```Json
"Jwt": {
  "Key": "",
  "Issuer": "",
  "Audience": ""
},
"SMTPConfig": {
    "Username": "",
    "SenderDisplayName": "",
    "SenderAddress": "",
    "Port": "",
    "Password": "",
    "Host": ""
},
"AllowedOrigins": {
    "OriginName": ""
}
```
docker-compose.yml maps environment variables:

  - ConnectionStrings__DefaultConnection=${CONNECTIONSTRINGS__DEFAULTCONNECTION}
  - Jwt__Key=${JWT__KEY}
  - Jwt__Issuer=${JWT__ISSUER}
  - Jwt__Audience=${JWT__AUDIENCE}
  - SMTPConfig__Username=${SMTPCONFIG__USERNAME}
  - SMTPConfig__SenderDisplayName=${SMTPCONFIG__SENDERDISPLAYNAME}
  - SMTPConfig__SenderAddress=${SMTPCONFIG__SENDERADDRESS}
  - SMTPConfig__Port=${SMTPCONFIG__PORT}
  - SMTPConfig__Password=${SMTPCONFIG__PASSWORD}
  - SMTPConfig__Host=${SMTPCONFIG__HOST}
  - AllowedOrigins__OriginName=${ALLOWEDORIGINS__ORIGINNAME}


Docker Compose reads values from .env.


ASP.NET Core automatically overrides appsettings.json using environment variables.

#### 3. Run with Docker Compose

```bash
# From the root directory
docker-compose up
```

This will start:
- SQL Server database on port `1433`
- Backend API on port `8080`

#### Access the application
- Backend API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`

#### Stop containers

```bash
docker-compose down

# To remove volumes (database data)
docker-compose down -v
```

## Architecture

This project follows **Clean Architecture** principles to ensure separation of concerns, testability, and maintainability.

- **Restaurent.Core**  
  Contains DTOs, entities , enums , helper classes , and services.  
  This layer does not depend on any other project.

- **Restaurent.Infrastructure**  
  Repositories, EF Core DbContext, database migrations.

- **Restaurent.WebAPI**  
  ASP.NET Core Web API layer responsible for handling HTTP requests, authentication, authorization and exposing endpoints.

The dependencies flow:
WebAPI → Infrastructure → Core

## 📂 Project Structure

```
RestaurantSolution/
├── src/
│   ├── Restaurant.WebAPI/          # ASP.NET Core Web API
│   │   ├── Controllers/            # API Controllers
|   |   ├── Middleware/             # Custom middleware
│   │   ├── StartupExtensions/      # Startup configuration extensions
│   │   ├── Template/               # Email templates 
│   │   ├── appsettings.json        # Configuration
│   │   ├── Program.cs              # Application entry point
│   │   ├── Dockerfile              # Docker configuration
│   │
│   ├── Restaurant.Core/            # Buisness Logic Layer
│   │   ├── Domain/                 # Domain models/entities and repository contracts
│   │   ├── DTO/                    # Data Transfer Objects
│   │   ├── Enums/                  # Enumerations
│   │   ├── Helpers/                # Helper classes
│   │   ├── Service/                # Business logic services
│   │   ├── ServiceContracts/       # Service interfaces
│   │   ├── CustomValidators/       # Custom validation logic
│   │
│   ├── Restaurant.Infrastructure/  # Data Access Layer
│   │   ├── DBContext/              # Entity Framework DbContext
│   │   ├── Migrations/             # Database migrations
│   │   ├── Repositories/           # Repository implementations
│   │
│   └── frontend/
│       └── RestaurantFrontend/     # React application
│           ├── src/
│           │   ├── axios/          # API configuration
│           │   ├── components/     # React components
│           │   ├── pages/          # Page components
│           │   ├── features/       # Redux slices
│           │   ├── services/       # API services
│           │   ├── store/          # Redux store
│           │   ├── App.jsx         # Root component
│           │   ├── main.jsx        # Application entry point
│           ├── index.html          # HTML template
│           ├── package.json        # NPM dependencies
│           ├── package-lock.json   # NPM lock file
│
├── docker-compose.yml              # Docker compose configuration
├── .gitignore
└── README.md
```

## API Documentation

API documentation is available via Swagger UI when running the backend:

```
https://localhost:7219/swagger
```

## API Design 

The backend API is organized into the following functional modules:

### Authentication
- User registration and login
- Google OAuth authentication
- JWT access & refresh token flow
- Secure logout

### Dishes
- Retrieve dishes and dish details
- Admin-only CRUD operations on dishes

### Categories
- Retrieve dish categories
- Admin-managed category lifecycle (with soft delete)

### Cart
- Manage user cart (add, update quantity, remove items)

### Orders
- Place orders from cart
- View order history and order details
- Admin order status management

### Addresses
- Add, edit, and manage delivery addresses

### Sample API Endpoints
- `POST /api/Account/login`
- `GET /api/Dishes`
- `POST /api/Address`
- `GET /api/Address/{id}`
- `POST /api/Orders`

Full request/response details are available via Swagger.

`https://localhost:7219/swagger`
or 
`http://localhost:8080/swagger` (with docker)

## Author

**Aditya Gupta**

- GitHub: [@Adityaa134](https://github.com/Adityaa134)
- LinkedIn: [Aditya Gupta](https://www.linkedin.com/in/aditya-gupta-897a3028a)
- Email: adityagupta9966@gmail.com


⭐ **If you like this project, please give it a star!** ⭐