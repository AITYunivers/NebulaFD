using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Qualifier
{
    public class QualifierItem : IReadable, IWritable
    {
        public string Name = string.Empty;
        public uint Handle;

        public void Read(ByteReader reader)
        {
            Name = reader.ReadAutoYuniversal();
            Handle = reader.ReadUInt();
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteAutoYunicode(Name);
            writer.WriteUInt(Handle);
        }
    }
}
