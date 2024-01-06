using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.MFAChunks
{
    public class MFAAltFlagIndex : Chunk
    {
        public int[] Index = new int[0];

        public MFAAltFlagIndex()
        {
            ChunkName = "MFAAltFlagIndex";
            ChunkID = 0x003C;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Index = new int[reader.ReadInt()];
            if (reader.Size() != (Index.Length * 4) + 4)
                return;

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
