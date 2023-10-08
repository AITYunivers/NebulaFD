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
            AnsiConsole.Write(new FigletText("SapphireD").Centered().Color(Color.DeepSkyBlue1));

            AnsiConsole.Progress().Start(ctx =>
            {
                ProgressTask? task = ctx.AddTask("[DeepSkyBlue3]Dumping images[/]", false);

                int progress = 0;
                string path = "Dumps\\" + Utilities.ClearName(SapDCore.PackageData.AppName) + "\\Images";
                while (!task.IsFinished)
                {
                    if (SapDCore.PackageData.ImageBank != null)
                    {
                        if (!task.IsStarted)
                            task.StartTask();

                        task.Value = progress;
                        task.MaxValue = SapDCore.PackageData.ImageBank.Images.Count;

                        foreach (Image image in SapDCore.PackageData.ImageBank.Images.Values.ToArray())
                        {
                            Directory.CreateDirectory(path);
                            image.GetBitmap().Save(path + "\\" + image.Handle + ".png");
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