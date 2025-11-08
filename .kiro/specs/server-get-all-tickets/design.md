# Design Document

## Overview

This feature implements a secure GET endpoint at `/api/tickets` that retrieves all tickets from the system. The endpoint uses JWT-based authentication and role-based authorization to ensure only authenticated Users (UserType.User) and Admins (UserType.Admin) can access the ticket list. The implementation follows the existing minimal API pattern in Program.cs and leverages the existing ITicketService business logic layer.

## Architecture

The endpoint follows the existing three-tier architecture:

1. **API Layer (Program.cs)**: Minimal API endpoint that handles HTTP requests, authentication, authorization, and response formatting
2. **Business Logic Layer (ITicketService)**: Service interface that will be extended with a GetAllTicketsAsync method
3. **Data Access Layer (ITicketDal)**: Already has GetAllAsync method for retrieving tickets from JSON storage

### Request Flow

```
HTTP GET /api/tickets
    ↓
Extract JWT from AuthToken cookie
    ↓
Validate JWT token (signature, expiration)
    ↓
Extract userType claim
    ↓
Authorize (userType == 1 or 2)
    ↓
Call ITicketService.GetAllTicketsAsync()
    ↓
Return HTTP 200 with JSON array of tickets
```

## Components and Interfaces

### 1. ITicketService Extension

Add a new method to the ITicketService interface:

```csharp
Task<IEnumerable<Ticket>> GetAllTicketsAsync();
```

This method will:
- Call ITicketDal.GetAllAsync() to retrieve tickets
- Return the ticket collection
- Allow exceptions to propagate to the API layer for error handling

### 2. TicketService Implementation

Implement the new method in TicketService class:

```csharp
public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
{
    return await _ticketDal.GetAllAsync();
}
```

### 3. Minimal API Endpoint

Add a new GET endpoint in Program.cs after the existing endpoints:

**Endpoint Signature:**
```csharp
app.MapGet("/api/tickets", async (HttpContext httpContext, ITicketService ticketService, IConfiguration configuration) => { ... })
```

**Authentication Logic:**
- Extract JWT token from "AuthToken" cookie (configurable via Authentication:CookieName)
- Use System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler to validate token
- Validate token signature using the same secret key from JwtService (SecretKey constant)
- Validate token expiration (NotBefore and Expires claims)
- Extract claims from validated token

**Authorization Logic:**
- Extract "userType" claim from JWT token
- Parse claim value to integer
- Allow access if userType == 1 (User) or userType == 2 (Admin)
- Return HTTP 403 for userType == 0 (Customer) or missing claim

**Response Handling:**
- Call ticketService.GetAllTicketsAsync()
- Return HTTP 200 with JSON serialized ticket array
- ASP.NET Core automatically serializes IEnumerable<Ticket> to JSON with camelCase

## Data Models

Uses existing Ticket model from TicketSystem.Models:

```csharp
public class Ticket
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Description { get; set; }
    public string Summary { get; set; }
    public string ImageUrl { get; set; }
    public TicketStatus Status { get; set; }
    public string Resolution { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

**Response Format:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "John Doe",
    "email": "john@example.com",
    "description": "Issue description",
    "summary": "",
    "imageUrl": "abc123.jpg",
    "status": "new",
    "resolution": "",
    "createdAt": "2025-11-08T10:30:00Z",
    "updatedAt": "2025-11-08T10:30:00Z"
  }
]
```

## Error Handling

### HTTP 401 Unauthorized
- AuthToken cookie is missing
- JWT token is invalid (malformed, wrong signature)
- JWT token is expired

**Response:**
```csharp
Results.Unauthorized()
```

### HTTP 403 Forbidden
- userType claim is missing
- userType value is 0 (Customer)
- userType value is not 1 or 2

**Response:**
```csharp
Results.Forbid()
```

### HTTP 500 Internal Server Error
- ITicketService.GetAllTicketsAsync() throws an exception
- Unexpected errors during token validation

**Response:**
```csharp
Results.Problem("An error occurred while retrieving tickets", statusCode: 500)
```

### HTTP 200 OK (Empty Collection)
- No tickets exist in the system
- Return empty JSON array: `[]`

## Implementation Notes

### JWT Validation

The endpoint needs to manually validate JWT tokens since ASP.NET Core authentication middleware is not configured. The validation logic should:

1. Extract token from cookie
2. Create TokenValidationParameters with:
   - ValidateIssuerSigningKey = true
   - IssuerSigningKey = SymmetricSecurityKey from JwtService.SecretKey
   - ValidateIssuer = true, ValidIssuer = "TicketSystem"
   - ValidateAudience = true, ValidAudience = "TicketSystemUsers"
   - ValidateLifetime = true
   - ClockSkew = TimeSpan.Zero (no tolerance for expired tokens)
3. Use JwtSecurityTokenHandler.ValidateToken() to validate
4. Extract ClaimsPrincipal from validated token

### Secret Key Access

The JwtService.SecretKey is a private constant. Options:
1. Duplicate the secret key in Program.cs (not ideal but simple)
2. Move secret key to appsettings.json and read via IConfiguration
3. Add a ValidateToken method to IJwtService interface

**Recommended:** Option 2 - Move secret key to configuration for consistency and future flexibility.

### CORS Configuration

The endpoint should use the existing "AllowFrontend" CORS policy by adding:
```csharp
.RequireCors("AllowFrontend");
```

### Performance Considerations

- ITicketDal.GetAllAsync() reads from JSON file with file locking and retry logic
- For large ticket collections, consider pagination in future iterations
- Current implementation loads all tickets into memory (acceptable for MVP)
