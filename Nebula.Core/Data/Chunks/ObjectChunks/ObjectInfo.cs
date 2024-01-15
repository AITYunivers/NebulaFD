using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.Data.Chunks.ObjectChunks
{
    public class ObjectInfo : Chunk
    {
        public ObjectInfoHeader Header = new();
        public string Name = string.Empty;
        public ObjectInfoProperties Properties = new();
        public ObjectInfoShader Shader = new();
        
        public uint IconHandle;

        public ObjectInfo()
        {
            ChunkName = "ObjectInfo";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            while (reader.HasMemory(8))
            {
                Chunk newChunk = InitChunk(reader);
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
