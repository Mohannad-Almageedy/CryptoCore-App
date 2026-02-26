using System;
using System.Collections.Generic;

namespace CryptoEdu.Services
{
    public class HistoryEntry
    {
        public DateTime Timestamp { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string OperationType { get; set; } = string.Empty; // Encrypt, Decrypt, Hash
    }

    /// <summary>
    /// Service managing the application session's cryptographic operation history.
    /// Allows the user to view recent encryptions, decryptions, and hashing operations.
    /// </summary>
    public static class HistoryService
    {
        private static readonly List<HistoryEntry> _history = new List<HistoryEntry>();

        public static IReadOnlyList<HistoryEntry> GetHistory()
        {
            return _history.AsReadOnly();
        }

        public static void LogOperation(string operationType, string title, string description)
        {
            var entry = new HistoryEntry
            {
                Timestamp = DateTime.Now,
                OperationType = operationType,
                Title = title,
                Description = description
            };

            _history.Insert(0, entry); // Add to top

            // Maintain max 100 entries to prevent memory bloat
            if (_history.Count > 100)
            {
                _history.RemoveAt(_history.Count - 1);
            }
        }

        public static void ClearHistory()
        {
            _history.Clear();
        }
    }
}
