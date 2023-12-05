using SapphireD;
using SapphireD.Core.Data;
using SapphireD.Core.Data.Chunks.AppChunks;
using SapphireD.Core.Data.Chunks.BankChunks.Fonts;
using SapphireD.Core.Data.Chunks.BankChunks.Images;
using SapphireD.Core.Data.Chunks.FrameChunks;
using SapphireD.Core.Data.Chunks.FrameChunks.Events;
using SapphireD.Core.Data.Chunks.MFAChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using Image = SapphireD.Core.Data.Chunks.BankChunks.Images.Image;

#pragma warning disable CA1416 // Validate platform compatibility
namespace GameDumper
{
    public class MFAParser : SapDPlugin
    {
        public string Name => "MFA Parser";

        public ImageBank IconBank = new ImageBank();

        public void Execute()
        {
            PackageData dat = SapDCore.PackageData;
            string path = "Dumps\\" + Utilities.ClearName(dat.AppName) + "\\";
            Directory.CreateDirectory(path);
            ByteWriter writer = new ByteWriter(new FileStream(path + Utilities.ClearName(Path.GetFileName(dat.EditorFilename)), FileMode.Create));

            IconBank.GraphicMode = dat.ImageBank.GraphicMode = dat.AppHeader.GraphicMode;
            IconBank.PaletteVersion = dat.ImageBank.PaletteVersion = dat.Frames[0].FramePalette.PaletteVersion;
            IconBank.PaletteEntries = dat.ImageBank.PaletteEntries = (short)dat.Frames[0].FramePalette.PaletteEntries;
            IconBank.Palette = dat.ImageBank.Palette = dat.Frames[0].FramePalette.Palette;

            foreach (ObjectInfo objectInfo in dat.FrameItems.Items.Values)
            {
                Bitmap iconBmp = new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\MMFActive.png"));
                switch (objectInfo.Header.Type)
                {
                    case 0: // Quick Backdrop
                        try
                        {
                            Bitmap bmp = dat.ImageBank.Images[((ObjectQuickBackdrop)objectInfo.Properties).Shape.Image].GetBitmap();
                            if (bmp.Width > bmp.Height)
                                iconBmp = bmp.ResizeImage(new Size(32, (int)Math.Round((float)bmp.Height / bmp.Width * 32.0)));
                            else
                                iconBmp = bmp.ResizeImage(new Size((int)Math.Round((float)bmp.Width / bmp.Height * 32.0), 32));
                        }
                        catch
                        {
                            iconBmp = new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\MMFQuickBackdrop.png"));
                        }
                        break;
                    case 1: // Backdrop
                        try
                        {
                            Bitmap bmp = dat.ImageBank.Images[((ObjectBackdrop)objectInfo.Properties).Image].GetBitmap();
                            if (bmp.Width > bmp.Height)
                                iconBmp = bmp.ResizeImage(new Size(32, (int)Math.Round((float)bmp.Height / bmp.Width * 32.0)));
                            else
                                iconBmp = bmp.ResizeImage(new Size((int)Math.Round((float)bmp.Width / bmp.Height * 32.0), 32));
                        }
                        catch
                        {
                            iconBmp = new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\MMFBackdrop.png"));
                        }
                        break;
                    case 2: // Active
                        try
                        {
                            Bitmap bmp = dat.ImageBank.Images[((ObjectCommon)objectInfo.Properties).ObjectAnimations.Animations[0].Directions[0].Frames[0]].GetBitmap();
                            if (bmp.Width > bmp.Height)
                                iconBmp = bmp.ResizeImage(new Size(32, (int)Math.Round((float)bmp.Height / bmp.Width * 32.0)));
                            else
                                iconBmp = bmp.ResizeImage(new Size((int)Math.Round((float)bmp.Width / bmp.Height * 32.0), 32));
                        }
                        catch
                        {
                            iconBmp = new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\MMFActive.png"));
                        }
                        break;
                    case 3: // String
                        iconBmp = new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\MMFString.png"));
                        break;
                    case 4: // Question and Answer
                        iconBmp = new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\MMFQ&A.png"));
                        break;
                    case 5: // Score
                        iconBmp = new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\MMFScore.png"));
                        break;
                    case 6: // Lives
                        iconBmp = new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\MMFLives.png"));
                        break;
                    case 7: // Counter
                        try
                        {
                            Bitmap bmp = dat.ImageBank.Images[((ObjectCommon)objectInfo.Properties).ObjectCounter.Frames[0]].GetBitmap();
                            if (bmp.Width > bmp.Height)
                                iconBmp = bmp.ResizeImage(new Size(32, (int)Math.Round((float)bmp.Height / bmp.Width * 32.0)));
                            else
                                iconBmp = bmp.ResizeImage(new Size((int)Math.Round((float)bmp.Width / bmp.Height * 32.0), 32));
                        }
                        catch
                        {
                            iconBmp = new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\MMFCounter.png"));
                        }
                        break;
                    case 8: // Formatted Text
                        iconBmp = new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\MMFFormattedText.png"));
                        break;
                    case 9: // Sub-Application
                        iconBmp = new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\MMFSubApplication.png"));
                        break;
                    case >= 32:
                        foreach (Extension posExt in dat.Extensions.Exts)
                            if (posExt.Handle == objectInfo.Header.Type - 32)
                            {
                                if (File.Exists("Plugins\\ObjectIcons\\" + posExt.Name + ".png"))
                                    iconBmp = new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\" + posExt.Name + ".png"));
                                break;
                            }
                        break;
                }

                var newIconImage = new Image();
                newIconImage.Handle = (uint)IconBank.Images.Count;
                newIconImage.FromBitmap(iconBmp);
                IconBank.Images.Add(newIconImage.Handle, newIconImage);
            }
            IconBank.ImageCount = IconBank.Images.Count;

            writer.WriteAscii("MFU2");
            writer.WriteShort(6);
            writer.WriteShort(dat.RuntimeSubversion);
            writer.WriteInt(dat.ProductVersion);
            writer.WriteInt(dat.ProductBuild);
            writer.WriteInt(0);
            writer.WriteAutoYunicode(dat.AppName);
            writer.WriteAutoYunicode("");
            writer.WriteAutoYunicode(dat.EditorFilename);
            writer.WriteInt(0);

            writer.WriteAscii("ATNF"); // Font Bank
            dat.FontBank.WriteMFA(writer);
            writer.WriteAscii("APMS"); // Sound Bank
            dat.SoundBank.WriteMFA(writer);
            writer.WriteAscii("ASUM"); // Music Bank
            writer.WriteInt(0);
            writer.WriteAscii("AGMI"); // Icon Bank
            IconBank.WriteMFA(writer);
            writer.WriteAscii("AGMI"); // Image Bank
            dat.ImageBank.WriteMFA(writer);

            writer.WriteAutoYunicode(dat.AppName);
            writer.WriteAutoYunicode(dat.Author);
            writer.WriteAutoYunicode("");
            writer.WriteAutoYunicode(dat.Copyright);
            writer.WriteAutoYunicode("");
            writer.WriteAutoYunicode("");
            writer.WriteInt(dat.AppHeader.AppWidth);
            writer.WriteInt(dat.AppHeader.AppHeight);
            writer.WriteColor(dat.AppHeader.BorderColor);
            dat.AppHeader.SyncFlags();
            writer.WriteUInt(dat.AppHeader.DisplayFlags.Value);
            writer.WriteUInt(dat.AppHeader.GraphicFlags.Value);
            writer.WriteAutoYunicode(dat.HelpFile);
            writer.WriteAutoYunicode("");
            writer.WriteInt((dat.AppHeader.InitScore + 1) * -1);
            writer.WriteInt((dat.AppHeader.InitLives + 1) * -1);
            writer.WriteInt(dat.AppHeader.FrameRate);
            writer.WriteInt(0);
            writer.WriteAutoYunicode(dat.TargetFilename);
            writer.WriteAutoYunicode("");
            writer.WriteAutoYunicode("");
            writer.WriteAutoYunicode(dat.About + " | SapphireD");
            writer.WriteInt(0);
            dat.BinaryFiles.WriteMFA(writer);

            writer.WriteInt(dat.AppHeader.ControlType.Length);
            for (int i = 0; i < dat.AppHeader.ControlType.Length; i++)
            {
                writer.WriteInt(dat.AppHeader.ControlType[i]);
                writer.WriteInt(dat.AppHeader.ControlKeys[i].Length);
                for (int ii = 0; ii < dat.AppHeader.ControlKeys[i].Length; ii++)
                    writer.WriteInt(dat.AppHeader.ControlKeys[i][ii]);
            }

            dat.MenuBar.WriteMFA(writer);
            writer.WriteInt(dat.AppHeader.WindowMenu);

            // Menu Images (Not implemented yet)
            writer.WriteInt(0);

            dat.GlobalValues.WriteMFA(writer, dat.GlobalValueNames);
            dat.GlobalStrings.WriteMFA(writer, dat.GlobalStringNames);
            writer.WriteInt(0); // Global Events

            writer.WriteInt(dat.AppHeader.GraphicMode);
            writer.WriteInt(9); // Icon Images
            for (int i = 0; i < 9; i++) // Icon Images
                writer.WriteInt(i); // Icon Image
            writer.WriteInt(0); // Qualifiers
            dat.Extensions.WriteMFA(writer);
            writer.WriteInt(dat.Frames.Count);

            long offsetEnd = writer.Tell() + 4 * dat.Frames.Count + 4;
            ByteWriter frameWriter = new ByteWriter(new MemoryStream());
            for (int i = 0; i < dat.Frames.Count; i++)
            {
                writer.WriteUInt((uint)(offsetEnd + frameWriter.Tell()));
                dat.Frames[i].WriteMFA(frameWriter, i);
            }

            writer.WriteUInt((uint)(offsetEnd + frameWriter.Tell()));
            writer.WriteWriter(frameWriter);

            writer.WriteByte(0);
            //writer.WriteBytes(File.ReadAllBytes("Plugins\\MFAChunks.bin"));

            frameWriter.Flush();
            frameWriter.Close();
            writer.Flush();
            writer.Close();
        }
    }
}
