using Nebula.Core.FileReaders;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using Spectre.Console;
using System.Diagnostics;
using System.Reflection;

namespace Nebula
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

            NebulaCore.Init();
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
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            AnsiConsole.MarkupLine($"[{NebulaCore.ColorRules[1]}]File Path:[/]");
            
            string path = Console.ReadLine().Trim().Trim('"');
            if (File.Exists(path))
            {
                NebulaCore.FilePath = path;
                fileReader = new ByteReader(File.ReadAllBytes(path));
            }
            else WaitForFile();
        }

        static void SelectReader()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            var fileReaders = from t in Assembly.GetAssembly(typeof(NebulaCore)).GetTypes()
                            where t.GetInterfaces().Contains(typeof(IFileReader))
                            && t.GetConstructor(Type.EmptyTypes) != null
                            select Activator.CreateInstance(t) as IFileReader;

            List<string> fileReaderNames = new()
            {
                $"[{NebulaCore.ColorRules[3]}]Auto-Detect[/]"
            };

            foreach (IFileReader fileReader in fileReaders)
                fileReaderNames.Add($"[{NebulaCore.ColorRules[3]}]{fileReader.Name}[/]");

            string? selectedReader = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                               .Title($"[{NebulaCore.ColorRules[1]}]Select a file-reader.[/]")
                                               .AddChoices(fileReaderNames)
                                               .HighlightStyle(NebulaCore.ColorRules[4]));
            
            NebulaCore.CurrentReader = null;
            foreach (IFileReader fileReader in fileReaders)
                if ($"[{NebulaCore.ColorRules[3]}]{fileReader.Name}[/]" == selectedReader)
                {
                    NebulaCore.CurrentReader = fileReader;
                    break;
                }

            if (NebulaCore.CurrentReader == null)
                AutoDetectReader();
        }

        static void AutoDetectReader()
        {
            string ext = Path.GetExtension(NebulaCore.FilePath).ToLower();
            switch (ext)
            {
                default:
                    NebulaCore.CurrentReader = new CCNFileReader();
                    ((CCNFileReader)NebulaCore.CurrentReader).CheckUnpacked(fileReader!);
                    break;
                case ".exe":
                    NebulaCore.CurrentReader = new EXEFileReader();
                    break;
                case ".mfa":
                    NebulaCore.CurrentReader = new MFAFileReader();
                    break;
                case ".anm":
                    NebulaCore.CurrentReader = new ANMFileReader();
                    break;
                case ".tmp":
                    NebulaCore.CurrentReader = new AGMIFileReader();
                    break;
                case ".zip":
                    NebulaCore.CurrentReader = new OpenFileReader();
                    break;
                case ".apk":
                    NebulaCore.CurrentReader = new APKFileReader();
                    break;
                case ".ipa":
                    NebulaCore.CurrentReader = new IPAFileReader();
                    break;
            }
        }

        static void ReadPackage()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            readStopwatch.Restart();
            AnsiConsole.MarkupLine($"[{NebulaCore.ColorRules[1]}]Reading game as \"{NebulaCore.CurrentReader.Name}\"[/]");
            Task readTask = new Task(() => NebulaCore.CurrentReader.LoadGame(fileReader!, NebulaCore.FilePath));
            readTask.Start();
            while (true)
            {
                if (NebulaCore.PackageData != null)
                {
                    NebulaCore.PackageData.CliUpdate();
                    break;
                }
            }
            readStopwatch.Stop();
        }

        static void SelectPlugin()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            AnsiConsole.MarkupLine($"[{NebulaCore.ColorRules[1]}]Reading finished in {readStopwatch.Elapsed.TotalSeconds} seconds[/]");
            List<INebulaPlugin> plugins = new();
            List<string> pluginNames = new()
            {
                $"[{NebulaCore.ColorRules[3]}]Quit[/]"
            };

            Directory.CreateDirectory("Plugins");
            foreach (var item in Directory.GetFiles("Plugins", "*.dll"))
            {
                var newAsm = Assembly.LoadFrom(Path.GetFullPath(item));
                foreach (var pluginType in newAsm.GetTypes())
                    if (pluginType.GetInterface(typeof(INebulaPlugin).FullName) != null)
                    {
                        INebulaPlugin plugin = (INebulaPlugin)Activator.CreateInstance(pluginType);
                        plugins.Add(plugin);
                        pluginNames.Add($"[{NebulaCore.ColorRules[3]}]{plugin.Name}[/]");
                    }
            }

            List<string> selectedTasks = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title($"[{NebulaCore.ColorRules[1]}]Select a task.[/]")
                    .InstructionsText(
                        $"[{NebulaCore.ColorRules[2]}](Press [{NebulaCore.ColorRules[1]}]<space>[/] to select a task, " +
                        $"[{NebulaCore.ColorRules[1]}]<enter>[/] to execute)\n(Quit will always execute last.)[/]")
                    .AddChoices(pluginNames)
                    .HighlightStyle(NebulaCore.ColorRules[4]));


            foreach (INebulaPlugin plugin in plugins)
                if (selectedTasks.Contains($"[{NebulaCore.ColorRules[3]}]{plugin.Name}[/]"))
                    plugin.Execute();

            if (selectedTasks.Contains($"[{NebulaCore.ColorRules[3]}]Quit[/]"))
                Environment.Exit(0);
            else SelectPlugin();
        }
    }
}