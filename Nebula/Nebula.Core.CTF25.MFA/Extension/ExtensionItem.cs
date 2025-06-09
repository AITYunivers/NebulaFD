using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Extension
{
    internal class ExtensionItem : IReadable
    {
        public string Name = string.Empty;
        public uint Handle;

        public void Read(ByteReader reader)
        {
            Handle = reader.ReadUInt();
            reader.SkipAutoYuniversal(); // FileName
            Name = reader.ReadAutoYuniversal();
            reader.Skip(4); // MagicNumber
            reader.SkipAutoYuniversal(); // SubType
            reader.Skip(4); // IsUnicode
        }
    }
}
