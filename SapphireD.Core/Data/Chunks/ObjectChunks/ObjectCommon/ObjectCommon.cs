using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon
{
    public class ObjectCommon : ObjectProperties
    {
        public BitDict Flags = new BitDict(new string[]       // Flags
        {
            "1", "2", "3", "4", "5"
        });

        public BitDict NewFlags = new BitDict(new string[]    // New Flags
        {
            "1", "2", "3", "4", "5"
        });

        public BitDict Preferences = new BitDict(new string[] // Preferences
        {
            "1", "2", "3", "4", "5"
        });

        public short[] Qualifiers = new short[8];
        public string Identifier = string.Empty;
        public Color BackColor = Color.White;

        private int AnimationOffset = 0;
        private int AlterableValuesOffset = 0;
        private int AlterableStringsOffset = 0;
        private int MovementsOffset = 0;
        private int DataOffset = 0;
        private int ExtensionOffset = 0;
        private int ValueOffset = 0;
        private int TransitionInOffset = 0;
        private int TransitionOutOffset = 0;

        public ObjectAnimations ObjectAnimations = new();
        public ObjectAlterableValues ObjectAlterableValues = new();
        public ObjectAlterableStrings ObjectAlterableStrings = new();
        public ObjectMovements ObjectMovements = new();
        public ObjectParagraphs ObjectParagraphs = new();
        public ObjectCounter ObjectCounter = new();
        public ObjectSubApplication ObjectSubApplication = new();
        public ObjectExtension ObjectExtension = new();
        public ObjectValue ObjectValue = new();
        public ObjectTransition ObjectTransitionIn = new();
        public ObjectTransition ObjectTransitionOut = new();

        public ObjectCommon()
        {
            ChunkName = "ObjectCommon";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            if (((ObjectInfo)extraInfo[0]).Header.Type == 0)
            {
                new ObjectQuickBackdrop().ReadCCN(reader, extraInfo);
                return;
            }
            else if (((ObjectInfo)extraInfo[0]).Header.Type == 1)
            {
                new ObjectBackdrop().ReadCCN(reader, extraInfo);
                return;
            }

            long StartOffset = reader.Tell();

            reader.Skip(6);
            int OrderCheck = reader.ReadInt();
            reader.Seek(StartOffset + 4);
            if (SapDCore.Build == 284 && OrderCheck == 0)
            {
                ValueOffset = reader.ReadShort();
                reader.Skip(4);
                MovementsOffset = reader.ReadShort();
                ExtensionOffset = reader.ReadShort();
                AnimationOffset = reader.ReadShort();
            }
            else
            {
                AnimationOffset = reader.ReadShort();
                MovementsOffset = reader.ReadShort();
                reader.Skip(4);
                ExtensionOffset = reader.ReadShort();
                ValueOffset = reader.ReadShort();
            }
            Flags.Value = reader.ReadUInt();
            for (int i = 0; i < 8; i++)
                Qualifiers[i] = reader.ReadShort();
            DataOffset = reader.ReadShort();
            AlterableValuesOffset = reader.ReadShort();
            AlterableStringsOffset = reader.ReadShort();
            NewFlags.Value = reader.ReadUShort();
            Preferences.Value = reader.ReadUShort();
            Identifier = reader.ReadAscii(4);
            BackColor = reader.ReadColor();
            TransitionInOffset = reader.ReadInt();
            TransitionOutOffset = reader.ReadInt();

            if (AnimationOffset > 0)
            {
                reader.Seek(StartOffset + AnimationOffset);
                ObjectAnimations.ReadCCN(reader, 0);
            }

            if (AlterableValuesOffset > 0)
            {
                reader.Seek(StartOffset + AlterableValuesOffset);
                ObjectAlterableValues.ReadCCN(reader);
            }

            if (AlterableStringsOffset > 0)
            {
                reader.Seek(StartOffset + AlterableStringsOffset);
                ObjectAlterableStrings.ReadCCN(reader);
            }

            if (MovementsOffset > 0)
            {
                reader.Seek(StartOffset + MovementsOffset);
                ObjectMovements.ReadCCN(reader);
            }

            if (DataOffset > 0)
            {
                reader.Seek(StartOffset + DataOffset);
                switch (Identifier.Substring(0, 2))
                {
                    //Text
                    case "TE":
                        ObjectParagraphs.ReadCCN(reader);
                        break;
                    //Counter
                    case "CN":
                    case "SC":
                    case "LI":
                        ObjectCounter.ReadCCN(reader);
                        break;
                    //Sub-Application
                    case "CC":
                        ObjectSubApplication.ReadCCN(reader);
                        break;
                }
            }

            if (ExtensionOffset > 0)
            {
                reader.Seek(StartOffset + ExtensionOffset);
                ObjectExtension.ReadCCN(reader);
            }

            if (ValueOffset > 0)
            {
                reader.Seek(StartOffset + ValueOffset);
                ObjectValue.ReadCCN(reader);
            }

            if (TransitionInOffset > 0)
            {
                reader.Seek(StartOffset + TransitionInOffset);
                ObjectTransitionIn.ReadCCN(reader);
            }

            if (TransitionOutOffset > 0)
            {
                reader.Seek(StartOffset + TransitionOutOffset);
                ObjectTransitionOut.ReadCCN(reader);
            }

            ((ObjectInfo)extraInfo[0]).Properties = this;
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
