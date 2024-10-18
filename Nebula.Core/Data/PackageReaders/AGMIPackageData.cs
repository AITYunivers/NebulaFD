using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.Data.PackageReaders
{
    public class AGMIPackageData : PackageData
    {
        public override void Read(ByteReader reader)
        {
            this.Log($"Running build '{NebulaCore.GetCommitHash()}'");
            Header = reader.ReadAscii(4);
            this.Log("Header: " + Header);
            NebulaCore.MFA = true;
            ImageBank.ReadMFA(reader);
        }
    }
}
