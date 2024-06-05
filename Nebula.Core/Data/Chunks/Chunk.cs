using Nebula.Core.Data.Chunks.AppChunks;
using Nebula.Core.Data.Chunks.ChunkTypes;
using Nebula.Core.Data.Chunks.MFAChunks;
using Nebula.Core.Data.Chunks.MFAChunks.MFAFrameChunks;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.Data.Chunks
{
    public abstract class Chunk
    {
        private static int chunkIndex;

        private string _chunkName = string.Empty;
        public string ChunkName { get { return _chunkName; } set { _chunkName = value; } }
        private short _chunkID;
        public short ChunkID { get { return _chunkID; } set { _chunkID = value; } }
        public int ChunkSize;
        public byte[] ChunkData = new byte[0];

        public static Chunk InitChunk(ByteReader byteReader)
        {
            short id = byteReader.ReadShort();
            short flag = byteReader.ReadShort();
            int size = byteReader.ReadInt();
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
                        if (NebulaCore.Fusion > 1.5f)
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

            if (Parameters.DumpAllChunks || Parameters.DumpUnknownChunks && !ChunkList.ChunkJumpTable.ContainsKey(id))
            {
                Directory.CreateDirectory("Chunks");
                File.WriteAllBytes($"Chunks\\Chunk-{string.Format("0x{0:X}", id)}_{Utilities.Utilities.ClearName(NebulaCore.PackageData.AppName)}.bin", newChunk.ChunkData);
            }

            if (dataReader == null)
                newChunk.Log($"Chunk data is null for chunk {newChunk.ChunkName} with flag {flag}");
            else if (dataReader.BaseStream.Length == 0 && id != 0x7F7F)
                newChunk.Log($"Chunk data is empty for chunk {newChunk.ChunkName} with flag {flag}");

            return newChunk;
        }

        public static Chunk InitMFAChunk(ByteReader byteReader)
        {
            short id = byteReader.ReadByte();
            if (id == 0) return new Last();
            int size = byteReader.ReadInt();
            var data = byteReader.ReadBytes(size);

            Chunk newChunk = MFAChunkJumpTable(id);
            newChunk.ChunkSize = size;
            newChunk.ChunkData = data;

            if (Parameters.DumpAllChunks || Parameters.DumpUnknownChunks && !MFAChunkList.ChunkJumpTable.ContainsKey(id))
            {
                Directory.CreateDirectory("Chunks");
                File.WriteAllBytes($"Chunks\\MFAChunk-{string.Format("0x{0:X}", id)}_{Utilities.Utilities.ClearName(NebulaCore.PackageData.AppName)}.bin", newChunk.ChunkData);
            }
            return newChunk;
        }

        public static Chunk InitMFAFrameChunk(ByteReader byteReader)
        {
            short id = byteReader.ReadByte();
            if (id == 0) return new Last();
            int size = byteReader.ReadInt();
            var data = byteReader.ReadBytes(size);

            Chunk newChunk = MFAFrameChunkJumpTable(id);
            newChunk.ChunkSize = size;
            newChunk.ChunkData = data;

            if (Parameters.DumpAllChunks || Parameters.DumpUnknownChunks && !MFAFrameChunkList.ChunkJumpTable.ContainsKey(id))
            {
                Directory.CreateDirectory("Chunks");
                File.WriteAllBytes($"Chunks\\MFAFrameChunk-{string.Format("0x{0:X}", id)}_{Utilities.Utilities.ClearName(NebulaCore.PackageData.AppName)}.bin", newChunk.ChunkData);
            }
            return newChunk;
        }

        public static Chunk InitMFAObjectChunk(ByteReader byteReader)
        {
            short id = byteReader.ReadByte();
            if (id == 0) return new Last();
            int size = byteReader.ReadInt();
            var data = byteReader.ReadBytes(size);

            Chunk newChunk = MFAObjectChunkJumpTable(id);
            newChunk.ChunkSize = size;
            newChunk.ChunkData = data;

            if (Parameters.DumpAllChunks || Parameters.DumpUnknownChunks && !MFAObjectChunkList.ChunkJumpTable.ContainsKey(id))
            {
                Directory.CreateDirectory("Chunks");
                File.WriteAllBytes($"Chunks\\MFAObjectChunk-{string.Format("0x{0:X}", id)}_{Utilities.Utilities.ClearName(NebulaCore.PackageData.AppName)}.bin", newChunk.ChunkData);
            }
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

        public static Chunk MFAChunkJumpTable(short id)
        {
            if (MFAChunkList.ChunkJumpTable.ContainsKey(id))
            {
                Chunk unkChunk = (Chunk)Activator.CreateInstance(MFAChunkList.ChunkJumpTable[id]);
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

        public static Chunk MFAFrameChunkJumpTable(short id)
        {
            if (MFAFrameChunkList.ChunkJumpTable.ContainsKey(id))
            {
                Chunk unkChunk = (Chunk)Activator.CreateInstance(MFAFrameChunkList.ChunkJumpTable[id]);
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

        public static Chunk MFAObjectChunkJumpTable(short id)
        {
            if (MFAObjectChunkList.ChunkJumpTable.ContainsKey(id))
            {
                Chunk unkChunk = (Chunk)Activator.CreateInstance(MFAObjectChunkList.ChunkJumpTable[id]);
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
