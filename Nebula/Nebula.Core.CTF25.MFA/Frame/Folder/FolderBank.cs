using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.MFA.Frame.Folder
{
    internal class FolderBank : Collection<FolderItem>, IReadable
    {
        public void Read(ByteReader reader)
        {
            int folderCount = reader.ReadInt();
            this.Log($"Found {folderCount} folder(s)", Logger.LogType.Debug);
            if (folderCount < 0)
                throw new InvalidDataException("Invalid folder count. Expected greater than or equal to 0, got " + folderCount);

            foreach (FolderItem folderItem in reader.ReadIReadables<FolderItem>(folderCount))
                Add(folderItem);

            for (int i = 0; i < folderCount; i++)
            {
                FolderItem folderItem = this[i];
                this.Log($"Folder {i}{(folderItem.Name == null ? "" : ": " + folderItem.Name)}", Logger.LogType.Debug);
            }
        }
    }
}
