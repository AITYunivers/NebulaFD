using GameDumper.AssetDumpers;
using Nebula.Core.Data.PackageReaders;
using Nebula.Core.Utilities;
using Spectre.Console;

namespace Nebula.Tools.GameDumper
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

                Task.WaitAll(runningTasks.ToArray());
            });
        }
    }
}