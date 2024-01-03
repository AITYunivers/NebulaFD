using SapphireD.Core.Data;
using SapphireD.Core.Data.PackageReaders;
using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.FileReaders
{
    public class AGMIFileReader : IFileReader
    {
        public string Name => "AGMI";
        public Dictionary<int, Bitmap> Icons { get { return _icons; } set { _icons = value; } }
        private Dictionary<int, Bitmap> _icons = new Dictionary<int, Bitmap>();

        public AGMIPackageData Package = new();

        public void LoadGame(ByteReader fileReader, string filePath)
        {
            Package.AppName = Path.GetFileNameWithoutExtension(filePath);
            Package.Read(fileReader);
        }

        public PackageData getPackageData() => Package!;

        public IFileReader Copy()
        {
            AGMIFileReader fileReader = new()
            {
                Package = Package,
                Icons = _icons
            };
            return fileReader;
        }
    }
}
