using Nebula;
using Nebula.Core.Data.Chunks.BankChunks.Images;
using Nebula.Core.Data.PackageReaders;
using Nebula.Core.Utilities;
using Nebula.Plugins.GameDumper;
using Spectre.Console;

namespace GameDumper.AssetDumpers
{
    public class IconDumper
    {
        public static void Execute()
        {
            ProgressTask? task = AssetDump.ProgressContext!.AddTask($"[{NebulaCore.ColorRules[4]}]Dumping Icons[/]");
            string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\Icons\\";
            task.MaxValue = ((MFAPackageData)NebulaCore.PackageData).IconBank.Images.Count;
            Directory.CreateDirectory(path);

            Image[] icons = ((MFAPackageData)NebulaCore.PackageData).IconBank.Images.Values.ToArray();
            for (int i = 0; i < icons.Length; i++)
            {
                icons[i].GetBitmap().Save(path + icons[i].Handle + ".png");
                icons[i].DisposeBmp();
                task.Value++;
            }
            task.StopTask();
        }
    }
}
