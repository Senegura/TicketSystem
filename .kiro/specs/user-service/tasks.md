# Implementation Plan

- [x] 1. Create UserRegistration and UserLogin models





  - Create UserRegistration.cs in TicketSystem.Models with Username, Password, and UserType properties
  - Create UserLogin.cs in TicketSystem.Models with Username and Password properties
  - Add XML documentation comments for all properties
  - _Requirements: 1.1, 1.2, 2.1_
-

- [x] 2. Create IUserService interface




  - Create IUserService.cs in TicketSystem.BL
  - Define RegisterAsync method that accepts UserRegistration and returns Task<User>
  - Define LoginAsync method that accepts UserLogin and returns Task<bool>
  - Add XML documentation comments for interface and methods
  - _Requirements: 1.1, 2.1_
-

- [x] 3. Implement UserService class



  - [x] 3.1 Create UserService class structure


    - Create UserService.cs in TicketSystem.BL implementing IUserService
    - Add constructor accepting IUserDal and ICryptoService parameters
    - Define static readonly fields for HashAlgorithm (SHA256) and Iterations (100000)
    - Add private readonly fields for injected dependencies
    - _Requirements: 1.1, 1.4_

  - [x] 3.2 Implement RegisterAsync method


    - Generate salt using CryptoService.GenerateSalt()
    - Calculate password hash using CryptoService.HashPassword() with password, salt, iterations, and algorithm
    - Create User object with username, userType, passwordHash, Base64-encoded salt, iterations, and algorithm name
    - Call UserDal.CreateAsync() to persist the user
    - Return the created user with assigned Id
    - _Requirements: 1.2, 1.3, 1.4, 1.5_

  - [x] 3.3 Implement LoginAsync method


    - Retrieve user by username using UserDal.GetByUsernameAsync()
    - Return false if user is null
    - Parse stored salt from Base64 string to byte array
    - Parse stored HashAlgorithm string to HashAlgorithmName
    - Calculate hash using CryptoService.HashPassword() with provided password and stored salt/iterations/algorithm
    - Compare calculated hash with stored PasswordHash
    - Return true if hashes match, false otherwise
    - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5_

  - [x] 3.4 Add XML documentation comments

    - Add XML documentation for UserService class
    - Add XML documentation for RegisterAsync method
    - Add XML documentation for LoginAsync method
    - Document all parameters and return values
    - _Requirements: 1.1, 2.1_
