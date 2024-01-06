using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.FrameChunks.Events
{
    public class Qualifier : Chunk
    {
        public ushort ObjectInfo;
        public short Type;

        public Qualifier()
        {
            ChunkName = "Qualifier";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            ObjectInfo = reader.ReadUShort();
            Type = reader.ReadShort();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
