# Requirements Document

## Introduction

This feature implements a secure ASP.NET Core Minimal API endpoint that retrieves all tickets from the system. The endpoint enforces authentication and authorization, ensuring only authenticated users with User or Admin roles can access the ticket list. The endpoint returns ticket data in JSON format.

## Glossary

- **TicketSystem**: The ASP.NET Core web application that manages support tickets
- **Minimal API Endpoint**: A lightweight HTTP endpoint defined using ASP.NET Core's minimal API approach
- **JWT Token**: JSON Web Token used for authentication, containing user claims
- **AuthToken Cookie**: HTTP-only cookie containing the JWT token
- **User Role**: A user type with value 1 (UserType.User) that can manage tickets
- **Admin Role**: A user type with value 2 (UserType.Admin) with full system access
- **ITicketService**: Business logic service interface for ticket operations
- **Ticket Collection**: A list of all tickets stored in the system

## Requirements

### Requirement 1

**User Story:** As a User or Admin, I want to retrieve all tickets from the system, so that I can view and manage support requests

#### Acceptance Criteria

1. WHEN a GET request is sent to "/api/tickets", THE TicketSystem SHALL invoke the ITicketService to retrieve all tickets
2. WHEN the ITicketService returns ticket data, THE TicketSystem SHALL serialize the tickets to JSON format
3. WHEN ticket retrieval is successful, THE TicketSystem SHALL return HTTP 200 status with the ticket collection in the response body
4. IF the ITicketService throws an exception during retrieval, THEN THE TicketSystem SHALL return HTTP 500 status with an error message
5. WHEN the ticket collection is empty, THE TicketSystem SHALL return HTTP 200 status with an empty JSON array

### Requirement 2

**User Story:** As a system administrator, I want the endpoint to validate authentication, so that only logged-in users can access ticket data

#### Acceptance Criteria

1. WHEN a request is received at "/api/tickets", THE TicketSystem SHALL extract the JWT token from the AuthToken cookie
2. IF the AuthToken cookie is not present in the request, THEN THE TicketSystem SHALL return HTTP 401 status
3. WHEN the JWT token is present, THE TicketSystem SHALL validate the token signature and expiration
4. IF the JWT token is invalid or expired, THEN THE TicketSystem SHALL return HTTP 401 status
5. WHEN the JWT token is valid, THE TicketSystem SHALL extract the userId and userType claims from the token

### Requirement 3

**User Story:** As a system administrator, I want the endpoint to enforce role-based authorization, so that only Users and Admins can access ticket data

#### Acceptance Criteria

1. WHEN the JWT token is validated, THE TicketSystem SHALL extract the userType claim value
2. IF the userType claim value equals 1 (User Role), THEN THE TicketSystem SHALL allow access to the endpoint
3. IF the userType claim value equals 2 (Admin Role), THEN THE TicketSystem SHALL allow access to the endpoint
4. IF the userType claim value equals 0 or any other value, THEN THE TicketSystem SHALL return HTTP 403 status
5. IF the userType claim is missing from the token, THEN THE TicketSystem SHALL return HTTP 403 status
