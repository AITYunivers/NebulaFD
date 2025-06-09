using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Common
{
    internal class CommonValue : IReadable
    {
        public void Read(ByteReader reader)
        {
            string name = reader.ReadAutoYuniversal();
            uint type = reader.ReadUInt();
            switch (type)
            {
                case 0: // Integer
                    int intValue = reader.ReadInt();
                    break;
                case 1: // Double
                    double doubleValue = reader.ReadDouble();
                    break;
                case 2: // String
                    string stringValue = reader.ReadAutoYuniversal();
                    break;
            }
        }
    }
}
