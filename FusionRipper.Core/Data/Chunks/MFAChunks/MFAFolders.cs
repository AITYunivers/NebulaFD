using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.MFAChunks
{
    public class MFAFolders : Chunk
    {
        public MFAFolder[] Folders = new MFAFolder[0];

        public MFAFolders()
        {
            ChunkName = "MFAFolders";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Folders = new MFAFolder[reader.ReadInt()];
            for (int i = 0; i < Folders.Length; i++)
            {
                Folders[i] = new MFAFolder();
                Folders[i].ReadMFA(reader);
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Folders.Length);
            foreach (MFAFolder folder in Folders)
                folder.WriteMFA(writer);
        }
    }
}
