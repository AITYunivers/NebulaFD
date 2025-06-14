using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.CTF25.CCN.ChunkData.Common
{
    internal class CommonTransitionData : IReadable, IWritable
    {
        public uint ModuleHandle;
        public string Identifier = string.Empty;
        public int Duration;
        public bool UseColor;
        public Color Color = Color.Black;

        public string FileName = string.Empty;
        public byte[] ParameterData = [];

        public void Read(ByteReader reader)
        {
            ModuleHandle = reader.ReadUInt(); // Is this right?
            Identifier = reader.ReadAscii(4);
            Duration = reader.ReadInt();
            UseColor = reader.ReadInt() != 0;
            Color = reader.ReadColor();

            int NameOffset = reader.ReadInt();
            int DataOffset = reader.ReadInt();
            int DataSize = reader.ReadInt();

            reader.Seek(NameOffset);
            FileName = reader.ReadYuniversal();

            reader.Seek(DataOffset);
            ParameterData = reader.ReadBytes(DataSize);
        }

        public void Write(ByteWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
