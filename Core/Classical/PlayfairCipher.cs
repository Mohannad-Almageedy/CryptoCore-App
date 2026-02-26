using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryptoEdu.Core.Interfaces;
using CryptoEdu.Core.Models;

namespace CryptoEdu.Core.Classical
{
    /// <summary>
    /// Implementation of the Playfair Cipher.
    /// Uses a 5x5 matrix and digraph substitution.
    /// </summary>
    public class PlayfairCipher : IClassicalCipher
    {
        public string Name => "Playfair Cipher";

        public string GetMathematicalRule()
        {
            return "1. Generate 5x5 Matrix from key (I/J combined).\n" +
                   "2. Split text into digraphs (pairs). Insert 'X' if a pair has identical letters.\n" +
                   "3. Same Row: Shift Right (Enc) / Left (Dec).\n" +
                   "4. Same Column: Shift Down (Enc) / Up (Dec).\n" +
                   "5. Rectangle: Swap columns.";
        }

        public string Encrypt(string plainText, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be empty.");

            char[,] matrix = GenerateMatrix(key);
            string preparedText = PrepareText(plainText);
            return ProcessText(preparedText, matrix, true);
        }

        public string Decrypt(string cipherText, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be empty.");

            char[,] matrix = GenerateMatrix(key);
            string preparedText = PrepareText(cipherText, padText: false); // ciphertext is already padded/digraphed correctly
            return ProcessText(preparedText, matrix, false);
        }

        public List<CipherStep> GetEncryptionSteps(string plainText, string key)
        {
            var steps = new List<CipherStep>();
            
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be empty.");

            char[,] matrix = GenerateMatrix(key);
            steps.Add(new CipherStep
            {
                Title = "Step 1: Matrix Generation",
                Description = $"Generate 5x5 matrix using the key '{key}'.",
                VisualizationData = MatrixToString(matrix),
                InputState = $"Key: {key}",
                OutputState = "Matrix Generated"
            });

            string preparedText = PrepareText(plainText);
            steps.Add(new CipherStep
            {
                Title = "Step 2: Prepare Plaintext",
                Description = "Remove non-letters, replace 'J' with 'I', insert 'X' between identical consecutive letters, and pad with 'X' if length is odd.",
                InputState = plainText,
                OutputState = preparedText
            });

            var sb = new StringBuilder();
            for (int i = 0; i < preparedText.Length; i += 2)
            {
                char a = preparedText[i];
                char b = preparedText[i + 1];
                string inputDigraph = $"{a}{b}";
                string outputDigraph = ProcessDigraph(a, b, matrix, true, out string rule);

                sb.Append(outputDigraph);

                steps.Add(new CipherStep
                {
                    Title = $"Step {(i / 2) + 3}: Encrypt Digraph '{inputDigraph}'",
                    Description = $"Rule applied: {rule}",
                    FormulaApplied = $"{inputDigraph} -> {outputDigraph}",
                    InputState = preparedText.Substring(0, i + 2),
                    OutputState = sb.ToString()
                });
            }

            return steps;
        }

        public List<CipherStep> GetDecryptionSteps(string cipherText, string key)
        {
            var steps = new List<CipherStep>();
            
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be empty.");

            char[,] matrix = GenerateMatrix(key);
            steps.Add(new CipherStep
            {
                Title = "Step 1: Matrix Generation",
                Description = $"Generate 5x5 matrix using the key '{key}'.",
                VisualizationData = MatrixToString(matrix),
                InputState = $"Key: {key}",
                OutputState = "Matrix Generated"
            });

            string preparedText = PrepareText(cipherText, padText: false);
            var sb = new StringBuilder();
            for (int i = 0; i < preparedText.Length; i += 2)
            {
                char a = preparedText[i];
                char b = preparedText[i + 1];
                string inputDigraph = $"{a}{b}";
                string outputDigraph = ProcessDigraph(a, b, matrix, false, out string rule);

                sb.Append(outputDigraph);

                steps.Add(new CipherStep
                {
                    Title = $"Step {(i / 2) + 2}: Decrypt Digraph '{inputDigraph}'",
                    Description = $"Rule applied: {rule}",
                    FormulaApplied = $"{inputDigraph} -> {outputDigraph}",
                    InputState = preparedText.Substring(0, i + 2),
                    OutputState = sb.ToString()
                });
            }

            return steps;
        }

        private char[,] GenerateMatrix(string key)
        {
            char[,] matrix = new char[5, 5];
            var usedChars = new HashSet<char>();
            
            key = key.ToUpper().Replace("J", "I");
            string alphabet = "ABCDEFGHIKLMNOPQRSTUVWXYZ"; // Without 'J'
            
            int r = 0, c = 0;

            // Fill with key characters
            foreach (char ch in key)
            {
                if (char.IsLetter(ch) && usedChars.Add(ch))
                {
                    matrix[r, c++] = ch;
                    if (c == 5) { c = 0; r++; }
                }
            }

            // Fill with remaining alphabet
            foreach (char ch in alphabet)
            {
                if (usedChars.Add(ch))
                {
                    matrix[r, c++] = ch;
                    if (c == 5) { c = 0; r++; }
                }
            }

            return matrix;
        }

        private string PrepareText(string text, bool padText = true)
        {
            text = text.ToUpper().Replace("J", "I");
            var filteredText = new string(text.Where(char.IsLetter).ToArray());

            if (!padText) return filteredText;

            var sb = new StringBuilder();
            for (int i = 0; i < filteredText.Length; i++)
            {
                sb.Append(filteredText[i]);
                if (i + 1 < filteredText.Length && filteredText[i] == filteredText[i + 1] && sb.Length % 2 != 0)
                {
                    sb.Append('X');
                }
            }

            if (sb.Length % 2 != 0)
            {
                sb.Append('X');
            }

            return sb.ToString();
        }

        private string ProcessText(string text, char[,] matrix, bool encrypt)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < text.Length; i += 2)
            {
                sb.Append(ProcessDigraph(text[i], text[i + 1], matrix, encrypt, out _));
            }
            return sb.ToString();
        }

        private string ProcessDigraph(char a, char b, char[,] matrix, bool encrypt, out string ruleApplied)
        {
            GetPosition(matrix, a, out int r1, out int c1);
            GetPosition(matrix, b, out int r2, out int c2);

            int shift = encrypt ? 1 : 4; // Adding 4 is equivalent to subtracting 1 in modulo 5 arithmetic

            if (r1 == r2)
            {
                ruleApplied = "Same Row: Shifted horizontally.";
                return $"{(matrix[r1, (c1 + shift) % 5])}{(matrix[r2, (c2 + shift) % 5])}";
            }
            else if (c1 == c2)
            {
                ruleApplied = "Same Column: Shifted vertically.";
                return $"{(matrix[(r1 + shift) % 5, c1])}{(matrix[(r2 + shift) % 5, c2])}";
            }
            else
            {
                ruleApplied = "Rectangle: Swapped columns.";
                return $"{(matrix[r1, c2])}{(matrix[r2, c1])}";
            }
        }

        private void GetPosition(char[,] matrix, char target, out int row, out int col)
        {
            for (row = 0; row < 5; row++)
            {
                for (col = 0; col < 5; col++)
                {
                    if (matrix[row, col] == target) return;
                }
            }
            throw new Exception($"Character '{target}' not found in matrix.");
        }

        private string MatrixToString(char[,] matrix)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    sb.Append(matrix[i, j]).Append(" ");
                }
                sb.AppendLine();
            }
            return sb.ToString().Trim();
        }
    }
}
