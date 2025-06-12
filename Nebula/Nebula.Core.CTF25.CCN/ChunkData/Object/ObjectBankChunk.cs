using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.CTF25.CCN.Chunks.Extensions;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.Chunks.Objects
{
    internal class ObjectBankChunk : CommonChunk
    {
        private ObjectItem[] _objectItems = [];

        public override void ReadChunkData(ByteReader reader)
        {
            int objectCount = reader.ReadInt();
            this.Log($"Found {objectCount} objects(s)", Logger.LogType.Debug);
            if (objectCount < 0)
                throw new InvalidDataException("Invalid object count. Expected greater than or equal to 0, got " + objectCount);

            _objectItems = reader.ReadIReadables<ObjectItem>(objectCount);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteInt(_objectItems.Length);
            writer.WriteIWritables(_objectItems);
        }

        public ObjectItem this[int index]
        {
            get => _objectItems[index];
            set => _objectItems[index] = value;
        }
    }
}
