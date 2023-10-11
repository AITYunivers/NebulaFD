using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon.ObjectMovementDefinitions
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
                long NodeOffset = reader.Tell();
                PathNodes[i] = new ObjectMovementPathNode();
                reader.Skip(1);
                byte NodeSize = reader.ReadByte();
                PathNodes[i].ReadCCN(reader);
                reader.Seek(NodeOffset + NodeSize);
            }
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
