using Nebula.Core.Data.Chunks.ChunkTypes;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.ObjectChunks
{
    public class ObjectInfoName : StringChunk
    {
        public ObjectInfoName()
        {
            ChunkName = "ObjectInfoName";
            ChunkID = 0x4445;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader);

            ((ObjectInfo)extraInfo[0]).Name = Value;
        }
    }
}
