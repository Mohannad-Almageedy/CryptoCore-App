using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryptoEdu.Core.Interfaces;
using CryptoEdu.Core.Models;

namespace CryptoEdu.Core.Classical
{
    /// <summary>
    /// Implementation of the One-Time Pad Cipher.
    /// The fundamental rule is that the key must be at least as long as the plaintext.
    /// </summary>
    public class OneTimePadCipher : IClassicalCipher
    {
        public string Name => "One-Time Pad Cipher";

        public string GetMathematicalRule()
        {
            return "Requirement: The key must be completely random, never reused, and at least as long as the plaintext.\n" +
                   "Encryption: C_i = (P_i + K_i) mod 26\n" +
                   "Decryption: P_i = (C_i - K_i + 26) mod 26\n" +
                   "Where K_i is the shift value (A=0, B=1, ..., Z=25) of the key character.";
        }

        public string Encrypt(string plainText, string key)
        {
            string cleanText = PrepareText(plainText);
            string cleanKey = PrepareKey(key, cleanText.Length);
            
            return ShiftText(cleanText, cleanKey, encrypt: true);
        }

        public string Decrypt(string cipherText, string key)
        {
            string cleanText = PrepareText(cipherText);
            string cleanKey = PrepareKey(key, cleanText.Length);
            
            return ShiftText(cleanText, cleanKey, encrypt: false);
        }

        public List<CipherStep> GetEncryptionSteps(string plainText, string key)
        {
            var steps = new List<CipherStep>();
            
            string cleanText = PrepareText(plainText);
            string cleanKey;

            try
            {
                cleanKey = PrepareKey(key, cleanText.Length);
            }
            catch (Exception ex)
            {
                steps.Add(new CipherStep
                {
                    Title = "Validation Error",
                    Description = ex.Message,
                    InputState = "Invalid Key",
                    OutputState = "Failed"
                });
                return steps;
            }
            
            steps.Add(new CipherStep
            {
                Title = "Step 1: Preparation & Validation",
                Description = "Clean the plaintext and ensure the key is at least as long as the plaintext.",
                InputState = $"Original Text: {plainText}\nOriginal Key: {key}",
                OutputState = $"Clean Text: {cleanText}\nClean Key: {cleanKey}"
            });

            var sb = new StringBuilder();

            for (int i = 0; i < cleanText.Length; i++)
            {
                char p = cleanText[i];
                int originalVal = p - 'A';
                int keyShift = cleanKey[i] - 'A';
                
                int newVal = (originalVal + keyShift) % 26;
                char c = (char)(newVal + 'A');
                
                sb.Append(c);

                steps.Add(new CipherStep
                {
                    Title = $"Step {i + 2}: Encrypt Character '{p}'",
                    Description = $"Use perfectly aligned key character '{cleanKey[i]}' (Shift: {keyShift}).",
                    FormulaApplied = $"C = ({originalVal} + {keyShift}) mod 26 = {newVal}",
                    InputState = cleanText.Substring(0, i + 1),
                    OutputState = sb.ToString()
                });
            }

            return steps;
        }

        public List<CipherStep> GetDecryptionSteps(string cipherText, string key)
        {
            var steps = new List<CipherStep>();
            
            string cleanText = PrepareText(cipherText);
            string cleanKey;

            try
            {
                cleanKey = PrepareKey(key, cleanText.Length);
            }
            catch (Exception ex)
            {
                steps.Add(new CipherStep
                {
                    Title = "Validation Error",
                    Description = ex.Message,
                    InputState = "Invalid Key",
                    OutputState = "Failed"
                });
                return steps;
            }
            
            steps.Add(new CipherStep
            {
                Title = "Step 1: Preparation & Validation",
                Description = "Clean the ciphertext and ensure the key is at least as long as the ciphertext.",
                InputState = $"Original CipherText: {cipherText}\nOriginal Key: {key}",
                OutputState = $"Clean CipherText: {cleanText}\nClean Key: {cleanKey}"
            });

            var sb = new StringBuilder();

            for (int i = 0; i < cleanText.Length; i++)
            {
                char c = cleanText[i];
                int originalVal = c - 'A';
                int keyShift = cleanKey[i] - 'A';
                
                int newVal = (originalVal - keyShift + 26) % 26;
                char p = (char)(newVal + 'A');
                
                sb.Append(p);

                steps.Add(new CipherStep
                {
                    Title = $"Step {i + 2}: Decrypt Character '{c}'",
                    Description = $"Use perfectly aligned key character '{cleanKey[i]}' (Shift: {keyShift}).",
                    FormulaApplied = $"P = ({originalVal} - {keyShift} + 26) mod 26 = {newVal}",
                    InputState = cleanText.Substring(0, i + 1),
                    OutputState = sb.ToString()
                });
            }

            return steps;
        }

        private string PrepareText(string text)
        {
            return new string(text.ToUpper().Where(char.IsLetter).ToArray());
        }

        private string PrepareKey(string key, int requiredLength)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be empty.");

            string cleanKey = new string(key.ToUpper().Where(char.IsLetter).ToArray());
            
            if (cleanKey.Length < requiredLength)
            {
                throw new ArgumentException($"One-Time Pad requires a key at least as long as the letters in the plaintext. Text length: {requiredLength}, Key length: {cleanKey.Length}");
            }

            return cleanKey;
        }

        private string ShiftText(string cleanText, string cleanKey, bool encrypt)
        {
            char[] buffer = cleanText.ToCharArray();

            for (int i = 0; i < buffer.Length; i++)
            {
                char current = buffer[i];
                int currentVal = current - 'A';
                int keyShift = cleanKey[i] - 'A';

                if (!encrypt)
                {
                    keyShift = -keyShift;
                }

                buffer[i] = (char)((currentVal + keyShift + 26) % 26 + 'A');
            }

            return new string(buffer);
        }
    }
}
