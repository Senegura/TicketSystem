# Design Document

## Overview

The User Data Access Layer (DAL) provides persistent storage for user accounts in the TicketSystem application. This design implements a SQLite-based data access layer that supports three user types (Customer, User, Admin) with secure password storage using cryptographic hashing. The implementation follows the existing architectural patterns established in the TicketSystem codebase, maintaining consistency with the layered architecture.

## Architecture

### Layer Placement

The User DAL follows the established three-layer architecture:

- **TicketSystem.Models**: Contains the `User` model and `UserType` enumeration
- **TicketSystem.DAL**: Contains the `IUserDal` interface and `UserDal` implementation
- **TicketSystem.Server**: Will consume the DAL through dependency injection (future integration)

### Technology Stack

- **Database**: SQLite 3.x via Microsoft.Data.Sqlite NuGet package
- **Language**: C# 12 with .NET 8
- **Data Access**: ADO.NET with parameterized queries
- **Storage Location**: App_Data/users.db

### Design Principles

1. **Consistency**: Follow existing patterns from TicketDal (async/await, error handling, XML documentation)
2. **Security**: Store passwords as hashes with salt and iterations; never store plaintext passwords
3. **Separation of Concerns**: DAL handles only data persistence, not business logic or password hashing
4. **Interface-Based Design**: Program against IUserDal interface for testability and flexibility

## Components and Interfaces

### UserType Enumeration

```csharp
namespace TicketSystem.Models;

/// <summary>
/// Defines the types of users in the system.
/// </summary>
public enum UserType
{
    /// <summary>
    /// A customer who can submit and view their own tickets.
    /// </summary>
    Customer = 0,
    
    /// <summary>
    /// A regular user who can manage tickets.
    /// </summary>
    User = 1,
    
    /// <summary>
    /// An administrator with full system access.
    /// </summary>
    Admin = 2
}
```

### User Model

```csharp
namespace TicketSystem.Models;

/// <summary>
/// Represents a user account in the system.
/// </summary>
public class User
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the username for authentication.
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the type of user account.
    /// </summary>
    public UserType UserType { get; set; }
    
    /// <summary>
    /// Gets or sets the hashed password.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the number of hash iterations applied.
    /// </summary>
    public int Iterations { get; set; }
    
    /// <summary>
    /// Gets or sets the salt value used in password hashing.
    /// </summary>
    public string Salt { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the name of the hashing algorithm used.
    /// </summary>
    public string HashAlgorithm { get; set; } = string.Empty;
}
```

**Design Decisions:**
- Use `int` for Id (auto-increment primary key in SQLite)
- All string properties default to `string.Empty` to avoid null reference warnings
- UserType is an enum for type safety and clarity
- Password security properties (PasswordHash, Salt, Iterations, HashAlgorithm) are stored but not used by DAL

### IUserDal Interface

```csharp
namespace TicketSystem.DAL;

/// <summary>
/// Defines the contract for user data access operations.
/// Provides CRUD operations for managing user entities.
/// </summary>
public interface IUserDal
{
    /// <summary>
    /// Creates a new user in the data store.
    /// </summary>
    /// <param name="user">The user to create. The Id will be auto-generated.</param>
    /// <returns>The created user with its assigned Id.</returns>
    Task<User> CreateAsync(User user);
    
    /// <summary>
    /// Retrieves all users from the data store.
    /// </summary>
    /// <returns>A collection of all users, or an empty collection if none exist.</returns>
    Task<IEnumerable<User>> GetAllAsync();
    
    /// <summary>
    /// Retrieves a specific user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetByIdAsync(int id);
    
    /// <summary>
    /// Retrieves a specific user by their username.
    /// </summary>
    /// <param name="username">The username of the user to retrieve.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetByUsernameAsync(string username);
    
    /// <summary>
    /// Updates an existing user in the data store.
    /// </summary>
    /// <param name="user">The user with updated values. The Id must match an existing user.</param>
    /// <returns>True if the user was updated successfully; false if the user was not found.</returns>
    Task<bool> UpdateAsync(User user);
    
    /// <summary>
    /// Deletes a user from the data store.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>True if the user was deleted successfully; false if the user was not found.</returns>
    Task<bool> DeleteAsync(int id);
}
```

