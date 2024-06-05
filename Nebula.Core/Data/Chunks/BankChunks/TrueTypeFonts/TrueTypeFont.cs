using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.BankChunks.TrueTypeFonts
{
    public class TrueTypeFont : Chunk
    {
        public string Name = string.Empty;
        public byte[] FontData = new byte[0];

        public TrueTypeFont()
        {
            ChunkName = "TrueTypeFont";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            int decompressedSize = reader.ReadInt();
            int size = reader.ReadInt();
            FontData = Decompressor.DecompressBlock(reader, size);
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
