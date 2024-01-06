using Nebula.Core.Memory;
using System.Diagnostics;
using System.Drawing;

namespace Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectCommon : ObjectInfoProperties
    {
        public BitDict ObjectFlags = new BitDict( // Object Flags
            "DisplayInFront",             // Display In Front (Unused?)
            "Background",                 // Background
            "SaveBackground",             // Save Background
            "RunBeforeFadeIn",            // Run Before Fade In
            "HasMovements",               // Has Movements
            "HasAnimations",              // Has Animations
            "TabStop",                    // Tab Stop Focus
            "WindowProcess",              // Is Window Process
            "HasAlterables",              // Has Alterable Values, Strings, and Flags
            "HasSprites",                 // Uses Images
            "InternalSaveBackground",     // Interal Save Background
            "DontFollowFrame",            // Follow the frame Disabled
            "DisplayAsBackground",        // Display as background
            "DontDestroyIfTooFar",        // Destroy object if too far from frame Disabled
            "DontInactivateIfTooFar",     // Inactivate if too far from window: No
            "InactivateIfTooFar",         // Inactivate if too far from window: Yes
            "HasText",                    // Uses Text
            "DontCreateAtStart", "", "",  // Create at start Disabled
            "DontResetFrameDuration"      // Do not reset current frame duration when the animation is modified
        );

        public BitDict NewObjectFlags = new BitDict( // New Object Flags
            "DontSaveBackground",     // Save background Disabled
            "WipeWithColor",          // Wipe with color
            "DontUseFineDetection",   // Use fine detection Disabled / Collision with Box 
            "VisibleAtStart",         // Visible at start
            "SolidObstacle",          // Obstacle Type: Obstacle
            "PlatformObstacle",       // Obstacle Type: Platform
            "LadderObstacle",         // Obstacle Type: Ladder
            "AutomaticRotations",     // Automatic Rotations Enabled
            "InitializeFlags"         // Initialize Flags
        );

        public short[] Qualifiers = new short[8];
        public string Identifier = string.Empty;
        public Color BackColor = Color.White;

        private int AnimationOffset = 0;
        private int AlterableValuesOffset = 0;
        private int AlterableStringsOffset = 0;
        private int MovementsOffset = 0;
        private int DataOffset = 0;
        private int ExtensionOffset = 0;
        private int ValueOffset = 0;
        private int TransitionInOffset = 0;
        private int TransitionOutOffset = 0;

        public ObjectAnimations ObjectAnimations = new();
        public ObjectAlterableValues ObjectAlterableValues = new();
        public ObjectAlterableStrings ObjectAlterableStrings = new();
        public ObjectMovements ObjectMovements = new();
        public ObjectParagraphs ObjectParagraphs = new();
        public ObjectCounter ObjectCounter = new();
        public ObjectFormattedText ObjectFormattedText = new();
        public ObjectSubApplication ObjectSubApplication = new();
        public ObjectExtension ObjectExtension = new();
        public ObjectValue ObjectValue = new();
        public TransitionChunk ObjectTransitionIn = new();
        public TransitionChunk ObjectTransitionOut = new();

        public ObjectCommon()
        {
            ChunkName = "ObjectCommon";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            if (((ObjectInfo)extraInfo[0]).Header.Type == 0)
            {
                new ObjectQuickBackdrop().ReadCCN(reader, extraInfo);
                return;
            }
            else if (((ObjectInfo)extraInfo[0]).Header.Type == 1)
            {
                new ObjectBackdrop().ReadCCN(reader, extraInfo);
                return;
            }

            long StartOffset = reader.Tell();

            int size = reader.ReadInt();
            reader.Skip(2);
            int OrderCheck = reader.ReadInt();
            File.WriteAllBytes("bitch.dat", reader.ReadBytes());
            reader.Seek(StartOffset + 4);
            if (NebulaCore.iOS && NebulaCore.Build >= 284)
            {
                ExtensionOffset = reader.ReadShort();
                MovementsOffset = reader.ReadShort();
                reader.Skip(2);
                ValueOffset = reader.ReadShort();
                AnimationOffset = reader.ReadShort();
                DataOffset = reader.ReadShort();
            }
            else if (NebulaCore.HTML)
            {
                reader.Skip(2);
                DataOffset = reader.ReadShort();
                reader.Skip(2);
                ValueOffset = reader.ReadShort();
                AnimationOffset = reader.ReadShort();
                MovementsOffset = reader.ReadShort();
            }
            else if (NebulaCore.Android && NebulaCore.Build >= 284)
            {
                MovementsOffset = reader.ReadShort();
                AnimationOffset = reader.ReadShort();
                reader.Skip(2);
                ValueOffset = reader.ReadShort();
                DataOffset = reader.ReadShort();
                ExtensionOffset = reader.ReadShort();
            }
            else if (NebulaCore.Build == 284 && OrderCheck == 0)
            {
                ValueOffset = reader.ReadShort();
                reader.Skip(4);
                MovementsOffset = reader.ReadShort();
                ExtensionOffset = reader.ReadShort();
                AnimationOffset = reader.ReadShort();
            }
            else if (NebulaCore.Build >= 284)
            {
                AnimationOffset = reader.ReadShort();
                MovementsOffset = reader.ReadShort();
                reader.Skip(4);
                ExtensionOffset = reader.ReadShort();
                ValueOffset = reader.ReadShort();
            }
            else
            {
                MovementsOffset = reader.ReadShort();
                AnimationOffset = reader.ReadShort();
                reader.Skip(2);
                ValueOffset = reader.ReadShort();
                DataOffset = reader.ReadShort();
                reader.Skip(2);
            }
            ObjectFlags.Value = reader.ReadUInt();
            for (int i = 0; i < 8; i++)
                Qualifiers[i] = reader.ReadShort();

            if ((NebulaCore.iOS && NebulaCore.Build >= 284) || NebulaCore.HTML)
                reader.Skip(2);
            else if (NebulaCore.Android && (NebulaCore.Build == 284 || NebulaCore.Build == 288))
                AnimationOffset = reader.ReadShort();
            else if (!NebulaCore.Android && NebulaCore.Build >= 284)
                DataOffset = reader.ReadShort();
            else
                ExtensionOffset = reader.ReadShort();

            AlterableValuesOffset = reader.ReadShort();
            AlterableStringsOffset = reader.ReadShort();
            NewObjectFlags.Value = reader.ReadUShort();
            if (NebulaCore.HTML)
                ExtensionOffset = reader.ReadShort();
            else
                reader.Skip(2);
            Identifier = reader.ReadAscii(4);
            BackColor = reader.ReadColor();
            TransitionInOffset = reader.ReadInt();
            TransitionOutOffset = reader.ReadInt();

            if (AnimationOffset > 0)
            {
                reader.Seek(StartOffset + AnimationOffset);
                ObjectAnimations.ReadCCN(reader, 0);
            }

            if (AlterableValuesOffset > 0)
            {
                reader.Seek(StartOffset + AlterableValuesOffset);
                ObjectAlterableValues.ReadCCN(reader);
            }

            if (AlterableStringsOffset > 0)
            {
                reader.Seek(StartOffset + AlterableStringsOffset);
                ObjectAlterableStrings.ReadCCN(reader);
            }

            if (MovementsOffset > 0)
            {
                reader.Seek(StartOffset + MovementsOffset);
                ObjectMovements.ReadCCN(reader);
            }

            if (DataOffset > 0)
            {
                reader.Seek(StartOffset + DataOffset);
                switch (Identifier.Substring(0, 2))
                {
                    //Text
                    case "TE":
                    case "QS":
                        ObjectParagraphs.ReadCCN(reader);
                        break;
                    //Counter
                    case "CN":
                    case "SC":
                    case "LI":
                        ObjectCounter.ReadCCN(reader);
                        break;
                    // Formatted Text
                    case "RT":
                        ObjectFormattedText.ReadCCN(reader);
                        break;
                    //Sub-Application
                    case "CC":
                        ObjectSubApplication.ReadCCN(reader);
                        break;
                }
            }

            if (ExtensionOffset > 0)
            {
                reader.Seek(StartOffset + ExtensionOffset);
                ObjectExtension.ReadCCN(reader);
            }

            if (ValueOffset > 0)
            {
                reader.Seek(StartOffset + ValueOffset);
                ObjectValue.ReadCCN(reader);
            }

            if (TransitionInOffset > 0)
            {
                reader.Seek(StartOffset + TransitionInOffset);
                ObjectTransitionIn.ReadCCN(reader);
            }

            if (TransitionOutOffset > 0)
            {
                reader.Seek(StartOffset + TransitionOutOffset);
                ObjectTransitionOut.ReadCCN(reader);
            }

            ((ObjectInfo)extraInfo[0]).Properties = this;
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
