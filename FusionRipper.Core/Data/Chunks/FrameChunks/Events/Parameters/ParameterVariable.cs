using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterVariable : ParameterChunk
    {
        public int Index;
        public int Operator;
        public bool Global;
        public object Value = 0;

        public ParameterVariable()
        {
            ChunkName = "ParameterVariable";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Index = reader.ReadInt();
            Operator = reader.ReadInt();
            Global = (bool)extraInfo[0];

            if ((bool)extraInfo[1])
                Value = reader.ReadDouble();
            else
                Value = reader.ReadInt();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Index);
            writer.WriteInt(Operator);
            if (Value is double)
                writer.WriteDouble((double)Value);
            else
                writer.WriteInt((int)Value);
        }
    }
}
