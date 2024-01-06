using Ressy;
using FusionRipper.Core.Data;
using FusionRipper.Core.Data.PackageReaders;
using FusionRipper.Core.Memory;
using FusionRipper.Core.Utilities;
using System.Drawing;
using System.Text;
using TsudaKageyu;

namespace FusionRipper.Core.FileReaders
{
    public class EXEFileReader : IFileReader
    {
        public string Name => "Normal EXE";
        public Dictionary<int, Bitmap> Icons { get { return _icons; } set { _icons = value; } }
        private Dictionary<int, Bitmap> _icons = new Dictionary<int, Bitmap>();

        public CCNPackageData Package = new();

        public void LoadGame(ByteReader fileReader, string filePath)
        {
            loadIcons(filePath);
            calculateEntryPoint(fileReader);

            if (!fileReader.HasMemory(1)) // Check for Unpacked
            {
                var portableExecutable = new PortableExecutable(filePath);
                foreach (var identifier in portableExecutable.GetResourceIdentifiers())
                    if (identifier.Type.Code == 6 && identifier.Name.Code == 11)
                    {
                        Package.ModulesDir = Encoding.Unicode.GetString(portableExecutable.GetResource(identifier).Data).Trim('\b', '\0');
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
                Logger.Log(this, "Invalid executable signature", 2, ConsoleColor.Red);

            exeReader.Seek(60);
            var hdrOffset = exeReader.ReadUInt16();
            exeReader.Seek(hdrOffset + 6);
            var numOfSections = exeReader.ReadUInt16();
            exeReader.Skip(240);

            var position = 0;
            for (var i = 0; i < numOfSections; i++)
            {
                var entry = exeReader.Tell();
                var sectionName = exeReader.ReadAscii();

                if (sectionName == ".extra")
                {
                    exeReader.Seek(entry + 20);
                    position = (int)exeReader.ReadUInt32(); //Pointer to raw data
                    break;
                }

                if (i >= numOfSections - 1)
                {
                    exeReader.Seek(entry + 16);
                    var size = exeReader.ReadInt();
                    var address = exeReader.ReadInt(); //Pointer to raw data

                    position = (int)(address + size);
                    break;
                }

                exeReader.Seek(entry + 40);
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
