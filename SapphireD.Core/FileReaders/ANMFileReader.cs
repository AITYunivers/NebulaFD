using SapphireD.Core.Data;
using SapphireD.Core.Data.PackageReaders;
using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.FileReaders
{
    public class ANMFileReader : FileReader
    {
        public string Name => "ANM";

        public ANMPackageData? Package;
        public Dictionary<int, Bitmap> Icons = new Dictionary<int, Bitmap>();

        public void LoadGame(ByteReader fileReader, string filePath)
        {
            Package = new ANMPackageData();
            Package.AppName = Path.GetFileNameWithoutExtension(filePath);
            Package.Read(fileReader);
        }

        public PackageData getPackageData() => Package!;
        public Dictionary<int, Bitmap> getIcons() => Icons;

        public FileReader Copy()
        {
            ANMFileReader fileReader = new()
            {
                Package = Package,
                Icons = Icons
            };
            return fileReader;
        }
    }
}
