using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.CTF25.CCN.Chunks.Objects.Properties;
using Nebula.Core.CTF25.CCN.Objects;
using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.Chunks.Objects
{
    internal class ObjectItem : IReadable, IWritable, IChunkReader
    {
        private List<IChunk> _chunks = [];
        private bool _logged;

        public void Read(ByteReader reader)
        {
            while (reader.HasMemory(8))
            {
                ChunkDefinition chunkDefinition = new ChunkDefinition();
                chunkDefinition.Read(reader);

                //this.Log($"Found Object Chunk 0x{chunkDefinition.GetID():X} known as '{chunkDefinition.GetChunkType()}'");
                ReadChunk(chunkDefinition, reader);

                if (chunkDefinition.GetChunkType() == EChunks.LAST)
                    break;
            }
        }

        public void ReadChunk(ChunkDefinition chunkDefinition, ByteReader reader)
        {
            CommonChunk? chunk = chunkDefinition.GetChunkType() switch
            {
                EChunks.OBJECT_HEADER => new ObjectHeaderChunk(),
                EChunks.OBJECT_NAME => new ObjectNameChunk(),
                EChunks.OBJECT_PROPERTIES => CreateProperties(),
                _ => null
            };

            if (chunk != null)
            {
                chunk.SetChunkDefinition(chunkDefinition);
                chunk.Read(reader);
                _chunks.Add(chunk);
            }

            if (!_logged && HasChunk<ObjectHeaderChunk>() && HasChunk<ObjectNameChunk>())
            {
                ObjectHeaderChunk objectHeader = GetFirstChunk<ObjectHeaderChunk>()!;
                ObjectNameChunk objectName = GetFirstChunk<ObjectNameChunk>()!;
                this.Log($"Object {objectHeader.Handle}: {objectName.Name}", Logger.LogType.Debug);

                _logged = true;
            }
        }

        private CommonChunk? CreateProperties()
        {
            ObjectHeaderChunk? header = GetFirstChunk<ObjectHeaderChunk>();
            if (header == null)
                return null;

            if (header.ObjectType == EObjectTypes.QUICK_BACKDROP)
                return new QuickBackdropData();
            if (header.ObjectType == EObjectTypes.BACKDROP)
                return new BackdropData();
            return new CommonPropertiesChunk(header.ObjectType);
        }

        public void Write(ByteWriter writer)
        {
            throw new NotImplementedException();
        }

        public T[] GetChunks<T>()
        {
            return [.. _chunks.Where(x => x is T).Select(x => (T)x)];
        }

        public T? GetFirstChunk<T>()
        {
            return GetChunks<T>().FirstOrDefault();
        }

        public bool HasChunk<T>()
        {
            return _chunks.Any(x => x is T);
        }
    }
}
