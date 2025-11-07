# User Service Design Document

## Overview

The UserService is a business logic service that manages user authentication operations including registration and login. It acts as an orchestrator between the Data Access Layer (UserDal) and the CryptoService, implementing secure user credential management. The service ensures that passwords are never stored in plaintext and validates user credentials during authentication.

## Architecture

The service follows the established layered architecture pattern:

```
TicketSystem.BL/
├── IUserService.cs          # Service interface
└── UserService.cs           # Service implementation
```

The service is stateless and can be registered as a scoped or transient service in the dependency injection container.

### Dependencies

- **IUserDal**: For user data persistence operations
- **ICryptoService**: For password hashing and salt generation
- **TicketSystem.Models**: For User, UserType, UserRegistration, and UserLogin models

## Components and Interfaces

### IUserService Interface

```csharp
namespace TicketSystem.BL;

/// <summary>
/// Defines the contract for user authentication and management operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Registers a new user with secure password storage.
    /// </summary>
    /// <param name="registration">The user registration information.</param>
    /// <returns>The created user with assigned Id.</returns>
    Task<User> RegisterAsync(UserRegistration registration);
    
    /// <summary>
    /// Authenticates a user with username and password.
    /// </summary>
    /// <param name="login">The user login credentials.</param>
    /// <returns>True if authentication succeeds; false otherwise.</returns>
    Task<bool> LoginAsync(UserLogin login);
}
```

### UserService Implementation

**Constructor:**
```csharp
public UserService(IUserDal userDal, ICryptoService cryptoService)
{
    _userDal = userDal;
    _cryptoService = cryptoService;
}
```

**Static Configuration:**
- `HashAlgorithm`: Static readonly field set to `HashAlgorithmName.SHA256`
- `Iterations`: Static readonly field set to `100000` (recommended PBKDF2 iterations)

**Key Methods:**
- `RegisterAsync`: Handles new user registration with secure password hashing
- `LoginAsync`: Validates user credentials against stored hash

## Data Models

### UserRegistration Model

```csharp
namespace TicketSystem.Models;

/// <summary>
/// Represents the data required to register a new user.
/// </summary>
public class UserRegistration
{
    /// <summary>
    /// Gets or sets the username for the new account.
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the plaintext password for the new account.
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the type of user account to create.
    /// </summary>
    public UserType UserType { get; set; }
}
```

### UserLogin Model

```csharp
namespace TicketSystem.Models;

/// <summary>
/// Represents the credentials required for user authentication.
/// </summary>
public class UserLogin
{
    /// <summary>
    /// Gets or sets the username for authentication.
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the plaintext password for authentication.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
```

## Registration Flow

### RegisterAsync Method Logic

1. **Generate Salt**: Call `_cryptoService.GenerateSalt()` to create a cryptographically secure random salt
2. **Hash Password**: Call `_cryptoService.HashPassword()` with:
   - The plaintext password from UserRegistration
   - The generated salt
   - The static Iterations value
   - The static HashAlgorithm value
3. **Create User Object**: Construct a new User instance with:
   - Username from UserRegistration
   - UserType from UserRegistration
   - PasswordHash from step 2
   - Salt (Base64-encoded) from step 1
   - Iterations (static value)
   - HashAlgorithm name (static value converted to string)
4. **Persist User**: Call `_userDal.CreateAsync()` to store the user in the database
5. **Return User**: Return the created user with assigned Id

### Registration Sequence Diagram

```
UserService -> CryptoService: GenerateSalt()
CryptoService -> UserService: byte[] salt

UserService -> CryptoService: HashPassword(password, salt, iterations, algorithm)
CryptoService -> UserService: string passwordHash

UserService -> UserDal: CreateAsync(user)
UserDal -> Database: INSERT INTO Users
Database -> UserDal: User with Id
UserDal -> UserService: User
```

## Login Flow

### LoginAsync Method Logic

1. **Retrieve User**: Call `_userDal.GetByUsernameAsync()` to fetch the user record
2. **Check User Exists**: If user is null, return false immediately
3. **Parse Salt**: Convert the stored Salt string (Base64) back to byte array
4. **Parse Algorithm**: Convert the stored HashAlgorithm string to HashAlgorithmName
5. **Calculate Hash**: Call `_cryptoService.HashPassword()` with:
   - The plaintext password from UserLogin
   - The parsed salt from the database
   - The stored Iterations value
   - The parsed HashAlgorithm value
