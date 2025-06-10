using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Font
{
    public class FontItem : IReadable, IWritable
    {
        public uint Handle;
        public uint Checksum;
        public int References;
        public int Size;
        public int Height;
        public int Width;
        public int Escapement;
        public int Orientation;
        public int Weight;
        public bool Italic;
        public bool Underline;
        public bool StrikeOut;
        public byte CharSet;
        public byte OutPrecision;
        public byte ClipPrecision;
        public byte Quality;
        public byte PitchAndFamily;
        public string Name = string.Empty;

        public void Read(ByteReader reader)
        {
            Handle = reader.ReadUInt();
            Checksum = reader.ReadUInt();
            References = reader.ReadInt();
            Size = reader.ReadInt();
            Height = reader.ReadInt();
            Width = reader.ReadInt();
            Escapement = reader.ReadInt();
            Orientation = reader.ReadInt();
            Weight = reader.ReadInt();
            Italic = reader.ReadBool();
            Underline = reader.ReadBool();
            StrikeOut = reader.ReadBool();
            CharSet = reader.ReadByte();
            OutPrecision = reader.ReadByte();
            ClipPrecision = reader.ReadByte();
            Quality = reader.ReadByte();
            PitchAndFamily = reader.ReadByte();
            Name = reader.ReadYuniversal(32);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteUInt(Handle);
            writer.WriteUInt(Checksum);
            writer.WriteInt(References);
            writer.WriteInt(Size);
            writer.WriteInt(Height);
            writer.WriteInt(Width);
            writer.WriteInt(Escapement);
            writer.WriteInt(Orientation);
            writer.WriteInt(Weight);
            writer.WriteBool(Italic);
            writer.WriteBool(Underline);
            writer.WriteBool(StrikeOut);
            writer.WriteByte(CharSet);
            writer.WriteByte(OutPrecision);
            writer.WriteByte(ClipPrecision);
            writer.WriteByte(Quality);
            writer.WriteByte(PitchAndFamily);
            writer.WriteYunicode(Name, 32);
        }
    }
}
