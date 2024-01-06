using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.FrameChunks
{
    public class FrameHandles : Chunk
    {
        public short[] FrameHandleIndex = new short[0];

        public FrameHandles()
        {
            ChunkName = "FrameHandles";
            ChunkID = 0x222B;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            var count = reader.Size() / 2;
            FrameHandleIndex = new short[count];

            for (int i = 0; i < count; i++)
                FrameHandleIndex[i] = reader.ReadShort();

            FRipCore.PackageData.FrameHandles = FrameHandleIndex.ToList();
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
