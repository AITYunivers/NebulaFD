using SapphireD.Core.FileReaders;
using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;
using Spectre.Console;
using System.Diagnostics;
using System.Reflection;

namespace SapphireD
{
    internal class Program
    {
        static Stopwatch readStopwatch = new Stopwatch();
        static ByteReader? fileReader;

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
            SpectreMain();
        }

        static void SpectreMain()
        {
            WaitForFile();
            SelectReader();
            ReadPackage();
            SelectPlugin();
        }

        static void WaitForFile()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(SapDCore.ConsoleFiglet);
            AnsiConsole.Write(SapDCore.ConsoleRule);

            AnsiConsole.MarkupLine("[DeepSkyBlue2]File Path:[/]");
            
            string path = Console.ReadLine().Trim().Trim('"');
            if (File.Exists(path))
            {
                SapDCore.FilePath = path;
                fileReader = new ByteReader(File.ReadAllBytes(path));
            }
            else WaitForFile();
        }

        static void SelectReader()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(SapDCore.ConsoleFiglet);
            AnsiConsole.Write(SapDCore.ConsoleRule);

            var fileReaders = from t in Assembly.GetAssembly(typeof(SapDCore)).GetTypes()
                            where t.GetInterfaces().Contains(typeof(IFileReader))
                            && t.GetConstructor(Type.EmptyTypes) != null
                            select Activator.CreateInstance(t) as IFileReader;

            List<string> fileReaderNames = new()
            {
                "Auto-Detect"
            };

            foreach (IFileReader fileReader in fileReaders)
                fileReaderNames.Add(fileReader.Name);

            string? selectedReader = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                                .Title("[DeepSkyBlue2]Select a file-reader.[/]")
                                                .AddChoices(fileReaderNames));

            SapDCore.CurrentReader = null;
            foreach (IFileReader fileReader in fileReaders)
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
                    SapDCore.CurrentReader = new CCNFileReader();
                    ((CCNFileReader)SapDCore.CurrentReader).CheckUnpacked(fileReader!);
                    break;
                case ".exe":
                    SapDCore.CurrentReader = new EXEFileReader();
                    break;
                case ".mfa":
                    SapDCore.CurrentReader = new MFAFileReader();
                    break;
                case ".anm":
                    SapDCore.CurrentReader = new ANMFileReader();
                    break;
                case ".tmp":
                    SapDCore.CurrentReader = new AGMIFileReader();
                    break;
                case ".zip":
                    SapDCore.CurrentReader = new OpenFileReader();
                    break;
                case ".apk":
                    SapDCore.CurrentReader = new APKFileReader();
                    break;
                case ".ipa":
                    SapDCore.CurrentReader = new IPAFileReader();
                    break;
            }
        }

        static void ReadPackage()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(SapDCore.ConsoleFiglet);
            AnsiConsole.Write(SapDCore.ConsoleRule);

            readStopwatch.Restart();
            AnsiConsole.MarkupLine($"[DeepSkyBlue2]Reading game as \"{SapDCore.CurrentReader.Name}\"[/]");
            Task readTask = new Task(() => SapDCore.CurrentReader.LoadGame(fileReader!, SapDCore.FilePath));
            readTask.Start();
            while (true)
            {
                if (SapDCore.PackageData != null)
                {
                    SapDCore.PackageData.CliUpdate();
                    break;
                }
            }
            readStopwatch.Stop();
        }

        static void SelectPlugin()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(SapDCore.ConsoleFiglet);
            AnsiConsole.Write(SapDCore.ConsoleRule);

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