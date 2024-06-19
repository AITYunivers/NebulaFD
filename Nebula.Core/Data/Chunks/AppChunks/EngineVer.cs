using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class EngineVer : Chunk
    {
        public BitDict EngineInfo = new BitDict
        (
            "Steam",   // Steam
            "", "2.5+" // 2.5+
        );

        public int EngineVersion;
        public int EngineSubversion;

        public EngineVer()
        {
            ChunkName = "EngineVer";
            ChunkID = 0x224F;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            EngineVersion = reader.ReadInt();
            EngineSubversion = reader.ReadInt();
            EngineInfo.Value = reader.ReadUInt();

            NebulaCore.PackageData.EngineVer = this;
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
