using FusionRipper.Core.FileReaders;
using FusionRipper.Core.Memory;
using FusionRipper.Core.Utilities;
using Spectre.Console;
using System.Diagnostics;
using System.Reflection;

namespace FusionRipper
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
                Directory.SetCurrentDirectory(pathToContentRoot!);
            }

            FRipCore.Init();
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
            AnsiConsole.Write(FRipCore.ConsoleFiglet);
            AnsiConsole.Write(FRipCore.ConsoleRule);

            AnsiConsole.MarkupLine($"[{FRipCore.ColorRules[1]}]File Path:[/]");
            
            string path = Console.ReadLine().Trim().Trim('"');
            if (File.Exists(path))
            {
                FRipCore.FilePath = path;
                fileReader = new ByteReader(File.ReadAllBytes(path));
            }
            else WaitForFile();
        }

        static void SelectReader()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(FRipCore.ConsoleFiglet);
            AnsiConsole.Write(FRipCore.ConsoleRule);

            var fileReaders = from t in Assembly.GetAssembly(typeof(FRipCore)).GetTypes()
                            where t.GetInterfaces().Contains(typeof(IFileReader))
                            && t.GetConstructor(Type.EmptyTypes) != null
                            select Activator.CreateInstance(t) as IFileReader;

            List<string> fileReaderNames = new()
            {
                $"[{FRipCore.ColorRules[3]}]Auto-Detect[/]"
            };

            foreach (IFileReader fileReader in fileReaders)
                fileReaderNames.Add($"[{FRipCore.ColorRules[3]}]{fileReader.Name}[/]");

            string? selectedReader = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                               .Title($"[{FRipCore.ColorRules[1]}]Select a file-reader.[/]")
                                               .AddChoices(fileReaderNames)
                                               .HighlightStyle(FRipCore.ColorRules[4]));
            
            FRipCore.CurrentReader = null;
            foreach (IFileReader fileReader in fileReaders)
                if ($"[{FRipCore.ColorRules[3]}]{fileReader.Name}[/]" == selectedReader)
                {
                    FRipCore.CurrentReader = fileReader;
                    break;
                }

            if (FRipCore.CurrentReader == null)
                AutoDetectReader();
        }

        static void AutoDetectReader()
        {
            string ext = Path.GetExtension(FRipCore.FilePath).ToLower();
            switch (ext)
            {
                default:
                    FRipCore.CurrentReader = new CCNFileReader();
                    ((CCNFileReader)FRipCore.CurrentReader).CheckUnpacked(fileReader!);
                    break;
                case ".exe":
                    FRipCore.CurrentReader = new EXEFileReader();
                    break;
                case ".mfa":
                    FRipCore.CurrentReader = new MFAFileReader();
                    break;
                case ".anm":
                    FRipCore.CurrentReader = new ANMFileReader();
                    break;
                case ".tmp":
                    FRipCore.CurrentReader = new AGMIFileReader();
                    break;
                case ".zip":
                    FRipCore.CurrentReader = new OpenFileReader();
                    break;
                case ".apk":
                    FRipCore.CurrentReader = new APKFileReader();
                    break;
                case ".ipa":
                    FRipCore.CurrentReader = new IPAFileReader();
                    break;
            }
        }

        static void ReadPackage()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(FRipCore.ConsoleFiglet);
            AnsiConsole.Write(FRipCore.ConsoleRule);

            readStopwatch.Restart();
            AnsiConsole.MarkupLine($"[{FRipCore.ColorRules[1]}]Reading game as \"{FRipCore.CurrentReader.Name}\"[/]");
            Task readTask = new Task(() => FRipCore.CurrentReader.LoadGame(fileReader!, FRipCore.FilePath));
            readTask.Start();
            while (true)
            {
                if (FRipCore.PackageData != null)
                {
                    FRipCore.PackageData.CliUpdate();
                    break;
                }
            }
            readStopwatch.Stop();
        }

        static void SelectPlugin()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(FRipCore.ConsoleFiglet);
            AnsiConsole.Write(FRipCore.ConsoleRule);

            AnsiConsole.MarkupLine($"[{FRipCore.ColorRules[1]}]Reading finished in {readStopwatch.Elapsed.TotalSeconds} seconds[/]");
            List<IFRipPlugin> plugins = new();
            List<string> pluginNames = new()
            {
                $"[{FRipCore.ColorRules[3]}]Quit[/]"
            };

            Directory.CreateDirectory("Plugins");
            foreach (var item in Directory.GetFiles("Plugins", "*.dll"))
            {
                var newAsm = Assembly.LoadFrom(Path.GetFullPath(item));
                foreach (var pluginType in newAsm.GetTypes())
                    if (pluginType.GetInterface(typeof(IFRipPlugin).FullName) != null)
                    {
                        IFRipPlugin plugin = (IFRipPlugin)Activator.CreateInstance(pluginType);
                        plugins.Add(plugin);
                        pluginNames.Add($"[{FRipCore.ColorRules[3]}]{plugin.Name}[/]");
                    }
            }

            List<string> selectedTasks = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title($"[{FRipCore.ColorRules[1]}]Select a task.[/]")
                    .InstructionsText(
                        $"[{FRipCore.ColorRules[2]}](Press [{FRipCore.ColorRules[1]}]<space>[/] to select a task, " +
                        $"[{FRipCore.ColorRules[1]}]<enter>[/] to execute)\n(Quit will always execute last.)[/]")
                    .AddChoices(pluginNames)
                    .HighlightStyle(FRipCore.ColorRules[4]));


            foreach (IFRipPlugin plugin in plugins)
                if (selectedTasks.Contains($"[{FRipCore.ColorRules[3]}]{plugin.Name}[/]"))
                    plugin.Execute();

            if (selectedTasks.Contains($"[{FRipCore.ColorRules[3]}]Quit[/]"))
                Environment.Exit(0);
            else SelectPlugin();
        }
    }
}