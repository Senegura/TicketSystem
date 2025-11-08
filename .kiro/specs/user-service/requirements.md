# Requirements Document

## Introduction

The User Service is a business logic component that manages user authentication operations including registration and login. It integrates with the Data Access Layer (UserDal) for user persistence and the CryptoService for secure password hashing. The service ensures secure user credential storage and validation using cryptographic hashing with salt.

## Glossary

- **UserService**: The business logic service responsible for user registration and authentication operations
- **UserDal**: The Data Access Layer component that handles user data persistence operations
- **CryptoService**: The cryptographic service that provides password hashing functionality
- **UserRegistration**: A data model containing username, password, and UserType for new user registration
- **UserLogin**: A data model containing username and password for user authentication
- **LoginResult**: A data model containing authentication result with success status, error message, user id, and user type
- **PasswordHash**: The cryptographically hashed representation of a user's password
- **Salt**: A random value used in password hashing to prevent rainbow table attacks
- **HashAlgorithm**: The cryptographic algorithm identifier used for password hashing (e.g., SHA256, SHA512)
- **Iterations**: The number of times the hashing algorithm is applied to strengthen the hash

## Requirements

### Requirement 1

**User Story:** As a system administrator, I want to register new users with secure password storage, so that user credentials are protected from unauthorized access

#### Acceptance Criteria

1. THE UserService SHALL accept UserDal and CryptoService instances via constructor injection
2. WHEN a UserRegistration model is provided, THE UserService SHALL generate a cryptographic salt value
3. WHEN a UserRegistration model is provided, THE UserService SHALL calculate the PasswordHash using the password, salt, and HashAlgorithm
4. THE UserService SHALL store the HashAlgorithm identifier as a static variable within the class
5. WHEN registration is complete, THE UserService SHALL persist the user data including username, PasswordHash, salt, HashAlgorithm, and UserType to the database via UserDal

### Requirement 2

**User Story:** As a registered user, I want to authenticate with my username and password, so that I can access the system securely

#### Acceptance Criteria

1. WHEN a UserLogin model is provided, THE UserService SHALL retrieve the user record including PasswordHash, salt, HashAlgorithm, and iterations from UserDal
2. WHEN the user record is retrieved, THE UserService SHALL calculate the hash using the provided password, stored salt, stored HashAlgorithm, and stored iterations
3. WHEN the calculated hash matches the stored PasswordHash, THE UserService SHALL return a LoginResult with success set to true, empty error message, the user id, and the user type
4. WHEN the calculated hash does not match the stored PasswordHash, THE UserService SHALL return a LoginResult with success set to false, error message "Invalid username or password", zero user id, and default user type
5. WHEN the user is not found in the database, THE UserService SHALL return a LoginResult with success set to false, error message "Invalid username or password", zero user id, and default user type

### Requirement 3

**User Story:** As a system administrator, I want to seed initial test users into the system, so that I can quickly set up a development or testing environment with predefined user accounts

#### Acceptance Criteria

1. THE UserService SHALL provide a SeedInitialData method that creates predefined user accounts
2. WHEN SeedInitialData is invoked, THE UserService SHALL create a user with username "customer@gmail.com" and password "customer"
3. WHEN SeedInitialData is invoked, THE UserService SHALL create a user with username "user@gmail.com" and password "user"
4. WHEN SeedInitialData is invoked, THE UserService SHALL create a user with username "admin@gmail.com" and password "admin"
5. WHEN creating each seeded user, THE UserService SHALL use the same secure password hashing process as the registration method
