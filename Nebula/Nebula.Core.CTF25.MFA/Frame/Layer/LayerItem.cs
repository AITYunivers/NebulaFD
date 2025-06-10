using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Layer
{
    internal class LayerItem : IReadable, IWritable
    {
        public string Name = string.Empty;
        public uint Flags;
        public float XCoefficient;
        public float YCoefficient;

        public void Read(ByteReader reader)
        {
            Name = reader.ReadAutoYuniversal();
            Flags = reader.ReadUInt();
            XCoefficient = reader.ReadFloat();
            YCoefficient = reader.ReadFloat();
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteAutoYunicode(Name);
            writer.WriteUInt(Flags);
            writer.WriteFloat(XCoefficient);
            writer.WriteFloat(YCoefficient);
        }
    }
}
