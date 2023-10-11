using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectAlterableStrings : Chunk
    {
        public string[] AlterableStrings = new string[0];

        public ObjectAlterableStrings()
        {
            ChunkName = "ObjectAlterableStrings";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            AlterableStrings = new string[reader.ReadShort()];
            for (int i = 0; i < AlterableStrings.Length; i++)
                AlterableStrings[i] = reader.ReadYuniversal();
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
