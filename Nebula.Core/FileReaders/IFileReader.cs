using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.FileReaders
{
    public interface IFileReader
    {
        string Name { get; }
        Dictionary<int, Bitmap> Icons { get; set; }
        string FilePath { get; set; }

        PackageData getPackageData();
        void LoadGame(ByteReader fileReader, string filePath);
        IFileReader Copy();
    }
}
