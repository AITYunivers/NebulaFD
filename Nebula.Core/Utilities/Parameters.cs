using Newtonsoft.Json;

namespace Nebula.Core.Utilities
{
    public class Parameters
    {
        public static Parameters Inst = new Parameters();
        public bool ignore_images = false;
        public bool ignore_fonts = false;
        public bool ignore_sounds = false;
        public bool ignore_music = false;
        public bool ignore_events = false;
        public bool ignore_shaders = false;
        public bool ignore_objects = false;
        public bool ignore_binaryfiles = false;
        public int[] ignore_frames = new int[0];

        public Parameters()
        {
            if (File.Exists("config.json"))
                JsonConvert.PopulateObject(File.ReadAllText("config.json"), this);
            File.WriteAllText("config.json", JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static bool DontIncludeImages => Inst.ignore_images;
        public static bool DontIncludeFonts => Inst.ignore_fonts;
        public static bool DontIncludeSounds => Inst.ignore_sounds;
        public static bool DontIncludeMusic => Inst.ignore_music;
        public static bool DontIncludeEvents => Inst.ignore_events;
        public static bool DontIncludeShaders => Inst.ignore_shaders;
        public static bool DontIncludeObjects => Inst.ignore_objects;
        public static bool DontIncludeBinaryFiles => Inst.ignore_binaryfiles;
        public static int[] DontIncludeFrames => Inst.ignore_frames;
    }
}
