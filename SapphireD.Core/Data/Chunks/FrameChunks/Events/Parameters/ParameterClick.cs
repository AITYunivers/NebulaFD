using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterClick : ParameterChunk
    {
        public byte Button;
        public byte IsDouble;

        public ParameterClick()
        {
            ChunkName = "ParameterClick";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Button = reader.ReadByte();
            IsDouble = reader.ReadByte();
        }
    }
}
