namespace TicketSystem.Models;

/// <summary>
/// Defines the types of users in the system.
/// </summary>
public enum UserType
{
    /// <summary>
    /// A customer who can submit and view their own tickets.
    /// </summary>
    Customer = 0,

    /// <summary>
    /// A regular user who can manage tickets.
    /// </summary>
    User = 1,

    /// <summary>
    /// An administrator with full system access.
    /// </summary>
    Admin = 2
}
