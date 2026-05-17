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
        public string FilePath { get; set; } = string.Empty;
        public Dictionary<int, Bitmap> Icons { get; set; } = new();
        public CCNPackageData Package { get; set; } = new();

        public void LoadGame(ByteReader fileReader, string filePath)
        {
            fileReader.Close();
            LoadIcons(FilePath = filePath);
            fileReader = new ByteReader(filePath, FileMode.Open);

            CalculateEntryPoint(fileReader);

            if (!fileReader.HasMemory(1))
            {
                try
                {
                    PortableExecutable portableExecutable = new PortableExecutable(filePath);
                    foreach (ResourceIdentifier identifier in portableExecutable.GetResourceIdentifiers())
                        if (identifier.Type.Code == 6 && identifier.Name.Code == 11)
                        {
                            Package.ModulesDir = Utilities.Utilities.ClearName(Encoding.Unicode.GetString(portableExecutable.GetResource(identifier).Data), '\\');
                            break;
                        }
                }
                catch (Exception ex)
                {
                    this.Log($"Failed to read PE resources: {ex.Message}", ConsoleColor.Yellow);
                }
                fileReader = new ByteReader(Path.ChangeExtension(filePath, "dat"), FileMode.Open);
                NebulaCore.Unpacked = true;
            }

            Package.PackData.Read(fileReader);
            Package.Read(fileReader);
        }

        public bool CheckInstaller(ByteReader fileReader)
        {
            CalculateEntryPoint(fileReader);
            string header = fileReader.HasMemory(4) ? fileReader.ReadAscii(4) : string.Empty;
            fileReader.Seek(0);
            return header == "wwgT";
        }

        public bool CheckChowdren(ByteReader fileReader)
        {
            fileReader.Seek(60);
            var hdrOffset = fileReader.ReadUShort();
            fileReader.Seek(hdrOffset + 6);
            var numOfSections = fileReader.ReadUShort();
            fileReader.Skip(240);

            bool isChowdren = false;
            for (var i = 0; i < numOfSections; i++)
            {
                string header = fileReader.ReadAsciiStop(16);
                fileReader.Skip(24);

                if (header == ".gfids")
                {
                    isChowdren = true;
                    break;
                }
            }

            fileReader.Seek(0);
            return isChowdren && File.Exists(Path.Combine(Path.GetDirectoryName(NebulaCore.FilePath)!, "Assets.dat"));
        }

        private void LoadIcons(string gamePath)
        {
            var icoExt = new IconExtractor(gamePath);
            var icos = IconUtil.Split(icoExt.GetIcon(0));

            foreach (var icon in icos)
                if (IconUtil.GetBitCount(icon) > 8 || icon.Width > 48)
                    Icons.TryAdd(icon.Width == 48 ? 64 : icon.Width, icon.ToBitmap());

            if (Icons.Count == 0)
                foreach (var icon in icos)
                    Icons.TryAdd(icon.Width == 48 ? 64 : icon.Width, icon.ToBitmap());

            foreach (var (key, size) in new[] { (16, 16), (32, 32), (64, 48), (128, 128), (256, 256) })
                if (!Icons.ContainsKey(key))
                    Icons.Add(key, Icons[GetLargestIcon()].ResizeImage(new Size(size, size)));
        }

        private int GetLargestIcon() =>
            Icons.Keys.Where(k => new[] { 256, 128, 64, 32, 16 }.Contains(k))
                      .OrderByDescending(k => k)
                      .First();

        private void CalculateEntryPoint(ByteReader exeReader)
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
            uint relocFallback = 0;
            for (var i = 0; i < numOfSections; i++)
            {
                string sectionName = exeReader.ReadAsciiStop(8);
                exeReader.Skip(8);
                uint sectionStart = exeReader.ReadUInt();
                uint sectionSize = exeReader.ReadUInt();
                exeReader.Skip(16);

                position = position == 0 ? sectionStart + sectionSize : position + sectionStart;

                if (sectionName == ".reloc")
                    relocFallback = sectionStart + sectionSize;
            }

            exeReader.Seek(position);
            if (!exeReader.HasMemory(1) && relocFallback != position && relocFallback != 0)
                exeReader.Seek(relocFallback);
        }

        public PackageData GetPackageData() => Package;

        public IFileReader Copy() => new EXEFileReader
        {
            Package = Package,
            Icons = Icons
        };
    }
}
