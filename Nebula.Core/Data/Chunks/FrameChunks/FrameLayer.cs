using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks
{
    public class FrameLayer : Chunk
    {
        private const float MobileCoefficientsScale = 9.18355E-41f;

        public BitDict LayerFlags = new BitDict(16,
            "", "",
            "DontSaveBackground", "", "",
            "WrapHorizontally",
            "WrapVertically",
            "PrevEffect", "", "", "",
            "", "", "", "", "", "",
            "HiddenAtStart"
        );

        public BitDict MFALayerFlags = new BitDict(8,
            "Visible",
            "Locked", "",
            "HiddenAtStart",
            "DontSaveBackground",
            "WrapHorizontally",
            "WrapVertically",
            "PrevEffect"
        );

        public float XCoefficient { get; set; } = 1.0f;
        public float YCoefficient { get; set; } = 1.0f;
        public int BackdropCount;
        public int BackdropIndex;
        public string Name { get; set; } = string.Empty;
        public FrameLayerEffect Effect = new FrameLayerEffect();

        public FrameLayer()
        {
            ChunkName = "FrameLayer";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            LayerFlags.Value = reader.ReadUInt();
            XCoefficient = reader.ReadFloat();
            YCoefficient = reader.ReadFloat();
            BackdropCount = reader.ReadInt();
            BackdropIndex = reader.ReadInt();
            Name = reader.ReadYuniversal();

            if (NebulaCore.Android || NebulaCore.iOS)
            {
                XCoefficient /= MobileCoefficientsScale;
                YCoefficient /= MobileCoefficientsScale;
            }
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Name = reader.ReadAutoYuniversal();
            MFALayerFlags.Value = reader.ReadUInt();
            XCoefficient = reader.ReadFloat();
            YCoefficient = reader.ReadFloat();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {
            throw new NotSupportedException("Writing FrameLayer to CCN is not supported.");
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteAutoYunicode(Name);
            writer.WriteUInt(MFALayerFlags.Value);
            writer.WriteFloat(XCoefficient);
            writer.WriteFloat(YCoefficient);
        }

        public void SyncFlags(bool fromMFA = false)
        {
            SyncFlag("HiddenAtStart", fromMFA);
            SyncFlag("DontSaveBackground", fromMFA);
            SyncFlag("WrapHorizontally", fromMFA);
            SyncFlag("WrapVertically", fromMFA);
            SyncFlag("PrevEffect", fromMFA);
        }

        private void SyncFlag(string flagName, bool fromMFA)
        {
            if (fromMFA)
                LayerFlags[flagName] = MFALayerFlags[flagName];
            else
                MFALayerFlags[flagName] = LayerFlags[flagName];
        }
    }
}
