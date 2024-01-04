using SapphireD;
using SapphireD.Core.Data;
using SapphireD.Core.Data.Chunks.AppChunks;
using SapphireD.Core.Data.Chunks.BankChunks.Images;
using SapphireD.Core.Data.Chunks.FrameChunks;
using SapphireD.Core.Data.Chunks.MFAChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Color = System.Drawing.Color;
using Image = SapphireD.Core.Data.Chunks.BankChunks.Images.Image;
using Size = System.Drawing.Size;

#pragma warning disable CA1416 // Validate platform compatibility
namespace GameDumper
{
    public class MFAParser : SapDPlugin
    {
        public string Name => "MFA Parser";

        public ImageBank IconBank = new ImageBank();
        public MFAAppIcon? AppIcons = null;

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

            if (SapDCore.CurrentReader!.Icons.Count == 5)
            {
                AppIcons = new();
                AppIcons.IconHandles = new uint[2];
                AppIcons.IconHandles[0] = 1;
                AppIcons.IconHandles[1] = 0;

                for (int i = 0; i < 5; i++)
                {
                    var newIconImage = new Image();
                    newIconImage.Handle = (uint)i;
                    newIconImage.FromBitmap(SapDCore.CurrentReader.Icons[(int)Math.Pow(2, 8 - i)]);
                    newIconImage.Flags["Alpha"] = true;
                    newIconImage.Flags["RGBA"] = true;
                    IconBank.Images.Add(newIconImage.Handle, newIconImage);
                }

                for (int i = 0, ii = 0; ii < 6; i = (i + 1) % 3, ii++)
                {
                    var newIconImage = new Image();
                    newIconImage.Handle = (uint)ii + 5;
                    newIconImage.FromBitmap(SapDCore.CurrentReader.Icons[(int)Math.Pow(2, 6 - i)]);
                    newIconImage.Flags["Alpha"] = true;
                    newIconImage.Flags["RGBA"] = true;
                    IconBank.Images.Add(newIconImage.Handle, newIconImage);
                }
            }

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
                        7 => MakeIcon(getCounterBmp(((ObjectCommon)objectInfo.Properties).ObjectCounter, ((ObjectCommon)objectInfo.Properties).ObjectValue), "MMFCounter"),
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
                newIconImage.Handle = (uint)IconBank.Images.Count;
                newIconImage.FromBitmap(iconBmp);
                newIconImage.Flags["Alpha"] = true;
                newIconImage.Flags["RGBA"] = true;
                IconBank.Images.Add(newIconImage.Handle, newIconImage);
                objectInfo.IconHandle = newIconImage.Handle;
            }
            foreach (Frame frm in dat.Frames)
            {
                Image frmImg = new Image();
                frmImg.Handle = (uint)IconBank.Images.Count;
                frmImg.FromBitmap(MakeFrameIcon(frm));
                frmImg.Flags["Alpha"] = true;
                frmImg.Flags["RGBA"] = true;
                IconBank.Images.Add(frmImg.Handle, frmImg);
                frm.MFAFrameInfo.IconHandle = (int)frmImg.Handle;
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
            writer.WriteAutoYunicode(dat.About + (string.IsNullOrEmpty(dat.About) ? "" : " | ")  + "Decompiled using SapphireD");
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
            for (int i = 2; i < 11; i++) // Icon Images
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

            if (AppIcons != null)
                AppIcons.WriteMFA(writer);
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
            if (source == null) return output;

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
                output = new Bitmap(32, 32);
                destRect = new Rectangle(16 - source.Width / 2,
                                         16 - source.Height / 2,
                                         source.Width, source.Height);

                using (Graphics graphics = Graphics.FromImage(output))
                {
                    graphics.Clear(Color.Transparent);
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

        public Bitmap MakeFrameIcon(Frame frm)
        {
            Bitmap output = new Bitmap(SapDCore.PackageData.AppHeader.AppWidth, SapDCore.PackageData.AppHeader.AppHeight);
            Rectangle destRect;
            using (Graphics graphics = Graphics.FromImage(output))
            {
                graphics.Clear(Color.FromArgb(255, frm.FrameHeader.Background));
                foreach (FrameInstance inst in frm.FrameInstances.Instances)
                {
                    ObjectInfo oi = SapDCore.PackageData.FrameItems.Items[(int)inst.ObjectInfo];
                    Image? img = null;
                    float alpha = 0f;
                    if (oi.Header.InkEffect != 1)
                        alpha = oi.Header.BlendCoeff / 255.0f;
                    else
                        alpha = oi.Header.InkEffectParam * 2.0f / 255.0f;
                    switch (oi.Header.Type)
                    {
                        case 0: // Quick Backdrop
                            if (((ObjectQuickBackdrop)oi.Properties).Shape.FillType == 0)
                            {
                                img = SapDCore.PackageData.ImageBank.Images[((ObjectQuickBackdrop)oi.Properties).Shape.Image];
                                destRect = new Rectangle(inst.PositionX, inst.PositionY,
                                                         ((ObjectQuickBackdrop)oi.Properties).Width,
                                                         ((ObjectQuickBackdrop)oi.Properties).Height);
                                doDraw(graphics, img.GetBitmap(), destRect, alpha);
                            }
                            break;
                        case 1: // Backdrop
                            img = SapDCore.PackageData.ImageBank.Images[((ObjectBackdrop)oi.Properties).Image];
                            destRect = new Rectangle(inst.PositionX, inst.PositionY,
                                                     img.Width, img.Height);
                            doDraw(graphics, img.GetBitmap(), destRect, alpha);
                            break;
                        case 2: // Active
                            img = SapDCore.PackageData.ImageBank.Images[((ObjectCommon)oi.Properties).ObjectAnimations.Animations.First().Value.Directions.First().Frames.First()];
                            destRect = new Rectangle(inst.PositionX - img.HotspotX,
                                                     inst.PositionY - img.HotspotY,
                                                     img.Width, img.Height);
                            doDraw(graphics, img.GetBitmap(), destRect, alpha);
                            break;
                        case 7: // Counter
                            ObjectCounter cntr = ((ObjectCommon)oi.Properties).ObjectCounter;
                            Bitmap cntrImg = getCounterBmp(cntr, ((ObjectCommon)oi.Properties).ObjectValue);

                            if (cntr.DisplayType == 1)
                            {
                                destRect = new Rectangle(inst.PositionX - cntrImg.Width,
                                                         inst.PositionY - cntrImg.Height,
                                                         cntrImg.Width, cntrImg.Height);
                                doDraw(graphics, cntrImg, destRect, alpha);
                            }
                            else if (cntr.DisplayType == 4)
                            {
                                destRect = new Rectangle(inst.PositionX, inst.PositionY,
                                                         cntrImg.Width, cntrImg.Height);
                                doDraw(graphics, cntrImg, destRect, alpha);
                            }
                            break;
                    }
                }
            }
            if (Directory.Exists("junk"))
                output.Save("junk\\" + frm.FrameName + ".png");
            return output.ResizeImage(64, 48);
        }

        private void doDraw(Graphics g, Bitmap sourceBitmap, Rectangle dest, float alpha)
        {
            using (ImageAttributes imageAttributes = new ImageAttributes())
            {
                ColorMatrix colorMatrix = new ColorMatrix();
                colorMatrix.Matrix33 = 1 - alpha;
                imageAttributes.SetColorMatrix(colorMatrix);
                imageAttributes.SetWrapMode(System.Drawing.Drawing2D.WrapMode.Tile);

                g.DrawImage(
                    sourceBitmap,
                    dest,
                    0, 0, dest.Width, dest.Height,
                    GraphicsUnit.Pixel,
                    imageAttributes
                );
            }
        }

        Dictionary<char, uint> counterID = new Dictionary<char, uint>()
        {
            { '0',  0 },
            { '1',  1 },
            { '2',  2 },
            { '3',  3 },
            { '4',  4 },
            { '5',  5 },
            { '6',  6 },
            { '7',  7 },
            { '8',  8 },
            { '9',  9 },
            { '-', 10 },
            { '+', 11 },
            { '.', 12 },
            { 'e', 13 },
        };

        private Bitmap getCounterBmp(ObjectCounter cntr, ObjectValue val)
        {
            Bitmap bmp = null;
            Graphics g = null;
            if (cntr.DisplayType == 1)
            {
                int width = 0;
                int height = 0;
                foreach (char c in val.Initial.ToString())
                {
                    uint id = counterID[c];
                    Image img = SapDCore.PackageData.ImageBank.Images[cntr.Frames[id]];
                    width += img.Width;
                    height = Math.Max(height, img.Height);
                }
                bmp = new Bitmap(width, height);
                g = Graphics.FromImage(bmp);
                int? prevX = null;
                foreach (char c in val.Initial.ToString().Reverse())
                {
                    uint id = counterID[c];
                    Image img = SapDCore.PackageData.ImageBank.Images[cntr.Frames[id]];
                    int xToDraw = width - img.Width;
                    if (prevX != null)
                        xToDraw = (int)prevX - img.Width;
                    g.DrawImageUnscaled(img.GetBitmap(), xToDraw, 0);
                    prevX = xToDraw;
                }
            }
            else if (cntr.DisplayType == 4)
            {
                double ratio = (double)(val.Initial - val.Minimum) / (val.Maximum - val.Minimum);
                Image img = SapDCore.PackageData.ImageBank.Images[cntr.Frames[(int)(cntr.Frames.Length * ratio)]];
                bmp = new Bitmap(img.Width, img.Height);
                g = Graphics.FromImage(bmp);
                g.DrawImageUnscaled(img.GetBitmap(), 0, 0);
            }

            if (g != null)
                g.Dispose();

            return bmp;
        }

        private Bitmap cropBmp(Bitmap bmp)
        {
            Rectangle bounds = getCropBounds(bmp);
            if (bounds.Width == bmp.Width &&
                bounds.Height == bmp.Height ||
                bounds.Width == 0 ||
                bounds.Height == 0)
                return bmp;
            Bitmap newBmp = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(newBmp))
                g.DrawImage(bmp, new Rectangle(0, 0, newBmp.Width, newBmp.Height), bounds, GraphicsUnit.Pixel);
            return newBmp;
        }

        private Rectangle getCropBounds(Bitmap bitmap)
        {
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int left = int.MaxValue, right = int.MinValue, top = int.MaxValue, bottom = int.MinValue;

            byte[] data = new byte[bmpData.Width * bmpData.Height * 4];
            Marshal.Copy(bmpData.Scan0, data, 0, data.Length);
            int ptr = 0;

            for (int y = 0; y < bmpData.Height; y++)
            {
                for (int x = 0; x < bmpData.Width; x++)
                {
                    if (data[ptr + 3] != 0)
                    {
                        left = Math.Min(left, x);
                        right = Math.Max(right, x);
                        top = Math.Min(top, y);
                        bottom = Math.Max(bottom, y);
                    }
                    ptr += 4;
                }
                ptr += bmpData.Stride - (bmpData.Width * 4);
            }

            bitmap.UnlockBits(bmpData);
            if (left > right || top > bottom)
                return Rectangle.Empty;
            return new Rectangle(left, top, right - left + 1, bottom - top + 1);
        }
    }
}
