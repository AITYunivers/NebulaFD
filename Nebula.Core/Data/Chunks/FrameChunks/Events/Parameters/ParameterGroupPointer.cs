using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterGroupPointer : ParameterChunk
    {
        public long CCNPointer;
        public int Pointer;
        public int ID;

        public ParameterGroupPointer()
        {
            ChunkName = "ParameterGroupPointer";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Pointer = reader.ReadInt();
            ID = reader.ReadInt();

            CCNPointer = reader.Tell() - 14 + Pointer;
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(0); // Pointer goes unused by the Editor
            writer.WriteInt(ID);
        }

        public override string ToString()
        {
            return "Group Pointer " + Pointer + ", " + ID;
        }
    }
}
