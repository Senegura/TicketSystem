# Implementation Plan

- [x] 1. Create UserType enumeration in Models project





  - Create UserType.cs file in TicketSystem.Models directory
  - Define enum with Customer (0), User (1), and Admin (2) values
  - Add XML documentation comments for each enum value
  - _Requirements: 2.1, 2.2, 2.3_


- [x] 2. Create User model in Models project




  - Create User.cs file in TicketSystem.Models directory
  - Implement all required properties: Id, Username, UserType, PasswordHash, Iterations, Salt, HashAlgorithm
  - Initialize string properties to string.Empty for nullable reference type compliance
  - Add comprehensive XML documentation comments for each property
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7_
- [x] 3. Add SQLite NuGet package to DAL project




- [ ] 3. Add SQLite NuGet package to DAL project

  - Add Microsoft.Data.Sqlite package reference (version 8.0.0) to TicketSystem.DAL.csproj
  - _Requirements: 4.1_

- [x] 4. Create IUserDal interface in DAL project





  - Create IUserDal.cs file in TicketSystem.DAL directory
  - Define CreateAsync method that accepts User and returns Task<User>
  - Define GetAllAsync method that returns Task<IEnumerable<User>>
  - Define GetByIdAsync method that accepts int id and returns Task<User?>
  - Define GetByUsernameAsync method that accepts string username and returns Task<User?>
  - Define UpdateAsync method that accepts User and returns Task<bool>
  - Define DeleteAsync method that accepts int id and returns Task<bool>
  - Add comprehensive XML documentation comments for interface and all methods
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5_

- [x] 5. Implement UserDal class with database initialization




- [x] 5.1 Create UserDal class skeleton and constructors







  - Create UserDal.cs file in TicketSystem.DAL directory
  - Implement IUserDal interface
  - Add private readonly field for database file path
  - Create default constructor that sets path to "App_Data/users.db"
  - Create parameterized constructor that accepts custom file path
  - Add XML documentation comments for class and constructors
  - _Requirements: 4.1, 4.2, 5.1, 5.2, 5.3_
-

- [x] 5.2 Implement database initialization logic




  - Create private EnsureDirectoryExists method to create App_Data directory if missing
  - Create private InitializeDatabaseAsync method to create database file and Users table
  - Define SQL schema: CREATE TABLE IF NOT EXISTS Users with all columns (Id, Username, UserType, PasswordHash, Iterations, Salt, HashAlgorithm)
  - Add UNIQUE constraint on Username column
  - Call initialization methods from constructors
  - _Requirements: 4.4, 4.5, 5.2_
- [x] 6. Implement CreateAsync method




- [ ] 6. Implement CreateAsync method

  - Open SQLite connection to database file
  - Execute INSERT statement with parameterized query for all User properties except Id
  - Retrieve auto-generated Id using last_insert_rowid()
  - Set Id on user object and return it
  - Handle SqliteException for constraint violations (duplicate username)
  - Add proper error handling and exception wrapping
  - _Requirements: 3.1, 4.3_

- [x] 7. Implement GetAllAsync method





  - Open SQLite connection to database file
  - Execute SELECT statement to retrieve all users
  - Use SqliteDataReader to read results
  - Map each row to User object (including enum conversion for UserType)
  - Return collection of User objects
  - Return empty collection if no users exist
  - Add proper error handling
  - _Requirements: 3.5, 4.3_

- [x] 8. Implement GetByIdAsync method





  - Open SQLite connection to database file
  - Execute SELECT statement with WHERE Id = @id parameterized query
  - Use SqliteDataReader to read single result
  - Map row to User object if found
  - Return User object or null if not found
  - Add proper error handling
  - _Requirements: 3.2, 4.3_

- [x] 9. Implement GetByUsernameAsync method





  - Open SQLite connection to database file
  - Execute SELECT statement with WHERE Username = @username parameterized query
  - Use SqliteDataReader to read single result
  - Map row to User object if found
  - Return User object or null if not found
  - Add proper error handling
  - _Requirements: 3.2, 4.3_

- [x] 10. Implement UpdateAsync method




  - Open SQLite connection to database file
  - Execute UPDATE statement with parameterized query for all User properties
  - Use WHERE Id = @id clause to target specific user
  - Check affected row count to determine if user was found
  - Return true if row was updated, false if user not found
  - Add proper error handling and exception wrapping
  - _Requirements: 3.3, 4.3_
-

- [x] 11. Implement DeleteAsync method




  - Open SQLite connection to database file
  - Execute DELETE statement with WHERE Id = @id parameterized query
  - Check affected row count to determine if user was found
  - Return true if row was deleted, false if user not found
  - Add proper error handling
  - _Requirements: 3.4, 4.3_


