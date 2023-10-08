using SapphireD.Core.Data.Chunks.FrameChunks;
using SapphireD.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireD.Core.Data.Chunks.ObjectChunks
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
