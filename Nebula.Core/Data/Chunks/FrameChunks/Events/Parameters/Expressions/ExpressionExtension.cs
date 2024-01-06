using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ExpressionExtension : ExpressionChunk
    {
        public byte[] Data;
        
        public ExpressionExtension()
        {
            ChunkName = "ExpressionExtension";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Data = reader.ReadBytes((int)extraInfo[0]);
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteBytes(Data);
        }
    }
}
