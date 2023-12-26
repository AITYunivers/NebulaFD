using System.Diagnostics;

namespace SapphireD.Core.Utilities
{
    public static class Logger
    {
        public static bool doLog = true;
        private static List<string> Logs = new();

        public static void Log(object parent, object message, int type = 0, ConsoleColor color = ConsoleColor.Black)
        {
            return;
            if (doLog)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                string info = $"[{DateTime.Now.ToString("HH:mm:ss")}] ";
                Console.Write(info);
                Console.ForegroundColor = color;
                if (color == ConsoleColor.Black)
                    Console.ResetColor();
                Console.WriteLine(message);
            }

            string outInfo = $"[{(type != 0 ? type + "\\" : "")}{parent.GetType().Name}\\{DateTime.Now.ToString("HH:mm:ss.ff")}] ";
            Logs.Add(outInfo + message);
            Debug.WriteLine(outInfo + message);

            Console.ResetColor();
            try
            {
                File.WriteAllLines("Latest.log", Logs);
            }
            catch {}
        }

        public static void Log(object message, int type = 0, ConsoleColor color = ConsoleColor.Black)
        {
            if (doLog)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                string info = $"[{DateTime.Now.ToString("HH:mm:ss")}] ";
                Console.Write(info);
                Console.ForegroundColor = color;
                if (color == ConsoleColor.Black)
                    Console.ResetColor();
                Console.WriteLine(message);
            }

            string outInfo = $"[{(type != 0 ? type + "\\" : "")}{DateTime.Now.ToString("HH:mm:ss.ff")}] ";
            File.AppendAllText("Latest.log", outInfo + message + "\n");

            Console.ResetColor();
        }
    }
}
