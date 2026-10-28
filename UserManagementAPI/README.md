### Running the API locally

**Prerequisites**
- .NET 8 SDK
- A terminal
- Swagger UI in a web browser

**Setup**

1. Clone the repository.
2. Navigate to the `UserManagementAPI` project directory.
3. Restore dependencies:

   ```bash
   dotnet restore
   ```

4. Generate a local JWT signing key:

   ```bash
   openssl rand -hex 32
   ```

5. Configure the API's signing key using .NET user secrets:

   ```bash
   dotnet user-secrets set "Jwt:Key" "YOUR_GENERATED_KEY"
   ```

6. Start the API:

   ```bash
   dotnet run
   ```

7. Open Swagger at the address shown by the application, adding `/swagger`.

### Generating an authentication token

The API uses JWT bearer authentication. Protected endpoints require a valid token.

1. Set the `JWT_KEY` environment variable to the same signing key configured for the API.

   macOS/Linux:

   ```bash
   export JWT_KEY="YOUR_GENERATED_KEY"
   ```

   Windows PowerShell:

   ```powershell
   $env:JWT_KEY="YOUR_GENERATED_KEY"
   ```

2. Navigate to the `JwtTestTokenGenerator` project directory.
3. Run:

   ```bash
   dotnet run
   ```

4. Copy the entire generated JWT.
5. In Swagger, click **Authorize**, paste the token without the `Bearer` prefix, and confirm.
6. Execute the API endpoints.

Tokens expire after 30 minutes. Generate a new token if authentication returns `401 Unauthorized`.

### Testing the API

- `GET /api/users` — Retrieve all users.
- `GET /api/users/{id}` — Retrieve a user by ID.
- `POST /api/users` — Create a user.
- `PUT /api/users/{id}` — Update a user.
- `DELETE /api/users/{id}` — Delete a user.

The API also supports input validation, duplicate-email handling, request logging, and centralized error handling.

**Note:** Data is stored in memory and resets when the API restarts. No database setup is required.

## API Requests and Expected Responses

The User Management API provides CRUD operations for managing users. All endpoints require JWT bearer authentication.

### Authentication

Before testing the endpoints:

1. Start the API using `dotnet run`.
2. Generate a valid JWT using the `JwtTestTokenGenerator` project.
3. Open Swagger UI at the application's `/swagger` endpoint.
4. Click **Authorize** and enter the complete JWT without the `Bearer` prefix.
5. Execute the endpoints below.

For direct HTTP requests, include:

```http
Authorization: Bearer YOUR_JWT_TOKEN
```

JWTs expire after 30 minutes. Generate a new token when necessary.

---

### 1. GET /api/users — Retrieve All Users

**Request**

```http
GET /api/users
Authorization: Bearer YOUR_JWT_TOKEN
```

**Expected response — 200 OK**

When no users have been created:

```json
[]
```

After users have been added:

```json
[
  {
    "id": 1,
    "firstName": "Jane",
    "lastName": "Doe",
    "email": "jane.doe@example.com",
    "department": "Engineering"
  },
  {
    "id": 2,
    "firstName": "John",
    "lastName": "Smith",
    "email": "john.smith@example.com",
    "department": "IT"
  }
]
```

### 2. GET /api/users/{id} — Retrieve a User by ID

**Request**

```http
GET /api/users/1
Authorization: Bearer YOUR_JWT_TOKEN
```

**Expected response — 200 OK**

```json
{
  "id": 1,
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane.doe@example.com",
  "department": "Engineering"
}
```

**User not found — 404 Not Found**

```http
GET /api/users/999
```

The API returns HTTP 404 when the requested user does not exist. The response body depends on the controller's implementation.

### 3. POST /api/users — Create a User

**Request**

```http
POST /api/users
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json
```

**Request body**

```json
{
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane.doe@example.com",
  "department": "Engineering"
}
```

**Expected response — 201 Created**

```json
{
  "id": 1,
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane.doe@example.com",
  "department": "Engineering"
}
```

The response also includes a `Location` header identifying the newly created resource.

User IDs are generated automatically. The exact ID depends on the users already created during the current application session.

### 4. PUT /api/users/{id} — Update a User

**Request**

```http
PUT /api/users/1
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json
```

**Request body**

```json
{
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane.doe@example.com",
  "department": "Product Development"
}
```

**Expected response — 204 No Content**

The API successfully updates the user and returns no response body.

To verify the update, execute:

```http
GET /api/users/1
```

