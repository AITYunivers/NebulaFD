using Nebula.Core.Data.Chunks.ChunkTypes;
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

        public BitDict PreferenceFlags = new BitDict( // Preference Flags
            "OEPREFS_BACKSAVE",
            "OEPREFS_SCROLLINGINDEPENDANT",
            "OEPREFS_QUICKDISPLAY",
            "OEPREFS_SLEEP",
            "OEPREFS_LOADONCALL",
            "OEPREFS_GLOBAL",
            "OEPREFS_BACKEFFECTS",
            "OEPREFS_KILL",
            "OEPREFS_INKEFFECTS",
            "OEPREFS_TRANSITIONS",
            "OEPREFS_FINECOLLISIONS"
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

            long startOffset = reader.Tell();
            //File.WriteAllBytes("bitch.bin", reader.ReadBytes());
            //reader.Seek(startOffset);
            reader.Skip(6);
            bool check = reader.ReadInt() == 0;
            reader.Skip(-6);

            GetOffset(reader, 0, check);
            GetOffset(reader, 1, check);
            GetOffset(reader, 2);
            GetOffset(reader, 3, check);
            GetOffset(reader, 4);
            GetOffset(reader, 5, check);
            
            ObjectFlags.Value = reader.ReadUInt();
            for (int i = 0; i < 8; i++)
                Qualifiers[i] = reader.ReadShort();

            GetOffset(reader, 6);
            GetOffset(reader, 7);
            GetOffset(reader, 8);
            NewObjectFlags.Value = reader.ReadUShort();
            GetOffset(reader, 9);
            Identifier = reader.ReadAscii(4);
            BackColor = reader.ReadColor();
            TransitionInOffset = reader.ReadInt();
            TransitionOutOffset = reader.ReadInt();

            if (AnimationOffset > 0)
            {
                reader.Seek(startOffset + AnimationOffset);
                ObjectAnimations.ReadCCN(reader, 0);
            }

            if (AlterableValuesOffset > 0)
            {
                reader.Seek(startOffset + AlterableValuesOffset);
                ObjectAlterableValues.ReadCCN(reader);
            }

            if (AlterableStringsOffset > 0)
            {
                reader.Seek(startOffset + AlterableStringsOffset);
                ObjectAlterableStrings.ReadCCN(reader);
            }

            if (MovementsOffset > 0)
            {
                reader.Seek(startOffset + MovementsOffset);
                ObjectMovements.ReadCCN(reader);
            }

            if (DataOffset > 0)
            {
                reader.Seek(startOffset + DataOffset);
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
                reader.Seek(startOffset + ExtensionOffset);
                ObjectExtension.ReadCCN(reader);
            }

            if (ValueOffset > 0)
            {
                reader.Seek(startOffset + ValueOffset);
                ObjectValue.ReadCCN(reader);
            }

            if (TransitionInOffset > 0)
            {
                reader.Seek(startOffset + TransitionInOffset);
                ObjectTransitionIn.ReadCCN(reader);
            }

            if (TransitionOutOffset > 0)
            {
                reader.Seek(startOffset + TransitionOutOffset);
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

        List<short> Offsets = new();
        public void GetOffset(ByteReader reader, int index, bool check = false)
        {
            short Offset = reader.ReadShort();
            Offsets.Add(Offset);

            if (NebulaCore.Android)
            {
                switch (index)
                {
                    case 0:
                        MovementsOffset = Offset;
                        break;
                    case 1:
                        if (NebulaCore.Build >= 284)
                            AlterableValuesOffset = Offset;
                        else
                            AnimationOffset = Offset;
                        break;
                    case 3:
                        ValueOffset = Offset;
                        break;
                    case 4:
                        DataOffset = Offset;
                        break;
                    case 5:
                        if (NebulaCore.Build >= 284)
                            ExtensionOffset = Offset;
                        else return;
                        break;
                    case 6:
                        if (NebulaCore.Build >= 284)
                            AnimationOffset = Offset;
                        else
                            ExtensionOffset = Offset;
                        break;
                    case 7:
                        if (NebulaCore.Build >= 284)
                            return;
                        else
                            AlterableValuesOffset = Offset;
                        break;
                    case 8:
                        if (NebulaCore.Build >= 284)
                            return;
                        else
                            AlterableStringsOffset = Offset;
                        break;
                    case 9:
                        PreferenceFlags.Value = (ushort)Offset;
                        break;
                }
            }
            else if (NebulaCore.iOS)
            {
                switch (index)
                {
                    case 0:
                        if (NebulaCore.Build >= 288)
                            ExtensionOffset = Offset;
                        else
                            MovementsOffset = Offset;
                        break;
                    case 1:
                        if (NebulaCore.Build >= 288)
                            MovementsOffset = Offset;
                        else
                            AnimationOffset = Offset;
                        break;
                    case 3:
                        ValueOffset = Offset;
                        break;
                    case 4:
                        if (NebulaCore.Build >= 288)
                            AnimationOffset = Offset;
                        else
                            DataOffset = Offset;
                        break;
                    case 5:
                        if (NebulaCore.Build >= 288)
                            DataOffset = Offset;
                        else return;
                        break;
                    case 6:
                        if (NebulaCore.Build >= 288)
                            return;
                        else
                            ExtensionOffset = Offset;
                        break;
                    case 7:
                        AlterableValuesOffset = Offset;
                        break;
                    case 8:
                        AlterableStringsOffset = Offset;
                        break;
                    case 9:
                        PreferenceFlags.Value = (ushort)Offset;
                        break;
                }
            }
            else if (NebulaCore.HTML)
            {
                switch (index)
                {
                    case 1:
                        DataOffset = Offset;
                        break;
                    case 3:
                        ValueOffset = Offset;
                        break;
                    case 4:
                        AnimationOffset = Offset;
                        break;
                    case 5:
                        MovementsOffset = Offset;
                        break;
                    case 7:
                        AlterableValuesOffset = Offset;
                        break;
                    case 8:
                        AlterableStringsOffset = Offset;
                        break;
                    case 9:
                        ExtensionOffset = Offset;
                        break;
                }
            }
            else if (NebulaCore.Build >= 284)
            {
                switch (index)
                {
                    case 0:
                        if (NebulaCore.Build == 284 && check)
                            ValueOffset = Offset;
                        else
                            AnimationOffset = Offset;
                        break;
                    case 1:
                        if (NebulaCore.Build == 284 && check)
                            return;
                        else
                            MovementsOffset = Offset;
                        break;
                    case 3:
                        if (NebulaCore.Build == 284 && check)
                            MovementsOffset = Offset;
                        else return;
                        break;
                    case 4:
                        ExtensionOffset = Offset;
                        break;
                    case 5:
                        if (NebulaCore.Build == 284 && check)
                            AnimationOffset = Offset;
                        else
                            ValueOffset = Offset;
                        break;
                    case 6:
                        DataOffset = Offset;
                        break;
                    case 7:
                        AlterableValuesOffset = Offset;
                        break;
                    case 8:
                        AlterableStringsOffset = Offset;
                        break;
                    case 9:
                        PreferenceFlags.Value = (ushort)Offset;
                        break;
                }
            }
            else
            {
                switch (index)
                {
                    case 0:
                        MovementsOffset = Offset;
                        break;
                    case 1:
                        AnimationOffset = Offset;
                        break;
                    case 3:
                        ValueOffset = Offset;
                        break;
                    case 4:
                        DataOffset = Offset;
                        break;
                    case 6:
                        ExtensionOffset = Offset;
                        break;
                    case 7:
                        AlterableValuesOffset = Offset;
                        break;
                    case 8:
                        AlterableStringsOffset = Offset;
                        break;
                    case 9:
                        PreferenceFlags.Value = (ushort)Offset;
                        break;
                }
            }
        }
    }
}
