using SapphireD.Core.Data;
using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.FileReaders
{
    public interface IFileReader
    {
        string Name { get; }
        Dictionary<int, Bitmap> Icons { get; set; }

        PackageData getPackageData();
        void LoadGame(ByteReader fileReader, string filePath);
        IFileReader Copy();
    }
}
