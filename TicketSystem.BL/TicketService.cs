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

    /// <summary>
    /// Retrieves all tickets from the system.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all tickets.</returns>
    public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
    {
        return await _ticketDal.GetAllAsync();
    }

    /// <summary>
    /// Retrieves a single ticket by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ticket to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the ticket if found, or null if not found.</returns>
    public async Task<Ticket?> GetTicketByIdAsync(Guid id)
    {
        return await _ticketDal.GetByIdAsync(id);
    }

    /// <summary>
    /// Updates an existing ticket with the provided information.
    /// Sets the UpdatedAt timestamp to the current UTC time.
    /// </summary>
    /// <param name="ticket">The ticket object containing updated information.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated ticket if successful, or null if the ticket was not found.</returns>
    public async Task<Ticket?> UpdateTicketAsync(Ticket ticket)
    {
        ticket.UpdatedAt = DateTime.UtcNow;
        var success = await _ticketDal.UpdateAsync(ticket);
        return success ? ticket : null;
    }
}
