using Nebula;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Utilities;
using Nebula.Tools.GameDumper;
using Spectre.Console;
using System.Drawing;
using Image = Nebula.Core.Data.Chunks.BankChunks.Images.Image;

namespace GameDumper.AssetDumpers
{
    public class SpriteSheetDumper
    {
        public static void Execute()
        {
            ProgressTask? task = AssetDump.ProgressContext!.AddTask($"[{NebulaCore.ColorRules[4]}]Dumping Sprite Sheets[/]", false);

            string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\SpriteSheets";
            while (!task.IsFinished)
            {
                if (NebulaCore.PackageData.ImageBank != null && NebulaCore.PackageData.FrameItems != null)
                {
                    if (NebulaCore.PackageData.ImageBank.Images.Count == 0 ||
                        NebulaCore.PackageData.FrameItems.Items.Count == 0)
                        return;

                    if (!task.IsStarted)
                        task.StartTask();

                    task.Value = 0;
                    task.MaxValue = NebulaCore.PackageData.FrameItems.Items.Count;

                    ObjectInfo[] objects = NebulaCore.PackageData.FrameItems.Items.Values.ToArray();
                    Directory.CreateDirectory(path);
                    for (int i = 0; i < objects.Length; i++)
                    {
                        ObjectInfo oI = objects[i];
                        switch (oI.Header.Type)
                        {
                            case 0: // Quick Backdrop
                                ObjectQuickBackdrop qBD = (ObjectQuickBackdrop)oI.Properties;
                                if (qBD.Shape.FillType != 3) continue;
                                Bitmap qBDOutput = new Bitmap(qBD.Width, qBD.Height);
                                using (Graphics qBDGraphics = Graphics.FromImage(qBDOutput))
                                {
                                    Image qBDImg = NebulaCore.PackageData.ImageBank.Images[qBD.Shape.Image];
                                    Rectangle qBDRect = new Rectangle(0, 0, qBD.Width, qBD.Height);
                                    Utilities.doDraw(qBDGraphics, qBDImg.GetBitmap(), qBDRect, 1.0f);
                                    qBDImg.DisposeBmp();
                                }
                                qBDOutput.Save($"{path}\\[{i}] {oI.Name}.png");
                                break;
                            case 1: // Backdrop
                                ObjectBackdrop bD = (ObjectBackdrop)oI.Properties;
                                Image bDImg = NebulaCore.PackageData.ImageBank.Images[bD.Image];
                                bDImg.GetBitmap().Save($"{path}\\[{i}] {oI.Name}.png");
                                bDImg.DisposeBmp();
                                break;
                            default: // Object Common
                                ObjectCommon oC = (ObjectCommon)oI.Properties;
                                switch (oI.Header.Type)
                                {
                                    case 2: // Active
                                        foreach (KeyValuePair<int, ObjectAnimation> aAnim in oC.ObjectAnimations.Animations)
                                        {
                                            foreach (ObjectDirection aDir in aAnim.Value.Directions)
                                            {
                                                int aColumns = (int)Math.Floor(Math.Pow(aDir.Frames.Length, 0.5));
                                                int aWidth, aHeight;
                                                foreach (uint aFrm in aDir.Frames)
                                                {
                                                    Image aImg = NebulaCore.PackageData.ImageBank.Images[aFrm];
                                                    int aImgWidth = aImg.Width;
                                                }
                                            }
                                        }
                                        break;
                                }
                                break;
                        }
                        //(path + "\\" + i + ".png");
                        task.Value++;
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[Red]Could not find the image bank.[/]");
                    Console.ReadKey();
                }
            }
        }
    }
}
