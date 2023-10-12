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
    public class FrameLayers : Chunk
    {
        public FrameLayer[] Layers = new FrameLayer[0];

        public FrameLayers()
        {
            ChunkName = "FrameLayers";
            ChunkID = 0x3341;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Layers = new FrameLayer[reader.ReadInt()];
            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i] = new FrameLayer();
                Layers[i].ReadCCN(reader);
            }

            ((Frame)extraInfo[0]).FrameLayers = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Layers = new FrameLayer[reader.ReadInt()];
            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i] = new FrameLayer();
                Layers[i].ReadMFA(reader);
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
