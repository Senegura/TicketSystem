namespace TicketSystem.Models;

/// <summary>
/// Represents the result of a login authentication attempt.
/// </summary>
public class LoginResult
{
    /// <summary>
    /// Gets or sets whether the authentication was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the error message if authentication failed.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the authenticated user's ID.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the authenticated user's type.
    /// </summary>
    public UserType UserType { get; set; }
}
