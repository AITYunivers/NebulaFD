using Nebula.Core.FileReaders;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using Spectre.Console;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

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
            Logger.Save();
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            SpectreMain();
        }

        static void SpectreMain()
        {
            WaitForFile();
            SelectReader();
            ReadPackage();
            SelectTool();
        }

        static void WaitForFile()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            AnsiConsole.MarkupLine($"[{NebulaCore.ColorRules[1]}]File Path:[/]");
            
            string path = Console.ReadLine().Trim().Trim('"');
            if (File.Exists(path))
                NebulaCore.FilePath = path;
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


            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);
            AnsiConsole.Status().Spinner(Spinner.Known.Dots2).Start("Loading file", ctx =>
            {
                fileReader = new ByteReader(File.ReadAllBytes(NebulaCore.FilePath));
            });

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
                /* if (((EXEFileReader)NebulaCore.CurrentReader).CheckInstaller(fileReader!))
                        NebulaCore.CurrentReader = new INSTFileReader();
                    else if (((EXEFileReader)NebulaCore.CurrentReader).CheckChowdren(fileReader!))
                        NebulaCore.CurrentReader = new ChowdrenFileReader(); */
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
                case ".gam":
                    if (File.Exists(Path.Combine(Path.GetDirectoryName(NebulaCore.FilePath), Path.GetFileNameWithoutExtension(NebulaCore.FilePath) + ".img")))
                        NebulaCore.CurrentReader = new KNPFileReader();
                    else
                        NebulaCore.CurrentReader = new CCNFileReader();
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
#if !DEBUG
            try
            {
#endif
                NebulaCore.CurrentReader.LoadGame(fileReader!, NebulaCore.FilePath);
#if !DEBUG
            }
            catch (Exception ex)
            {
                Logger.Log(NebulaCore.CurrentReader.GetType(), ex.Message);
                Logger.Save();
                throw;
            }
#endif
            Logger.Save();
            GC.Collect();
            readStopwatch.Stop();
        }

        static Stopwatch? toolStopwatch = null;
        static void SelectTool()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            if (toolStopwatch == null)
                AnsiConsole.MarkupLine($"[{NebulaCore.ColorRules[1]}]Reading finished in {readStopwatch.Elapsed.TotalSeconds} seconds[/]");
            else
                AnsiConsole.MarkupLine($"[{NebulaCore.ColorRules[1]}]Tool(s) finished in {toolStopwatch.Elapsed.TotalSeconds} seconds[/]");
            List<INebulaTool> tools = new();
            List<string> toolNames = new()
            {
                $"[{NebulaCore.ColorRules[3]}]Quit[/]"
            };

            Directory.CreateDirectory("Tools");
            foreach (var item in Directory.GetFiles("Tools", "*.dll"))
            {
                var newAsm = Assembly.LoadFrom(Path.GetFullPath(item));
                foreach (var toolType in newAsm.GetTypes())
                    if (toolType.GetInterface(typeof(INebulaTool).FullName) != null)
                    {
                        INebulaTool tool = (INebulaTool)Activator.CreateInstance(toolType);
                        tools.Add(tool);
                        toolNames.Add($"[{NebulaCore.ColorRules[3]}]{tool.Name}[/]");
                    }
            }

            List<string> selectedTasks = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title($"[{NebulaCore.ColorRules[1]}]Select a task.[/]")
                    .InstructionsText(
                        $"[{NebulaCore.ColorRules[2]}](Press [{NebulaCore.ColorRules[1]}]<space>[/] to select a task, " +
                        $"[{NebulaCore.ColorRules[1]}]<enter>[/] to execute)\n(Quit will always execute last.)[/]")
                    .AddChoices(toolNames)
                    .HighlightStyle(NebulaCore.ColorRules[4]));
            selectedTasks = selectedTasks.Select(str => Markup.Remove(str)).ToList();

            toolStopwatch = Stopwatch.StartNew();
            foreach (INebulaTool tool in tools)
                if (selectedTasks.Contains(tool.Name))
                {
#if !DEBUG
                    try
                    {
#endif
                        tool.Execute();
#if !DEBUG
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(tool.GetType(), ex.Message);
                        Logger.Save();
                        throw;
                    }
#endif
                    Logger.Save();
                    GC.Collect();
                }
            toolStopwatch.Stop();

            if (selectedTasks.Contains($"Quit"))
                Environment.Exit(0);
            else SelectTool();
        }
    }
}