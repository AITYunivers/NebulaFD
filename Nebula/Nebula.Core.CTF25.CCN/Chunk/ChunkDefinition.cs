using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.Chunk
{
    public class ChunkDefinition : IReadable
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
        }

        public virtual byte[] DecompressData(byte[] data)
        {
            switch (_compressionType)
            {
                case EChunkCompressionType.UNCOMPRESSED:
                    return data;
                case EChunkCompressionType.ZLIB:
                    _compressionType = EChunkCompressionType.UNCOMPRESSED;
                    return Decompressor.DecompressZlib(data);
                case EChunkCompressionType.XOR:
                    _compressionType = EChunkCompressionType.UNCOMPRESSED;
                    return Decryption.DecryptXor(data);
                case EChunkCompressionType.ZLIB | EChunkCompressionType.XOR:
                    _compressionType = EChunkCompressionType.UNCOMPRESSED;
                    return Decryption.DecompressXor(data, _id);
                default:
                    throw new InvalidDataException($"Unknown compression type: {(int)_compressionType}");
            }
        }

        public virtual void InitData(ByteReader reader, bool resetPosition = true)
        {
            long oldPos = reader.Tell();
            reader.Seek(_dataOffset);
            _chunkData = reader.ReadBytes(_dataSize);
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
                _chunkData = DecompressData(_chunkData);
            return _chunkData;
        }

        public ByteReader MakeReader(ByteReader reader)
        {
            InitData(reader);
            return MakeReader();
        }

        public ByteReader MakeReader()
        {
            return new ByteReader(GetData());
        }

        public ushort GetID()
        {
            return _id;
        }

        public EChunks GetChunkType()
        {
            return (EChunks)_id;
        }
    }
}
