namespace Clamper.Base.Generating
{
    /// <summary>
    ///     Result of the process of generation
    /// </summary>
    public class ClamperGenerationResult
    {
        /// <summary>
        ///     Was the process successful or not
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        ///     Formatted error message if the process failed
        /// </summary>
        public string Error { get; set; }
    }
}