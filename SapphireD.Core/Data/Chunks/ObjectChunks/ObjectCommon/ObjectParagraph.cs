using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectParagraph : Chunk
    {
        public BitDict Flags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public ushort FontHandle;
        public string Value = string.Empty;
        public Color Color = Color.White;

        public ObjectParagraph()
        {
            ChunkName = "ObjectParagraph";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            FontHandle = reader.ReadUShort();
            Flags.Value = reader.ReadUShort();
            Color = reader.ReadColor();
            Value = reader.ReadYuniversal();
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
