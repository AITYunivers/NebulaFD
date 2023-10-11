using SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;

namespace SapphireD.Core.Data.Chunks.ObjectChunks
{
    public class ObjectInfo : Chunk
    {
        public ObjectInfoHeader Header = new();
        public string Name = string.Empty;
        public ObjectProperties Properties = new();

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            while (true)
            {
                Chunk newChunk = InitChunk(reader);
                Logger.Log(this, $"Reading Object Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})");

                ByteReader chunkReader = new ByteReader(newChunk.ChunkData!);
                newChunk.ReadCCN(chunkReader, this);
                newChunk.ChunkData = null;
                if (newChunk.ChunkID == 0x7F7F)
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
