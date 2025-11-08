# Design Document

## Overview

The server-login feature implements a secure authentication endpoint using ASP.NET Core Minimal API. The endpoint accepts user credentials (username and password), validates them through the existing IUserService, generates a signed JWT token containing user claims, and returns the token both in the response body and as an HTTP-only cookie.

This design leverages the existing authentication infrastructure (IUserService, IJwtService) and follows the project's layered architecture pattern where the API layer orchestrates business logic services.

## Architecture

### Component Interaction Flow

```
Client Request (POST /api/auth/login)
    ↓
Login Minimal API Endpoint
    ↓
IUserService.LoginAsync() → Validates credentials
    ↓
LoginResult (Success/Failure)
    ↓
[If Success] IJwtService.SignToken() → Generates JWT
    ↓
Response + Cookie
```

### Layer Responsibilities

- **API Layer (Minimal API Endpoint)**: Request validation, orchestration, response formatting, cookie management
- **Business Logic Layer (IUserService)**: Credential validation, user authentication
- **Business Logic Layer (IJwtService)**: JWT token generation and signing
- **Models Layer**: DTOs for request/response data

## Components and Interfaces

### 1. Login Request DTO

A new model class to represent the login request payload:

```csharp
namespace TicketSystem.Models;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
```

**Note**: The requirements mention "email and password" but the existing UserLogin model uses "Username". We'll use Username to maintain consistency with the existing codebase, as the username field can accept email addresses.

### 2. Login Response DTO

A new model class to represent the successful login response:

```csharp
namespace TicketSystem.Models;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public UserType UserType { get; set; }
}
```

### 3. Minimal API Endpoint

The login endpoint will be registered in Program.cs using Minimal API syntax:

```csharp
app.MapPost("/api/auth/login", async (
    LoginRequest request,
    IUserService userService,
    IJwtService jwtService,
    HttpContext httpContext) =>
{
    // Implementation details in next section
});
```

### 4. Cookie Configuration

Add cookie configuration to appsettings.json:

```json
{
  "Authentication": {
    "CookieName": "AuthToken",
    "ExpirationMinutes": 1440
  }
}
```

## Data Models

### Input Model: LoginRequest
- **Username**: string - The user's username (required, non-empty)
- **Password**: string - The user's password (required, non-empty)

### Output Model: LoginResponse
- **Token**: string - The signed JWT token
- **UserId**: int - The authenticated user's ID
- **UserType**: UserType - The user's type (enum)

### JWT Claims Structure
The JWT payload will contain the following claims:
- **userId**: string representation of the user's ID
- **userType**: string representation of the UserType enum value
- **sub** (subject): username
- **iat** (issued at): timestamp
- **exp** (expiration): timestamp

## Error Handling

### Validation Errors (400 Bad Request)
- Missing or empty username
- Missing or empty password
- Invalid JSON format

Response format:
```json
{
  "error": "Username and password are required"
}
```

### Authentication Errors (401 Unauthorized)
- Invalid credentials
- User not found

Response format:
```json
{
  "error": "Invalid username or password"
}
```

### Server Errors (500 Internal Server Error)
- JWT signing failures
- Unexpected exceptions

Response format:
```json
{
  "error": "An error occurred during authentication"
}
```

## Implementation Notes

### Cookie Configuration

The authentication cookie will be configured with the following security settings:

```csharp
var cookieOptions = new CookieOptions
{
    HttpOnly = true,        // Prevents JavaScript access
    Secure = true,          // HTTPS only
    SameSite = SameSiteMode.Strict,  // CSRF protection
    Expires = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes)
};
```

### Endpoint Implementation Logic

1. **Request Validation**
   - Check if LoginRequest is null
   - Validate Username and Password are not empty
   - Return 400 if validation fails

2. **Authentication**
   - Create UserLogin object from LoginRequest
   - Call IUserService.LoginAsync()
   - Check LoginResult.Success

3. **Token Generation** (if authentication succeeds)
   - Create claims array with userId and userType
   - Call IJwtService.SignToken() with claims and 1440 minutes expiration (24 hours)
   - Get expiration time from configuration

4. **Response Construction**
   - Create LoginResponse with token and user info
   - Set HTTP-only cookie with token
   - Return 200 OK with LoginResponse

5. **Error Handling**
   - Return 401 for authentication failures
   - Return 500 for unexpected errors
   - Log errors appropriately

### Service Registration

IJwtService needs to be registered in Program.cs if not already registered:

```csharp
builder.Services.AddSingleton<IJwtService, JwtService>();
```

The IJwtService implementation handles all JWT signing configuration internally.

### Endpoint Registration Location

The login endpoint should be registered in Program.cs after the service registrations and before `app.Run()`, grouped with other API endpoints for maintainability.

## Security Considerations

1. **HTTPS Only**: The Secure flag on cookies ensures tokens are only transmitted over HTTPS
2. **HttpOnly Cookies**: Prevents XSS attacks by making cookies inaccessible to JavaScript
3. **SameSite Protection**: Mitigates CSRF attacks
4. **Token Expiration**: Tokens expire after configured duration (default 1440 minutes / 24 hours)
5. **Password Validation**: Delegated to IUserService which uses secure hashing
6. **Error Messages**: Generic error messages to prevent username enumeration

## Configuration Requirements

The following configuration should be added to appsettings.json for cookie management:

```json
{
  "Authentication": {
    "CookieName": "AuthToken",
    "ExpirationMinutes": 1440
  }
}
```

For development, appsettings.Development.json can override with different values. JWT signing is handled by IJwtService which manages its own configuration. The default expiration is set to 1440 minutes (24 hours).
