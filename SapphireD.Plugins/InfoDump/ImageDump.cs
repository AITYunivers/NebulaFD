using SapphireD.Core.Data.Chunks.BankChunks.Images;
using SapphireD.Core.Utilities;
using Spectre.Console;

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

                        Image[] images = SapDCore.PackageData.ImageBank.Images.Values.ToArray();
                        for (int i = 0; i < images.Length; i++)
                        {
                            Directory.CreateDirectory(path);
                            images[i].GetBitmap().Save(path + "\\" + images[i].Handle + ".png");
                            task.Value = ++progress;
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