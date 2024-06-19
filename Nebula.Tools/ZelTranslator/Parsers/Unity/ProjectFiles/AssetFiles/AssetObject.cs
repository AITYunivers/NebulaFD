using Nebula.Core.Data.Chunks.ObjectChunks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZelTranslator_SD.Parsers.Unity.ProjectFiles.AssetFiles
{
    public class AssetObject : IAssetFile
    {
        public static List<string> UsedNames = new();
        public string Name;

        public AssetObject(ObjectInfo objInfo)
        {
            Name = objInfo.Name;
            if (UsedNames.Contains(Name))
                for (int i = 1;; i++)
                    if (!UsedNames.Contains(objInfo.Name + $" ({i})"))
                    {
                        Name = objInfo.Name + $" ({i})";
                        break;
                    }
        }

        public void Write(string projectPath)
        {

        }
    }
}
