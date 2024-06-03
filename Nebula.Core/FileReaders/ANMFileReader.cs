using Nebula.Core.Data;
using Nebula.Core.Data.PackageReaders;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.FileReaders
{
    public class ANMFileReader : IFileReader
    {
        public string Name => "ANM";
        public Dictionary<int, Bitmap> Icons { get { return _icons; } set { _icons = value; } }
        private Dictionary<int, Bitmap> _icons = new Dictionary<int, Bitmap>();

        public string FilePath { get { return _filePath; } set { _filePath = value; } }
        public string _filePath = string.Empty;

        public ANMPackageData Package = new();

        public void LoadGame(ByteReader fileReader, string filePath)
        {
            Package.AppName = Path.GetFileNameWithoutExtension(_filePath = filePath);
            Package.Read(fileReader);
        }

        public PackageData getPackageData() => Package!;

        public IFileReader Copy()
        {
            ANMFileReader fileReader = new()
            {
                Package = Package,
                Icons = _icons
            };
            return fileReader;
        }
    }
}
