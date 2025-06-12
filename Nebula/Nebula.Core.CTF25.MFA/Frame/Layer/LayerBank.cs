using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.MFA.Layer
{
    internal class LayerBank : IReadable, IWritable
    {
        private LayerItem[] _layerItems = [];

        public void Read(ByteReader reader)
        {
            int layerCount = reader.ReadInt();
            this.Log($"Found {layerCount} layer(s)", Logger.LogType.Debug);
            if (layerCount < 0)
                throw new InvalidDataException("Invalid layer count. Expected greater than or equal to 0, got " + layerCount);

            _layerItems = reader.ReadIReadables<LayerItem>(layerCount);

            for (int i = 0; i < layerCount; i++)
                this.Log($"Layer {i}: {_layerItems[i].Name}", Logger.LogType.Debug);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(_layerItems.Length);
            writer.WriteIWritables(_layerItems);
        }

        public LayerItem this[int index]
        {
            get => _layerItems[index];
            set => _layerItems[index] = value;
        }
    }
}
