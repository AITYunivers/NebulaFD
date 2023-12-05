using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events
{
    public class Comment : Chunk
    {
        public int Handle;
        public string Value = string.Empty;

        public Comment()
        {
            ChunkName = "Comment";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadInt();
            Value = reader.ReadAutoYuniversal();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Handle);
            writer.WriteAutoYunicode(Value);
        }
    }
}
