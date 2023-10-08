using SapphireD.Core.FileReaders;
using SapphireD.Core.Utilities;
using Spectre.Console;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SapphireD
{
    internal class Program
    {
        static Stopwatch readStopwatch = new Stopwatch();

        static async Task Main(string[] args)
        {
            var processModule = Process.GetCurrentProcess().MainModule;
            if (processModule != null)
            {
                var pathToExe = processModule.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }

            SapDCore.Init();
            Logger.doLog = false;
            await SpectreMain();
            return;
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
                    SapDCore.CurrentReader = new ExeFileReader();
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
            Console.WriteLine($"Reading game with \"{SapDCore.CurrentReader.Name}\"");
            SapDCore.CurrentReader.LoadGame(path);
            readStopwatch.Stop();

        SELECT_TOOL:
            //Console.Clear();
            Console.WriteLine($"Reading finished in {readStopwatch.Elapsed.TotalSeconds} seconds");
            FileReader game = SapDCore.CurrentReader.Copy();
        }

        static async Task SpectreMain()
        {
            WaitForFile();
            SelectReader();
            ReadPackage();
            AnsiConsole.WriteLine($"Reading finished in {readStopwatch.Elapsed.TotalSeconds} seconds");
        }

        static void WaitForFile()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("SapphireD").Centered().Color(Color.DeepSkyBlue1));
            AnsiConsole.Write(new Text("File Path:\n"));
            
            string path = Console.ReadLine().Trim().Trim('"');
            if (File.Exists(path))
                SapDCore.FilePath = path;
            else WaitForFile();
        }

        static void SelectReader()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("SapphireD").Centered().Color(Color.DeepSkyBlue1));
            var fileReaders = from t in Assembly.GetAssembly(typeof(SapDCore)).GetTypes()
                            where t.GetInterfaces().Contains(typeof(FileReader))
                            && t.GetConstructor(Type.EmptyTypes) != null
                            select Activator.CreateInstance(t) as FileReader;

            List<string> fileReaderNames = new()
            {
                "Auto-Detect"
            };

            foreach (FileReader fileReader in fileReaders)
                fileReaderNames.Add(fileReader.Name);

            string? selectedReader = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                            .Title("Select a file-reader.")
                                            .AddChoices(fileReaderNames));

            SapDCore.CurrentReader = null;
            foreach (FileReader fileReader in fileReaders)
                if (fileReader.Name == selectedReader)
                {
                    SapDCore.CurrentReader = fileReader;
                    break;
                }

            if (SapDCore.CurrentReader == null)
                AutoDetectReader();
        }

        static void AutoDetectReader()
        {
            string ext = Path.GetExtension(SapDCore.FilePath).ToLower();
            switch (ext)
            {
                default:
                    break;
            }
            SapDCore.CurrentReader = new ExeFileReader();
        }

        static void ReadPackage()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("SapphireD").Centered().Color(Color.DeepSkyBlue1));
            readStopwatch.Restart();
            AnsiConsole.WriteLine($"Reading game with \"{SapDCore.CurrentReader.Name}\"");
            Task readTask = new Task(() => SapDCore.CurrentReader.LoadGame(SapDCore.FilePath));
            readTask.Start();
            AnsiConsole.Progress()
                .Start(ctx =>
                {
                    // Define tasks
                    var task1 = ctx.AddTask("[DeepSkyBlue1]Reading chunks[/]", false);

                    while (!task1.IsFinished)
                    {
                        if (SapDCore.PackageData != null && SapDCore.PackageData.Chunks.Count > 1)
                        {
                            if (!task1.IsStarted)
                                task1.StartTask();

                            int count = 0;
                            foreach (Task task in SapDCore.PackageData.ChunkReaders.ToArray())
                                if ((int)task.Status > 4)
                                    count++;

                            task1.Value = count;
                            task1.MaxValue = SapDCore.PackageData.ChunkReaders.Count;
                        }
                    }
                });
            readStopwatch.Stop();
        }
    }
}