using Nebula.Core.Data;
using Nebula.Core.Data.Image;
using Nebula.Core.Memory;
using System.Buffers;
using System.Drawing;

namespace Nebula.Core.CTF25.MFA.Image
{
    public class ImageItem : BaseImage, IReadable
    {
        private long? _offset;
        private int _dataSize;

        public void Read(ByteReader reader)
        {
            Handle = reader.ReadUInt();
            /*
            i32 Checksum    | 0x00
            i32 References  | 0x04
            */  reader.Skip  (0x08);
            _dataSize = reader.ReadInt();
            Width = reader.ReadUShort();
            Height = reader.ReadUShort();
            Type = (EImageType)reader.ReadByte();
            Flags = (EImageFlags)reader.ReadByte();
            /*
            u16 Padding     | 0x00
            i16 HotspotX    | 0x02
            i16 HotspotY    | 0x04
            i16 ActionX     | 0x06
            i16 ActionY     | 0x08
            i32 Transparent | 0x0A
            */ reader.Skip   (0x0E);
            _offset = reader.Tell();
            reader.Skip(_dataSize); // Image Data
        }

        public override byte[] GetImageData(ByteReader reader, out int dataSize)
        {
            if (_offset == null)
            {
                dataSize = 0;
                return [];
            }

            long pos = reader.Tell();
            reader.Seek(_offset.Value);
            byte[] output;

            if ((Flags & EImageFlags.LZX) != 0)
            {
                dataSize = reader.ReadInt(); // Decompressed Size
                output = ArrayPool<byte>.Shared.Rent(dataSize);
                Decompressor.DecompressZlib(reader, output, dataSize);
            }
            else
            {
                output = ArrayPool<byte>.Shared.Rent(dataSize = _dataSize);
                reader.BaseStream.ReadExactly(output, 0, _dataSize);
            }

            reader.Seek(pos);
            return output;
        }
    }
}
