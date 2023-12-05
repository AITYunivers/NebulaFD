using SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.MFAChunks.MFAObjectChunks
{
    public class MFAActive : MFAObjectLoader
    {
        public ObjectAnimation[] Animations = new ObjectAnimation[0];

        public MFAActive()
        {
            ChunkName = "MFAActive";
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            base.ReadMFA(reader, extraInfo);

            if (reader.ReadByte() == 0) return;

            Animations = new ObjectAnimation[reader.ReadUInt()];
            for (int i = 0; i < Animations.Length; i++)
            {
                Animations[i] = new ObjectAnimation();
                Animations[i].ReadMFA(reader);
            }
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            base.WriteMFA(writer, extraInfo);

            writer.WriteByte(1);
            writer.WriteInt(Animations.Length);
            foreach (ObjectAnimation animation in Animations)
                animation.WriteMFA(writer);
        }
    }
}
