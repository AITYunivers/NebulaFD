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
            if (FrameEvents.QualifierJumptable.ContainsKey(ObjectInfoParent))
                ObjectInfoParent = FrameEvents.QualifierJumptable[ObjectInfoParent];

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
                output += " from " + NebulaCore.PackageData.FrameItems.Items[ObjectInfoParent].Name;
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
    }
}