**Expected response — 200 OK**

```json
{
  "id": 1,
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane.doe@example.com",
  "department": "Product Development"
}
```

### 5. DELETE /api/users/{id} — Delete a User

**Request**

```http
DELETE /api/users/1
Authorization: Bearer YOUR_JWT_TOKEN
```

**Expected response — 204 No Content**

The user is deleted successfully.

To verify deletion, execute:

```http
GET /api/users/1
```

**Expected response — 404 Not Found**

The deleted user can no longer be retrieved.

---

## Validation and Error Handling Tests

The API uses DTO validation, service-level checks, and centralized exception handling to reject invalid user data.

### Test 1: Missing Required Fields

**Request**

```http
POST /api/users
Content-Type: application/json
Authorization: Bearer YOUR_JWT_TOKEN
```

**Request body**

```json
{
  "firstName": "",
  "lastName": "Doe",
  "email": "jane.doe@example.com",
  "department": "Engineering"
}
```

**Expected response — 400 Bad Request**

The API rejects the request because `firstName` is required and cannot be empty.

ASP.NET Core returns a validation error response describing the invalid field.

### Test 2: Invalid Email Address

**Request body**

```json
{
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "invalid-email",
  "department": "Engineering"
}
```

**Expected response — 400 Bad Request**

The API rejects the request because the email address does not satisfy the configured email validation rules.

### Test 3: Duplicate Email Address

First, create a user with:

```json
{
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane.doe@example.com",
  "department": "Engineering"
}
```

Then attempt to create another user:

```json
{
  "firstName": "Janet",
  "lastName": "Smith",
  "email": "jane.doe@example.com",
  "department": "IT"
}
```

**Expected response — 409 Conflict**

The API rejects the second request because another user already has that email address.

### Test 4: Unauthorized Request

**Request without JWT**

```http
GET /api/users
```

**Expected response — 401 Unauthorized**

The API rejects the request because no valid bearer token was provided.

A typical response includes:

```http
HTTP/1.1 401 Unauthorized
WWW-Authenticate: Bearer
```

### Test 5: Expired or Invalid JWT

**Request**

```http
GET /api/users
Authorization: Bearer INVALID_OR_EXPIRED_TOKEN
```

**Expected response — 401 Unauthorized**

The API rejects expired tokens and tokens that fail signature, issuer, or audience validation.

---

## Middleware Testing

The application includes custom request logging and centralized error-handling middleware, along with ASP.NET Core JWT authentication and authorization middleware.

### Request Logging

**Test procedure**

1. Start the API with `dotnet run`.
2. Execute `GET /api/users` through Swagger.
3. Observe the application terminal.

**Expected behavior**

The logging middleware records information about the HTTP request and response, including the request method, path, and response status code.

The exact log message format depends on the implementation of `RequestLoggingMiddleware`.

### Centralized Error Handling

**Test procedure**

Trigger an exception that reaches the `ErrorHandlingMiddleware`.

**Expected behavior**

The middleware logs the exception and returns an appropriate HTTP error response instead of exposing an unhandled exception to the client.

The API uses standardized error responses for supported exception types.

**Note:** Ordinary DTO validation failures may be handled automatically by ASP.NET Core before reaching the custom exception handler.

---

## Additional Testing Notes

- **In-memory storage:** User data is stored in memory and is reset whenever the API restarts.
- **Automatic IDs:** User IDs are assigned by the service and may differ from the examples.
- **JWT expiration:** Generated testing tokens expire after 30 minutes.
- **Security:** JWT signing keys must be configured locally and should never be committed to GitHub.
- **Swagger:** Swagger UI supports JWT authorization through the **Authorize** button.
- **HTTP status codes:** Successful CRUD operations return `200`, `201`, or `204`; invalid or unauthorized operations return appropriate `4xx` responses.

### AI-Assisted Development and Debugging

Throughout this project, I used Copilot as an AI coding assistant to fulfill the AI-assisted development, debugging, and middleware implementation objectives outlined in the course activities. AI assistance was used to help structure and implement the CRUD endpoints, improve input validation, troubleshoot compilation and runtime errors, configure dependency injection, and implement request logging, centralized exception handling, and JWT authentication. I also used AI to understand error messages, evaluate suggested solutions, and troubleshoot issues involving package dependencies, port conflicts, and JWT token validation. I reviewed the generated code, applied the necessary changes, and independently built, executed, and tested the API using Swagger to verify its functionality. 