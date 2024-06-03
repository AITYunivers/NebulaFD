using Nebula.Core.Data.Chunks.BankChunks.Images;
using Nebula.Core.Data.PackageReaders;
using Nebula.Core.Utilities;
using Spectre.Console;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using Image = Nebula.Core.Data.Chunks.BankChunks.Images.Image;
using System.Diagnostics;
using Nebula.Core.Data.Chunks.BankChunks.Sounds;
using System.Reflection;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Data.Chunks.BankChunks.Music;
using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.FileReaders;
using Nebula.Core.Data.Chunks.BankChunks.Shaders;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using GameDumper.AssetDumpers;
#pragma warning disable CA1416

namespace Nebula.Plugins.GameDumper
{
    public class AssetDump : INebulaTool
    {
        public string Name => "Asset Dumper";
        public static ProgressContext? ProgressContext;

        public void Execute()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            List<string> tasknames = new()
            {
                $"[{NebulaCore.ColorRules[3]}]Exit[/]",
            };

            if (NebulaCore.PackageData.ImageBank.Images.Count > 0)
                tasknames.Add($"[{NebulaCore.ColorRules[3]}]Dump Images[/]");
            if (NebulaCore.PackageData is MFAPackageData && ((MFAPackageData)NebulaCore.PackageData).IconBank.Images.Count > 0)
                tasknames.Add($"[{NebulaCore.ColorRules[3]}]Dump Icons[/]");
            if (NebulaCore.PackageData.SoundBank.Sounds.Count > 0)
                tasknames.Add($"[{NebulaCore.ColorRules[3]}]Dump Sounds[/]");
            if (NebulaCore.PackageData.MusicBank.Music.Count > 0)
                tasknames.Add($"[{NebulaCore.ColorRules[3]}]Dump Music[/]");
            if (NebulaCore.PackageData.TrueTypeFontBank.Fonts.Count > 0)
                tasknames.Add($"[{NebulaCore.ColorRules[3]}]Dump Fonts[/]");
            if (NebulaCore.PackageData.PackData.Items.Length > 0)
                tasknames.Add($"[{NebulaCore.ColorRules[3]}]Dump Packed Data[/]");
            if (NebulaCore.PackageData.BinaryFiles.Items.Count > 0)
                tasknames.Add($"[{NebulaCore.ColorRules[3]}]Dump Binary Files[/]");
            if (NebulaCore.PackageData.ShaderBank.Shaders.Count > 0)
                tasknames.Add($"[{NebulaCore.ColorRules[3]}]Dump Shaders[/]");

            List<string> selectedTasks = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title($"[{NebulaCore.ColorRules[1]}]Select a task.[/]")
                    .InstructionsText(
                        $"[{NebulaCore.ColorRules[2]}](Press [{NebulaCore.ColorRules[1]}]<space>[/] to select a task, " +
                        $"[{NebulaCore.ColorRules[1]}]<enter>[/] to execute)[/]")
                    .AddChoices(tasknames)
                    .HighlightStyle(NebulaCore.ColorRules[4]));


            List<Task> runningTasks = new List<Task>();
            foreach (string task in selectedTasks)
            {
                switch (Markup.Remove(task))
                {
                    case "Dump Images":
                        runningTasks.Add(new Task(ImageDumper.Execute));
                        break;
                    case "Dump Icons":
                        runningTasks.Add(new Task(IconDumper.Execute));
                        break;
                    case "Dump Sprite Sheets":
                        runningTasks.Add(new Task(SpriteSheetDumper.Execute));
                        break;
                    case "Dump Sounds":
                        runningTasks.Add(new Task(SoundDumper.Execute));
                        break;
                    case "Dump Music":
                        runningTasks.Add(new Task(MusicDumper.Execute));
                        break;
                    case "Dump Fonts":
                        runningTasks.Add(new Task(FontDumper.Execute));
                        break;
                    case "Dump Packed Data":
                        runningTasks.Add(new Task(PackDataDumper.Execute));
                        break;
                    case "Dump Binary Files":
                        runningTasks.Add(new Task(BinaryFileDumper.Execute));
                        break;
                    case "Dump Shaders":
                        runningTasks.Add(new Task(ShaderDumper.Execute));
                        break;
                }
            }

            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            AnsiConsole.Progress().Start(ctx =>
            {
                ProgressContext = ctx;

                foreach (Task task in runningTasks)
                    task.Start();

                foreach (Task task in runningTasks)
                    task.Wait();
            });
        }
    }
}