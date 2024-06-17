using Nebula.Core.Utilities;
using Nebula.Tools.MappingTool.Tools;
using Spectre.Console;

namespace Nebula.Tools.MappingTool
{
    public class MappingTool : INebulaTool
    {
        public string Name => "Mapping Tool";

        public void Execute()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            List<string> tasknames = new()
            {
                $"[{NebulaCore.ColorRules[3]}]Exit[/]",
                $"[{NebulaCore.ColorRules[3]}]Generate Map[/]",
                $"[{NebulaCore.ColorRules[3]}]Load Map[/]"
            };

            string selectedTask = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[{NebulaCore.ColorRules[1]}]Select a task.[/]")
                    .AddChoices(tasknames)
                    .HighlightStyle(NebulaCore.ColorRules[4]));


            List<Task> runningTasks = new List<Task>();
            switch (Markup.Remove(selectedTask))
            {
                case "Generate Map":
                    MakeMap.Execute();
                    break;
                case "Load Map":
                    ReadMap.Execute();
                    break;
            }

            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);
        }
    }
}
