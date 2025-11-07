using System.Security.Cryptography;
using TicketSystem.DAL;
using TicketSystem.Models;

namespace TicketSystem.BL;

/// <summary>
/// Provides user authentication and management operations including registration and login.
/// Implements secure password storage using cryptographic hashing with salt.
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
    /// </summary>
    /// <param name="registration">The user registration information containing username, password, and user type.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created user with assigned Id.</returns>
    public async Task<User> RegisterAsync(UserRegistration registration)
    {
        // Generate cryptographically secure salt
        byte[] salt = _cryptoService.GenerateSalt();

        // Hash the password with salt, iterations, and algorithm
        string passwordHash = _cryptoService.HashPassword(
            registration.Password,
            salt,
            Iterations,
            HashAlgorithm);

        // Create user object with hashed credentials
        var user = new User
        {
            Username = registration.Username,
            UserType = registration.UserType,
            PasswordHash = passwordHash,
            Salt = Convert.ToBase64String(salt),
            Iterations = Iterations,
            HashAlgorithm = HashAlgorithm.Name
        };

        // Persist user to database and return with assigned Id
        return await _userDal.CreateAsync(user);
    }

    /// <summary>
    /// Authenticates a user with username and password.
    /// </summary>
    /// <param name="login">The user login credentials containing username and password.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is true if authentication succeeds; false otherwise.</returns>
    public async Task<bool> LoginAsync(UserLogin login)
    {
        // Retrieve user by username
        User? user = await _userDal.GetByUsernameAsync(login.Username);

        // Return false if user not found
        if (user == null)
        {
            return false;
        }

        // Parse stored salt from Base64 string to byte array
        byte[] salt = Convert.FromBase64String(user.Salt);

        // Parse stored HashAlgorithm string to HashAlgorithmName
        HashAlgorithmName algorithm = new HashAlgorithmName(user.HashAlgorithm);

        // Calculate hash using provided password and stored parameters
        string calculatedHash = _cryptoService.HashPassword(
            login.Password,
            salt,
            user.Iterations,
            algorithm);

        // Compare calculated hash with stored hash
        return calculatedHash == user.PasswordHash;
    }
}
