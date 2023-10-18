using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events
{
    public class Condition : Chunk
    {
        public BitDict Flags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public BitDict OtherFlags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public short ObjectType;
        public short Num;
        public short ObjectInfo;
        public short ObjectInfoList;
        public Parameter[] Parameters = new Parameter[0];
        public byte DefType;
        public short Identifier;

        public Condition()
        {
            ChunkName = "Condition";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long endPosition = reader.Tell() + reader.ReadUShort();

            ObjectType = reader.ReadShort();
            Num = reader.ReadShort();
            ObjectInfo = reader.ReadShort();
            ObjectInfoList = reader.ReadShort();
            Flags.Value = reader.ReadByte();
            OtherFlags.Value = reader.ReadByte();
            Parameters = new Parameter[reader.ReadByte()];
            DefType = reader.ReadByte();
            Identifier = reader.ReadShort();

            for (int i = 0; i < Parameters.Length; i++)
            {
                Parameters[i] = new Parameter();
                Parameters[i].ReadCCN(reader);
            }

            reader.Seek(endPosition);
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            ReadCCN(reader, extraInfo);
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
