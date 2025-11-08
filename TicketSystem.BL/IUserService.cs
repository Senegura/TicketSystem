namespace TicketSystem.BL;

using TicketSystem.Models;

/// <summary>
/// Defines the contract for user authentication and management operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Registers a new user with secure password storage.
    /// </summary>
    /// <param name="registration">The user registration information containing username, password, and user type.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created user with assigned Id.</returns>
    Task<User> RegisterAsync(UserRegistration registration);

    /// <summary>
    /// Authenticates a user with username and password.
    /// </summary>
    /// <param name="login">The user login credentials containing username and password.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a LoginResult with success status, error message, user id, and user type.</returns>
    Task<LoginResult> LoginAsync(UserLogin login);

    /// <summary>
    /// Seeds initial test user accounts for development and testing environments.
    /// Creates predefined users: customer, user, and admin with default passwords.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SeedInitialData();
}
