using ILGPU.IR.Types;
using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterPosition : ParameterChunk
    {
        public BitDict PositionFlags = new BitDict( // Position Flags
            "OffsetFromDirection",   // Located: In direction of Active
            "OffsetFromActionPoint", // Originating from: Action Point
            "InheritDirection",      // Orientation: In direction of Active
            "DontInheritDirection"   // Orientation: Normal
        );

        public ushort ObjectInfoParent;
        public short X;
        public short Y;
        public short Slope;
        public short Angle;
        public int Direction;
        public short TypeParent;
        public short ObjectInfoList;
        public short Layer;

        public ParameterPosition()
        {
            ChunkName = "ParameterPosition";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            ObjectInfoParent = reader.ReadUShort();
            PositionFlags.Value = reader.ReadUShort();
            X = reader.ReadShort();
            Y = reader.ReadShort();
            Slope = reader.ReadShort();
            Angle = reader.ReadShort();
            Direction = reader.ReadInt();
            TypeParent = reader.ReadShort();
            ObjectInfoList = reader.ReadShort();
            Layer = reader.ReadShort();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            if (FrameEvents.QualifierJumptable.ContainsKey(Tuple.Create(ObjectInfoParent, TypeParent)))
                ObjectInfoParent = FrameEvents.QualifierJumptable[Tuple.Create(ObjectInfoParent, TypeParent)];

            writer.WriteUShort(ObjectInfoParent);
            writer.WriteUShort((ushort)PositionFlags.Value);
            writer.WriteShort(X);
            writer.WriteShort(Y);
            writer.WriteShort(Slope);
            writer.WriteShort(Angle);
            writer.WriteInt(Direction);
            writer.WriteShort(TypeParent);
            writer.WriteShort(ObjectInfoList);
            writer.WriteShort(Layer);
        }

        public override string ToString()
        {
            string output = $"({X},{Y})";
            if (ObjectInfoParent != ushort.MaxValue)
                output += " from " + GetObjectName();
            else
                output += " layer " + (Layer + 1);
            if (PositionFlags.Value != 8)
            {
                output += " (";
                bool add = false;
                if (PositionFlags["OffsetFromActionPoint"])
                {
                    output += "action point";
                    add = true;
                }
                if (PositionFlags["OffsetFromDirection"])
                {
                    if (add)
                        output += ", ";
                    output += "located";
                    add = true;
                }
                if (PositionFlags["InheritDirection"])
                {
                    if (add)
                        output += ", ";
                    output += "oriented";
                }
                output += ")";
            }
            return output;
        }

        public ObjectInfo? GetObject()
        {
            if (Parent?.FrameEvents?.Qualifiers.Where(x => x.ObjectInfo == ObjectInfoParent).Any() == true)
                return null;
            else if (NebulaCore.MFA && Parent?.FrameEvents?.EventObjects.Count > 0)
                return NebulaCore.PackageData.FrameItems.Items[(int)Parent.FrameEvents.EventObjects[ObjectInfoParent].ItemHandle];
            else
                return NebulaCore.PackageData.FrameItems.Items[ObjectInfoParent];
        }

        public string GetObjectName()
        {
            ObjectInfo? objectInfo = GetObject();
            if (objectInfo != null)
                return objectInfo.Name;
            Qualifier[] qualifier = Parent?.FrameEvents?.Qualifiers.Where(x => x.ObjectInfo == ObjectInfoParent && x.Type == TypeParent).ToArray()!;
            if (qualifier.Length > 0)
                return GetQualifierName(qualifier.First());
            return "Unknown Object";
        }

        public string GetQualifierName(Qualifier qualifier)
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
    }
}
