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
- ChatGPT for AI-assisted development and debugging

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
http://localhost:<PORT>/swagger
```

Replace `<PORT>` with the port shown in the terminal.

---

## Generating a JWT for Testing

Because the API uses JWT authentication, protected endpoints require a valid bearer token.

The repository includes a separate `JwtTestTokenGenerator` utility for local testing.

### 1. Configure the Generator

Open another terminal and navigate to the generator directory:

```bash
cd JwtTestTokenGenerator
```

Set the `JWT_KEY` environment variable to the **same key** used in the API configuration.

**macOS / Linux:**

```bash
export JWT_KEY="YOUR_GENERATED_KEY"
```

**Windows PowerShell:**

```powershell
$env:JWT_KEY="YOUR_GENERATED_KEY"
```

### 2. Generate the Token

Run:

```bash
dotnet run
```

The program outputs a JWT.

The generated token includes the configured issuer, audience, user claims, and a 30-minute expiration.

### 3. Authorize in Swagger

1. Open the API's Swagger UI.
2. Click **Authorize**.
3. Paste the complete JWT without the `Bearer` prefix.
4. Confirm authorization.
5. Execute the API endpoints.

Swagger automatically includes the token in authenticated requests.

If you receive `401 Unauthorized`, verify that the signing keys match and that the token has not expired.

---

## Example API Requests and Responses

All examples assume a valid JWT has been supplied through Swagger or the `Authorization` header.

### Create a User

**POST `/api/users`**

Request body:

```json
{
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane.doe@example.com",
  "department": "Engineering"
}
```

Expected response: **201 Created**

```json
{
  "id": 1,
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane.doe@example.com",
  "department": "Engineering"
}
```

### Retrieve All Users

**GET `/api/users`**

Expected response: **200 OK**

```json
[
  {
    "id": 1,
    "firstName": "Jane",
    "lastName": "Doe",
    "email": "jane.doe@example.com",
    "department": "Engineering"
  }
]
```

If no users have been created, the API returns an empty array.

### Retrieve a User by ID

**GET `/api/users/1`**

Expected response: **200 OK**

```json
{
  "id": 1,
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane.doe@example.com",
  "department": "Engineering"
}
```

If the user does not exist, the API returns **404 Not Found**.

### Update a User

**PUT `/api/users/1`**

Request body:

```json
{
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane.doe@example.com",
  "department": "Product Development"
}
```

Expected response: **204 No Content**

The user is updated successfully. A subsequent GET request returns the updated information.

### Delete a User

**DELETE `/api/users/1`**

Expected response: **204 No Content**

A subsequent GET request for the deleted user returns **404 Not Found**.

---

## Validation and Authentication Test Cases

| Test Case | Expected HTTP Status |
|---|---|
| Retrieve existing users with a valid JWT | 200 OK |
| Retrieve a nonexistent user | 404 Not Found |
| Create a valid user | 201 Created |
| Update an existing user | 204 No Content |
| Delete an existing user | 204 No Content |
| Submit missing or invalid required fields | 400 Bad Request |
| Submit an invalid email address | 400 Bad Request |
| Attempt to create a duplicate email | 409 Conflict |
| Request a protected endpoint without a JWT | 401 Unauthorized |
| Request a protected endpoint with an invalid or expired JWT | 401 Unauthorized |

### Testing Request Logging

Run the API and execute requests through Swagger. The application terminal displays logs generated by the request logging middleware.

### Testing Error Handling

The centralized error-handling middleware catches unhandled exceptions and returns appropriate JSON error responses. Input validation errors may also be handled directly by ASP.NET Core's model validation system.

---

## AI-Assisted Development and Debugging

Throughout this project, I used ChatGPT as an AI coding assistant to fulfill the AI-assisted development, debugging, and middleware implementation objectives outlined in the course activities, which originally referenced Microsoft Copilot.

AI assistance was used to help structure and implement CRUD endpoints, improve input validation, troubleshoot compilation and runtime errors, configure dependency injection, and implement request logging, centralized exception handling, and JWT authentication.

I also used AI to understand error messages, evaluate suggested solutions, and troubleshoot issues involving package dependencies, port conflicts, and JWT token validation.

I reviewed the generated code, applied the necessary changes, and independently built, executed, and tested the API using Swagger to verify its functionality.

Although ChatGPT was used instead of Microsoft Copilot, the work followed the intended AI-assisted development and debugging process described in the project instructions.

---

## Course Assignment Requirements

This project was developed to demonstrate the following learning objectives:

| Requirement | Implementation |
|---|---|
| GitHub repository | Public repository containing project source code |
| CRUD operations | GET, POST, PUT, and DELETE user endpoints |
| AI-assisted debugging | ChatGPT-assisted troubleshooting and code improvements |
| Input validation | DTO validation, email checks, and duplicate prevention |
| Middleware | Logging, exception handling, JWT authentication, and authorization |

## Limitations and Future Improvements

This is an educational project intended to demonstrate backend API development.

Current limitations include:

- In-memory storage rather than a persistent database
- No user registration or login endpoint
- JWTs generated using a separate local testing utility
- No production deployment configuration

Potential future improvements include Entity Framework Core database persistence, automated integration tests, user authentication with secure credential storage, and role-based authorization.

## Author

Developed as part of the Coursera *Back-End Development with .NET* course.

GitHub: [lannahb](https://github.com/lannahb)
