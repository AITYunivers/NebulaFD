using Nebula;
using Nebula.Core.Data.Chunks.BankChunks.Sounds;
using Nebula.Core.Utilities;
using Nebula.Plugins.GameDumper;
using Spectre.Console;

namespace GameDumper.AssetDumpers
{
    public class SoundDumper
    {
        public static void Execute()
        {
            ProgressTask? task = AssetDump.ProgressContext!.AddTask($"[{NebulaCore.ColorRules[4]}]Dumping Sounds[/]", false);
            string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\Sounds\\";
            task.MaxValue = NebulaCore.PackageData.SoundBank.Sounds.Count;
            Directory.CreateDirectory(path);

            Sound[] sounds = NebulaCore.PackageData.SoundBank.Sounds.Values.ToArray();
            for (int i = 0; i < sounds.Length; i++)
            {
                File.WriteAllBytes(path + sounds[i].Name + GetExtension(sounds[i].Data), sounds[i].Data);
                task.Value++;
            }
            task.StopTask();
        }

        public static string GetExtension(byte[] data)
        {
            if (data[0] == 'R' && data[1] == 'I' && data[2] == 'F' && data[3] == 'F')
                return ".wav";
            if (data[0] == 0xFF && data[1] == 0xFB && data[2] == 0x90)
                return ".wav";
            if (data[0] == 'O' && data[1] == 'g' && data[2] == 'g' && data[3] == 'S')
                return ".ogg";
            if (data[0] == 'F' && data[1] == 'O' && data[2] == 'R' && data[3] == 'M')
                return ".aiff";
            if (data[0] == 'I' && data[1] == 'D' && data[2] == '3')
                return ".mp3";

            // Because of Clickteam stole the MOD replayer from open-source OpenMPT library,
            // there's more file formats that can be supported by modflt.sft.
            return ".wav";
        }
    }
}
