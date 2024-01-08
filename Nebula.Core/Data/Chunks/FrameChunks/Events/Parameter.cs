using Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters;
using Nebula.Core.Memory;
using System.Linq.Expressions;
using System;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events
{
    public class Parameter : Chunk
    {
        public int Code;
        public ParameterChunk Data = new();

        public Parameter()
        {
            ChunkName = "Parameter";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long endPosition = reader.Tell() + reader.ReadUShort();

            Code = reader.ReadShort();
            Data = Code switch
            {
                1 => new ParameterObject(),
                2 or 42 => new ParameterTimer(),
                3 or 4 or 10 or 11 or 12 or 14 or 17 or 26 or
                31 or 43 or 44 or 50 or 58 or 60 or 61 => new ParameterShort(),
                5 or 25 or 29 or 34 or 48 or 49 or 56 or 67 or 70 => new ParameterInt(),
                6 or 7 or 35 or 36 => new ParameterSample(),
                9 => new ParameterCreate(),
                13 => new ParameterEvery(),
                15 or 22 or 23 or 27 or 28 or 45 or
                46 or 52 or 53 or 54 or 59 or 62 => new ParameterExpressions(),
                16 or 21 => new ParameterPosition(),
                18 => new ParameterShoot(),
                19 => new ParameterZone(),
                24 => new ParameterColor(),
                40 or 41 or 64 => new ParameterString(),
                32 => new ParameterClick(),
                33 => new ParameterFile(),
                55 => new ParameterExtension(),
                38 => new ParameterGroup(),
                39 => new ParameterGroupPointer(),
                47 or 51 => new ParameterDoubleShort(),
                68 => new ParameterVariables(),
                69 => new ParameterChildEvent(),
                57 => new ParameterMovement(),
                _ => new ParameterChunk()
            };
            Data.ReadCCN(reader);

            reader.Seek(endPosition);
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            ByteWriter paramWriter = new ByteWriter(new MemoryStream());
            paramWriter.WriteShort((short)Code);
            Data.WriteMFA(paramWriter);

            writer.WriteUShort((ushort)(paramWriter.Tell() + 2));
            writer.WriteWriter(paramWriter);
            paramWriter.Flush();
            paramWriter.Close();
        }

        public override string ToString() => Data.ToString();
    }
}
