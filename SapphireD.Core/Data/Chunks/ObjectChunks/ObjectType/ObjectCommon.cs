using SapphireD.Core.Memory;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireD.Core.Data.Chunks.ObjectChunks.ObjectType
{
    public class ObjectCommon : ObjectType
    {
        public short[] Qualifiers;
        public string Identifier;
        public Color BackColor;

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

        public ObjectCommon()
        {
            ChunkName = "ObjectCommon";
        }

        public override void ReadCCN(ByteReader reader)
        {
            Qualifiers = new short[8];

            reader.ReadInt();
            int MovementOffset = reader.ReadShort();
            int AnimationOffset = reader.ReadShort();
            reader.ReadShort();
            int CounterOffset = reader.ReadShort();
            int DataOffset = reader.ReadShort();
            reader.ReadShort();
            Flags.Value = reader.ReadUInt();
            for (int i = 0; i < 8; i++)
                Qualifiers[i] = reader.ReadShort();
            int ExtensionOffset = reader.ReadShort();
            int AlterableValuesOffset = reader.ReadShort();
            int AlterableStringsOffset = reader.ReadShort();
            NewFlags.Value = reader.ReadUShort();
            Preferences.Value = reader.ReadUShort();
            Identifier = reader.ReadAscii(4);
            BackColor = reader.ReadColor();
            int FadeInOffset = reader.ReadShort();
            int FadeOutOffset = reader.ReadShort();
        }

        public override void ReadMFA(ByteReader reader)
        {

        }

        public override void WriteCCN(ByteWriter writer)
        {

        }

        public override void WriteMFA(ByteWriter writer)
        {

        }
    }
}
