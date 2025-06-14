using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core.CTF25.CCN.ChunkData.Object.Properties
{
    internal class AlterableNameData : IReadable
    {
        public string[] AlterableValueNames = [];
        public string[] AlterableFlagNames = [];
        public string[] AlterableStringNames = [];

        public void Read(ByteReader reader)
        {
            reader.Skip(2); // Size?
            ushort valueOffset = reader.ReadUShort();
            reader.Skip(2); // Maybe offset is int? Idk
            ushort flagOffset = reader.ReadUShort();
            reader.Skip(2); // Maybe offset is int? Idk
            ushort stringOffset = reader.ReadUShort();
            reader.Skip(2); // Maybe offset is int? Idk

            reader.Seek(valueOffset);
            AlterableValueNames = reader.ReadYuniversals<ushort>(reader.ReadShort(), valueOffset);

            reader.Seek(flagOffset);
            AlterableFlagNames = reader.ReadYuniversals<ushort>(reader.ReadShort(), flagOffset);

            reader.Seek(stringOffset);
            AlterableStringNames = reader.ReadYuniversals<ushort>(reader.ReadShort(), stringOffset);
        }
    }
}
