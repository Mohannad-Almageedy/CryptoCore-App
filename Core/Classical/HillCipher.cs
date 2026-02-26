using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryptoEdu.Core.Interfaces;
using CryptoEdu.Core.Models;

namespace CryptoEdu.Core.Classical
{
    /// <summary>
    /// Implementation of the Hill Cipher.
    /// Supports 2x2 and 3x3 matrices using string keys (length 4 or 9).
    /// </summary>
    public class HillCipher : IClassicalCipher
    {
        public string Name => "Hill Cipher";

        public string GetMathematicalRule()
        {
            return "Encryption: C = (K * P) mod 26\n" +
                   "Decryption: P = (K^-1 * C) mod 26\n" +
                   "Where K is an NxN matrix (usually 2x2 or 3x3) converted from the key string.";
        }

        public string Encrypt(string plainText, string key)
        {
            int[,] keyMatrix = BuildKeyMatrix(key);
            int n = keyMatrix.GetLength(0);
            
            string preparedText = PrepareText(plainText, n);
            return ProcessText(preparedText, keyMatrix, n);
        }

        public string Decrypt(string cipherText, string key)
        {
            int[,] keyMatrix = BuildKeyMatrix(key);
            int n = keyMatrix.GetLength(0);
            
            int det = CalculateDeterminant(keyMatrix, n);
            int detInv = ModInverse(det, 26);
            if (detInv == -1)
                throw new Exception($"The key matrix is not invertible modulo 26. Determinant: {det}");

            int[,] inverseMatrix = GetInverseMatrix(keyMatrix, n, detInv);
            
            string preparedText = PrepareText(cipherText, n);
            return ProcessText(preparedText, inverseMatrix, n);
        }

        public List<CipherStep> GetEncryptionSteps(string plainText, string key)
        {
            var steps = new List<CipherStep>();
            
            int[,] keyMatrix = BuildKeyMatrix(key);
            int n = keyMatrix.GetLength(0);
            
            steps.Add(new CipherStep
            {
                Title = "Step 1: Key Matrix Generation",
                Description = $"Generate {n}x{n} matrix from key '{key}'. A=0, B=1, ... Z=25.",
                VisualizationData = MatrixToString(keyMatrix, n),
                InputState = $"Key: {key}",
                OutputState = $"{n}x{n} Matrix created"
            });

            string preparedText = PrepareText(plainText, n);
            steps.Add(new CipherStep
            {
                Title = "Step 2: Plaintext Formatting",
                Description = $"Remove non-letters and pad with 'X' so the length is a multiple of {n}.",
                InputState = plainText,
                OutputState = preparedText
            });

            var sb = new StringBuilder();
            
            for (int i = 0; i < preparedText.Length; i += n)
            {
                string block = preparedText.Substring(i, n);
                int[] vector = block.Select(c => c - 'A').ToArray();
                int[] resultVector = MultiplyMatrixVector(keyMatrix, vector, n);
                
                string encryptedBlock = new string(resultVector.Select(v => (char)(v + 'A')).ToArray());
                sb.Append(encryptedBlock);

                string formula = $"[ {string.Join(", ", vector)} ] * Matrix mod 26 = [ {string.Join(", ", resultVector)} ] = '{encryptedBlock}'";

                steps.Add(new CipherStep
                {
                    Title = $"Step {(i / n) + 3}: Multiply Block '{block}'",
                    Description = $"Convert '{block}' to numbers, multiply by the key matrix, apply modulo 26, and convert back to letters.",
                    FormulaApplied = formula,
                    InputState = preparedText.Substring(0, i + n),
                    OutputState = sb.ToString()
                });
            }

            return steps;
        }

        public List<CipherStep> GetDecryptionSteps(string cipherText, string key)
        {
            var steps = new List<CipherStep>();
            
            int[,] keyMatrixInit = BuildKeyMatrix(key);
            int n = keyMatrixInit.GetLength(0);

            steps.Add(new CipherStep
            {
                Title = "Step 1: Original Key Matrix",
                Description = $"Generated {n}x{n} matrix from key.",
                VisualizationData = MatrixToString(keyMatrixInit, n),
                InputState = $"Key: {key}",
                OutputState = "Matrix Generated"
            });

            int det = CalculateDeterminant(keyMatrixInit, n);
            int detInv = ModInverse(det, 26);
            if (detInv == -1)
                throw new Exception("Key matrix is not invertible mod 26.");

            int[,] inverseMatrix = GetInverseMatrix(keyMatrixInit, n, detInv);

            steps.Add(new CipherStep
            {
                Title = "Step 2: Inverse Matrix Calculation",
                Description = $"Calculate the inverse of the key matrix modulo 26 using DetInv={detInv}.",
                VisualizationData = MatrixToString(inverseMatrix, n),
                FormulaApplied = "K^-1 = det(K)^-1 * adj(K) mod 26",
                InputState = "Original Matrix",
                OutputState = "Inverse Matrix generated"
            });

            string preparedText = PrepareText(cipherText, n);
            var sb = new StringBuilder();

            for (int i = 0; i < preparedText.Length; i += n)
            {
                string block = preparedText.Substring(i, n);
                int[] vector = block.Select(c => c - 'A').ToArray();
                int[] resultVector = MultiplyMatrixVector(inverseMatrix, vector, n);
                
                string decryptedBlock = new string(resultVector.Select(v => (char)(v + 'A')).ToArray());
                sb.Append(decryptedBlock);

                string formula = $"[ {string.Join(", ", vector)} ] * InvMatrix mod 26 = [ {string.Join(", ", resultVector)} ] = '{decryptedBlock}'";

                steps.Add(new CipherStep
                {
                    Title = $"Step {(i / n) + 3}: Multiply Block '{block}'",
                    Description = $"Multiply by inverse matrix modulo 26.",
                    FormulaApplied = formula,
                    InputState = preparedText.Substring(0, i + n),
                    OutputState = sb.ToString()
                });
            }

            return steps;
        }

