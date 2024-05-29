using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterGroupPointer : ParameterChunk
    {
        public int Pointer;
        public long PointerFull;
        public short ID;

        public ParameterGroupPointer()
        {
            ChunkName = "ParameterGroupPointer";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            PointerFull = reader.Tell();
            PointerFull += Pointer = reader.ReadInt();
            ID = reader.ReadShort();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Pointer);
            writer.WriteShort(ID);
        }

        public override string ToString()
        {
            return "Group Pointer " + Pointer + ", " + ID;
        }
    }
}
