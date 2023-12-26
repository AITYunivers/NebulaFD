using SapphireD.Core.Data.Chunks.FrameChunks.LayerChunks;
using SapphireD.Core.Memory;
using SapphireD.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireD.Core.Data.Chunks.FrameChunks
{
    public class FrameLayerEffects : Chunk
    {
        public FrameLayerEffects()
        {
            ChunkName = "FrameLayerEffects";
            ChunkID = 0x3345;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            for (int i = 0; i < ((Frame)extraInfo[0]).FrameLayers.Layers.Length; i++)
            {
                if (reader.Tell() < reader.Size()) return;
                FrameLayerEffect effect = new FrameLayerEffect();
                effect.ReadCCN(reader);
                ((Frame)extraInfo[0]).FrameLayers.Layers[i].Effect = effect;
            }
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            for (int i = 0; i < ((Frame)extraInfo[0]).FrameLayers.Layers.Length; i++)
            {
                FrameLayerEffect effect = new FrameLayerEffect();
                effect.ReadMFA(reader);
                ((Frame)extraInfo[0]).FrameLayers.Layers[i].Effect = effect;
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteByte(37);
            ByteWriter chunkWriter = new ByteWriter(new MemoryStream());
            for (int i = 0; i < ((Frame)extraInfo[0]).FrameLayers.Layers.Length; i++)
                ((Frame)extraInfo[0]).FrameLayers.Layers[i].Effect.WriteMFA(chunkWriter);
            writer.WriteInt((int)chunkWriter.Tell());
            writer.WriteWriter(chunkWriter);
            chunkWriter.Flush();
            chunkWriter.Close();
        }
    }
}
