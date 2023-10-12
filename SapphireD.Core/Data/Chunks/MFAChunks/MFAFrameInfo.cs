using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.MFAChunks
{
    public class MFAFrameInfo : Chunk
    {
        public int Handle;
        public int EditorX;
        public int EditorY;
        public int Stamp;
        public int EditorLayer;

        public MFAFrameInfo()
        {
            ChunkName = "MFAFrameInfo";
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
