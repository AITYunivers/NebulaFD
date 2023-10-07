using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.ObjectChunks
{
    public class FrameItems : Chunk
    {
        public int Count;
        public ObjectInfo[] Objects;

        public FrameItems()
        {
            ChunkName = "FrameItems";
            ChunkID = 0x2229;
        }

        public override void ReadCCN(ByteReader reader)
        {
            Count = reader.ReadInt();
            Objects = new ObjectInfo[Count];

            for (int oi = 0; oi < Count; oi++)
            {
                if (reader.Tell() >= reader.Size()) break;
                ObjectInfo obj = new ObjectInfo();
                obj.ReadCCN(reader);
                Objects[oi] = obj;
            }

            SapDCore.PackageData.FrameItems = this;
        }

        public override void ReadMFA(ByteReader reader)
        {

        }

        public override void WriteCCN(ByteWriter writer)
        {

        }

        public override void WriteMFA(ByteWriter writer)
        {

        }
    }
}
