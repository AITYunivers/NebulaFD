using FusionRipper.Core.Memory;

namespace FusionRipper.Core.Data.Chunks.AppChunks
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

            var pkgData = FRipCore.PackageData;
            pkgData.EditorFilename = Value;

            if (FRipCore.Build > 285)
                Decryption.MakeKey(pkgData.AppName, pkgData.Copyright, Value);
            else
                Decryption.MakeKey(Value, pkgData.AppName, pkgData.Copyright);

            if (string.IsNullOrEmpty(pkgData.AppName))
                pkgData.AppName = Path.GetFileNameWithoutExtension(Value);
        }
    }
}
