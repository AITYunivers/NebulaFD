namespace ZelTranslator_SD.Parsers.GameMakerStudio2
{
    public class ProjectYYP
    {
        public class RootObject
        {
            public string resourceType = "GMProject";
            public string resourceVersion = "1.6";
            public string name { get; set; }
            public Resource[] resources { get; set; }
            public Option[] Options { get; set; }
            public int defaultScriptType = 0;
            public bool isEcma = false;
            public Configs configs = new Configs();
            public RoomOrderNode[] RoomOrderNodes { get; set; }
            public Folder[] Folders { get; set; }
            public AudioGroup[] AudioGroups { get; set; }
            public TextureGroup[] TextureGroups { get; set; }
            public IncludedFile[] IncludedFiles = new IncludedFile[0];
            public Metadata MetaData = new Metadata();
        }

        // Resources
        public class Resource
        {
            public ResourceID id { get; set; }
            public int order { get; set; }
        }

        public class ResourceID
        {
            public string name { get; set; }
            public string path { get; set; }
        }

        // Options
        public class Option
        {
            public string name { get; set; }
            public string path { get; set; }
        }

        // Configs
        public class Configs
        {
            public string name = "Default";
            public object[] children = new object[0];
        }

        // Room Order Nodes
        public class RoomOrderNode
        {
            public RoomID roomId { get; set; }
        }

        public class RoomID
        {
            public string name { get; set; }
            public string path { get; set; }
        }

        // Folders
        public class Folder
        {
            public string resourceType = "GMFolder";
            public string resourceVersion = "1.0";
            public string name { get; set; }
            public string folderPath { get; set; }
            public int order { get; set; }
        }

        // Audio Groups
        public class AudioGroup
        {
            public string resourceType = "GMAudioGroup";
            public string resourceVersion = "1.3";
            public string name = "audiogroup_default";
            public int targets = -1;
        }

        // Texture Groups
        public class TextureGroup
        {
            public string resourceType = "GMTextureGroup";
            public string resourceVersion = "1.3";
            public string name = "Default";
            public bool isScaled = true;
            public string compressFormat = "bz2";
            public string loadType = "default";
            public string directory = "";
            public bool autocrop = true;
            public int border = 2;
            public int mipsToGenerate = 0;
            public object groupParent = null;
            public int targets = -1;
        }

        // Included Files
        public class IncludedFile {}

        // Metadata
        public class Metadata
        {
            public string IDEVersion = "2022.11.1.56";
        }
    }
}
