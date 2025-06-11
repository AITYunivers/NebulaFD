using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.CCN.Chunks
{
    internal class ExtendedHeaderChunk : CommonChunk
    {
        public ulong Flags;
        public uint CompressionFlags;
        public short ScreenRatio;
        public short ScreenAngle;
        public short ViewMode;
        public ushort ExtraFlags;

        public override void ReadChunkData(ByteReader reader)
        {
            Flags = reader.ReadULong();
            CompressionFlags = reader.ReadUInt();
            ScreenRatio = reader.ReadShort();
            ScreenAngle = reader.ReadShort();
            ViewMode = reader.ReadShort();
            ExtraFlags = reader.ReadUShort();

            this.Log("Build Type ID: " + (byte)((Flags >> 32) & 0xFF), Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteULong(Flags);
            writer.WriteUInt(CompressionFlags);
            writer.WriteShort(ScreenRatio);
            writer.WriteShort(ScreenAngle);
            writer.WriteShort(ViewMode);
            writer.WriteUShort(ExtraFlags);
        }
    }
}
