using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.Chunk
{
    public abstract class CommonChunk : IReadable, IWritable, IChunk
    {
        public ChunkDefinition? ChunkDefinition = null;

        public void Read(ByteReader reader)
        {
            if (ChunkDefinition == null)
                return;

            ByteReader chunkReader = ChunkDefinition.MakeReader(reader);
            chunkReader.SetUnicode(reader.IsUnicode());
            ReadChunkData(chunkReader);
        }

        public void Write(ByteWriter writer)
        {
            if (ChunkDefinition == null)
                return;

            ChunkDefinition.Write(writer);
            ByteWriter chunkWriter = new ByteWriter();
            WriteChunkData(chunkWriter);

            writer.WriteInt((int)chunkWriter.Tell());
            writer.WriteWriter(chunkWriter);
        }

        public abstract void ReadChunkData(ByteReader reader);
        public abstract void WriteChunkData(ByteWriter writer);

        public void SetChunkDefinition(ChunkDefinition chunkDefinition)
        {
            ChunkDefinition = chunkDefinition;
        }

        public ChunkDefinition GetChunkDefinition()
        {
            return ChunkDefinition ?? new ChunkDefinition();
        }
    }
}
