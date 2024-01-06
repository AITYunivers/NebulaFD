using Nebula.Core.FileReaders;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks
{
    public class FrameMosaicTable : IntChunk
    {
        public static List<short> Handles = new();
        public static List<short> PosXs = new();
        public static List<short> PosYs = new();

        public FrameMosaicTable()
        {
            ChunkName = "FrameMosaicTable";
            ChunkID = 0x3348;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            if (NebulaCore.CurrentReader is not OpenFileReader) return;

            int count = (int)reader.Size() / 6;
            for (int i = 0; i < count; i++)
            {
                if (i < Handles.Count && Handles[i] == 0)
                {
                    Handles[i] = reader.ReadShort();
                    PosXs[i] = reader.ReadShort();
                    PosYs[i] = reader.ReadShort();
                }
                else
                {
                    Handles.Add(reader.ReadShort());
                    PosXs.Add(reader.ReadShort());
                    PosYs.Add(reader.ReadShort());
                }
            }
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }
    }
}
