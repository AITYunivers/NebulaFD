using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.MFA.Image
{
    public class ImageBank : IReadable
    {
        private ImageItem[] _imageItems = [];

        public void Read(ByteReader reader)
        {
            string bankHeader = reader.ReadAscii(4);
            if (bankHeader != "AGMI")
                throw new InvalidDataException("Invalid image bank header. Expected AGMI, got " + bankHeader);

            int graphicMode = reader.ReadInt();
            int pVersion = reader.ReadShort();
            int pEntries = reader.ReadShort();
            reader.Skip(pEntries * 4); // Color Palette

            int imageCount = reader.ReadInt();
            this.Log($"Found {imageCount} image(s)", Logger.LogType.Debug);
            if (imageCount < 0)
                throw new InvalidDataException("Invalid image count. Expected greater than or equal to 0, got " + imageCount);

            _imageItems = new ImageItem[imageCount];
            for (int i = 0; i < imageCount; i++)
            {
                ImageItem imageItem = new ImageItem();
                imageItem.Read(reader);
                _imageItems[i] = imageItem;

                this.Log($"Image {imageItem.Handle}: {imageItem.Width}x{imageItem.Height}", Logger.LogType.Debug);
            }
        }

        public ImageItem this[int index]
        {
            get => _imageItems[index];
            set => _imageItems[index] = value;
        }
    }
}
