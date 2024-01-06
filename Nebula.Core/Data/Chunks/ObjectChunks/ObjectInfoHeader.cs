using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.Data.Chunks.ObjectChunks
{
    public class ObjectInfoHeader : Chunk
    {
        public BitDict ObjectFlags = new BitDict( // Object Flags
            "LoadOnCall", "",           // Load on call
            "GlobalObject", "", "", "", // Global Object
            "DontCreateAtStart"         // Create at start Disabled
        );

        public int Handle;
        public int Type;
        public int InkEffect;
        public Color RGBCoeff = Color.FromArgb(0, 255, 255, 255);
        public byte BlendCoeff;
        public uint InkEffectParam;

        public ObjectInfoHeader()
        {
            ChunkName = "ObjectInfoHeader";
            ChunkID = 0x4444;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadShort();
            Type = reader.ReadShort();
            ObjectFlags.Value = reader.ReadUShort();
            reader.Skip(2);
            InkEffect = reader.ReadShort();
            reader.Skip(2);
            if (InkEffect != 1)
            {
                if (NebulaCore.D3D == 0)
                    reader.Skip(4);
                else
                {
                    var b = reader.ReadByte();
                    var g = reader.ReadByte();
                    var r = reader.ReadByte();
                    RGBCoeff = Color.FromArgb(0, r, g, b);
                    BlendCoeff = (byte)(255 - reader.ReadByte());
                }
            }
            else
                InkEffectParam = reader.ReadUInt();

            ((ObjectInfo)extraInfo[0]).Header = this;
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

        public void SyncFlags(BitDict MFAFlags)
        {
            ObjectFlags.Value = 0; // Default Value
            ObjectFlags["LoadOnCall"] = MFAFlags["LoadOnCall"];
            ObjectFlags["GlobalObject"] = MFAFlags["GlobalObject"];

            /*public BitDict ObjectFlags = new BitDict( // Object Flags
                "NoEditorSync",           // Editor synchronization: No
                "NameTypeEditorSync", "", // Editor synchronization: Same name and type
                "NoAutoUpdate"            // Auto-update Disabled
            );*/

            /*public BitDict ObjectFlags = new BitDict( // Object Flags
                "DontCreateAtStart"         // Create at start Disabled
            );*/
        }
    }
}
