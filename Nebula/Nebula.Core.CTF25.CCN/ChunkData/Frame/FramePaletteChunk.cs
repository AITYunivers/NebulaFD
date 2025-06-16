using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Collections;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Nebula.Core.CTF25.CCN.ChunkData.Frame
{
    internal class FramePaletteChunk : CommonChunk, ICollection<Color>
    {
        public ushort Version = 768;
        private Collection<Color> _colors = [];

        public override void ReadChunkData(ByteReader reader)
        {
            Version = reader.ReadUShort();
            _colors = [.. reader.ReadColors(reader.ReadUShort())];

            this.Log($"Found {_colors.Count} palette colors.", Logger.LogType.Debug);
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            writer.WriteUShort(Version);
            writer.WriteUShort((ushort)_colors.Count);
            writer.WriteColors(_colors);
        }

        public int Count => _colors.Count;
        public bool IsReadOnly => false;
        public void Add(Color item) => _colors.Add(item);
        public bool Remove(Color item) => _colors.Remove(item);
        public void Clear() => _colors.Clear();
        public bool Contains(Color item) => _colors.Contains(item);
        public void CopyTo(Color[] array, int arrayIndex) => _colors.CopyTo(array, arrayIndex);
        public IEnumerator<Color> GetEnumerator() => _colors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
