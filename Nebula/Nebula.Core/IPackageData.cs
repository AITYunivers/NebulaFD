using Nebula.Core.Memory;

namespace Nebula.Core
{
    public interface IPackageData
    {
        public void Read(ByteReader reader);
        public bool Check(ByteReader reader);
    }
}
