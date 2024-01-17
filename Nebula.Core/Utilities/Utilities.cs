using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Memory;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Nebula.Core.Utilities
{
    public static class Utilities
    {
        public static string ClearName(string ogName)
        {
            var str = string.Join("", ogName.Split(Path.GetInvalidFileNameChars()));
            str = str.Replace("?", "");
            return str;
        }
        public static string ReadYuniversal(this ByteReader reader, int len=-1)
        {
            if (NebulaCore.Unicode) return reader.ReadWideString(len);
            else return reader.ReadAscii(len);
        }
        public static byte[] GetBuffer(this ByteWriter writer)
        {
            var buf = ((MemoryStream)writer.BaseStream).GetBuffer();
            Array.Resize(ref buf, (int)writer.Size());
            return buf;
        }
        public static Bitmap ResizeImage(this Bitmap imgToResize, int size) => ResizeImage(imgToResize, size, size);
        public static Bitmap ResizeImage(this Bitmap imgToResize, int width, int height) => ResizeImage(imgToResize, new Size(width, height));
        public static Bitmap ResizeImage(this Bitmap imgToResize, Size size)
        {
            var destRect = new Rectangle(0, 0, size.Width, size.Height);
            var destImage = new Bitmap(size.Width, size.Height);

            destImage.SetResolution(imgToResize.Width, imgToResize.Height);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.GammaCorrected;
                graphics.InterpolationMode = InterpolationMode.Bilinear;
                graphics.SmoothingMode = SmoothingMode.Default;
                graphics.PixelOffsetMode = PixelOffsetMode.None;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(imgToResize, destRect, 0, 0, imgToResize.Width, imgToResize.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
                return text;

            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
        public static string ReplaceLast(this string text, string search, string replace)
        {
            int pos = text.LastIndexOf(search);
            if (pos < 0)
                return text;

            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
        public static string GetHex(this byte[] data, int count = -1, int position = 0)
        {
            var actualCount = count;
            if (actualCount == -1) actualCount = data.Length;
            string temp = "";
            for (int i = 0; i < actualCount; i++)
            {
                temp += data[i].ToString("X2");
                temp += " ";
            }
            return temp;
        }
        public static T[] To1DArray<T>(T[,] input)
        {
            // Step 1: get total size of 2D array, and allocate 1D array.
            int size = input.Length;
            T[] result = new T[size];

            // Step 2: copy 2D array elements into a 1D array.
            int write = 0;
            for (int i = 0; i <= input.GetUpperBound(0); i++)
            {
                for (int z = 0; z <= input.GetUpperBound(1); z++)
                {
                    result[write++] = input[i, z];
                }
            }

            // Step 3: return the new array.
            return result;
        }

        public static IEnumerable<Type> TypesImplementingInterface(Type interfaceType, params Type[] desiredConstructorSignature)
        {
            if (interfaceType == null) throw new ArgumentNullException("interfaceType");
            if (!interfaceType.IsInterface) throw new ArgumentOutOfRangeException("interfaceType");

            return AppDomain
                   .CurrentDomain
                   .GetAssemblies()
                   .SelectMany(a => a.GetTypes())
                   .Where(t => t.IsAssignableFrom(interfaceType))
                   .Where(t => !t.IsInterface)
                   .Where(t => t.GetConstructor(desiredConstructorSignature) != null);
        }

        public static T ConstructInstance<T>(Type t, params object[] parameterList)
        {
            Type[] signature = parameterList.Select(p => p.GetType()).ToArray();
            ConstructorInfo constructor = t.GetConstructor(signature);
            return (T)constructor.Invoke(parameterList);
        }

        public static string ToHex(this Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

        public static string Trim(this string s, params char[] chars)
        {
            foreach (char c in chars)
                s.Trim(c);
            return s;
        }

        public static Bitmap MakeFrameImg(Frame frm, int layer = -1, bool showHiddenObjects = false, bool useFrameSize = true)
        {
            Bitmap output;
            if (useFrameSize || frm.BitmapCache == null && layer == -1)
                output = new Bitmap(frm.FrameHeader.Width, frm.FrameHeader.Height);
            else
                output = new Bitmap(NebulaCore.PackageData.AppHeader.AppWidth, NebulaCore.PackageData.AppHeader.AppHeight);
            Rectangle destRect;
            using (Graphics graphics = Graphics.FromImage(output))
            {
                if (frm.BitmapCache != null && layer == -1)
                    graphics.DrawImageUnscaled(frm.BitmapCache, 0, 0);
                else
                {
                    graphics.Clear(Color.FromArgb(layer < 1 ? 255 : 0, frm.FrameHeader.Background));
                    foreach (FrameInstance inst in frm.FrameInstances.Instances)
                    {
                        if ((layer != -1 && layer != inst.Layer) || !NebulaCore.PackageData.FrameItems.Items.ContainsKey((int)inst.ObjectInfo))
                            continue;
                        ObjectInfo oi = NebulaCore.PackageData.FrameItems.Items[(int)inst.ObjectInfo];
                        Data.Chunks.BankChunks.Images.Image? img = null;
                        float alpha = 0f;
                        if (oi.Header.InkEffect != 1)
                            alpha = oi.Header.BlendCoeff / 255.0f;
                        else
                            alpha = oi.Header.InkEffectParam * 2.0f / 255.0f;
                        switch (oi.Header.Type)
                        {
                            case 0: // Quick Backdrop
                                if (((ObjectQuickBackdrop)oi.Properties).Shape.FillType == 3)
                                {
                                    img = NebulaCore.PackageData.ImageBank.Images[((ObjectQuickBackdrop)oi.Properties).Shape.Image];
                                    destRect = new Rectangle(inst.PositionX, inst.PositionY,
                                                             ((ObjectQuickBackdrop)oi.Properties).Width,
                                                             ((ObjectQuickBackdrop)oi.Properties).Height);
                                    doDraw(graphics, img.GetBitmap(), destRect, alpha);
                                }
                                break;
                            case 1: // Backdrop
                                img = NebulaCore.PackageData.ImageBank.Images[((ObjectBackdrop)oi.Properties).Image];
                                destRect = new Rectangle(inst.PositionX, inst.PositionY,
                                                         img.Width, img.Height);
                                doDraw(graphics, img.GetBitmap(), destRect, alpha);
                                break;
                            default:
                                ObjectCommon oc = ((ObjectCommon)oi.Properties);
                                if (!showHiddenObjects && (!oc.NewObjectFlags["VisibleAtStart"] || oc.ObjectFlags["DontCreateAtStart"] || inst.InstanceFlags["CreateOnly"]))
                                    continue;
                                switch (oi.Header.Type)
                                {
                                    case 2: // Active
                                        img = NebulaCore.PackageData.ImageBank.Images[oc.ObjectAnimations.GetFirst().Directions.First().Frames.First()];
                                        destRect = new Rectangle(inst.PositionX - img.HotspotX,
                                                                 inst.PositionY - img.HotspotY,
                                                                 img.Width, img.Height);
                                        doDraw(graphics, img.GetBitmap(), destRect, alpha);
                                        break;
                                    case 7: // Counter
                                        ObjectCounter cntr = oc.ObjectCounter;
                                        Bitmap cntrImg = GetCounterBmp(cntr, oc.ObjectValue);

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
                                break;
                        }
                    }
                }
            }
            if (frm.BitmapCache == null && layer == -1)
            {
                frm.BitmapCache = output;
                if (!useFrameSize)
                    return MakeFrameImg(frm, layer, showHiddenObjects, useFrameSize);
            }
            return output;
        }

        private static void doDraw(Graphics g, Bitmap sourceBitmap, Rectangle dest, float alpha)
        {
            using (ImageAttributes imageAttributes = new ImageAttributes())
            {
                ColorMatrix colorMatrix = new ColorMatrix();
                colorMatrix.Matrix33 = 1 - alpha;
                imageAttributes.SetColorMatrix(colorMatrix);
                imageAttributes.SetWrapMode(System.Drawing.Drawing2D.WrapMode.Tile);

                if (sourceBitmap.Width != dest.Width || sourceBitmap.Height != dest.Height)
                {
                    Bitmap bTD = new Bitmap(dest.Width, dest.Height);
                    using (Graphics gTD = Graphics.FromImage(bTD))
                    {
                        gTD.DrawImage(
                            sourceBitmap,
                            new Rectangle(0, 0, dest.Width, dest.Height),
                            0, 0, dest.Width, dest.Height,
                            GraphicsUnit.Pixel,
                            imageAttributes
                        );
                    }
                    g.DrawImageUnscaled(bTD, dest.X, dest.Y);
                }
                else
                {
                    g.DrawImage(
                        sourceBitmap,
                        dest,
                        0, 0, dest.Width, dest.Height,
                        GraphicsUnit.Pixel,
                        imageAttributes
                    );
                }
            }
        }

        static Dictionary<char, uint> counterID = new Dictionary<char, uint>()
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

        public static Bitmap GetCounterBmp(ObjectCounter cntr, ObjectValue val)
        {
            Bitmap bmp = null;
            Graphics g = null;
            if (cntr.DisplayType == 1)
            {
                int width = 0;
                int height = 0;
                string value = val.Initial.ToString();

                if (cntr.IntDigitPadding)
                    for (int i = 0; i < cntr.IntDigitCount - val.Initial.ToString().Length; i++)
                        value = '0' + value;

                foreach (char c in value)
                {
                    uint id = counterID[c];
                    Data.Chunks.BankChunks.Images.Image img = NebulaCore.PackageData.ImageBank.Images[cntr.Frames[id]];
                    width += img.Width;
                    height = Math.Max(height, img.Height);
                }
                bmp = new Bitmap(width, height);
                g = Graphics.FromImage(bmp);
                int? prevX = null;
                foreach (char c in value.Reverse())
                {
                    uint id = counterID[c];
                    Data.Chunks.BankChunks.Images.Image img = NebulaCore.PackageData.ImageBank.Images[cntr.Frames[id]];
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
                Data.Chunks.BankChunks.Images.Image img = NebulaCore.PackageData.ImageBank.Images[cntr.Frames[(int)((cntr.Frames.Length - 1) * ratio)]];
                bmp = new Bitmap(img.Width, img.Height);
                g = Graphics.FromImage(bmp);
                g.DrawImageUnscaled(img.GetBitmap(), 0, 0);
            }

            if (g != null)
                g.Dispose();

            return bmp;
        }
    }
}

