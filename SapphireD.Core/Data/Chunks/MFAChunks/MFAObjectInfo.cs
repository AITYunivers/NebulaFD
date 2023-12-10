using SapphireD.Core.Data.Chunks.AppChunks;
using SapphireD.Core.Data.Chunks.MFAChunks.MFAObjectChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;
using Spectre.Console.Rendering;
using System.Diagnostics;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.MFAChunks
{
    public class MFAObjectInfo : Chunk
    {
        public BitDict ObjectFlags = new BitDict( // Object Flags
            "LoadOnCall", "",         // Load on call
            "GlobalObject", "",       // Global Object
            "NoEditorSync",           // Editor synchronization: No
            "NameTypeEditorSync", "", // Editor synchronization: Same name and type
            "NoAutoUpdate"            // Auto-update Disabled
        );

        public int ObjectType;
        public int Handle;
        public string Name = string.Empty;
        public bool Transparent;
        public int InkEffect;
        public uint InkEffectParameter;
        public int AntiAliasing;
        public int IconType;
        public uint IconHandle;
        public MFAObjectLoader ObjectLoader = new();

        // Chunks
        public MFACounterFlags? CounterFlags = null;   // 0x16
        public MFAObjectEffects? ObjectEffects = null; // 0x2D

        public MFAObjectInfo()
        {
            ChunkName = "MFAObjectInfo";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            ObjectType = reader.ReadInt();
            Handle = reader.ReadInt();
            Name = reader.ReadAutoYuniversal();
            Transparent = reader.ReadInt() == 1;
            InkEffect = reader.ReadInt();
            InkEffectParameter = reader.ReadUInt();
            AntiAliasing = reader.ReadInt();
            ObjectFlags.Value = reader.ReadUInt();
            IconType = reader.ReadInt();
            IconHandle = reader.ReadUInt();

            while (true)
            {
                Chunk newChunk = InitMFAChunk(reader);
                Logger.Log(this, $"Reading MFA Object Chunk 0x{newChunk.ChunkID.ToString("X")} ({newChunk.ChunkName})");

                ByteReader chunkReader = new ByteReader(newChunk.ChunkData!);
                newChunk.ReadMFA(chunkReader, this);
                newChunk.ChunkData = new byte[0];
                if (newChunk is Last)
                    break;
            }

            switch (ObjectType)
            {
                case 0: // Quick Backdrop
                    ObjectLoader = new MFAQuickBackdrop();
                    break;
                case 1: // Backdrop
                    ObjectLoader = new MFABackdrop();
                    break;
                case 2: // Active
                    ObjectLoader = new MFAActive();
                    break;
                case 3: // String
                case 4: // Question & Answer
                    ObjectLoader = new MFAString();
                    break;
                case 5: // Score
                case 6: // Lives
                case 7: // Counter
                    ObjectLoader = new MFACounter();
                    break;
                case 8: // Formatted Text
                case 9: // Sub-Application
                default:
                    ObjectLoader = new MFAExtensionObject();
                    break;
            }
            ObjectLoader.ReadMFA(reader);
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(ObjectType);
            writer.WriteInt(Handle);
            writer.WriteAutoYunicode(Name);
            writer.WriteInt(Transparent ? 1 : 0);
            writer.WriteInt(InkEffect);
            writer.WriteUInt(InkEffectParameter);
            writer.WriteInt(AntiAliasing);
            writer.WriteUInt(ObjectFlags.Value);
            writer.WriteInt(IconType);
            writer.WriteUInt(IconHandle);

            if (CounterFlags != null)
                CounterFlags.WriteMFA(writer);
            if (ObjectEffects != null)
                ObjectEffects.WriteMFA(writer);
            writer.WriteByte(0); // Last Chunk

            ObjectLoader.WriteMFA(writer);
        }

        public void SyncFlags(BitDict CCNFlags)
        {
            ObjectFlags.Value = 32; // Default Value
            ObjectFlags["LoadOnCall"] = CCNFlags["LoadOnCall"];
            ObjectFlags["GlobalObject"] = CCNFlags["GlobalObject"];

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
