using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;

namespace SapphireD.Core.Data.Chunks.ObjectChunks
{
    public class ObjectInfo : Chunk
    {
        public ObjectInfoHeader Header;
        public string Name;

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            while (true)
            {
                if (reader.Tell() >= reader.Size()) break;
                var newChunk = InitChunk(reader);
                Logger.Log(this, $"Reading Object Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})");

                ByteReader chunkReader = new ByteReader(newChunk.ChunkData!);
                newChunk.ReadCCN(chunkReader, this);
                newChunk.ChunkData = null;
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
