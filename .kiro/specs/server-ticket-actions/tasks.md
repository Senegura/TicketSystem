# Implementation Plan

- [x] 1. Extend ITicketService interface with new methods





  - Add GetTicketByIdAsync method signature that accepts Guid id and returns Task<Ticket?>
  - Add UpdateTicketAsync method signature that accepts Ticket object and returns Task<Ticket?>
  - _Requirements: 1.9, 2.12_
- [x] 2. Implement new methods in TicketService class




- [ ] 2. Implement new methods in TicketService class

  - [x] 2.1 Implement GetTicketByIdAsync method


    - Call _ticketDal.GetByIdAsync with the provided id
    - Return the ticket result (may be null if not found)
    - _Requirements: 1.9_
  
  - [x] 2.2 Implement UpdateTicketAsync method

    - Set ticket.UpdatedAt to DateTime.UtcNow
    - Call _ticketDal.UpdateAsync with the ticket object
    - Return the updated ticket if successful, null if not found
    - _Requirements: 2.12_

- [x] 3. Create authentication helper method in Program.cs




  - [x] 3.1 Implement ValidateAuthenticationAsync helper method


    - Accept HttpContext and IConfiguration as parameters
    - Extract JWT token from AuthToken cookie
    - Return (false, Results.Unauthorized()) if token is missing or empty
    - Validate token using JwtSecurityTokenHandler with configured secret key
    - Return (false, Results.Unauthorized()) if token validation fails
    - Extract userType claim from validated token
    - Return (false, Results.Forbid()) if userType claim is missing or cannot be parsed
    - Return (false, Results.Forbid()) if userType is not 1 or 2
    - Return (true, null) if authentication and authorization succeed
    - _Requirements: 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 2.2, 2.3, 2.4, 2.5, 2.6, 2.7, 2.8_
-

- [x] 4. Implement GET /api/tickets/{id} endpoint




  - [x] 4.1 Register MapGet endpoint with route "/api/tickets/{id}"


    - Accept Guid id as route parameter
    - Inject HttpContext, ITicketService, and IConfiguration as dependencies
    - _Requirements: 1.1_
  
  - [x] 4.2 Add authentication and authorization logic

    - Call ValidateAuthenticationAsync helper method
    - Return error result if authentication fails
    - _Requirements: 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8_
  
  - [x] 4.3 Implement ticket retrieval logic

    - Call ticketService.GetTicketByIdAsync with the id parameter
    - Return HTTP 404 Not Found if ticket is null
    - Return HTTP 200 OK with ticket data if found
    - _Requirements: 1.9, 1.10, 1.11_
  
  - [x] 4.4 Add error handling with try-catch block

    - Wrap logic in try-catch
    - Log exception to console error stream
    - Return HTTP 500 Internal Server Error on exception
    - _Requirements: 1.12_
  
  - [x] 4.5 Add CORS policy to endpoint

    - Call .RequireCors("AllowFrontend") on the endpoint
    - _Requirements: 1.1_

- [x] 5. Implement PUT /api/tickets/{id} endpoint





  - [x] 5.1 Register MapPut endpoint with route "/api/tickets/{id}"


    - Accept Guid id as route parameter
    - Accept Ticket object from request body
    - Inject HttpContext, ITicketService, and IConfiguration as dependencies
    - _Requirements: 2.1_
  

  - [ ] 5.2 Add authentication and authorization logic
    - Call ValidateAuthenticationAsync helper method
    - Return error result if authentication fails
    - _Requirements: 2.2, 2.3, 2.4, 2.5, 2.6, 2.7, 2.8_

  
  - [ ] 5.3 Implement request validation logic
    - Return HTTP 400 Bad Request if ticket object is null
    - Return HTTP 400 Bad Request with ID mismatch message if route id does not match ticket.Id

    - _Requirements: 2.10, 2.11_
  
  - [ ] 5.4 Implement ticket update logic
    - Call ticketService.UpdateTicketAsync with the ticket object
    - Return HTTP 404 Not Found if result is null

    - Return HTTP 200 OK with updated ticket data if successful
    - _Requirements: 2.12, 2.13, 2.14_
  
  - [ ] 5.5 Add error handling with try-catch block
    - Wrap logic in try-catch

    - Log exception to console error stream
    - Return HTTP 500 Internal Server Error on exception
    - _Requirements: 2.15_
  
  - [ ] 5.6 Add CORS policy to endpoint
    - Call .RequireCors("AllowFrontend") on the endpoint
    - _Requirements: 2.1_
