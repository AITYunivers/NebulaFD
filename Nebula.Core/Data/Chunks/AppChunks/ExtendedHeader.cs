using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Diagnostics;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class ExtendedHeader : Chunk
    {
        public BitDict Flags = new BitDict(3288334336,
            "KeepScreenRatio", "",                   // Keep screen ratio
            "AntiAliasing",                          // Anti-aliasing when resizing
            "ForceGlobalRefresh", "",                // Force global refresh
            "RightToLeftReading",                    // Right-to-left reading
            "StatusLine",                            // Status Line
            "RightToLeftLayout",                     // Right-to-left layout
            "EnableAds", "",                         // Enable Ads
            "AutoEnd",                               // Auto End
            "DisableBackButton",                     // Disable Back Button
            "SmoothResizing",                        // Smooth resizing (Android)
            "CrashReporting",                        // Enable online crash reporting (Android)
            "RequireGPU",                            // Require GPU (Android)
            "KeepResourcesBetweenFrames",            // Keep resources between frames (Html5)
            "OpenGL1",                               // Display Mode: OpenGL ES 1.1 (Android)
            "OpenGL30",                              // Display Mode: OpenGL ES 3.0 (Android)
            "OpenGL31",                              // Display Mode: OpenGL ES 2.0 (Android)
            "UseSystemFont",                         // Use system font in text objects (Android)
            "RunEvenIfNotFocus",                     // Run even if not focus (Html5)
            "KeyboardOverApp",                       // Display keyboard over application window (Android)
            "DontOptimizeStrings",                   // Optimize string objects Disabled
            "PreloaderQuit",                         // Preloader Quit (Html5)
            "LoadDataAtStart",                       // Load all data at start (Html5)
            "LoadSoundsOnTouch",                     // Load Sounds On Touch (Html5)
            "DontIgnoreDestroyFar",                  // Ignore "Destroy if too far" option if "Inactivate if too far" is set to No Disabled
            "DisableIME",                            // Disable IME
            "ReduceCPUUsage", "",                    // Reduce CPU Usage
            "PremultipliedAlpha",                    // Premultiplied alpha
            "OptimizePlaySample"                     // Optimize 'Play Sample'
        );

        public BitDict CompressionFlags = new BitDict(1049120,
            "CompressionLevelMax",            // Compression Level: Maximum
            "CompressSounds",                 // Compress Sounds
            "IncludeExternalFiles",           // Include external files
            "NoAutoImageFilters",             // Image Filters: Automatic Disabled
            "NoAutoSoundFilters", "", "", "", // Sound Filters: Automatic Disabled
            "DontDisplayBuildWarning",        // Display build warning messages Disabled
            "OptimizeImageSize"               // Optimize image size in RAM
        );

        public BitDict ExtraFlags = new BitDict(
            "EnableLoadOnCall",     // Enable object "Load on Call" options (Android)
            "WindowsLikeCollisions" // Windows-like collisions on other platforms
        );

        public byte BuildType;
        public int ScreenRatio;
        public int ScreenAngle;
        public short ViewMode;

        public ExtendedHeader()
        {
            ChunkName = "ExtendedHeader";
            ChunkID = 0x2245;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Flags.Value = reader.ReadUInt();
            BuildType = reader.ReadByte();
            switch (BuildType)
            {
                case 0: // Windows EXE Application
                case 1: // Windows Screen Saver
                case 2: // Sub-Application
                    NebulaCore.Windows = true;
                    break;
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
                case 74: // Nintendo Switch
                case 75: // Xbox One
                case 78: // Playstation 4
                    if (NebulaCore.Fusion != 3.0f)
                    {
                        this.Log($"Fusion 3 detected, correcting.", Spectre.Console.Color.Yellow3_1);
                        NebulaCore.Fusion = 3.0f;
                    }
                    break;
            }
            reader.Skip(3);
            CompressionFlags.Value = reader.ReadUInt();
            ScreenRatio = reader.ReadShort();
            ScreenAngle = reader.ReadShort();
            ViewMode = reader.ReadShort();
            ExtraFlags.Value = reader.ReadUShort();

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
