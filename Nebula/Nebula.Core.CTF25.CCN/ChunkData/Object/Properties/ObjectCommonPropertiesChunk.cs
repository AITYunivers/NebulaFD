using Nebula.Core.CTF25.CCN.Chunk;
using Nebula.Core.CTF25.CCN.ChunkData.Object.Properties;
using Nebula.Core.CTF25.CCN.ChunkData.Object.Properties.Animation;
using Nebula.Core.CTF25.CCN.ChunkData.Object.Properties.Movement;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.CTF25.CCN.Chunks.Objects.Properties
{
    internal class ObjectCommonPropertiesChunk : CommonChunk
    {
        public uint ObjectFlags;
        public uint NewObjectFlags;
        public uint PreferenceFlags;
        public short[] Qualifiers = [-1, -1, -1, -1, -1, -1, -1, -1];
        public string Identifier = string.Empty;
        public Color Background = Color.White;

        public AnimationBank? Animations;
        public int[]? AlterableValues;
        public uint? AlterableFlags;
        public string[]? AlterableStrings;
        public MovementBank? Movements;
        public object? ObjectData;

        private int _animationOffset, _alterableValuesOffset, _alterableStringsOffset,
        _movementsOffset, _dataOffset, _extensionOffset, _valueOffset, _transitionInOffset,
        _transitionOutOffset, _alterableNamesOffset;

        public override void ReadChunkData(ByteReader reader)
        {
            reader.Skip(4); // Size
            GetOffset(reader, 0);
            GetOffset(reader, 1);
            GetOffset(reader, 2);
            GetOffset(reader, 3);
            GetOffset(reader, 4);
            GetOffset(reader, 5);
            ObjectFlags = reader.ReadUInt();
            Qualifiers = reader.ReadShorts(8);
            GetOffset(reader, 6);
            GetOffset(reader, 7);
            GetOffset(reader, 8);
            NewObjectFlags = reader.ReadUShort();
            PreferenceFlags = reader.ReadUShort();
            Identifier = reader.ReadAscii(4);
            Background = reader.ReadColor();
            _transitionInOffset = reader.ReadInt();
            _transitionOutOffset = reader.ReadInt();
            _alterableNamesOffset = reader.ReadInt();

            if (_animationOffset > 0)
            {
                using ByteReader animationReader = reader.Split(_animationOffset);
                (Animations = new AnimationBank()).Read(animationReader);
            }

            if (_alterableValuesOffset > 0)
            {
                reader.Seek(_alterableValuesOffset);
                AlterableValues = reader.ReadInts(reader.ReadUShort());
                AlterableFlags = reader.ReadUInt();
            }

            if (_alterableStringsOffset > 0)
            {
                reader.Seek(_alterableStringsOffset);
                AlterableStrings = reader.ReadYuniversals(reader.ReadUShort());
            }

            if (_movementsOffset > 0)
            {
                using ByteReader movementReader = reader.Split(_movementsOffset);
                (Movements = new MovementBank()).Read(movementReader);
            }

            if (_dataOffset > 0)
            {
                using ByteReader dataReader = reader.Split(_dataOffset);
                switch (Identifier[..2])
                {
                    // Text
                    case "TE":
                    case "QS":
                        ObjectData = new StringData();
                        ((StringData)ObjectData).Read(dataReader);
                        break;
                    // Counter
                    case "CN":
                    case "SC":
                    case "LI":
                        ObjectData = new CounterData();
                        ((CounterData)ObjectData).Read(dataReader);
                        break;
                    // Formatted Text
                    case "RT":
                        //ObjectFormattedText.ReadCCN(reader);
                        break;
                    // Sub-Application
                    case "CC":
                        //ObjectSubApplication.ReadCCN(reader);
                        break;
                }
            }

            if (_extensionOffset > 0)
            {
                reader.Seek(_extensionOffset);
            }

            if (_valueOffset > 0)
            {
                reader.Seek(_valueOffset);
            }

            if (_transitionInOffset > 0)
            {
                reader.Seek(_transitionInOffset);
            }

            if (_transitionOutOffset > 0)
            {
                reader.Seek(_transitionOutOffset);
            }

            if (_alterableNamesOffset > 0)
            {
                reader.Seek(_alterableNamesOffset);
            }
        }

        public virtual void GetOffset(ByteReader reader, int index)
        {
            ushort Offset = reader.ReadUShort();
            bool min284 = NebulaAPI.PackageData.GetFusionBuild() >= 284;

            switch (index)
            {
                case 0:
                    if (min284)
                        _animationOffset = Offset;
                    else
                        _movementsOffset = Offset;
                    break;
                case 1:
                    if (min284)
                        _movementsOffset = Offset;
                    else
                        _animationOffset = Offset;
                    break;
                case 3:
                    if (!min284)
                        _valueOffset = Offset;
                    break;
                case 4:
                    if (min284)
                        _extensionOffset = Offset;
                    else
                        _dataOffset = Offset;
                    break;
                case 5:
                    if (min284)
                        _valueOffset = Offset;
                    break;
                case 6:
                    if (min284)
                        _dataOffset = Offset;
                    else
                        _extensionOffset = Offset;
                    break;
                case 7:
                    _alterableValuesOffset = Offset;
                    break;
                case 8:
                    _alterableStringsOffset = Offset;
                    break;
            }
        }

        public override void WriteChunkData(ByteWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
