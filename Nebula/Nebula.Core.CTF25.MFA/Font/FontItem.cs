using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Font
{
    public class FontItem : IReadable
    {
        public string Name = string.Empty;
        public uint Handle;

        public void Read(ByteReader reader)
        {
            Handle = reader.ReadUInt();
            /*
            i32 Checksum        | 0x00
            i32 References      | 0x04
            i32 Size            | 0x08
            i32 Height          | 0x0C
            i32 Width           | 0x10
            i32 Escapement      | 0x14
            i32 Orientation     | 0x18
            i32 Weight          | 0x1C
            u8  Italic          | 0x20
            u8  Underline       | 0x21
            u8  StrikeOut       | 0x22
            u8  CharSet         | 0x23
            u8  OutPrecision    | 0x24
            u8  ClipPrecision   | 0x25
            u8  Quality         | 0x26
            u8  PitchAndFamily  | 0x27
            */  reader.Skip      (0x28);
            Name = reader.ReadYuniversal(32);
        }
    }
}
