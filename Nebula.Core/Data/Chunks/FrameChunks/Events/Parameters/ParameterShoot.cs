using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterShoot : ParameterChunk
    {
        public BitDict ShootFlags = new BitDict( // Shoot Flags
            "", "", "CalculateDirection" // Launch in select directions Disabled
        );

        public short ObjectInfoParent;
        public short X;
        public short Y;
        public short Slope;
        public short Angle;
        public int Direction;
        public short TypeParent;
        public short ObjectInfoList;
        public short Layer;

        public ushort ObjectInstance;
        public ushort ObjectInfo;
        public short ShootSpeed;

        public ParameterShoot()
        {
            ChunkName = "ParameterShoot";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            ObjectInfoParent = reader.ReadShort();
            ShootFlags.Value = reader.ReadUShort();
            X = reader.ReadShort();
            Y = reader.ReadShort();
            Slope = reader.ReadShort();
            Angle = reader.ReadShort();
            Direction = reader.ReadInt();
            TypeParent = reader.ReadShort();
            ObjectInfoList = reader.ReadShort();
            Layer = reader.ReadShort();

            ObjectInstance = reader.ReadUShort();
            ObjectInfo = reader.ReadUShort();
            reader.Skip(4);
            ShootSpeed = reader.ReadShort();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteShort(ObjectInfoParent);
            writer.WriteUShort((ushort)ShootFlags.Value);
            writer.WriteShort(X);
            writer.WriteShort(Y);
            writer.WriteShort(Slope);
            writer.WriteShort(Angle);
            writer.WriteInt(Direction);
            writer.WriteShort(TypeParent);
            writer.WriteShort(ObjectInfoList);
            writer.WriteShort(Layer);

            writer.WriteUShort(ObjectInstance);
            writer.WriteUShort(ObjectInfo);
            writer.WriteInt(0);
            writer.WriteShort(ShootSpeed);
        }

        public override string ToString()
        {
            string output = NebulaCore.PackageData.FrameItems.Items[ObjectInfo].Name;
            if (!ShootFlags["CalculateDirection"])
                output += " toward " + GetDirection();
            return output + " at speed " + ShootSpeed;
        }

        public string GetDirection()
        {
            string output = "...........";
            BitDict dict = new BitDict();
            dict.Value = (uint)Direction;
            if (dict.Value == uint.MaxValue)
                output += ".";
            else if (dict["31"])
                output += "..........";
            else if (dict["30"])
                output += ".........";
            else if (dict["29"] || dict["28"] || dict["27"])
                output += "........";
            else if (dict["26"] || dict["25"] || dict["24"])
                output += ".......";
            else if (dict["20"] || dict["21"] || dict["22"] || dict["23"])
                output += "......";
            else if (dict["17"] || dict["18"] || dict["19"])
                output += ".....";
            else if (dict["14"] || dict["15"] || dict["16"])
                output += "....";
            else if (dict["10"] || dict["11"] || dict["12"] || dict["13"])
                output += "...";
            else if (dict["7"] || dict["8"] || dict["9"])
                output += "..";
            else if (dict["4"] || dict["5"] || dict["6"])
                output += ".";
            return output;
        }
    }
}
