using SapphireD.Core.Data.Chunks.ObjectChunks;
using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.MFAChunks
{
    public class MFAFrameInfo : Chunk
    {
        public int Handle;
        public int EditorX;
        public int EditorY;
        public int IconHandle;
        public int EditorLayer;
        public MFAObjectInfo[] Objects = new MFAObjectInfo[0];
        public MFAFolders Folders = new();

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
