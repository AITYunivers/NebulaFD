using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectCounter : Chunk
    {
        public int Size;
        public int Width;
        public int Height;
        public short Player;
        public uint DisplayType;

        public bool IntDigitPadding;
        public bool FloatWholePadding;
        public bool FloatDecimalPadding;
        public bool FloatPadding;

        public byte IntDigitCount;
        public byte FloatWholeCount;
        public byte FloatDecimalCount;

        public uint Font;
        public uint[] Frames = new uint[0];
        public ObjectShape Shape = new();

        public ObjectCounter()
        {
            ChunkName = "ObjectCounter";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Size = reader.ReadInt();
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            Player = reader.ReadShort();
            DisplayType = reader.ReadUShort();
            ushort digitCounts = reader.ReadUShort();
            IntDigitCount = (byte)(digitCounts & 0xF);
            FloatWholeCount = (byte)(((digitCounts & 0xF0) >> 4) + 1);
            FloatDecimalCount = (byte)((digitCounts & 0xF000) >> 12);
            IntDigitPadding = IntDigitCount > 0;
            FloatWholePadding = (digitCounts & 0x200) != 0;
            FloatDecimalPadding = (digitCounts & 0x400) != 0;
            FloatPadding = (digitCounts & 0x800) != 0;
            Font = reader.ReadUShort();

            switch (DisplayType)
            {
                case 1:
                case 4:
                case 50:
                    Frames = new uint[reader.ReadShort()];
                    for (int i = 0; i < Frames.Length; i++)
                        Frames[i] = reader.ReadUShort();
                    break;
                case 2:
                case 3:
                case 5:
                    Shape.ReadCCN(reader);
                    break;
            }
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
