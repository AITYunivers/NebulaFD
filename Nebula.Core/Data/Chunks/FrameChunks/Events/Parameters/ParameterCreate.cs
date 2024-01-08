using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterCreate : ParameterChunk
    {
        public BitDict CreateFlags = new BitDict(
            "OffsetFromDirection",   // Located: In direction of Active
            "OffsetFromActionPoint", // Originating from: Action Point
            "InheritDirection",      // Orientation: In direction of Active
            "DontInheritDirection"   // Orientation: Normal
        );

        public short ObjectInfoParent;
        public short X;
        public short Y;
        public short Slope;
        public short Angle;
        public int Direction;
        public short TypeParent;
        public short ObjectInfoList;
        public short Layer;

        public ushort ObjectInstances;
        public ushort ObjectInfo;

        public ParameterCreate()
        {
            ChunkName = "ParameterCreate";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            ObjectInfoParent = reader.ReadShort();
            CreateFlags.Value = reader.ReadUShort();
            X = reader.ReadShort();
            Y = reader.ReadShort();
            Slope = reader.ReadShort();
            Angle = reader.ReadShort();
            Direction = reader.ReadInt();
            TypeParent = reader.ReadShort();
            ObjectInfoList = reader.ReadShort();
            Layer = reader.ReadShort();

            ObjectInstances = reader.ReadUShort();
            ObjectInfo = reader.ReadUShort();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteShort(ObjectInfoParent);
            writer.WriteUShort((ushort)CreateFlags.Value);
            writer.WriteShort(X);
            writer.WriteShort(Y);
            writer.WriteShort(Slope);
            writer.WriteShort(Angle);
            writer.WriteInt(Direction);
            writer.WriteShort(TypeParent);
            writer.WriteShort(ObjectInfoList);
            writer.WriteShort(Layer);

            writer.WriteUShort(ObjectInstances);
            writer.WriteUShort(ObjectInfo);
        }

        public override string ToString()
        {
            string output = $"{NebulaCore.PackageData.FrameItems.Items[ObjectInfo].Name} at ({X},{Y})";
            if (ObjectInfoParent != -1)
                output += " from " + NebulaCore.PackageData.FrameItems.Items[ObjectInfoParent].Name;
            else
                output += " layer " + (Layer + 1);
            if (CreateFlags.Value != 8)
            {
                output += " (";
                bool add = false;
                if (CreateFlags["OffsetFromActionPoint"])
                {
                    output += "action point";
                    add = true;
                }
                if (CreateFlags["OffsetFromDirection"])
                {
                    if (add)
                        output += ", ";
                    output += "located";
                    add = true;
                }
                if (CreateFlags["InheritDirection"])
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
