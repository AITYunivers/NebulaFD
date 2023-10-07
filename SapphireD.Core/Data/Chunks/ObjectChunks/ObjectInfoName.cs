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

        public override void ReadCCN(ByteReader reader)
        {
            base.ReadCCN(reader);

            ObjectInfo.curInfo.Name = Value;
        }
    }
}
