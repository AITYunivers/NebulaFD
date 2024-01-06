using FusionRipper.Core.Memory;
using FusionRipper.Core.Utilities;
using System.Drawing;

namespace FusionRipper.Core.Data.Chunks.ObjectChunks
{
    public class FrameItems : Chunk
    {
        public int Count;
        public Dictionary<int, ObjectInfo> Items = new Dictionary<int, ObjectInfo>();

        public FrameItems()
        {
            ChunkName = "FrameItems";
            ChunkID = 0x2229;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Count = reader.ReadInt();

            for (int oi = 0; oi < Count; oi++)
            {
                ObjectInfo newOI = new ObjectInfo();
                newOI.ReadCCN(reader);
                Items.Add(newOI.Header.Handle, newOI);
            }

            FRipCore.PackageData.FrameItems = this;
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
