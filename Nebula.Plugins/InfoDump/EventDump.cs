using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.Data.Chunks.FrameChunks.Events;
using Nebula.Core.Utilities;
using Spectre.Console;

namespace Nebula.Plugins.GameDumper
{
    public class EventDump : INebulaTool
    {
        public string Name => "Event Dumper";

        public void Execute()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            AnsiConsole.Progress().Start(ctx =>
            {
                ProgressTask? task = ctx.AddTask($"[{NebulaCore.ColorRules[4]}]Dumping events[/]", false);

                int progress = 0;
                int max = 0;
                string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\Events";
                foreach (Frame frm in NebulaCore.PackageData.Frames.Values)
                    max += frm.FrameEvents.Events.Count;

                while (!task.IsFinished)
                {
                    if (NebulaCore.PackageData.Frames != null)
                    {
                        if (NebulaCore.PackageData.Frames.Count == 0)
                            return;

                        if (!task.IsStarted)
                            task.StartTask();

                        task.Value = progress;
                        task.MaxValue = max;

                        Directory.CreateDirectory(path);
                        foreach (Frame frm in NebulaCore.PackageData.Frames.Values)
                        {
                            string frmPath = Path.Combine(path, Utilities.ClearName(frm.FrameName) + ".txt");
                            File.WriteAllLines(frmPath, GetEventsAsString(frm));
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

        public List<string> GetEventsAsString(Frame frm)
        {
            List<string> evtStrs = new();
            int indent = 0;
            for (int e = 0; e < frm.FrameEvents.Events.Count; e++)
            {
                string header = "";
                for (int i = 0; i < indent; i++)
                    header += "\t";

                Event evt = frm.FrameEvents.Events[e];
                for (int c = 0; c < evt.Conditions.Count; c++)
                    evtStrs.Add(header + (c == 0 ? "* " : "+ ") + evt.Conditions[c].ToString());
                for (int a = 0; a < evt.Actions.Count; a++)
                    evtStrs.Add(header + "\t" + evt.Actions[a].ToString());

                if (e + 1 < frm.FrameEvents.Events.Count)
                    evtStrs.Add("");
            }
            return evtStrs;
        }
    }
}