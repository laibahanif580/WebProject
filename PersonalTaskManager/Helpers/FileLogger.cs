using System;
using System.IO;

namespace PersonalTaskManager.Helpers
{
    public static class FileLogger   //cant be instantiated and only have static members
    {
        private static readonly string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "error.log");

        public static void LogError(Exception ex, string message = "")
        {
            try
            {
                // Ensure Logs folder exists
                var logDir = Path.GetDirectoryName(logFilePath);
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                // Log format
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {message}\n{ex}\n\n";

                // Append to file
                File.AppendAllText(logFilePath, logMessage);
            }
            catch
            {
                // Avoid throwing errors from logger itself
            }
        }
    }
}
