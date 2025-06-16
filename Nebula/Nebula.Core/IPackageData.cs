using Nebula.Core.Data;
using Nebula.Core.Memory;

namespace Nebula.Core
{
    public interface IPackageData : IReadable
    {
        public bool Check(ByteReader reader);
        public int GetFusionBuild();
    }
}
