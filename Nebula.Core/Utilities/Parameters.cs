using Newtonsoft.Json;

namespace Nebula.Core.Utilities
{
    public class Parameters
    {
        public static Parameters Inst = new Parameters();
        public string comment_images = "Whether or not to avoid reading image data";
        public bool ignore_images = false;
        public string comment_fonts = "Whether or not to avoid reading font data";
        public bool ignore_fonts = false;
        public string comment_sounds = "Whether or not to avoid reading sound data";
        public bool ignore_sounds = false;
        public string comment_music = "Whether or not to avoid reading music data";
        public bool ignore_music = false;
        public string comment_events = "Whether or not to avoid reading events";
        public bool ignore_events = false;
        public string comment_shaders = "Whether or not to avoid reading shaders and shader data";
        public bool ignore_shaders = false;
        public string comment_objects = "Whether or not to avoid reading objects and object data";
        public bool ignore_objects = false;
        public string comment_binaryfiles = "Whether or not to avoid reading binary file data";
        public bool ignore_binaryfiles = false;
        public string comment_chunks = "Whether or not to dump unknown chunks to a 'Chunks' folder";
        public bool dump_unk_chunks = false;
        public string comment_allchunks = "Whether or not to dump all chunks to a 'Chunks' folder";
        public bool dump_all_chunks = false;
        public string comment_unicode = "Whether or not to force reading as unicode";
        public bool force_unicode = false;
        public string comment_gpu = "Whether or not to use the GPU to translate images (EXPERIMENTAL)";
        public bool gpu_acceleration = false;
        public string comment_evtlog = "Whether or not to silently log events to the log file";
        public bool silent_log_events = false;
        public string comment_invmask = "Whether or not to invert the ignore_frames selection";
        public bool invert_ignore_frames = false;
        public string comment_frames = "An array of frame ids to ignore reading, index starts at 0";
        public int[] ignore_frames = new int[0];

        public Parameters()
        {
            if (File.Exists("config.json"))
                JsonConvert.PopulateObject(File.ReadAllText("config.json"), this);
            string[] seri = JsonConvert.SerializeObject(this, Formatting.Indented).Split('\n');
            for (int i = 0; i < seri.Length; i++)
                if (seri[i].Trim().StartsWith("\"comment_"))
                    seri[i] = "// " + seri[i].Split('"')[^2];
            File.WriteAllText("config.json", string.Join('\n', seri));
        }

        public static bool DontIncludeImages => Inst.ignore_images;
        public static bool DontIncludeFonts => Inst.ignore_fonts;
        public static bool DontIncludeSounds => Inst.ignore_sounds;
        public static bool DontIncludeMusic => Inst.ignore_music;
        public static bool DontIncludeEvents => Inst.ignore_events;
        public static bool DontIncludeShaders => Inst.ignore_shaders;
        public static bool DontIncludeObjects => Inst.ignore_objects;
        public static bool DontIncludeBinaryFiles => Inst.ignore_binaryfiles;
        public static bool DumpUnknownChunks => Inst.dump_unk_chunks;
        public static bool DumpAllChunks => Inst.dump_all_chunks;
        public static bool ForceUnicode => Inst.force_unicode;
        public static bool GPUAcceleration => Inst.gpu_acceleration;
        public static bool SilentLogEvents => Inst.silent_log_events;
        public static bool InvertFrameMask => Inst.invert_ignore_frames;
        public static int[] DontIncludeFrames => Inst.ignore_frames;
    }
}
