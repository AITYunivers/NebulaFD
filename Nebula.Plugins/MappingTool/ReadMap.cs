using Nebula;
using Nebula.Core.Utilities;
using Spectre.Console;
using Newtonsoft.Json;

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

            JsonConvert.PopulateObject(File.ReadAllText("test.json"), NebulaCore.PackageData, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All});
        }
    }
}