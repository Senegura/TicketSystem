# Requirements Document

## Introduction

This feature adds two authenticated API endpoints for ticket management operations: retrieving a single ticket by its ID and updating an existing ticket. Both endpoints will be implemented using ASP.NET Core Minimal API and will require JWT-based authentication with claims validation using the IJwtService. These endpoints enable authenticated users to view and modify individual ticket details, supporting the ticket management workflow.

## Glossary

- **TicketSystem**: The ASP.NET Core web application that provides ticket management functionality
- **Minimal API**: ASP.NET Core's lightweight approach to building HTTP APIs with minimal code
- **JWT (JSON Web Token)**: A compact, URL-safe token format used for authentication and authorization
- **IJwtService**: The service interface responsible for JWT token generation and validation operations
- **AuthToken Cookie**: An HTTP-only cookie that stores the JWT token for authentication
- **Ticket**: A support request entity with properties including Id, Name, Email, Description, Summary, ImageUrl, Status, Resolution, CreatedAt, and UpdatedAt
- **ITicketService**: The business logic service interface for ticket operations
- **Authenticated User**: A user who has successfully logged in and possesses a valid JWT token with claims
- **Claims**: Key-value pairs embedded in a JWT token that contain user identity and authorization information
- **User Type**: An enumeration indicating the role of a user (User=1, Admin=2)

## Requirements

### Requirement 1

**User Story:** As an authenticated user, I want to retrieve a specific ticket by its ID, so that I can view the detailed information about that ticket

#### Acceptance Criteria

1. WHEN an HTTP GET request is sent to "/api/tickets/{id}", THE TicketSystem SHALL extract the ticket ID from the route parameter
2. WHEN the request is received, THE TicketSystem SHALL extract the JWT token from the AuthToken cookie
3. IF the AuthToken cookie is missing or empty, THEN THE TicketSystem SHALL return HTTP 401 Unauthorized response
4. WHEN a JWT token is present, THE TicketSystem SHALL validate the token signature, issuer, audience, and expiration using the configured secret key
5. IF the JWT token validation fails, THEN THE TicketSystem SHALL return HTTP 401 Unauthorized response
6. WHEN the JWT token is valid, THE TicketSystem SHALL extract the userType claim from the token
7. IF the userType claim is missing or cannot be parsed, THEN THE TicketSystem SHALL return HTTP 403 Forbidden response
8. IF the userType value is not 1 (User) or 2 (Admin), THEN THE TicketSystem SHALL return HTTP 403 Forbidden response
9. WHEN the user is authorized, THE TicketSystem SHALL call ITicketService to retrieve the ticket by the provided ID
10. IF the ticket is not found, THEN THE TicketSystem SHALL return HTTP 404 Not Found response
11. WHEN the ticket is found, THE TicketSystem SHALL return HTTP 200 OK response with the ticket data in JSON format
12. IF an exception occurs during processing, THEN THE TicketSystem SHALL return HTTP 500 Internal Server Error response with an error message

### Requirement 2

**User Story:** As an authenticated user, I want to update an existing ticket by its ID, so that I can modify ticket details such as status, summary, or resolution

#### Acceptance Criteria

1. WHEN an HTTP PUT request is sent to "/api/tickets/{id}", THE TicketSystem SHALL extract the ticket ID from the route parameter
2. WHEN the request is received, THE TicketSystem SHALL extract the JWT token from the AuthToken cookie
3. IF the AuthToken cookie is missing or empty, THEN THE TicketSystem SHALL return HTTP 401 Unauthorized response
4. WHEN a JWT token is present, THE TicketSystem SHALL validate the token signature, issuer, audience, and expiration using the configured secret key
5. IF the JWT token validation fails, THEN THE TicketSystem SHALL return HTTP 401 Unauthorized response
6. WHEN the JWT token is valid, THE TicketSystem SHALL extract the userType claim from the token
7. IF the userType claim is missing or cannot be parsed, THEN THE TicketSystem SHALL return HTTP 403 Forbidden response
8. IF the userType value is not 1 (User) or 2 (Admin), THEN THE TicketSystem SHALL return HTTP 403 Forbidden response
9. WHEN the user is authorized, THE TicketSystem SHALL parse the request body as a Ticket object
10. IF the request body is null or cannot be parsed, THEN THE TicketSystem SHALL return HTTP 400 Bad Request response
11. IF the ticket ID in the route parameter does not match the ticket ID in the request body, THEN THE TicketSystem SHALL return HTTP 400 Bad Request response with a message indicating ID mismatch
12. WHEN the request is valid, THE TicketSystem SHALL call ITicketService to update the ticket with the provided data
13. IF the ticket is not found, THEN THE TicketSystem SHALL return HTTP 404 Not Found response
14. WHEN the ticket is updated successfully, THE TicketSystem SHALL return HTTP 200 OK response with the updated ticket data in JSON format
15. IF an exception occurs during processing, THEN THE TicketSystem SHALL return HTTP 500 Internal Server Error response with an error message
