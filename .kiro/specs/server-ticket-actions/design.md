# Design Document

## Overview

This feature implements two authenticated Minimal API endpoints for ticket management: GET and PUT operations for individual tickets. Both endpoints follow the existing authentication pattern used in the `/api/tickets` GET all endpoint, utilizing JWT token validation from cookies and role-based authorization. The implementation will extend the ITicketService interface with new methods and leverage the existing ITicketDal methods (GetByIdAsync and UpdateAsync) for data access.

## Architecture

### Endpoint Structure

Both endpoints will be registered in `Program.cs` using the Minimal API pattern with `app.MapGet()` and `app.MapPut()`. They will follow the established authentication and authorization flow:

1. Extract JWT token from AuthToken cookie
2. Validate token using configured secret key and validation parameters
3. Extract and validate userType claim (must be 1 or 2)
4. Execute business logic via ITicketService
5. Return appropriate HTTP response

### Service Layer Extension

The `ITicketService` interface will be extended with two new methods:
- `Task<Ticket?> GetTicketByIdAsync(Guid id)` - Retrieves a single ticket
- `Task<Ticket?> UpdateTicketAsync(Ticket ticket)` - Updates an existing ticket

The `TicketService` implementation will delegate to the existing `ITicketDal` methods (`GetByIdAsync` and `UpdateAsync`).

### Authentication Flow

Both endpoints will reuse the authentication logic from the existing `/api/tickets` GET endpoint:
- Token extraction from cookie
- JWT validation with TokenValidationParameters
- Claims extraction and validation
- Role-based authorization (User=1 or Admin=2)

## Components and Interfaces

### 1. ITicketService Extension

**New Methods:**
```csharp
Task<Ticket?> GetTicketByIdAsync(Guid id);
Task<Ticket?> UpdateTicketAsync(Ticket ticket);
```

**Purpose:** Provide business logic layer methods for single ticket retrieval and update operations.

### 2. TicketService Implementation

**GetTicketByIdAsync Implementation:**
- Accepts a Guid id parameter
- Calls `_ticketDal.GetByIdAsync(id)`
- Returns the ticket or null if not found

**UpdateTicketAsync Implementation:**
- Accepts a Ticket object
- Sets the UpdatedAt timestamp to current UTC time
- Calls `_ticketDal.UpdateAsync(ticket)`
- Returns the updated ticket if successful, null if not found

### 3. Minimal API Endpoints

**GET /api/tickets/{id}**
- Route parameter: `Guid id`
- Dependencies: `HttpContext`, `ITicketService`, `IConfiguration`
- Authentication: JWT token from AuthToken cookie
- Authorization: userType claim must be 1 or 2
- Success response: HTTP 200 with ticket JSON
- Error responses: 401 (unauthorized), 403 (forbidden), 404 (not found), 500 (server error)

**PUT /api/tickets/{id}**
- Route parameter: `Guid id`
- Request body: `Ticket` object
- Dependencies: `HttpContext`, `ITicketService`, `IConfiguration`
- Authentication: JWT token from AuthToken cookie
- Authorization: userType claim must be 1 or 2
- Validation: Route ID must match request body ID
- Success response: HTTP 200 with updated ticket JSON
- Error responses: 400 (bad request), 401 (unauthorized), 403 (forbidden), 404 (not found), 500 (server error)

## Data Models

### Ticket Model (Existing)

The existing `Ticket` model will be used without modifications:
- `Guid Id` - Unique identifier
- `string Name` - Customer name
- `string Email` - Customer email
- `string Description` - Issue description
- `string Summary` - Brief summary
- `string ImageUrl` - Image file path
- `TicketStatus Status` - Current status (enum)
- `string Resolution` - Resolution details
- `DateTime CreatedAt` - Creation timestamp
- `DateTime UpdatedAt` - Last update timestamp

### Request/Response Flow

**GET /api/tickets/{id}:**
- Input: Guid id from route
- Output: Ticket object or error response

**PUT /api/tickets/{id}:**
- Input: Guid id from route + Ticket object from body
- Output: Updated Ticket object or error response

## Error Handling

### Authentication Errors

**Missing or Invalid Token:**
- Condition: AuthToken cookie is missing, empty, or token validation fails
- Response: HTTP 401 Unauthorized
- No error details exposed to client for security

**Insufficient Permissions:**
- Condition: userType claim is missing, invalid, or not 1 or 2
- Response: HTTP 403 Forbidden
- No error details exposed to client for security

### Validation Errors

**Invalid Request Body (PUT only):**
- Condition: Request body is null or cannot be parsed as Ticket
- Response: HTTP 400 Bad Request with error message

**ID Mismatch (PUT only):**
- Condition: Route parameter ID does not match request body ID
- Response: HTTP 400 Bad Request with message "Ticket ID in URL does not match ticket ID in request body"

### Business Logic Errors

**Ticket Not Found:**
- Condition: ITicketService returns null (ticket does not exist)
- Response: HTTP 404 Not Found

### System Errors

**Unexpected Exceptions:**
- Condition: Any unhandled exception during processing
- Response: HTTP 500 Internal Server Error with generic error message
- Logging: Exception details written to console error stream

## Implementation Notes

### Code Reuse

The authentication and authorization logic will be extracted into a reusable helper method to avoid duplication across the three authenticated endpoints (GET all, GET by ID, PUT by ID). This helper method will:
- Accept HttpContext and IConfiguration
- Return a tuple of (bool isAuthorized, IResult? errorResult)
- Handle token extraction, validation, and claims checking

### CORS Configuration

Both endpoints will use `.RequireCors("AllowFrontend")` to match the existing CORS policy configuration.

### Configuration Dependencies

Both endpoints rely on the following configuration values:
- `Authentication:CookieName` (default: "AuthToken")
- `Authentication:SecretKey` (required for JWT validation)
- JWT validation parameters: Issuer="TicketSystem", Audience="TicketSystemUsers"

### UpdatedAt Timestamp

The UpdateTicketAsync method in TicketService will automatically set the UpdatedAt property to DateTime.UtcNow before calling the DAL, ensuring the timestamp is always current regardless of what the client sends.
