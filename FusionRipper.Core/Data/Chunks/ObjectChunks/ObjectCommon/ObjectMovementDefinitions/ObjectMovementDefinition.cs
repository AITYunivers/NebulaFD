using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.ObjectChunks.ObjectCommon.ObjectMovementDefinitions
{
    public class ObjectMovementDefinition : Chunk
    {
        public short Control;
        public short Type;
        public byte Move;
        public byte Opt;
        public int StartingDirection;

        public ObjectMovementDefinition()
        {
            ChunkName = "ObjectMovementDefinition";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Control = (short)extraInfo[1];
            Type = (short)extraInfo[2];
            Move = (byte)extraInfo[3];
            Opt = (byte)extraInfo[4];
            StartingDirection = (int)extraInfo[5];
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Control = (short)extraInfo[1];
            Type = (short)extraInfo[2];
            Move = (byte)extraInfo[3];
            Opt = (byte)extraInfo[4];
            StartingDirection = (int)extraInfo[5];
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
