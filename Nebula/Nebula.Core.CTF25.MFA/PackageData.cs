using Nebula.Core.CTF25.MFA.BinaryFile;
using Nebula.Core.CTF25.MFA.Common;
using Nebula.Core.CTF25.MFA.Extension;
using Nebula.Core.CTF25.MFA.Font;
using Nebula.Core.CTF25.MFA.Frame;
using Nebula.Core.CTF25.MFA.Image;
using Nebula.Core.CTF25.MFA.Music;
using Nebula.Core.CTF25.MFA.Qualifier;
using Nebula.Core.CTF25.MFA.Sound;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;

namespace Nebula.Core.CTF25.MFA
{
    public class PackageData : IPackageData
    {
        public int ProductBuild;

        public void Read(ByteReader reader)
        {
            this.Log($"Running alpha build");
            string header = reader.ReadAscii(4);
            reader.SetUnicode(true);
            this.Log("Project Header: " + header);

            if (header != "MFU2")
                throw new InvalidDataException("Invalid project header. Expected MFU2, got " + header);

            ushort runtimeVersion = reader.ReadUShort();
            ushort runtimeSubversion = reader.ReadUShort();
            int productVersion = reader.ReadInt();
            ProductBuild = reader.ReadInt();
            this.Log("Fusion Build: " + ProductBuild);

            int language = reader.ReadInt();
            string appName = reader.ReadAutoYuniversal();
            this.Log("App Name: " + appName);
            reader.ReadAutoYuniversal();
            string editorFilename = reader.ReadAutoYuniversal();
            this.Log("Editor Filename: " + appName);

            int stampLength = reader.ReadInt();
            byte[] stamp = reader.ReadBytes(stampLength);

            FontBank fontBank = [];
            fontBank.Read(reader);

            SoundBank soundBank = [];
            soundBank.Read(reader);

            MusicBank musicBank = [];
            musicBank.Read(reader);

            ImageBank iconBank = [];
            iconBank.Read(reader);

            ImageBank imageBank = [];
            imageBank.Read(reader);

            /*
            AYS AppName
            AYS Author
            AYS Description
            AYS Copyright
            AYS Company
            AYS Version
            */ reader.SkipAutoYuniversal(6);

            /*
            i32 AppWidth    | 0x00
            i32 AppHeight   | 0x04
            Col BorderColor | 0x08
            u32 DisplayFlags| 0x0C
            u32 GraphicFlags| 0x10
            */ reader.Skip   (0x14);

            /*
            AYS HelpFile
            AYS Unknown
            */ reader.SkipAutoYuniversal(2);

            int score = (reader.ReadInt() + 1) * -1;
            int lives = (reader.ReadInt() + 1) * -1;

            /*
            i32 FrameRate   | 0x00
            i32 BuildType   | 0x04
            */ reader.Skip   (0x08);

            /*
            AYS TargetFilename
            AYS Unknown
            AYS Unknown
            AYS About
            */ reader.SkipAutoYuniversal(4);

            reader.Skip(4);

            BinaryFileBank binaryFileBank = [];
            binaryFileBank.Read(reader);

            int controlCount = reader.ReadInt();
            for (int i = 0; i < controlCount; i++)
            {
                reader.Skip(4);
                reader.Skip(reader.ReadInt() * 4);
            }

            // Menu Bar
            reader.Skip(reader.ReadInt());
            int windowMenu = reader.ReadInt();

            // Menu Images (Not implemented yet)
            reader.Skip(reader.ReadInt() * 8);

            CommonValue[] globalValues = reader.ReadIReadables<CommonValue>(reader.ReadInt());
            CommonValue[] globalStrings = reader.ReadIReadables<CommonValue>(reader.ReadInt());

            // Global Events
            reader.Skip(reader.ReadInt());

            int graphicMode = reader.ReadInt();

            // Icons
            reader.Skip(reader.ReadInt() * 4);

            QualifierBank qualifierBank = [];
            qualifierBank.Read(reader);

            ExtensionBank extensionBank = [];
            extensionBank.Read(reader);

            FrameBank frameBank = [];
            frameBank.Read(reader);

            while (true)
            {
                bool isLast = reader.ReadByte() == 0x00;
                if (isLast)
                    break;

                reader.Skip(reader.ReadInt()); // Data
            }
        }

        public bool Check(ByteReader reader)
        {
            return reader.PeekHeader() == "MFU2";
        }

        public int GetFusionBuild()
        {
            return ProductBuild;
        }
    }
}
