using Nebula.Core.Memory;
using System.Diagnostics;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class ExtendedHeader : Chunk
    {
        public BitDict Flags = new BitDict(
            "KeepScreenRatio", "",                   // Keep screen ratio
            "AntiAliasing", "", "",                  // Anti-aliasing when resizing
            "RightToLeftReading", "",                // Right-to-left reading
            "RightToLeftLayout", "", "", "", "", "", // Right-to-left layout
            "", "", "", "", "", "", "", "", "",      //
            "DontOptimizeStrings", "", "", "",       // Optimize string objects Disabled
            "DontIgnoreDestroyFar",                  // Ignore "Destroy if too far" option if "Inactivate if too far" is set to No Disabled
            "DisableIME",                            // Disable IME
            "ReduceCPUUsage", "",                    // Reduce CPU Usage
            "PremultipliedAlpha",                    // Premultiplied alpha
            "OptimizePlaySample"                     // Optimize 'Play Sample'
        );

        public BitDict CompressionFlags = new BitDict(
            "CompressionLevelMax",            // Compression Level: Maximum
            "CompressSounds",                 // Compress Sounds
            "IncludeExternalFiles",           // Include external files
            "NoAutoImageFilters",             // Image Filters: Automatic Disabled
            "NoAutoSoundFilters", "", "", "", // Sound Filters: Automatic Disabled
            "DontDisplayBuildWarning",        // Display build warning messages Disabled
            "OptimizeImageSize"               // Optimize image size in RAM
        );

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
            switch (BuildType)
            {
                case 10: // Adobe Flash
                    NebulaCore.Flash = true;
                    break;
                case 12: // Android / OUYA Application
                case 34: // Android App Bundle
                    NebulaCore.Android = true;
                    break;
                case 13: // iOS Application
                case 14: // iOS Xcode Project
                case 15: // Final iOS Xcode Project
                    NebulaCore.iOS = true;
                    break;
                case 27: // HTML5 Development
                case 28: // HTML5 Final Project
                    NebulaCore.HTML = true;
                    break;
            }
            reader.Skip(3);
            CompressionFlags.Value = reader.ReadUInt();
            ScreenRatio = reader.ReadShort();
            ScreenAngle = reader.ReadShort();
            reader.Skip(4);

            NebulaCore.PackageData.ExtendedHeader = this;
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
