using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class GlobalValueNames : Chunk
    {
        public string[] Names = new string[0];

        public GlobalValueNames()
        {
            ChunkName = "GlobalValueNames";
            ChunkID = 0x223C;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Names = new string[reader.ReadInt()];
            for (int i = 0; i < Names.Length; i++)
                Names[i] = reader.ReadYuniversal();

            NebulaCore.PackageData.GlobalValueNames = this;
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
