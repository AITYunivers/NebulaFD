using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.Utilities;
using Spectre.Console;

namespace Nebula.Plugins.GameDumper
{
    public class FrameDump : INebulaPlugin
    {
        public string Name => "Frame Dumper";
        private bool ShowHiddenObjects = false;
        private bool DumpLayers = true;
        private bool ExpandBounds = false;

        public void Execute()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            AnsiConsole.Progress().Start(ctx =>
            {
                ProgressTask? task = ctx.AddTask($"[{NebulaCore.ColorRules[4]}]Dumping frames[/]", false);

                int progress = 0;
                string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\Frames";
                while (!task.IsFinished)
                {
                    if (NebulaCore.PackageData.Frames != null)
                    {
                        if (NebulaCore.PackageData.Frames.Count == 0)
                            return;

                        if (!task.IsStarted)
                            task.StartTask();

                        task.Value = progress;
                        task.MaxValue = NebulaCore.PackageData.Frames.Count;

                        foreach (Frame frm in NebulaCore.PackageData.Frames)
                        {
                            string frmPath = Path.Combine(path, Utilities.ClearName(frm.FrameName));
                            Directory.CreateDirectory(frmPath);
                            Utilities.MakeFrameImg(frm, -1, ShowHiddenObjects).Save(frmPath + "\\Frame.png");
                            if (DumpLayers)
                                for (int i = 0; i < frm.FrameLayers.Layers.Length; i++)
                                    Utilities.MakeFrameImg(frm, i, ShowHiddenObjects).Save(frmPath + "\\Layer " + i + ".png");
                            task.Value = ++progress;
                        }
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[Red]Could not find any frames.[/]");
                        Console.ReadKey();
                    }
                }
            });
        }
    }
}