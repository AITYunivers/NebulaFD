using Nebula.Core.Data;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using System.Drawing;
using Image = Nebula.Core.Data.Chunks.BankChunks.Images.Image;

namespace Nebula.Tools.ZelTranslator_SD.GDevelop
{
    public class GDBMFWriter
    {
        static int a = 0;
        public static string CounterToBMF(PackageData gameData, ObjectCommon objCommon)
        {
            string writer = string.Empty;

            int bmpWidth = 0;
            int bmpHeight = 0;
            foreach (uint imgId in objCommon.ObjectCounter.Frames)
            {
                Image img = gameData.ImageBank.Images[imgId];
                bmpWidth += img.Width;
                bmpHeight = Math.Max(bmpHeight, img.Height);
            }

            Bitmap bmp = new Bitmap(bmpWidth, bmpHeight);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Background
                g.FillRectangle(Brushes.OrangeRed, new Rectangle(0, 0, bmp.Width, bmp.Height));
                bmp.MakeTransparent(Color.OrangeRed);
                int bmpPosition = 0;
                for (int imgId = 0; imgId < objCommon.ObjectCounter.Frames.Length; imgId++)
                {
                    // Source Image
                    Bitmap img = gameData.ImageBank.Images[objCommon.ObjectCounter.Frames[imgId]].GetBitmap();
                    Rectangle rect = new Rectangle(bmpPosition, 0, img.Width, img.Height);
                    g.DrawImage(img, rect);
                    bmpPosition += img.Width;
                }
            }
            bmp.Save($"outputimg{a}.png");
            a++;
            return writer;
        }
    }
}
