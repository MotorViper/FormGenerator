namespace Helpers
{
    public class StorageLogger: ILogging
    {
        public string Text { get; set; }

        /// <summary>
        /// Log an error.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="overview">Shortened version of the message.</param>
        public void LogError(string message, string overview)
        {
            Text += $"{overview}[E]: {message}\n";
        }

        /// <summary>
        /// Log an informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="overview">Shortened version of the message.</param>
        public void LogMessage(string message, string overview)
        {
            Text += $"{overview}[I]: {message}\n";
        }

        /// <summary>
        /// Reset the logger if relevant.
        /// </summary>
        public void Reset()
        {
            Text = "";
        }
    }
}