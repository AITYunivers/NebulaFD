using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Common
{
    internal class CommonValue : IReadable, IWritable
    {
        public string Name = string.Empty;
        public object? Value;

        public void Read(ByteReader reader)
        {
            Name = reader.ReadAutoYuniversal();
            uint type = reader.ReadUInt();
            switch (type)
            {
                case 0: // Integer
                    Value = reader.ReadInt();
                    break;
                case 1: // Double
                    Value = reader.ReadDouble();
                    break;
                case 2: // String
                    Value = reader.ReadAutoYuniversal();
                    break;
            }
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteAutoYunicode(Name);
            if (Value is int intValue)
            {
                writer.WriteUInt(0); // Type = Integer
                writer.WriteInt(intValue);
            }
            else if (Value is double doubleValue)
            {
                writer.WriteUInt(1); // Type = Double
                writer.WriteDouble(doubleValue);
            }
            else if (Value is string stringValue)
            {
                writer.WriteUInt(2); // Type = String
                writer.WriteAutoYunicode(stringValue);
            }
        }
    }
}
