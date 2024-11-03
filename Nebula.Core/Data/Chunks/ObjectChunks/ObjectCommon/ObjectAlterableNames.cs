using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectAlterableNames : Chunk
    {
        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            ObjectCommon oC = (ObjectCommon)extraInfo[0];

            long baseOffset = reader.Tell();
            reader.ReadUShort();
            ushort altValOffset = reader.ReadUShort();
            reader.ReadUShort();
            ushort altFlgOffset = reader.ReadUShort();
            reader.ReadUShort();
            ushort altStrOffset = reader.ReadUShort();
            reader.ReadUShort();

            reader.Seek(baseOffset + altValOffset);
            short altValCnt = reader.ReadShort();
            List<ushort> altValOffsets = new();
            for (int i = 0; i < altValCnt; i++)
                altValOffsets.Add(reader.ReadUShort());
            for (int i = 0; i < altValCnt; i++)
            {
                reader.Seek(baseOffset + altValOffset + altValOffsets[i]);
                oC.ObjectAlterableValues.Names[i] = reader.ReadYuniversal();
            }

            reader.Seek(baseOffset + altFlgOffset);
            short altFlgCnt = reader.ReadShort();
            List<ushort> altFlgOffsets = new();
            for (int i = 0; i < altFlgCnt; i++)
                altFlgOffsets.Add(reader.ReadUShort());
            for (int i = 0; i < altFlgCnt; i++)
            {
                reader.Seek(baseOffset + altFlgOffset + altFlgOffsets[i]);
                oC.ObjectAlterableValues.FlagNames[i] = reader.ReadYuniversal();
            }

            reader.Seek(baseOffset + altStrOffset);
            short altStrCnt = reader.ReadShort();
            List<ushort> altStrOffsets = new();
            for (int i = 0; i < altStrCnt; i++)
                altStrOffsets.Add(reader.ReadUShort());
            for (int i = 0; i < altStrCnt; i++)
            {
                reader.Seek(baseOffset + altStrOffset + altStrOffsets[i]);
                oC.ObjectAlterableStrings.Names[i] = reader.ReadYuniversal();
            }
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
