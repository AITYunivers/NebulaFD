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
        public FrameLayer[] Layers;

        public FrameLayers()
        {
            ChunkName = "FrameLayers";
            ChunkID = 0x3341;
        }

        public override void ReadCCN(ByteReader reader)
        {
            int count = reader.ReadInt();
            Layers = new FrameLayer[count];

            for (int i = 0; i < count; i++)
            {
                FrameLayer layer = new FrameLayer();
                layer.ReadCCN(reader);
                Layers[i] = layer;
            }

            Frame.curFrame.FrameLayers = this;
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
