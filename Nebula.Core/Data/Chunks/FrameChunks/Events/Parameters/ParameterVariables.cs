using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterVariables : ParameterChunk
    {
        public BitDict ValueFlags = new BitDict( // Flags
            "Val1Exists",
            "Val1Global",
            "Val1Double",
            "",
            "Val2Exists",
            "Val2Global",
            "Val2Double",
            "",
            "Val3Exists",
            "Val3Global",
            "Val3Double",
            "",
            "Val4Exists",
            "Val4Global",
            "Val4Double"
        );
        public BitDict FlagMasks = new BitDict(); // Flag Masks
        public BitDict FlagValues = new BitDict(); // Flag Values
        public ParameterVariable[] Variables = new ParameterVariable[0];

        public ParameterVariables()
        {
            ChunkName = "ParameterVariables";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            ValueFlags.Value = reader.ReadUInt();
            FlagMasks.Value = reader.ReadUInt();
            FlagValues.Value = reader.ReadUInt();

            int Count;
            for (Count = 0; Count < 4; Count++)
                if (ValueFlags != Count * 4)
                    break;

            Variables = new ParameterVariable[Count];
            for (int i = 0; i < Count; i++)
            {
                Variables[i] = new ParameterVariable();
                Variables[i].ReadCCN(reader, ValueFlags == Count * 4 + 1, ValueFlags == Count * 4 + 2);
            }
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteUInt(ValueFlags.Value);
            writer.WriteUInt(FlagMasks.Value);
            writer.WriteUInt(FlagValues.Value);

            foreach (ParameterVariable variable in Variables)
                variable.WriteMFA(writer);
        }

        public override string ToString()
        {
            return "Parameter Variables";
        }
    }
}
