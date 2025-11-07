using System.Security.Claims;

namespace TicketSystem.BL
{
    /// <summary>
    /// Service interface for JWT token generation and signing operations
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Signs a JWT token with the provided claims and expiration time
        /// </summary>
        /// <param name="claims">Collection of claims to include in the token</param>
        /// <param name="expiresInMinutes">Token expiration time in minutes (default: 60)</param>
        /// <returns>Signed JWT token as a string</returns>
        string SignToken(IEnumerable<Claim> claims, int expiresInMinutes = 60);
    }
}
