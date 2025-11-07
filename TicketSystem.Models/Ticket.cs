namespace TicketSystem.Models;

/// <summary>
/// Represents a support ticket in the system.
/// </summary>
public class Ticket
{
    /// <summary>
    /// Gets or sets the unique identifier for the ticket.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the customer who submitted the ticket.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the customer.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the detailed description of the issue.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a brief summary of the issue.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL or path to an uploaded image related to the ticket.
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status of the ticket.
    /// </summary>
    public TicketStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the resolution details for the ticket.
    /// </summary>
    public string Resolution { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the UTC timestamp when the ticket was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the ticket was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
