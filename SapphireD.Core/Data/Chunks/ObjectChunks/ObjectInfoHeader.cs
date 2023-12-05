using SapphireD.Core.Memory;
using System.Security.AccessControl;

namespace SapphireD.Core.Data.Chunks.ObjectChunks
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
            reader.ReadShort();
            InkEffect = reader.ReadInt();
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
