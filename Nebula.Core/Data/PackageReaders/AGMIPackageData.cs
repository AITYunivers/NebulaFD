using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.Data.PackageReaders
{
    public class AGMIPackageData : PackageData
    {
        public override void Read()
        {
            Logger.Log(this, $"Running {NebulaCore.BuildDate} build.");
            Header = Reader.ReadAscii(4);
            Logger.Log(this, "Header: " + Header);
            NebulaCore.MFA = true;
            ImageBank.ReadMFA(Reader);
        }
    }
}
