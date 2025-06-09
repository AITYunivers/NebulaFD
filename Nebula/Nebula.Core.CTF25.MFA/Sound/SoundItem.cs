using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Sound
{
    public class SoundItem : IReadable
    {
        public string Name = string.Empty;
        public uint Handle;

        public void Read(ByteReader reader)
        {
            Handle = reader.ReadUInt();
            /*
            u32 Checksum    | 0x00
            i32 References  | 0x04
            */  reader.Skip  (0x08);
            int dataSize = reader.ReadInt();
            /*
            u32 Flags       | 0x00
            i32 Frequency   | 0x04
            */  reader.Skip  (0x08);
            int nameLength = reader.ReadInt();
            Name = reader.ReadYunicodeStop(nameLength);
            reader.Skip(dataSize - nameLength * (reader.IsUnicode() ? 2 : 1)); // Sound Data
        }
    }
}
