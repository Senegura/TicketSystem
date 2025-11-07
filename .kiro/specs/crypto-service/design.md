# Crypto Service Design Document

## Overview

The CryptoService is a business logic service that provides secure password hashing and salt generation functionality. It implements industry-standard cryptographic operations using .NET's built-in cryptography libraries to ensure secure password storage.

## Architecture

The service follows a simple service pattern within the Business Logic layer:

```
TicketSystem.BL/
├── ICryptoService.cs          # Service interface
└── CryptoService.cs           # Service implementation
```

The service is stateless and can be registered as a singleton in the dependency injection container.

## Components and Interfaces

### ICryptoService Interface

```csharp
public interface ICryptoService
{
    string HashPassword(string password, byte[] salt, int iterationCount, HashAlgorithmName hashAlgorithm);
    byte[] GenerateSalt(int size = 32);
}
```

### CryptoService Implementation

The service uses the following .NET cryptography components:
- `Rfc2898DeriveBytes` (PBKDF2) for password hashing with configurable hash algorithms
- `RandomNumberGenerator` for cryptographically secure salt generation

## Data Models

### Input Parameters

**HashPassword Method:**
- `password` (string): The plaintext password to hash
- `salt` (byte[]): The cryptographic salt to use
- `iterationCount` (int): Number of iterations for the key derivation function
- `hashAlgorithm` (HashAlgorithmName): The hash algorithm to use (e.g., SHA256, SHA512)

**GenerateSalt Method:**
- `size` (int, optional): The size of the salt in bytes (default: 32)

### Output

**HashPassword Method:**
- Returns: `string` - Base64-encoded password hash

**GenerateSalt Method:**
- Returns: `byte[]` - Cryptographically secure random salt

## Error Handling

The service will validate inputs and throw appropriate exceptions:

- `ArgumentNullException`: When password is null or empty
- `ArgumentNullException`: When salt is null
- `ArgumentOutOfRangeException`: When iteration count is less than 1
- `ArgumentOutOfRangeException`: When salt size is less than 1

The service relies on .NET's cryptography libraries for secure implementation, which handle cryptographic errors internally.

## Security Considerations

1. **PBKDF2 Algorithm**: Using Rfc2898DeriveBytes provides PBKDF2 (Password-Based Key Derivation Function 2), which is designed for password hashing
2. **Configurable Iterations**: Allows adjusting computational cost as hardware improves
3. **Secure Random**: RandomNumberGenerator uses the operating system's cryptographically secure random number generator
4. **Salt Storage**: Salts should be stored alongside hashed passwords (they are not secret)
5. **Algorithm Flexibility**: Supporting HashAlgorithmName allows upgrading to stronger algorithms without code changes

## Implementation Notes

- Default salt size of 32 bytes (256 bits) provides strong security
- Minimum recommended iteration count is 10,000 for PBKDF2
- The hash output size depends on the chosen algorithm (SHA256 = 32 bytes, SHA512 = 64 bytes)
- Base64 encoding is used for the hash output to enable easy string storage
