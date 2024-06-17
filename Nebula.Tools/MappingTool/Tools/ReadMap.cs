using Nebula.Core.Data;
using Nebula.Tools.MappingTool.Structure;
using Newtonsoft.Json;
using Spectre.Console;
using static Nebula.Core.Utilities.Enums;

namespace Nebula.Tools.MappingTool.Tools
{
    public class ReadMap
    {
        public static MapStructure.Project? Project;

        public static void Execute()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            AnsiConsole.MarkupLine($"[{NebulaCore.ColorRules[1]}]File Path:[/]");

            while (true)
            {
                string path = Console.ReadLine()!.Trim().Trim('"');
                if (File.Exists(path) && Path.GetExtension(path) == ".nmp")
                {
                    NebulaCore.FilePath = path;
                    try
                    {
                        Project = JsonConvert.DeserializeObject<MapStructure.Project>(path)!;
                        if (Project != null)
                            break;
                    }
                    catch { }
                }
                AnsiConsole.Clear();
                AnsiConsole.Write(NebulaCore.ConsoleFiglet);
                AnsiConsole.Write(NebulaCore.ConsoleRule);

                AnsiConsole.MarkupLine($"[{Color.Red}]Invalid Map File[/]");
                AnsiConsole.MarkupLine($"[{NebulaCore.ColorRules[1]}]File Path:[/]");
            }

            if (Project == null)
                return;

            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            AnsiConsole.MarkupLine($"[{NebulaCore.ColorRules[1]}]Reading map file.[/]");

            PackageData pckg = NebulaCore.PackageData;
            pckg.AppHeader.GraphicMode = (short)Project.Settings.GraphicMode;
            pckg.ExtendedHeader.BuildType = (byte)Project.Settings.BuildType;
            // Show Deprecated Build Types
            pckg.TargetFilename = Project.Settings.BuildFilename;
            pckg.ExtendedHeader.CompressionFlags["CompressionLevelMax"] = Project.Settings.CompressionLevel == MapStructure.CompressionLevel.Maximum;
            pckg.ExtendedHeader.CompressionFlags["CompressSounds"] = Project.Settings.CompressSounds;
            pckg.ExtendedHeader.CompressionFlags["DontDisplayBuildWarning"] = !Project.Settings.DisplayBuildWarningMessages;
            // Command Line
            pckg.AppHeader.OtherFlags[AppHeaderOtherFlags.DebuggerShortcuts] = Project.Settings.EnableDebuggerShortcuts;
            pckg.AppHeader.OtherFlags[AppHeaderOtherFlags.ShowDebugger] = Project.Settings.ShowDebugger;
            // Crash Display Last Event
            pckg.AppHeader.GraphicFlags["EnableProfiling"] = Project.Settings.EnableProfiler;
            pckg.AppHeader.GraphicFlags["DontStartProfiling"] = !Project.Settings.StartProfilingAtStartOfFrame;
            // Profile Top Level Conditions Only
            pckg.AppHeader.GraphicFlags["RecordSlowestLoops"] = Project.Settings.RecordSlowestAppLoops;
            pckg.AppHeader.GraphicFlags["DontOptimizeEvents"] = !Project.Settings.OptimizeEvents;
            pckg.ExtendedHeader.CompressionFlags["OptimizeImageSize"] = Project.Settings.OptimizeImageSizeInRAM;
            pckg.ExtendedHeader.Flags["OptimizePlaySample"] = Project.Settings.OptimizePlaySample;
            // Merge Play And Set Sample Actions
            // Build Cache
        }
    }
}