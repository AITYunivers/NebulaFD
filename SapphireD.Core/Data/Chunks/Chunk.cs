using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;

namespace SapphireD.Core.Data.Chunks
{
    public abstract class Chunk
    {
        private static int chunkIndex = 0;

        private string chunkName;
        public string ChunkName { get { return chunkName; } set { chunkName = value; } }
        private short chunkID;
        public short ChunkID { get { return chunkID; } set { chunkID = value; } }
        public int ChunkSize;
        public byte[]? ChunkData;

        public static Chunk InitChunk(ByteReader byteReader)
        {
            short id = byteReader.ReadInt16();
            short flag = byteReader.ReadInt16();
            int size = byteReader.ReadInt32();
            var rawData = byteReader.ReadBytes(size);
            var dataReader = new ByteReader(rawData);
            byte[] newData = new byte[0];

            if (size > 0)
            {
                switch (flag)
                {
                    default:
                        newData = dataReader.ReadBytes(size);
                        break;
                    case 1:
                        if (SapDCore.Fusion > 1.5f)
                            newData = Decompressor.Decompress(dataReader, out var DecompressedSize1);
                        else
                            newData = Decompressor.DecompressOPF(dataReader, out var DecompressedSize2);
                        break;
                    case 2:
                        newData = dataReader.ReadBytes(size);
                        Decryption.TransformChunk(newData);
                        break;
                    case 3:
                        newData = Decryption.DecodeMode3(dataReader.ReadBytes(size), id, out var DecompressedSize3);
                        break;
                }
            }

            Chunk newChunk = ChunkJumpTable(id);
            newChunk.ChunkSize = size;
            newChunk.ChunkData = newData;

            if (!ChunkList.ChunkJumpTable.ContainsKey(id))
                File.WriteAllBytes($"Chunks\\[{chunkIndex++}] Chunk-{string.Format("0x{0:X}", id)}.bin", newChunk.ChunkData);

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

        public abstract void ReadCCN(ByteReader reader, params object[] extraInfo);
        public abstract void WriteCCN(ByteWriter writer, params object[] extraInfo);
        public abstract void ReadMFA(ByteReader reader, params object[] extraInfo);
        public abstract void WriteMFA(ByteWriter writer, params object[] extraInfo);

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
