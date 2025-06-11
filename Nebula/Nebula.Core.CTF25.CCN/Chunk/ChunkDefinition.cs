using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Buffers;

namespace Nebula.Core.CTF25.CCN.Chunk
{
    public class ChunkDefinition : IReadable, IWritable
    {
        private ushort _id;
        private byte[]? _chunkData = null;
        private long _dataOffset;
        private int _dataSize;
        private EChunkCompressionType _compressionType = EChunkCompressionType.UNCOMPRESSED;

        public virtual void Read(ByteReader reader)
        {
            _id = reader.ReadUShort();
            _compressionType = (EChunkCompressionType)reader.ReadUShort();
            _dataSize = reader.ReadInt();
            _dataOffset = reader.Tell();
            reader.Skip(_dataSize);
        }

        public virtual void Write(ByteWriter writer)
        {
            writer.WriteUShort(_id);
            writer.WriteUShort((ushort)_compressionType);
        }

        public virtual void DecompressData()
        {
            if (_chunkData == null)
                return;

            byte[] result;
            switch (_compressionType)
            {
                case EChunkCompressionType.UNCOMPRESSED:
                    return;
                case EChunkCompressionType.ZLIB:
                    _compressionType = EChunkCompressionType.UNCOMPRESSED;
                    result = Decompressor.DecompressZlib(_chunkData);
                    break;
                case EChunkCompressionType.XOR:
                    _compressionType = EChunkCompressionType.UNCOMPRESSED;
                    Decryption.DecryptXor(_chunkData);
                    return;
                case EChunkCompressionType.ZLIB | EChunkCompressionType.XOR:
                    _compressionType = EChunkCompressionType.UNCOMPRESSED;
                    result = Decryption.DecompressXor(_chunkData, _id);
                    break;
                default:
                    throw new InvalidDataException($"Unknown compression type: {(int)_compressionType}");
            }

            ArrayPool<byte>.Shared.Return(_chunkData);
            _chunkData = ArrayPool<byte>.Shared.Rent(_dataSize = result.Length);
            Array.Copy(result, _chunkData, _dataSize);
        }

        public virtual void InitData(ByteReader reader, bool resetPosition = true)
        {
            long oldPos = reader.Tell();
            reader.Seek(_dataOffset);
            _chunkData = ArrayPool<byte>.Shared.Rent(_dataSize);
            reader.BaseStream.ReadExactly(_chunkData, 0, _dataSize);
            if (resetPosition)
                reader.Seek(oldPos);
        }

        public int GetDataSize()
        {
            return _dataSize;
        }

        public bool DataInitialized()
        {
            return _chunkData != null;
        }

        public virtual byte[] GetData()
        {
            if (_chunkData == null)
                return [];

            if (_compressionType != EChunkCompressionType.UNCOMPRESSED)
                DecompressData();
            return _chunkData;
        }

        public ByteReader MakeReader(ByteReader reader)
        {
            InitData(reader);
            return MakeReader();
        }

        public ByteReader MakeReader()
        {
            return new ByteReader(GetData(), GetDataSize());
        }

        public ushort GetID()
        {
            return _id;
        }

        public EChunks GetChunkType()
        {
            return (EChunks)_id;
        }

        public void UnloadData()
        {
            if (_chunkData != null)
            {
                ArrayPool<byte>.Shared.Return(_chunkData);
                _chunkData = null;
            }
        }
    }
}
