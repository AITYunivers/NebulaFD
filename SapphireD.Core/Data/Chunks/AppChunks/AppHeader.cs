using SapphireD.Core.Memory;
using System.Drawing;

namespace SapphireD.Core.Data.Chunks.AppChunks
{
    public class AppHeader : Chunk
    {
        public BitDict Flags = new BitDict(new string[]      // Flags
        {
            "1", "2", "3", "4", "5"
        });

        public BitDict NewFlags = new BitDict(new string[]   // New Flags
        {
            "1", "2", "3", "4", "5"
        });

        public BitDict OtherFlags = new BitDict(new string[] // Other Flags
        {
            "1", "2", "3", "4", "5"
        });

        public short GraphicMode; // Color Mode
        public short AppWidth;    // Window Width
        public short AppHeight;   // Window Height
        public Color BorderColor; // Border Color
        public int FrameCount;    // Number of Frames
        public int FrameRate;     // Frames Per Second
        public byte WindowMenu;   // Window Menu Index

        public short[] ControlType = new short[4];     // Control Type Per Player
        public short[,] ControlKeys = new short[4, 8]; // Control Keys Per Player
        public uint InitScore; // Initial Score
        public uint InitLives; // Initial Number of Lives

        public AppHeader()
        {
            ChunkName = "AppHeader";
            ChunkID = 0x2223;
        }

        public override void ReadCCN(ByteReader reader)
        {
            reader.Skip(4);
            Flags.Value = (uint)reader.ReadShort();
            NewFlags.Value = (uint)reader.ReadShort();
            GraphicMode = reader.ReadShort();
            OtherFlags.Value = (uint)reader.ReadShort();

            AppWidth = reader.ReadShort();
            AppHeight = reader.ReadShort();

            InitScore = reader.ReadUInt();
            InitLives = reader.ReadUInt();

            for (int i = 0; i < 4; i++)
                ControlType[i] = reader.ReadShort();

            for (int i = 0; i < 4; i++)
                for (int ii = 0; ii < 8; ii++)
                    ControlKeys[i, ii] = reader.ReadShort();

            BorderColor = reader.ReadColor();
            FrameCount = reader.ReadInt();
            FrameRate = reader.ReadInt();
            WindowMenu = reader.ReadByte();
            reader.Skip(3);

            SapDCore.PackageData.AppHeader = this;
        }

        public override void ReadMFA(ByteReader reader)
        {

        }

        public override void WriteCCN(ByteWriter writer)
        {

        }

        public override void WriteMFA(ByteWriter writer)
        {

        }
    }
}
