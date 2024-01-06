using FusionRipper;
using FusionRipper.Core.Utilities;
using Spectre.Console;

namespace MappingTool
{
    public class ReadMap : IFRipPlugin
    {
        public string Name => "Read Map File";

        public void Execute()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(FRipCore.ConsoleFiglet);
            AnsiConsole.Write(FRipCore.ConsoleRule);

            AnsiConsole.Markup("[Red]WORK IN PROGRESS![/]");
            Console.ReadKey();
        }
    }
}