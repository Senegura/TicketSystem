using System.Security.Cryptography;

namespace TicketSystem.BL
{
    /// <summary>
    /// Provides cryptographic services for password hashing and salt generation.
    /// </summary>
    public interface ICryptoService
    {
        /// <summary>
        /// Hashes a password using PBKDF2 with the specified parameters.
        /// </summary>
        /// <param name="password">The plaintext password to hash.</param>
        /// <param name="salt">The cryptographic salt to use in the hashing process.</param>
        /// <param name="iterationCount">The number of iterations for the key derivation function.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use (e.g., SHA256, SHA512).</param>
        /// <returns>A Base64-encoded string representing the hashed password.</returns>
        string HashPassword(string password, byte[] salt, int iterationCount, HashAlgorithmName hashAlgorithm);

        /// <summary>
        /// Generates a cryptographically secure random salt.
        /// </summary>
        /// <param name="size">The size of the salt in bytes. Default is 32 bytes.</param>
        /// <returns>A byte array containing the cryptographically secure random salt.</returns>
        byte[] GenerateSalt(int size = 32);
    }
}
