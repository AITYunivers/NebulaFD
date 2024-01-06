using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.MFAChunks
{
    public class MFACounterAltFlags : Chunk
    {
        public BitDict CounterAltFlags = new BitDict( // Counter Flags
            "FixedDigitCount" // Fixed number of digits
        );

        public byte FixedDigits;

        public MFACounterAltFlags()
        {
            ChunkName = "MFACounterAltFlags";
            ChunkID = 0x0017;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            CounterAltFlags.Value = reader.ReadByte();
            FixedDigits = reader.ReadByte();

            (extraInfo[0] as MFAObjectInfo).CounterAltFlags = this;
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteByte((byte)ChunkID);
            ByteWriter chunkWriter = new ByteWriter(new MemoryStream());
            chunkWriter.WriteByte((byte)CounterAltFlags.Value);
            chunkWriter.WriteByte(FixedDigits);
            writer.WriteInt((int)chunkWriter.Tell());
            writer.WriteWriter(chunkWriter);
            chunkWriter.Flush();
            chunkWriter.Close();
        }
    }
}
