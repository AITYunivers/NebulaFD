using Nebula;
using Nebula.Core.Data;
using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.Data.Chunks.BankChunks.Images;
using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.Data.Chunks.MFAChunks;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Color = System.Drawing.Color;
using Image = Nebula.Core.Data.Chunks.BankChunks.Images.Image;
using Size = System.Drawing.Size;

#pragma warning disable CA1416 // Validate platform compatibility
namespace Nebula.Tools.GameDumper
{
    public class MFAParser : INebulaTool
    {
        public string Name => "MFA Parser";

        public ImageBank IconBank = new ImageBank();
        public MFAAppIcon? AppIcons = null;
        public MFAExtraFlags ExtraFlags = new();
        public MFAModulePath ModulePath = new();

        public void Execute()
        {
            PackageData dat = NebulaCore.PackageData;
            string path = "Dumps\\" + Utilities.ClearName(dat.AppName) + "\\";
            Directory.CreateDirectory(path);
            ByteWriter writer = new ByteWriter(new FileStream(path + Utilities.ClearName(Path.GetFileName(string.IsNullOrEmpty(dat.EditorFilename) ? dat.AppName + ".mfa" : dat.EditorFilename)), FileMode.Create));

            IconBank.GraphicMode = dat.ImageBank.GraphicMode = dat.AppHeader.GraphicMode;
            if (dat.Frames.Count > 0)
            {
                IconBank.PaletteVersion = dat.ImageBank.PaletteVersion = dat.Frames.First().FramePalette.PaletteVersion;
                IconBank.PaletteEntries = dat.ImageBank.PaletteEntries = (short)dat.Frames.First().FramePalette.PaletteEntries;
                IconBank.Palette = dat.ImageBank.Palette = dat.Frames.First().FramePalette.Palette;
            }

            this.Log("Converting images to MFA format");
            Stopwatch sw = Stopwatch.StartNew();
            List<Task> imgLoading = new List<Task>();
            foreach (Image img in dat.ImageBank.Images.Values)
                imgLoading.Add(Task.Factory.StartNew(img.PrepareForMfa));
            Task.WaitAll(imgLoading.ToArray());
            GC.Collect();
            this.Log($"Done! ({sw.Elapsed.TotalSeconds} seconds)");

            this.Log("Generating Application, Object, and Frame icons");
            sw.Restart();
            if (NebulaCore.CurrentReader!.Icons.Count == 5)
            {
                AppIcons = new();
                AppIcons.IconHandles = new uint[2];
                AppIcons.IconHandles[0] = 1;
                AppIcons.IconHandles[1] = 0;

                for (int i = 0; i < 5; i++)
                {
                    var newIconImage = new Image();
                    newIconImage.Handle = (uint)i;
                    newIconImage.FromBitmap(NebulaCore.CurrentReader.Icons[(int)Math.Pow(2, 8 - i)]);
                    newIconImage.Flags["Alpha"] = true;
                    newIconImage.Flags["RGBA"] = true;
                    IconBank.Images.Add(newIconImage.Handle, newIconImage);
                }

                for (int i = 0, ii = 0; ii < 6; i = (i + 1) % 3, ii++)
                {
                    var newIconImage = new Image();
                    newIconImage.Handle = (uint)ii + 5;
                    newIconImage.FromBitmap(NebulaCore.CurrentReader.Icons[(int)Math.Pow(2, 6 - i)]);
                    newIconImage.Flags["Alpha"] = true;
                    newIconImage.Flags["RGBA"] = true;
                    IconBank.Images.Add(newIconImage.Handle, newIconImage);
                }
            }
            else
            {
                AppIcons = new();
                AppIcons.IconHandles = new uint[2];
                AppIcons.IconHandles[0] = 1;
                AppIcons.IconHandles[1] = 0;
                Bitmap MMFAppIcon = new Bitmap(Bitmap.FromFile("Tools\\ObjectIcons\\MMFApplication.png"));
                int[] iconSizes = new int[5] { 256, 128, 48, 32, 16};

                for (int i = 0; i < 5; i++)
                {
                    var newIconImage = new Image();
                    newIconImage.Handle = (uint)i;
                    Bitmap resizedIcon = MMFAppIcon.ResizeImage(iconSizes[i]);
                    newIconImage.FromBitmap(resizedIcon);
                    resizedIcon.Dispose();
                    newIconImage.Flags["Alpha"] = true;
                    newIconImage.Flags["RGBA"] = true;
                    IconBank.Images.Add(newIconImage.Handle, newIconImage);
                }

                for (int i = 0, ii = 0; ii < 6; i = (i + 1) % 3, ii++)
                {
                    var newIconImage = new Image();
                    newIconImage.Handle = (uint)ii + 5;
                    Bitmap resizedIcon = MMFAppIcon.ResizeImage(iconSizes[2 + i]);
                    newIconImage.FromBitmap(resizedIcon);
                    resizedIcon.Dispose();
                    newIconImage.Flags["Alpha"] = true;
                    newIconImage.Flags["RGBA"] = true;
                    IconBank.Images.Add(newIconImage.Handle, newIconImage);
                }

                MMFAppIcon.Dispose();
            }

            Dictionary<object, Bitmap> IconBmps = new();
            foreach (ObjectInfo objectInfo in dat.FrameItems.Items.Values)
            {
                object iconBmpo = "Tools\\ObjectIcons\\MMFActive.png";
                try
                {
                    iconBmpo = GenerateObjectIcon(objectInfo);
                }
                catch {}

                var newIconImage = new Image();
                newIconImage.Handle = (uint)IconBank.Images.Count;
                if (iconBmpo is string iconPath)
                {
                    if (!IconBmps.ContainsKey(iconPath))
                        IconBmps.Add(iconPath, new Bitmap(Bitmap.FromFile(iconPath)));
                    newIconImage.FromBitmap(IconBmps[iconPath]);
                }
                else if (iconBmpo is Bitmap iconBmp)
                {
                    if (!IconBmps.ContainsKey(iconBmp))
                        IconBmps.Add(iconBmp, null!);
                    newIconImage.FromBitmap(iconBmp);
                }
                else
                {
                    string activeIconPath = "Tools\\ObjectIcons\\MMFActive.png";
                    if (!IconBmps.ContainsKey(activeIconPath))
                        IconBmps.Add(activeIconPath, new Bitmap(Bitmap.FromFile(activeIconPath)));
                    newIconImage.FromBitmap(IconBmps[activeIconPath]);
                }
                newIconImage.Flags["Alpha"] = true;
                newIconImage.Flags["RGBA"] = true;
                IconBank.Images.Add(newIconImage.Handle, newIconImage);
                objectInfo.IconHandle = newIconImage.Handle;
            }
            GC.Collect();

            foreach (KeyValuePair<object, Bitmap> bmp in IconBmps)
                if (bmp.Key is Bitmap keyBmp)
                    keyBmp.Dispose();
                else if (bmp.Value != null)
                    bmp.Value.Dispose();

            foreach (Frame frm in dat.Frames)
            {
                Image frmImg = new Image();
                frmImg.Handle = (uint)IconBank.Images.Count;
                if (!Parameters.DontIncludeImages)
                {
                    Bitmap frmIcon = MakeFrameIcon(frm);
                    frmImg.FromBitmap(frmIcon);
                    frmIcon.Dispose();
                }
                frmImg.Flags["Alpha"] = true;
                frmImg.Flags["RGBA"] = true;
                IconBank.Images.Add(frmImg.Handle, frmImg);
                frm.MFAFrameInfo.IconHandle = (int)frmImg.Handle;
            }
            GC.Collect();
            IconBank.ImageCount = IconBank.Images.Count;
            this.Log($"Done! ({sw.Elapsed.TotalSeconds} seconds)");

            writer.WriteAscii("MFU2");
            writer.WriteShort(6);
            writer.WriteShort(dat.RuntimeSubversion);
            writer.WriteInt(NebulaCore.Fusion > 1.5f ? dat.ProductVersion : 0);
            writer.WriteInt(NebulaCore.Fusion > 1.5f ? dat.ProductBuild : 280);
            writer.WriteInt(0);
            writer.WriteAutoYunicode(dat.AppName);
            writer.WriteAutoYunicode("");
            writer.WriteAutoYunicode(dat.EditorFilename);
            writer.WriteInt(0);

            this.Log("Writing Font Bank");
            sw.Restart();
            writer.WriteAscii("ATNF"); // Font Bank
            dat.FontBank.WriteMFA(writer);
            this.Log($"Done! ({sw.Elapsed.TotalSeconds} seconds)");
            this.Log("Writing Sound Bank");
            sw.Restart();
            writer.WriteAscii("APMS"); // Sound Bank
            dat.SoundBank.WriteMFA(writer);
            this.Log($"Done! ({sw.Elapsed.TotalSeconds} seconds)");
            this.Log("Writing Music Bank");
            sw.Restart();
            writer.WriteAscii("ASUM"); // Music Bank
            dat.MusicBank.WriteMFA(writer);
            this.Log($"Done! ({sw.Elapsed.TotalSeconds} seconds)");
            this.Log("Writing Icon Bank");
            sw.Restart();
            writer.WriteAscii("AGMI"); // Icon Bank
            IconBank.WriteMFA(writer);
            this.Log($"Done! ({sw.Elapsed.TotalSeconds} seconds)");
            this.Log("Writing Image Bank");
            sw.Restart();
            writer.WriteAscii("AGMI"); // Image Bank
            dat.ImageBank.WriteMFA(writer);
            this.Log($"Done! ({sw.Elapsed.TotalSeconds} seconds)");

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
            writer.WriteInt(dat.ExtendedHeader.BuildType);
            writer.WriteAutoYunicode(dat.TargetFilename);
            writer.WriteAutoYunicode("");
            writer.WriteAutoYunicode("");
            writer.WriteAutoYunicode(dat.About + (string.IsNullOrEmpty(dat.About) ? "" : " | ")  + "Decompiled using Nebula");
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
                if (!NebulaCore.MFA && NebulaCore.Fusion >= 1.0f && frm.Handle == -1)
                    frm.Handle = dat.FrameHandles.IndexOf((short)i);
                this.Log("Writing frame '" + frm.FrameName + "'");
                sw.Restart();
                frm.WriteMFA(frameWriter);
                this.Log($"Done! ({sw.Elapsed.TotalSeconds} seconds)");
            }

