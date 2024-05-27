using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterChildEvent : ParameterChunk
    {
        public ushort[] ObjectInfos = new ushort[0];

        public ParameterChildEvent()
        {
            ChunkName = "ParameterChildEvent";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            ObjectInfos = new ushort[reader.ReadInt() * 2];
            for (int i = 0; i < ObjectInfos.Length; i++)
                ObjectInfos[i] = reader.ReadUShort();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(ObjectInfos.Length / 2);
            foreach (ushort oI in ObjectInfos)
            {
                if (FrameEvents.QualifierJumptable.ContainsKey(oI))
                    writer.WriteUShort(FrameEvents.QualifierJumptable[oI]);
                else
                    writer.WriteUShort(oI);
            }
        }

        public override string ToString()
        {
            return "Child Events";
        }
    }
}
