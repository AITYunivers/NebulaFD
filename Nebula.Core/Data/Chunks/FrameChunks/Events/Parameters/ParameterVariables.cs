using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterVariables : ParameterChunk
    {
        public int Flags;
        public int FlagMasks;
        public int FlagValues;
        public ParameterVariable[] Variables = new ParameterVariable[0];

        public ParameterVariables()
        {
            ChunkName = "ParameterVariables";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Flags = reader.ReadInt();
            FlagMasks = reader.ReadInt();
            FlagValues = reader.ReadInt();

            int Mask = 1;
            int Count = 0;
            for (Count = 0; Count < 4; Count++)
            {
                if ((Flags & Mask) == 0)
                    break;

                Mask <<= 4;
            }

            Variables = new ParameterVariable[Count];
            int MaskGlobal = 2;
            int MaskDouble = 4;
            for (int i = 0 ; i < Count; i++)
            {
                Variables[i] = new ParameterVariable();
                Variables[i].ReadCCN(reader, (Flags & MaskGlobal) != 0, (Flags & MaskDouble) != 0);
                MaskGlobal <<= 4;
                MaskDouble <<= 4;
            }
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Flags);
            writer.WriteInt(FlagMasks);
            writer.WriteInt(FlagValues);

            foreach (ParameterVariable variable in Variables)
                variable.WriteMFA(writer);
        }

        public override string ToString()
        {
            return "Parameter Variables";
        }
    }
}
