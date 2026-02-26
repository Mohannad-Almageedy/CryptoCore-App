using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using CryptoEdu.Core.Interfaces;

namespace CryptoEdu.Core.Modern
{
    /// <summary>
    /// Professional implementation of AES-256 (Advanced Encryption Standard).
    /// Uses CBC mode, generates a secure random IV per encryption,
    /// and prepends the IV to the final ciphertext for proper decryption.
    /// </summary>
    public class AesCipher : ICipher
    {
        public string Name => "AES-256 (Advanced Encryption Standard)";

        public string Encrypt(string plainText, string key)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("Plaintext cannot be empty.");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be empty.");

            byte[] keyBytes = GetValidAesKey(key);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256;
                aesAlg.Key = keyBytes;
                aesAlg.GenerateIV(); // Produce a fresh IV
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                byte[] encryptedBytes;

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // Prepend IV to the stream
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encryptedBytes = msEncrypt.ToArray();
                    }
                }

                // Return Base-64 encoded string containing IV + Ciphertext
                return Convert.ToBase64String(encryptedBytes);
            }
        }

        public string Decrypt(string cipherText, string key)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("Ciphertext cannot be empty.");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be empty.");

            byte[] fullCipherBytes;
            try
            {
                fullCipherBytes = Convert.FromBase64String(cipherText);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid ciphertext format. Expected Base64 string.");
            }

            byte[] keyBytes = GetValidAesKey(key);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256;
                int ivLength = aesAlg.BlockSize / 8; // Usually 16 bytes for AES

                if (fullCipherBytes.Length < ivLength)
                    throw new ArgumentException("Ciphertext is too short to contain a valid IV.");

                // Extract IV from the beginning
                byte[] iv = new byte[ivLength];
                Array.Copy(fullCipherBytes, 0, iv, 0, ivLength);

                aesAlg.Key = keyBytes;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                string plaintext = null;

                using (MemoryStream msDecrypt = new MemoryStream(fullCipherBytes, ivLength, fullCipherBytes.Length - ivLength))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

                return plaintext;
            }
        }

        /// <summary>
        /// AES-256 requires a 32-byte key.
        /// We use SHA-256 to hash the user's string key into exactly 32 cryptographically pseudo-random bytes.
        /// </summary>
        private byte[] GetValidAesKey(string passphrase)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(passphrase);
                return sha256.ComputeHash(keyBytes);
            }
        }
    }
}
