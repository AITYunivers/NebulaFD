using SapphireD;
using SapphireD.Core.Data;
using SapphireD.Core.Data.Chunks.AppChunks;
using SapphireD.Core.Data.Chunks.BankChunks.Fonts;
using SapphireD.Core.Data.Chunks.BankChunks.Images;
using SapphireD.Core.Data.Chunks.FrameChunks;
using SapphireD.Core.Data.Chunks.FrameChunks.Events;
using SapphireD.Core.Data.Chunks.MFAChunks;
using SapphireD.Core.Data.Chunks.MFAChunks.MFAObjectChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Image = SapphireD.Core.Data.Chunks.BankChunks.Images.Image;
using Size = System.Drawing.Size;

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
            IconBank.PaletteVersion = dat.ImageBank.PaletteVersion = dat.Frames.First().FramePalette.PaletteVersion;
            IconBank.PaletteEntries = dat.ImageBank.PaletteEntries = (short)dat.Frames.First().FramePalette.PaletteEntries;
            IconBank.Palette = dat.ImageBank.Palette = dat.Frames.First().FramePalette.Palette;

            foreach (ObjectInfo objectInfo in dat.FrameItems.Items.Values)
            {
                Bitmap iconBmp = new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\MMFActive.png"));
                try
                {
                    iconBmp = objectInfo.Header.Type switch
                    {
                        0 => MakeIcon(dat.ImageBank.Images[((ObjectQuickBackdrop)objectInfo.Properties).Shape.Image].GetBitmap(), "MMFQuickBackdrop"),
                        1 => MakeIcon(dat.ImageBank.Images[((ObjectBackdrop)objectInfo.Properties).Image].GetBitmap(), "MMFBackdrop"),
                        2 => MakeIcon(dat.ImageBank.Images[((ObjectCommon)objectInfo.Properties).ObjectAnimations.Animations[0].Directions[0].Frames[0]].GetBitmap(), "MMFActive"),
                        3 => new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\MMFString.png")),
                        4 => new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\MMFQ&A.png")),
                        5 => new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\MMFScore.png")),
                        6 => new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\MMFLives.png")),
                        7 => MakeIcon(dat.ImageBank.Images[((ObjectCommon)objectInfo.Properties).ObjectCounter.Frames[0]].GetBitmap(), "MMFCounter"),
                        8 => new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\MMFFormattedText.png")),
                        9 => new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\MMFSubApplication.png")),
                        _ => new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\MMFActive.png"))
                    };
                    if (objectInfo.Header.Type >= 32)
                    {
                        foreach (Extension posExt in dat.Extensions.Exts.Values)
                            if (posExt.Handle == objectInfo.Header.Type - 32 && File.Exists("Plugins\\ObjectIcons\\" + posExt.Name + ".png"))
                            {
                                iconBmp = new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\" + posExt.Name + ".png"));
                                break;
                            }
                    }
                } catch {}

                var newIconImage = new Image();
                newIconImage.Handle = (uint)IconBank.Images.Count + 20;
                newIconImage.FromBitmap(iconBmp);
                IconBank.Images.Add(newIconImage.Handle, newIconImage);
                objectInfo.IconHandle = newIconImage.Handle;
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
                Frame frm = dat.Frames[i];
                if (!SapDCore.MFA)
                    frm.Handle = dat.FrameHandles.IndexOf((short)i);
                frm.WriteMFA(frameWriter);
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

        public Bitmap MakeIcon(Bitmap source, string @default)
        {
            Bitmap output = new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\" + @default + ".png"));

            if (source.Width > 32 || source.Height > 32)
            {
                if (source.Width > source.Height)
                    output = source.ResizeImage(new Size(32, (int)Math.Round((float)source.Height / source.Width * 32.0)));
                else
                    output = source.ResizeImage(new Size((int)Math.Round((float)source.Width / source.Height * 32.0), 32));
            }
            else
            {
                Rectangle destRect;
                if (source.Width > source.Height)
                {
                    output = new Bitmap(32, source.Height);
                    destRect = new Rectangle(0, 16 - source.Height / 2, source.Width, source.Height);
                }
                else
                {
                    output = new Bitmap(source.Width, 32);
                    destRect = new Rectangle(16 - source.Width / 2, 0, source.Width, source.Height);
                }

                using (Graphics graphics = Graphics.FromImage(output))
                {
                    graphics.DrawImage(source, destRect);
                }
            }

            // Alpha Fix
            bool hasPixels = false;
            {
                var bitmapData = output.LockBits(new Rectangle(0, 0, output.Width, output.Height),
                                                 ImageLockMode.ReadOnly,
                                                 PixelFormat.Format32bppArgb);
                var length = Math.Abs(bitmapData.Stride) * output.Height;
                var bytes = new byte[length];
                Marshal.Copy(bitmapData.Scan0, bytes, 0, length);

                for (int i = 3; i < length; i += 4)
                {
                    bytes[i] = (byte)Math.Ceiling(bytes[i] / 255f);
                    if (!hasPixels && bytes[i] == 1) hasPixels = true;
                    bytes[i] *= 255;
                }

                Marshal.Copy(bytes, 0, bitmapData.Scan0, length);
                output.UnlockBits(bitmapData);
            }

            if (hasPixels && output.Width > 1) return output;
            else return new Bitmap(Bitmap.FromFile("Plugins\\ObjectIcons\\" + @default + ".png"));
        }
    }
}
