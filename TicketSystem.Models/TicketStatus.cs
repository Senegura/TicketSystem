namespace TicketSystem.Models;

/// <summary>
/// Represents the current status of a support ticket in the system.
/// </summary>
public enum TicketStatus
{
    /// <summary>
    /// Newly created ticket that has not been assigned or worked on yet.
    /// </summary>
    New = 0,

    /// <summary>
    /// Ticket is currently being worked on by support staff.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Issue has been resolved and is awaiting final closure.
    /// </summary>
    Resolved = 2,

    /// <summary>
    /// Ticket has been closed and no further action is required.
    /// </summary>
    Closed = 3
}
