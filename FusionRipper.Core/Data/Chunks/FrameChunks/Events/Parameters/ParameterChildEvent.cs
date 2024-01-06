using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.FrameChunks.Events.Parameters
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

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(ObjectInfos.Length / 2);
            foreach (short oI in ObjectInfos)
                writer.WriteShort(oI);
        }
    }
}
