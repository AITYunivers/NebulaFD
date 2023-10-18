using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterFile : ParameterChunk
    {
        public BitDict Flags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public string FileName = string.Empty;
        public string Command = string.Empty;

        public ParameterFile()
        {
            ChunkName = "ParameterFile";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Flags.Value = reader.ReadUShort();
            FileName = reader.ReadAscii(260);
            Command = reader.ReadAscii();
        }
    }
}
