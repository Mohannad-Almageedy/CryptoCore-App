using System.Collections.Generic;
using CryptoEdu.Core.Models;

namespace CryptoEdu.Core.Interfaces
{
    /// <summary>
    /// Represents a classical cipher algorithm that supports step-by-step tracing for educational purposes.
    /// </summary>
    public interface IClassicalCipher : ICipher
    {
        /// <summary>
        /// Gets the principal mathematical rule or formula for this cipher.
        /// </summary>
        string GetMathematicalRule();

        /// <summary>
        /// Encrypts the plaintext and returns a step-by-step trace of the operation.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="key">The encryption key.</param>
        /// <returns>A list of steps explaining how the encryption was performed.</returns>
        List<CipherStep> GetEncryptionSteps(string plainText, string key);

        /// <summary>
        /// Decrypts the ciphertext and returns a step-by-step trace of the operation.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <param name="key">The decryption key.</param>
        /// <returns>A list of steps explaining how the decryption was performed.</returns>
        List<CipherStep> GetDecryptionSteps(string cipherText, string key);
    }
}
