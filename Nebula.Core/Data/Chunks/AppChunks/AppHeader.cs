using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.Memory;
using System.Drawing;
using static Nebula.Core.Utilities.Enums;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class AppHeader : Chunk
    {
        public BitDict Flags = new BitDict(4294944001, typeof(AppHeaderFlags));
        public BitDict NewFlags = new BitDict(2048, typeof(AppHeaderNewFlags));
        public BitDict OtherFlags = new BitDict(4294951041, typeof(AppHeaderOtherFlags));
        public BitDict DisplayFlags = new BitDict(240, // Display Flags (MFA Only)
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
        ); // MFA Only
        public BitDict GraphicFlags = new BitDict(807471233, // Graphic Flags (MFA Only)
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
        ); // MFA Only

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
                DisplayFlags["MaximizedOnBoot"] = Flags == AppHeaderFlags.MaximizedOnBoot;
                DisplayFlags["ResizeDisplay"] = Flags == AppHeaderFlags.ResizeDisplay;
                DisplayFlags["ChangeResolutionMode"] = Flags == AppHeaderFlags.ChangeResolutionMode;
                DisplayFlags["AllowFullscreenSwitch"] = Flags == AppHeaderFlags.AllowFullscreenSwitch;
                DisplayFlags["Heading"] = Flags != AppHeaderFlags.HeadingDisabled;
                DisplayFlags["HeadingWhenMaximized"] = Flags == AppHeaderFlags.HeadingWhenMaximized;
                DisplayFlags["MenuBar"] = Flags == AppHeaderFlags.MenuBar;
                DisplayFlags["MenuDisplayed"] = Flags != AppHeaderFlags.MenuDisplayedDisabled;
                DisplayFlags["NoMinimizeBox"] = NewFlags == AppHeaderNewFlags.NoMinimizeBox;
                DisplayFlags["NoMaximizeBox"] = NewFlags == AppHeaderNewFlags.NoMaximizeBox;
                DisplayFlags["NoThickFrame"] = NewFlags == AppHeaderNewFlags.NoThickFrame;
                DisplayFlags["DontCenterFrame"] = NewFlags == AppHeaderNewFlags.DontCenterFrame;
                DisplayFlags["DisableCloseButton"] = NewFlags == AppHeaderNewFlags.DisableCloseButton;
                DisplayFlags["HiddenAtStart"] = NewFlags == AppHeaderNewFlags.HiddenAtStart;
                DisplayFlags["KeepScreenRatio"] = ext.Flags["KeepScreenRatio"];
                DisplayFlags["AntiAliasing"] = ext.Flags["AntiAliasing"];
                DisplayFlags["RightToLeftReading"] = ext.Flags["RightToLeftReading"];
                DisplayFlags["RightToLeftLayout"] = ext.Flags["RightToLeftLayout"];
                DisplayFlags["FitInside"] = Flags == AppHeaderFlags.FitInside;

                GraphicFlags["MultiSamples"] = Flags == AppHeaderFlags.MultiSamples;
                GraphicFlags["MachineIndependentSpeed"] = Flags == AppHeaderFlags.MachineIndependentSpeed;
                GraphicFlags["PlaySoundsOverFrames"] = NewFlags == AppHeaderNewFlags.PlaySoundsOverFrames;
                GraphicFlags["DontMuteOnLostFocus"] = NewFlags == AppHeaderNewFlags.DontMuteOnLostFocus;
                GraphicFlags["DontStopScreenSaver"] = NewFlags["DontStopScreenSaver"];
                GraphicFlags["EnableVisualThemes"] = NewFlags["EnableVisualThemes"];
                GraphicFlags["VSync"] = NewFlags == AppHeaderNewFlags.VSync;
                GraphicFlags["RunWhenMinimized"] = NewFlags == AppHeaderNewFlags.RunWhenMinimized;
                GraphicFlags["RunWhileResizing"] = NewFlags == AppHeaderNewFlags.RunWhileResizing;
                GraphicFlags["DebuggerShortcuts"] = OtherFlags == AppHeaderOtherFlags.DebuggerShortcuts;
                GraphicFlags["DontShowDebugger"] = OtherFlags != AppHeaderOtherFlags.ShowDebugger;
                GraphicFlags["DontShareSubAppData"] = OtherFlags == AppHeaderOtherFlags.DontShareSubAppData;
                GraphicFlags["Direct3D9"] = OtherFlags == AppHeaderOtherFlags.Direct3D9or11 && OtherFlags != AppHeaderOtherFlags.Direct3D8or11;
                GraphicFlags["Direct3D8"] = OtherFlags != AppHeaderOtherFlags.Direct3D9or11 && OtherFlags == AppHeaderOtherFlags.Direct3D8or11;
                GraphicFlags["DontIgnoreDestroyFar"] = ext.Flags["DontIgnoreDestroyFar"];
                GraphicFlags["DisableIME"] = ext.Flags["DisableIME"];
                GraphicFlags["ReduceCPUUsage"] = ext.Flags["ReduceCPUUsage"];
                GraphicFlags["Direct3D11"] = OtherFlags == AppHeaderOtherFlags.Direct3D9or11 && OtherFlags == AppHeaderOtherFlags.Direct3D8or11;
                GraphicFlags["PremultipliedAlpha"] = ext.Flags["PremultipliedAlpha"];
                GraphicFlags["DontOptimizeEvents"] = !FrameEvents.OptimizedEvents;
            }
            else
            {
                Flags.SetFlag(AppHeaderFlags.HeadingWhenMaximized, DisplayFlags["HeadingWhenMaximized"]);
                Flags.SetFlag(AppHeaderFlags.HeadingDisabled, !DisplayFlags["Heading"]);
                Flags.SetFlag(AppHeaderFlags.FitInside, DisplayFlags["FitInside"]);
                Flags.SetFlag(AppHeaderFlags.MachineIndependentSpeed, GraphicFlags["MachineIndependentSpeed"]);
                Flags.SetFlag(AppHeaderFlags.ResizeDisplay, DisplayFlags["ResizeDisplay"]);
                Flags.SetFlag(AppHeaderFlags.MenuDisplayedDisabled, !DisplayFlags["MenuDisplayed"]);
                Flags.SetFlag(AppHeaderFlags.MenuBar, DisplayFlags["MenuBar"]);
                Flags.SetFlag(AppHeaderFlags.MaximizedOnBoot, DisplayFlags["MaximizedOnBoot"]);
                Flags.SetFlag(AppHeaderFlags.MultiSamples, GraphicFlags["MultiSamples"]);
                Flags.SetFlag(AppHeaderFlags.ChangeResolutionMode, DisplayFlags["ChangeResolutionMode"]);
                Flags.SetFlag(AppHeaderFlags.AllowFullscreenSwitch, DisplayFlags["AllowFullscreenSwitch"]);

                NewFlags.SetFlag(AppHeaderNewFlags.PlaySoundsOverFrames, GraphicFlags["PlaySoundsOverFrames"]);
                NewFlags.SetFlag(AppHeaderNewFlags.DontMuteOnLostFocus, GraphicFlags["DontMuteOnLostFocus"]);
                NewFlags.SetFlag(AppHeaderNewFlags.NoMinimizeBox, DisplayFlags["NoMinimizeBox"]);
                NewFlags.SetFlag(AppHeaderNewFlags.NoMaximizeBox, DisplayFlags["NoMaximizeBox"]);
                NewFlags.SetFlag(AppHeaderNewFlags.NoThickFrame, DisplayFlags["NoThickFrame"]);
                NewFlags.SetFlag(AppHeaderNewFlags.DontCenterFrame, DisplayFlags["DontCenterFrame"]);
                NewFlags.SetFlag(AppHeaderNewFlags.DontStopScreenSaver, GraphicFlags["DontStopScreenSaver"]);
                NewFlags.SetFlag(AppHeaderNewFlags.DisableCloseButton, DisplayFlags["DisableCloseButton"]);
                NewFlags.SetFlag(AppHeaderNewFlags.HiddenAtStart, DisplayFlags["HiddenAtStart"]);
                NewFlags.SetFlag(AppHeaderNewFlags.VSync, GraphicFlags["VSync"]);
                NewFlags.SetFlag(AppHeaderNewFlags.RunWhenMinimized, GraphicFlags["RunWhenMinimized"]);
                NewFlags.SetFlag(AppHeaderNewFlags.RunWhileResizing, GraphicFlags["RunWhileResizing"]);

                OtherFlags.SetFlag(AppHeaderOtherFlags.DebuggerShortcuts, GraphicFlags["DebuggerShortcuts"]);
                OtherFlags.SetFlag(AppHeaderOtherFlags.DontShareSubAppData, GraphicFlags["DontShareSubAppData"]);
                OtherFlags.SetFlag(AppHeaderOtherFlags.ShowDebugger, !GraphicFlags["DontShowDebugger"]);
                OtherFlags.SetFlag(AppHeaderOtherFlags.Direct3D9or11, GraphicFlags["Direct3D9"] || GraphicFlags["Direct3D11"]);
                OtherFlags.SetFlag(AppHeaderOtherFlags.Direct3D8or11, GraphicFlags["Direct3D8"] || GraphicFlags["Direct3D11"]);

                ext.Flags["KeepScreenRatio"] = DisplayFlags["KeepScreenRatio"];
                ext.Flags["AntiAliasing"] = DisplayFlags["AntiAliasing"];
                ext.Flags["RightToLeftReading"] = DisplayFlags["RightToLeftReading"];
                ext.Flags["RightToLeftLayout"] = DisplayFlags["RightToLeftLayout"];
                ext.Flags["DontIgnoreDestroyFar"] = GraphicFlags["DontIgnoreDestroyFar"];
                ext.Flags["DisableIME"] = GraphicFlags["DisableIME"];
                ext.Flags["ReduceCPUUsage"] = GraphicFlags["ReduceCPUUsage"];
                ext.Flags["PremultipliedAlpha"] = GraphicFlags["PremultipliedAlpha"];

                FrameEvents.OptimizedEvents = !GraphicFlags["DontOptimizeEvents"];
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

            public BitDict ext.CompressionFlags = new BitDict(
                "CompressionLevelMax",            // Compression Level: Maximum
                "CompressSounds",                 // Compress Sounds
                "IncludeExternalFiles",           // Include external files
                "NoAutoImageFilters",             // Image Filters: Automatic Disabled
                "NoAutoSoundFilters", "", "", "", // Sound Filters: Automatic Disabled
                "DontDisplayBuildWarning",        // Display build warning messages Disabled
            );
            */
        }
    }
}
