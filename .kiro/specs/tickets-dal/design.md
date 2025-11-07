# Design Document: Tickets DAL

## Overview

This design implements a JSON file-based data access layer for the TicketSystem application. The solution follows the repository pattern with a clear separation between the domain model (TicketSystem.Models), data access interface and implementation (TicketSystem.DAL), and provides CRUD operations for ticket entities stored in a JSON file.

The design leverages .NET 8's built-in JSON serialization capabilities with System.Text.Json, configured to handle PascalCase C# properties while serializing to camelCase JSON format to match the provided data structure.

## Architecture

### Layered Architecture

```
┌─────────────────────────────────┐
│   TicketSystem.Server (API)    │
│   - Controllers                 │
└────────────┬────────────────────┘
             │
             ▼
┌─────────────────────────────────┐
│   TicketSystem.BL (Business)    │
│   - Business Logic              │
└────────────┬────────────────────┘
             │
             ▼
┌─────────────────────────────────┐
│   TicketSystem.DAL (Data)       │
│   - ITicketDal (Interface)      │
│   - TicketDal (Implementation)  │
└────────────┬────────────────────┘
             │
             ▼
┌─────────────────────────────────┐
│   TicketSystem.Models           │
│   - Ticket (Domain Model)       │
└─────────────────────────────────┘
             │
             ▼
┌─────────────────────────────────┐
│   File System                   │
│   - App_Data/tickets.json       │
└─────────────────────────────────┘
```

### Project Dependencies

- **TicketSystem.DAL** → references → **TicketSystem.Models**
- **TicketSystem.BL** → references → **TicketSystem.DAL** and **TicketSystem.Models**
- **TicketSystem.Server** → references → **TicketSystem.BL**

## Components and Interfaces

### 1. Ticket Model (TicketSystem.Models)

**File:** `TicketSystem.Models/Ticket.cs`

**Purpose:** Domain entity representing a support ticket

**Properties:**
- `Id` (Guid) - Unique identifier
- `Name` (string) - Customer name
- `Email` (string) - Customer email
- `Description` (string) - Detailed issue description
- `Summary` (string) - Brief issue summary
- `ImageUrl` (string) - Path to uploaded image
- `Status` (TicketStatus enum) - Ticket status (New, InProgress, Resolved, Closed)
- `Resolution` (string) - Resolution details
- `CreatedAt` (DateTime) - Creation timestamp (UTC)
- `UpdatedAt` (DateTime) - Last update timestamp (UTC)

**Design Decisions:**
- All properties use PascalCase following C# conventions
- Nullable reference types enabled for proper null handling
- DateTime properties store UTC timestamps for consistency
- Status uses TicketStatus enum for type safety and validation

### 2. TicketStatus Enum (TicketSystem.Models)

**File:** `TicketSystem.Models/TicketStatus.cs`

**Purpose:** Enumeration of valid ticket statuses

**Values:**
- `New` = 0 - Newly created ticket
- `InProgress` = 1 - Ticket being worked on
- `Resolved` = 2 - Issue resolved, awaiting closure
- `Closed` = 3 - Ticket closed

**JSON Serialization:**
- Enum serialized as string in JSON (e.g., "New", "InProgress")
- Uses `JsonStringEnumConverter` with camelCase naming policy
- JSON values: "new", "inProgress", "resolved", "closed"

**Design Decisions:**
- Enum provides compile-time type safety
- String serialization maintains readability in JSON file
- CamelCase in JSON matches the provided data format
- Numeric values allow for future ordering/filtering

### 3. ITicketDal Interface (TicketSystem.DAL)

**File:** `TicketSystem.DAL/ITicketDal.cs`

**Purpose:** Contract defining data access operations for tickets

**Methods:**
```csharp
Task<Ticket> CreateAsync(Ticket ticket);
Task<IEnumerable<Ticket>> GetAllAsync();
Task<Ticket?> GetByIdAsync(Guid id);
Task<bool> UpdateAsync(Ticket ticket);
Task<bool> DeleteAsync(Guid id);
```

**Design Decisions:**
- All methods are asynchronous to support scalability
- GetByIdAsync returns nullable Ticket for not-found scenarios
- Update and Delete return bool to indicate success/failure
- Interface allows for future implementations (database, cloud storage)

### 4. TicketDal Implementation (TicketSystem.DAL)

**File:** `TicketSystem.DAL/TicketDal.cs`

**Purpose:** JSON file-based implementation of ITicketDal

**Key Responsibilities:**
- Read/write tickets from/to JSON file
- Manage file system operations (create directory, handle locks)
- Serialize C# objects with camelCase JSON properties
- Ensure data integrity during concurrent operations

**Internal Design:**

**File Path Management:**
- Uses `Path.Combine` to construct platform-independent paths
- Default location: `App_Data/tickets.json` relative to application root
- Constructor accepts optional custom file path for testing

