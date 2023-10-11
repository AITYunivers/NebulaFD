using Joveler.Compression.ZLib;
using SapphireD.Core.Data;
using SapphireD.Core.FileReaders;
using Spectre.Console;

namespace SapphireD
{
    public static class SapDCore
    {
        public const string BuildDate = "7/19/23";
        public static FigletText ConsoleFiglet = new FigletText("SapphireD").Centered().Color(Color.DeepSkyBlue1);
        public static Rule ConsoleRule = new Rule().RuleStyle("DeepSkyBlue2");

        public static float Fusion = 2.5f;
        public static bool Plus;
        public static bool Seeded;
        public static bool Android;
        public static bool iOS;
        public static bool MFA;

        public static int Build;
        public static bool? _unicode = null;
        public static bool Unicode => _unicode != null && _unicode == true;

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
            else if (Fusion > 0 && Fusion < 1.0f)
                str = "CNC";
            else if (Fusion == 1.0f)
                str = "MMF 1.0";
            else if (Fusion >= 1.1 && Fusion < 1.5f)
                str = "TGF 1.0";
            else if (Fusion >= 1.5f && Fusion < 2.0f)
                str = "MMF 1.5";
            else if (Fusion == 2.0f)
                str = "MMF 2";
            else if (Fusion >= 2.1f && Fusion < 2.5f)
                str = "TGF 2";
            else if (Fusion >= 2.5f && Fusion < 3.0f)
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
            else if (MFA)
                str += " MFA";

            return str;
        }
    }
}