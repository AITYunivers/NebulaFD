using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Layer
{
    internal class LayerItem : IReadable
    {
        public string Name = string.Empty;

        public void Read(ByteReader reader)
        {
            Name = reader.ReadAutoYuniversal();
            uint flags = reader.ReadUInt();
            float xCoefficient = reader.ReadFloat();
            float yCoefficient = reader.ReadFloat();
        }
    }
}
