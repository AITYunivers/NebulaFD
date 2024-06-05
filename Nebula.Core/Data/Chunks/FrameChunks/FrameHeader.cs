using Nebula.Core.Memory;
using System.Diagnostics;
using System.Drawing;

namespace Nebula.Core.Data.Chunks.FrameChunks
{
    public class FrameHeader : Chunk
    {
        public int Width;
        public int Height;
        public Color Background = Color.White;

        public BitDict FrameFlags = new BitDict( // Frame Flags (Runtime Only)
            "DisplayTitle",                    // Display frame title in window caption
            "GrabDesktop",                     // Grab desktop at start
            "KeepDisplay", "", "",             // Keep display from previous frame
            "HandleBgCollisions", "", "",      // Handle background collisions even out of window
            "ResizeToScreen", "",              // Resize to screen size at start
            "ForceLoadOnCall", "", "", "", "", // Force Load On Call option for all objects: Yes (force)
            "TimerBasedMovements", "", "",     // Timer-based movements
            "DontInclude",                     // Don't include at build time
            "DontEraseBackground",             // Direct3D: don't erase background if the frame has an effect
            "ForceLoadOnCallIgnore"            // Force Load On Call option for all objects: Yes (ignore)
        );

        public BitDict MFAFrameFlags = new BitDict( // Frame Flags (Editor Only)
            "GrabDesktop",                 // Grab desktop at start
            "KeepDisplay",                 // Keep display from previous frame
            "HandleBgCollisions",          // Handle background collisions even out of window
            "DisplayTitle",                // Display frame title in window caption
            "ResizeToScreen",              // Resize to screen size at start
            "ForceLoadOnCall", "",         // Force Load On Call option for all objects: Yes (force)
            "ScreenSaverSetup",            // Screen saver setup frame
            "TimerBasedMovements", "",     // Timer-based movements
            "DontIncludeGlobalEvents", "", // Include global events Disabled
            "DontInclude",                 // Don't include at build time
            "DontEraseBackground", "",     // Direct3D: don't erase background if the frame has an effect
            "ForceLoadOnCallIgnore"        // Force Load On Call option for all objects: Yes (ignore)
        );

        public FrameHeader()
        {
            ChunkName = "FrameHeader";
            ChunkID = 0x3334;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            if (NebulaCore.Fusion > 1.5f)
            {
                Width = reader.ReadInt();
                Height = reader.ReadInt();
                Background = reader.ReadColor();
                FrameFlags.Value = reader.ReadUInt();
            }
            else
            {
                Width = reader.ReadShort();
                Height = reader.ReadShort();
                Background = reader.ReadColor();
                FrameFlags.Value = reader.ReadUShort();
            }

            ((Frame)extraInfo[0]).FrameHeader = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            Background = reader.ReadColor();
            MFAFrameFlags.Value = reader.ReadUInt();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Width);
            writer.WriteInt(Height);
            writer.WriteColor(Background);
            writer.WriteUInt(MFAFrameFlags.Value);
        }

        public void SyncFlags(bool fromMFA = false)
        {
            if (!fromMFA)
            {
                MFAFrameFlags.Value = 260; // Default Value
                MFAFrameFlags["GrabDesktop"] = FrameFlags["GrabDesktop"];
                MFAFrameFlags["KeepDisplay"] = FrameFlags["KeepDisplay"];
                MFAFrameFlags["HandleBgCollisions"] = FrameFlags["HandleBgCollisions"];
                MFAFrameFlags["DisplayTitle"] = FrameFlags["DisplayTitle"];
                MFAFrameFlags["ResizeToScreen"] = FrameFlags["ResizeToScreen"];
                MFAFrameFlags["ForceLoadOnCall"] = FrameFlags["ForceLoadOnCall"];
                MFAFrameFlags["TimerBasedMovements"] = FrameFlags["TimerBasedMovements"];
                MFAFrameFlags["DontInclude"] = FrameFlags["DontInclude"];
                MFAFrameFlags["DontEraseBackground"] = FrameFlags["DontEraseBackground"];
                MFAFrameFlags["ForceLoadOnCallIgnore"] = FrameFlags["ForceLoadOnCallIgnore"];
            }
            else
            {
                FrameFlags.Value = 32800; // Default Value;
                FrameFlags["DisplayTitle"] = MFAFrameFlags["DisplayTitle"];
                FrameFlags["GrabDesktop"] = MFAFrameFlags["GrabDesktop"];
                FrameFlags["KeepDisplay"] = MFAFrameFlags["KeepDisplay"];
                FrameFlags["HandleBgCollisions"] = MFAFrameFlags["HandleBgCollisions"];
                FrameFlags["ResizeToScreen"] = MFAFrameFlags["ResizeToScreen"];
                FrameFlags["ForceLoadOnCall"] = MFAFrameFlags["ForceLoadOnCall"];
                FrameFlags["TimerBasedMovements"] = MFAFrameFlags["TimerBasedMovements"];
                FrameFlags["DontInclude"] = MFAFrameFlags["DontInclude"]; 
                FrameFlags["DontEraseBackground"] = MFAFrameFlags["DontEraseBackground"]; 
                FrameFlags["ForceLoadOnCallIgnore"] = MFAFrameFlags["ForceLoadOnCallIgnore"];
            }

            /*public BitDict MFAFrameFlags = new BitDict( // Frame Flags (Editor Only)
                "ScreenSaverSetup",            // Screen saver setup frame
                "DontIncludeGlobalEvents", "", // Include global events Disabled
            );*/
        }
    }
}
