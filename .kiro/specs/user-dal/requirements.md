# Requirements Document

## Introduction

This document specifies the requirements for implementing a User Data Access Layer (DAL) for the TicketSystem application. The User DAL will provide persistent storage for user accounts with three distinct user types (Customers, Users, Admins) and secure password storage using hashing with salt and iterations. The system will store user data in a local SQLite database located in the App_Data directory.

## Glossary

- **User DAL**: The Data Access Layer component responsible for user persistence operations
- **User Model**: The domain entity representing a user account in the system
- **User Type**: An enumeration defining the three categories of users (Customer, User, Admin)
- **Password Hash**: The cryptographically hashed representation of a user's password
- **Salt**: A random value added to passwords before hashing to prevent rainbow table attacks
- **Iterations**: The number of times the hashing algorithm is applied to strengthen password security
- **Hash Algorithm**: The cryptographic algorithm used to hash the password (e.g., PBKDF2, SHA256)
- **SQLite Database**: A lightweight, file-based relational database system
- **CRUD Operations**: Create, Read, Update, and Delete operations for data persistence
- **App_Data Directory**: The designated folder for storing application data files

## Requirements

### Requirement 1

**User Story:** As a system architect, I want a User model with comprehensive properties, so that the system can store all necessary user account information including secure password data.

#### Acceptance Criteria

1. THE User Model SHALL contain an Id property of type integer or GUID
2. THE User Model SHALL contain a Username property of type string
3. THE User Model SHALL contain a UserType property referencing the UserType enumeration
4. THE User Model SHALL contain a PasswordHash property of type string for storing hashed passwords
5. THE User Model SHALL contain an Iterations property of type integer for storing the hash iteration count
6. THE User Model SHALL contain a Salt property of type string for storing the password salt value
7. THE User Model SHALL contain a HashAlgorithm property of type string for storing the name of the hashing algorithm used

### Requirement 2

**User Story:** As a system architect, I want a UserType enumeration with three distinct values, so that the system can differentiate between Customer, User, and Admin account types.

#### Acceptance Criteria

1. THE UserType Enumeration SHALL define a Customer value
2. THE UserType Enumeration SHALL define a User value
3. THE UserType Enumeration SHALL define an Admin value

### Requirement 3

**User Story:** As a developer, I want an IUserDal interface defining CRUD operations, so that the system has a clear contract for user persistence operations.

#### Acceptance Criteria

1. THE IUserDal Interface SHALL define a Create method that accepts a User Model and returns the created User Model with assigned Id
2. THE IUserDal Interface SHALL define a Read method that accepts a user Id and returns the corresponding User Model or null if not found
3. THE IUserDal Interface SHALL define an Update method that accepts a User Model and returns a boolean indicating success
4. THE IUserDal Interface SHALL define a Delete method that accepts a user Id and returns a boolean indicating success
5. THE IUserDal Interface SHALL define a GetAll method that returns a collection of all User Models

### Requirement 4

**User Story:** As a developer, I want a UserDal implementation that persists data to SQLite, so that user accounts are stored reliably in a local database.

#### Acceptance Criteria

1. THE UserDal Implementation SHALL store user data in a SQLite database file
2. THE UserDal Implementation SHALL locate the SQLite database file in the App_Data directory
3. THE UserDal Implementation SHALL implement all methods defined in the IUserDal interface
4. WHEN the UserDal is instantiated, THE UserDal SHALL create the database file if it does not exist
5. WHEN the UserDal is instantiated, THE UserDal SHALL create the users table if it does not exist

### Requirement 5

**User Story:** As a system administrator, I want the database file stored in the App_Data directory, so that application data is organized in a standard location.

#### Acceptance Criteria

1. THE UserDal Implementation SHALL construct the database file path using the App_Data directory
2. IF the App_Data directory does not exist, THEN THE UserDal SHALL create the directory
3. THE UserDal Implementation SHALL name the database file "users.db"
