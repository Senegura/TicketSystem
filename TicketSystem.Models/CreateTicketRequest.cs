namespace TicketSystem.Models;

/// <summary>
/// Represents a request to create a new support ticket.
/// </summary>
public class CreateTicketRequest
{
    /// <summary>
    /// Gets or sets the full name of the person submitting the ticket.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the person submitting the ticket.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the issue being reported.
    /// </summary>
    public string IssueDescription { get; set; } = string.Empty;
}
