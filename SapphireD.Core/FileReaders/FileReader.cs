using SapphireD.Core.Data;
using System.Drawing;

namespace SapphireD.Core.FileReaders
{
    public interface FileReader
    {
        string Name { get; }

        PackageData getPackageData();
        void LoadGame(string gamePath);
        Dictionary<int, Bitmap> getIcons();
        FileReader Copy();
    }
}
