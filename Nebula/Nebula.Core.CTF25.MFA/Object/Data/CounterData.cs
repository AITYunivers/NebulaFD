using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.CTF25.MFA.Object.Data
{
    internal class CounterData : CommonObjectData
    {
        public override void ReadUncommonData(ByteReader reader)
        {
            int defaultValue = reader.ReadInt();
            int minValue = reader.ReadInt();
            int maxValue = reader.ReadInt();
            uint displayType = reader.ReadUInt();
            uint fillType = reader.ReadUInt();
            Color color1 = reader.ReadColor();
            Color color2 = reader.ReadColor();
            bool verticalGradient = reader.ReadBool4();
            uint barDirection = reader.ReadUInt();

            int width = reader.ReadInt();
            int height = reader.ReadInt();

            reader.Skip(reader.ReadInt() * 4); // Image Handles
            uint fontHandle = reader.ReadUInt();
        }
    }
}
