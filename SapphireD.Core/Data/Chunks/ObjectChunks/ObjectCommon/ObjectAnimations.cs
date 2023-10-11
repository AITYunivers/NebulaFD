using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectAnimations : Chunk
    {
        public ObjectAnimation[] Animations = new ObjectAnimation[0];

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

            Animations = new ObjectAnimation[Count];
            for (int i = 0; i < Count; i++)
            {
                Animations[i] = new ObjectAnimation();
                if (Offsets[i] != 0)
                {
                    reader.Seek(StartOffset + Offsets[i]);
                    Animations[i].ReadCCN(reader);
                }
            }

            if (extraInfo.Length == 0)
                SapDCore.PackageData.ObjectAnimations = this;
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
