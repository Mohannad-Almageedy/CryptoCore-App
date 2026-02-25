namespace CryptoEdu.Core.Interfaces
{
    /// <summary>
    /// The base interface for all cryptographic algorithms in CryptoEdu.
    /// </summary>
    public interface ICipher
    {
        /// <summary>
        /// Gets the name of the cipher.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Encrypts the given plaintext using the provided key.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="key">The key to use for encryption.</param>
        /// <returns>The encrypted ciphertext.</returns>
        string Encrypt(string plainText, string key);

        /// <summary>
        /// Decrypts the given ciphertext using the provided key.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <param name="key">The key to use for decryption.</param>
        /// <returns>The decrypted plaintext.</returns>
        string Decrypt(string cipherText, string key);
    }
}
