using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterMovement : ParameterChunk
    {
        public short ID;
        public string Name = string.Empty;

        public ParameterMovement()
        {
            ChunkName = "ParameterMovement";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            ID = reader.ReadShort();
            Name = reader.ReadYuniversalStop(33);
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteShort(ID);
            writer.WriteYunicode(Name, 33);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name))
                Name = "Movement #" + ID;
            return $"{Name} (number {ID})";
        }
    }
}
