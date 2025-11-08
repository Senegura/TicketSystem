using TicketSystem.DAL;
using TicketSystem.Models;

namespace TicketSystem.BL;

/// <summary>
/// Provides ticket business logic operations including ticket creation.
/// Implements default values for new tickets and delegates persistence to the data access layer.
/// </summary>
public class TicketService : ITicketService
{
    private readonly ITicketDal _ticketDal;

    /// <summary>
    /// Initializes a new instance of the TicketService class.
    /// </summary>
    /// <param name="ticketDal">The data access layer for ticket operations.</param>
    public TicketService(ITicketDal ticketDal)
    {
        _ticketDal = ticketDal;
    }

    /// <summary>
    /// Creates a new ticket with the provided information.
    /// Sets default values for Status (New), Summary (empty), and Resolution (empty).
    /// </summary>
    /// <param name="fullName">The full name of the customer submitting the ticket.</param>
    /// <param name="email">The email address of the customer.</param>
    /// <param name="description">The detailed description of the issue.</param>
    /// <param name="imageFileName">The optional filename of an uploaded image associated with the ticket.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created ticket with assigned Id and timestamps.</returns>
    public async Task<Ticket> CreateTicketAsync(string fullName, string email, string description, string? imageFileName)
    {
        // Create ticket object with provided data and default values
        var ticket = new Ticket
        {
            Name = fullName,
            Email = email,
            Description = description,
            ImageUrl = imageFileName ?? string.Empty,
            Status = TicketStatus.New,
            Summary = string.Empty,
            Resolution = string.Empty
        };

        // Persist ticket to database
        var createdTicket = await _ticketDal.CreateAsync(ticket);

        return createdTicket;
    }
}
