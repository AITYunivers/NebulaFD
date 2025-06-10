using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Frame.Instance
{
    internal class InstanceItem : IReadable
    {
        public void Read(ByteReader reader)
        {
            int x = reader.ReadInt();
            int y = reader.ReadInt();
            uint layerHandler = reader.ReadUInt();
            uint handle = reader.ReadUInt();
            ushort flags = reader.ReadUShort();
            short instanceValue = reader.ReadShort();
            uint parentType = reader.ReadUInt(); // Is this right?
            uint objectHandle = reader.ReadUInt(); 
            uint parentHandle = reader.ReadUInt(); // Is this right?
        }
    }
}
