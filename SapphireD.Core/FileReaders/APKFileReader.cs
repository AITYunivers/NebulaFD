using SapphireD.Core.Data;
using SapphireD.Core.Data.PackageReaders;
using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;
using System.Drawing;
using System.IO.Compression;

namespace SapphireD.Core.FileReaders
{
    public class APKFileReader : FileReader
    {
        public string Name => "APK";

        public CCNPackageData Package = new();
        public Dictionary<int, Bitmap> Icons = new();

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
            }
            archive.Dispose();

            if (ccnReader != null)
                Package.Read(ccnReader);
        }

        public PackageData getPackageData() => Package!;
        public Dictionary<int, Bitmap> getIcons() => Icons;

        public FileReader Copy()
        {
            CCNFileReader fileReader = new()
            {
                Package = Package,
                Icons = Icons
            };
            return fileReader;
        }
    }
}
