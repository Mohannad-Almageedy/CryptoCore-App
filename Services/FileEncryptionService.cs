using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoEdu.Services
{
    /// <summary>
    /// Professional service for handling file-based cryptography.
    /// Utilizes streaming to ensure large files do not consume excessive RAM.
    /// </summary>
    public static class FileEncryptionService
    {
        // 8192 is an optimal buffer size for file I/O operations
        private const int BufferSize = 1048576; // 1 MB buffer for fast chunks

        /// <summary>
        /// Encrypts a file using AES-256 in CBC mode, prepending the IV to the output file.
        /// </summary>
        public static async Task EncryptFileAsync(string inputFilePath, string outputFilePath, string password, IProgress<int> progress)
        {
            if (!File.Exists(inputFilePath))
                throw new FileNotFoundException("Input file not found.", inputFilePath);

            byte[] keyBytes = DeriveAesKey(password);
            long totalBytes = new FileInfo(inputFilePath).Length;
            long processedBytes = 0;

            using (FileStream fsSrc = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            using (FileStream fsDest = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.GenerateIV(); // Unique IV per file

                // 1. Write IV to beginning of the destination stream
                await fsDest.WriteAsync(aesAlg.IV, 0, aesAlg.IV.Length);

                // 2. Encrypt and stream
                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                using (CryptoStream cs = new CryptoStream(fsDest, encryptor, CryptoStreamMode.Write))
                {
                    byte[] buffer = new byte[BufferSize];
                    int bytesRead;

                    while ((bytesRead = await fsSrc.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await cs.WriteAsync(buffer, 0, bytesRead);
                        processedBytes += bytesRead;

                        if (totalBytes > 0 && progress != null)
                        {
                            int percentage = (int)((processedBytes * 100) / totalBytes);
                            progress.Report(percentage);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decrypts a file encrypted by EncryptFileAsync, automatically extracting the leading IV.
        /// </summary>
        public static async Task DecryptFileAsync(string inputFilePath, string outputFilePath, string password, IProgress<int> progress)
        {
            if (!File.Exists(inputFilePath))
                throw new FileNotFoundException("Input file not found.", inputFilePath);

            byte[] keyBytes = DeriveAesKey(password);
            long totalBytes = new FileInfo(inputFilePath).Length;
            long processedBytes = 0;

            using (FileStream fsSrc = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            using (FileStream fsDest = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            using (Aes aesAlg = Aes.Create())
            {
                int ivLength = aesAlg.BlockSize / 8; // 16 bytes
                if (fsSrc.Length < ivLength)
                    throw new Exception("Encrypted file is too small to contain a valid IV.");

                // 1. Read IV from the beginning
                byte[] iv = new byte[ivLength];
                int readIv = await fsSrc.ReadAsync(iv, 0, iv.Length);
                if (readIv != ivLength)
                    throw new Exception("Failed to read the complete IV from the file.");

                aesAlg.Key = keyBytes;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                // Adjust processed bytes for correct progress parsing
                processedBytes += ivLength;

                // 2. Decrypt and stream
                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                using (CryptoStream cs = new CryptoStream(fsSrc, decryptor, CryptoStreamMode.Read))
                {
                    byte[] buffer = new byte[BufferSize];
                    int bytesRead;

                    while ((bytesRead = await cs.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fsDest.WriteAsync(buffer, 0, bytesRead);
                        processedBytes += bytesRead;

                        if (totalBytes > 0 && progress != null)
                        {
                            int percentage = (int)((processedBytes * 100) / totalBytes);
                            progress.Report(percentage);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Derives a consistent 32-byte AES key using SHA256 hashing of the user password.
        /// </summary>
        private static byte[] DeriveAesKey(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
