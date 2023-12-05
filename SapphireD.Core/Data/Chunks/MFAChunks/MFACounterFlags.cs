using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.MFAChunks
{
    public class MFACounterFlags : Chunk
    {
        public BitDict CounterFlags = new BitDict( // Counter Flags
            "IntFixedDigitCount",     // Fixed number of digits
            "FloatFixedWholeCount",   // Number of significant digits
            "FloatFixedDecimalCount", // Number of digits after decimal point
            "FloatPadLeft"            // Add 0's to the left
        );

        public byte FixedDigits;
        public byte SignificantDigits;
        public byte DecimalPoints;

        public MFACounterFlags()
        {
            ChunkName = "MFACounterFlags";
            ChunkID = 0x0016;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            CounterFlags.Value = reader.ReadByte();
            FixedDigits = reader.ReadByte();
            SignificantDigits = reader.ReadByte();
            DecimalPoints = reader.ReadByte();

            (extraInfo[0] as MFAObjectInfo).CounterFlags = this;
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
