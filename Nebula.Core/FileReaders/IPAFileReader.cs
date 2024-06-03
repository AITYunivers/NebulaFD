using Nebula.Core.Data;
using Nebula.Core.Data.PackageReaders;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Compression;

namespace Nebula.Core.FileReaders
{
    public class IPAFileReader : IFileReader
    {
        public string Name => "IPA";
        public Dictionary<int, Bitmap> Icons { get { return _icons; } set { _icons = value; } }
        private Dictionary<int, Bitmap> _icons = new Dictionary<int, Bitmap>();

        public string FilePath { get { return _filePath; } set { _filePath = value; } }
        public string _filePath = string.Empty;

        public CCNPackageData Package = new();

        public void LoadGame(ByteReader fileReader, string filePath)
        {
            ByteReader? ccnReader = null;
            ZipArchive archive = ZipFile.OpenRead(_filePath = filePath);
            string appName = string.Empty;
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (Path.GetExtension(entry.Name) == ".cci")
                {
                    File.Delete("open.ccj");
                    entry.ExtractToFile("open.ccj");
                    ccnReader = new ByteReader(File.ReadAllBytes("open.ccj"));
                    File.Delete("open.ccj");
                    appName = Directory.GetParent(entry.FullName)?.Name.Replace(".app", "");
                }
                else if (entry.Name.StartsWith("AppIcon60x60@") && _icons.Count == 0)
                {
                    loadIcons(new Bitmap(Bitmap.FromStream(entry.Open())));
                }
            }
            archive.Dispose();

            if (ccnReader != null)
            {
                Package.Read(ccnReader);
                Package.AppName = appName;
            }
        }

        private void loadIcons(Bitmap bmp)
        {
            var imageAttr = new ImageAttributes();
            imageAttr.SetColorMatrix(new ColorMatrix(
                                         new[]
                                             {
                                                 new[] {0.0F, 0.0F, 1.0F, 0.0F, 0.0F},
                                                 new[] {0.0F, 1.0F, 0.0F, 0.0F, 0.0F},
                                                 new[] {1.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                                                 new[] {0.0F, 0.0F, 0.0F, 1.0F, 0.0F},
                                                 new[] {0.0F, 0.0F, 0.0F, 0.0F, 1.0F}
                                             }
                                         ));
            var temp = new Bitmap(bmp.Width, bmp.Height);
            GraphicsUnit pixel = GraphicsUnit.Pixel;
            using (Graphics g = Graphics.FromImage(temp))
            {
                g.DrawImage(bmp, Rectangle.Round(bmp.GetBounds(ref pixel)), 0, 0, bmp.Width, bmp.Height,
                            GraphicsUnit.Pixel, imageAttr);
            }
            bmp = temp;

            _icons.Add(16,  bmp.ResizeImage(new Size(16, 16)));
            _icons.Add(32,  bmp.ResizeImage(new Size(32, 32)));
            _icons.Add(64,  bmp.ResizeImage(new Size(48, 48)));
            _icons.Add(128, bmp.ResizeImage(new Size(128, 128)));
            _icons.Add(256, bmp.ResizeImage(new Size(256, 256)));
        }

        public PackageData getPackageData() => Package!;

        public IFileReader Copy()
        {
            CCNFileReader fileReader = new()
            {
                Package = Package,
                Icons = _icons
            };
            return fileReader;
        }
    }
}
