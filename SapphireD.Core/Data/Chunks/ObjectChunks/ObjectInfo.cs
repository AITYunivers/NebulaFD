using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;

namespace SapphireD.Core.Data.Chunks.ObjectChunks
{
    public class ObjectInfo : Chunk
    {
        public static ObjectInfo curInfo;

        public ObjectInfoHeader Header;
        public string Name;

        public override void ReadCCN(ByteReader reader)
        {
            curInfo = this;
            while (true)
            {
                if (reader.Tell() >= reader.Size()) break;
                var newChunk = InitChunk(reader);
                string log = $"Reading Object Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})";
                Logger.Log(this, log);
                ByteReader chunkReader = new ByteReader(ChunkData);
                newChunk.ReadCCN(chunkReader);
            }
        }

        public override void ReadMFA(ByteReader reader)
        {

        }

        public override void WriteCCN(ByteWriter writer)
        {

        }

        public override void WriteMFA(ByteWriter writer)
        {

        }
    }
}
