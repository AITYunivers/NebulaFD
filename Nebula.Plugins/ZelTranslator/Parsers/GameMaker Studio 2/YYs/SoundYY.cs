namespace ZelTranslator_SD.Parsers.GameMakerStudio2
{
    public class SoundYY
    {
        public class RootObject
        {
            public string resourceType = "GMSound";
            public string resourceVersion = "1.0";
            public string name { get; set; }
            public audioGroupId audioGroupId = new audioGroupId();
            public int bitDepth = 1;
            public int bitRate = 128;
            public int compression = 3;
            public int conversionMode = 0;
            public float duration { get; set; }
            public Parent parent = new Parent();
            public bool preload = false;
            public int sampleRate { get; set; }
            public string soundFile { get; set; }
            public int type = 1; // Mono=0 Stereo=1 3D=2
            public float volume = 1.0f;
            
        }

        public class audioGroupId
        {
            public string name = "audiogroup_default";
            public string path = "audiogroups/audiogroup_default";
        }

        public class Parent
        {
            public string name = "Sounds";
            public string path = "folders/Sounds.yy";
        }

    }
}
