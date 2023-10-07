namespace SapphireD.Core.Memory
{
    public static class Decryption
    {
        public static byte[] _decryptionKey;
        public static byte MagicChar = 54;

        public static byte[] KeyString(string str)
        {
            var result = new List<byte>();
            result.Capacity = str.Length * 2;
            foreach (char code in str)
            {
                if ((code & 0xFF) != 0)
                    result.Add((byte)(code & 0xFF));

                if (((code >> 8) & 0xFF) != 0)
                    result.Add((byte)((code >> 8) & 0xFF));
            }
            return result.ToArray();
        }

        public static byte[] MakeKeyCombined(byte[] data)
        {
            int dataLen = data.Length;
            Array.Resize(ref data, 256);

            byte lastKeyByte = MagicChar;
            byte v34 = MagicChar;

            for (int i = 0; i <= dataLen; i++)
            {
                v34 = (byte)((v34 << 7) + (v34 >> 1));
                data[i] ^= v34;
                lastKeyByte += (byte)(data[i] * ((v34 & 1) + 2));
            }

            data[dataLen + 1] = lastKeyByte;
            return data;
        }

        public static void MakeKey(string data1, string data2, string data3)
        {
            var bytes = new List<byte>();
            bytes.AddRange(KeyString(data1 ?? ""));
            bytes.AddRange(KeyString(data2 ?? ""));
            bytes.AddRange(KeyString(data3 ?? ""));
            _decryptionKey = MakeKeyCombined(bytes.ToArray());
            InitDecryptionTable(_decryptionKey, Decryption.MagicChar);
        }

        public static byte[] DecodeMode3(byte[] chunkData, int chunkId, out int decompressed)
        {
            var reader = new ByteReader(chunkData);
            var decompressedSize = reader.ReadUInt32();

            var rawData = reader.ReadBytes((int)reader.Size());

            if ((chunkId & 1) == 1 && SapDCore.Build > 284)
                rawData[0] ^= (byte)((byte)(chunkId & 0xFF) ^ (byte)(chunkId >> 0x8));

            TransformChunk(rawData);

            using (var data = new ByteReader(rawData))
            {
                var compressedSize = data.ReadUInt32();
                decompressed = (int)decompressedSize;
                return Decompressor.DecompressBlock(data, (int)compressedSize);
            }
        }

        private static byte[] decodeBuffer = new byte[256];
        public static bool valid;

        public static bool InitDecryptionTable(byte[] magic_key, byte magic_char)
        {
            for (int i = 0; i < 256; i++)
                decodeBuffer[i] = (byte)i;

            Func<byte, byte> rotate = (byte value) => (byte)((value << 7) | (value >> 1));

            byte accum = magic_char;
            byte hash = magic_char;

            bool never_reset_key = true;

            byte i2 = 0;
            byte key = 0;
            for (uint i = 0; i < 256; ++i, ++key)
            {

                hash = rotate(hash);

                if (never_reset_key)
                {
                    accum += ((hash & 1) == 0) ? (byte)2 : (byte)3;
                    accum *= magic_key[key];
                }

                if (hash == magic_key[key])
                {
                    hash = rotate(magic_char);
                    key = 0;

                    never_reset_key = false;
                }

                i2 += (byte)((hash ^ magic_key[key]) + decodeBuffer[i]);

                (decodeBuffer[i2], decodeBuffer[i]) = (decodeBuffer[i], decodeBuffer[i2]);
            }
            valid = true;
            return true;
        }

        public static bool TransformChunk(byte[] chunk)
        {
            if (!valid) return false;
            byte[] tempBuf = new byte[256];
            Array.Copy(decodeBuffer, tempBuf, 256);

            byte i = 0;
            byte i2 = 0;
            for (int j = 0; j < chunk.Length; j++)
            {
                ++i;
                i2 += (byte)tempBuf[i];
                (tempBuf[i2], tempBuf[i]) = (tempBuf[i], tempBuf[i2]);
                var xor = tempBuf[(byte)(tempBuf[i] + tempBuf[i2])];
                chunk[j] ^= xor;
            }
            return true;
        }
    }
}