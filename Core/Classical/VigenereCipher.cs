using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryptoEdu.Core.Interfaces;
using CryptoEdu.Core.Models;

namespace CryptoEdu.Core.Classical
{
    /// <summary>
    /// Implementation of the Vigenère Cipher.
    /// Uses a repeating key to shift plaintext characters.
    /// </summary>
    public class VigenereCipher : IClassicalCipher
    {
        public string Name => "Vigenère Cipher";

        public string GetMathematicalRule()
        {
            return "1. Repeat the key to match the length of the plaintext.\n" +
                   "2. Encryption: C_i = (P_i + K_i) mod 26\n" +
                   "3. Decryption: P_i = (C_i - K_i + 26) mod 26\n" +
                   "Where K_i is the shift value (A=0, B=1, ..., Z=25) of the key character.";
        }

        public string Encrypt(string plainText, string key)
        {
            ValidateKey(key);
            string cleanKey = new string(key.ToUpper().Where(char.IsLetter).ToArray());
            return ShiftText(plainText, cleanKey, encrypt: true);
        }

        public string Decrypt(string cipherText, string key)
        {
            ValidateKey(key);
            string cleanKey = new string(key.ToUpper().Where(char.IsLetter).ToArray());
            return ShiftText(cipherText, cleanKey, encrypt: false);
        }

        public List<CipherStep> GetEncryptionSteps(string plainText, string key)
        {
            ValidateKey(key);
            string cleanKey = new string(key.ToUpper().Where(char.IsLetter).ToArray());
            
            var steps = new List<CipherStep>();
            
            steps.Add(new CipherStep
            {
                Title = "Step 1: Key Preparation",
                Description = "Clean the key to contain only letters and convert to uppercase.",
                InputState = $"Original Key: {key}",
                OutputState = $"Clean Key: {cleanKey}"
            });

            var sb = new StringBuilder();
            int keyIndex = 0;

            for (int i = 0; i < plainText.Length; i++)
            {
                char p = plainText[i];
                if (!char.IsLetter(p))
                {
                    sb.Append(p);
                    continue;
                }

                char offset = char.IsUpper(p) ? 'A' : 'a';
                int originalVal = p - offset;
                int keyShift = cleanKey[keyIndex % cleanKey.Length] - 'A';
                
                int newVal = (originalVal + keyShift) % 26;
                char c = (char)(newVal + offset);
                
                sb.Append(c);

                steps.Add(new CipherStep
                {
                    Title = $"Step {i + 2}: Encrypt Character '{p}'",
                    Description = $"Use key character '{cleanKey[keyIndex % cleanKey.Length]}' (Shift: {keyShift}).",
                    FormulaApplied = $"C = ({originalVal} + {keyShift}) mod 26 = {newVal}",
                    InputState = plainText.Substring(0, i + 1),
                    OutputState = sb.ToString()
                });

                keyIndex++;
            }

            return steps;
        }

        public List<CipherStep> GetDecryptionSteps(string cipherText, string key)
        {
            ValidateKey(key);
            string cleanKey = new string(key.ToUpper().Where(char.IsLetter).ToArray());
            
            var steps = new List<CipherStep>();
            
            steps.Add(new CipherStep
            {
                Title = "Step 1: Key Preparation",
                Description = "Clean the key to contain only letters and convert to uppercase.",
                InputState = $"Original Key: {key}",
                OutputState = $"Clean Key: {cleanKey}"
            });

            var sb = new StringBuilder();
            int keyIndex = 0;

            for (int i = 0; i < cipherText.Length; i++)
            {
                char c = cipherText[i];
                if (!char.IsLetter(c))
                {
                    sb.Append(c);
                    continue;
                }

                char offset = char.IsUpper(c) ? 'A' : 'a';
                int originalVal = c - offset;
                int keyShift = cleanKey[keyIndex % cleanKey.Length] - 'A';
                
                int newVal = (originalVal - keyShift + 26) % 26;
                char p = (char)(newVal + offset);
                
                sb.Append(p);

                steps.Add(new CipherStep
                {
                    Title = $"Step {i + 2}: Decrypt Character '{c}'",
                    Description = $"Use key character '{cleanKey[keyIndex % cleanKey.Length]}' (Shift: {keyShift}).",
                    FormulaApplied = $"P = ({originalVal} - {keyShift} + 26) mod 26 = {newVal}",
                    InputState = cipherText.Substring(0, i + 1),
                    OutputState = sb.ToString()
                });

                keyIndex++;
            }

            return steps;
        }

        private string ShiftText(string text, string key, bool encrypt)
        {
            char[] buffer = text.ToCharArray();
            int keyIndex = 0;

            for (int i = 0; i < buffer.Length; i++)
            {
                char current = buffer[i];
                if (char.IsLetter(current))
                {
                    char offset = char.IsUpper(current) ? 'A' : 'a';
                    int currentVal = current - offset;
                    int keyShift = key[keyIndex % key.Length] - 'A';

                    if (!encrypt)
                    {
                        keyShift = -keyShift;
                    }

                    buffer[i] = (char)((currentVal + keyShift + 26) % 26 + offset);
                    keyIndex++;
                }
            }

            return new string(buffer);
        }

        private void ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be empty.");
            
            if (!key.Any(char.IsLetter))
                throw new ArgumentException("Key must contain at least one letter.");
        }
    }
}
