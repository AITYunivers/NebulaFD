using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Frame.Folder
{
    internal class FolderItem : IReadable
    {
        public string? Name;

        public void Read(ByteReader reader)
        {
            byte header = reader.ReadByte(); // Named wrong idk what it really is
            reader.Skip(1); // Unknown

            if (header == 112) // Flags related maybe?
            {
                reader.Skip(1); // Unknown
                header = reader.ReadByte(); // No way this is right lol
            }
            else reader.Skip(2);

            if (header == 4) // Flags related? Idk if this can happen without 112?
            {
                Name = reader.ReadAutoYuniversal();
                reader.Skip(reader.ReadInt() * 4); // Children
            }
            else
                reader.Skip(4); // Child
        }
    }
}
