using Nebula.Core.Memory;

namespace Nebula.Core.Data
{
    public interface IReadable
    {
        public void Read(ByteReader reader);
    }
}
