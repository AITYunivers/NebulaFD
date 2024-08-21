using ILGPU.IR;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

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

            if (NebulaCore.Build != EngineVersion)
            {
                this.Log($"Build was modified from {EngineVersion}.{EngineSubversion} to {NebulaCore.Build}, reverting.", Spectre.Console.Color.Yellow3_1);
                NebulaCore.Build = EngineVersion;

                if (!NebulaCore.Unpacked)
                {
                    PackageData pkgData = NebulaCore.PackageData;
                    Decryption.MakeKey(pkgData.AppName, pkgData.Copyright, pkgData.EditorFilename);
                }
            }

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
