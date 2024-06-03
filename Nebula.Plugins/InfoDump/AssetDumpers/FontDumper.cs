using Nebula;
using Nebula.Core.Data.Chunks.BankChunks.Music;
using Nebula.Core.Data.Chunks.BankChunks.TrueTypeFonts;
using Nebula.Core.Utilities;
using Nebula.Plugins.GameDumper;
using Spectre.Console;

namespace GameDumper.AssetDumpers
{
    public class FontDumper
    {
        public static void Execute()
        {
            ProgressTask? task = AssetDump.ProgressContext!.AddTask($"[{NebulaCore.ColorRules[4]}]Dumping Fonts[/]", false);
            string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\Fonts\\";
            task.MaxValue = NebulaCore.PackageData.TrueTypeFontBank.Fonts.Count;
            Directory.CreateDirectory(path);

            TrueTypeFont[] font = NebulaCore.PackageData.TrueTypeFontBank.Fonts.ToArray();
            for (int i = 0; i < font.Length; i++)
            {
                File.WriteAllBytes(path + font[i].Name + ".ttf", font[i].FontData);
                task.Value++;
            }
            task.StopTask();
        }
    }
}