6. **Compare Hashes**: Compare the calculated hash with the stored PasswordHash
7. **Return Result**: Return true if hashes match, false otherwise

### Login Sequence Diagram

```
UserService -> UserDal: GetByUsernameAsync(username)
UserDal -> Database: SELECT * FROM Users WHERE Username = ?
Database -> UserDal: User record or null
UserDal -> UserService: User or null

[If user exists]
UserService -> CryptoService: HashPassword(password, storedSalt, storedIterations, storedAlgorithm)
CryptoService -> UserService: string calculatedHash

UserService: Compare calculatedHash with storedPasswordHash
UserService: Return true/false
```

## Error Handling

### Registration Errors

**Duplicate Username:**
- UserDal will throw `InvalidOperationException` due to UNIQUE constraint
- Let exception propagate to caller (controller/API layer)
- Caller should catch and return appropriate HTTP status (409 Conflict)

**Invalid Input:**
- Validate at controller/API layer before calling RegisterAsync
- UserService assumes valid input (non-null, non-empty username/password)

**Database Errors:**
- UserDal wraps database exceptions in `InvalidOperationException` or `IOException`
- Let exceptions propagate to caller for proper error handling

### Login Errors

**User Not Found:**
- Return false (not an exception)
- Caller should treat as authentication failure

**Invalid Password:**
- Return false (not an exception)
- Caller should treat as authentication failure

**Database Errors:**
- UserDal wraps database exceptions
- Let exceptions propagate to caller

### Security Considerations for Error Handling

- **Don't reveal user existence**: Return false for both "user not found" and "invalid password"
- **Timing attacks**: Consider constant-time comparison for password hashes (future enhancement)
- **Rate limiting**: Implement at API layer, not in UserService

## Security Considerations

### Password Hashing Configuration

**Algorithm Choice:**
- SHA256 provides good balance of security and performance
- Can be upgraded to SHA512 in future by changing static field

**Iteration Count:**
- 100,000 iterations recommended by OWASP (2023)
- Provides strong protection against brute-force attacks
- Can be increased in future as hardware improves

**Salt Generation:**
- 32-byte (256-bit) salt from CryptoService
- Cryptographically secure random generation
- Unique per user

### Password Storage

- **Never log passwords**: Ensure plaintext passwords are never logged
- **Clear sensitive data**: Consider clearing password strings from memory after hashing
- **Secure transmission**: Passwords should only be transmitted over HTTPS

### Authentication Best Practices

- **No password hints**: Don't store or return password hints
- **Account lockout**: Consider implementing at API layer after multiple failed attempts
- **Password complexity**: Enforce at API/UI layer, not in UserService
- **Session management**: Implement at API layer using JWT or cookies

## Implementation Notes

### Static Configuration

```csharp
private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;
private static readonly int Iterations = 100000;
```

These values are hardcoded as static fields to ensure consistency across all user registrations. Future enhancements could move these to configuration files.

### Base64 Encoding

- Salt is stored as Base64 string in database for easy serialization
- Convert byte[] to Base64 using `Convert.ToBase64String()`
- Convert Base64 to byte[] using `Convert.FromBase64String()`

### HashAlgorithmName Conversion

- Store as string in database: `HashAlgorithm.Name` property
- Parse from string: `new HashAlgorithmName(storedValue)`

### Async/Await Pattern

- All methods are async to support async database operations
- Follow async best practices (no blocking calls)
- Use ConfigureAwait(false) if needed for library code

## Dependencies

### Project References

TicketSystem.BL already references:
- TicketSystem.Models (for User, UserType)
- TicketSystem.DAL (for IUserDal)

### NuGet Packages

No additional packages required. Uses existing:
- System.Security.Cryptography (for HashAlgorithmName)
- System.Threading.Tasks (for async/await)

## Future Enhancements

1. **Configuration-based settings**: Move HashAlgorithm and Iterations to appsettings.json
2. **Password validation**: Add password complexity requirements
3. **Email verification**: Add email field and verification workflow
4. **Password reset**: Add forgot password functionality
5. **Multi-factor authentication**: Add 2FA support
6. **Account lockout**: Track failed login attempts
7. **Password history**: Prevent password reuse
8. **Audit logging**: Log authentication events for security monitoring
9. **Constant-time comparison**: Implement timing-attack resistant hash comparison
10. **Return user on login**: Return User object instead of bool for successful logins
