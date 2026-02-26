using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryptoEdu.Core.Interfaces;
using CryptoEdu.Core.Models;

namespace CryptoEdu.Core.Classical
{
    /// <summary>
    /// Implementation of the Row/Column Transposition Cipher.
    /// Uses a keyword to determine column extraction order.
    /// </summary>
    public class RowColumnTranspositionCipher : IClassicalCipher
    {
        public string Name => "Row/Column Transposition Cipher";

        public string GetMathematicalRule()
        {
            return "Transposition Rule: Write the plaintext row by row into a matrix matching the key length.\n" +
                   "Determine the column sequence by sorting the key alphabetically.\n" +
                   "Read columns vertically according to that sequence to produce the ciphertext.";
        }

        public string Encrypt(string plainText, string key)
        {
            ValidateKey(key);
            string cleanKey = new string(key.ToUpper().Where(char.IsLetter).ToArray());
            string cleanText = PrepareText(plainText, cleanKey.Length);
            
            char[,] matrix = BuildEncryptionMatrix(cleanText, cleanKey.Length);
            int[] columnOrder = GetColumnOrder(cleanKey);

            return ExtractColumns(matrix, columnOrder, cleanKey.Length, cleanText.Length / cleanKey.Length);
        }

        public string Decrypt(string cipherText, string key)
        {
            ValidateKey(key);
            string cleanKey = new string(key.ToUpper().Where(char.IsLetter).ToArray());
            string cleanText = PrepareText(cipherText, cleanKey.Length); // Padding should already be correct, but just in case
            
            int[] columnOrder = GetColumnOrder(cleanKey);
            int rows = cleanText.Length / cleanKey.Length;
            char[,] matrix = BuildDecryptionMatrix(cleanText, columnOrder, cleanKey.Length, rows);

            return ReadMatrixByRow(matrix, cleanKey.Length, rows);
        }

        public List<CipherStep> GetEncryptionSteps(string plainText, string key)
        {
            var steps = new List<CipherStep>();

            try
            {
                ValidateKey(key);
            }
            catch (Exception ex)
            {
                steps.Add(new CipherStep { Title = "Validation Error", Description = ex.Message });
                return steps;
            }

            string cleanKey = new string(key.ToUpper().Where(char.IsLetter).ToArray());
            string cleanText = PrepareText(plainText, cleanKey.Length);
            int cols = cleanKey.Length;
            int rows = cleanText.Length / cols;

            steps.Add(new CipherStep
            {
                Title = "Step 1: Preparation",
                Description = $"Clean plaintext and pad with 'X' to match a multiple of {cols} (key length).",
                InputState = plainText,
                OutputState = cleanText
            });

            char[,] matrix = BuildEncryptionMatrix(cleanText, cols);
            int[] columnOrder = GetColumnOrder(cleanKey);
            string sortedKey = new string(cleanKey.OrderBy(c => c).ToArray());
            
            // Build visual representation
            var sbVis = new StringBuilder();
            sbVis.AppendLine($"Key:       {string.Join(" ", cleanKey.ToCharArray())}");
            sbVis.AppendLine($"Order:     {string.Join(" ", columnOrder.Select(o => o + 1))}");
            sbVis.AppendLine("           " + new string('-', cols * 2 - 1));
            sbVis.Append(MatrixToString(matrix, rows, cols));

            steps.Add(new CipherStep
            {
                Title = "Step 2: Build Matrix",
                Description = "Write plaintext row by row under the key. Determine extraction order by sorting the key alphabetically.",
                VisualizationData = sbVis.ToString().TrimEnd(),
                FormulaApplied = $"Sorted Key: {sortedKey}",
                InputState = $"Key: {cleanKey}",
                OutputState = $"Order: {string.Join(", ", columnOrder.Select(o => o + 1))}"
            });

            string cipherText = ExtractColumns(matrix, columnOrder, cols, rows);

            steps.Add(new CipherStep
            {
                Title = "Step 3: Extract Columns",
                Description = "Read the matrix downward, taking columns in the numerical sequence determined in Step 2.",
                InputState = "Matrix Grid",
                OutputState = cipherText
            });

            return steps;
        }

