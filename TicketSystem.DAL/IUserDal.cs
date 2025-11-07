using TicketSystem.Models;

namespace TicketSystem.DAL;

/// <summary>
/// Defines the contract for user data access operations.
/// Provides CRUD operations for managing user entities.
/// </summary>
public interface IUserDal
{
    /// <summary>
    /// Creates a new user in the data store.
    /// </summary>
    /// <param name="user">The user to create. The Id will be auto-generated.</param>
    /// <returns>The created user with its assigned Id.</returns>
    Task<User> CreateAsync(User user);

    /// <summary>
    /// Retrieves all users from the data store.
    /// </summary>
    /// <returns>A collection of all users, or an empty collection if none exist.</returns>
    Task<IEnumerable<User>> GetAllAsync();

    /// <summary>
    /// Retrieves a specific user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a specific user by their username.
    /// </summary>
    /// <param name="username">The username of the user to retrieve.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetByUsernameAsync(string username);

    /// <summary>
    /// Updates an existing user in the data store.
    /// </summary>
    /// <param name="user">The user with updated values. The Id must match an existing user.</param>
    /// <returns>True if the user was updated successfully; false if the user was not found.</returns>
    Task<bool> UpdateAsync(User user);

    /// <summary>
    /// Deletes a user from the data store.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>True if the user was deleted successfully; false if the user was not found.</returns>
    Task<bool> DeleteAsync(int id);
}
