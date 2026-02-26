using System.Collections.Generic;
using CryptoEdu.Core.Classical;
using CryptoEdu.Core.Interfaces;
using CryptoEdu.Core.Modern;

namespace CryptoEdu.Core
{
    /// <summary>
    /// A central registry for all cryptographic algorithms implemented in the application.
    /// Used to easily populate UI dropdowns and resolve algorithms by name.
    /// </summary>
    public static class CipherRegistry
    {
        private static readonly List<IClassicalCipher> _classicalCiphers = new List<IClassicalCipher>
        {
            new CaesarCipher(),
            new MonoalphabeticCipher(),
            new PlayfairCipher(),
            new HillCipher(),
            new VigenereCipher(),
            new OneTimePadCipher(),
            new RailFenceCipher(),
            new RowColumnTranspositionCipher()
        };

        private static readonly List<ICipher> _modernCiphers = new List<ICipher>
        {
            new AesCipher(),
            new DesCipher(),
            new RsaCipher()
        };

        /// <summary>
        /// Retrieves all classical ciphers that support educational step-by-step functionality.
        /// </summary>
        public static IEnumerable<IClassicalCipher> GetClassicalCiphers()
        {
            return _classicalCiphers;
        }

        /// <summary>
        /// Retrieves all modern cryptographic algorithms suitable for professional file encryption.
        /// </summary>
        public static IEnumerable<ICipher> GetModernCiphers()
        {
            return _modernCiphers;
        }
    }
}
