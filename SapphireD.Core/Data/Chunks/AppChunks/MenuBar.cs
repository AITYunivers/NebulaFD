using SapphireD.Core.Memory;
using System.Diagnostics;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class MenuBar : Chunk
    {
        public List<MenuItem> Items = new();
        public List<byte> AccelShift = new();
        public List<short> AccelKey = new();
        public List<short> AccelId = new();
        public byte[] Data = new byte[0];

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
            if (menuSize > 0 && SapDCore.Fusion > 2.0f)
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

                if (menuItem.Flags["Parent"])
                    menuItem.Items = ReadMenuItems(reader);

                menuItems.Add(menuItem);
                if (menuItem.Flags["Footer"])
                    break;
            }
            return menuItems;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            long realStartOffset = reader.Tell();
            uint mainSize = reader.ReadUInt();
            long startOffset = reader.Tell();
            uint headerSize = reader.ReadUInt();
            int menuOffset = reader.ReadInt();
            int menuSize = reader.ReadInt();
            if (menuSize == 0)
            {
                reader.Seek(startOffset + mainSize);
                return;
            }
            int accelOffset = reader.ReadInt();
            int accelSize = reader.ReadInt();

            reader.Seek(startOffset + menuOffset + 4);
            //Items = ReadMenuItems(reader);

            reader.Seek(startOffset + accelOffset);
            for (int i = 0; i < accelSize / 8; i++)
            {
                AccelShift.Add(reader.ReadByte());
                reader.Skip(1);
                AccelKey.Add(reader.ReadInt16());
                AccelId.Add(reader.ReadInt16());
                reader.Skip(2);
            }

            reader.Seek(realStartOffset);
            Data = reader.ReadBytes((int)mainSize + 4);
            //File.WriteAllBytes(ChunkName, Data);
            Debug.Assert(reader.Tell() == startOffset + mainSize);
            //reader.Seek(startOffset + mainSize);
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteBytes(File.ReadAllBytes(ChunkName));
            return;
            ByteWriter menuWriter = new ByteWriter(new MemoryStream());

            menuWriter.WriteInt(20);
            menuWriter.WriteInt(20);

            ByteWriter dataWriter = new ByteWriter(new MemoryStream());
            foreach (MenuItem menuItem in Items)
                menuItem.WriteMFA(dataWriter);

            menuWriter.WriteUInt((uint)dataWriter.Tell() + 4);
            menuWriter.WriteUInt((uint)dataWriter.Tell() + 24);
            menuWriter.WriteInt(AccelKey.Count * 8);
            menuWriter.WriteInt(0);
            menuWriter.WriteWriter(dataWriter);

            for (int i = 0; i < AccelKey.Count; i++)
            {
                menuWriter.WriteByte(AccelShift[i]);
                menuWriter.WriteByte(0);
                menuWriter.WriteShort(AccelKey[i]);
                menuWriter.WriteShort(AccelId[i]);
                menuWriter.WriteShort(0);
            }

            writer.WriteUInt((uint)menuWriter.Tell());
            writer.WriteWriter(menuWriter);
        }
    }
}
