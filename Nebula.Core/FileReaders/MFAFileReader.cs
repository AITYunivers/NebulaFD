using Nebula.Core.Data;
using Nebula.Core.Data.PackageReaders;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.FileReaders
{
    public class MFAFileReader : IFileReader, IDisposable
    {
        public string Name => "MFA";
        public Dictionary<int, Bitmap> Icons { get; set; } = new();
        public string FilePath { get; set; } = string.Empty;
        public MFAPackageData Package { get; private set; } = new();

        private bool _disposed;

        public void LoadGame(ByteReader fileReader, string filePath)
        {
            ArgumentNullException.ThrowIfNull(fileReader);
            ArgumentException.ThrowIfNullOrEmpty(filePath);
            FilePath = filePath;
            Package.Read(fileReader);
        }

        public PackageData GetPackageData() => Package;

        public IFileReader Copy() => new MFAFileReader
        {
            Package = Package,
            Icons = new Dictionary<int, Bitmap>(Icons),
            FilePath = FilePath
        };

        public void Dispose()
        {
            if (_disposed) return;
            foreach (var bmp in Icons.Values)
                bmp?.Dispose();
            Icons.Clear();
            _disposed = true;
        }
    }
}
