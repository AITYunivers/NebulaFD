using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks
{
    public class FrameLayer : Chunk
    {
        public BitDict LayerFlags = new BitDict( // Layer Flags
            "XCoefficient",                       // 
            "YCoefficient",                       // 
            "DontSaveBackground", "", "",         // Save Background Disabled
            "WrapHorizontally",                   // Wrap Horizontally
            "WrapVertically",                     // Wrap Vertically
            "PrevEffect", "", "", "",             // Same effect as previous layer
            "", "", "", "", "", "",               //
            "HiddenAtStart"                       // Visible at start Disabled
        );

        public BitDict MFALayerFlags = new BitDict( // Layer Flags
            "Visible",            // Visible
            "Locked", "",         // Locked
            "HiddenAtStart",      // Visible at start Disabled
            "DontSaveBackground", // Save Background Disabled
            "WrapHorizontally",   // Wrap Horizontally
            "WrapVertically",     // Wrap Vertically
            "PrevEffect"          // Same effect as previous layer
        );

        public float XCoefficient;
        public float YCoefficient;
        public int BackdropCount;
        public int BackdropIndex;
        public string Name = string.Empty;
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
                XCoefficient /= 9.18355E-41f;
                YCoefficient /= 9.18355E-41f;
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
            if (!fromMFA)
            {
                MFALayerFlags.Value = 1; // Default Value
                MFALayerFlags["HiddenAtStart"] = LayerFlags["HiddenAtStart"];
                MFALayerFlags["DontSaveBackground"] = LayerFlags["DontSaveBackground"];
                MFALayerFlags["WrapHorizontally"] = LayerFlags["WrapHorizontally"];
                MFALayerFlags["WrapVertically"] = LayerFlags["WrapVertically"];
                MFALayerFlags["PrevEffect"] = LayerFlags["PrevEffect"];
            }
            else
            {
                LayerFlags.Value = 16; // Default Value
                LayerFlags["DontSaveBackground"] = MFALayerFlags["DontSaveBackground"];
                LayerFlags["WrapHorizontally"] = MFALayerFlags["WrapHorizontally"];
                LayerFlags["WrapVertically"] = MFALayerFlags["WrapVertically"];
                LayerFlags["PrevEffect"] = MFALayerFlags["PrevEffect"];
                LayerFlags["HiddenAtStart"] = MFALayerFlags["HiddenAtStart"];
            }

            /*public BitDict MFALayerFlags = new BitDict( // Layer Flags
                "Visible",            // Visible
                "Locked", "",         // Locked
            );*/
        }
    }
}
