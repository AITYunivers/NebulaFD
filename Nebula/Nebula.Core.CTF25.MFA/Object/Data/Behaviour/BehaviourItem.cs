using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object.Data.Behaviour
{
    internal class BehaviourItem : IReadable, IWritable
    {
        public string Name = string.Empty;

        public void Read(ByteReader reader)
        {
            Name = reader.ReadAutoYuniversal();
            reader.Skip(reader.ReadInt()); // Data
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteAutoYunicode(Name);
            writer.WriteInt(0); // Data
        }
    }
}