        public List<CipherStep> GetDecryptionSteps(string cipherText, string key)
        {
            var steps = new List<CipherStep>();

            try
            {
                ValidateKey(key);
            }
            catch (Exception ex)
            {
                steps.Add(new CipherStep { Title = "Validation Error", Description = ex.Message });
                return steps;
            }

            string cleanKey = new string(key.ToUpper().Where(char.IsLetter).ToArray());
            string cleanText = PrepareText(cipherText, cleanKey.Length);
            int cols = cleanKey.Length;
            int rows = cleanText.Length / cols;

            steps.Add(new CipherStep
            {
                Title = "Step 1: Determine Pattern",
                Description = "Calculate column extraction order based on alphabetical sorting of the key.",
                InputState = $"Key: {cleanKey}",
                OutputState = $"Columns: {cols}, Rows: {rows}"
            });

            int[] columnOrder = GetColumnOrder(cleanKey);
            char[,] matrix = BuildDecryptionMatrix(cleanText, columnOrder, cols, rows);

            var sbVis = new StringBuilder();
            sbVis.AppendLine($"Key:       {string.Join(" ", cleanKey.ToCharArray())}");
            sbVis.AppendLine($"Order:     {string.Join(" ", columnOrder.Select(o => o + 1))}");
            sbVis.AppendLine("           " + new string('-', cols * 2 - 1));
            sbVis.Append(MatrixToString(matrix, rows, cols));

            steps.Add(new CipherStep
            {
                Title = "Step 2: Rebuild Matrix",
                Description = "Fill the matrix columns vertically using the ciphertext chunks according to the extraction order.",
                VisualizationData = sbVis.ToString().TrimEnd(),
                InputState = cleanText,
                OutputState = "Matrix Reconstructed"
            });

            string plainText = ReadMatrixByRow(matrix, cols, rows);

            steps.Add(new CipherStep
            {
                Title = "Step 3: Read Row by Row",
                Description = "Read the reconstructed matrix row by row to reveal the plaintext.",
                InputState = "Reconstructed Matrix",
                OutputState = plainText
            });

            return steps;
        }

        private void ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be empty.");
            if (!key.Any(char.IsLetter))
                throw new ArgumentException("Key must contain at least one valid letter.");
        }

        private string PrepareText(string text, int keyLen)
        {
            string clean = new string(text.ToUpper().Where(char.IsLetterOrDigit).ToArray());
            int padding = keyLen - (clean.Length % keyLen);
            if (padding != keyLen)
            {
                clean += new string('X', padding);
            }
            return clean;
        }

        private int[] GetColumnOrder(string key)
        {
            // E.g., Key: HACK -> Sorted: A C H K
            // Ranks assigned based on stable sort of indices
            return key.Select((c, i) => new { Char = c, Index = i })
                      .OrderBy(x => x.Char)
                      .Select(x => x.Index)
                      .ToArray();
        }

        private char[,] BuildEncryptionMatrix(string text, int cols)
        {
            int rows = text.Length / cols;
            char[,] matrix = new char[rows, cols];

            int idx = 0;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    matrix[r, c] = text[idx++];
                }
            }
            return matrix;
        }

        private string ExtractColumns(char[,] matrix, int[] columnOrder, int cols, int rows)
        {
            var sb = new StringBuilder();
            
            // For numbering matching: order array holds the orig index of the sorted characters.
            // Example Key: BAT (B:1, A:0, T:2)
            // columnOrder = [1, 0, 2] -> 1st chunk from col 1 (A), 2nd from col 0 (B), 3rd from col 2 (T).

            for (int i = 0; i < cols; i++)
            {
                int targetCol = columnOrder[i];
                for (int r = 0; r < rows; r++)
                {
                    sb.Append(matrix[r, targetCol]);
                }
            }

            return sb.ToString();
        }

        private char[,] BuildDecryptionMatrix(string text, int[] columnOrder, int cols, int rows)
        {
            char[,] matrix = new char[rows, cols];
            int idx = 0;

            for (int i = 0; i < cols; i++)
            {
                int targetCol = columnOrder[i];
                for (int r = 0; r < rows; r++)
                {
                    matrix[r, targetCol] = text[idx++];
                }
            }

            return matrix;
        }

        private string ReadMatrixByRow(char[,] matrix, int cols, int rows)
        {
            var sb = new StringBuilder();
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    sb.Append(matrix[r, c]);
                }
            }
            return sb.ToString();
        }

        private string MatrixToString(char[,] matrix, int rows, int cols)
        {
            var sb = new StringBuilder();
            for (int r = 0; r < rows; r++)
            {
                sb.Append("           "); // Indent to align with key visualization
                for (int c = 0; c < cols; c++)
                {
                    sb.Append(matrix[r, c]).Append(" ");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
