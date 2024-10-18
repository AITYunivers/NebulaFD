using Nebula.Core.Data.Chunks.BankChunks.Images;
using Nebula.Core.Data.Chunks.BankChunks.Sounds;
using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Drawing;
using System.Text;
using Color = System.Drawing.Color;
using Font = Nebula.Core.Data.Chunks.BankChunks.Fonts.Font;
using Image = Nebula.Core.Data.Chunks.BankChunks.Images.Image;

#pragma warning disable CS8602
namespace Nebula.Core.Data.PackageReaders
{
    public class KNPPackageData : PackageData
    {
        public string FilePath = string.Empty;

        public override void Read(ByteReader reader)
        {
            this.Log($"Running build '{NebulaCore.GetCommitHash()}'");
            NebulaCore._yunicode = false;
            NebulaCore.Fusion = 0.0f;
            AppName = Path.GetFileNameWithoutExtension(FilePath);

            if (File.Exists(FilePath + ".gam"))
                ReadGameData(new ByteReader(new MemoryStream(File.ReadAllBytes(FilePath + ".gam"))));
            if (File.Exists(FilePath + ".snd"))
                ReadSoundBank(new ByteReader(new MemoryStream(File.ReadAllBytes(FilePath + ".snd"))));
            if (File.Exists(FilePath + ".mus"))
                ReadMusicBank(new ByteReader(new MemoryStream(File.ReadAllBytes(FilePath + ".mus"))));
            if (File.Exists(FilePath + ".img"))
                ReadImageBank(new ByteReader(new MemoryStream(File.ReadAllBytes(FilePath + ".img"))));
            if (File.Exists(FilePath + ".mtf"))
                ReadFontBank(new ByteReader(new MemoryStream(File.ReadAllBytes(FilePath + ".mtf"))));

            FinishParsing();
        }

        public void ReadGameData(ByteReader reader)
        {
            Header = reader.ReadAscii(4);
            reader.Skip(2);
            AppName = reader.ReadYuniversalStop(80);
            Author = reader.ReadYuniversalStop(80);
            reader.Skip(80); // Company
            AppHeader.GraphicMode = reader.ReadShort();
            AppHeader.AppWidth = reader.ReadShort();
            AppHeader.AppHeight = reader.ReadShort();
            AppHeader.BorderColor = reader.ReadColor();
            AppHeader.Flags.Value = reader.ReadUInt(); // Default: 356
            AppHeader.Flags["FitInside"] = false; // Panic Key (F5)
            reader.Skip(4);
            AppHeader.ControlType = new int[4];
            AppHeader.ControlKeys = new int[4][];
            for (int i = 0; i < 4; i++)
                AppHeader.ControlType[i] = reader.ReadShort();
            for (int i = 0; i < 4; i++)
            {
                AppHeader.ControlKeys[i] = new int[6];
                for (int ii = 0; ii < 6; ii++)
                    AppHeader.ControlKeys[i][ii] = reader.ReadShort();
            }
            AppHeader.InitLives = reader.ReadInt();
            reader.Skip(8); // Lives Minimum and Maximum (Not in CTF2.5)
            AppHeader.InitScore = reader.ReadInt();
            reader.Skip(8); // Score Minimum and Maximum (Not in CTF2.5)
            reader.Skip(4); // Offset to the start of Global Color Palette
            reader.Skip(2); // Frame Count
            reader.Skip(4); // Help File Type
            for (int i = 0; i < 256; i++)
                reader.Skip(4); // Frame Offset
            for (int i = 0; i < 256; i++)
                reader.Skip(2); // Frame Handle
            long iconOffset = reader.Tell();
            reader.Skip(512); // Icon Data
            reader.Skip(128);
            while (reader.HasMemory(256 * 4))
                ReadFrameData(reader);

            reader.Seek(iconOffset);
            ReadIconData(reader);
        }

        public void ReadFrameData(ByteReader reader)
        {
            Frame frm = new Frame();
            int StampSize = reader.ReadInt();
            reader.Skip(16);
            for (int i = 0; i < 256; i++)
                frm.FramePalette.Palette.Add(reader.ReadColor(1));
            reader.Skip(StampSize);
            if (!reader.HasMemory(1)) return;
            reader.Skip(4);
            frm.FrameHeader.Width = reader.ReadShort();
            frm.FrameHeader.Height = reader.ReadShort();
            frm.FrameName = reader.ReadYuniversalStop(32);
            frm.FramePassword = reader.ReadYuniversalStop(10);
            reader.Skip(4);
            frm.FrameHeader.FrameFlags.Value = reader.ReadUShort();
            long endOffset = reader.Tell() + reader.ReadUInt() - 36;
            reader.Skip(58);
            ushort ObjectCount = reader.ReadUShort();
            reader.Skip(4);
            //for (int i = 0; i < ObjectCount; i++)
                //ReadObjectData(reader);
            reader.Seek(endOffset);
            Frames.Add(frm);
        }

        public void ReadObjectData(ByteReader reader)
        {
            string objName = reader.ReadYuniversalStop(40);
            reader.Skip(24);
            uint paragraphData = reader.ReadUInt();
            reader.Skip(2 + paragraphData);
            uint randomData = reader.ReadUInt();
            reader.Skip(14 + randomData);
        }

