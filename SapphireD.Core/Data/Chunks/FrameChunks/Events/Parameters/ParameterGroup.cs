using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterGroup : ParameterChunk
    {
        public BitDict GroupFlags = new BitDict( // Group Flags
            "InactiveOnStart", // Active when frame starts Disabled
            "Closed"           // Closed
        );

        public short ID;
        public string Name = string.Empty;

        public ParameterGroup()
        {
            ChunkName = "ParameterGroup";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            GroupFlags.Value = reader.ReadUShort();
            ID = reader.ReadShort();
            Name = reader.ReadYuniversal();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteUShort((ushort)GroupFlags.Value);
            writer.WriteShort(ID);
            writer.WriteUnicode(Name, true);
        }
    }
}