            writer.WriteUInt((uint)(offsetEnd + frameWriter.Tell()));
            writer.WriteWriter(frameWriter);

            if (AppIcons != null)
                AppIcons.WriteMFA(writer);
            ExtraFlags.SyncFlags();
            ExtraFlags.WriteMFA(writer);
            if ((ModulePath.ModulePath = dat.ModulesDir) != string.Empty)
                ModulePath.WriteMFA(writer);
            writer.WriteByte(0);

            frameWriter.Flush();
            frameWriter.Close();
            writer.Flush();
            writer.Close();
        }

        public object GenerateObjectIcon(ObjectInfo objectInfo)
        {
            ImageBank imgBnk = NebulaCore.PackageData.ImageBank;
            switch (objectInfo.Header.Type)
            {
                case 0:
                    Image bmp0 = imgBnk.Images[((ObjectQuickBackdrop)objectInfo.Properties).Shape.Image];
                    if (MakeIcon(bmp0.GetBitmap(), out Bitmap bmp00))
                    {
                        bmp0.DisposeBmp();
                        return bmp00;
                    }
                    bmp0?.DisposeBmp();
                    return "Tools\\ObjectIcons\\MMFQuickBackdrop.png";
                case 1:
                    Image bmp1 = imgBnk.Images[((ObjectBackdrop)objectInfo.Properties).Image];
                    if (MakeIcon(bmp1.GetBitmap(), out Bitmap bmp01))
                    {
                        bmp1.DisposeBmp();
                        return bmp01;
                    }
                    bmp1?.DisposeBmp();
                    return "Tools\\ObjectIcons\\MMFBackdrop.png";
                case 2:
                    Image bmp2 = imgBnk.Images[((ObjectCommon)objectInfo.Properties).ObjectAnimations.Animations[0].Directions[0].Frames[0]];
                    if (MakeIcon(bmp2.GetBitmap(), out Bitmap bmp02))
                    {
                        bmp2.DisposeBmp();
                        return bmp02;
                    }
                    bmp2?.DisposeBmp();
                    return "Tools\\ObjectIcons\\MMFActive.png";
                case 3:
                    return "Tools\\ObjectIcons\\MMFString.png";
                case 4:
                    return "Tools\\ObjectIcons\\MMFQ&A.png";
                case 5:
                    return "Tools\\ObjectIcons\\MMFScore.png";
                case 6:
                    return "Tools\\ObjectIcons\\MMFLives.png";
                case 7:
                    Bitmap bmp7 = getCounterBmp(((ObjectCommon)objectInfo.Properties).ObjectCounter, ((ObjectCommon)objectInfo.Properties).ObjectValue);
                    if (MakeIcon(bmp7, out Bitmap bmp07))
                    {
                        bmp7.Dispose();
                        return bmp07;
                    }
                    bmp7?.Dispose();
                    return "Tools\\ObjectIcons\\MMFCounter.png";
                case 8:
                    return "Tools\\ObjectIcons\\MMFFormattedText.png";
                case 9:
                    return "Tools\\ObjectIcons\\MMFSubApplication.png";
                default:
                    foreach (Extension posExt in NebulaCore.PackageData.Extensions.Exts.Values)
                        if (posExt.Handle == objectInfo.Header.Type - 32 && File.Exists("Tools\\ObjectIcons\\" + posExt.Name + ".png"))
                            return "Tools\\ObjectIcons\\" + posExt.Name + ".png";
                    break;
            }
            return "Tools\\ObjectIcons\\MMFActive.png";
        }

