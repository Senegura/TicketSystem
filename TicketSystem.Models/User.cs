namespace TicketSystem.Models;

/// <summary>
/// Represents a user account in the system.
/// </summary>
public class User
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the username for authentication.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of user account.
    /// </summary>
    public UserType UserType { get; set; }

    /// <summary>
    /// Gets or sets the hashed password.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of hash iterations applied.
    /// </summary>
    public int Iterations { get; set; }

    /// <summary>
    /// Gets or sets the salt value used in password hashing.
    /// </summary>
    public string Salt { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the hashing algorithm used.
    /// </summary>
    public string HashAlgorithm { get; set; } = string.Empty;
}
