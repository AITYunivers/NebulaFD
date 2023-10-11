using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectCounter : Chunk
    {
        public BitDict Flags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public int Size;
        public int Width;
        public int Height;
        public short Player;
        public short DisplayType;
        public short Font;
        public int[] Frames = new int[0];
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
            DisplayType = reader.ReadShort();
            Flags.Value = reader.ReadUShort();
            Font = reader.ReadShort();

            switch (DisplayType)
            {
                case 1:
                case 4:
                case 50:
                    Frames = new int[reader.ReadShort()];
                    for (int i = 0; i < Frames.Length; i++)
                        Frames[i] = reader.ReadShort();
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
