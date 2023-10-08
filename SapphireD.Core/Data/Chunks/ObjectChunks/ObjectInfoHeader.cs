using SapphireD.Core.Data.Chunks.FrameChunks;
using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks
{
    public class ObjectInfoHeader : Chunk
    {
        public short Handle;
        public short Type;
        public short Flags;
        public int InkEffect;
        public int InkEffectParam;

        public ObjectInfoHeader()
        {
            ChunkName = "ObjectInfoHeader";
            ChunkID = 0x4444;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadShort();
            Type = reader.ReadShort();
            Flags = reader.ReadShort();
            reader.ReadShort();
            InkEffect = reader.ReadInt();
            InkEffectParam = reader.ReadInt();

            ((ObjectInfo)extraInfo[0]).Header = this;
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
