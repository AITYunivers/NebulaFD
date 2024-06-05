using Nebula;
using Nebula.Core.Data.Chunks.BankChunks.Images;
using Nebula.Core.Utilities;
using Nebula.Plugins.GameDumper;
using Spectre.Console;

namespace GameDumper.AssetDumpers
{
    public class ImageDumper
    {
        public static void Execute()
        {
            ProgressTask? task = AssetDump.ProgressContext!.AddTask($"[{NebulaCore.ColorRules[4]}]Dumping Images[/]");
            string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\Images\\";
            task.MaxValue = NebulaCore.PackageData.ImageBank.Images.Count;
            Directory.CreateDirectory(path);

            Image[] images = NebulaCore.PackageData.ImageBank.Images.Values.ToArray();
            for (int i = 0; i < images.Length; i++)
            {
                images[i].GetBitmap().Save(path + images[i].Handle + ".png");
                images[i].DisposeBmp();
                task.Value++;
            }
            task.StopTask();
        }
    }
}
