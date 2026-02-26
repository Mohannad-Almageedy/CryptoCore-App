using System;
using System.Security.Cryptography;
using System.Text;
using CryptoEdu.Core.Modern;

namespace CryptoEdu.Services
{
    /// <summary>
    /// Professional service for generating secure random strings, passwords, and asymmetric keys.
    /// </summary>
    public static class KeyGeneratorService
    {
        private const string AlphanumericChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private const string SpecialChars = "!@#$%^&*()_-+=<>?";

        /// <summary>
        /// Generates a cryptographically secure random string of a specified length.
        /// </summary>
        public static string GenerateSecurePassword(int length, bool includeSpecialCharacters = true)
        {
            if (length < 1)
                throw new ArgumentException("Length must be at least 1.", nameof(length));

            string characterSet = AlphanumericChars;
            if (includeSpecialCharacters)
            {
                characterSet += SpecialChars;
            }

            var sb = new StringBuilder(length);
            
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] byteBuffer = new byte[4];
                while (sb.Length < length)
                {
                    rng.GetBytes(byteBuffer);
                    uint randomValue = BitConverter.ToUInt32(byteBuffer, 0);
                    int index = (int)(randomValue % characterSet.Length);
                    sb.Append(characterSet[index]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates a new 2048-bit RSA Public/Private key pair in XML format.
        /// </summary>
        public static void GenerateRsaKeyPair(out string publicKeyXml, out string privateKeyXml)
        {
            RsaCipher.GenerateKeys(out publicKeyXml, out privateKeyXml);
        }
    }
}
