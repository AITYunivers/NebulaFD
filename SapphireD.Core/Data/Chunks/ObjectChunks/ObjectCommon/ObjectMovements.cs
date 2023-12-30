using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectMovements : Chunk
    {
        public ObjectMovement[] Movements = new ObjectMovement[0];

        public ObjectMovements()
        {
            ChunkName = "ObjectMovements";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long StartOffset = reader.Tell();

            if (SapDCore.Fusion < 2.0f)
                return;

            Movements = new ObjectMovement[reader.ReadInt()];
            for (int i = 0; i < Movements.Length; i++)
            {
                long MovementOffset = reader.Tell();
                Movements[i] = new ObjectMovement();
                Movements[i].ReadCCN(reader, StartOffset);
                reader.Seek(MovementOffset + 16);
            }
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Movements = new ObjectMovement[reader.ReadInt()];
            for (int i = 0; i < Movements.Length; i++)
            {
                Movements[i] = new ObjectMovement();
                Movements[i].ReadMFA(reader);
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Movements.Length);
            foreach (ObjectMovement movement in Movements)
                movement.WriteMFA(writer);
        }
    }
}