**JSON Serialization Configuration:**
```csharp
JsonSerializerOptions:
- PropertyNamingPolicy = JsonNamingPolicy.CamelCase
- WriteIndented = true (for readability)
- DefaultIgnoreCondition = JsonIgnoreCondition.Never
- Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase))
```

**Concurrency Strategy:**
- Use file locking mechanism with `FileStream` and `FileShare.None` for writes
- Read operations use `FileShare.Read` to allow concurrent reads
- Implement retry logic with exponential backoff for locked files

**Error Handling:**
- Wrap file operations in try-catch blocks
- Log exceptions (future: integrate with ILogger)
- Return appropriate defaults (empty list, null, false) on errors
- Throw exceptions for critical failures (invalid data, permissions)

## Data Models

### JSON File Structure

**File:** `TicketSystem.Server/App_Data/tickets.json`

```json
[
  {
    "id": "b2a1f7c0-2a93-4d8f-a23e-9b1a24dcf5a1",
    "name": "John Doe",
    "email": "john.doe@example.com",
    "description": "My laptop overheats and shuts down after 10 minutes of use.",
    "summary": "Laptop overheating and unexpected shutdown.",
    "imageUrl": "uploads/laptop_issue.jpg",
    "status": "new",
    "resolution": "",
    "createdAt": "2025-10-27T14:35:00Z",
    "updatedAt": "2025-10-27T14:35:00Z"
  }
]
```

**Design Decisions:**
- Root element is an array of ticket objects
- All property names use camelCase
- DateTime values stored in ISO 8601 format with UTC timezone (Z suffix)
- Empty strings used for null/empty values (resolution)
- GUID stored as string in standard format

### C# to JSON Mapping

| C# Property | JSON Property | Type | Notes |
|-------------|---------------|------|-------|
| Id | id | Guid → string | Serialized as hyphenated string |
| Name | name | string | Direct mapping |
| Email | email | string | Direct mapping |
| Description | description | string | Direct mapping |
| Summary | summary | string | Direct mapping |
| ImageUrl | imageUrl | string | Direct mapping |
| Status | status | TicketStatus → string | Enum serialized as camelCase string |
| Resolution | resolution | string | Empty string if no resolution |
| CreatedAt | createdAt | DateTime → string | ISO 8601 UTC format |
| UpdatedAt | updatedAt | DateTime → string | ISO 8601 UTC format |

## Error Handling

### File System Errors

**Scenario:** JSON file doesn't exist
- **Handling:** Create App_Data directory if needed, initialize empty JSON array `[]`
- **Requirements:** 9.1, 9.2

**Scenario:** File is locked by another process
- **Handling:** Retry with exponential backoff (3 attempts, 100ms, 200ms, 400ms delays)
- **Fallback:** Throw IOException with descriptive message
- **Requirements:** 9.3, 9.5

**Scenario:** Insufficient permissions
- **Handling:** Throw UnauthorizedAccessException with clear message
- **Requirements:** 9.3, 9.5

**Scenario:** Corrupted JSON file
- **Handling:** Log error, backup corrupted file, initialize new empty array
- **Requirements:** 9.3

### Data Validation Errors

**Scenario:** Ticket with duplicate Id
- **Handling:** CreateAsync generates new GUID, ignores provided Id
- **Requirements:** 4.1

**Scenario:** Update/Delete non-existent ticket
- **Handling:** Return false to indicate operation failed
- **Requirements:** 7.5, 8.4

**Scenario:** Invalid GUID format
- **Handling:** Return null for GetByIdAsync, false for Update/Delete
- **Requirements:** 6.3, 7.5, 8.4

## Implementation Notes

### Dependency Injection Setup

The TicketDal should be registered in the DI container in `Program.cs`:

```csharp
builder.Services.AddScoped<ITicketDal, TicketDal>();
```

### File Path Configuration

The JSON file path should be configurable via:
1. Constructor parameter (for testing)
2. Configuration file (appsettings.json)
3. Default: `App_Data/tickets.json`

### Performance Considerations

- **File Size:** JSON file approach suitable for < 10,000 tickets
- **Read Performance:** O(n) for GetByIdAsync (acceptable for small datasets)
- **Write Performance:** Full file rewrite on each operation (acceptable for low write frequency)
- **Future Optimization:** Consider migrating to database when ticket count exceeds 10,000

### Security Considerations

- Validate file paths to prevent directory traversal attacks
- Ensure App_Data folder is not publicly accessible via web server
- Sanitize user input before storing in JSON (prevent injection)
- Consider encrypting sensitive data (email addresses) in future iterations

## Future Enhancements

1. **Database Migration:** Prepare for migration to SQL Server or PostgreSQL
2. **Caching Layer:** Add in-memory cache for frequently accessed tickets
3. **Audit Trail:** Track all changes with user and timestamp
4. **Soft Delete:** Mark tickets as deleted instead of removing
5. **Search/Filter:** Add methods for filtering by status, date range, etc.
6. **Pagination:** Support for large result sets
7. **Validation:** Add data annotations and validation logic
