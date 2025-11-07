namespace TicketSystem.Models;

/// <summary>
/// Represents the data required to register a new user.
/// </summary>
public class UserRegistration
{
    /// <summary>
    /// Gets or sets the username for the new account.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the plaintext password for the new account.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of user account to create.
    /// </summary>
    public UserType UserType { get; set; }
}
