using System;
using System.Security.Cryptography;
using System.Text;
using CryptoEdu.Core.Interfaces;

namespace CryptoEdu.Core.Modern
{
    /// <summary>
    /// Professional implementation of the RSA Asymmetric Cipher.
    /// Provides methods for generating Public/Private key pairs and executing encryption.
    /// Uses OAEP padding for optimal security.
    /// </summary>
    public class RsaCipher : ICipher
    {
        public string Name => "RSA (Asymmetric Encryption)";

        /// <summary>
        /// Encrypts the plaintext using the recipient's Public Key.
        /// </summary>
        /// <param name="plainText">The message to encrypt.</param>
        /// <param name="publicKeyXml">The XML string representing the Public Key.</param>
        /// <returns>Base64 encoded ciphertext.</returns>
        public string Encrypt(string plainText, string publicKeyXml)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("Plaintext cannot be empty.");
            if (string.IsNullOrEmpty(publicKeyXml))
                throw new ArgumentException("Public Key cannot be empty.");

            try
            {
                using (RSA rsa = RSA.Create())
                {
                    rsa.FromXmlString(publicKeyXml);
                    
                    byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedData = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.OaepSHA256);
                    
                    return Convert.ToBase64String(encryptedData);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to encrypt using the provided RSA Public Key: {ex.Message}");
            }
        }

        /// <summary>
        /// Decrypts the ciphertext using the recipient's Private Key.
        /// </summary>
        /// <param name="cipherText">The Base64 encoded ciphertext.</param>
        /// <param name="privateKeyXml">The XML string representing the Private Key.</param>
        /// <returns>The original plaintext string.</returns>
        public string Decrypt(string cipherText, string privateKeyXml)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("Ciphertext cannot be empty.");
            if (string.IsNullOrEmpty(privateKeyXml))
                throw new ArgumentException("Private Key cannot be empty.");

            byte[] dataToDecrypt;
            try
            {
                dataToDecrypt = Convert.FromBase64String(cipherText);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid ciphertext format. Expected Base64 string.");
            }

            try
            {
                using (RSA rsa = RSA.Create())
                {
                    rsa.FromXmlString(privateKeyXml);
                    
                    byte[] decryptedData = rsa.Decrypt(dataToDecrypt, RSAEncryptionPadding.OaepSHA256);
                    return Encoding.UTF8.GetString(decryptedData);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to decrypt. Ensure you are using the correct RSA Private Key. Details: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates a new RSA 2048-bit key pair.
        /// </summary>
        /// <param name="publicKey">The newly generated Public Key in XML format.</param>
        /// <param name="privateKey">The newly generated Private Key in XML format.</param>
        public static void GenerateKeys(out string publicKey, out string privateKey)
        {
            using (RSA rsa = RSA.Create(2048))
            {
                publicKey = rsa.ToXmlString(false); // False = Public key only
                privateKey = rsa.ToXmlString(true);   // True = Public + Private key
            }
        }
    }
}
