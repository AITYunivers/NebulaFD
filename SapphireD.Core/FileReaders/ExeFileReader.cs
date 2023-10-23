using Ressy;
using SapphireD.Core.Data;
using SapphireD.Core.Data.PackageReaders;
using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;
using System.Drawing;
using System.Text;
using TsudaKageyu;

namespace SapphireD.Core.FileReaders
{
    public class EXEFileReader : FileReader
    {
        public string Name => "Normal EXE";

        public CCNPackageData Package = new();
        public Dictionary<int, Bitmap> Icons = new Dictionary<int, Bitmap>();

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
                if (IconUtil.GetBitCount(icon) != 8)
                    Icons.TryAdd(icon.Width, icon.ToBitmap());
            }

            // 32-Bit 16x16
            if (!Icons.ContainsKey(16))
                Icons.Add(16, Icons.Last().Value.ResizeImage(new Size(16, 16)));
            // 32-Bit 32x32
            if (!Icons.ContainsKey(32))
                Icons.Add(32, Icons.Last().Value.ResizeImage(new Size(32, 32)));
            // 32-Bit 48x48
            if (!Icons.ContainsKey(48))
                Icons.Add(48, Icons.Last().Value.ResizeImage(new Size(48, 48)));
            // 32-Bit 128x128
            if (!Icons.ContainsKey(128))
                Icons.Add(128, Icons.Last().Value.ResizeImage(new Size(128, 128)));
            // 32-Bit 256x256
            if (!Icons.ContainsKey(256))
                Icons.Add(256, Icons.Last().Value.ResizeImage(new Size(256, 256)));
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
        public Dictionary<int, Bitmap> getIcons() => Icons;

        public FileReader Copy()
        {
            EXEFileReader fileReader = new()
            {
                Package = Package,
                Icons = Icons
            };
            return fileReader;
        }
    }
}
