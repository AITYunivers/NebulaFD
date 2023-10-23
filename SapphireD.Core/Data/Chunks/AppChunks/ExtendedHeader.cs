using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class ExtendedHeader : Chunk
    {
        public BitDict Flags = new BitDict(
            "KeepScreenRatio", "1",
            "AntiAliasingWhenResizing", "2", "3",
            "RightToLeftReading", "4",
            "RightToLeftLayout", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18",
            "DontOptimizeStrings", "19", "20", "21",
            "DontIgnoreDestroy",
            "DisableIME",
            "ReduceCPUUsage", "22",
            "PremultipliedAlpha",
            "OptimizePlaySample");

        public BitDict CompressionFlags = new BitDict(
            "CompressionLevelMax",
            "CompressSounds",
            "IncludeExternalFiles",
            "NoAutoImageFilters",
            "NoAutoSoundFilters", "1", "2", "3",
            "DontDisplayBuildWarning",
            "OptimizeImageSize");

        public BitDict ViewFlags = new BitDict("1");
        public BitDict NewFlags = new BitDict("1");

        public byte BuildType;
        public int ScreenRatio;
        public int ScreenAngle;

        public ExtendedHeader()
        {
            ChunkName = "AppHeader2";
            ChunkID = 0x2245;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Flags.Value = reader.ReadUInt();
            BuildType = reader.ReadByte();
            reader.Skip(3);
            CompressionFlags.Value = reader.ReadUInt();
            ScreenRatio = reader.ReadShort();
            ScreenAngle = reader.ReadShort();
            ViewFlags.Value = reader.ReadUShort();
            NewFlags.Value = reader.ReadUShort();

            SapDCore.PackageData.ExtendedHeader = this;
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
