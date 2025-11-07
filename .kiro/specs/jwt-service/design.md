# JWT Service Design Document

## Overview

The JWT Service provides token generation and signing capabilities for the TicketSystem application. It follows a clean interface-implementation pattern, residing in the Business Logic layer (TicketSystem.BL). The service uses the System.IdentityModel.Tokens.Jwt library (part of Microsoft.IdentityModel.Tokens) to create and sign JWT tokens with HMAC-SHA256 algorithm.

## Architecture

### Layer Placement
- **Project**: TicketSystem.BL
- **Namespace**: TicketSystem.BL
- **Dependencies**: 
  - System.IdentityModel.Tokens.Jwt (NuGet package)
  - System.Security.Claims (built-in)

### Design Pattern
The service follows the Interface Segregation and Dependency Inversion principles:
- `IJwtService` interface defines the contract
- `JwtService` class provides the concrete implementation
- Consumers depend on the interface, not the implementation

## Components and Interfaces

### IJwtService Interface

```csharp
namespace TicketSystem.BL
{
    public interface IJwtService
    {
        /// <summary>
        /// Signs a JWT token with the provided claims and expiration time
        /// </summary>
        /// <param name="claims">Collection of claims to include in the token</param>
        /// <param name="expiresInMinutes">Token expiration time in minutes (default: 60)</param>
        /// <returns>Signed JWT token as a string</returns>
        string SignToken(IEnumerable<Claim> claims, int expiresInMinutes = 60);
    }
}
```

### JwtService Implementation

```csharp
namespace TicketSystem.BL
{
    public class JwtService : IJwtService
    {
        // Hard-coded secret key for signing tokens
        private const string SecretKey = "[256-bit cryptographically strong key]";
        
        // Default token configuration
        private const string Issuer = "TicketSystem";
        private const string Audience = "TicketSystemUsers";
        
        public string SignToken(IEnumerable<Claim> claims, int expiresInMinutes = 60)
        {
            // Implementation details in implementation section
        }
    }
}
```

## Data Models

### Input Parameters
- **claims**: `IEnumerable<Claim>` - Collection of claims to embed in the token
  - Standard claims: Subject (sub), Name, Email, Role
  - Custom claims: UserId, UserType, etc.
- **expiresInMinutes**: `int` - Token lifetime in minutes (default: 60)

### Output
- **Token**: `string` - Complete JWT in format: `{header}.{payload}.{signature}`

### Internal Data Structures
- **SecurityKey**: Symmetric security key derived from the secret
- **SigningCredentials**: Credentials object containing the key and algorithm
- **JwtSecurityToken**: Token object before serialization
- **TokenDescriptor**: Configuration object for token generation

## Token Structure

The generated JWT will have three parts:

1. **Header**
   ```json
   {
     "alg": "HS256",
     "typ": "JWT"
   }
   ```

2. **Payload** (example)
   ```json
   {
     "sub": "user123",
     "name": "John Doe",
     "role": "Admin",
     "iss": "TicketSystem",
     "aud": "TicketSystemUsers",
     "exp": 1699999999,
     "iat": 1699996399
   }
   ```

3. **Signature**
   - HMAC-SHA256 hash of header and payload using the secret key

## Implementation Details

### Token Signing Process

1. **Key Preparation**
   - Convert the hard-coded secret key string to byte array using UTF8 encoding
   - Create a `SymmetricSecurityKey` from the byte array
   - Create `SigningCredentials` with the key and SecurityAlgorithms.HmacSha256

2. **Token Configuration**
   - Create a `JwtSecurityToken` object with:
     - Issuer: "TicketSystem"
     - Audience: "TicketSystemUsers"
     - Claims: Provided claims collection
     - NotBefore: Current UTC time
     - Expires: Current UTC time + expiresInMinutes
     - SigningCredentials: The credentials created in step 1

3. **Token Generation**
   - Use `JwtSecurityTokenHandler` to write the token to string
   - Return the serialized token string

### Secret Key Generation

The hard-coded secret key will be a 256-bit (32-byte) cryptographically strong random string, base64-encoded for readability. Example format:
```
"Kj8mN2pQ4rT6wY9zB3cF5gH7jK0lM3nP6qS8tV1xZ4aC"
```

This provides sufficient entropy for HMAC-SHA256 algorithm.

## Error Handling

### Exception Scenarios

1. **Null or Empty Claims**
   - Behavior: Allow empty claims collection (token with only standard claims)
   - No exception thrown

2. **Invalid Expiration Time**
   - Condition: expiresInMinutes <= 0
   - Action: Use default value (60 minutes)
   - No exception thrown

3. **Token Generation Failure**
   - Condition: Unexpected error during token creation
   - Action: Let the exception bubble up (SecurityTokenException)
   - Caller responsibility to handle

### Validation

The service does NOT validate:
- Claim values (caller's responsibility)
- Token expiration (handled by token validation middleware)
- Token format (handled by JWT library)

The service focuses solely on token creation and signing.

## Dependencies

### NuGet Packages Required

```xml
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.0.0" />
```

This package includes:
- JwtSecurityTokenHandler
- JwtSecurityToken
- SigningCredentials
- SymmetricSecurityKey

### Framework Dependencies

- System.Security.Claims (built-in)
- System.Text (built-in for encoding)

## Usage Example

```csharp
// In a controller or service
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    
    public AuthController(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }
    
    public IActionResult Login(LoginRequest request)
    {
        // Authenticate user...
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.UserType.ToString())
        };
        
        string token = _jwtService.SignToken(claims, expiresInMinutes: 120);
        
        return Ok(new { token });
    }
}
```

## Security Considerations

1. **Secret Key Storage**
   - Hard-coded in the service (as per requirements)
   - Should be a cryptographically strong random string
   - Minimum 256 bits (32 bytes) for HS256

2. **Token Lifetime**
   - Default 60 minutes balances security and usability
   - Shorter lifetimes reduce risk of token theft
   - Longer lifetimes reduce authentication frequency

3. **Algorithm Choice**
   - HMAC-SHA256 (HS256) is symmetric and efficient
   - Suitable for server-to-server or server-to-client scenarios
   - Secret key must be protected on the server

4. **Future Enhancements** (Out of Scope)
   - Token refresh mechanism
   - Token revocation/blacklisting
   - Asymmetric signing (RS256) for distributed systems
   - Key rotation strategy
