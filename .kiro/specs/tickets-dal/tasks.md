# Implementation Plan

- [x] 1. Create TicketStatus enum in Models project





  - Define enum with values: New, InProgress, Resolved, Closed
  - Add XML documentation comments for each enum value
  - _Requirements: 1.1, 1.5_

- [x] 2. Create Ticket model class in Models project





  - Define Ticket class with all required properties using PascalCase
  - Add properties: Id (Guid), Name, Email, Description, Summary, ImageUrl, Status (TicketStatus), Resolution, CreatedAt, UpdatedAt
  - Enable nullable reference types for proper null handling
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5_

- [x] 3. Create ITicketDal interface in DAL project




  - Add project reference from TicketSystem.DAL to TicketSystem.Models
  - Define interface with async CRUD method signatures
  - Add methods: CreateAsync, GetAllAsync, GetByIdAsync, UpdateAsync, DeleteAsync
  - Include XML documentation for each method
  - _Requirements: 2.1, 2.2, 2.3, 2.4_

- [x] 4. Implement TicketDal class with JSON serialization setup





  - Create TicketDal class implementing ITicketDal interface
  - Configure JsonSerializerOptions with camelCase naming policy
  - Add JsonStringEnumConverter for TicketStatus enum serialization
  - Set up file path management with configurable location
  - Add private helper method to ensure App_Data directory exists
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 9.1, 9.2_

- [x] 5. Implement CreateAsync method





  - Generate new GUID for ticket Id
  - Set CreatedAt and UpdatedAt to current UTC timestamp
  - Load existing tickets from JSON file
  - Add new ticket to collection
  - Serialize and save to JSON file with camelCase properties
  - Return created ticket
  - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5_
-

- [x] 6. Implement GetAllAsync method




  - Check if JSON file exists
  - Return empty collection if file doesn't exist
  - Read and deserialize JSON file
  - Return all tickets as IEnumerable
  - Handle file access exceptions gracefully
  - _Requirements: 5.1, 5.2, 5.3, 9.3_

- [x] 7. Implement GetByIdAsync method




  - Load all tickets from JSON file
  - Search for ticket matching the provided Id
  - Return matching ticket if found
  - Return null if ticket not found
  - _Requirements: 6.1, 6.2, 6.3_

- [x] 8. Implement UpdateAsync method




  - Load all tickets from JSON file
  - Find ticket by Id in collection
  - Return false if ticket not found
  - Update ticket properties with new values
  - Set UpdatedAt to current UTC timestamp
  - Serialize and save updated collection to JSON file
  - Return true on success
  - _Requirements: 7.1, 7.2, 7.3, 7.4, 7.5_
-

- [x] 9. Implement DeleteAsync method




  - Load all tickets from JSON file
  - Find ticket by Id in collection
  - Return false if ticket not found
  - Remove ticket from collection
  - Serialize and save updated collection to JSON file
  - Return true on success
  - _Requirements: 8.1, 8.2, 8.3, 8.4_
-

- [x] 10. Add error handling and file locking




  - Wrap file operations in try-catch blocks
  - Implement file locking with FileStream and FileShare settings
  - Handle IOException, UnauthorizedAccessException, JsonException
  - Add retry logic with exponential backoff for locked files
  - Ensure atomic write operations to prevent data corruption
  - _Requirements: 9.3, 9.4, 9.5_

- [x] 11. Register TicketDal in dependency injection container




  - Add project reference from TicketSystem.Server to TicketSystem.DAL
  - Register ITicketDal and TicketDal as scoped service in Program.cs
  - Configure file path from appsettings.json if needed
  - _Requirements: 3.1, 3.2, 3.3_

- [x] 12. Seed initial data from provided JSON example





  - Create tickets.json file in App_Data folder
  - Add the 5 sample tickets from the provided JSON data
  - Ensure status values use camelCase format
  - Verify file is properly formatted and readable
  - _Requirements: 3.2, 3.3_
