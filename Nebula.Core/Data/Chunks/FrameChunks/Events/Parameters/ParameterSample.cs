using Nebula.Core.Data.Chunks.BankChunks.Sounds;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterSample : ParameterChunk
	{
		public BitDict SampleFlags = new BitDict( // Sample Flags
            "Uninteruptable" // Uninteruptable
		);

		public short Handle;
        public string Name = string.Empty;

        public ParameterSample()
        {
            ChunkName = "ParameterSample";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadShort();
            SampleFlags.Value = reader.ReadUShort();
            if (NebulaCore.Build >= 296 && (NebulaCore.Windows || NebulaCore.Android) && NebulaCore.Fusion >= 2.5 || NebulaCore.Android && NebulaCore.Build <= 290)
                SoundBank.SampleParameters.Add(this);
            else
                Name = reader.ReadYuniversal();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteShort(Handle);
            writer.WriteUShort((ushort)SampleFlags.Value);
            writer.WriteYunicode(Name, true);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
