using ILGPU.IR.Types;
using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
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
            // added here just because without it - any other changes with qualifiers won't do anything (will still get KeyNotFoundException, means that it won't get added)
            if (Parent?.FrameEvents?.Qualifiers.Where(x => x.ObjectInfo == ObjectInfoParent).Any() == true || (NebulaCore.Windows && NebulaCore.Build >= 296 && NebulaCore.Fusion >= 2.5 &&(ObjectInfoParent & 0x8000) != 0))
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
            else
                return ObjectCommon.TryGetQualifier(this, ObjectInfoParent, ObjectInfoList);
        }
    }
}