        public void ReadIconData(ByteReader reader)
        {
            Image img = new Image();
            img.Handle = 0;
            img.Width = 32;
            img.Height = 32;
            img.ImageData = reader.ReadBytes(490);
            img.TransparentColor = Color.Black;
            img.GraphicMode = 3;
            img.Flags["RLE"] = true;
            AppIcon.Icon = img.GetBitmap();

            NebulaCore.CurrentReader.Icons.Add(16,  AppIcon.Icon.ResizeImage(16));
            NebulaCore.CurrentReader.Icons.Add(32,  AppIcon.Icon);
            NebulaCore.CurrentReader.Icons.Add(64,  AppIcon.Icon.ResizeImage(48));
            NebulaCore.CurrentReader.Icons.Add(128, AppIcon.Icon.ResizeImage(128));
            NebulaCore.CurrentReader.Icons.Add(256, AppIcon.Icon.ResizeImage(256));
        }

        public void ReadSoundBank(ByteReader reader)
        {
            int sndCnt = reader.ReadInt();
            int[] sndOffsets = new int[sndCnt];
            int[] sndSizes = new int[sndCnt];
            for (int i = 0; i < sndCnt; i++)
            {
                sndOffsets[i] = reader.ReadInt();
                sndSizes[i] = reader.ReadInt();
            }

            for (uint i = 0; i < sndCnt; i++)
            {
                if (sndOffsets[i] == 0)
                    continue;
                reader.Seek(sndOffsets[i]);
                reader.Skip(6);
                int sndSize = reader.ReadInt();
                Sound snd = new Sound();
                snd.Name = reader.ReadYuniversalStop(22);
                snd.Data = Sound.FixSoundData(reader.ReadBytes(sndSize));
                SoundBank.Sounds.Add(i, snd);
            }
        }

        public void ReadMusicBank(ByteReader reader)
        {
            int musCnt = reader.ReadInt();
            int[] musOffsets = new int[musCnt];
            int[] musSizes = new int[musCnt];
            for (int i = 0; i < musCnt; i++)
            {
                musOffsets[i] = reader.ReadInt();
                musSizes[i] = reader.ReadInt();
            }

            for (uint i = 0; i < musCnt; i++)
            {
                if (musOffsets[i] == 0)
                    continue;
                reader.Seek(musOffsets[i]);
                reader.Skip(6);
                int musSize = reader.ReadInt();
                string musName = reader.ReadYuniversalStop(22);
                byte[] musData = reader.ReadBytes(musSize);
            }
        }

        public void ReadImageBank(ByteReader reader)
        {
            int imgCnt = reader.ReadInt();
            int[] imgOffsets = new int[imgCnt];
            int[] imgSizes = new int[imgCnt];
            for (int i = 0; i < imgCnt; i++)
            {
                imgOffsets[i] = reader.ReadInt();
                imgSizes[i] = reader.ReadInt();
            }

            for (uint i = 0; i < imgCnt; i++)
            {
                if (imgOffsets[i] == 0)
                    continue;
                Image img = new Image();
                reader.Seek(imgOffsets[i]);
                reader.Skip(6);
                img.Handle = i;
                int imgSize = reader.ReadInt();
                img.Width = reader.ReadShort();
                img.Height = reader.ReadShort();
                reader.Skip(2);
                img.HotspotX = reader.ReadShort();
                img.HotspotY = reader.ReadShort();
                img.ActionPointX = reader.ReadShort();
                img.ActionPointY = reader.ReadShort();
                img.ImageData = reader.ReadBytes(imgSize);
                img.TransparentColor = Color.Black;
                img.GraphicMode = 3;
                img.Flags["RLE"] = true;
                ImageBank.Images.Add(img.Handle, img);
            }
        }

        public void ReadFontBank(ByteReader reader)
        {
            int fntCnt = reader.ReadInt();
            int[] fntOffsets = new int[fntCnt];
            int[] fntSizes = new int[fntCnt];
            for (int i = 0; i < fntCnt; i++)
            {
                fntOffsets[i] = reader.ReadInt();
                fntSizes[i] = reader.ReadInt();
            }

            for (uint i = 0; i < fntCnt; i++)
            {
                if (fntOffsets[i] == 0)
                    continue;
                reader.Seek(fntOffsets[i]);
                Font fnt = new Font();
                fnt.Handle = i;
                reader.Skip(18);
                fnt.Name = reader.ReadYuniversalStop(32);
                string fntSubName = reader.ReadYuniversalStop(32);
                reader.Skip(22);
                FontBank.Fonts.Add(fnt.Handle, fnt);
            }
        }

        public void FinishParsing()
        {
            EditorFilename = FilePath + ".mfa";
            for (int i = 0; i < Frames.Count; i++)
            {
                if (string.IsNullOrEmpty(Frames[i].FrameName))
                    Frames[i].FrameName = "Frame " + (i + 1);
                Frames[i].Handle = i;
                Frames[i].FrameHeader.FrameFlags.Value = 32800;
            }
            AppHeader.Flags.Value = 4294944001;
            AppHeader.NewFlags.Value = 2048;
            AppHeader.OtherFlags.Value = 4294951041;
            AppHeader.OtherFlags["Direct3D9or11"] = false;
            AppHeader.OtherFlags["Direct3D8or11"] = false;
            ExtendedHeader.Flags.Value = 3288334336;
            ExtendedHeader.CompressionFlags.Value = 1049120;
        }
    }
}
