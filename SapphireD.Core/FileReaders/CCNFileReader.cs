using SapphireD.Core.Data;
using SapphireD.Core.Data.PackageReaders;
using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.FileReaders
{
    public class CCNFileReader : FileReader
    {
        public string Name => Unpacked ? "Unpacked EXE" : "CCN";

        public CCNPackageData Package = new();
        public Dictionary<int, Bitmap> Icons = new Dictionary<int, Bitmap>();

        public bool Unpacked;

        public void LoadGame(ByteReader fileReader, string filePath)
        {
            if (Unpacked)
                Package.PackData.Read(fileReader);

            Package.Read(fileReader);
        }

        public void CheckUnpacked(ByteReader fileReader)
        {
            Unpacked = fileReader.ReadAscii(4) == "wwww";
            fileReader.Seek(0);
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
