using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class AppHeader : Chunk
    {
        public BitDict Flags = new BitDict( // Flags
            "HeadingWhenMaximized",    // Heading When Maximized
            "HeadingDisabled",         // Heading Disabled
            "FitInside",               // Fit inside (black bars)
            "MachineIndependentSpeed", // Machine-independent speed
            "ResizeDisplay", "", "",   // Resize display to fill window size
            "MenuDisplayedDisabled",   // Menu displayed on boot-up Disabled
            "MenuBar",                 // Menu Bar
            "MaximizedOnBoot",         // Maximized on boot-up
            "MultiSamples",            // Multi-Samples
            "ChangeResolutionMode",    // Change Resolution Mode
            "AllowFullscreenSwitch"    // Allow user to switch to/from full screen
        );
        public BitDict NewFlags = new BitDict( // New Flags
            "PlaySoundsOverFrames", "", "", // Play sounds over frames
            "DontMuteOnLostFocus",          // Do not mute samples when application loses focus
            "NoMinimizeBox",                // No Minimize box
            "NoMaximizeBox",                // No Maximize box
            "NoThickFrame",                 // No Thick frame
            "DontCenterFrame",              // Do not center frame area in window
            "DontStopScreenSaver",          // Do not stop screen saver when input event
            "DisableCloseButton",           // Disable Close button
            "HiddenAtStart",                // Hidden at start
            "EnableVisualThemes",           // Enable Visual Themes
            "VSync",                        // V-Sync
            "RunWhenMinimized", "",         // Run when minimized
            "RunWhileResizing"              // Run while resizing
        );
        public BitDict OtherFlags = new BitDict( // Other Flags
            "DebuggerShortcuts", "", "",            // Enable debugger keyboard shortcuts
            "DontShareSubAppData", "", "",          // Do not share data if run as sub-application
            "IncludeExternalFiles",                 // Include external files
            "ShowDebugger", "", "", "", "", "", "", // Show Debugger
            "Direct3D9or11",                        // Display Mode: Direct 3D 9 / Direct 3D 11
            "Direct3D8or11"                         // Display Mode: Direct 3D 8 / Direct 3D 11
        );
        public BitDict DisplayFlags = new BitDict( // Display Flags (MFA Only)
            "MaximizedOnBoot",       // Maximized on boot-up
            "ResizeDisplay",         // Resize display to fill window size
            "ChangeResolutionMode",  // Change Resolution Mode
            "AllowFullscreenSwitch", // Allow user to switch to/from full screen
            "Heading",               // Heading
            "HeadingWhenMaximized",  // Heading when maximized
            "MenuBar",               // Menu bar
            "MenuDisplayed",         // Menu displayed on boot-up
            "NoMinimizeBox",         // No Minimize box
            "NoMaximizeBox",         // No Maximize box
            "NoThickFrame",          // No Thick frame
            "DontCenterFrame",       // Do not center frame area in window
            "DisableCloseButton",    // Disable Close button
            "HiddenAtStart", "",     // Hidden at start
            "KeepScreenRatio",       // Keep screen ratio
            "AntiAliasing", "",      // Anti-aliasing when resizing
            "RightToLeftReading",    // Right-to-left reading
            "RightToLeftLayout", "", // Right-to-left layout
            "FitInside"              // Fit inside (black bars)
        );
        public BitDict GraphicFlags = new BitDict( // Graphic Flags (MFA Only)
            "MultiSamples",                  // Multi-samples
            "MachineIndependentSpeed",       // Machine-independent speed
            "PlaySoundsOverFrames",          // Play sounds over frames
            "DontMuteOnLostFocus",           // Do not mute samples when application loses focus
            "DontStopScreenSaver", "", "",   // Do not stop screen saver when input evemt
            "EnableVisualThemes",            // Enable Visual Themes
            "VSync",                         // V-Sync
            "RunWhenMinimized",              // Run when minimized
            "RunWhileResizing",              // Run while resizing
            "DebuggerShortcuts",             // Enable debugger keyboard shortcuts
            "DontShowDebugger",              // Show Debugger Disabled
            "DontShareSubAppData",           // Do not share data if run as sub-application
            "Direct3D9",                     // Display Mode: Direct3D 9
            "Direct3D8", "", "", "", "", "", // Display Mode: Direct3D 8
            "DontIgnoreDestroyFar",          // Ignore "Destroy if too far" option if "Inactivate if too far" is set to No Disabled
            "DisableIME",                    // Disable IME
            "ReduceCPUUsage", "", "",        // Reduce CPU usage
            "EnableProfiling",               // Enable profiling
            "DontStartProfiling",            // Start profiling at start of frame Disabled
            "Direct3D11",                    // Display Mode: Direct3D 11
            "PremultipliedAlpha",            // Premultiplied alpha
            "DontOptimizeEvents",            // Optimize events Disabled
            "RecordSlowestLoops"             // Record slowest app loops
        );

        public short GraphicMode = 4; /* Color Mode
                                       * 3: 256 colors
                                       * 4: 16 million colors
                                       * 6: 32768 colors
                                       * 7: 65536 colors
                                       */
        public short AppWidth = 640;            // Window Width
        public short AppHeight = 480;           // Window Height
        public Color BorderColor = Color.White; // Border Color
        public int FrameCount;                  // Number of Frames
        public int FrameRate = 60;              // Frames Per Second
        public int WindowMenu;                  // Window Menu Index

        public int[] ControlType = new int[4]     // Control Type Per Player
        {
            5, 5, 5, 5
        };
        public int[][] ControlKeys = new int[4][] // Control Keys Per Player
        {
            new int[8] {38, 40, 37, 39, 16, 17, 32, 13},
            new int[8] {38, 40, 37, 39, 16, 17, 32, 13},
            new int[8] {38, 40, 37, 39, 16, 17, 32, 13},
            new int[8] {38, 40, 37, 39, 16, 17, 32, 13}
        };

        public int InitScore = (0 + 1) * -1; // Initial Score
        public int InitLives = (3 + 1) * -1; // Initial Number of Lives

        public AppHeader()
        {
            ChunkName = "AppHeader";
            ChunkID = 0x2223;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            reader.Skip(4);
            Flags.Value = (uint)reader.ReadShort();      // Default: 4294944001
            NewFlags.Value = (uint)reader.ReadShort();   // Default: 2048
            GraphicMode = reader.ReadShort();
            OtherFlags.Value = (uint)reader.ReadShort(); // Default: 4294951041

            AppWidth = reader.ReadShort();
            AppHeight = reader.ReadShort();

            InitScore = reader.ReadInt();
            InitLives = reader.ReadInt();

            for (int i = 0; i < 4; i++)
                ControlType[i] = reader.ReadShort();

            for (int i = 0; i < 4; i++)
                for (int ii = 0; ii < 8; ii++)
                    ControlKeys[i][ii] = reader.ReadShort();

            BorderColor = reader.ReadColor();
            FrameCount = reader.ReadInt();
            FrameRate = reader.ReadInt();
            WindowMenu = reader.ReadInt();

            NebulaCore.PackageData.AppHeader = this;
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

        public void SyncFlags(bool fromMFA = false)
        {
            ExtendedHeader ext = NebulaCore.PackageData.ExtendedHeader;

            if (!fromMFA)
            {
                DisplayFlags.Value = 240; // Default Value
                DisplayFlags["MaximizedOnBoot"] = Flags["MaximizedOnBoot"];
                DisplayFlags["ResizeDisplay"] = Flags["ResizeDisplay"];
                DisplayFlags["ChangeResolutionMode"] = Flags["ChangeResolutionMode"];
                DisplayFlags["AllowFullscreenSwitch"] = Flags["AllowFullscreenSwitch"];
                DisplayFlags["Heading"] = !Flags["HeadingDisabled"];
                DisplayFlags["HeadingWhenMaximized"] = Flags["HeadingWhenMaximized"];
                DisplayFlags["MenuBar"] = Flags["MenuBar"];
                DisplayFlags["MenuDisplayed"] = !Flags["MenuDisplayedDisabled"];
                DisplayFlags["NoMinimizeBox"] = NewFlags["NoMinimizeBox"];
                DisplayFlags["NoMaximizeBox"] = NewFlags["NoMaximizeBox"];
                DisplayFlags["NoThickFrame"] = NewFlags["NoThickFrame"];
                DisplayFlags["DontCenterFrame"] = NewFlags["DontCenterFrame"];
                DisplayFlags["DisableCloseButton"] = NewFlags["DisableCloseButton"];
                DisplayFlags["HiddenAtStart"] = NewFlags["HiddenAtStart"];
                DisplayFlags["KeepScreenRatio"] = ext.Flags["KeepScreenRatio"];
                DisplayFlags["AntiAliasing"] = ext.Flags["AntiAliasing"];
                DisplayFlags["RightToLeftReading"] = ext.Flags["RightToLeftReading"];
                DisplayFlags["RightToLeftLayout"] = ext.Flags["RightToLeftLayout"];
                DisplayFlags["FitInside"] = Flags["FitInside"];

                GraphicFlags.Value = 807471233; // Default Value
                GraphicFlags["MultiSamples"] = Flags["MultiSamples"];
                GraphicFlags["MachineIndependentSpeed"] = Flags["MachineIndependentSpeed"];
                GraphicFlags["PlaySoundsOverFrames"] = NewFlags["PlaySoundsOverFrames"];
                GraphicFlags["DontMuteOnLostFocus"] = NewFlags["DontMuteOnLostFocus"];
                GraphicFlags["DontStopScreenSaver"] = NewFlags["DontStopScreenSaver"];
                GraphicFlags["EnableVisualThemes"] = NewFlags["EnableVisualThemes"];
                GraphicFlags["VSync"] = NewFlags["VSync"];
                GraphicFlags["RunWhenMinimized"] = NewFlags["RunWhenMinimized"];
                GraphicFlags["RunWhileResizing"] = NewFlags["RunWhileResizing"];
                GraphicFlags["DebuggerShortcuts"] = OtherFlags["DebuggerShortcuts"];
                GraphicFlags["DontShowDebugger"] = !OtherFlags["ShowDebugger"];
                GraphicFlags["DontShareSubAppData"] = OtherFlags["DontShareSubAppData"];
                GraphicFlags["Direct3D9"] = OtherFlags["Direct3D9or11"] && !OtherFlags["Direct3D8or11"];
                GraphicFlags["Direct3D8"] = OtherFlags["Direct3D8or11"] && !OtherFlags["Direct3D9or11"];
                GraphicFlags["DontIgnoreDestroyFar"] = ext.Flags["DontIgnoreDestroyFar"];
                GraphicFlags["DisableIME"] = ext.Flags["DisableIME"];
                GraphicFlags["ReduceCPUUsage"] = ext.Flags["ReduceCPUUsage"];
                GraphicFlags["Direct3D11"] = OtherFlags["Direct3D9or11"] && OtherFlags["Direct3D8or11"];
                GraphicFlags["PremultipliedAlpha"] = ext.Flags["PremultipliedAlpha"];
            }
            else
            {
                Flags.Value = 4294944001; // Default Value
                Flags["HeadingWhenMaximized"] = DisplayFlags["HeadingWhenMaximized"];
                Flags["HeadingDisabled"] = !DisplayFlags["Heading"];
                Flags["FitInside"] = DisplayFlags["FitInside"];
                Flags["MachineIndependentSpeed"] = GraphicFlags["MachineIndependentSpeed"];
                Flags["ResizeDisplay"] = DisplayFlags["ResizeDisplay"];
                Flags["MenuDisplayedDisabled"] = !DisplayFlags["MenuDisplayed"];
                Flags["MenuBar"] = DisplayFlags["MenuBar"];
                Flags["MaximizedOnBoot"] = DisplayFlags["MaximizedOnBoot"];
                Flags["MultiSamples"] = GraphicFlags["MultiSamples"];
                Flags["ChangeResolutionMode"] = DisplayFlags["ChangeResolutionMode"];
                Flags["AllowFullscreenSwitch"] = DisplayFlags["AllowFullscreenSwitch"];

                NewFlags.Value = 2048; // Default Value
                NewFlags["PlaySoundsOverFrames"] = GraphicFlags["PlaySoundsOverFrames"];
                NewFlags["DontMuteOnLostFocus"] = GraphicFlags["DontMuteOnLostFocus"];
                NewFlags["NoMinimizeBox"] = DisplayFlags["NoMinimizeBox"];
                NewFlags["NoMaximizeBox"] = DisplayFlags["NoMaximizeBox"];
                NewFlags["NoThickFrame"] = DisplayFlags["NoThickFrame"];
                NewFlags["DontCenterFrame"] = DisplayFlags["DontCenterFrame"];
                NewFlags["DontStopScreenSaver"] = GraphicFlags["DontStopScreenSaver"];
                NewFlags["DisableCloseButton"] = DisplayFlags["DisableCloseButton"];
                NewFlags["HiddenAtStart"] = DisplayFlags["HiddenAtStart"];
                NewFlags["EnableVisualThemes"] = GraphicFlags["EnableVisualThemes"];
                NewFlags["VSync"] = GraphicFlags["VSync"];
                NewFlags["RunWhenMinimized"] = GraphicFlags["RunWhenMinimized"];
                NewFlags["RunWhileResizing"] = GraphicFlags["RunWhileResizing"];

                OtherFlags.Value = 4294951041; // Default Value
                OtherFlags["DebuggerShortcuts"] = GraphicFlags["DebuggerShortcuts"];
                OtherFlags["DontShareSubAppData"] = GraphicFlags["DontShareSubAppData"];
                OtherFlags["ShowDebugger"] = !GraphicFlags["DontShowDebugger"];
                OtherFlags["Direct3D9or11"] = GraphicFlags["Direct3D9"] || GraphicFlags["Direct3D11"];
                OtherFlags["Direct3D8or11"] = GraphicFlags["Direct3D8"] || GraphicFlags["Direct3D11"];

                ext.Flags.Value = 3288334336; // Default Value
                ext.Flags["KeepScreenRatio"] = DisplayFlags["KeepScreenRatio"];
                ext.Flags["AntiAliasing"] = DisplayFlags["AntiAliasing"];
                ext.Flags["RightToLeftReading"] = DisplayFlags["RightToLeftReading"];
                ext.Flags["RightToLeftLayout"] = DisplayFlags["RightToLeftLayout"];
                ext.Flags["DontIgnoreDestroyFar"] = GraphicFlags["DontIgnoreDestroyFar"];
                ext.Flags["DisableIME"] = GraphicFlags["DisableIME"];
                ext.Flags["ReduceCPUUsage"] = GraphicFlags["ReduceCPUUsage"];
                ext.Flags["PremultipliedAlpha"] = GraphicFlags["PremultipliedAlpha"];

                ext.CompressionFlags.Value = 1049120; // Default Value
            }

            /*
            public BitDict OtherFlags = new BitDict( // Other Flags
                "IncludeExternalFiles",                     // Include external files
            );

            public BitDict GraphicFlags = new BitDict( // Graphic Flags (MFA Only)
                "EnableProfiling",               // Enable profiling
                "DontStartProfiling",            // Start profiling at start of frame Disabled
                "DontOptimizeEvents",            // Optimize events Disabled
                "RecordSlowestLoops"             // Record slowest app loops
            );

             public BitDict ext.Flags = new BitDict(
                "DontOptimizeStrings", "", "", "",       // Optimize string objects Disabled
                "OptimizePlaySample"                     // Optimize 'Play Sample'
            );

            public BitDict ext.CompressionFlags = new BitDict(
                "CompressionLevelMax",            // Compression Level: Maximum
                "CompressSounds",                 // Compress Sounds
                "IncludeExternalFiles",           // Include external files
                "NoAutoImageFilters",             // Image Filters: Automatic Disabled
                "NoAutoSoundFilters", "", "", "", // Sound Filters: Automatic Disabled
                "DontDisplayBuildWarning",        // Display build warning messages Disabled
                "OptimizeImageSize"               // Optimize image size in RAM
            );
            */
        }
    }
}
