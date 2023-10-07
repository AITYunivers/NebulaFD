using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;

namespace SapphireD.Core.Data.Chunks
{
    public abstract class Chunk
    {
        private string chunkName;
        public string ChunkName { get { return chunkName; } set { chunkName = value; } }
        private short chunkID;
        public short ChunkID { get { return chunkID; } set { chunkID = value; } }
        public int ChunkSize;
        public static byte[] ChunkData;

        public static Chunk InitChunk(ByteReader byteReader)
        {
            short id = byteReader.ReadInt16();
            short flag = byteReader.ReadInt16();
            int size = byteReader.ReadInt32();
            var rawData = byteReader.ReadBytes(size);
            var dataReader = new ByteReader(rawData);

            switch (flag)
            {
                default:
                    ChunkData = dataReader.ReadBytes(size);
                    break;
                case 1:
                    ChunkData = Decompressor.Decompress(dataReader, out var DecompressedSize);
                    break;
                case 2:
                    ChunkData = dataReader.ReadBytes(size);
                    Decryption.TransformChunk(ChunkData);
                    break;
                case 3:
                    ChunkData = Decryption.DecodeMode3(dataReader.ReadBytes(size), id, out DecompressedSize);
                    break;
            }

            Chunk newChunk = ChunkJumpTable(id);
            newChunk.ChunkSize = size;

            if (dataReader == null)
                Logger.Log(newChunk, $"Chunk data is null for chunk {newChunk.ChunkName} with flag {flag}");
            else if (dataReader.BaseStream.Length == 0 && id != 32639)
                Logger.Log(newChunk, $"Chunk data is empty for chunk {newChunk.ChunkName} with flag {flag}");

            return newChunk;
        }

        public void WriteChunk(ByteWriter byteWriter)
        {
            throw new NotImplementedException();
        }

        public abstract void ReadCCN(ByteReader reader);
        public abstract void WriteCCN(ByteWriter writer);
        public abstract void ReadMFA(ByteReader reader);
        public abstract void WriteMFA(ByteWriter writer);

        public static Chunk ChunkJumpTable(short id)
        {
            if (ChunkList.ChunkJumpTable.ContainsKey(id))
            {
                Chunk unkChunk = (Chunk)Activator.CreateInstance(ChunkList.ChunkJumpTable[id]);
                unkChunk.ChunkID = id;
                return unkChunk;
            }
            else
            {
                Chunk unkChunk = new UnknownChunk();
                unkChunk.ChunkID = id;
                return unkChunk;
            }
        }
    }
}
