using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class SubApplicationData : CommonObjectData
    {
        public string FilePath = string.Empty;
        public int Width;
        public int Height;
        public uint Flags;
        public uint? FrameHandle;

        public override void ReadUncommonData(ByteReader reader)
        {
            FilePath = reader.ReadAutoYuniversal();
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            Flags = reader.ReadUInt();
            if ((Flags & 0x4000) != 0)
                FrameHandle = reader.ReadUInt();
            reader.Skip(4); // Unknown
        }

        public override void WriteUncommonData(ByteWriter writer)
        {
            writer.WriteAutoYunicode(FilePath);
            writer.WriteInt(Width);
            writer.WriteInt(Height);
            writer.WriteUInt(Flags);
            if ((Flags & 0x4000) != 0)
                writer.WriteUInt(FrameHandle!.Value);
            writer.WriteInt(-1); // Unknown
        }
    }
}
