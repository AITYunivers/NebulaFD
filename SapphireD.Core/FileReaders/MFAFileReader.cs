using SapphireD.Core.Data;
using SapphireD.Core.Data.PackageReaders;
using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.FileReaders
{
    public class MFAFileReader : FileReader
    {
        public string Name => "MFA";

        public MFAPackageData Package = new();
        public Dictionary<int, Bitmap> Icons = new Dictionary<int, Bitmap>();

        public void LoadGame(ByteReader fileReader, string filePath)
        {
            Package.Read(fileReader);
        }

        public PackageData getPackageData() => Package!;
        public Dictionary<int, Bitmap> getIcons() => Icons;

        public FileReader Copy()
        {
            MFAFileReader fileReader = new()
            {
                Package = Package,
                Icons = Icons
            };
            return fileReader;
        }
    }
}
