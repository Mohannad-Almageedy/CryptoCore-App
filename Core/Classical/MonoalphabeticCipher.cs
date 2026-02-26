using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryptoEdu.Core.Interfaces;
using CryptoEdu.Core.Models;

namespace CryptoEdu.Core.Classical
{
    /// <summary>
    /// Implementation of the Monoalphabetic Cipher.
    /// Uses a 26-character shuffled alphabet as a key.
    /// </summary>
    public class MonoalphabeticCipher : IClassicalCipher
    {
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public string Name => "Monoalphabetic Cipher";

        public string GetMathematicalRule()
        {
            return "Substitution Rule: Each letter in the plaintext is replaced by the corresponding letter in the 26-letter key alphabet.\n" +
                   "Example: A -> Key[0], B -> Key[1], ..., Z -> Key[25]";
        }

        public string Encrypt(string plainText, string key)
        {
            ValidateKey(key);
            key = key.ToUpper();
            
            var sb = new StringBuilder();
            foreach (char c in plainText)
            {
                if (char.IsLetter(c))
                {
                    bool isLower = char.IsLower(c);
                    int index = char.ToUpper(c) - 'A';
                    char mappedChar = key[index];
                    sb.Append(isLower ? char.ToLower(mappedChar) : mappedChar);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public string Decrypt(string cipherText, string key)
        {
            ValidateKey(key);
            key = key.ToUpper();
            
            var sb = new StringBuilder();
            foreach (char c in cipherText)
            {
                if (char.IsLetter(c))
                {
                    bool isLower = char.IsLower(c);
                    int index = key.IndexOf(char.ToUpper(c));
                    if (index == -1) 
                        throw new Exception($"Character '{char.ToUpper(c)}' not found in key.");
                        
                    char mappedChar = Alphabet[index];
                    sb.Append(isLower ? char.ToLower(mappedChar) : mappedChar);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public List<CipherStep> GetEncryptionSteps(string plainText, string key)
        {
            ValidateKey(key);
            key = key.ToUpper();
            
            var steps = new List<CipherStep>();
            
            steps.Add(new CipherStep
            {
                Title = "Step 1: Alphabet Mapping",
                Description = "Map the standard alphabet to the provided key.",
                VisualizationData = BuildMappingTable(Alphabet, key),
                InputState = "Standard Alphabet",
                OutputState = "Substitution Key Mapping generated"
            });

            var sb = new StringBuilder();
            for (int i = 0; i < plainText.Length; i++)
            {
                char c = plainText[i];
                if (!char.IsLetter(c))
                {
                    sb.Append(c);
                    continue;
                }

                bool isLower = char.IsLower(c);
                char upperC = char.ToUpper(c);
                int index = upperC - 'A';
                char mappedChar = key[index];
                char finalChar = isLower ? char.ToLower(mappedChar) : mappedChar;
                
                sb.Append(finalChar);

                steps.Add(new CipherStep
                {
                    Title = $"Step {i + 2}: Substitute '{c}'",
                    Description = $"Find the index of '{upperC}' in the standard alphabet ({index}). Replace it with the character at index {index} in the key: '{key[index]}'.",
                    FormulaApplied = $"P='{upperC}' -> Index={index} -> C='{key[index]}'",
                    InputState = plainText.Substring(0, i + 1),
                    OutputState = sb.ToString()
                });
            }

            return steps;
        }

        public List<CipherStep> GetDecryptionSteps(string cipherText, string key)
        {
            ValidateKey(key);
            key = key.ToUpper();
            
            var steps = new List<CipherStep>();
            
            steps.Add(new CipherStep
            {
                Title = "Step 1: Reverse Alphabet Mapping",
                Description = "Map the provided key back to the standard alphabet.",
                VisualizationData = BuildMappingTable(key, Alphabet),
                InputState = "Substitution Key Mapping",
                OutputState = "Reverse Mapping ready"
            });

            var sb = new StringBuilder();
            for (int i = 0; i < cipherText.Length; i++)
            {
                char c = cipherText[i];
                if (!char.IsLetter(c))
                {
                    sb.Append(c);
                    continue;
                }

                bool isLower = char.IsLower(c);
                char upperC = char.ToUpper(c);
                int index = key.IndexOf(upperC);
                char mappedChar = Alphabet[index];
                char finalChar = isLower ? char.ToLower(mappedChar) : mappedChar;
                
                sb.Append(finalChar);

                steps.Add(new CipherStep
                {
                    Title = $"Step {i + 2}: Reverse Substitute '{c}'",
                    Description = $"Find the index of '{upperC}' in the key ({index}). Replace it with the character at index {index} in the standard alphabet: '{Alphabet[index]}'.",
                    FormulaApplied = $"C='{upperC}' -> Index={index} -> P='{Alphabet[index]}'",
                    InputState = cipherText.Substring(0, i + 1),
                    OutputState = sb.ToString()
                });
            }

            return steps;
        }

        private void ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be empty.");
            
            if (key.Length != 26)
                throw new ArgumentException("Monoalphabetic cipher key must contain exactly 26 characters.");

            if (new string(key.ToUpper().Distinct().ToArray()).Length != 26)
                throw new ArgumentException("Monoalphabetic cipher key must contain 26 unique characters with no duplicates.");
        }

        private string BuildMappingTable(string top, string bottom)
        {
            return $"Plain:  {string.Join(" ", top.ToCharArray())}\nCipher: {string.Join(" ", bottom.ToCharArray())}";
        }
    }
}
