using System.Text.Json.Serialization;

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
    /// Serialized as a string instead of an integer.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserType UserType { get; set; }
}
