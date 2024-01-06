using FusionRipper.Core.Data.Chunks.BankChunks.Images;
using FusionRipper.Core.Data.PackageReaders;
using FusionRipper.Core.Utilities;
using Spectre.Console;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using Image = FusionRipper.Core.Data.Chunks.BankChunks.Images.Image;

namespace FusionRipper.Plugins.GameDumper
{
    public class ImageDump : IFRipPlugin
    {
        public string Name => "Image Dumper";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public void Execute()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(FRipCore.ConsoleFiglet);
            AnsiConsole.Write(FRipCore.ConsoleRule);

            AnsiConsole.Progress().Start(ctx =>
            {
                ProgressTask? task = ctx.AddTask($"[{FRipCore.ColorRules[4]}]Dumping images[/]", false);

                int progress = 0;
                string path = "Dumps\\" + Utilities.ClearName(FRipCore.PackageData.AppName) + "\\Images";
                while (!task.IsFinished)
                {
                    if (FRipCore.PackageData.ImageBank != null)
                    {
                        if (FRipCore.PackageData.ImageBank.Images.Count == 0)
                            return;

                        if (!task.IsStarted)
                            task.StartTask();

                        task.Value = progress;
                        task.MaxValue = FRipCore.PackageData.ImageBank.Images.Count;
                        if (FRipCore.PackageData is MFAPackageData && (FRipCore.PackageData as MFAPackageData).IconBank != null)
                            task.MaxValue += (FRipCore.PackageData as MFAPackageData).IconBank.Images.Count;

                        Image[] images = FRipCore.PackageData.ImageBank.Images.Values.ToArray();
                        for (int i = 0; i < images.Length; i++)
                        {
                            Directory.CreateDirectory(path);
                            images[i].GetBitmap().Save(path + "\\" + images[i].Handle + ".png");
                            task.Value = ++progress;
                        }

                        if (FRipCore.PackageData is MFAPackageData && (FRipCore.PackageData as MFAPackageData).IconBank != null)
                        {
                            if ((FRipCore.PackageData as MFAPackageData).IconBank.Images.Count == 0)
                                return;

                            images = (FRipCore.PackageData as MFAPackageData).IconBank.Images.Values.ToArray();
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