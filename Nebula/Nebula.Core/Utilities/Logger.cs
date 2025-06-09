using System.Diagnostics;

namespace Nebula.Core.Utilities
{
    public static class Logger
    {
        public enum LogType
        {
            Info,
            Warning,
            Error,
            Debug
        }

        const bool DoLog = true;
        private static string FileName = string.Empty;
        private static string TimeStamp = string.Empty;
        private static List<string> Logs = new();

        public static void Log(this object parent, object message, LogType type = LogType.Info)
        {
            Log(parent.GetType(), message, type);
        }

        public static void Log(Type parentType, object message, LogType type = LogType.Info)
        {
#if !DEBUG
            if (type == LogType.Debug)
                return;
#endif

            Debug.WriteLine($"[{type}] {message}");
            if (DoLog)
            {
                ConsoleColor originalColor = Console.ForegroundColor;
                Console.ForegroundColor = type switch
                {
                    LogType.Warning => ConsoleColor.Yellow,
                    LogType.Error => ConsoleColor.Red,
                    LogType.Debug => ConsoleColor.Magenta,
                    _ => originalColor
                };
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}\\{type}] {message.ToString()!}");
                Console.ForegroundColor = originalColor;

                Logs.Add($"[{parentType.Name}\\{DateTime.Now.ToString("HH:mm:ss.ff")}\\{type}] {message}");
            }
        }

        public static void SilentLog(this object parent, object message)
        {
            SilentLogType(parent.GetType(), message);
        }

        public static void SilentLogType(Type parentType, object message)
        {
            if (DoLog)
                Logs.Add($"[{parentType.Name}\\{DateTime.Now.ToString("HH:mm:ss.ff")}] {message}");
        }

        public static void Save()
        {
            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");

            if (string.IsNullOrEmpty(TimeStamp))
                TimeStamp = DateTime.Now.ToString("s").Replace(':', '-');

            if (string.IsNullOrEmpty(FileName))
            {
                int id = 0;
                foreach (string dir in Directory.GetFiles("Logs"))
                    if (Path.GetFileName(dir).StartsWith("log_" + TimeStamp))
                        id++;
                FileName = $"log_{TimeStamp}_{id}";
            }

            File.WriteAllLines($"Logs\\{FileName}.log", Logs);
            File.WriteAllLines("Logs\\latest.log", Logs);
        }
    }
}
