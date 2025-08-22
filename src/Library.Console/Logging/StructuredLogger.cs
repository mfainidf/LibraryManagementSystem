using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Library.Console.Logging
{
    public class StructuredLogger
    {
        private readonly string _logFile;

        public StructuredLogger(string logFile = "library.log")
        {
            _logFile = logFile;
        }

        public async Task LogAsync(string message, LogType type = LogType.Info, object context = null)
        {
            var logEntry = new
            {
                Timestamp = DateTime.UtcNow,
                Type = type.ToString(),
                Message = message,
                Context = context
            };

            var logLine = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });

            await File.AppendAllTextAsync(_logFile, logLine + Environment.NewLine);
        }

        public async Task LogUserActivityAsync(string activity, string userId, string details = null)
        {
            await LogAsync(activity, LogType.Info, new
            {
                UserId = userId,
                Details = details,
                Category = "UserActivity"
            });
        }

        public async Task LogErrorAsync(Exception ex, string context = null)
        {
            await LogAsync("An error occurred", LogType.Error, new
            {
                ExceptionType = ex.GetType().Name,
                ExceptionMessage = ex.Message,
                StackTrace = ex.StackTrace,
                Context = context
            });
        }

        public async Task LogSecurityEventAsync(string eventType, string userId, string details = null)
        {
            await LogAsync(eventType, LogType.Info, new
            {
                UserId = userId,
                Details = details,
                Category = "Security"
            });
        }
    }
}
