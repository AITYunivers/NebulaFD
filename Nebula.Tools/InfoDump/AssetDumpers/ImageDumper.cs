using Nebula;
using Nebula.Core.Data.Chunks.BankChunks.Images;
using Nebula.Core.Utilities;
using Nebula.Tools.GameDumper;
using Spectre.Console;
using System.Drawing;
using Image = Nebula.Core.Data.Chunks.BankChunks.Images.Image;

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
            List<Task> imgTasks = new List<Task>();
            if (Parameters.GPUAcceleration)
                ImageTranslatorGPU.GetAccelerator();
            foreach (Image image in images)
            {
                Task imgTask = Task.Factory.StartNew(() =>
                {
                    Bitmap bmp = image.GetBitmap();
                    bmp.Save(path + image.Handle + ".png");
                    image.DisposeBmp();
                    task.Value++;
                });
                imgTasks.Add(imgTask);
            }
            Task.WaitAll(imgTasks.ToArray());
            task.StopTask();
        }
    }
}
