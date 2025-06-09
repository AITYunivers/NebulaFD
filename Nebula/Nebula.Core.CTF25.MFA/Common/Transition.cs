using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.CTF25.MFA.Common
{
    internal class Transition : IReadable
    {
        public void Read(ByteReader reader)
        {
            string fileName = reader.ReadAutoYuniversal();
            string moduleName = reader.ReadAutoYuniversal();
            int module = reader.ReadInt();
            string identifier = reader.ReadAscii(4);
            int duration = reader.ReadInt();
            bool useColor = reader.ReadBool4();
            Color color = reader.ReadColor();
            reader.Skip(reader.ReadInt()); // Parameter Data
        }
    }
}
