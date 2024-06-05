using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.BankChunks.Images
{
    public class Image15 : Image
    {
        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUInt();

            List<int> offsets = NebulaCore.PackageData.ImageOffsets.SortedOffsets;
            int startingOffset = (int)reader.Tell();
            int compressedSize = 3;
            if (offsets.IndexOf(startingOffset) != offsets.Count - 1)
                compressedSize = offsets[offsets.IndexOf(startingOffset) + 1] - startingOffset - 4;
            int decompressedSize = reader.ReadInt();
            var compressedBuffer = reader.ReadBytes(compressedSize - 4);

            Task task = new Task(() =>
            {
                ByteReader decompressedReader = new ByteReader(Decompressor.DecompressOPFBlock(compressedBuffer));
                Checksum = decompressedReader.ReadShort();
                References = decompressedReader.ReadInt();
                var dataSize = decompressedReader.ReadInt();
                Width = decompressedReader.ReadShort();
                Height = decompressedReader.ReadShort();
                GraphicMode = decompressedReader.ReadByte();
                Flags.Value = decompressedReader.ReadByte();
                HotspotX = decompressedReader.ReadShort();
                HotspotY = decompressedReader.ReadShort();
                ActionPointX = decompressedReader.ReadShort();
                ActionPointY = decompressedReader.ReadShort();
                ImageData = decompressedReader.ReadBytes(dataSize);

                ImageBank.LoadedImageCount++;
            });

            task.Start();
            ImageBank.TaskManager.Add(task);
        }
    }
}
