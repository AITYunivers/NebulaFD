using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.CTF25.CCN.ChunkData.Object.Properties
{
    internal class FormattedTextData : IReadable, IWritable
    {
        public uint Flags;
        public Color Color;
        public int Width;
        public int Height;
        public byte[] Data = [];

        public void Read(ByteReader reader)
        {
            reader.Skip(4); // Size
            reader.Skip(4); // Unknown
            Flags = reader.ReadUInt();
            Color = reader.ReadColor();
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            reader.Skip(4); // Unknown
            Data = reader.ReadBytes(reader.ReadInt());
        }

        public void Write(ByteWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
