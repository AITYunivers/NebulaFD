using SapphireD.Core.Data.Chunks.FrameChunks;
using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class AnimationOffsets : Chunk
    {
        public List<int> Offsets = new();

        public AnimationOffsets()
        {
            ChunkName = "AnimationOffsets";
            ChunkID = 0x444A;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            while (reader.HasMemory(4))
                Offsets.Add(reader.ReadInt());

            SapDCore.PackageData.AnimationOffsets = this;
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
