using System.Security.Cryptography;

namespace TicketSystem.BL
{
    /// <summary>
    /// Provides cryptographic services for secure password hashing and salt generation.
    /// Implements industry-standard cryptographic operations using PBKDF2 and secure random number generation.
    /// </summary>
    public class CryptoService : ICryptoService
    {
        /// <summary>
        /// Hashes a password using PBKDF2 with the specified salt, iteration count, and hash algorithm.
        /// </summary>
        /// <param name="password">The plaintext password to hash.</param>
        /// <param name="salt">The cryptographic salt to use in the hashing process.</param>
        /// <param name="iterationCount">The number of iterations for the key derivation function.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use (e.g., SHA256, SHA512).</param>
        /// <returns>A Base64-encoded string representing the hashed password.</returns>
        /// <exception cref="ArgumentNullException">Thrown when password is null or empty, or when salt is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when iteration count is less than 1.</exception>
        public string HashPassword(string password, byte[] salt, int iterationCount, HashAlgorithmName hashAlgorithm)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password), "Password cannot be null or empty.");
            }

            if (salt == null)
            {
                throw new ArgumentNullException(nameof(salt), "Salt cannot be null.");
            }

            if (iterationCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(iterationCount), "Iteration count must be greater than 0.");
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterationCount, hashAlgorithm))
            {
                int hashSize = hashAlgorithm.Name switch
                {
                    "SHA256" => 32,
                    "SHA384" => 48,
                    "SHA512" => 64,
                    "SHA1" => 20,
                    _ => 32 // Default to SHA256 size
                };

                byte[] hash = pbkdf2.GetBytes(hashSize);
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Generates a cryptographically secure random salt value.
        /// </summary>
        /// <param name="size">The size of the salt in bytes. Default is 32 bytes (256 bits).</param>
        /// <returns>A byte array containing the cryptographically secure random salt.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when size is less than or equal to 0.</exception>
        public byte[] GenerateSalt(int size = 32)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "Salt size must be greater than 0.");
            }

            byte[] salt = new byte[size];
            RandomNumberGenerator.Fill(salt);
            return salt;
        }
    }
}
