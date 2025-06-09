using Nebula.Core.Data;
using Nebula.Core.Data.Image;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Image
{
    public class ImageItem : BaseImage, IReadable
    {
        public void Read(ByteReader reader)
        {
            Handle = reader.ReadUInt();
            /*
            i32 Checksum    | 0x00
            i32 References  | 0x04
            */  reader.Skip  (0x08);
            int dataSize = reader.ReadInt();
            Width = reader.ReadUShort();
            Height = reader.ReadUShort();
            /*
            u8  GraphicMode | 0x00
            u8  Flags       | 0x01
            u16 Padding     | 0x02
            i16 HotspotX    | 0x04
            i16 HotspotY    | 0x06
            i16 ActionX     | 0x08
            i16 ActionY     | 0x0A
            i32 Transparent | 0x0C
            */ reader.Skip   (0x10);
            reader.Skip(dataSize); // Image Data
        }
    }
}