        public bool MakeIcon(Bitmap source, out Bitmap output)
        {
            output = null;
            if (source == null) return false;
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

            if (!hasPixels || output.Width <= 1)
                output.Dispose();
            return hasPixels && output.Width > 1;
        }

        public Bitmap MakeFrameIcon(Frame frm)
        {
            Bitmap output = new Bitmap(NebulaCore.PackageData.AppHeader.AppWidth, NebulaCore.PackageData.AppHeader.AppHeight);
            Rectangle destRect;
            using (Graphics graphics = Graphics.FromImage(output))
            {
                graphics.Clear(Color.FromArgb(255, frm.FrameHeader.Background));
                foreach (FrameInstance inst in frm.FrameInstances.Instances)
                {
                    if (!NebulaCore.PackageData.FrameItems.Items.ContainsKey((int)inst.ObjectInfo))
                        continue;
                    try
                    {

                        ObjectInfo oi = NebulaCore.PackageData.FrameItems.Items[(int)inst.ObjectInfo];
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
                                    img = NebulaCore.PackageData.ImageBank.Images[((ObjectQuickBackdrop)oi.Properties).Shape.Image];
                                    destRect = new Rectangle(inst.PositionX, inst.PositionY,
                                                             ((ObjectQuickBackdrop)oi.Properties).Width,
                                                             ((ObjectQuickBackdrop)oi.Properties).Height);
                                    doDraw(graphics, img.GetBitmap(), destRect, alpha);
                                    img.DisposeBmp();
                                }
                                break;
                            case 1: // Backdrop
                                img = NebulaCore.PackageData.ImageBank.Images[((ObjectBackdrop)oi.Properties).Image];
                                destRect = new Rectangle(inst.PositionX, inst.PositionY,
                                                         img.Width, img.Height);
                                doDraw(graphics, img.GetBitmap(), destRect, alpha);
                                img.DisposeBmp();
                                break;
                            case 2: // Active
                                img = NebulaCore.PackageData.ImageBank.Images[((ObjectCommon)oi.Properties).ObjectAnimations.Animations.First().Value.Directions.First().Frames.First()];
                                destRect = new Rectangle(inst.PositionX - img.HotspotX,
                                                         inst.PositionY - img.HotspotY,
                                                         img.Width, img.Height);
                                doDraw(graphics, img.GetBitmap(), destRect, alpha);
                                img.DisposeBmp();
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
                                cntrImg?.Dispose();
                                break;
                        }
                    }
                    catch {}
                }
            }
            Bitmap resizedOutput = output.ResizeImage(64, 48);
            output.Dispose();
            return resizedOutput;
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
                    Image img = NebulaCore.PackageData.ImageBank.Images[cntr.Frames[id]];
                    width += img.Width;
                    height = Math.Max(height, img.Height);
                }
                bmp = new Bitmap(width, height);
                g = Graphics.FromImage(bmp);
                int? prevX = null;
                foreach (char c in val.Initial.ToString().Reverse())
                {
                    uint id = counterID[c];
                    Image img = NebulaCore.PackageData.ImageBank.Images[cntr.Frames[id]];
                    int xToDraw = width - img.Width;
                    if (prevX != null)
                        xToDraw = (int)prevX - img.Width;
                    g.DrawImageUnscaled(img.GetBitmap(), xToDraw, 0);
                    img.DisposeBmp();
                    prevX = xToDraw;
                }
            }
            else if (cntr.DisplayType == 4)
            {
                double ratio = (double)(val.Initial - val.Minimum) / (val.Maximum - val.Minimum);
                Image img = NebulaCore.PackageData.ImageBank.Images[cntr.Frames[(int)((cntr.Frames.Length - 1) * ratio)]];
                bmp = new Bitmap(img.Width, img.Height);
                g = Graphics.FromImage(bmp);
                g.DrawImageUnscaled(img.GetBitmap(), 0, 0);
                img.DisposeBmp();
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
