using Nebula.Core.Data.Chunks.ChunkTypes;
using Nebula.Core.Data.Chunks.FrameChunks.Events;
using Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters;
using Nebula.Core.Memory;
using System.Diagnostics;
using System.Drawing;
using Nebula.Core.Data.Chunks.AppChunks;

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
        private int AlterableNamesOffset = 0;

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
        public ObjectAlterableNames ObjectAlterableNames = new();

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
            reader.Skip(6);
            bool check = reader.ReadInt() == 0;
            reader.Skip(-6);

            GetOffset(reader, 0, check);
            GetOffset(reader, 1, check);
            GetOffset(reader, 2);
            GetOffset(reader, 3, check);
            GetOffset(reader, 4);
            GetOffset(reader, 5, check);
            
            if (NebulaCore.Fusion == 1.5f)
            {
                reader.Skip(2);
                ObjectFlags.Value = reader.ReadUShort();
            }
            else
                ObjectFlags.Value = reader.ReadUInt();

            for (int i = 0; i < 8; i++)
                Qualifiers[i] = reader.ReadShort();

            GetOffset(reader, 6);
            GetOffset(reader, 7);
            if (NebulaCore.Fusion > 1.5f)
                GetOffset(reader, 8);
            NewObjectFlags.Value = reader.ReadUShort();
            GetOffset(reader, 9);
            Identifier = reader.ReadAscii(4);
            BackColor = reader.ReadColor();
            TransitionInOffset = reader.ReadInt();
            TransitionOutOffset = reader.ReadInt();
            GetOffset(reader, 10);

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
                    // Text
                    case "TE":
                    case "QS":
                        ObjectParagraphs.ReadCCN(reader);
                        break;
                    // Counter
                    case "CN":
                    case "SC":
                    case "LI":
                        ObjectCounter.ReadCCN(reader);
                        break;
                    // Formatted Text
                    case "RT":
                        ObjectFormattedText.ReadCCN(reader);
                        break;
                    // Sub-Application
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

            if (AlterableNamesOffset > 0)
            {
                reader.Seek(startOffset + AlterableNamesOffset);
                ObjectAlterableNames.ReadCCN(reader, this);
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

        // method for getting qualifiers in ACE
        public static string TryGetQualifierACE(ACEventBase ace, ushort ObjectInfo, short ObjectType)
        {
            if (ace.Parent?.Parent.Qualifiers.Where(q => q.ObjectInfo == ObjectInfo && q.Type == ObjectType).Any() != true)
            {
                if ((ObjectInfo & 0x8000) != 0)
                    ace.Parent?.Parent.Qualifiers.Add(new Qualifier() { ObjectInfo = ObjectInfo, Type = ObjectType}); 
            }
            Qualifier[] qualifiers = ace.Parent?.Parent.Qualifiers.Where(x => x.ObjectInfo == ObjectInfo && x.Type == ObjectType).ToArray()!;
            if (qualifiers.Length > 0)
                return GetQualifierName(qualifiers.First());
            return "Unknown Object";
        }

        // method for getting qualifiers in different places
        public static string TryGetQualifier(ParameterChunk paramChunk, ushort ObjectInfo, short ObjectType)
        {
            if (paramChunk.Parent?.FrameEvents?.Qualifiers.Where(q => q.ObjectInfo == ObjectInfo && q.Type == ObjectType).Any() != true)
            {
                if ((ObjectInfo & 0x8000) != 0)
                    paramChunk.Parent?.FrameEvents?.Qualifiers.Add(new Qualifier() { ObjectInfo = ObjectInfo, Type = ObjectType });
            }
            Qualifier[] qualifiers = paramChunk.Parent?.FrameEvents?.Qualifiers.Where(x => x.ObjectInfo == ObjectInfo && x.Type == ObjectType).ToArray()!;
            if (qualifiers.Length > 0)
                return GetQualifierName(qualifiers.First());
            return "Unknown Object";
        }

        public static string GetQualifierName(Qualifier qualifier)
        {
            string qualifierName = "Group." + (qualifier.ObjectInfo & 0x7FFF) switch
            {
                0 => "Player",
                1 => "Good",
                2 => "Neutral",
                3 => "Bad",
                4 => "Enemies",
                5 => "Friends",
                6 => "Bullets",
                7 => "Arms",
                8 => "Bonus",
                9 => "Collectables",
                10 => "Traps",
                11 => "Doors",
                12 => "Keys",
                13 => "Texts",
                14 => "0",
                15 => "1",
                16 => "2",
                17 => "3",
                18 => "4",
                19 => "5",
                20 => "6",
                21 => "7",
                22 => "8",
                23 => "9",
                24 => "Parents",
                25 => "Children",
                26 => "Data",
                27 => "Timed",
                28 => "Engine",
                29 => "Areas",
                30 => "Reference Points",
                31 => "Radar Enemies",
                32 => "Radar Friends",
                33 => "Radar Neutrals",
                34 => "Music",
                35 => "Sound",
                36 => "Waveform",
                37 => "Background Scenery",
                38 => "Foreground Scenery",
                39 => "Decorations",
                40 => "Water",
                41 => "Clouds",
                42 => "Empty",
                43 => "Fog",
                44 => "Flowers",
                45 => "Animals",
                46 => "Bosses",
                47 => "NPC",
                48 => "Vehicles",
                49 => "Rockets",
                50 => "Balls",
                51 => "Bombs",
                52 => "Explosions",
                53 => "Particles",
                54 => "Clothes",
                55 => "Glow",
                56 => "Arrows",
                57 => "Buttons",
                58 => "Cursors",
                59 => "Drawing Tools",
                60 => "Indicator",
                61 => "Shapes",
                62 => "Shields",
                63 => "Shifting Blocks",
                64 => "Magnets",
                65 => "Negative Matter",
                66 => "Neutral Matter",
                67 => "Positive Matter",
                68 => "Breakable",
                69 => "Dissolving",
                70 => "Dialogue",
                71 => "HUD",
                72 => "Inventory",
                73 => "Inventory Item",
                74 => "Interface",
                75 => "Movable",
                76 => "Perspective",
                77 => "Calculation Objects",
                78 => "Invisible",
                79 => "Masks",
                80 => "Obstacles",
                81 => "Value Holder",
                82 => "Helpful",
                83 => "Powerups",
                84 => "Targets",
                85 => "Trapdoors",
                86 => "Dangers",
                87 => "Forbidden",
                88 => "Physical objects",
                89 => "3D Objects",
                90 => "Generic 1",
                91 => "Generic 2",
                92 => "Generic 3",
                93 => "Generic 4",
                94 => "Generic 5",
                95 => "Generic 6",
                96 => "Generic 7",
                97 => "Generic 8",
                98 => "Generic 9",
                99 => "Generic 10",
                _ => string.Empty
            };

            qualifierName += "." + qualifier.Type switch
            {
                2 => "Sprite",
                3 => "Text",
                4 => "Question",
                5 => "Score",
                6 => "Lives",
                7 => "Counter",
                _ => string.Empty
            };

            if (qualifier.Type >= 32 && NebulaCore.PackageData.Extensions.Exts.ContainsKey(qualifier.Type - 32))
            {
                Extension ext = NebulaCore.PackageData.Extensions.Exts[qualifier.Type - 32];
                qualifierName += ext.Name;
            }

            if (qualifierName.StartsWith("Group.."))
                return "Unknown Qualifier";
            else
                return qualifierName;
        }

        public void GetOffset(ByteReader reader, int index, bool check = false)
        {
            ushort Offset = reader.ReadUShort();

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
                    case 10:
						AlterableNamesOffset = Offset;
                        break;
                }
            }
            else if (NebulaCore.iOS)
            {
                switch (index)
                {
                    case 0:
                        if (NebulaCore.Build >= 284)
                            ExtensionOffset = Offset;
                        else
                            MovementsOffset = Offset;
                        break;
                    case 1:
                        if (NebulaCore.Build >= 284)
                            MovementsOffset = Offset;
                        else
                            AnimationOffset = Offset;
                        break;
                    case 3:
                        ValueOffset = Offset;
                        break;
                    case 4:
                        if (NebulaCore.Build >= 284)
                            AnimationOffset = Offset;
                        else
                            DataOffset = Offset;
                        break;
                    case 5:
                        if (NebulaCore.Build >= 284)
                            DataOffset = Offset;
                        else return;
                        break;
                    case 6:
                        if (NebulaCore.Build >= 284)
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
					case 10:
						AlterableNamesOffset = Offset;
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
					case 10:
						AlterableNamesOffset = Offset;
						break;
				}
            }
            else if (NebulaCore.Fusion == 1.5f)
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
                    case 9:
                        PreferenceFlags.Value = (ushort)Offset;
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
					case 10:
						AlterableNamesOffset = Offset;
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
					case 10:
						AlterableNamesOffset = Offset;
						break;
				}
            }
        }
    }
}
