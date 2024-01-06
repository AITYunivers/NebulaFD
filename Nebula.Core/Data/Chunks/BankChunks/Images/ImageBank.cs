using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.Data.Chunks.BankChunks.Images
{
    public class ImageBank : Chunk
    {
        // AGMIBank
        public int GraphicMode;
        public short PaletteVersion;
        public short PaletteEntries;
        public List<Color> Palette = new();

        // ImageBank
        public int ImageCount;
        public Dictionary<uint, Image> Images = new();
        public static int LoadedImageCount;
        public static List<Task> TaskManager = new();

        public ImageBank()
        {
            ChunkName = "ImageBank";
            ChunkID = 0x6666;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            NebulaCore.PackageData.ImageBank = this;

            LoadedImageCount = 0;
            TaskManager.Clear();

            if (NebulaCore.Android || NebulaCore.iOS|| NebulaCore.Flash || NebulaCore.HTML)
            {
                reader.Skip(2);
                ImageCount = reader.ReadShort();
            }
            else
            {
                ImageCount = reader.ReadInt();
            }

            for (int i = 0; i < ImageCount; i++)
            {
                Image img = Image.NewImage();
                img.ReadCCN(reader);
                Images[img.Handle] = img;
            }

            foreach (Task task in TaskManager)
                task.Wait();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            GraphicMode = reader.ReadInt();
            PaletteVersion = reader.ReadShort();
            PaletteEntries = reader.ReadShort();
            for (int i = 0; i < PaletteEntries; i++)
                Palette.Add(reader.ReadColor());

            ImageCount = reader.ReadInt();
            for (int i = 0; i < ImageCount; i++)
            {
                Image img = Image.NewImage();
                img.ReadMFA(reader);
                Images[img.Handle] = img;
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(GraphicMode);
            writer.WriteShort(PaletteVersion);
            writer.WriteShort(PaletteEntries);
            foreach (Color col in Palette)
                writer.WriteColor(col);

            writer.WriteInt(ImageCount);
            foreach (Image img in Images.Values)
                img.WriteMFA(writer);
        }
    }
}
