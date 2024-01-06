using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterZone : ParameterChunk
    {
        public short X1;
        public short Y1;
        public short X2;
        public short Y2;

        public ParameterZone()
        {
            ChunkName = "ParameterZone";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            X1 = reader.ReadShort();
            Y1 = reader.ReadShort();
            X2 = reader.ReadShort();
            Y2 = reader.ReadShort();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteShort(X1);
            writer.WriteShort(Y1);
            writer.WriteShort(X2);
            writer.WriteShort(Y2);
        }
    }
}
