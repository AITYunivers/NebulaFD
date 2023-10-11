using SapphireD;
using SapphireD.Core.Utilities;
using Spectre.Console;

namespace MappingTool
{
    public class ReadMap : SapDPlugin
    {
        public string Name => "Read Map File";

        public void Execute()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(SapDCore.ConsoleFiglet);
            AnsiConsole.Write(SapDCore.ConsoleRule);

            AnsiConsole.Markup("[Red]WORK IN PROGRESS![/]");
            Console.ReadKey();
        }
    }
}