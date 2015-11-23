namespace Helpers
{
    /// <summary>
    /// Interface for logging WPF messages.
    /// </summary>
    public interface IWPFLogging
    {
        /// <summary>
        /// Log an error.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="overview">Shortened version of the message.</param>
        void LogError(string message, string overview);

        /// <summary>
        /// Log an informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="overview">Shortened version of the message.</param>
        void LogMessage(string message, string overview);
    }
}
