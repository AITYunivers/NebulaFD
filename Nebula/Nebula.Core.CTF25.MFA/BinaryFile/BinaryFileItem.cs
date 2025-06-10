using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.BinaryFile
{
    public class BinaryFileItem : IReadable, IWritable
    {
        public string Name = string.Empty;

        public void Read(ByteReader reader)
        {
            Name = reader.ReadAutoYuniversal();
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteAutoYunicode(Name);
        }
    }
}
