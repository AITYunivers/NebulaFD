using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class ExtensionsMini : Extensions
    {
        public ExtensionsMini()
        {
            ChunkName = "ExtensionsMini";
            ChunkID = 0x2228;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Exts = new();
            ushort Count = reader.ReadUShort();
            ushort MaxHandle = reader.ReadUShort();

            for (int i = 0; i < Count; i++)
            {
                Extension ext = new Extension();
                ext.ReadCCN(reader, true);
                Exts.Add(ext.Handle, ext);
            }

            NebulaCore.PackageData.Extensions = this;
        }
    }
}
