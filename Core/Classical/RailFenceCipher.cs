using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryptoEdu.Core.Interfaces;
using CryptoEdu.Core.Models;

namespace CryptoEdu.Core.Classical
{
    /// <summary>
    /// Implementation of the Rail Fence Cipher (Transposition Cipher).
    /// </summary>
    public class RailFenceCipher : IClassicalCipher
    {
        public string Name => "Rail Fence Cipher";

        public string GetMathematicalRule()
        {
            return "Transposition Rule: Write the plaintext downwards and diagonally on successive 'rails' of an imaginary fence, then reading off each row successively. The key is the number of rails.";
        }

        public string Encrypt(string plainText, string key)
        {
            if (!int.TryParse(key, out int rails) || rails < 2)
                throw new ArgumentException("Key for Rail Fence cipher must be an integer >= 2.");

            string cleanText = PrepareText(plainText);
            
            char[,] grid = BuildEncryptionGrid(cleanText, rails);
            return ReadGridByRow(grid, rails, cleanText.Length);
        }

        public string Decrypt(string cipherText, string key)
        {
            if (!int.TryParse(key, out int rails) || rails < 2)
                throw new ArgumentException("Key for Rail Fence cipher must be an integer >= 2.");

            string cleanText = PrepareText(cipherText);
            
            char[,] grid = BuildDecryptionGrid(cleanText, rails);
            return ReadGridZigZag(grid, rails, cleanText.Length);
        }

        public List<CipherStep> GetEncryptionSteps(string plainText, string key)
        {
            var steps = new List<CipherStep>();
            
            if (!int.TryParse(key, out int rails) || rails < 2)
            {
                steps.Add(new CipherStep { Title = "Error", Description = "Key must be an integer >= 2." });
                return steps;
            }

            string cleanText = PrepareText(plainText);
            
            steps.Add(new CipherStep
            {
                Title = "Step 1: Preparation",
                Description = "Clean the plaintext (remove spaces/punctuation) to prepare for transposition.",
                InputState = plainText,
                OutputState = cleanText
            });

            char[,] grid = BuildEncryptionGrid(cleanText, rails);
            string visualGrid = GridToString(grid, rails, cleanText.Length);
            
            steps.Add(new CipherStep
            {
                Title = "Step 2: Zig-Zag Grid Generation",
                Description = $"Place characters diagonally up and down across {rails} rails.",
                VisualizationData = visualGrid,
                InputState = $"Rails: {rails}",
                OutputState = "Grid built"
            });

            string cipherText = ReadGridByRow(grid, rails, cleanText.Length);

            steps.Add(new CipherStep
            {
                Title = "Step 3: Read Row by Row",
                Description = "Read the grid from left to right, line by line, to form the ciphertext.",
                FormulaApplied = "Concat(Row_1, Row_2, ..., Row_K)",
                InputState = "Completed Grid",
                OutputState = cipherText
            });

            return steps;
        }

        public List<CipherStep> GetDecryptionSteps(string cipherText, string key)
        {
            var steps = new List<CipherStep>();
            
            if (!int.TryParse(key, out int rails) || rails < 2)
            {
                steps.Add(new CipherStep { Title = "Error", Description = "Key must be an integer >= 2." });
                return steps;
            }

            string cleanText = PrepareText(cipherText);
            
            steps.Add(new CipherStep
            {
                Title = "Step 1: Preparation",
                Description = "Clean the ciphertext (remove spaces/punctuation) to prepare for transposition.",
                InputState = cipherText,
                OutputState = cleanText
            });

            char[,] grid = BuildDecryptionGrid(cleanText, rails);
            string visualGrid = GridToString(grid, rails, cleanText.Length);
            
            steps.Add(new CipherStep
            {
                Title = "Step 2: Grid Reconstruction",
                Description = "Mark the zig-zag path, then fill those spots row by row using the ciphertext.",
                VisualizationData = visualGrid,
                InputState = $"Rails: {rails}",
                OutputState = "Grid reconstructed"
            });

            string plainText = ReadGridZigZag(grid, rails, cleanText.Length);

            steps.Add(new CipherStep
            {
                Title = "Step 3: Read Zig-Zag Path",
                Description = "Trace the zig-zag path to read the original plaintext.",
                InputState = "Reconstructed Grid",
                OutputState = plainText
            });

            return steps;
        }

        private string PrepareText(string text)
        {
            return new string(text.ToUpper().Where(char.IsLetterOrDigit).ToArray());
        }

        private char[,] BuildEncryptionGrid(string text, int rails)
        {
            char[,] grid = new char[rails, text.Length];
            
            for (int r = 0; r < rails; r++)
                for (int c = 0; c < text.Length; c++)
                    grid[r, c] = '.';

            int row = 0;
            bool dirDown = false;

            for (int i = 0; i < text.Length; i++)
            {
                if (row == 0 || row == rails - 1)
                    dirDown = !dirDown;

                grid[row, i] = text[i];
                row += dirDown ? 1 : -1;
            }

            return grid;
        }

        private char[,] BuildDecryptionGrid(string text, int rails)
        {
            char[,] grid = new char[rails, text.Length];
            
            for (int r = 0; r < rails; r++)
                for (int c = 0; c < text.Length; c++)
                    grid[r, c] = '\n';

            // Phase 1: Mark the zig-zag path
            int row = 0;
            bool dirDown = false;

            for (int i = 0; i < text.Length; i++)
            {
                if (row == 0) dirDown = true;
                if (row == rails - 1) dirDown = false;

                grid[row, i] = '*';
                row += dirDown ? 1 : -1;
            }

            // Phase 2: Fill the marked spots row by row
            int index = 0;
            for (int r = 0; r < rails; r++)
            {
                for (int c = 0; c < text.Length; c++)
                {
                    if (grid[r, c] == '*' && index < text.Length)
                    {
                        grid[r, c] = text[index++];
                    }
                    else if (grid[r, c] != '*')
                    {
                        grid[r, c] = '.';
                    }
                }
            }

            return grid;
        }

        private string ReadGridByRow(char[,] grid, int rails, int length)
        {
            var sb = new StringBuilder();
            for (int r = 0; r < rails; r++)
            {
                for (int c = 0; c < length; c++)
                {
                    if (grid[r, c] != '.')
                        sb.Append(grid[r, c]);
                }
            }
            return sb.ToString();
        }

        private string ReadGridZigZag(char[,] grid, int rails, int length)
        {
            var sb = new StringBuilder();
            int row = 0;
            bool dirDown = false;

            for (int i = 0; i < length; i++)
            {
                if (row == 0) dirDown = true;
                if (row == rails - 1) dirDown = false;

                if (grid[row, i] != '.')
                    sb.Append(grid[row, i]);
                    
                row += dirDown ? 1 : -1;
            }

            return sb.ToString();
        }

        private string GridToString(char[,] grid, int rails, int length)
        {
            var sb = new StringBuilder();
            for (int r = 0; r < rails; r++)
            {
                for (int c = 0; c < length; c++)
                {
                    sb.Append(grid[r, c]).Append(" ");
                }
                sb.AppendLine();
            }
            return sb.ToString().Trim();
        }
    }
}
