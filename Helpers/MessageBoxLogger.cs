﻿using System.Windows;

namespace Helpers
{
    /// <summary>
    /// Class to log WPF messages using a MessageBox.
    /// </summary>
    public class MessageBoxLogger : ILogging
    {
        /// <summary>
        /// Log an error.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="overview">Shortened version of the message.</param>
        public void LogError(string message, string overview)
        {
            MessageBox.Show(message, overview, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Log an informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="overview">Shortened version of the message.</param>
        public void LogMessage(string message, string overview, int offset = 0)
        {
            MessageBox.Show(message, overview, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Reset the logger if relevant.
        /// </summary>
        public void Reset()
        {
        }
    }
}
