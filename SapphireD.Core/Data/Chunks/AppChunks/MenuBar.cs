using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class MenuBar : Chunk
    {
        public List<MenuItem> Items = new();
        public List<byte> AccelShift = new();
        public List<short> AccelKey = new();
        public List<short> AccelId = new();

        public MenuBar()
        {
            ChunkName = "MenuBar";
            ChunkID = 0x2226;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            reader.Skip(4);
            int menuOffset = reader.ReadInt32();
            int menuSize = reader.ReadInt32();
            int accelOffset = reader.ReadInt32();
            int accelSize = reader.ReadInt32();

            reader.Seek(menuOffset);
            reader.Skip(4);
            if (menuSize > 0)
                Items = ReadMenuItems(reader);

            reader.Seek(accelOffset);
            for (int i = 0; i < accelSize / 8; i++)
            {
                AccelShift.Add(reader.ReadByte());
                reader.Skip(1);
                AccelKey.Add(reader.ReadInt16());
                AccelId.Add(reader.ReadInt16());
                reader.Skip(2);
            }

            SapDCore.PackageData.MenuBar = this;
        }

        public List<MenuItem> ReadMenuItems(ByteReader reader)
        {
            List<MenuItem> menuItems = new List<MenuItem>();
            while (true)
            {
                MenuItem menuItem = new MenuItem();
                menuItem.ReadCCN(reader);

                if (ByteFlag.GetFlag(menuItem.Flags, 4))
                    menuItem.Items = ReadMenuItems(reader);

                menuItems.Add(menuItem);
                if (ByteFlag.GetFlag(menuItem.Flags, 7))
                    break;
            }
            return menuItems;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            long start = reader.Tell();
            int size = reader.ReadInt();
            reader.Skip(4);
            int menuOffset = reader.ReadInt32();
            int menuSize = reader.ReadInt32();
            int accelOffset = reader.ReadInt32();
            int accelSize = reader.ReadInt32();

            reader.Seek(start + menuOffset);
            reader.Skip(4);
            if (menuSize > 0)
                Items = ReadMenuItems(reader);

            reader.Seek(start + accelOffset);
            for (int i = 0; i < accelSize / 8; i++)
            {
                AccelShift.Add(reader.ReadByte());
                reader.Skip(1);
                AccelKey.Add(reader.ReadInt16());
                AccelId.Add(reader.ReadInt16());
                reader.Skip(2);
            }
            reader.Seek(start + size);
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
