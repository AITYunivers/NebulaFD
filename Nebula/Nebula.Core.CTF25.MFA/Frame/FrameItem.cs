using Nebula.Core.CTF25.MFA.Common;
using Nebula.Core.CTF25.MFA.Frame.Folder;
using Nebula.Core.CTF25.MFA.Frame.Instance;
using Nebula.Core.CTF25.MFA.Layer;
using Nebula.Core.CTF25.MFA.Object;
using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.CTF25.MFA.Frame
{
    internal class FrameItem : IReadable
    {
        public string Name = string.Empty;

        public void Read(ByteReader reader)
        {
            uint handle = reader.ReadUInt();
            Name = reader.ReadAutoYuniversal();
            int width = reader.ReadInt();
            int height = reader.ReadInt();
            Color color = reader.ReadColor();
            uint flags = reader.ReadUInt();
            int maxObjects = reader.ReadInt();
            string password = reader.ReadAutoYuniversal();
            reader.Skip(reader.ReadInt()); // Unknown
            int editorX = reader.ReadInt();
            int editorY = reader.ReadInt();

            int paletteEntries = reader.ReadInt();
            for (int i = 0; i < paletteEntries; i++)
            {
                Color paletteColor = reader.ReadColor();
            }

            int iconHandle = reader.ReadInt();
            int editorLayer = reader.ReadInt();

            LayerBank layerBank = new LayerBank();
            layerBank.Read(reader);

            bool hasTransitionIn = reader.ReadBool();
            if (hasTransitionIn)
            {
                Transition transitionIn = new Transition();
                transitionIn.Read(reader);
            }

            bool hasTransitionOut = reader.ReadBool();
            if (hasTransitionOut)
            {
                Transition transitionOut = new Transition();
                transitionOut.Read(reader);
            }

            ObjectBank objectBank = new ObjectBank();
            objectBank.Read(reader);

            FolderBank folderBank = new FolderBank();
            folderBank.Read(reader);

            InstanceBank instanceBank = new InstanceBank();
            instanceBank.Read(reader);

            reader.Skip(reader.ReadInt()); // Events

            while (true)
            {
                bool isLast = reader.ReadByte() == 0x00;
                if (isLast)
                    break;

                reader.Skip(reader.ReadInt()); // Data
            }
        }
    }
}
