using SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.MFAChunks.MFAObjectChunks
{
    public class MFAActive : MFAObjectLoader
    {
        public Dictionary<int, ObjectAnimation> Animations = new Dictionary<int, ObjectAnimation>();

        public MFAActive()
        {
            ChunkName = "MFAActive";
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            base.ReadMFA(reader, extraInfo);

            if (reader.ReadByte() == 0) return;

            Animations = new Dictionary<int, ObjectAnimation>();
            uint count = reader.ReadUInt();
            for (int i = 0; i < count; i++)
            {
                ObjectAnimation anim = new ObjectAnimation();
                anim.ReadMFA(reader);
                Animations.Add(i, anim);
            }
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            base.WriteMFA(writer, extraInfo);
            if (Animations.Count == 0)
            {
                writer.WriteByte(0);
                return;
            }    
            writer.WriteByte(1);
            writer.WriteInt(Animations.Count);
            foreach (ObjectAnimation animation in Animations.Values)
                animation.WriteMFA(writer);
        }
    }
}
