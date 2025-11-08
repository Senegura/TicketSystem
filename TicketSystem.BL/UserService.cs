using System.Security.Cryptography;
using TicketSystem.DAL;
using TicketSystem.Models;

namespace TicketSystem.BL;

/// <summary>
/// Provides user authentication and management operations including registration, login, and seed data.
/// Implements secure password hashing using PBKDF2 with configurable algorithms and iterations.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserDal _userDal;
    private readonly ICryptoService _cryptoService;

    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;
    private static readonly int Iterations = 100000;

    /// <summary>
    /// Initializes a new instance of the UserService class.
    /// </summary>
    /// <param name="userDal">The data access layer for user operations.</param>
    /// <param name="cryptoService">The cryptographic service for password hashing.</param>
    public UserService(IUserDal userDal, ICryptoService cryptoService)
    {
        _userDal = userDal;
        _cryptoService = cryptoService;
    }

    /// <summary>
    /// Registers a new user with secure password storage.
    /// Generates a cryptographic salt, hashes the password using PBKDF2, and persists the user to the database.
    /// </summary>
    /// <param name="registration">The user registration information containing username, password, and user type.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created user with assigned Id.</returns>
    public async Task<User> RegisterAsync(UserRegistration registration)
    {
        // Generate cryptographic salt
        byte[] salt = _cryptoService.GenerateSalt();

        // Hash the password with salt, iterations, and algorithm
        string passwordHash = _cryptoService.HashPassword(
            registration.Password,
            salt,
            Iterations,
            HashAlgorithm
        );

        // Create user object with hashed credentials
        var user = new User
        {
            Username = registration.Username,
            UserType = registration.UserType,
            PasswordHash = passwordHash,
            Salt = Convert.ToBase64String(salt),
            Iterations = Iterations,
            HashAlgorithm = HashAlgorithm.Name ?? "SHA256"
        };

        // Persist user to database
        var createdUser = await _userDal.CreateAsync(user);

        return createdUser;
    }

    /// <summary>
    /// Authenticates a user with username and password.
    /// Retrieves the user from the database, recalculates the password hash, and compares it with the stored hash.
    /// </summary>
    /// <param name="login">The user login credentials containing username and password.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a LoginResult with success status, error message, user id, and user type.</returns>
    public async Task<LoginResult> LoginAsync(UserLogin login)
    {
        // Retrieve user by username
        var user = await _userDal.GetByUsernameAsync(login.Username);

        // Return failure if user not found
        if (user == null)
        {
            return new LoginResult
            {
                Success = false,
                ErrorMessage = "Invalid username or password",
                UserId = 0,
                UserType = default
            };
        }

        // Parse stored salt from Base64
        byte[] salt = Convert.FromBase64String(user.Salt);

        // Parse stored hash algorithm
        var hashAlgorithm = new HashAlgorithmName(user.HashAlgorithm);

        // Calculate hash with provided password and stored parameters
        string calculatedHash = _cryptoService.HashPassword(
            login.Password,
            salt,
            user.Iterations,
            hashAlgorithm
        );

        // Compare calculated hash with stored hash
        if (calculatedHash == user.PasswordHash)
        {
            return new LoginResult
            {
                Success = true,
                ErrorMessage = "",
                UserId = user.Id,
                UserType = user.UserType
            };
        }

        // Return failure if hashes don't match
        return new LoginResult
        {
            Success = false,
            ErrorMessage = "Invalid username or password",
            UserId = 0,
            UserType = default
        };
    }

    /// <summary>
    /// Seeds initial test user accounts for development and testing environments.
    /// Creates three predefined users: customer, user, and admin with default passwords.
    /// This method should only be called in development or testing environments.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SeedInitialData()
    {
        // Create customer user
        await RegisterAsync(new UserRegistration
        {
            Username = "customer@gmail.com",
            Password = "customer",
            UserType = UserType.Customer
        });

        // Create regular user
        await RegisterAsync(new UserRegistration
        {
            Username = "user@gmail.com",
            Password = "user",
            UserType = UserType.User
        });

        // Create admin user
        await RegisterAsync(new UserRegistration
        {
            Username = "admin@gmail.com",
            Password = "admin",
            UserType = UserType.Admin
        });
    }
}
