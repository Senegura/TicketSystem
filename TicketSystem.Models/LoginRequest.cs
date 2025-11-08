namespace TicketSystem.Models;

/// <summary>
/// Represents the request payload for user login.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Gets or sets the username for authentication.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password for authentication.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
