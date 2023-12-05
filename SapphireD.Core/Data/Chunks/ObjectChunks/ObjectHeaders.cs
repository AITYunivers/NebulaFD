using SapphireD.Core.Memory;
using System.Diagnostics;

namespace SapphireD.Core.Data.Chunks.ObjectChunks
{
    public class ObjectHeaders : Chunk
    {
        public ObjectHeaders()
        {
            ChunkName = "ObjectHeaders";
            ChunkID = 0x2253;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            FrameItems newItems = new FrameItems();
            while (reader.Size() > reader.Tell())
            {
                ObjectInfo oI = new ObjectInfo();
                oI.Header.Handle = reader.ReadShort();
                oI.Header.Type = reader.ReadShort();
                oI.Header.ObjectFlags.Value = reader.ReadUShort();
                reader.Skip(2);
                oI.Header.InkEffect = reader.ReadInt();
                oI.Header.InkEffectParam = reader.ReadUInt();
                newItems.Items.Add(oI.Header.Handle, oI);
            }
            SapDCore.PackageData.FrameItems = newItems;
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
    }
}
