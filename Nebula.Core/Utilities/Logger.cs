using Spectre.Console;
using System.Diagnostics;

namespace Nebula.Core.Utilities
{
    public static class Logger
    {
        const bool DoLog = true;
        private static string FileName = string.Empty;
        private static string TimeStamp = string.Empty;
        private static List<string> Logs = new();

        public static void Log(this object parent, object message, Color? color = null)
        {
            LogType(parent.GetType(), message, color);
        }

        public static void LogType(Type parentType, object message, Color? color = null)
        {
            if (color == null)
                color = NebulaCore.ColorRules[3];
            Debug.WriteLine(message);
            if (DoLog)
            {
                AnsiConsole.MarkupLine($"[{NebulaCore.ColorRules[1]}][[{DateTime.Now.ToString("HH:mm:ss")}]][/] [{color}]{Markup.Escape(message.ToString()!)}[/]");
                Logs.Add($"[{parentType.Name}\\{DateTime.Now.ToString("HH:mm:ss.ff")}] {message}");
            }
        }

        public static void SilentLog(this object parent, object message, Color? color = null)
        {
            SilentLogType(parent.GetType(), message, color);
        }

        public static void SilentLogType(Type parentType, object message, Color? color = null)
        {
            if (color == null)
                color = NebulaCore.ColorRules[3];
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
