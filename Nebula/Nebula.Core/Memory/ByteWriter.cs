using Nebula.Core.Data;
using Nebula.Core.Utilities;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Nebula.Core.Memory
{
    public class ByteWriter : BinaryWriter
    {
        public ByteWriter() : base(new MemoryStream()){}
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
        public void WriteSingle(float value) => Write(value);
        public void WriteFloat(float value) => Write(value);
        public void WriteDouble(double value) => Write(value);
        public void WriteString(string value) => Write(value);
        public void WriteBool(bool value) => Write(value);
        public void WriteBool4(bool value) => Write(value ? 1 : 0);

        /*public void WriteYuniversal(string value, bool addZero = false)
        {
            if (Settings.Yunicode) WriteUnicode(value, addZero);
            else WriteAscii(value);
        }*/

        public void WriteBytes(byte[] value, bool prependSize = false)
        {
            if (prependSize)
                WriteInt(value.Length);
            Write(value);
        }
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
        public void WriteYunicode(string value, bool appendZero = false)
        {
            WriteBytes(Encoding.Unicode.GetBytes(value));
            if (appendZero) WriteShort(0);
        }
        public void WriteYunicode(string value, int length)
        {
            byte[] toWrite = Encoding.Unicode.GetBytes(value);
            Array.Resize(ref toWrite, length * 2);
            WriteBytes(toWrite);
        }

        public void WriteAutoYunicode(string value)
        {
            WriteShort((short)value.Length);
            WriteShort(-32768);
            WriteYunicode(value);
        }

        public void WriteColor(Color color)
        {
            WriteByte(color.R);
            WriteByte(color.G);
            WriteByte(color.B);
            WriteByte(color.A);
        }

        public void WriteColors(Color[] colors)
        {
            foreach (Color c in colors)
                WriteColor(c);
        }

        public void WriteColors(ICollection<Color> colors)
        {
            foreach (Color c in colors)
                WriteColor(c);
        }

        public void WriteShorts(short[] shorts)
        {
            foreach (short s in shorts)
                WriteShort(s);
        }

        public void WriteShorts(ICollection<short> shorts)
        {
            foreach (short s in shorts)
                WriteShort(s);
        }

        public void WriteUShorts(ushort[] shorts)
        {
            foreach (ushort s in shorts)
                WriteUShort(s);
        }

        public void WriteUShorts(ICollection<ushort> shorts)
        {
            foreach (ushort s in shorts)
                WriteUShort(s);
        }

        public void WriteInts(int[] ints)
        {
            foreach (int i in ints)
                WriteInt(i);
        }

        public void WriteInts(ICollection<int> ints)
        {
            foreach (int i in ints)
                WriteInt(i);
        }

        public void WriteUInts(uint[] ints)
        {
            foreach (uint i in ints)
                WriteUInt(i);
        }

        public void WriteUInts(ICollection<uint> ints)
        {
            foreach (uint i in ints)
                WriteUInt(i);
        }

        public void WriteLongs(long[] longs)
        {
            foreach (long l in longs)
                WriteLong(l);
        }

        public void WriteLongs(ICollection<long> longs)
        {
            foreach (long l in longs)
                WriteLong(l);
        }

        public void WriteULongs(ulong[] longs)
        {
            foreach (ulong l in longs)
                WriteULong(l);
        }

        public void WriteULongs(ICollection<ulong> longs)
        {
            foreach (ulong l in longs)
                WriteULong(l);
        }

        public void WriteIWritables<T>(T[] values) where T : IWritable
        {
            foreach (IWritable value in values)
                value?.Write(this);
        }

        public void WriteIWritables<T>(ICollection<T> values) where T : IWritable
        {
            foreach (IWritable value in values)
                value?.Write(this);
        }

        public void WriteIWritablesWithOffsets<T1, T2>(T1[] values)
            where T1 : IWritable
            where T2 : unmanaged, INumber<T2>
        {
            long[] offsets = new long[values.Length];
            long offsetsSize = Marshal.SizeOf<T2>() * values.Length;
            using ByteWriter writer = new ByteWriter();
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == null)
                {
                    offsets[i] = 0;
                    continue;
                }

                offsets[i] = writer.Tell() + offsetsSize;
                values[i].Write(writer);
            }

            for (int i = 0; i < offsets.Length; i++)
            {
                long offset = offsets[i];
                T2 offsetCast = Unsafe.As<long, T2>(ref offset);
                int size = Marshal.SizeOf<T2>();
                byte[] bytes = new byte[size];
                MemoryMarshal.Write(bytes.AsSpan(), in offsetCast);
                WriteBytes(bytes);
            }

            WriteWriter(writer);
        }

        public void WriteIWritablesWithOffsets<T1, T2>(ICollection<T1> values)
            where T1 : IWritable
            where T2 : unmanaged, INumber<T2>
        {
            long[] offsets = new long[values.Count];
            long offsetsSize = Marshal.SizeOf<T2>() * values.Count;
            using ByteWriter writer = new ByteWriter();
            for (int i = 0; i < values.Count; i++)
            {
                T1? value = values.ElementAt(i);
                if (value == null)
                {
                    offsets[i] = 0;
                    continue;
                }

                offsets[i] = writer.Tell() + offsetsSize;
                value.Write(writer);
            }

            for (int i = 0; i < offsets.Length; i++)
            {
                long offset = offsets[i];
                T2 offsetCast = Unsafe.As<long, T2>(ref offset);
                int size = Marshal.SizeOf<T2>();
                byte[] bytes = new byte[size];
                MemoryMarshal.Write(bytes.AsSpan(), in offsetCast);
                WriteBytes(bytes);
            }

            WriteWriter(writer);
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