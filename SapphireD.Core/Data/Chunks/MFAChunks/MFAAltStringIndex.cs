using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.MFAChunks
{
    public class MFAAltStringIndex : Chunk
    {
        public int[] Index = new int[0];

        public MFAAltStringIndex()
        {
            ChunkName = "MFAAltStringIndex";
            ChunkID = 0x003B;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Index = new int[reader.ReadInt()];
            for (int i = 0; i < Index.Length; i++)
                Index[i] = reader.ReadInt();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
