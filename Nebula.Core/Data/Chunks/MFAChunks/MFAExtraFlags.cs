using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.MFAChunks
{
    public class MFAExtraFlags : Chunk
    {
        public BitDict ExtraFlags = new BitDict(512,
            "", "",
            "DontOptimizeImageSize",    // Optimize image size in RAM Disabled
            "ProfileTopLevelConds",     // Profile top-level conditions only
            "DontOptimizePlaySample",   // Optimize 'Play Sample' Disabled
            "BuildCache",               // Build Cache
            "AllowAltsForNonActives",   // Allow alterable values for counters for strings
            "",
            "UnpackedEXE",              // Unpacked EXE
            "FakeObjsInGlobalEvts",     // Allow global events to count objects that don't exist
            "DPIAware",                 // DPI aware
            "DontOptimizeStrings",      // Optimize string objects Disabled
            "",
            "DontDisplayCrashLine",     // Crash: display last event line Disabled
            "WindowsLikeCollision"      // Windows-like collisions on other platforms
        );

        public MFAExtraFlags()
        {
            ChunkName = "MFAExtraFlags";
            ChunkID = 0x003C;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            ExtraFlags.Value = reader.ReadUInt();
            reader.ReadAutoYuniversal();
            SyncFlags(true);
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            ByteWriter chunkWriter = new ByteWriter(new MemoryStream());
            {
                chunkWriter.WriteUInt(ExtraFlags.Value);
                chunkWriter.WriteAutoYunicode("");
            }

            writer.WriteByte((byte)ChunkID);
            writer.WriteInt((int)chunkWriter.Tell());
            writer.WriteWriter(chunkWriter);
            chunkWriter.Flush();
            chunkWriter.Close();
        }

        public void SyncFlags(bool fromMFA = false)
        {
            if (fromMFA)
            {
                NebulaCore.PackageData.ExtendedHeader.CompressionFlags["OptimizeImageSize"] = !ExtraFlags["DontOptimizeImageSize"];
                NebulaCore.PackageData.ExtendedHeader.Flags["OptimizePlaySample"] = !ExtraFlags["DontOptimizePlaySample"];
                NebulaCore.PackageData.ExtendedHeader.Flags["DontOptimizeStrings"] = ExtraFlags["DontOptimizeStrings"];
            }
            else
            {
                ExtraFlags["DontOptimizeImageSize"] = !NebulaCore.PackageData.ExtendedHeader.CompressionFlags["OptimizeImageSize"];
                ExtraFlags["DontOptimizePlaySample"] = !NebulaCore.PackageData.ExtendedHeader.Flags["OptimizePlaySample"];
                ExtraFlags["AllowAltsForNonActives"] = true;
                ExtraFlags["UnpackedEXE"] = NebulaCore.PackageData.ModulesDir != string.Empty;
                ExtraFlags["DontOptimizeStrings"] = NebulaCore.PackageData.ExtendedHeader.Flags["DontOptimizeStrings"];
            }
        }
    }
}
