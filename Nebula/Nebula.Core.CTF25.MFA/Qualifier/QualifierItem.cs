using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Qualifier
{
    public class QualifierItem : IReadable
    {
        public string Name = string.Empty;
        public uint Handle;

        public void Read(ByteReader reader)
        {
            Name = reader.ReadAutoYuniversal();
            Handle = reader.ReadUInt();
        }
    }
}
