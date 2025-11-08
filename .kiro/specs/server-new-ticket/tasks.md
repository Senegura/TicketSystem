# Implementation Plan

- [x] 1. Create business logic layer components





  - Create ITicketService interface with CreateTicketAsync method
  - Create TicketService implementation that uses ITicketDal to persist tickets
  - Set default values for new tickets (Status, empty Summary/Resolution)
  - _Requirements: 1.1, 4.1, 4.2_

- [x] 2. Create request DTO model





  - Create CreateTicketRequest class in TicketSystem.Models with FullName, Email, and IssueDescription properties
  - Add XML documentation comments for all properties
  - _Requirements: 1.1_
-

- [x] 3. Register TicketService in dependency injection




  - Add ITicketService registration to Program.cs service collection
  - Use singleton lifetime to match existing service patterns
  - _Requirements: 1.1_
-

- [x] 4. Implement file upload validation helper




  - Create inline validation logic for file size (max 5MB)
  - Create inline validation logic for file type (JPEG, PNG, GIF) checking both content type and extension
  - _Requirements: 2.2, 2.3, 2.4_

- [x] 5. Implement file storage logic




  - Create logic to ensure App_Data/uploads directory exists
  - Generate unique filename using Guid.NewGuid() with preserved extension
  - Implement file saving using FileStream
  - _Requirements: 3.1, 3.2, 3.3, 3.4_
-

- [x] 6. Create minimal API endpoint for ticket submission




  - Add POST /api/tickets endpoint in Program.cs
  - Accept multipart/form-data with fullName, email, issueDescription, and optional image fields
  - Inject ITicketService dependency into endpoint
  - _Requirements: 1.1, 2.1, 4.5_

- [x] 7. Implement request validation in endpoint




  - Validate fullName is not null/empty and has at least 2 characters
  - Validate email is not null/empty and matches email format regex pattern
  - Validate issueDescription is not null/empty and has at least 10 characters
  - Return HTTP 400 with structured error details for validation failures
  - _Requirements: 1.2, 1.3, 1.4, 1.5, 4.3_

- [x] 8. Integrate file upload handling in endpoint





  - Call file validation helper for uploaded image (if present)
  - Call file storage logic to save validated image
  - Handle file upload errors and return HTTP 400 for validation failures
  - Pass image filename to TicketService.CreateTicketAsync
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5_
-

- [x] 9. Implement success and error responses




  - Return HTTP 201 with created ticket data on success (with or without image)
  - Return HTTP 500 with error message for server errors during file storage
  - Ensure CORS headers are applied via existing AllowFrontend policy
  - Add try-catch block for exception handling
  - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5_
