using FFMediaToolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Services.IO
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }
    public class LoggerService
    {
        private static string _logFilePath = PathService.CreateFile(PathService.LogFolder, "Log");
        private static void Log(string message, LogLevel logLevel)
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | {logLevel.ToString().ToUpper()} | {message}";
            File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
        }

        public static void Debug(string message)
        {
            Log(message, LogLevel.Debug);
        }

        public static void Info(string message)
        {
            Log(message, LogLevel.Info);
        }

        public static void Warn(string message)
        {
            Log(message, LogLevel.Warn);
        }

        public static void Error(string message)
        {
            Log(message, LogLevel.Error);
        }

        public static void Fatal(string message)
        {
            Log(message, LogLevel.Fatal);
        }
    }
}
