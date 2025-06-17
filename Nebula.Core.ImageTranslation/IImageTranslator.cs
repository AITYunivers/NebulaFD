using Nebula.Core.Data.Image;

namespace Nebula.Core.ImageTranslation
{
    // Idk if I'll use this, gonna try using Static classes first
    public interface IImageTranslator
    {
        public void Translate(Stream dataStream, BaseImage baseImage, Span<byte> result);
    }
}
