using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.MFA.Frame.Folder
{
    internal class FolderBank : IReadable
    {
        private FolderItem[] _folderItems = [];

        public void Read(ByteReader reader)
        {
            int folderCount = reader.ReadInt();
            this.Log($"Found {folderCount} folder(s)", Logger.LogType.Debug);
            if (folderCount < 0)
                throw new InvalidDataException("Invalid folder count. Expected greater than or equal to 0, got " + folderCount);

            _folderItems = new FolderItem[folderCount];
            for (int i = 0; i < folderCount; i++)
            {
                FolderItem folderItem = new FolderItem();
                folderItem.Read(reader);
                _folderItems[i] = folderItem;
                this.Log($"Folder {i}{(folderItem.Name == null ? "" : ": " + folderItem.Name)}", Logger.LogType.Debug);
            }
        }

        public FolderItem this[int index]
        {
            get => _folderItems[index];
            set => _folderItems[index] = value;
        }
    }
}