**Design Decisions:**
- Follow async/await pattern consistent with ITicketDal
- Add `GetByUsernameAsync` method for authentication scenarios (common use case)
- Return `User?` for nullable reference types (C# 12 best practice)
- Return `bool` for Update/Delete to indicate success/failure

### UserDal Implementation

The UserDal class implements IUserDal using SQLite with the following key features:

**Constructor and Initialization:**
- Default constructor uses "App_Data/users.db" path
- Parameterized constructor accepts custom file path (for testing)
- Initialization creates App_Data directory if missing
- Initialization creates database and users table if they don't exist

**Database Schema:**
```sql
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    UserType INTEGER NOT NULL,
    PasswordHash TEXT NOT NULL,
    Iterations INTEGER NOT NULL,
    Salt TEXT NOT NULL,
    HashAlgorithm TEXT NOT NULL
)
```

**Key Implementation Details:**
- Use parameterized queries to prevent SQL injection
- Use `Microsoft.Data.Sqlite` for database access
- Implement proper connection management (using statements)
- Map UserType enum to INTEGER in database
- Handle SQLite exceptions and convert to meaningful error messages
- Follow async/await pattern throughout

## Data Models

### Database Schema

**Users Table:**
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | INTEGER | PRIMARY KEY AUTOINCREMENT | Auto-generated unique identifier |
| Username | TEXT | NOT NULL UNIQUE | Unique username for authentication |
| UserType | INTEGER | NOT NULL | User type (0=Customer, 1=User, 2=Admin) |
| PasswordHash | TEXT | NOT NULL | Cryptographically hashed password |
| Iterations | INTEGER | NOT NULL | Number of hash iterations |
| Salt | TEXT | NOT NULL | Random salt value |
| HashAlgorithm | TEXT | NOT NULL | Name of hashing algorithm |

**Indexes:**
- Primary key index on Id (automatic)
- Unique index on Username (automatic from UNIQUE constraint)

### Data Mapping

**C# to SQLite Type Mapping:**
- `int` → INTEGER
- `string` → TEXT
- `UserType` (enum) → INTEGER

**Enum Mapping:**
- Customer = 0
- User = 1
- Admin = 2

## Error Handling

### Exception Strategy

Follow the error handling patterns established in TicketDal:

1. **Database Connection Errors**: Wrap in `IOException` with descriptive message
2. **SQL Errors**: Wrap in `InvalidOperationException` with query context
3. **Constraint Violations**: Wrap in `InvalidOperationException` (e.g., duplicate username)
4. **File System Errors**: Wrap in `IOException` for directory/file access issues
5. **Not Found Scenarios**: Return `null` or `false` (not exceptions)

### Error Messages

Provide clear, actionable error messages:
- Include operation context (Create, Update, Delete, etc.)
- Include relevant identifiers (username, id)
- Avoid exposing sensitive information (passwords, hashes)
- Include inner exception for debugging

### Example Error Handling

```csharp
try
{
    // Database operation
}
catch (SqliteException ex) when (ex.SqliteErrorCode == 19) // CONSTRAINT violation
{
    throw new InvalidOperationException($"Username '{user.Username}' already exists.", ex);
}
catch (SqliteException ex)
{
    throw new InvalidOperationException($"Database error during user creation: {ex.Message}", ex);
}
catch (IOException ex)
{
    throw new IOException($"Failed to access database file: {ex.Message}", ex);
}
```

## Security Considerations

### Password Storage

- **Never store plaintext passwords**: Only PasswordHash, Salt, Iterations, and HashAlgorithm
- **DAL is not responsible for hashing**: Business logic layer handles password hashing
- **All password fields are required**: Enforce NOT NULL constraints

### SQL Injection Prevention

- **Always use parameterized queries**: Never concatenate user input into SQL
- **Validate input at BL layer**: DAL assumes validated input from business logic

### Username Uniqueness

- **Enforce UNIQUE constraint**: Database-level constraint prevents duplicates
- **Handle constraint violations gracefully**: Return meaningful error messages

### Data Access Control

- **No built-in authorization**: DAL provides data access; authorization is BL responsibility
- **All methods are public**: Access control should be implemented at higher layers

## Dependencies

### NuGet Packages

Add to TicketSystem.DAL.csproj:
```xml
<PackageReference Include="Microsoft.Data.Sqlite" Version="8.0.0" />
```

### Project References

- TicketSystem.DAL references TicketSystem.Models (already exists)

## Future Enhancements

1. **Connection Pooling**: Consider connection pooling for high-traffic scenarios
2. **Async Initialization**: Make database initialization async
3. **Migration Support**: Add schema versioning for future database changes
4. **Logging**: Integrate with ILogger for structured logging
5. **Soft Deletes**: Add IsDeleted flag instead of hard deletes
6. **Audit Fields**: Add CreatedAt, UpdatedAt timestamps
7. **Query Optimization**: Add indexes for common query patterns
8. **Bulk Operations**: Add methods for bulk insert/update/delete
