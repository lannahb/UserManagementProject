# User Management API

**ASP.NET Core Web API | .NET 8 | C# | JWT Authentication**

A RESTful User Management API developed as part of the *Back-End Development with .NET* course on Coursera.

This project demonstrates API development using ASP.NET Core, including CRUD operations, input validation, dependency injection, JWT authentication, request logging, centralized error handling, and AI-assisted development and debugging.

## Project Structure

```text
UserManagementProject/
├── UserManagementAPI/
│   ├── Controllers/
│   ├── DTOs/
│   ├── Middleware/
│   ├── Models/
│   ├── Services/
│   ├── Program.cs
│   ├── appsettings.json
│   └── UserManagementAPI.csproj
├── JwtTestTokenGenerator/
│   ├── Program.cs
│   └── JwtTestTokenGenerator.csproj
└── README.md
```

**UserManagementAPI** is the main application. **JwtTestTokenGenerator** is a separate utility for generating JWTs during local testing; it is not a production authentication service.

## Technologies Used

- C# and .NET 8
- ASP.NET Core Web API
- Swagger / OpenAPI
- JWT Bearer Authentication
- Dependency Injection
- Data Annotations and DTO Validation
- Custom ASP.NET Core Middleware
- In-Memory Data Storage
- Copilot for AI-assisted development and debugging

## Features

### User Management (CRUD)

The API supports the following operations:

| Method | Endpoint | Description | Success Response |
|---|---|---|---|
| GET | `/api/users` | Retrieve all users | 200 OK |
| GET | `/api/users/{id}` | Retrieve a user by ID | 200 OK |
| POST | `/api/users` | Create a new user | 201 Created |
| PUT | `/api/users/{id}` | Update an existing user | 204 No Content |
| DELETE | `/api/users/{id}` | Delete an existing user | 204 No Content |

All endpoints require JWT bearer authentication.

### Input Validation

The API validates incoming user data to prevent invalid records from being processed.

Validation includes:

- Required user fields
- Valid email address format
- Maximum field lengths
- Whitespace-only input checks
- Duplicate email prevention
- Validation during user creation and updates

Invalid requests return appropriate HTTP error responses.

### Middleware

The application includes:

**Request Logging Middleware**
- Logs incoming HTTP requests
- Records request methods and paths
- Logs outgoing response status codes

**Error Handling Middleware**
- Provides centralized exception handling
- Logs application errors
- Returns consistent JSON error responses
- Prevents unhandled exception details from being exposed to API consumers

**Authentication and Authorization Middleware**
- Validates JWT bearer tokens
- Verifies token signatures, issuer, audience, and expiration
- Rejects unauthorized requests with HTTP 401

### Dependency Injection

The application uses ASP.NET Core dependency injection to register `IUserService` and its implementation, `UserService`, as a singleton service.

User data is stored in memory using a thread-safe service implementation. Data resets when the application restarts.

---

## Getting Started

### Prerequisites

Install the following:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Git
- A code editor such as Visual Studio Code or Visual Studio
- OpenSSL (for generating a random signing key)

### 1. Clone the Repository

```bash
git clone https://github.com/lannahb/UserManagementProject.git
cd UserManagementProject
```

### 2. Restore Dependencies

Navigate to the API project:

```bash
cd UserManagementAPI
dotnet restore
```

### 3. Configure JWT Authentication

The API requires a signing key for JWT authentication.

Generate a random key:

```bash
openssl rand -hex 32
```

Copy the generated value.

Configure the key using .NET user secrets from the `UserManagementAPI` directory:

```bash
dotnet user-secrets set "Jwt:Key" "YOUR_GENERATED_KEY"
```

Replace `YOUR_GENERATED_KEY` with the actual generated value.

The API uses these settings from `appsettings.json`:

```json
{
  "Jwt": {
    "Issuer": "UserManagementAPI",
    "Audience": "UserManagementAPIClients"
  }
}
```

The signing key is intentionally excluded from the repository.

**Security:** Never commit signing keys, generated JWTs, or other secrets to GitHub.

### 4. Run the API

From the `UserManagementAPI` directory:

```bash
dotnet run
```

The terminal displays the address on which the application is listening.

Open Swagger UI at:

```text
http://localhost:<PORT
