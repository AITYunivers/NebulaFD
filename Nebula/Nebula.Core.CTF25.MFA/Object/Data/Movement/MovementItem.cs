using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Object.Data.Movement
{
    internal class MovementItem : IReadable
    {
        public string Name = string.Empty;

        public void Read(ByteReader reader)
        {
            Name = reader.ReadAutoYuniversal();
            string extension = reader.ReadAutoYuniversal();
            int id = reader.ReadInt();
            reader.Skip(reader.ReadInt()); // Movement Data
        }
    }
}
