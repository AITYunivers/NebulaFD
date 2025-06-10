using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object.Data.Movement
{
    internal class MovementItem : IReadable, IWritable
    {
        public string Name = string.Empty;
        public string Extension = string.Empty;
        public uint Handle = 0;

        public void Read(ByteReader reader)
        {
            Name = reader.ReadAutoYuniversal();
            Extension = reader.ReadAutoYuniversal();
            Handle = reader.ReadUInt();
            reader.Skip(reader.ReadInt()); // Movement Data
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteAutoYunicode(Name);
            writer.WriteAutoYunicode(Extension);
            writer.WriteUInt(Handle);
            writer.WriteInt(0); // Movement Data
        }
    }
}
