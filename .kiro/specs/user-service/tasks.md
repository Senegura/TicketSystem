# Implementation Plan

- [x] 1. Create UserRegistration, UserLogin, and LoginResult models




  - Create UserRegistration.cs in TicketSystem.Models with Username, Password, and UserType properties
  - Create UserLogin.cs in TicketSystem.Models with Username and Password properties
  - Create LoginResult.cs in TicketSystem.Models with Success (bool), ErrorMessage (string), UserId (int), and UserType properties
  - Add XML documentation comments for all properties
  - _Requirements: 1.1, 1.2, 2.1_

- [x] 2. Create IUserService interface





  - Create IUserService.cs in TicketSystem.BL
  - Define RegisterAsync method that accepts UserRegistration and returns Task<User>
  - Define LoginAsync method that accepts UserLogin and returns Task<LoginResult>
  - Define SeedInitialData method that returns Task
  - Add XML documentation comments for interface and methods
  - _Requirements: 1.1, 2.1, 3.1_


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




  - [x] 3.3 Implement LoginAsync method



    - Retrieve user by username using UserDal.GetByUsernameAsync()
    - Return LoginResult with Success=false, ErrorMessage="Invalid username or password", UserId=0, UserType=default if user is null
    - Parse stored salt from Base64 string to byte array
    - Parse stored HashAlgorithm string to HashAlgorithmName
    - Calculate hash using CryptoService.HashPassword() with provided password and stored salt/iterations/algorithm
    - Compare calculated hash with stored PasswordHash
    - Return LoginResult with Success=true, ErrorMessage="", UserId=user.Id, UserType=user.UserType if hashes match
    - Return LoginResult with Success=false, ErrorMessage="Invalid username or password", UserId=0, UserType=default if hashes don't match
    - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5_


  - [x] 3.4 Implement SeedInitialData method

    - Create SeedInitialData method that seeds three test users
    - Create user with username "customer@gmail.com", password "customer", UserType.Customer
    - Create user with username "user@gmail.com", password "user", UserType.User
    - Create user with username "admin@gmail.com", password "admin", UserType.Admin
    - Use RegisterAsync method for each user to ensure consistent password hashing
    - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5_

  - [x] 3.5 Add XML documentation comments





    - Add XML documentation for UserService class
    - Add XML documentation for RegisterAsync method
    - Add XML documentation for LoginAsync method (update to reflect LoginResult return type)
    - Add XML documentation for SeedInitialData method
    - Document all parameters and return values
    - _Requirements: 1.1, 2.1, 3.1_
