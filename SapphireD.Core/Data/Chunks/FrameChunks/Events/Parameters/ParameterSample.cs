using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterSample : ParameterChunk
    {
        public BitDict Flags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public short Handle;
        public string Name = string.Empty;

        public ParameterSample()
        {
            ChunkName = "ParameterSample";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadShort();
            Flags.Value = reader.ReadUShort();
            Name = reader.ReadYuniversal();
        }
    }
}
