using SapphireD.Core.Data;
using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.FileReaders
{
    public interface FileReader
    {
        string Name { get; }

        PackageData getPackageData();
        void LoadGame(ByteReader fileReader, string filePath);
        Dictionary<int, Bitmap> getIcons();
        FileReader Copy();
    }
}
