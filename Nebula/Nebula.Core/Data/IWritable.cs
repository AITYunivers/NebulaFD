using Nebula.Core.Memory;

namespace Nebula.Core.Data
{
    public interface IWritable
    {
        public void Write(ByteWriter writer);
    }
}
