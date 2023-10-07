using SapphireD.Core.Utilities;
using System.Drawing;
using System.Text;

namespace SapphireD.Core.Memory
{
    public class ByteWriter : BinaryWriter
    {
        public ByteWriter(Stream input) : base(input)
        {
        }

        public ByteWriter(Stream input, Encoding encoding) : base(input, encoding)
        {
        }



        public ByteWriter(byte[] data) : base(new MemoryStream(data))
        {
        }

        public ByteWriter(string path, FileMode fileMode) : base(new FileStream(path, fileMode))
        {
        }

        public void Seek(Int64 offset, SeekOrigin seekOrigin = SeekOrigin.Begin)
        {
            BaseStream.Seek(offset, seekOrigin);
        }

        public void Skip(Int64 count)
        {
            BaseStream.Seek(count, SeekOrigin.Current);
        }


        public Int64 Tell()
        {
            return BaseStream.Position;
        }

        public Int64 Size()
        {
            return BaseStream.Length;
        }

        public bool Check(int size)
        {
            return Size() - Tell() >= size;
        }

        public bool Eof()
        {
            return BaseStream.Position < BaseStream.Length;
        }

        public void WriteInt8(byte value) => Write(value);
        public void WriteUInt8(sbyte value) => Write(value);

        public void WriteInt16(short value) => Write(value);
        public void WriteInt32(int value) => Write(value);
        public void WriteInt64(long value) => Write(value);

        public void WriteUInt16(ushort value) => Write(value);
        public void WriteUInt32(UInt32 value) => Write(value);
        public void WriteUInt64(ulong value) => Write(value);
        public void WriteSingle(float value) => Write(value);

        public void WriteBytes(byte[] value) => Write(value);
        public void WriteDouble(double value) => Write(value);
        public void WriteString(string value) => Write(value);

        /*public void WriteUniversal(string value, bool addZero = false)
        {
            if (Settings.Unicode) WriteUnicode(value, addZero);
            else WriteAscii(value);
        }*/


        public void WriteAscii(string value)
        {
            var bytes = Encoding.ASCII.GetBytes(value);
            for (int i = 0; i < bytes.Length; i++)
            {
                WriteInt8(bytes[i]);
            }
        }

        public void WriteUnicode(string value, bool appendZero = false)
        {
            var bytes = Encoding.Unicode.GetBytes(value);
            for (int i = 0; i < bytes.Length; i++)
            {
                WriteInt8(bytes[i]);
            }

            if (appendZero) WriteInt16(0);
        }

        public void WriteColor(Color color)
        {
            WriteInt8(color.R);
            WriteInt8(color.G);
            WriteInt8(color.B);
            WriteInt8(color.A);
        }

        public void WriteWriter(ByteWriter toWrite)
        {
            byte[] data = ((MemoryStream)toWrite.BaseStream).GetBuffer();
            Array.Resize<byte>(ref data, (int)toWrite.Tell());
            this.WriteBytes(data);
        }

        public byte[] ToArray()
        {
            return this.GetBuffer();
        }
    }
}