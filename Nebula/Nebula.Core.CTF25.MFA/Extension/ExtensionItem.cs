using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Extension
{
    internal class ExtensionItem : IReadable, IWritable
    {
        public uint Handle = 0;
        public string FileName = string.Empty;
        public string Name = string.Empty;
        public uint MagicNumber = 0;
        public string SubType = string.Empty;
        public bool IsUnicode = false;

        public void Read(ByteReader reader)
        {
            Handle = reader.ReadUInt();
            FileName = reader.ReadAutoYuniversal();
            Name = reader.ReadAutoYuniversal();
            MagicNumber = reader.ReadUInt();
            SubType = reader.ReadAutoYuniversal();
            IsUnicode = reader.ReadBool4();
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteUInt(Handle);
            writer.WriteAutoYunicode(FileName);
            writer.WriteAutoYunicode(Name);
            writer.WriteUInt(MagicNumber);
            writer.WriteAutoYunicode(SubType);
            writer.WriteBool4(IsUnicode);
        }
    }
}
