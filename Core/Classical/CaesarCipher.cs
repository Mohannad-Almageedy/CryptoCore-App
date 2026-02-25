using System;
using System.Collections.Generic;
using System.Text;
using CryptoEdu.Core.Interfaces;
using CryptoEdu.Core.Models;

namespace CryptoEdu.Core.Classical
{
    /// <summary>
    /// Implementation of the Caesar Cipher (Shift Cipher).
    /// </summary>
    public class CaesarCipher : IClassicalCipher
    {
        public string Name => "Caesar Cipher";

        public string GetMathematicalRule()
        {
            return "Encryption: C = (P + K) mod 26\nDecryption: P = (C - K) mod 26\nWhere K is the shift amount.";
        }

        public string Encrypt(string plainText, string key)
        {
            if (!int.TryParse(key, out int shift))
                throw new ArgumentException("Key for Caesar cipher must be an integer.");

            return ShiftText(plainText, shift);
        }

        public string Decrypt(string cipherText, string key)
        {
            if (!int.TryParse(key, out int shift))
                throw new ArgumentException("Key for Caesar cipher must be an integer.");

            return ShiftText(cipherText, -shift);
        }

        public List<CipherStep> GetEncryptionSteps(string plainText, string key)
        {
            var steps = new List<CipherStep>();
            if (!int.TryParse(key, out int shift))
                throw new ArgumentException("Key must be an integer.");
            
            // Normalize shift
            shift = (shift % 26 + 26) % 26;

            var sb = new StringBuilder();

            for (int i = 0; i < plainText.Length; i++)
            {
                char p = plainText[i];
                if (!char.IsLetter(p))
                {
                    sb.Append(p);
                    continue;
                }

                char offset = char.IsUpper(p) ? 'A' : 'a';
                int originalIndex = p - offset;
                int newIndex = (originalIndex + shift) % 26;
                char c = (char)(newIndex + offset);

                sb.Append(c);

                steps.Add(new CipherStep
                {
                    Title = $"Step {i + 1}: Shift Character '{p}'",
                    Description = $"Shift '{p}' by {shift} positions in the alphabet to get '{c}'.",
                    FormulaApplied = $"C = ({originalIndex} + {shift}) mod 26 = {newIndex}",
                    InputState = plainText.Substring(0, i + 1),
                    OutputState = sb.ToString()
                });
            }

            return steps;
        }

        public List<CipherStep> GetDecryptionSteps(string cipherText, string key)
        {
            var steps = new List<CipherStep>();
            if (!int.TryParse(key, out int shift))
                throw new ArgumentException("Key must be an integer.");
            
            // Normalize shift
            shift = (shift % 26 + 26) % 26;

            var sb = new StringBuilder();

            for (int i = 0; i < cipherText.Length; i++)
            {
                char c = cipherText[i];
                if (!char.IsLetter(c))
                {
                    sb.Append(c);
                    continue;
                }

                char offset = char.IsUpper(c) ? 'A' : 'a';
                int originalIndex = c - offset;
                int newIndex = (originalIndex - shift + 26) % 26;
                char p = (char)(newIndex + offset);

                sb.Append(p);

                steps.Add(new CipherStep
                {
                    Title = $"Step {i + 1}: Shift Character '{c}' back",
                    Description = $"Shift '{c}' backward by {shift} positions in the alphabet to get '{p}'.",
                    FormulaApplied = $"P = ({originalIndex} - {shift}) mod 26 = {newIndex}",
                    InputState = cipherText.Substring(0, i + 1),
                    OutputState = sb.ToString()
                });
            }

            return steps;
        }

        private string ShiftText(string text, int shift)
        {
            shift = (shift % 26 + 26) % 26; // Handle negative shifts correctly
            char[] buffer = text.ToCharArray();
            
            for (int i = 0; i < buffer.Length; i++)
            {
                char c = buffer[i];
                if (char.IsLetter(c))
                {
                    char offset = char.IsUpper(c) ? 'A' : 'a';
                    buffer[i] = (char)((c - offset + shift) % 26 + offset);
                }
            }

            return new string(buffer);
        }
    }
}
