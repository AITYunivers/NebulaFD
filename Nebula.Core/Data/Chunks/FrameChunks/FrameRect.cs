using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks
{
    public class FrameRect : Chunk
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public FrameRect()
        {
            ChunkName = "FrameRect";
            ChunkID = 0x3342;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Left = reader.ReadInt();
            Top = reader.ReadInt();
            Right = reader.ReadInt();
            Bottom = reader.ReadInt();

            ((Frame)extraInfo[0]).FrameRect = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Left = reader.ReadInt();
            Top = reader.ReadInt();
            Right = reader.ReadInt();
            Bottom = reader.ReadInt();

            ((Frame)extraInfo[0]).FrameRect = this;
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteByte(0x21);
            writer.WriteInt(16);
            writer.WriteInt(Left);
            writer.WriteInt(Top);
            writer.WriteInt(Right);
            writer.WriteInt(Bottom);
        }
    }
}
