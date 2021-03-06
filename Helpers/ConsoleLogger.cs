﻿using System;

namespace Helpers
{
    public class ConsoleLogger : ILogging
    {
        public void LogError(string message, string overview)
        {
            Console.WriteLine("E: " + message);
        }

        public void LogMessage(string message, string overview, int offset = 0)
        {
            Console.WriteLine("M: " + message);
        }

        public void Reset()
        {
        }
    }
}
