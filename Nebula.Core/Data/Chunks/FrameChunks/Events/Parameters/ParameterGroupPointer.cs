using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterGroupPointer : ParameterChunk
    {
        public int Pointer;
        public long? MFAPointer;
        public long CCNPointer;
        public short ID;

        public ParameterGroup? parentGroup;

        public ParameterGroupPointer()
        {
            ChunkName = "ParameterGroupPointer";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {/*
            CCNPointer = (long)extraInfo[0];
            CCNPointer += (Pointer = reader.ReadInt()) + 30;*/
            CCNPointer = reader.Tell();
            CCNPointer += Pointer = reader.ReadInt();
            ID = reader.ReadShort();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {/*
            MFAPointer = (long)extraInfo[0];
            if (parentGroup?.MFAPointer == null)
                writer.WriteInt(0);
            else
                writer.WriteInt((int)(MFAPointer - parentGroup.MFAPointer));*/
            writer.Write(Pointer);
            writer.WriteShort(ID);
        }

        public override string ToString()
        {
            return "Group Pointer " + Pointer + ", " + ID;
        }
    }
}
