using Ressy;
using Nebula.Core.Data;
using Nebula.Core.Data.PackageReaders;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Drawing;
using System.Text;
using TsudaKageyu;

namespace Nebula.Core.FileReaders
{
    public class EXEFileReader : IFileReader
    {
        public string Name => "Normal EXE";
        public Dictionary<int, Bitmap> Icons { get { return _icons; } set { _icons = value; } }
        private Dictionary<int, Bitmap> _icons = new Dictionary<int, Bitmap>();

        public string FilePath { get { return _filePath; } set { _filePath = value; } }
        public string _filePath = string.Empty;

        public CCNPackageData Package = new();

        public void LoadGame(ByteReader fileReader, string filePath)
        {
            loadIcons(_filePath = filePath);
            calculateEntryPoint(fileReader);

            if (!fileReader.HasMemory(1)) // Check for Unpacked
            {
                PortableExecutable portableExecutable = new PortableExecutable(filePath);
                IReadOnlyList<ResourceIdentifier> resourceIdentifiers = portableExecutable.GetResourceIdentifiers();
                foreach (ResourceIdentifier identifier in resourceIdentifiers)
                    if (identifier.Type.Code == 6 && identifier.Name.Code == 11)
                    {
                        Package.ModulesDir = Utilities.Utilities.ClearName(Encoding.Unicode.GetString(portableExecutable.GetResource(identifier).Data), '\\');
                        break;
                    }
                fileReader = new ByteReader(Path.ChangeExtension(filePath, "dat"), FileMode.Open);
            }

            Package.PackData.Read(fileReader);
            Package.Read(fileReader);
        }

        private void loadIcons(string gamePath)
        {
            var icoExt = new IconExtractor(gamePath);
            var icos = IconUtil.Split(icoExt.GetIcon(0));

            foreach (var icon in icos)
            {
                if (IconUtil.GetBitCount(icon) > 8 || icon.Width > 48)
                    _icons.TryAdd(icon.Width == 48 ? 64 : icon.Width, icon.ToBitmap());
            }

            if (_icons.Count == 0)
                foreach (var icon in icos)
                    _icons.TryAdd(icon.Width == 48 ? 64 : icon.Width, icon.ToBitmap());

            // 32-Bit 16x16
            if (!_icons.ContainsKey(16))
                _icons.Add(16, _icons[getLargestIcon()].ResizeImage(new Size(16, 16)));
            // 32-Bit 32x32
            if (!_icons.ContainsKey(32))
                _icons.Add(32, _icons[getLargestIcon()].ResizeImage(new Size(32, 32)));
            // 32-Bit 48x48 (Written as 64 for math reasons)
            if (!_icons.ContainsKey(64))
                _icons.Add(64, _icons[getLargestIcon()].ResizeImage(new Size(48, 48)));
            // 32-Bit 128x128
            if (!_icons.ContainsKey(128))
                _icons.Add(128, _icons[getLargestIcon()].ResizeImage(new Size(128, 128)));
            // 32-Bit 256x256
            if (!_icons.ContainsKey(256))
                _icons.Add(256, _icons[getLargestIcon()].ResizeImage(new Size(256, 256)));
        }

        private int getLargestIcon()
        {
            if (_icons.ContainsKey(256))
                return 256;
            else if (_icons.ContainsKey(128))
                return 128;
            else if (_icons.ContainsKey(64))
                return 64;
            else if (_icons.ContainsKey(32))
                return 32;
            else
                return 16;
        }

        private void calculateEntryPoint(ByteReader exeReader)
        {
            var sig = exeReader.ReadAscii(2);
            if (sig != "MZ")
                this.Log("Invalid executable signature", ConsoleColor.Red);
            exeReader.Seek(60);
            var hdrOffset = exeReader.ReadUShort();
            exeReader.Seek(hdrOffset + 6);
            var numOfSections = exeReader.ReadUShort();
            exeReader.Skip(240);

            uint position = 0;
            for (var i = 0; i < numOfSections; i++)
            {
                if (position == 0)
                {
                    exeReader.Skip(16);
                    position += exeReader.ReadUInt();
                    position += exeReader.ReadUInt();
                    exeReader.Skip(16);
                }
                else
                {
                    exeReader.Skip(16);
                    position += exeReader.ReadUInt();
                    exeReader.Skip(20);
                }
            }

            exeReader.Seek(position);
        }

        public PackageData getPackageData() => Package!;

        public IFileReader Copy()
        {
            EXEFileReader fileReader = new()
            {
                Package = Package,
                Icons = _icons
            };
            return fileReader;
        }
    }
}
