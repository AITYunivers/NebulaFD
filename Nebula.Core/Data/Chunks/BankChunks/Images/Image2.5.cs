using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.BankChunks.Images
{
    public class Image25 : Image
    {
        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUInt();
            if (NebulaCore.Build >= 284)
                Handle--;

            var decompressedSize = reader.ReadInt();
            var compSize = reader.ReadInt();
            var compressedBuffer = reader.ReadBytes(compSize);
            Task task = new Task(() =>
            {
                ByteReader decompressedReader = new ByteReader(Decompressor.DecompressBlock(compressedBuffer));
                Checksum = decompressedReader.ReadInt();
                References = decompressedReader.ReadInt();
                var dataSize = decompressedReader.ReadInt();
                Width = decompressedReader.ReadShort();
                Height = decompressedReader.ReadShort();
                GraphicMode = decompressedReader.ReadByte();
                Flags.Value = decompressedReader.ReadByte();
                decompressedReader.ReadShort();
                HotspotX = decompressedReader.ReadShort();
                HotspotY = decompressedReader.ReadShort();
                ActionPointX = decompressedReader.ReadShort();
                ActionPointY = decompressedReader.ReadShort();
                TransparentColor = decompressedReader.ReadColor();
                if (Flags["LZX"])
                {
                    decompressedSize = decompressedReader.ReadInt();
                    ImageData = Decompressor.DecompressBlock(decompressedReader, (int)(decompressedReader.Size() - decompressedReader.Tell()));
                }
                else ImageData = decompressedReader.ReadBytes(dataSize);
                decompressedReader.Dispose();

                ImageBank.LoadedImageCount++;
            });

            task.Start();
            ImageBank.TaskManager.Add(task);
        }
    }
}