        private int[,] BuildKeyMatrix(string key)
        {
            key = new string(key.ToUpper().Where(char.IsLetter).ToArray());
            int n = (int)Math.Sqrt(key.Length);
            
            if (n * n != key.Length || (n != 2 && n != 3))
                throw new ArgumentException("Key length must be 4 (for 2x2) or 9 (for 3x3 matrix) pure letters.");

            int[,] matrix = new int[n, n];
            int idx = 0;
            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    matrix[r, c] = key[idx++] - 'A';
                }
            }
            return matrix;
        }

        private string PrepareText(string text, int n)
        {
            string clean = new string(text.ToUpper().Where(char.IsLetter).ToArray());
            int padding = n - (clean.Length % n);
            if (padding != n)
            {
                clean += new string('X', padding);
            }
            return clean;
        }

        private string ProcessText(string text, int[,] matrix, int n)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < text.Length; i += n)
            {
                int[] vector = text.Substring(i, n).Select(c => c - 'A').ToArray();
                int[] result = MultiplyMatrixVector(matrix, vector, n);
                foreach (int val in result)
                {
                    sb.Append((char)(val + 'A'));
                }
            }
            return sb.ToString();
        }

        private int[] MultiplyMatrixVector(int[,] matrix, int[] vector, int n)
        {
            int[] result = new int[n];
            for (int r = 0; r < n; r++)
            {
                int sum = 0;
                for (int c = 0; c < n; c++)
                {
                    sum += matrix[r, c] * vector[c];
                }
                result[r] = Mod(sum, 26);
            }
            return result;
        }

        private int CalculateDeterminant(int[,] m, int n)
        {
            if (n == 2)
            {
                return Mod((m[0, 0] * m[1, 1]) - (m[0, 1] * m[1, 0]), 26);
            }
            else // n == 3
            {
                long det = m[0, 0] * (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1])
                         - m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0])
                         + m[0, 2] * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);
                return Mod((int)det, 26);
            }
        }

        private int[,] GetInverseMatrix(int[,] m, int n, int detInv)
        {
            int[,] inv = new int[n, n];
            if (n == 2)
            {
                inv[0, 0] = Mod(m[1, 1] * detInv, 26);
                inv[0, 1] = Mod(-m[0, 1] * detInv, 26);
                inv[1, 0] = Mod(-m[1, 0] * detInv, 26);
                inv[1, 1] = Mod(m[0, 0] * detInv, 26);
            }
            else // n == 3
            {
                inv[0, 0] = Mod((m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1]) * detInv, 26);
                inv[0, 1] = Mod(-(m[0, 1] * m[2, 2] - m[0, 2] * m[2, 1]) * detInv, 26);
                inv[0, 2] = Mod((m[0, 1] * m[1, 2] - m[0, 2] * m[1, 1]) * detInv, 26);
                
                inv[1, 0] = Mod(-(m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0]) * detInv, 26);
                inv[1, 1] = Mod((m[0, 0] * m[2, 2] - m[0, 2] * m[2, 0]) * detInv, 26);
                inv[1, 2] = Mod(-(m[0, 0] * m[1, 2] - m[0, 2] * m[1, 0]) * detInv, 26);
                
                inv[2, 0] = Mod((m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]) * detInv, 26);
                inv[2, 1] = Mod(-(m[0, 0] * m[2, 1] - m[0, 1] * m[2, 0]) * detInv, 26);
                inv[2, 2] = Mod((m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0]) * detInv, 26);
            }
            return inv;
        }

        private int Mod(int a, int b) => (a % b + b) % b;

        private int ModInverse(int a, int m)
        {
            a = Mod(a, m);
            for (int x = 1; x < m; x++)
            {
                if ((a * x) % m == 1)
                    return x;
            }
            return -1;
        }

        private string MatrixToString(int[,] matrix, int n)
        {
            var sb = new StringBuilder();
            for (int r = 0; r < n; r++)
            {
                var row = new List<string>();
                for (int c = 0; c < n; c++)
                {
                    row.Add(matrix[r, c].ToString().PadLeft(2));
                }
                sb.AppendLine($"[ {string.Join(" ", row)} ]");
            }
            return sb.ToString().Trim();
        }
    }
}
