using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class GlobalValues : Chunk
    {
        public int[] Values = new int[0];

        public GlobalValues()
        {
            ChunkName = "GlobalValues";
            ChunkID = 0x2232;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Values = new int[reader.ReadShort()];
            for (int i = 0; i < Values.Length; i++)
                Values[i] = reader.ReadInt();

            SapDCore.PackageData.GlobalValues = this;
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
