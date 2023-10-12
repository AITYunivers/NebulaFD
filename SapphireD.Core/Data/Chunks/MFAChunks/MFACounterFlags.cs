using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.MFAChunks
{
    public class MFACounterFlags : Chunk
    {
        public BitDict Flags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

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
            Flags.Value = reader.ReadByte();
            FixedDigits = reader.ReadByte();
            SignificantDigits = reader.ReadByte();
            DecimalPoints = reader.ReadByte();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
