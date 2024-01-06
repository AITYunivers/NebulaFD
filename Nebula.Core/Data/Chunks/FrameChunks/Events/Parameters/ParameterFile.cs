using Nebula.Core.Memory;
using System.Diagnostics;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterFile : ParameterChunk
    {
        public BitDict FileFlags = new BitDict( // File Flags
            "WaitForEnd",     // Wait for end
            "HideApplication" // Hide application
        );

        public string FileName = string.Empty;
        public string Command = string.Empty;

        public ParameterFile()
        {
            ChunkName = "ParameterFile";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            FileFlags.Value = reader.ReadUShort();
            FileName = reader.ReadAscii(260);
            Command = reader.ReadAscii();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteUShort((ushort)FileFlags.Value);
            writer.WriteAscii(FileName, 260);
            writer.WriteAscii(Command, true);
        }
    }
}
