using Nebula.Core.Data;
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
        public string Name => "APK";
        public Dictionary<int, Bitmap> Icons { get { return _icons; } set { _icons = value; } }
        private Dictionary<int, Bitmap> _icons = new Dictionary<int, Bitmap>();

        Dictionary<string, byte[]> soundFiles = new(); // dictionary for saving each founded sound

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
                        soundFiles[entry.Name] = ms.ToArray(); // saving it to dictionary
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
                LoadAndroidSounds(soundFiles); // put here for fixing issues with sound dumping
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

        private void LoadAndroidSounds(Dictionary<string, byte[]> soundFiles)
        {
            // no sounds - skip
            if (soundFiles.Count == 0 || NebulaCore.PackageData.SoundBank.Sounds.Count == 0)
                return;

            foreach (var soundFile in soundFiles)
            {
                // getting file's name without extension
                string fileName = Path.GetFileNameWithoutExtension(soundFile.Key).ToLower();
                // since Android sound names are s0001, s0002 and etc, we'll try to find its real name in sound bank from its handle in file's name
                if (fileName.StartsWith('s') && fileName.Length >= 5 && uint.TryParse(fileName.Substring(1), out uint fileHandle))
                {
                    if (NebulaCore.PackageData.SoundBank.Sounds.ContainsKey(fileHandle))
                    {
                        var sound = NebulaCore.PackageData.SoundBank.Sounds[fileHandle]; 
                        sound.Data = soundFile.Value;
                        sound.Flags["PlayFromDisk"] = false;
                        continue;
                    }
                }
            }            
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
