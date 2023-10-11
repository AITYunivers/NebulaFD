using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class AppHeader : Chunk
    {
        public BitDict Flags = new BitDict(new string[]        // Flags
        {
            "1", "2", "3", "4", "5"
        });

        public BitDict NewFlags = new BitDict(new string[]     // New Flags
        {
            "1", "2", "3", "4", "5"
        });

        public BitDict OtherFlags = new BitDict(new string[]   // Other Flags
        {
            "1", "2", "3", "4", "5"
        });

        public BitDict DisplayFlags = new BitDict(new string[] // Display Flags (MFA Only)
        {
            "1", "2", "3", "4", "5"
        });

        public BitDict GraphicFlags = new BitDict(new string[] // Graphic Flags (MFA Only)
        {
            "1", "2", "3", "4", "5"
        });

        public short GraphicMode = 4;           // Color Mode
        public short AppWidth = 640;            // Window Width
        public short AppHeight = 480;           // Window Height
        public Color BorderColor = Color.White; // Border Color
        public int FrameCount;                  // Number of Frames
        public int FrameRate = 60;              // Frames Per Second
        public int WindowMenu;                  // Window Menu Index

        public short[] ControlType = new short[4]     // Control Type Per Player
        {
            5, 5, 5, 5
        };
        public short[,] ControlKeys = new short[4, 8] // Control Keys Per Player
        {
            {38, 40, 37, 39, 16, 17, 32, 13},
            {38, 40, 37, 39, 16, 17, 32, 13},
            {38, 40, 37, 39, 16, 17, 32, 13},
            {38, 40, 37, 39, 16, 17, 32, 13}
        };
        public int InitScore = (0 + 1) * -1; // Initial Score
        public int InitLives = (3 + 1) * -1; // Initial Number of Lives

        public AppHeader()
        {
            ChunkName = "AppHeader";
            ChunkID = 0x2223;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            reader.Skip(4);
            Flags.Value = (uint)reader.ReadShort();
            NewFlags.Value = (uint)reader.ReadShort();
            GraphicMode = reader.ReadShort();
            OtherFlags.Value = (uint)reader.ReadShort();

            AppWidth = reader.ReadShort();
            AppHeight = reader.ReadShort();

            InitScore = reader.ReadInt();
            InitLives = reader.ReadInt();

            for (int i = 0; i < 4; i++)
                ControlType[i] = reader.ReadShort();

            for (int i = 0; i < 4; i++)
                for (int ii = 0; ii < 8; ii++)
                    ControlKeys[i, ii] = reader.ReadShort();

            BorderColor = reader.ReadColor();
            FrameCount = reader.ReadInt();
            FrameRate = reader.ReadInt();
            WindowMenu = reader.ReadInt();

            SapDCore.PackageData.AppHeader = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}
