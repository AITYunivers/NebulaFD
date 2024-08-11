using Nebula.Core.Data;
using Nebula.Core.Data.PackageReaders;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.FileReaders
{
    public class CCNFileReader : IFileReader
    {
        public string Name => NebulaCore.Unpacked ? "Unpacked EXE" : "CCN";
        public Dictionary<int, Bitmap> Icons { get { return _icons; } set { _icons = value; } }
        private Dictionary<int, Bitmap> _icons = new Dictionary<int, Bitmap>();

        public string FilePath { get { return _filePath; } set { _filePath = value; } }
        public string _filePath = string.Empty;

        public CCNPackageData Package = new();

        public void LoadGame(ByteReader fileReader, string filePath)
        {
            _filePath = filePath;
            if (NebulaCore.Unpacked)
                Package.PackData.Read(fileReader);

            Package.Read(fileReader);
        }

        public void CheckUnpacked(ByteReader fileReader)
        {
            NebulaCore.Unpacked = fileReader.ReadAscii(4) == "wwww";
            fileReader.Seek(0);
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
