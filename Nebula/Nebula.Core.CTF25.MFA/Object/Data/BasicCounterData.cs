using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class BasicCounterData : CommonObjectData
    {
        public override void ReadUncommonData(ByteReader reader)
        {
            int player = reader.ReadInt();
            reader.Skip(reader.ReadInt() * 4); // Image Handles
            bool useText = reader.ReadBool4();
            Color color = reader.ReadColor();
            uint fontHandle = reader.ReadUInt();

            int width = reader.ReadInt();
            int height = reader.ReadInt();
        }
    }
}
