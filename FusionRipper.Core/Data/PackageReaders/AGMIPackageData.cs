using FusionRipper.Core.Memory;
using FusionRipper.Core.Utilities;
using Spectre.Console;

namespace FusionRipper.Core.Data.PackageReaders
{
    public class AGMIPackageData : PackageData
    {
        public override void Read(ByteReader reader)
        {
            Logger.Log(this, $"Running {FRipCore.BuildDate} build.");
            Header = reader.ReadAscii(4);
            Logger.Log(this, "Header: " + Header);
            FRipCore.MFA = true;
            ImageBank.ReadMFA(reader);
        }

        public override void CliUpdate()
        {
            AnsiConsole.Progress().Start(ctx =>
            {
                ProgressTask? imageReading = ctx.AddTask("[DeepSkyBlue3]Reading images[/]", true);

                while (!imageReading.IsFinished)
                {
                    if (FRipCore.PackageData != null && ImageBank.Images.Count > 0)
                    {
                        if (!imageReading.IsStarted)
                            imageReading.StartTask();

                        imageReading.Value = Data.Chunks.BankChunks.Images.ImageBank.LoadedImageCount;
                        imageReading.MaxValue = FRipCore.PackageData.ImageBank.ImageCount;
                    }
                }
            });
        }
    }
}
