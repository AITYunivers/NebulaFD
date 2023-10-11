using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectTransition : Chunk
    {
        public BitDict Flags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public int ID;
        public int Duration;
        public Color Color = Color.Black;

        public string FileName = string.Empty;
        public long FileOffset;

        public ObjectTransition()
        {
            ChunkName = "ObjectTransition";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long StartOffset = reader.Tell();

            reader.Skip(4);
            ID = reader.ReadInt();
            Duration = reader.ReadInt();
            Flags.Value = reader.ReadUInt();
            Color = reader.ReadColor();

            int NameOffset = reader.ReadInt();
            int DataOffset = reader.ReadInt();

            reader.Seek(StartOffset + NameOffset);
            FileName = reader.ReadYuniversal();
            FileOffset = StartOffset + DataOffset;
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
