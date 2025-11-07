using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace TicketSystem.BL
{
    /// <summary>
    /// Service implementation for JWT token generation and signing operations
    /// </summary>
    public class JwtService : IJwtService
    {
        // Hard-coded secret key for signing tokens (256-bit cryptographically strong key)
        private const string SecretKey = "Kj8mN2pQ4rT6wY9zB3cF5gH7jK0lM3nP6qS8tV1xZ4aC";
        
        // Default token configuration
        private const string Issuer = "TicketSystem";
        private const string Audience = "TicketSystemUsers";

        /// <summary>
        /// Signs a JWT token with the provided claims and expiration time
        /// </summary>
        /// <param name="claims">Collection of claims to include in the token</param>
        /// <param name="expiresInMinutes">Token expiration time in minutes (default: 60)</param>
        /// <returns>Signed JWT token as a string</returns>
        public string SignToken(IEnumerable<Claim> claims, int expiresInMinutes = 60)
        {
            // Convert secret key to byte array and create SymmetricSecurityKey
            var keyBytes = Encoding.UTF8.GetBytes(SecretKey);
            var securityKey = new SymmetricSecurityKey(keyBytes);

            // Create SigningCredentials with HMAC-SHA256 algorithm
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Use default expiration if invalid value provided
            if (expiresInMinutes <= 0)
            {
                expiresInMinutes = 60;
            }

            // Build JwtSecurityToken with claims, issuer, audience, and expiration
            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: signingCredentials
            );

            // Use JwtSecurityTokenHandler to serialize token to string
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);

            // Return the signed token string
            return tokenString;
        }
    }
}
