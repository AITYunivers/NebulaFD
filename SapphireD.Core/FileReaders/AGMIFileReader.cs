using SapphireD.Core.Data;
using SapphireD.Core.Data.PackageReaders;
using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.FileReaders
{
    public class AGMIFileReader : FileReader
    {
        public string Name => "AGMI";

        public AGMIPackageData? Package;
        public Dictionary<int, Bitmap> Icons = new Dictionary<int, Bitmap>();

        public void LoadGame(ByteReader fileReader, string filePath)
        {
            Package = new AGMIPackageData();
            Package.AppName = Path.GetFileNameWithoutExtension(filePath);
            Package.Read(fileReader);
        }

        public PackageData getPackageData() => Package!;
        public Dictionary<int, Bitmap> getIcons() => Icons;

        public FileReader Copy()
        {
            AGMIFileReader fileReader = new()
            {
                Package = Package,
                Icons = Icons
            };
            return fileReader;
        }
    }
}
