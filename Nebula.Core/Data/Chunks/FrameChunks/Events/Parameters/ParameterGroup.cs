using Nebula.Core.Data.Chunks.MFAChunks.MFAObjectChunks;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterGroup : ParameterChunk
    {
        public BitDict GroupFlags = new BitDict( // Group Flags
            "InactiveOnStart", // Active when frame starts Disabled
            "Closed"           // Closed
        );

        public long CCNPointer;
        public long? MFAPointer;
        public short ID;
        public string Name = string.Empty;

        public List<ParameterGroupPointer> childrenPointers = new();

        public ParameterGroup()
        {
            ChunkName = "ParameterGroup";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {/*
            CCNPointer = (long)extraInfo[0];*/
            CCNPointer = reader.Tell() - 32;
            GroupFlags.Value = reader.ReadUShort();
            ID = reader.ReadShort();
            Name = reader.ReadYuniversalStop(79);
            reader.Skip(34); // Weird password check
            reader.Skip(4); // Checksum
            reader.Skip(2);

            if (NebulaCore.Plus)
                Name = "Group " + ID;
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            //MFAPointer = (long)extraInfo[0];
            writer.WriteUShort((ushort)GroupFlags.Value);
            writer.WriteShort(ID);
            writer.WriteYunicode(Name, 79);
            // Weird password check
            {
                writer.WriteLong(-4317047546775076864);
                writer.WriteLong(215584877707264);
                writer.WriteBytes(new byte[18]);
            }
            writer.WriteUInt(Checksum(Name));
            writer.WriteShort(0);

            long returnPointer = writer.Tell();
            foreach (ParameterGroupPointer child in childrenPointers)
            {
                if (child.MFAPointer != null)
                {
                    writer.Seek((long)child.MFAPointer);
                    writer.WriteInt((int)(child.MFAPointer - MFAPointer));
                }
            }
            writer.Seek(returnPointer);
        }

        private uint Checksum(string name)
        {
            uint output = 0x3939;
            foreach (char c in name)
                output += (uint)(c ^ 0x7FFF);
            return output;
        }

        public override string ToString()
        {
            return "[GROUP] " + Name;
        }
    }
}
