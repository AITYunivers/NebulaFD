using SapphireD.Core.Utilities;
using System.Drawing;
using System.Text;

namespace SapphireD.Core.Memory
{
    public class ByteWriter : BinaryWriter
    {
        public ByteWriter(Stream input) : base(input){}
        public ByteWriter(Stream input, Encoding encoding) : base(input, encoding){}
        public ByteWriter(byte[] data) : base(new MemoryStream(data)){}
        public ByteWriter(string path, FileMode fileMode) : base(new FileStream(path, fileMode)){}
        public void Seek(long offset, SeekOrigin seekOrigin = SeekOrigin.Begin) => BaseStream.Seek(offset, seekOrigin);
        public void Skip(long count) => BaseStream.Seek(count, SeekOrigin.Current);
        public long Tell() => BaseStream.Position;
        public long Size() => BaseStream.Length;
        public bool Check(int size) => Size() - Tell() >= size;
        public bool Eof() => BaseStream.Position < BaseStream.Length;

        public void WriteByte(byte value) => Write(value);
        public void WriteInt8(byte value) => Write(value);
        public void WriteChar(char value) => Write(value);
        public void WriteUInt8(sbyte value) => Write(value);

        public void WriteShort(short value) => Write(value);
        public void WriteInt16(short value) => Write(value);
        public void WriteInt(int value) => Write(value);
        public void WriteInt32(int value) => Write(value);
        public void WriteLong(long value) => Write(value);
        public void WriteInt64(long value) => Write(value);

        public void WriteUShort(ushort value) => Write(value);
        public void WriteUInt16(ushort value) => Write(value);
        public void WriteUInt(uint value) => Write(value);
        public void WriteUInt32(uint value) => Write(value);
        public void WriteULong(ulong value) => Write(value);
        public void WriteUInt64(ulong value) => Write(value);

        public void WriteBytes(byte[] value) => Write(value);
        public void WriteSingle(float value) => Write(value);
        public void WriteFloat(float value) => Write(value);
        public void WriteDouble(double value) => Write(value);
        public void WriteString(string value) => Write(value);

        /*public void WriteUniversal(string value, bool addZero = false)
        {
            if (Settings.Unicode) WriteUnicode(value, addZero);
            else WriteAscii(value);
        }*/


        public void WriteAscii(string value, bool appendZero = false)
        {
            WriteBytes(Encoding.ASCII.GetBytes(value));
            if (appendZero) WriteByte(0);
        }
        public void WriteAscii(string value, int length)
        {
            byte[] toWrite = Encoding.ASCII.GetBytes(value);
            Array.Resize(ref toWrite, length);
            WriteBytes(toWrite);
        }
        public void WriteUnicode(string value, bool appendZero = false)
        {
            WriteBytes(Encoding.Unicode.GetBytes(value));
            if (appendZero) WriteShort(0);
        }
        public void WriteUnicode(string value, int length)
        {
            byte[] toWrite = Encoding.Unicode.GetBytes(value);
            Array.Resize(ref toWrite, length * 2);
            WriteBytes(toWrite);
        }

        public void WriteAutoYunicode(string value)
        {
            WriteShort((short)value.Length);
            WriteShort(-32768);
            WriteUnicode(value);
        }

        public void WriteColor(Color color)
        {
            WriteByte(color.R);
            WriteByte(color.G);
            WriteByte(color.B);
            WriteByte(color.A);
        }

        public void WriteWriter(ByteWriter toWrite)
        {
            byte[] data = ((MemoryStream)toWrite.BaseStream).GetBuffer();
            Array.Resize(ref data, (int)toWrite.Tell());
            WriteBytes(data);
        }

        public byte[] ToArray() => this.GetBuffer();
    }
}