using SapphireD.Core.Data.Chunks.AppChunks;
using SapphireD.Core.Data.Chunks.MFAChunks.MFAObjectChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.MFAChunks
{
    public class MFAObjectInfo : Chunk
    {
        public BitDict Flags = new BitDict(new string[]
        {
            "1", "2", "3", "4", "5"
        });

        public int ObjectType;
        public int Handle;
        public string Name = string.Empty;
        public Color Transparent;
        public int InkEffect;
        public uint InkEffectParameter;
        public int AntiAliasing;
        public int IconType;
        public int IconHandle;
        public MFAObjectLoader ObjectLoader = new();

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
            Transparent = reader.ReadColor();
            InkEffect = reader.ReadInt();
            InkEffectParameter = reader.ReadUInt();
            AntiAliasing = reader.ReadInt();
            Flags.Value = reader.ReadUInt();
            IconType = reader.ReadInt();
            IconHandle = reader.ReadInt();

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
                case 0:
                    ObjectLoader = new MFAQuickBackdrop();
                    break;
                case 1:
                    ObjectLoader = new MFABackdrop();
                    break;
                case 2:
                    ObjectLoader = new MFAActive();
                    break;
                case 3:
                    ObjectLoader = new MFAString();
                    break;
                case 7:
                    ObjectLoader = new MFACounter();
                    break;
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

        }
    }
}
