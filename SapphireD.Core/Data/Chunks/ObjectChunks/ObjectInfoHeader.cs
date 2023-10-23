using SapphireD.Core.Data.Chunks.FrameChunks;
using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks
{
    public class ObjectInfoHeader : Chunk
    {
        public BitDict Flags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public int Handle;
        public int Type;
        public int InkEffect;
        public uint InkEffectParam;

        public ObjectInfoHeader()
        {
            ChunkName = "ObjectInfoHeader";
            ChunkID = 0x4444;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadShort();
            Type = reader.ReadShort();
            Flags.Value = reader.ReadUShort();
            reader.ReadShort();
            InkEffect = reader.ReadInt();
            InkEffectParam = reader.ReadUInt();

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
