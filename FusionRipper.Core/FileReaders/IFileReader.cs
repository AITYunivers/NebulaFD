using FusionRipper.Core.Data;
using FusionRipper.Core.Memory;
using System.Drawing;

namespace FusionRipper.Core.FileReaders
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
