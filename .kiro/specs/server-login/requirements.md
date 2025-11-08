# Requirements Document

## Introduction

This feature implements a secure server-side login endpoint using ASP.NET Core Minimal API. The endpoint authenticates users by validating their credentials through the IUserService, generates a JWT payload with user claims, and returns both the token and sets it as an HTTP-only cookie for secure session management.

## Glossary

- **Login Endpoint**: The HTTP API endpoint that accepts user credentials and returns authentication tokens
- **IUserService**: The business logic service interface responsible for user authentication and validation
- **JWT Payload**: A JSON Web Token containing signed claims about the authenticated user
- **User Claims**: Assertions about the user identity including userId and UserType
- **Private Key**: The cryptographic key used to sign JWT tokens
- **HTTP-only Cookie**: A secure cookie that cannot be accessed via JavaScript, used to store the authentication token

## Requirements

### Requirement 1

**User Story:** As a client application, I want to send user credentials to a login endpoint, so that I can authenticate users and receive a secure token.

#### Acceptance Criteria

1. THE Login Endpoint SHALL accept HTTP POST requests with email and password in the request body
2. WHEN the request body is missing email or password, THE Login Endpoint SHALL return a 400 Bad Request response
3. THE Login Endpoint SHALL use IUserService to validate the provided credentials
4. WHEN credentials are invalid, THE Login Endpoint SHALL return a 401 Unauthorized response
5. WHEN credentials are valid, THE Login Endpoint SHALL return a 200 OK response with the JWT payload

### Requirement 2

**User Story:** As a security administrator, I want JWT tokens to contain specific user claims, so that the application can identify and authorize users properly.

#### Acceptance Criteria

1. WHEN a user successfully authenticates, THE Login Endpoint SHALL generate a JWT payload containing a userId claim
2. WHEN a user successfully authenticates, THE Login Endpoint SHALL generate a JWT payload containing a UserType claim
3. THE Login Endpoint SHALL use IJwtService to generate and sign the JWT payload with a private key

### Requirement 3

**User Story:** As a client application, I want to receive the authentication token in multiple formats, so that I can use it for subsequent API requests.

#### Acceptance Criteria

1. WHEN authentication succeeds, THE Login Endpoint SHALL return the JWT token in the response body
2. WHEN authentication succeeds, THE Login Endpoint SHALL set an HTTP-only cookie containing the JWT token
3. THE Login Endpoint SHALL configure the authentication cookie with the Secure flag enabled
4. THE Login Endpoint SHALL configure the authentication cookie with the SameSite attribute set to Strict or Lax

### Requirement 4

**User Story:** As a developer, I want the login endpoint to follow ASP.NET Core Minimal API conventions, so that it integrates seamlessly with the existing application architecture.

#### Acceptance Criteria

1. THE Login Endpoint SHALL be implemented using ASP.NET Core Minimal API syntax
2. THE Login Endpoint SHALL be registered in the application's endpoint routing configuration
3. THE Login Endpoint SHALL use dependency injection to access IUserService and IJwtService
4. THE Login Endpoint SHALL be accessible at the route "/api/auth/login" or similar RESTful path
