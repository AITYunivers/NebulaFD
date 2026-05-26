using Nebula.Core.Data;
using Nebula.Core.Data.Chunks.BankChunks.Sounds;
using Nebula.Core.Data.Chunks.BankChunks.TrueTypeFonts;
using Nebula.Core.Data.PackageReaders;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Drawing;
using System.IO.Compression;

namespace Nebula.Core.FileReaders
{
    public class APKFileReader : IFileReader
    {
        public string Name => "APK/XAPK";
        public Dictionary<int, Bitmap> Icons { get { return _icons; } set { _icons = value; } }
        private Dictionary<int, Bitmap> _icons = new Dictionary<int, Bitmap>();

        public string FilePath { get { return _filePath; } set { _filePath = value; } }
        public string _filePath = string.Empty;

        public CCNPackageData Package = new();

        public bool Unpacked;

        public void LoadGame(ByteReader fileReader, string filePath)
        {
            ByteReader? ccnReader = null;

            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Read);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (Directory.GetParent(entry.FullName)?.Name == "assets" || Directory.GetParent(entry.FullName)?.Name == "raw")
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
                        using MemoryStream ms = new();
                        entry.Open().CopyTo(ms);
                        SoundBank.ExternalFiles[Path.GetFileNameWithoutExtension(entry.Name)] = ms.ToArray(); // saving it to dictionary
                    }
                    else if (Path.GetExtension(entry.Name) == ".mp4") // for unpacking video files saved by Video Android extension
                    {
                        using var stream = entry.Open();
                        using var memoryStream = new MemoryStream();
                        stream.CopyTo(memoryStream);
                        byte[] fileData = memoryStream.ToArray();

                        // add videos to binary files list for dumping
                        BinaryFile binFile = new BinaryFile
                        {
                            FileName = entry.Name,
                            FileData = fileData
                        };
                        NebulaCore.PackageData.BinaryFiles.Items.Add(binFile);
                    }
                }
                if (Directory.GetParent(entry.FullName)?.Name == "fonts" && Path.GetExtension(entry.Name) == ".ttf")
                {
                    loadFonts(entry); 
                }

                if ((Directory.GetParent(entry.FullName)?.Name == "mipmap-xxhdpi-v4" || Directory.GetParent(entry.FullName)?.Name == "mipmap-xxxhdpi-v4") &&
                    entry.Name == "ic_launcher.png") // for newer versions
                {
                    loadIcons(new Bitmap(Bitmap.FromStream(entry.Open())));
                }
                else if ((Directory.GetParent(entry.FullName)?.Name == "drawable-xhdpi" || Directory.GetParent(entry.FullName)?.Name == "drawable-xxhdpi-v4" ||
                    Directory.GetParent(entry.FullName)?.Name == "drawable-xxxhdpi-v4") && entry.Name == "launcher.png") // for older versions
                {
                    loadIcons(new Bitmap(Bitmap.FromStream(entry.Open())));
                }
            }

            if (ccnReader != null)
            {
                NebulaCore.Android = true; // flag is needed for fixing some issues
                Package.Read(ccnReader);
            }
        }

        private void loadFonts(ZipArchiveEntry entry)
        {
            using var stream = entry.Open(); // getting path for retrieving fonts
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            string fontName = Path.GetFileNameWithoutExtension(entry.Name); // found one - getting its name
            byte[] fontData = memoryStream.ToArray(); // getting its data
            TrueTypeFont ttf = new TrueTypeFont
            {
                Name = fontName,
                FontData = fontData
            };
            NebulaCore.PackageData.TrueTypeFontBank.Fonts.Add(ttf); // then add font to the bank
        }


        private void loadIcons(Bitmap bmp)
        {
            if (_icons.Count > 0) 
                return;

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
