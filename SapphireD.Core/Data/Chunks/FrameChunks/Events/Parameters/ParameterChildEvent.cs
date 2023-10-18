using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterChildEvent : ParameterChunk
    {
        public short[] ObjectInfos = new short[0];

        public ParameterChildEvent()
        {
            ChunkName = "ParameterChildEvent";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            ObjectInfos = new short[reader.ReadInt() * 2];
            for (int i = 0; i < ObjectInfos.Length; i++)
                ObjectInfos[i] = reader.ReadShort();
        }
    }
}
