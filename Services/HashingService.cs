using System;
using System.Security.Cryptography;
using System.Text;

namespace CryptoEdu.Services
{
    /// <summary>
    /// Professional service for providing one-way cryptographic hashing functions.
    /// Includes Support for MD5, SHA-1, SHA-256, SHA-512, and HMAC-SHA256.
    /// </summary>
    public static class HashingService
    {
        public static string ComputeMD5(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return ToHexString(hashBytes);
            }
        }

        public static string ComputeSHA1(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha1.ComputeHash(inputBytes);
                return ToHexString(hashBytes);
            }
        }

        public static string ComputeSHA256(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return ToHexString(hashBytes);
            }
        }

        public static string ComputeSHA512(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha512.ComputeHash(inputBytes);
                return ToHexString(hashBytes);
            }
        }

        public static string ComputeHMACSHA256(string input, string key)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("HMAC key cannot be empty.");

            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            using (HMACSHA256 hmac = new HMACSHA256(keyBytes))
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = hmac.ComputeHash(inputBytes);
                return ToHexString(hashBytes);
            }
        }

        /// <summary>
        /// Converts a byte array to a lowercase hexadecimal string.
        /// </summary>
        private static string ToHexString(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
