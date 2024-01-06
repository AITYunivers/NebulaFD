using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.AppChunks
{
    public class BinaryFiles : Chunk
    {
        public int Count;
        public List<BinaryFile> Items = new();

        public BinaryFiles()
        {
            ChunkName = "BinaryFiles";
            ChunkID = 0x2238;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Count = reader.ReadInt32();
            for (int i = 0; i < Count; i++)
            {
                BinaryFile item = new BinaryFile();
                item.ReadCCN(reader);
                Items.Add(item);
            }

            FRipCore.PackageData.BinaryFiles = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Count = reader.ReadInt32();
            for (int i = 0; i < Count; i++)
            {
                BinaryFile item = new BinaryFile();
                item.ReadMFA(reader);
                Items.Add(item);
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Count);
            foreach (BinaryFile item in Items)
                item.WriteMFA(writer);
        }
    }
}
