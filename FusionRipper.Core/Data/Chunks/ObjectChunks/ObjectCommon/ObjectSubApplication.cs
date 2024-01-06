
using FusionRipper.Core.Memory;
using System.Diagnostics;

namespace FusionRipper.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectSubApplication : Chunk
    {
        public BitDict SubAppFlags = new BitDict( // Sub-Application Flags
            "ShareGlobalValues",     // Share with parent application: Global values & strings
            "SharePlayerLives",      // Share with parent application: Lives
            "SharePlayerScores",     // Share with parent application: Scores
            "ShareWindowAttributes", // Share Windows Attributes (Unused?)
            "Stretch",               // Stretch frame to object size
            "Popup",                 // Popup Window
            "Caption",               // Caption
            "ToolCaption",           // Tool Captio
            "Border",                // Border
            "ResizeWindow",          // Resizable
            "SystemMenu",            // System Menu
            "DisableClose",          // Disable Close
            "Modal",                 // Modal
            "DialogueFrame",         // Dialog Frame
            "Internal",              // Source: Frame from this application
            "HideOnClose",           // Hidden on Close
            "CustomSize",            // Customizable size
            "InternalAboutBox",      // Internal About Box (Unused?)
            "ClipSiblings",          // Clip Siblings
            "SharePlayerControls",   // Share with parent application: Player Controls
            "MDI",                   // MDI Child
            "Docked", "",            // Docked
            "DockedVertical",        // Docked Top
            "DockedHorizontal",      // Docked Right
            "Reopen",                // Reopen (Unused?)
            "Sprite",                // Display as sprite
            "IgnoreResize"           // Windows: ignore parent's 'Resize Display' option
        );

        public int Width;
        public int Height;
        public short Version;
        public short StartFrame;
        public string Name = string.Empty;

        public ObjectSubApplication()
        {
            ChunkName = "ObjectSubApplication";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            reader.Skip(4);
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            Version = reader.ReadShort();
            StartFrame = reader.ReadShort();
            SubAppFlags.Value = reader.ReadUInt();
            reader.Skip(8);
            Name = reader.ReadYuniversal();
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
