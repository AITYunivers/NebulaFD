using Nebula.Core.Data.PackageReaders;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.MFAChunks
{
    public class MFAAppIcon : Chunk
    {
        public uint[] IconHandles = new uint[0];

        public MFAAppIcon()
        {
            ChunkName = "MFAAppIcon";
            ChunkID = 0x009A;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            IconHandles = new uint[reader.ReadInt()];
            for (int i = 0; i < IconHandles.Length; i++)
                IconHandles[i] = reader.ReadUInt();

            if (IconHandles.Length == 2)
            {
                try
                {
                    NebulaCore.CurrentReader.Icons.Add(256, ((MFAPackageData)NebulaCore.PackageData).IconBank[IconHandles[1]].GetBitmap());
                    NebulaCore.CurrentReader.Icons.Add(128, ((MFAPackageData)NebulaCore.PackageData).IconBank[IconHandles[0]].GetBitmap());
                }
                catch
                {
                    NebulaCore.CurrentReader.Icons.Add(256, ((MFAPackageData)NebulaCore.PackageData).IconBank[IconHandles[1] + 1].GetBitmap());
                    NebulaCore.CurrentReader.Icons.Add(128, ((MFAPackageData)NebulaCore.PackageData).IconBank[IconHandles[0] + 1].GetBitmap());
                }
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            ByteWriter chunkWriter = new ByteWriter(new MemoryStream());
            {
                chunkWriter.WriteInt(IconHandles.Length);
                foreach (uint handle in IconHandles)
                    chunkWriter.WriteUInt(handle);
            }

            writer.WriteByte((byte)ChunkID);
            writer.WriteInt((int)chunkWriter.Tell());
            writer.WriteWriter(chunkWriter);
            chunkWriter.Flush();
            chunkWriter.Close();
        }
    }
}
