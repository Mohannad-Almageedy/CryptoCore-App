using System.Collections.Generic;

namespace CryptoEdu.Core.Models
{
    /// <summary>
    /// Represents a single educational step during the encryption or decryption process.
    /// This is used by the UI to render step-by-step visual explanations.
    /// </summary>
    public class CipherStep
    {
        /// <summary>
        /// The title or name of the step (e.g., "Matrix Generation", "Character Substitution").
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The detailed description of what happened in this step.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The mathematical formula applied during this step, if applicable.
        /// </summary>
        public string FormulaApplied { get; set; } = string.Empty;
        
        /// <summary>
        /// The input state before this step was applied.
        /// </summary>
        public string InputState { get; set; } = string.Empty;
        
        /// <summary>
        /// The output state after this step was applied.
        /// </summary>
        public string OutputState { get; set; } = string.Empty;

        /// <summary>
        /// Holds custom data like a grid (e.g., Playfair 5x5 matrix) to be rendered by the UI.
        /// </summary>
        public object? VisualizationData { get; set; }
    }
}
