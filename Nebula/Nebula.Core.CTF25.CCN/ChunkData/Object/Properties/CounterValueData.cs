using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.ChunkData.Object.Properties
{
    internal class CounterValueData : IReadable, IWritable
    {
        public short Size;
        public int Initial;
        public int Minimum;
        public int Maximum;

        public void Read(ByteReader reader)
        {
            Size = reader.ReadShort();
            Initial = reader.ReadInt();
            Minimum = reader.ReadInt();
            Maximum = reader.ReadInt();
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteShort(Size);
            writer.WriteInt(Initial);
            writer.WriteInt(Minimum);
            writer.WriteInt(Maximum);
        }
    }
}
