using Nebula;
using Nebula.Core.Utilities;
using Spectre.Console;
using Newtonsoft.Json;

namespace Nebula.Tools.MappingTool
{
    // I'll probably have every map have to be created manually

    /*public class MakeMap : INebulaPlugin
    {
        public string Name => "Make Map File";

        public void Execute()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            File.WriteAllText("test.json", JsonConvert.SerializeObject(NebulaCore.PackageData, 
                Formatting.Indented, 
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All }
                ));
        }
    }*/
}