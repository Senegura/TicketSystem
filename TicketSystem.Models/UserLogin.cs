namespace TicketSystem.Models;

/// <summary>
/// Represents the credentials required for user authentication.
/// </summary>
public class UserLogin
{
    /// <summary>
    /// Gets or sets the username for authentication.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the plaintext password for authentication.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
