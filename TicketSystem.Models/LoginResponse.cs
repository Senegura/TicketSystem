namespace TicketSystem.Models;

/// <summary>
/// Represents the response payload for a successful login.
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// Gets or sets the JWT authentication token.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the authenticated user's ID.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the authenticated user's type.
    /// </summary>
    public UserType UserType { get; set; }
}
