using Nebula.Core.Data;
using Nebula.Core.Data.PackageReaders;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Drawing;

namespace Nebula.Core.FileReaders
{
    public class KNPFileReader : IFileReader
    {
        public string Name => "KNP";
        public Dictionary<int, Bitmap> Icons { get { return _icons; } set { _icons = value; } }
        private Dictionary<int, Bitmap> _icons = new Dictionary<int, Bitmap>();

        public string FilePath { get { return _filePath; } set { _filePath = value; } }
        public string _filePath = string.Empty;

        public KNPPackageData Package = new();

        public bool Unpacked;

        public void LoadGame(ByteReader fileReader, string filePath)
        {
            Package.FilePath = filePath.ReplaceLast(Path.GetExtension(_filePath = filePath), "");
            Package.Read(null!);
        }

        public PackageData getPackageData() => Package!;

        public IFileReader Copy()
        {
            KNPFileReader fileReader = new()
            {
                Package = Package,
                Icons = _icons
            };
            return fileReader;
        }
    }
}
