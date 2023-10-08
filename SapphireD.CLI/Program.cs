using SapphireD.Core.FileReaders;
using SapphireD.Core.Utilities;
using Spectre.Console;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using Color = Spectre.Console.Color;

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
            SelectPlugin();
        }

        static void WaitForFile()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("SapphireD").Centered().Color(Color.DeepSkyBlue1));
            AnsiConsole.MarkupLine("[DeepSkyBlue2]File Path:[/]");
            
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
                                            .Title("[DeepSkyBlue2]Select a file-reader.[/]")
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
            AnsiConsole.MarkupLine($"[DeepSkyBlue2]Reading game as \"{SapDCore.CurrentReader.Name}\"[/]");
            Task readTask = new Task(() => SapDCore.CurrentReader.LoadGame(SapDCore.FilePath));
            readTask.Start();
            AnsiConsole.Progress()
                .Start(ctx =>
                {
                    // Define tasks
                    ProgressTask? task1 = ctx.AddTask("[DeepSkyBlue3]Reading chunks[/]", false);
                    ProgressTask? task2 = null;

                    while (!task1.IsFinished)
                    {
                        if (SapDCore.PackageData != null && SapDCore.PackageData.Chunks.Count > 1)
                        {
                            if (!task1.IsStarted)
                                task1.StartTask();

                            int count = 0;
                            foreach (Task task in SapDCore.PackageData.ChunkReaders.ToArray())
                                if (task != null && (int)task.Status > 4)
                                    count++;

                            task1.Value = count;
                            task1.MaxValue = SapDCore.PackageData.ChunkReaders.Count;

                            if (SapDCore.PackageData.ImageBank != null && SapDCore.PackageData.ImageBank.Images != null)
                            {
                                if (task2 == null)
                                    task2 = ctx.AddTask("[DeepSkyBlue3]Reading images[/]", true);

                                task2.Value = SapDCore.PackageData.ImageBank.Images.Count;
                                task2.MaxValue = SapDCore.PackageData.ImageBank.ImageCount;
                            }
                        }
                    }
                });
            readStopwatch.Stop();
        }

        static void SelectPlugin()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("SapphireD").Centered().Color(Color.DeepSkyBlue1));

            AnsiConsole.MarkupLine($"[DeepSkyBlue2]Reading finished in {readStopwatch.Elapsed.TotalSeconds} seconds[/]");
            List<SapDPlugin> plugins = new();
            List<string> pluginNames = new()
            {
                "Quit"
            };

            Directory.CreateDirectory("Plugins");
            foreach (var item in Directory.GetFiles("Plugins", "*.dll"))
            {
                var newAsm = Assembly.LoadFrom(Path.GetFullPath(item));
                foreach (var pluginType in newAsm.GetTypes())
                    if (pluginType.GetInterface(typeof(SapDPlugin).FullName) != null)
                    {
                        SapDPlugin plugin = (SapDPlugin)Activator.CreateInstance(pluginType);
                        plugins.Add(plugin);
                        pluginNames.Add(plugin.Name);
                    }
            }

            List<string> selectedTasks = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("[DeepSkyBlue2]Select a task.[/]")
                    .InstructionsText(
                        "[DeepSkyBlue3](Press [DeepSkyBlue1]<space>[/] to select a task, " +
                        "[DeepSkyBlue1]<enter>[/] to execute)\n(Quit will always execute last.)[/]")
                    .AddChoices(pluginNames));


            foreach (SapDPlugin plugin in plugins)
                if (selectedTasks.Contains(plugin.Name))
                    plugin.Execute();

            if (selectedTasks.Contains("Quit"))
                Environment.Exit(0);
            else SelectPlugin();
        }
    }
}