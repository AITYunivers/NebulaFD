using Nebula.Core.CTF25.MFA.Common;
using Nebula.Core.CTF25.MFA.Frame.Folder;
using Nebula.Core.CTF25.MFA.Frame.Instance;
using Nebula.Core.CTF25.MFA.Layer;
using Nebula.Core.CTF25.MFA.Object;
using Nebula.Core.Data;
using Nebula.Core.Memory;
using System.Drawing;
using System.Reflection.Metadata;

namespace Nebula.Core.CTF25.MFA.Frame
{
    internal class FrameItem : IReadable, IWritable
    {
        public uint Handle;
        public string Name = string.Empty;
        public int Width;
        public int Height;
        public Color Color;
        public uint Flags;
        public int MaxObjects;
        public string Password = string.Empty;
        public int EditorX;
        public int EditorY;
        public Color[] Palette = [];
        public uint IconHandle;
        public uint EditorLayerHandle;
        public LayerBank Layers = new LayerBank();
        public Transition? TransitionIn = null;
        public Transition? TransitionOut = null;
        public ObjectBank Objects = new ObjectBank();
        public FolderBank Folders = new FolderBank();
        public InstanceBank Instances = new InstanceBank();

        public void Read(ByteReader reader)
        {
            Handle = reader.ReadUInt();
            Name = reader.ReadAutoYuniversal();
            Width = reader.ReadInt();
            Height = reader.ReadInt();
            Color = reader.ReadColor();
            Flags = reader.ReadUInt();
            MaxObjects = reader.ReadInt();
            Password = reader.ReadAutoYuniversal();
            reader.Skip(reader.ReadInt()); // Unknown
            EditorX = reader.ReadInt();
            EditorY = reader.ReadInt();

            Palette = new Color[reader.ReadInt()];
            for (int i = 0; i < Palette.Length; i++)
                Palette[i] = reader.ReadColor();

            IconHandle = reader.ReadUInt();
            EditorLayerHandle = reader.ReadUInt();

            Layers.Read(reader);

            bool hasTransitionIn = reader.ReadBool();
            if (hasTransitionIn)
            {
                TransitionIn = new Transition();
                TransitionIn.Read(reader);
            }

            bool hasTransitionOut = reader.ReadBool();
            if (hasTransitionOut)
            {
                TransitionOut = new Transition();
                TransitionOut.Read(reader);
            }

            Objects.Read(reader);
            Folders.Read(reader);
            Instances.Read(reader);

            reader.Skip(reader.ReadInt()); // Events

            while (true)
            {
                bool isLast = reader.ReadByte() == 0x00;
                if (isLast)
                    break;

                reader.Skip(reader.ReadInt()); // Data
            }
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteUInt(Handle);
            writer.WriteAutoYunicode(Name);
            writer.WriteInt(Width);
            writer.WriteInt(Height);
            writer.WriteColor(Color);
            writer.WriteUInt(Flags);
            writer.WriteInt(MaxObjects);
            writer.WriteAutoYunicode(Password);
            writer.WriteInt(0); // Unknown
            writer.WriteInt(EditorX);
            writer.WriteInt(EditorY);

            writer.WriteInt(Palette.Length);
            foreach (Color item in Palette)
                writer.WriteColor(item);

            writer.WriteUInt(IconHandle);
            writer.WriteUInt(EditorLayerHandle);

            Layers.Write(writer);

            writer.WriteBool(TransitionIn != null);
            TransitionIn?.Write(writer);

            writer.WriteBool(TransitionOut != null);
            TransitionOut?.Write(writer);

            Objects.Write(writer);
            writer.WriteInt(0); // Folders.Write(writer);
            Instances.Write(writer);

            writer.WriteInt(0); // Events
            writer.WriteByte(0); // LAST Chunk
        }
    }
}
