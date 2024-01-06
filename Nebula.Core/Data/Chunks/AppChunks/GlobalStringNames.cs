using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class GlobalStringNames : Chunk
    {
        public string[] Names = new string[0];

        public GlobalStringNames()
        {
            ChunkName = "GlobalStringNames";
            ChunkID = 0x223D;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Names = new string[reader.ReadInt()];
            for (int i = 0; i < Names.Length; i++)
                Names[i] = reader.ReadYuniversal();

            NebulaCore.PackageData.GlobalStringNames = this;
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
