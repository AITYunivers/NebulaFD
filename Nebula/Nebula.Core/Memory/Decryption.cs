namespace Nebula.Core.Memory
{
    public static class Decryption
    {
        public static byte[]? DecryptionKey;

        public static byte[] KeyString(string str)
        {
            var result = new List<byte>();
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

            byte lastKeyByte = 0;
            byte v34 = 0;

            for (int i = 0; i < dataLen; i++)
            {
                v34 = (byte)((v34 << 7) + (v34 >> 1));
                data[i] ^= v34;
                lastKeyByte += (byte)(data[i] * ((v34 & 1) + 2));
            }

            Array.Resize(ref data, 128);
            Array.Resize(ref data, 256);
            if (dataLen < 255)
                data[dataLen + 1] = lastKeyByte;
            return data;
        }

        public static void MakeKey(params string[] data)
        {
            var bytes = new List<byte>();
            foreach (string s in data)
                bytes.AddRange(KeyString(s ?? ""));
            DecryptionKey = MakeKeyCombined(bytes.ToArray());
            InitDecryptionTable(DecryptionKey);
        }

        public static byte[] DecompressXor(byte[] chunkData, int chunkId)
        {
            using ByteReader reader = new ByteReader(chunkData);
            uint decompressedSize = reader.ReadUInt();

            byte[] rawData = reader.ReadBytes((int)reader.Size());

            if ((chunkId & 1) == 1)
                rawData[0] ^= (byte)((byte)(chunkId & 0xFF) ^ (byte)(chunkId >> 0x8));

            byte[] xorData = DecryptXor(rawData);

            using (ByteReader data = new ByteReader(xorData))
            {
                int compressedSize = data.ReadInt();
                return Decompressor.DecompressZlib(data, compressedSize);
            }
        }

        private static byte[] decodeBuffer = new byte[256];
        public static bool valid;

        public static bool InitDecryptionTable(byte[] magic_key)
        {
            decodeBuffer = [.. Enumerable.Range(0, 256).Select(i => (byte)i)];
            static byte rotate(byte value) => (byte)((value << 7) | (value >> 1));

            byte accum = 0;
            byte hash = 0;
            bool neverResetKey = true;

            byte i2 = 0;
            byte key = 0;
            for (uint i = 0; i < 256; ++i, ++key)
            {

                hash = rotate(hash);

                if (neverResetKey)
                {
                    accum += ((hash & 1) == 0) ? (byte)2 : (byte)3;
                    accum *= magic_key[key];
                }

                if (hash == magic_key[key])
                {
                    hash = rotate(0);
                    key = 0;

                    neverResetKey = false;
                }

                i2 += (byte)((hash ^ magic_key[key]) + decodeBuffer[i]);

                (decodeBuffer[i2], decodeBuffer[i]) = (decodeBuffer[i], decodeBuffer[i2]);
            }
            valid = true;
            return true;
        }

        // RC4 Cipher Algorithm with a custom Key-Gen
        public static byte[] DecryptXor(byte[] chunk)
        {
            if (!valid)
                return chunk;

            byte[] tempBuf = new byte[256];
            Array.Copy(decodeBuffer, tempBuf, 256);

            byte i = 0;
            byte i2 = 0;
            for (int j = 0; j < chunk.Length; j++)
            {
                ++i;
                i2 += tempBuf[i];
                (tempBuf[i2], tempBuf[i]) = (tempBuf[i], tempBuf[i2]);
                byte xor = tempBuf[(byte)(tempBuf[i] + tempBuf[i2])];
                chunk[j] ^= xor;
            }

            return chunk;
        }
    }
}