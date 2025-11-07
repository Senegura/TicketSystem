using TicketSystem.Models;

namespace TicketSystem.DAL;

/// <summary>
/// Defines the contract for ticket data access operations.
/// Provides CRUD operations for managing ticket entities.
/// </summary>
public interface ITicketDal
{
    /// <summary>
    /// Creates a new ticket in the data store.
    /// </summary>
    /// <param name="ticket">The ticket to create. A new Id will be generated.</param>
    /// <returns>The created ticket with its assigned Id and timestamps.</returns>
    Task<Ticket> CreateAsync(Ticket ticket);

    /// <summary>
    /// Retrieves all tickets from the data store.
    /// </summary>
    /// <returns>A collection of all tickets, or an empty collection if none exist.</returns>
    Task<IEnumerable<Ticket>> GetAllAsync();

    /// <summary>
    /// Retrieves a specific ticket by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ticket to retrieve.</param>
    /// <returns>The ticket if found; otherwise, null.</returns>
    Task<Ticket?> GetByIdAsync(Guid id);

    /// <summary>
    /// Updates an existing ticket in the data store.
    /// </summary>
    /// <param name="ticket">The ticket with updated values. The Id must match an existing ticket.</param>
    /// <returns>True if the ticket was updated successfully; false if the ticket was not found.</returns>
    Task<bool> UpdateAsync(Ticket ticket);

    /// <summary>
    /// Deletes a ticket from the data store.
    /// </summary>
    /// <param name="id">The unique identifier of the ticket to delete.</param>
    /// <returns>True if the ticket was deleted successfully; false if the ticket was not found.</returns>
    Task<bool> DeleteAsync(Guid id);
}
