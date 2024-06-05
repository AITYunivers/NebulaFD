using Nebula.Core.Data;
using Nebula.Core.Data.PackageReaders;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.FileReaders
{
    public class MFAFileReader : IFileReader
    {
        public string Name => "MFA";
        public Dictionary<int, Bitmap> Icons { get { return _icons; } set { _icons = value; } }
        private Dictionary<int, Bitmap> _icons = new Dictionary<int, Bitmap>();

        public string FilePath { get { return _filePath; } set { _filePath = value; } }
        public string _filePath = string.Empty;

        public MFAPackageData Package = new();

        public void LoadGame(ByteReader fileReader, string filePath)
        {
            _filePath = filePath;
            Package.Read(fileReader);
        }

        public PackageData getPackageData() => Package!;

        public IFileReader Copy()
        {
            MFAFileReader fileReader = new()
            {
                Package = Package,
                Icons = _icons
            };
            return fileReader;
        }
    }
}
