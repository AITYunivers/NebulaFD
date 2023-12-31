using Joveler.Compression.ZLib;
using SapphireD.Core.Data;
using SapphireD.Core.FileReaders;
using Spectre.Console;

namespace SapphireD
{
    public static class SapDCore
    {
        public const string BuildDate = "12/5/23";
        public static FigletText ConsoleFiglet = new FigletText("SapphireD").Centered().Color(Color.DeepSkyBlue1);
        public static Rule ConsoleRule = new Rule().RuleStyle("DeepSkyBlue2");

        public static float Fusion = 2.5f;
        public static bool Plus;
        public static bool Seeded;
        public static bool Android;
        public static bool iOS;
        public static bool MFA;
        public static bool Flash;
        public static bool HTML;
        public static int D3D
        {
            get
            {
                if (PackageData.AppHeader.OtherFlags["Direct3D9or11"] && PackageData.AppHeader.OtherFlags["Direct3D8or11"])
                    return 11;
                else if (PackageData.AppHeader.OtherFlags["Direct3D9or11"])
                    return 9;
                else if (PackageData.AppHeader.OtherFlags["Direct3D8or11"])
                    return 8;
                else
                    return 0;
            }
            set
            {

            }
        }

        public static int Build;
        public static bool? _unicode = null;
        public static bool Unicode => _unicode != null && _unicode == true;

        public static string FilePath = string.Empty;
        public static string Parameters = string.Empty;
        public static FileReader? CurrentReader;
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
            else if (Fusion == 0.1f)
                str = "CNC";
            else if (Fusion == 1.0f)
                str = "MMF 1";
            else if (Fusion == 1.1)
                str = "TGF 1";
            else if (Fusion == 1.5f)
                str = "MMF 1.5";
            else if (Fusion == 2.0f)
                str = "MMF 2";
            else if (Fusion == 2.1f)
                str = "TGF 2";
            else if (Fusion == 2.5f)
                str = "CTF 2.5";
            else if (Fusion == 3)
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
