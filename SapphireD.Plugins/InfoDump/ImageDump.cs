using SapphireD.Core.Data.Chunks.BankChunks.Images;
using SapphireD.Core.Data.PackageReaders;
using SapphireD.Core.Utilities;
using Spectre.Console;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using Image = SapphireD.Core.Data.Chunks.BankChunks.Images.Image;

namespace SapphireD.Plugins.GameDumper
{
    public class ImageDump : SapDPlugin
    {
        public string Name => "Image Dumper";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public void Execute()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(SapDCore.ConsoleFiglet);
            AnsiConsole.Write(SapDCore.ConsoleRule);

            AnsiConsole.Progress().Start(ctx =>
            {
                ProgressTask? task = ctx.AddTask("[DeepSkyBlue3]Dumping images[/]", false);

                int progress = 0;
                string path = "Dumps\\" + Utilities.ClearName(SapDCore.PackageData.AppName) + "\\Images";
                while (!task.IsFinished)
                {
                    if (SapDCore.PackageData.ImageBank != null)
                    {
                        if (SapDCore.PackageData.ImageBank.Images.Count == 0)
                            return;

                        if (!task.IsStarted)
                            task.StartTask();

                        task.Value = progress;
                        task.MaxValue = SapDCore.PackageData.ImageBank.Images.Count;
                        if (SapDCore.PackageData is MFAPackageData && (SapDCore.PackageData as MFAPackageData).IconBank != null)
                            task.MaxValue += (SapDCore.PackageData as MFAPackageData).IconBank.Images.Count;

                        Image[] images = SapDCore.PackageData.ImageBank.Images.Values.ToArray();
                        for (int i = 0; i < images.Length; i++)
                        {
                            Directory.CreateDirectory(path);
                            images[i].GetBitmap().Save(path + "\\" + images[i].Handle + ".png");
                            task.Value = ++progress;
                        }

                        if (SapDCore.PackageData is MFAPackageData && (SapDCore.PackageData as MFAPackageData).IconBank != null)
                        {
                            if ((SapDCore.PackageData as MFAPackageData).IconBank.Images.Count == 0)
                                return;

                            images = (SapDCore.PackageData as MFAPackageData).IconBank.Images.Values.ToArray();
                            for (int i = 0; i < images.Length; i++)
                            {
                                Directory.CreateDirectory(path + "\\Icons");
                                images[i].GetBitmap().Save(path + "\\Icons\\" + images[i].Handle + ".png");
                                task.Value = ++progress;
                            }
                        }
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[Red]Could not find the image bank.[/]");
                        Console.ReadKey();
                    }
                }
            });
        }
    }
}