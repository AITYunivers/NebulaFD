using Nebula.Core.Data;
using Nebula.Tools.MappingTool.Structure;
using Newtonsoft.Json;
using Spectre.Console;
using static Nebula.Core.Utilities.Enums;
using static Nebula.Tools.MappingTool.Structure.MapStructure;
using NObjectInfo = Nebula.Core.Data.Chunks.ObjectChunks.ObjectInfo;
using NObjectCommon = Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon.ObjectCommon;
using NObjectAnimations = Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon.ObjectAnimations;
using NObjectAnimation = Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon.ObjectAnimation;
using NObjectExtension = Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon.ObjectExtension;

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
                if (File.Exists(path) && Path.GetExtension(path) == ".nmf")
                {
                    NebulaCore.FilePath = path;
                    try
                    {
                        Project = JsonConvert.DeserializeObject<MapStructure.Project>(File.ReadAllText(path))!;
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
            if (Project.Values != null)
                ReadValues(Project.Values, pckg);
            if (Project.ObjectInfos != null)
                ReadObjects(Project, pckg);
            /*pckg.AppHeader.GraphicMode = (short)Project.Settings.GraphicMode;
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
            // Build Cache*/
        }

        public static void ReadValues(Values proj, PackageData data)
        {
            if (data.GlobalValues.Values.Length < proj.GlobalValues.Length)
                Array.Resize(ref data.GlobalValues.Values, proj.GlobalValues.Length);
            if (data.GlobalValueNames.Names.Length < proj.GlobalValues.Length)
                Array.Resize(ref data.GlobalValueNames.Names, proj.GlobalValues.Length);

            for (int i = 0; i < proj.GlobalValues.Length; i++)
                data.GlobalValueNames.Names[i] = proj.GlobalValues[i].Name;

            if (data.GlobalStrings.Strings.Length < proj.GlobalStrings.Length)
                Array.Resize(ref data.GlobalStrings.Strings, proj.GlobalStrings.Length);
            if (data.GlobalStringNames.Names.Length < proj.GlobalStrings.Length)
                Array.Resize(ref data.GlobalValueNames.Names, proj.GlobalStrings.Length);

            for (int i = 0; i < proj.GlobalStrings.Length; i++)
                data.GlobalStringNames.Names[i] = proj.GlobalStrings[i].Name;
        }

        public static void ReadObjects(MapStructure.Project proj, PackageData data)
        {
            foreach (KeyValuePair<uint, ObjectInfo> objInfoData in proj.ObjectInfos)
            {
                NObjectInfo objInfo = data.FrameItems.Items[(int)objInfoData.Key];
                NObjectCommon common = (NObjectCommon)objInfo.Properties;
                ReadObjectValues(objInfoData.Value.Values, common);
                if (objInfoData.Value.ActiveData != null)
                    ReadObjectActiveData(objInfoData.Value.ActiveData, common.ObjectAnimations);
            }
        }

        public static void ReadObjectValues(ObjectValues proj, NObjectCommon data)
        {
            if (data.ObjectAlterableValues.AlterableValues.Length < proj.AlterableValues.Length)
                Array.Resize(ref data.ObjectAlterableValues.AlterableValues, proj.AlterableValues.Length);
            if (data.ObjectAlterableValues.Names.Length < proj.AlterableValues.Length)
                Array.Resize(ref data.ObjectAlterableValues.Names, proj.AlterableValues.Length);

            for (int i = 0; i < proj.AlterableValues.Length; i++)
                data.ObjectAlterableValues.Names[i] = proj.AlterableValues[i].Name;


            if (data.ObjectAlterableStrings.AlterableStrings.Length < proj.AlterableStrings.Length)
                Array.Resize(ref data.ObjectAlterableStrings.AlterableStrings, proj.AlterableStrings.Length);
            if (data.ObjectAlterableStrings.Names.Length < proj.AlterableStrings.Length)
                Array.Resize(ref data.ObjectAlterableStrings.Names, proj.AlterableStrings.Length);

            for (int i = 0; i < proj.AlterableStrings.Length; i++)
                data.ObjectAlterableStrings.Names[i] = proj.AlterableStrings[i].Name;


            if (data.ObjectAlterableValues.FlagNames.Length < proj.AlterableFlags.Length)
                Array.Resize(ref data.ObjectAlterableValues.FlagNames, proj.AlterableFlags.Length);

            for (int i = 0; i < proj.AlterableFlags.Length; i++)
                data.ObjectAlterableValues.FlagNames[i] = proj.AlterableFlags[i].Name;
        }

        public static void ReadObjectActiveData(ActiveData proj, NObjectAnimations data)
        {
            foreach (KeyValuePair<int, NObjectAnimation> animData in data.Animations)
                if (proj.Animations.ContainsKey((uint)animData.Key))
                    animData.Value.Name = proj.Animations[(uint)animData.Key].Name;
        }
    }
}