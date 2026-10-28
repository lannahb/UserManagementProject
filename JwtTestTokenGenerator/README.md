# JWT Test Token Generator

This console application is a supporting utility for the [User Management API](../README.md) project.

It generates JSON Web Tokens (JWTs) for testing protected API endpoints through Swagger or other HTTP clients.

## Purpose

The User Management API uses JWT bearer authentication. This utility generates temporary testing tokens with:

- HMAC-SHA256 signing
- Issuer: `UserManagementAPI`
- Audience: `UserManagementAPIClients`
- Test user claims
- 30-minute token expiration

This utility is intended for **local development and testing only**. It is not a production authentication service.

## How to Run

1. Generate a signing key if you haven't already:

   ```bash
   openssl rand -hex 32
   ```

2. Set the `JWT_KEY` environment variable to the same key configured as `Jwt:Key` in the main API.

   macOS/Linux:

   ```bash
   export JWT_KEY="YOUR_GENERATED_KEY"
   ```

   Windows PowerShell:

   ```powershell
   $env:JWT_KEY="YOUR_GENERATED_KEY"
   ```

3. From the `JwtTestTokenGenerator` directory, run:

   ```bash
   dotnet run
   ```

4. Copy the generated JWT.

5. Open the main API's Swagger UI, click **Authorize**, and paste the entire token without the `Bearer` prefix.

## Important Notes

- The signing key must match the one configured in the main API.
- Tokens expire after 30 minutes.
- Never commit signing keys or generated tokens to GitHub.
- The generator does not create user accounts or provide a login endpoint.

For full project setup and API testing instructions, see the [root README](../README.md).
