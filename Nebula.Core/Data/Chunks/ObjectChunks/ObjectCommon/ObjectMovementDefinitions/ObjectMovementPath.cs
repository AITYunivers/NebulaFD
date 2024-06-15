using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon.ObjectMovementDefinitions
{
    public class ObjectMovementPath : ObjectMovementDefinition
    {
        public short NodeCount;
        public short MinimumSpeed;
        public short MaximumSpeed;
        public byte Loop;
        public byte Reposition;
        public byte Reverse;
        public ObjectMovementPathNode[] PathNodes = new ObjectMovementPathNode[0];

        public ObjectMovementPath()
        {
            ChunkName = "ObjectMovementPath";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader, extraInfo);

            NodeCount = reader.ReadShort();
            MinimumSpeed = reader.ReadShort();
            MaximumSpeed = reader.ReadShort();
            Loop = reader.ReadByte();
            Reposition = reader.ReadByte();
            Reverse = reader.ReadByte();
            reader.Skip(1);

            PathNodes = new ObjectMovementPathNode[NodeCount];
            for (int i = 0; i < NodeCount; i++)
            {
                PathNodes[i] = new ObjectMovementPathNode();

                long NodeOffset = reader.Tell();
                byte NodeSize = 14;

                if (NebulaCore.Fusion > 1.5f)
                {
                    reader.Skip(1);
                    NodeSize = reader.ReadByte();
                }

                PathNodes[i].ReadCCN(reader);

                reader.Seek(NodeOffset + NodeSize);
            }
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader, extraInfo);

            NodeCount = reader.ReadShort();
            MinimumSpeed = reader.ReadShort();
            MaximumSpeed = reader.ReadShort();
            Loop = reader.ReadByte();
            Reposition = reader.ReadByte();
            Reverse = reader.ReadByte();
            reader.Skip(1);

            PathNodes = new ObjectMovementPathNode[NodeCount];
            for (int i = 0; i < NodeCount; i++)
            {
                long NodeOffset = reader.Tell();
                PathNodes[i] = new ObjectMovementPathNode();
                reader.Skip(1);
                byte NodeSize = reader.ReadByte();
                PathNodes[i].ReadCCN(reader);
                reader.Seek(NodeOffset + NodeSize);
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteShort(NodeCount);
            writer.WriteShort(MinimumSpeed);
            writer.WriteShort(MaximumSpeed);
            writer.WriteByte(Loop);
            writer.WriteByte(Reposition);
            writer.WriteByte(Reverse);
            writer.WriteByte(0);

            byte oldSize = 0;
            foreach (ObjectMovementPathNode node in PathNodes)
            {
                ByteWriter nodeWriter = new ByteWriter(new MemoryStream());
                node.WriteMFA(nodeWriter);
                writer.WriteByte(oldSize);
                oldSize = (byte)(nodeWriter.Tell() + 2);
                writer.WriteByte(oldSize);
                writer.WriteWriter(nodeWriter);
                nodeWriter.Flush();
                nodeWriter.Close();
            }
        }
    }
}
