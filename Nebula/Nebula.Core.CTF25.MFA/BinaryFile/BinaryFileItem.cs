using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.BinaryFile
{
    public class BinaryFileItem : IReadable
    {
        public string Name = string.Empty;

        public void Read(ByteReader reader)
        {
            Name = reader.ReadAutoYuniversal();
        }
    }
}
