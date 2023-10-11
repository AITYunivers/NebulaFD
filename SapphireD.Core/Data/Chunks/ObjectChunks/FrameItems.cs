using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.ObjectChunks
{
    public class FrameItems : Chunk
    {
        public int Count;
        public ObjectInfo[] Objects = new ObjectInfo[0];

        public FrameItems()
        {
            ChunkName = "FrameItems";
            ChunkID = 0x2229;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Count = reader.ReadInt();
            Objects = new ObjectInfo[Count];

            for (int oi = 0; oi < Count; oi++)
            {
                Objects[oi] = new ObjectInfo();
                Objects[oi].ReadCCN(reader);
            }

            SapDCore.PackageData.FrameItems = this;
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
