using K4os.Compression.LZ4;
using SapphireD.Core.Memory;
using System;

namespace SapphireD.Core.Data.Chunks.BankChunks.Images
{
    public class Image25Plus : Image
    {
        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUInt() - 1;
            Checksum = reader.ReadInt();
            References = reader.ReadInt();
            reader.Skip(4);
            var dataSize = reader.ReadInt();
            Width = reader.ReadShort();
            Height = reader.ReadShort();
            GraphicMode = reader.ReadByte();
            Flags.Value = reader.ReadByte();
            reader.Skip(2);
            HotspotX = reader.ReadShort();
            HotspotY = reader.ReadShort();
            ActionPointX = reader.ReadShort();
            ActionPointY = reader.ReadShort();
            TransparentColor = reader.ReadColor();
            var decompSizePlus = reader.ReadInt();
            var rawImg = reader.ReadBytes(Math.Max(0, dataSize - 4));
            var task = new Task(() =>
            {
                var target = new byte[decompSizePlus];
                LZ4Codec.Decode(rawImg, target);
                ImageData = target;
                ImageBank.LoadedImageCount++;
            });

            task.RunSynchronously();
            ImageBank.TaskManager.Add(task);
        }
    }
}
