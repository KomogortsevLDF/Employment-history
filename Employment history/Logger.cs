using System;
using System.IO;

namespace Employment_history
{
    public class Logger
    {
        private readonly string logFilePath;

        public Logger(string logFilePath)
        {
            this.logFilePath = logFilePath;
        }

        public void LogEvent(string username, string eventType, string description)
        {
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}, {username}, {eventType}, {description}");
            }
        }
    }
}
