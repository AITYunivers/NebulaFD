using SapphireD.Core.Memory;
using System.Reflection.PortableExecutable;

namespace SapphireD.Core.Data.Chunks.BankChunks.Shaders
{
    public class ShaderParameter : Chunk
    {
        public byte Type;
        public string Name = string.Empty;

        public int Value;
        public float FloatValue;

        public ShaderParameter()
        {
            ChunkName = "ShaderParameter";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Name = reader.ReadAutoYuniversal();
            Type = (byte)reader.ReadInt();

            if (Type == 1)
                FloatValue = reader.ReadFloat();
            else
                Value = reader.ReadInt();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteAutoYunicode(Name);
            writer.WriteInt(Type);

            if (Type == 1)
                writer.WriteFloat(FloatValue);
            else
                writer.WriteInt(Value);
        }
    }
}
