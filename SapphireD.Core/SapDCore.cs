using Joveler.Compression.ZLib;
using SapphireD.Core.Data;
using SapphireD.Core.FileReaders;

namespace SapphireD
{
    public static class SapDCore
    {
        public const string BuildDate = "7/19/23";

        public static float Fusion = 2.5f;
        public static bool Plus;
        public static bool Seeded;
        public static bool Android;
        public static bool iOS;

        public static int Build;
        public static bool Unicode = true;

        public static string FilePath = string.Empty;
        public static string Parameters = string.Empty;
        public static FileReader CurrentReader;
        public static PackageData PackageData => CurrentReader.getPackageData();

        public static void Init()
        {
            ZLibInit.GlobalInit("zlibwapi.dll");
            File.WriteAllText("Latest.log", "");
        }

        public static string GameType()
        {
            string str = string.Empty;

            if (Fusion == 0)
                str = "KNP";
            else if (Fusion > 0 && Fusion < 1.5f)
                str = "CNC";
            else if (Fusion >= 1.5f && Fusion < 2)
                str = "MMF 1.5";
            else if (Fusion >= 2 && Fusion < 2.5f)
                str = "MMF 2";
            else if (Fusion >= 2.5f && Fusion < 3)
                str = "CTF 2.5";
            else if (Fusion >= 3)
                str = "F3";

            if (Seeded)
                str += " Seeded";
            else if (Plus)
                str += "+";

            if (Android)
                str += " Android";
            else if (iOS)
                str += " iOS";

            return str;
        }
    }
}