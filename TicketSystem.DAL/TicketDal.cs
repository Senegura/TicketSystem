using System.Text.Json;
using System.Text.Json.Serialization;
using TicketSystem.Models;

namespace TicketSystem.DAL;

/// <summary>
/// JSON file-based implementation of the ticket data access layer.
/// Stores ticket data in a JSON file with camelCase property names.
/// </summary>
public class TicketDal : ITicketDal
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the TicketDal class with the default file path.
    /// </summary>
    public TicketDal() : this(Path.Combine("App_Data", "tickets.json"))
    {
    }

    /// <summary>
    /// Initializes a new instance of the TicketDal class with a custom file path.
    /// </summary>
    /// <param name="filePath">The path to the JSON file for storing tickets.</param>
    public TicketDal(string filePath)
    {
        _filePath = filePath;
        
        // Configure JSON serialization options
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }

    /// <summary>
    /// Ensures the App_Data directory exists, creating it if necessary.
    /// </summary>
    private void EnsureDirectoryExists()
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    /// <summary>
    /// Reads tickets from the JSON file with proper file locking and retry logic.
    /// </summary>
    /// <returns>A list of tickets from the file, or an empty list if the file doesn't exist.</returns>
    private async Task<List<Ticket>> ReadTicketsWithLockingAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<Ticket>();
        }

        const int maxRetries = 3;
        var delays = new[] { 100, 200, 400 }; // Exponential backoff delays in milliseconds

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                // Use FileStream with FileShare.Read to allow concurrent reads
                using var fileStream = new FileStream(
                    _filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 4096,
                    useAsync: true);

                var tickets = await JsonSerializer.DeserializeAsync<List<Ticket>>(fileStream, _jsonOptions);
                return tickets ?? new List<Ticket>();
            }
            catch (IOException) when (attempt < maxRetries - 1)
            {
                // File is locked, wait and retry with exponential backoff
                await Task.Delay(delays[attempt]);
            }
            catch (JsonException ex)
            {
                // Corrupted JSON file - log error and return empty list
                // Future: integrate with ILogger
                Console.Error.WriteLine($"Corrupted JSON file at {_filePath}: {ex.Message}");
                return new List<Ticket>();
            }
            catch (UnauthorizedAccessException ex)
            {
                // Insufficient permissions - throw with clear message
                throw new UnauthorizedAccessException($"Access denied to file: {_filePath}", ex);
            }
        }

        // All retry attempts failed
        throw new IOException($"Unable to read file after {maxRetries} attempts. File may be locked: {_filePath}");
    }

    /// <summary>
    /// Writes tickets to the JSON file with proper file locking and atomic operations.
    /// </summary>
    /// <param name="tickets">The list of tickets to write.</param>
    private async Task WriteTicketsWithLockingAsync(List<Ticket> tickets)
    {
        EnsureDirectoryExists();

        const int maxRetries = 3;
        var delays = new[] { 100, 200, 400 }; // Exponential backoff delays in milliseconds

        // Write to a temporary file first for atomic operations
        var tempFilePath = _filePath + ".tmp";

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                // Write to temporary file with exclusive access
                using (var fileStream = new FileStream(
                    tempFilePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 4096,
                    useAsync: true))
                {
                    await JsonSerializer.SerializeAsync(fileStream, tickets, _jsonOptions);
                    await fileStream.FlushAsync();
                }

                // Atomic replace: delete old file and rename temp file
                if (File.Exists(_filePath))
                {
                    File.Delete(_filePath);
                }
                File.Move(tempFilePath, _filePath);

                return; // Success
            }
            catch (IOException) when (attempt < maxRetries - 1)
            {
                // File is locked, wait and retry with exponential backoff
                await Task.Delay(delays[attempt]);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Insufficient permissions - clean up temp file and throw
                if (File.Exists(tempFilePath))
                {
                    try { File.Delete(tempFilePath); } catch { /* Ignore cleanup errors */ }
                }
                throw new UnauthorizedAccessException($"Access denied to file: {_filePath}", ex);
            }
            catch (Exception)
            {
                // Clean up temp file on any other error
                if (File.Exists(tempFilePath))
                {
                    try { File.Delete(tempFilePath); } catch { /* Ignore cleanup errors */ }
                }
                throw;
            }
        }

        // Clean up temp file if all retries failed
        if (File.Exists(tempFilePath))
        {
            try { File.Delete(tempFilePath); } catch { /* Ignore cleanup errors */ }
        }

        // All retry attempts failed
        throw new IOException($"Unable to write file after {maxRetries} attempts. File may be locked: {_filePath}");
    }

    /// <summary>
    /// Creates a new ticket in the JSON store.
    /// Generates a new GUID for the ticket Id and sets timestamps.
    /// </summary>
    /// <param name="ticket">The ticket to create.</param>
    /// <returns>The created ticket with assigned Id and timestamps.</returns>
    public async Task<Ticket> CreateAsync(Ticket ticket)
    {
        try
        {
            // Generate new GUID for ticket Id
            ticket.Id = Guid.NewGuid();
            
            // Set CreatedAt and UpdatedAt to current UTC timestamp
            var now = DateTime.UtcNow;
            ticket.CreatedAt = now;
            ticket.UpdatedAt = now;
            
            // Load existing tickets from JSON file with proper locking
            var tickets = await ReadTicketsWithLockingAsync();
            
            // Add new ticket to collection
            tickets.Add(ticket);
            
            // Serialize and save to JSON file with proper locking and atomic operations
            await WriteTicketsWithLockingAsync(tickets);
            
            // Return created ticket
            return ticket;
        }
        catch (IOException ex)
        {
            throw new IOException($"Failed to create ticket due to file access error: {ex.Message}", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException($"Failed to create ticket due to insufficient permissions: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new JsonException($"Failed to create ticket due to JSON serialization error: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Retrieves all tickets from the JSON store.
    /// Returns an empty collection if the file doesn't exist.
    /// </summary>
    /// <returns>A collection of all tickets.</returns>
    public async Task<IEnumerable<Ticket>> GetAllAsync()
    {
        try
        {
            // Read tickets with proper file locking and retry logic
            return await ReadTicketsWithLockingAsync();
        }
        catch (IOException ex)
        {
            // Handle file access exceptions gracefully
            // Log the exception (future: integrate with ILogger)
            Console.Error.WriteLine($"Failed to read tickets: {ex.Message}");
            return new List<Ticket>();
        }
        catch (UnauthorizedAccessException ex)
        {
            // Handle permission exceptions gracefully
            Console.Error.WriteLine($"Access denied when reading tickets: {ex.Message}");
            return new List<Ticket>();
        }
    }

    /// <summary>
    /// Retrieves a specific ticket by its Id from the JSON store.
    /// </summary>
    /// <param name="id">The unique identifier of the ticket to retrieve.</param>
    /// <returns>The matching ticket if found, otherwise null.</returns>
    public async Task<Ticket?> GetByIdAsync(Guid id)
    {
        // Load all tickets from JSON file
        var tickets = await GetAllAsync();
        
        // Search for ticket matching the provided Id
        var ticket = tickets.FirstOrDefault(t => t.Id == id);
        
        // Return matching ticket if found, null if ticket not found
        return ticket;
    }

    /// <summary>
    /// Updates an existing ticket in the JSON store.
    /// Sets the UpdatedAt timestamp to the current UTC time.
    /// </summary>
    /// <param name="ticket">The ticket with updated values.</param>
    /// <returns>True if the ticket was found and updated, false if not found.</returns>
    public async Task<bool> UpdateAsync(Ticket ticket)
    {
        try
        {
            // Load all tickets from JSON file with proper locking
            var tickets = await ReadTicketsWithLockingAsync();
            
            // Find ticket by Id in collection
            var existingTicket = tickets.FirstOrDefault(t => t.Id == ticket.Id);
            
            // Return false if ticket not found
            if (existingTicket == null)
            {
                return false;
            }
            
            // Update ticket properties with new values
            existingTicket.Name = ticket.Name;
            existingTicket.Email = ticket.Email;
            existingTicket.Description = ticket.Description;
            existingTicket.Summary = ticket.Summary;
            existingTicket.ImageUrl = ticket.ImageUrl;
            existingTicket.Status = ticket.Status;
            existingTicket.Resolution = ticket.Resolution;
            
            // Set UpdatedAt to current UTC timestamp
            existingTicket.UpdatedAt = DateTime.UtcNow;
            
            // Serialize and save updated collection to JSON file with proper locking
            await WriteTicketsWithLockingAsync(tickets);
            
            // Return true on success
            return true;
        }
        catch (IOException ex)
        {
            throw new IOException($"Failed to update ticket due to file access error: {ex.Message}", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException($"Failed to update ticket due to insufficient permissions: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new JsonException($"Failed to update ticket due to JSON serialization error: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Deletes a ticket from the JSON store by its Id.
    /// </summary>
    /// <param name="id">The unique identifier of the ticket to delete.</param>
    /// <returns>True if the ticket was found and deleted, false if not found.</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            // Load all tickets from JSON file with proper locking
            var tickets = await ReadTicketsWithLockingAsync();
            
            // Find ticket by Id in collection
            var ticketToDelete = tickets.FirstOrDefault(t => t.Id == id);
            
            // Return false if ticket not found
            if (ticketToDelete == null)
            {
                return false;
            }
            
            // Remove ticket from collection
            tickets.Remove(ticketToDelete);
            
            // Serialize and save updated collection to JSON file with proper locking
            await WriteTicketsWithLockingAsync(tickets);
            
            // Return true on success
            return true;
        }
        catch (IOException ex)
        {
            throw new IOException($"Failed to delete ticket due to file access error: {ex.Message}", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException($"Failed to delete ticket due to insufficient permissions: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new JsonException($"Failed to delete ticket due to JSON serialization error: {ex.Message}", ex);
        }
    }
}
