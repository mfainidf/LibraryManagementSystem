using System;
using System.IO;
using System.Threading.Tasks;

namespace Library.Console.Logging
{
    public static class Logger
    {
        private static readonly string LogFile = "library.log";

        public static async Task LogAsync(string message, LogType type = LogType.Info)
        {
            var logMessage = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} [{type}] {message}";
            await File.AppendAllTextAsync(LogFile, logMessage + Environment.NewLine);
        }
    }

    public enum LogType
    {
        Info,
        Error,
        Warning
    }
}
