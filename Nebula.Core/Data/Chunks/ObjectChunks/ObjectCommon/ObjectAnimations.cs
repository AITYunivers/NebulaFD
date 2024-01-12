using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectAnimations : Chunk
    {
        public Dictionary<int, ObjectAnimation> Animations = new Dictionary<int, ObjectAnimation>();

        public ObjectAnimations()
        {
            ChunkName = "ObjectAnimations";
            ChunkID = 0x4449;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long StartOffset = reader.Tell();
            reader.ReadShort();
            short Count = reader.ReadShort();

            short[] Offsets = new short[Count];
            for (int i = 0; i < Count; i++)
                Offsets[i] = reader.ReadShort();

            Animations = new Dictionary<int, ObjectAnimation>();
            for (int i = 0; i < Count; i++)
            {
                if (Offsets[i] != 0)
                {
                    reader.Seek(StartOffset + Offsets[i]);
                    ObjectAnimation anim = new ObjectAnimation();
                    anim.ReadCCN(reader);
                    Animations.Add(i, anim);
                }
            }

            if (extraInfo.Length == 0)
                NebulaCore.PackageData.ObjectAnimations = this;
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

        public ObjectAnimation GetFirst()
        {
            foreach (var animation in Animations.Values)
                if (animation.Directions.Count > 0)
                    return animation;
            return null;
        }
    }
}
