# Implementation Plan

- [x] 1. Add JWT NuGet package to TicketSystem.BL project





  - Add System.IdentityModel.Tokens.Jwt package reference to TicketSystem.BL.csproj
  - Restore packages to ensure the dependency is available
  - _Requirements: 2.4_

- [x] 2. Create IJwtService interface





  - Create IJwtService.cs file in TicketSystem.BL project
  - Define the SignToken method signature with claims and expiration parameters
  - Add XML documentation comments for the interface and method
  - _Requirements: 1.1, 1.2, 1.3, 1.4_

- [x] 3. Implement JwtService class





  - Create JwtService.cs file in TicketSystem.BL project
  - Implement IJwtService interface
  - Add hard-coded private secret key constant (256-bit cryptographically strong string)
  - Add issuer and audience constants
  - _Requirements: 2.1, 2.2, 2.3, 2.5, 3.1, 3.3_
- [x] 4. Implement SignToken method




- [ ] 4. Implement SignToken method

  - Convert secret key to byte array and create SymmetricSecurityKey
  - Create SigningCredentials with HMAC-SHA256 algorithm
  - Build JwtSecurityToken with claims, issuer, audience, and expiration
  - Use JwtSecurityTokenHandler to serialize token to string
  - Return the signed token string
  - _Requirements: 3.2, 4.1, 4.2, 4.3, 5.1, 5.2, 5.3, 5.4, 6.1, 6.2, 6.3, 6.4_
