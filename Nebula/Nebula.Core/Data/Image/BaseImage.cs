using Nebula.Core.Memory;

namespace Nebula.Core.Data.Image
{
    public abstract class BaseImage
    {
        public uint Handle;

        // Image Metadata
        public int Width;
        public int Height;
        public EImageFlags Flags;
        public EImageType Type = EImageType.RGBA;
        public uint TransparentColor;

        public abstract byte[] GetImageData(ByteReader reader, out int dataSize);
    }
}
