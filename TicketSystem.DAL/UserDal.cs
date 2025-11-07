using Microsoft.Data.Sqlite;
using TicketSystem.Models;

namespace TicketSystem.DAL;

/// <summary>
/// SQLite-based implementation of the user data access layer.
/// Stores user data in a SQLite database with secure password hashing support.
/// </summary>
public class UserDal : IUserDal
{
    private readonly string _databasePath;

    /// <summary>
    /// Initializes a new instance of the UserDal class with the default database path.
    /// Creates the database and Users table if they don't exist.
    /// </summary>
    public UserDal() : this(Path.Combine("App_Data", "users.db"))
    {
    }

    /// <summary>
    /// Initializes a new instance of the UserDal class with a custom database path.
    /// Creates the database and Users table if they don't exist.
    /// </summary>
    /// <param name="databasePath">The path to the SQLite database file.</param>
    public UserDal(string databasePath)
    {
        _databasePath = databasePath;
        EnsureDirectoryExists();
        InitializeDatabaseAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Ensures the directory for the database file exists, creating it if necessary.
    /// </summary>
    private void EnsureDirectoryExists()
    {
        var directory = Path.GetDirectoryName(_databasePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    /// <summary>
    /// Initializes the database by creating the Users table if it doesn't exist.
    /// </summary>
    private async Task InitializeDatabaseAsync()
    {
        try
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            await connection.OpenAsync();

            var createTableCommand = connection.CreateCommand();
            createTableCommand.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    UserType INTEGER NOT NULL,
                    PasswordHash TEXT NOT NULL,
                    Iterations INTEGER NOT NULL,
                    Salt TEXT NOT NULL,
                    HashAlgorithm TEXT NOT NULL
                )";

            await createTableCommand.ExecuteNonQueryAsync();
        }
        catch (SqliteException ex)
        {
            throw new InvalidOperationException($"Failed to initialize database: {ex.Message}", ex);
        }
        catch (IOException ex)
        {
            throw new IOException($"Failed to access database file: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Creates a new user in the data store.
    /// </summary>
    /// <param name="user">The user to create. The Id will be auto-generated.</param>
    /// <returns>The created user with its assigned Id.</returns>
    public async Task<User> CreateAsync(User user)
    {
        try
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Users (Username, UserType, PasswordHash, Iterations, Salt, HashAlgorithm)
                VALUES (@username, @userType, @passwordHash, @iterations, @salt, @hashAlgorithm);
                SELECT last_insert_rowid();";

            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@userType", (int)user.UserType);
            command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@iterations", user.Iterations);
            command.Parameters.AddWithValue("@salt", user.Salt);
            command.Parameters.AddWithValue("@hashAlgorithm", user.HashAlgorithm);

            var result = await command.ExecuteScalarAsync();
            user.Id = Convert.ToInt32(result);

            return user;
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
    }

    /// <summary>
    /// Retrieves all users from the data store.
    /// </summary>
    /// <returns>A collection of all users, or an empty collection if none exist.</returns>
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        try
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Username, UserType, PasswordHash, Iterations, Salt, HashAlgorithm
                FROM Users";

            var users = new List<User>();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var user = new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    UserType = (UserType)reader.GetInt32(2),
                    PasswordHash = reader.GetString(3),
                    Iterations = reader.GetInt32(4),
                    Salt = reader.GetString(5),
                    HashAlgorithm = reader.GetString(6)
                };
                users.Add(user);
            }

            return users;
        }
        catch (SqliteException ex)
        {
            throw new InvalidOperationException($"Database error while retrieving users: {ex.Message}", ex);
        }
        catch (IOException ex)
        {
            throw new IOException($"Failed to access database file: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Retrieves a specific user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    public async Task<User?> GetByIdAsync(int id)
    {
        try
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Username, UserType, PasswordHash, Iterations, Salt, HashAlgorithm
                FROM Users
                WHERE Id = @id";

            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    UserType = (UserType)reader.GetInt32(2),
                    PasswordHash = reader.GetString(3),
                    Iterations = reader.GetInt32(4),
                    Salt = reader.GetString(5),
                    HashAlgorithm = reader.GetString(6)
                };
            }

            return null;
        }
        catch (SqliteException ex)
        {
            throw new InvalidOperationException($"Database error while retrieving user with Id {id}: {ex.Message}", ex);
        }
        catch (IOException ex)
        {
            throw new IOException($"Failed to access database file: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Retrieves a specific user by their username.
    /// </summary>
    /// <param name="username">The username of the user to retrieve.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        try
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Username, UserType, PasswordHash, Iterations, Salt, HashAlgorithm
                FROM Users
                WHERE Username = @username";

            command.Parameters.AddWithValue("@username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    UserType = (UserType)reader.GetInt32(2),
                    PasswordHash = reader.GetString(3),
                    Iterations = reader.GetInt32(4),
                    Salt = reader.GetString(5),
                    HashAlgorithm = reader.GetString(6)
                };
            }

            return null;
        }
        catch (SqliteException ex)
        {
            throw new InvalidOperationException($"Database error while retrieving user with username '{username}': {ex.Message}", ex);
        }
        catch (IOException ex)
        {
            throw new IOException($"Failed to access database file: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Updates an existing user in the data store.
    /// </summary>
    /// <param name="user">The user with updated values. The Id must match an existing user.</param>
    /// <returns>True if the user was updated successfully; false if the user was not found.</returns>
    public async Task<bool> UpdateAsync(User user)
    {
        try
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Users
                SET Username = @username,
                    UserType = @userType,
                    PasswordHash = @passwordHash,
                    Iterations = @iterations,
                    Salt = @salt,
                    HashAlgorithm = @hashAlgorithm
                WHERE Id = @id";

            command.Parameters.AddWithValue("@id", user.Id);
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@userType", (int)user.UserType);
            command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@iterations", user.Iterations);
            command.Parameters.AddWithValue("@salt", user.Salt);
            command.Parameters.AddWithValue("@hashAlgorithm", user.HashAlgorithm);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19) // CONSTRAINT violation
        {
            throw new InvalidOperationException($"Username '{user.Username}' already exists.", ex);
        }
        catch (SqliteException ex)
        {
            throw new InvalidOperationException($"Database error during user update: {ex.Message}", ex);
        }
        catch (IOException ex)
        {
            throw new IOException($"Failed to access database file: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Deletes a user from the data store.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>True if the user was deleted successfully; false if the user was not found.</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                DELETE FROM Users
                WHERE Id = @id";

            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        catch (SqliteException ex)
        {
            throw new InvalidOperationException($"Database error while deleting user with Id {id}: {ex.Message}", ex);
        }
        catch (IOException ex)
        {
            throw new IOException($"Failed to access database file: {ex.Message}", ex);
        }
    }
}
