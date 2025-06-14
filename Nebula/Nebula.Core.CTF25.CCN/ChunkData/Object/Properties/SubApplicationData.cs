using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.ChunkData.Object.Properties
{
    internal class SubApplicationData : IReadable, IWritable
    {
        public int Width;
        public int Height;
        public ushort Version;
        public ushort FrameHandle;
        public uint Flags;
        public string FilePath = string.Empty;

        public void Read(ByteReader reader)
        {
            reader.Skip(4); // Size?
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            Version = reader.ReadUShort();
            FrameHandle = reader.ReadUShort();
            Flags = reader.ReadUInt();
            reader.Skip(8); // Padding
            FilePath = reader.ReadAutoYuniversal();
        }

        public void Write(ByteWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
