using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.ObjectChunks
{
    public class ObjectInfoProperties : Chunk
    {
        public ObjectInfoProperties()
        {
            ChunkName = "ObjectInfoProperties";
            ChunkID = 0x4446;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

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
