using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks
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
                // Need this for debug ccns, dunno why
                if (reader.Tell() >= reader.Size())
                {
                    Array.Resize(ref Layers, i);
                    break;
                }

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
                Layers[i].SyncFlags(true);
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            if (Layers.Length == 0)
                Layers = new FrameLayer[1] { new FrameLayer() };

            writer.WriteInt(Layers.Length);
            foreach (FrameLayer layer in Layers)
            {
                layer.SyncFlags();
                layer.WriteMFA(writer);
            }
        }
    }
}
