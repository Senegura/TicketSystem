# Implementation Plan

- [x] 1. Extend ITicketService interface with GetAllTicketsAsync method




  - Add Task<IEnumerable<Ticket>> GetAllTicketsAsync() method signature to ITicketService.cs
  - Add XML documentation comments describing the method's purpose and return value
  - _Requirements: 1.1_
-

- [x] 2. Implement GetAllTicketsAsync in TicketService class




  - Add GetAllTicketsAsync method implementation in TicketService.cs
  - Call _ticketDal.GetAllAsync() and return the result
  - Allow exceptions to propagate to the API layer
  - _Requirements: 1.1_

- [x] 3. Move JWT secret key to configuration





  - Add "Authentication:SecretKey" setting to appsettings.json with the value from JwtService.SecretKey
  - Update JwtService.cs to read secret key from IConfiguration instead of using hard-coded constant
  - Inject IConfiguration into JwtService constructor
  - _Requirements: 2.3, 2.4_
-

- [x] 4. Implement GET /api/tickets minimal API endpoint with authentication and authorization



- [x] 4.1 Create endpoint structure and extract JWT token from cookie






  - Add app.MapGet("/api/tickets", ...) endpoint in Program.cs after the POST /api/tickets endpoint
  - Inject HttpContext, ITicketService, and IConfiguration parameters
  - Extract JWT token from AuthToken cookie (use configuration key "Authentication:CookieName" with default "AuthToken")
  - Return HTTP 401 if cookie is missing
  - _Requirements: 2.1, 2.2_
-

- [x] 4.2 Implement JWT token validation logic





  - Create TokenValidationParameters with secret key from configuration
  - Set ValidateIssuerSigningKey = true, ValidIssuer = "TicketSystem", ValidAudience = "TicketSystemUsers"
  - Set ValidateLifetime = true and ClockSkew = TimeSpan.Zero
  - Use JwtSecurityTokenHandler.ValidateToken() to validate the token
  - Return HTTP 401 if token is invalid or expired
  - Extract ClaimsPrincipal from validated token
  - _Requirements: 2.3, 2.4, 2.5_

- [x] 4.3 Implement role-based authorization logic

  - Extract "userType" claim from the validated token's claims
  - Return HTTP 403 if userType claim is missing
  - Parse userType claim value to integer
  - Return HTTP 403 if userType is 0 (Customer) or any value other than 1 or 2
  - Allow access if userType is 1 (User) or 2 (Admin)
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5_

- [x] 4.4 Implement ticket retrieval and response handling

  - Call ticketService.GetAllTicketsAsync() to retrieve all tickets
  - Return HTTP 200 with the ticket collection (ASP.NET Core will serialize to JSON automatically)
  - Handle empty collections by returning HTTP 200 with empty JSON array
  - _Requirements: 1.1, 1.2, 1.3, 1.5_

- [x] 4.5 Implement error handling for exceptions

  - Wrap endpoint logic in try-catch block
  - Catch exceptions from ITicketService and return HTTP 500 with error message
  - Log exceptions to console (Console.Error.WriteLine)
  - Return Results.Problem("An error occurred while retrieving tickets", statusCode: 500)
  - _Requirements: 1.4_

- [x] 4.6 Add CORS policy to endpoint

  - Add .RequireCors("AllowFrontend") to the endpoint
  - Ensure endpoint follows the same CORS pattern as existing endpoints
  - _Requirements: 1.1_
