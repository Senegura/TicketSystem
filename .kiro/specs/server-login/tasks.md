# Implementation Plan

- [x] 1. Create DTOs for login request and response





  - Create LoginRequest model in TicketSystem.Models with Username and Password properties
  - Create LoginResponse model in TicketSystem.Models with Token, UserId, and UserType properties
  - _Requirements: 1.1, 3.1_
-

- [x] 2. Add authentication configuration to appsettings.json




  - Add Authentication section with CookieName and ExpirationMinutes (1440) settings
  - _Requirements: 3.2, 3.3, 3.4_
- [x] 3. Register IJwtService in dependency injection container




- [ ] 3. Register IJwtService in dependency injection container

  - Add IJwtService and JwtService registration in Program.cs if not already present
  - Verify service is available for injection
  - _Requirements: 2.4_

- [x] 4. Implement the login minimal API endpoint





  - [x] 4.1 Create the POST endpoint at /api/auth/login in Program.cs


    - Map the endpoint using MapPost with LoginRequest parameter
    - Inject IUserService, IJwtService, and HttpContext dependencies
    - _Requirements: 4.1, 4.2, 4.3, 4.4_
  
  - [x] 4.2 Implement request validation logic

    - Validate LoginRequest is not null
    - Validate Username and Password are not empty
    - Return 400 Bad Request with error message if validation fails
    - _Requirements: 1.2_
  
  - [x] 4.3 Implement authentication logic

    - Create UserLogin object from LoginRequest
    - Call IUserService.LoginAsync() with credentials
    - Check LoginResult.Success property
    - Return 401 Unauthorized with error message if authentication fails
    - _Requirements: 1.3, 1.4_
  
  - [x] 4.4 Implement JWT token generation

    - Create claims collection with userId (as string) and userType (as string) claims
    - Call IJwtService.SignToken() with claims and 1440 minutes expiration
    - _Requirements: 2.1, 2.2, 2.3_
  
  - [x] 4.5 Implement response with token and cookie

    - Create LoginResponse with token, userId, and userType
    - Configure CookieOptions with HttpOnly=true, Secure=true, SameSite=Strict
    - Set cookie expiration to 1440 minutes from current time
    - Add cookie to HttpContext.Response using configured cookie name
    - Return 200 OK with LoginResponse
    - _Requirements: 3.1, 3.2, 3.3, 3.4, 1.5_
  

  - [ ] 4.6 Add error handling for unexpected exceptions
    - Wrap endpoint logic in try-catch block
    - Return 500 Internal Server Error with generic error message
    - Log exceptions appropriately
    - _Requirements: 1.5_
