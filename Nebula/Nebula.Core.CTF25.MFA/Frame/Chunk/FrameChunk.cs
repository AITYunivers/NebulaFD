using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.MFA.Frame.Chunk
{
    internal class FrameChunk : IReadable
    {
        private ushort _id;
        private byte[]? _chunkData = null;
        private long _dataOffset;
        private int _dataSize;

        public void Read(ByteReader reader)
        {
            _id = reader.ReadUShort();
            if (IsLastChunk())
                return;

            _dataSize = reader.ReadInt();
            _dataOffset = reader.Tell();
        }

        public void InitData(ByteReader reader, bool resetPosition = true)
        {
            long oldPos = reader.Tell();
            reader.Seek(_dataOffset);
            _chunkData = reader.ReadBytes(_dataSize);
            if (resetPosition)
                reader.Seek(oldPos);
        }

        public bool DataInitialized()
        {
            return _chunkData != null;
        }

        public byte[] GetData()
        {
            return _chunkData ?? [];
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

        public EFrameChunks GetChunkType()
        {
            return (EFrameChunks)_id;
        }

        public bool IsLastChunk()
        {
            return _id == (ushort)EFrameChunks.LAST;
        }
    }
}
