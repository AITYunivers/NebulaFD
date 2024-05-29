using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.MFAChunks
{
    public class MFAExtraFlags : Chunk
    {
        public BitDict ExtraFlags = new BitDict(
            "", "", "", "", "", "",
            "AllowAltsForNonActives" // Allow alterable values for counters for strings
        );

        public MFAExtraFlags()
        {
            ChunkName = "MFAExtraFlags";
            ChunkID = 0x003C;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            ExtraFlags.Value = reader.ReadUInt();
            reader.ReadAutoYuniversal();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            ByteWriter chunkWriter = new ByteWriter(new MemoryStream());
            {
                chunkWriter.WriteUInt(ExtraFlags.Value);
                chunkWriter.WriteAutoYunicode("");
            }

            writer.WriteByte((byte)ChunkID);
            writer.WriteInt((int)chunkWriter.Tell());
            writer.WriteWriter(chunkWriter);
            chunkWriter.Flush();
            chunkWriter.Close();
        }
    }
}
