using Nebula.Core.Data;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;

namespace Nebula.Core.CTF25.MFA.Layer
{
    internal class LayerBank : Collection<LayerItem>, IReadable, IWritable
    {
        public void Read(ByteReader reader)
        {
            int layerCount = reader.ReadInt();
            this.Log($"Found {layerCount} layer(s)", Logger.LogType.Debug);
            if (layerCount < 0)
                throw new InvalidDataException("Invalid layer count. Expected greater than or equal to 0, got " + layerCount);

            foreach (LayerItem layerItem in reader.ReadIReadables<LayerItem>(layerCount))
                layerItem.Read(reader);

            for (int i = 0; i < layerCount; i++)
                this.Log($"Layer {i}: {this[i].Name}", Logger.LogType.Debug);
        }

        public void Write(ByteWriter writer)
        {
            writer.WriteInt(Count);
            writer.WriteIWritables(this);
        }
    }
}
