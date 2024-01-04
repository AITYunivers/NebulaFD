using Joveler.Compression.ZLib.Checksum;
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
            Name = reader.ReadYuniversalStop(79);
            reader.Skip(34); // Weird password check
            reader.Skip(4); // Checksum
            reader.Skip(2);

            if (SapDCore.Plus)
                Name = "Group " + ID;
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteUShort((ushort)GroupFlags.Value);
            writer.WriteShort(ID);
            writer.WriteUnicode(Name, 79);
            // Weird password check
            {
                writer.WriteLong(-4317047546775076864);
                writer.WriteLong(215584877707264);
                writer.WriteBytes(new byte[18]);
            }
            writer.WriteUInt(Checksum(Name));
            writer.WriteShort(0);
        }

        private uint Checksum(string name)
        {
            uint output = 0x3939;
            foreach (char c in name)
                output += (uint)(c ^ 0x7FFF);
            return output;
        }
    }
}
