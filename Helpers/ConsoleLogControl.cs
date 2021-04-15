namespace Helpers
{
    public partial class ConsoleLogControl : ILogControl
    {

        private class NullLogger : ILogging
        {
            public void LogError(string message, string overview)
            {
            }

            public void LogMessage(string message, string overview, int offset = 0)
            {
            }

            public void Reset()
            {
            }
        }

        private readonly ILogging _logger = new NullLogger();

        public void ResetLogging()
        {
            IOCContainer.Instance.Register<ILogging>(new NullLogger());
        }

        public void SetLogging(bool loggingOn)
        {
            ILogging logging;
            if (loggingOn)
                logging = new ConsoleLogger();
            else
                logging = new NullLogger();
            IOCContainer.Instance.Register<ILogging>(logging);
        }
    }
}
