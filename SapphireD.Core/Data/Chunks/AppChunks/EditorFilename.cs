using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class EditorFilename : StringChunk
    {
        public EditorFilename()
        {
            ChunkName = "EditorFilename";
            ChunkID = 0x222E;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            base.ReadCCN(reader);

            var pkgData = SapDCore.PackageData;
            pkgData.EditorFilename = Value;

            if (SapDCore.Build > 285)
                Decryption.MakeKey(pkgData.AppName, pkgData.Copyright, Value);
            else
                Decryption.MakeKey(Value, pkgData.AppName, pkgData.Copyright);
        }
    }
}
