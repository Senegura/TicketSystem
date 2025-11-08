namespace TicketSystem.BL;

using TicketSystem.Models;

/// <summary>
/// Defines the contract for ticket business logic operations.
/// </summary>
public interface ITicketService
{
    /// <summary>
    /// Creates a new ticket with the provided information.
    /// </summary>
    /// <param name="fullName">The full name of the customer submitting the ticket.</param>
    /// <param name="email">The email address of the customer.</param>
    /// <param name="description">The detailed description of the issue.</param>
    /// <param name="imageFileName">The optional filename of an uploaded image associated with the ticket.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created ticket with assigned Id and timestamps.</returns>
    Task<Ticket> CreateTicketAsync(string fullName, string email, string description, string? imageFileName);

    /// <summary>
    /// Retrieves all tickets from the system.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all tickets in the system.</returns>
    Task<IEnumerable<Ticket>> GetAllTicketsAsync();
}
