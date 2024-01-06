using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.Data.Chunks.MFAChunks.MFAObjectChunks
{
    public class MFASubApplication : MFAObjectLoader
    {
        public BitDict SubAppFlags = new BitDict( // Sub-Application Flags
            "ShareGlobalValues",     // Share with parent application: Global values & strings
            "SharePlayerLives",      // Share with parent application: Lives
            "SharePlayerScores",     // Share with parent application: Scores
            "ShareWindowAttributes", // Share Windows Attributes (Unused?)
            "Stretch",               // Stretch frame to object size
            "Popup",                 // Popup Window
            "Caption",               // Caption
            "ToolCaption",           // Tool Caption
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
            "Docked",                // Docked
            "MFACheck",              // MFA Check
            "DockedVertical",        // Docked Top
            "DockedHorizontal",      // Docked Right
            "Reopen",                // Reopen (Unused?)
            "Sprite",                // Display as sprite
            "IgnoreResize"           // Windows: ignore parent's 'Resize Display' option
        );

        public string Name = string.Empty;
        public int Width;
        public int Height;
        public int StartFrame;
        public int WindowIcon;

        public MFASubApplication()
        {
            ChunkName = "MFASubApplication";
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            base.ReadMFA(reader, extraInfo);

            Name = reader.ReadAutoYuniversal();
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            SubAppFlags.Value = reader.ReadUInt();
            if (SubAppFlags["Internal"])
                StartFrame = reader.ReadInt();
            reader.Skip(4);
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            base.WriteMFA(writer, extraInfo);

            writer.WriteAutoYunicode(Name);
            writer.WriteInt(Width);
            writer.WriteInt(Height);
            writer.WriteUInt(SubAppFlags.Value);
            if (SubAppFlags["Internal"])
                writer.WriteInt(StartFrame);
            writer.WriteInt(-1);
        }
    }
}
