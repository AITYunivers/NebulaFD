using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class FormattedTextData : CommonObjectData
    {
        public override void ReadUncommonData(ByteReader reader)
        {
            int width = reader.ReadInt();
            int height = reader.ReadInt();
            uint flags = reader.ReadUInt();
            Color color = reader.ReadColor();
            reader.Skip(reader.ReadInt()); // Data
        }
    }
}
