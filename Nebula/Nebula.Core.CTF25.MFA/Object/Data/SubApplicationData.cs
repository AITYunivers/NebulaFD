using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class SubApplicationData : CommonObjectData
    {
        public override void ReadUncommonData(ByteReader reader)
        {
            string filePath = reader.ReadAutoYuniversal();
            int width = reader.ReadInt();
            int height = reader.ReadInt();
            uint flags = reader.ReadUInt();
            if ((flags & 0x4000) != 0)
            {
                uint frameHandle = reader.ReadUInt();
            }
            reader.Skip(4); // Unknown
        }
    }
}
