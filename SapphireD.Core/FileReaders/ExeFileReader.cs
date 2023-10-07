using SapphireD.Core.Data;
using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;
using System.Diagnostics;
using System.Drawing;
using TsudaKageyu;

namespace SapphireD.Core.FileReaders
{
    public class ExeFileReader : FileReader
    {
        public string Name => "Normal EXE";

        public PackageData Package;
        public Dictionary<int, Bitmap> Icons = new Dictionary<int, Bitmap>();

        public void LoadGame(string gamePath)
        {
            SapDCore.CurrentReader = this;
            loadIcons(gamePath);

            var reader = new ByteReader(gamePath, FileMode.Open);
            readHeader(reader);
            PackData packData = new PackData();
            packData.Read(reader);

            Package = new PackageData();
            Package.Read(reader);
        }

        private void loadIcons(string gamePath)
        {
            var icoExt = new IconExtractor(gamePath);
            var icos = IconUtil.Split(icoExt.GetIcon(0));

            foreach (var icon in icos)
            {
                if (IconUtil.GetBitCount(icon) == 8)
                    Icons.TryAdd(icon.Width + 1, icon.ToBitmap());
                else
                    Icons.TryAdd(icon.Width, icon.ToBitmap());
            }

            // 256c 16x16
            if (!Icons.ContainsKey(17))
                Icons.Add(17, Icons.Last().Value.ResizeImage(new Size(16, 16)));
            // 256c 32x32
            if (!Icons.ContainsKey(33))
                Icons.Add(33, Icons.Last().Value.ResizeImage(new Size(32, 32)));
            // 256c 48x48
            if (!Icons.ContainsKey(49))
                Icons.Add(49, Icons.Last().Value.ResizeImage(new Size(48, 48)));

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

        private int readHeader(ByteReader reader)
        {
            var entryPoint = calculateEntryPoint(reader);
            reader.Seek(entryPoint);

            var firstShort = reader.PeekUInt16();
            if (firstShort != 0x7777)
            {
                SapDCore.Fusion = 1.5f;
                Logger.Log(this, $"1.5 game detected. First short: {firstShort.ToString("X")}");
            }
            return (int)reader.Tell();
        }

        private int calculateEntryPoint(ByteReader exeReader)
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
                    var size = exeReader.ReadUInt32();
                    var address = exeReader.ReadUInt32(); //Pointer to raw data

                    position = (int)(address + size);
                    break;
                }

                exeReader.Seek(entry + 40);
            }

            exeReader.Seek(position);

            return (int)exeReader.Tell();
        }

        public PackageData getPackageData() => Package;

        public Dictionary<int, Bitmap> getIcons() => Icons;

        public FileReader Copy()
        {
            ExeFileReader fileReader = new ExeFileReader();
            fileReader.Package = Package;
            fileReader.Icons = Icons;
            return fileReader;
        }

        public class PackData
        {
            public List<PackFile> Items = new List<PackFile>();
            public uint FormatVersion;

            public void Read(ByteReader reader)
            {
                long start = reader.Tell();
                reader.Skip(8);

                uint headerSize = reader.ReadUInt32();
                Debug.Assert(headerSize == 32);
                uint dataSize = reader.ReadUInt32();

                reader.Seek((int)(start + dataSize - 32));
                var uheader = reader.ReadAscii(4);
                SapDCore.Unicode = uheader != "PAME";
                reader.Seek(start + 16);

                FormatVersion = reader.ReadUInt32();
                reader.Skip(4);
                Debug.Assert(reader.ReadInt32() == 0);

                uint count = reader.ReadUInt32();

                long offset = reader.Tell();
                for (int i = 0; i < count; i++)
                {
                    if (!reader.HasMemory(2)) break;
                    ushort value = reader.ReadUInt16();
                    if (!reader.HasMemory(value)) break;
                    reader.Skip(value * 2);
                    if (!reader.HasMemory(value)) break;
                }

                var newHeader = reader.ReadAscii(4);
                bool hasBingo = newHeader != "PAME" && newHeader != "PAMU";
                reader.Seek(offset);
                for (int i = 0; i < count; i++)
                {
                    var item = new PackFile();
                    item.HasBingo = hasBingo;
                    item.Read(reader);
                    Items.Add(item);
                }
            }

            public class PackFile
            {
                public string PackFilename = "ERROR";
                public byte[] Data;
                public bool HasBingo;
                public bool Compressed;
                public int size;
                public void Read(ByteReader exeReader)
                {
                    ushort len = exeReader.ReadUInt16();
                    PackFilename = exeReader.ReadYuniversal(len);
                    exeReader.Skip(4);
                    size = exeReader.ReadInt32();
                    if (exeReader.PeekInt16() == -9608)
                    {
                        Data = Decompressor.DecompressBlock(exeReader, size);
                        Compressed = true;
                    }
                    else
                        Data = exeReader.ReadBytes(size);

                    Logger.Log(this, $"New packfile: {PackFilename}" + (Compressed ? " (Compressed)" : ""));
                }
            }
        }
    }
}
