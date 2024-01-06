using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterExtension : ParameterChunk
    {
        public short Type;
        public short Code;
        public byte[] Data = new byte[0];

        public ParameterExtension()
        {
            ChunkName = "ParameterExtension";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            short dataSize = reader.ReadShort();
            Type = reader.ReadShort();
            Code = reader.ReadShort();
            Data = reader.ReadBytes(dataSize);
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteShort((short)Data.Length);
            writer.WriteShort(Type);
            writer.WriteShort(Code);
            writer.WriteBytes(Data);
        }
    }
}
