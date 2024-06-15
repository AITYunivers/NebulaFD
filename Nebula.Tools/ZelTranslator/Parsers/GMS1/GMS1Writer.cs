using Nebula;
using Nebula.Core.Data;
using Nebula.Core.FileReaders;
using Nebula.Core.Utilities;
//using Nebula;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZelTranslator_SD.Parsers
{
    public class GMS1Writer
    {
        public static void Write(PackageData gameData)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            AnsiConsole.Markup("[Green]GameMaker Studio 1.4.9999 Translator (WIP)[/]");

        }
    }
}
