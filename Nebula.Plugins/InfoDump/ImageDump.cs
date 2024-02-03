using Nebula.Core.Data.Chunks.BankChunks.Images;
using Nebula.Core.Data.PackageReaders;
using Nebula.Core.Utilities;
using Spectre.Console;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using Image = Nebula.Core.Data.Chunks.BankChunks.Images.Image;
using System.Diagnostics;

namespace Nebula.Plugins.GameDumper
{
    public class ImageDump : INebulaPlugin
    {
        public string Name => "Image Dumper";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public void Execute()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            AnsiConsole.Progress().Start(ctx =>
            {
                ProgressTask? task = ctx.AddTask($"[{NebulaCore.ColorRules[4]}]Dumping images[/]", false);

                int progress = 0;
                string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\Images";
                while (!task.IsFinished)
                {
                    if (NebulaCore.PackageData.ImageBank != null)
                    {
                        if (NebulaCore.PackageData.ImageBank.Images.Count == 0)
                            return;

                        if (!task.IsStarted)
                            task.StartTask();

                        task.Value = progress;
                        task.MaxValue = NebulaCore.PackageData.ImageBank.Images.Count;
                        if (NebulaCore.PackageData is MFAPackageData && (NebulaCore.PackageData as MFAPackageData).IconBank != null)
                            task.MaxValue += (NebulaCore.PackageData as MFAPackageData).IconBank.Images.Count;

                        Image[] images = NebulaCore.PackageData.ImageBank.Images.Values.ToArray();
                        for (int i = 0; i < images.Length; i++)
                        {
                            Directory.CreateDirectory(path);
                            File.WriteAllBytes(path + "\\" + images[i].Handle + ".bin", images[i].ImageData);
                            images[i].GetBitmap().Save(path + "\\" + images[i].Handle + ".png");
                            task.Value = ++progress;
                            Debug.Assert(images[i].GraphicMode == 0);
                        }

                        if (NebulaCore.PackageData is MFAPackageData && (NebulaCore.PackageData as MFAPackageData).IconBank != null)
                        {
                            if ((NebulaCore.PackageData as MFAPackageData).IconBank.Images.Count == 0)
                                return;

                            images = (NebulaCore.PackageData as MFAPackageData).IconBank.Images.Values.ToArray();
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