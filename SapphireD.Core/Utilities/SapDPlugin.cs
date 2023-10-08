using SapphireD.Core.FileReaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireD.Core.Utilities
{
    public interface SapDPlugin
    {
        string Name { get; }
        void Execute();
    }
}
