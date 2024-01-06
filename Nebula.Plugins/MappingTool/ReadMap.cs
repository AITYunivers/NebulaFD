using Nebula;
using Nebula.Core.Utilities;
using Spectre.Console;

namespace MappingTool
{
    public class ReadMap : INebulaPlugin
    {
        public string Name => "Read Map File";

        public void Execute()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            AnsiConsole.Markup("[Red]WORK IN PROGRESS![/]");
            Console.ReadKey();
        }
    }
}