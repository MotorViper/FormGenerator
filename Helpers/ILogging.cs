﻿namespace Helpers
{
    /// <summary>
    /// Interface for logging messages.
    /// </summary>
    public interface ILogging
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
        /// <param name="offset">Suggested offset for message.</param>
        void LogMessage(string message, string overview, int offset = 0);

        /// <summary>
        /// Reset the logger if relevant.
        /// </summary>
        void Reset();
    }
}
