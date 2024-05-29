using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.MFAChunks
{
    public class MFAAltFlags : Chunk
    {
        public MFAAltFlag[] AlterableFlags = new MFAAltFlag[0];

        public MFAAltFlags()
        {
            ChunkName = "MFAAltFlags";
            ChunkID = 0x0039;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            AlterableFlags = new MFAAltFlag[reader.ReadInt()];
            for (int i = 0; i < AlterableFlags.Length; i++)
            {
                AlterableFlags[i] = new MFAAltFlag();
                AlterableFlags[i].ReadMFA(reader);
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
