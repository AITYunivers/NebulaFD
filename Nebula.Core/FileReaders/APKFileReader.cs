using Nebula.Core.Data;
using Nebula.Core.Data.PackageReaders;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Drawing;
using System.IO.Compression;

namespace Nebula.Core.FileReaders
{
    public class APKFileReader : IFileReader
    {
        public string Name => "APK";
        public Dictionary<int, Bitmap> Icons { get { return _icons; } set { _icons = value; } }
        private Dictionary<int, Bitmap> _icons = new Dictionary<int, Bitmap>();

        public CCNPackageData Package = new();

        public void Preload(string filePath) { }

        public bool Unpacked;

        public void LoadGame(ByteReader fileReader, string filePath)
        {
            ByteReader? ccnReader = null;
            ZipArchive archive = ZipFile.OpenRead(filePath);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (Directory.GetParent(entry.FullName)?.Name == "raw")
                {
                    if (Path.GetExtension(entry.Name) == ".ccn")
                    {
                        File.Delete("open.ccj");
                        entry.ExtractToFile("open.ccj");
                        ccnReader = new ByteReader(File.ReadAllBytes("open.ccj"));
                        File.Delete("open.ccj");
                    }
                    else if (Path.GetExtension(entry.Name) == ".mp3" ||
                             Path.GetExtension(entry.Name) == ".ogg" ||
                             Path.GetExtension(entry.Name) == ".wav")
                    {
                        // TODO
                    }
                }
                else if ((Directory.GetParent(entry.FullName)?.Name == "drawable-xhdpi" ||
                         Directory.GetParent(entry.FullName)?.Name == "drawable-xxxhdpi-v4") &&
                         entry.Name == "launcher.png")
                {
                    loadIcons(new Bitmap(Bitmap.FromStream(entry.Open())));
                }
            }
            archive.Dispose();

            if (ccnReader != null)
                Package.Read(ccnReader);
        }

        private void loadIcons(Bitmap bmp)
        {
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
