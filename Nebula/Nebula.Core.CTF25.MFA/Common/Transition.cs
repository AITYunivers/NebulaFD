using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.CTF25.MFA.Common
{
    internal class Transition : IReadable, IWritable
    {
        public string FileName = string.Empty;
        public string ModuleName = string.Empty;
        public uint ModuleHandle = 0;
        public string Identifier = string.Empty;
        public int Duration = 0;
        public bool UseColor = false;
        public Color Color = Color.White;
        public byte[] ParameterData = [];

        public void Read(ByteReader reader)
        {
            FileName = reader.ReadAutoYuniversal();
            ModuleName = reader.ReadAutoYuniversal();
            ModuleHandle = reader.ReadUInt();
            Identifier = reader.ReadAscii(4);
            Duration = reader.ReadInt();
            UseColor = reader.ReadBool4();
            Color = reader.ReadColor();
            ParameterData = reader.ReadBytes(reader.ReadInt());
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteAutoYunicode(FileName);
            writer.WriteAutoYunicode(ModuleName);
            writer.WriteUInt(ModuleHandle);
            writer.WriteAscii(Identifier);
            writer.WriteInt(Duration);
            writer.WriteBool4(UseColor);
            writer.WriteColor(Color);
            writer.WriteInt(ParameterData.Length);
            writer.WriteBytes(ParameterData);
        }
    }
}
