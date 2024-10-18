using Joveler.Compression.ZLib;
using Nebula.Core.Data;
using Nebula.Core.FileReaders;
using Spectre.Console;
using System.Diagnostics;
using System.Reflection;

namespace Nebula
{
    public static class NebulaCore
    {
        public static Color[] ColorRules = new Color[]
        {
            Color.DarkViolet_1,    // Header
            Color.MediumPurple1,   // Text Header
            Color.LightSteelBlue3, // Text Footer
            Color.LightSlateGrey,  // Unselected
            Color.MediumPurple2,   // Selected
        };
        public static FigletText ConsoleFiglet = new FigletText("NebulaFD").Centered().Color(ColorRules[0]);
        public static Rule ConsoleRule = new Rule().RuleStyle(ColorRules[0]);

        public static float Fusion = 2.5f;
        public static bool Plus;
        public static bool Seeded;
        public static bool Unpacked;
        public static bool Windows;
        public static bool Android;
        public static bool iOS;
        public static bool MFA;
        public static bool Flash;
        public static bool HTML;
        public static int D3D
        {
            get
            {
                if (Fusion == 1.5)
                    return 0;

                if (PackageData.AppHeader.OtherFlags["Direct3D9or11"] && PackageData.AppHeader.OtherFlags["Direct3D8or11"])
                    return 11;
                else if (PackageData.AppHeader.OtherFlags["Direct3D9or11"])
                    return 9;
                else if (PackageData.AppHeader.OtherFlags["Direct3D8or11"])
                    return 8;
                else
                    return 0;
            }
            set {}
        }

        public static int Build;
        public static bool? _yunicode = null;
        public static bool Yunicode => _yunicode != null && _yunicode == true;

        public static string FilePath = string.Empty;
        public static IFileReader? CurrentReader;
        public static PackageData PackageData => CurrentReader.getPackageData();

        public static void Init()
        {
            ZLibInit.GlobalInit("zlibwapi.dll");
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

        public static string GetCommitHash()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string? version = fileVersionInfo.ProductVersion;
            if (version != null && version.Length > 6)
                return version.Substring(6, 7);
            return "Unknown";
        }
    }
}
