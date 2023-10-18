using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterGroup : ParameterChunk
    {
        public BitDict Flags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public short ID;
        public string Name = string.Empty;

        public ParameterGroup()
        {
            ChunkName = "ParameterGroup";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Flags.Value = reader.ReadUShort();
            ID = reader.ReadShort();
            Name = reader.ReadYuniversal();
        }
    }
}
