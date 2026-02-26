using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using CryptoEdu.Core.Interfaces;

namespace CryptoEdu.Core.Modern
{
    /// <summary>
    /// Implementation of Triple DES (3DES).
    /// Note: DES is considered a legacy algorithm; 3DES is included for educational/legacy purposes.
    /// Uses CBC mode, generates a fresh IV per encryption, and prepends it to the base64 ciphertext.
    /// </summary>
    #pragma warning disable SYSLIB0021 // TripleDES is obsolete in .NET 6+ but used here for educational completeness
    public class DesCipher : ICipher
    {
        public string Name => "Triple DES (3DES)";

        public string Encrypt(string plainText, string key)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("Plaintext cannot be empty.");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be empty.");

            byte[] keyBytes = GetValid3DesKey(key);

            using (TripleDES desAlg = TripleDES.Create())
            {
                desAlg.Key = keyBytes;
                desAlg.GenerateIV();
                desAlg.Mode = CipherMode.CBC;
                desAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = desAlg.CreateEncryptor(desAlg.Key, desAlg.IV);
                byte[] encryptedBytes;

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(desAlg.IV, 0, desAlg.IV.Length);

                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encryptedBytes = msEncrypt.ToArray();
                    }
                }

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

            byte[] keyBytes = GetValid3DesKey(key);

            using (TripleDES desAlg = TripleDES.Create())
            {
                int ivLength = desAlg.BlockSize / 8; // Usually 8 bytes for 3DES

                if (fullCipherBytes.Length < ivLength)
                    throw new ArgumentException("Ciphertext is too short to contain a valid IV.");

                byte[] iv = new byte[ivLength];
                Array.Copy(fullCipherBytes, 0, iv, 0, ivLength);

                desAlg.Key = keyBytes;
                desAlg.IV = iv;
                desAlg.Mode = CipherMode.CBC;
                desAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = desAlg.CreateDecryptor(desAlg.Key, desAlg.IV);
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
        /// 3DES typically uses a 24-byte key (192 bits).
        /// We hash the user key via SHA256 and truncate it to the first 24 bytes.
        /// </summary>
        private byte[] GetValid3DesKey(string passphrase)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(passphrase));
                byte[] key24 = new byte[24];
                Array.Copy(hash, 0, key24, 0, 24);
                return key24;
            }
        }
    }
    #pragma warning restore SYSLIB0021
}
