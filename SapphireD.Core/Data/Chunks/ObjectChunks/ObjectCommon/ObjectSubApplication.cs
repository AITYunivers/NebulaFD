
using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectSubApplication : Chunk
    {
        public BitDict Options = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public int Width;
        public int Height;
        public short Version;
        public short StartFrame;
        public string Name;

        public ObjectSubApplication()
        {
            ChunkName = "ObjectSubApplication";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            reader.Skip(4);
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            Version = reader.ReadShort();
            StartFrame = reader.ReadShort();
            Options.Value = reader.ReadUInt();
            reader.Skip(8);
            Name = reader.ReadYuniversal();
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
