using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class FrameHandles : Chunk
    {
        public short[] FrameHandleIndex;

        public FrameHandles()
        {
            ChunkName = "FrameHandles";
            ChunkID = 0x222B;
        }

        public override void ReadCCN(ByteReader reader)
        {
            var count = reader.Size() / 2;
            FrameHandleIndex = new short[count];

            for (int i = 0; i < count; i++)
                FrameHandleIndex[i] = reader.ReadShort();

            SapDCore.PackageData.FrameHandles = FrameHandleIndex;
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
