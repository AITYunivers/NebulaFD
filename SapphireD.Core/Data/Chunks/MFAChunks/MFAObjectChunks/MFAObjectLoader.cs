using SapphireD.Core.Data.Chunks.FrameChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.MFAChunks.MFAObjectChunks
{
    public class MFAObjectLoader : Chunk
    {
        public BitDict ObjectFlags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public BitDict NewObjectFlags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public Color Background = Color.White;
        public short[] Qualifiers = new short[8];
        public ObjectAlterableValues AlterableValues = new();
        public ObjectAlterableStrings AlterableStrings = new();
        public ObjectMovements Movements = new();
        public ObjectBehaviours Behaviours = new();
        public ObjectTransition TransitionIn = new();
        public ObjectTransition TransitionOut = new();

        public MFAObjectLoader()
        {
            ChunkName = "MFAObjectLoader";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            ObjectFlags.Value = reader.ReadUInt();
            NewObjectFlags.Value = reader.ReadUInt();
            Background = reader.ReadColor();
            for (int i = 0; i < Qualifiers.Length; i++)
                Qualifiers[i] = reader.ReadShort();
            reader.Skip(2);

            AlterableValues.ReadMFA(reader);
            AlterableStrings.ReadMFA(reader);
            Movements.ReadMFA(reader);
            Behaviours.ReadMFA(reader);

            if (reader.ReadByte() == 1)
                TransitionIn.ReadMFA(reader);

            if (reader.ReadByte() == 1)
                TransitionOut.ReadMFA(reader);
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
