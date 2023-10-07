using SapphireD.Core.FileReaders;
using SapphireD.Core.Utilities;
using System.Diagnostics;

namespace SapphireD
{
    internal class Program
    {
        static FileReader reader;
        static Stopwatch readStopwatch = new Stopwatch();

        static void Main(string[] args)
        {
            var processModule = Process.GetCurrentProcess().MainModule;
            if (processModule != null)
            {
                var pathToExe = processModule.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }

            SapDCore.Init();

        START:
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("SapphireD");
            Console.WriteLine("    by Yunivers\n");
            Console.ResetColor();

        PATH:
            Console.WriteLine("File Path:");
            string path = Console.ReadLine().Trim().Trim('"');
            if (File.Exists(path))
                SapDCore.FilePath = path;
            else
                goto PATH;

        READER:
            string ext = Path.GetExtension(SapDCore.FilePath).ToLower();
            switch (ext)
            {
                case ".exe":
                    reader = new ExeFileReader();
                    break;
                default:
                    Console.WriteLine("Unknown extension: " + ext);
                    Console.WriteLine("\nSelect Reader:");
                    if (!int.TryParse(Console.ReadLine().Trim(), out int readerID))
                        throw new NotImplementedException();
                    else
                        goto READER;
                    break;
            }

            Console.Clear();
            readStopwatch.Restart();
            Console.Clear();
            Console.WriteLine($"Reading game with \"{reader.Name}\"");
            reader.LoadGame(path);
            readStopwatch.Stop();

        SELECT_TOOL:
            //Console.Clear();
            Console.WriteLine($"Reading finished in {readStopwatch.Elapsed.TotalSeconds} seconds");
            FileReader game = reader.Copy();
        }
    }
}