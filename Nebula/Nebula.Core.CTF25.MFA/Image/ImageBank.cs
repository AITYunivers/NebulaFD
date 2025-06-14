using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.MFA.Image
{
    public class ImageBank : Collection<ImageItem>, IReadable
    {
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

            foreach (ImageItem imageItem in reader.ReadIReadables<ImageItem>(imageCount))
            {
                Add(imageItem);
                this.Log($"Image {imageItem.Handle}: {imageItem.Width}x{imageItem.Height}", Logger.LogType.Debug);
            }
        }
    }
}
