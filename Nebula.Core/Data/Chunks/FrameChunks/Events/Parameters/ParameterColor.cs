using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterColor : ParameterChunk
    {
        public Color Color;

        public ParameterColor()
        {
            ChunkName = "ParameterColor";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Color = reader.ReadColor();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteColor(Color);
        }
    }
}
