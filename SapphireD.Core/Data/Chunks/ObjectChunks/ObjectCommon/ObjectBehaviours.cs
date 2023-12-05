using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectBehaviours : Chunk
    {
        public ObjectBehaviour[] Behaviours = new ObjectBehaviour[0];

        public ObjectBehaviours()
        {
            ChunkName = "ObjectBehaviours";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Behaviours = new ObjectBehaviour[reader.ReadInt()];
            for (int i = 0; i < Behaviours.Length; i++)
            {
                Behaviours[i] = new ObjectBehaviour();
                Behaviours[i].ReadMFA(reader);
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Behaviours.Length);
            foreach (ObjectBehaviour behaviour in Behaviours)
                behaviour.WriteMFA(writer);
        }
    }
}
