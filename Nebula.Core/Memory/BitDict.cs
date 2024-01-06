namespace Nebula.Core.Memory
{
    public static class ByteFlag
    {
        public static bool GetFlag(uint flagbyte, int pos)
        {
            uint mask = (uint)(1 << pos);
            uint result = flagbyte & mask;
            return result == mask;
        }
    }

    public class BitDict
    {
        public string[] Keys = new string[0];
        public uint Value { get; set; }

        public BitDict()
        {
            List<string> keys = new List<string>();
            for (int i = 0; i < 32; i++)
                keys.Add(i.ToString());
            Keys = keys.ToArray();
        }

        public BitDict(params string[] keys) => Keys = keys;
        public bool this[string key]
        {
            get => GetFlag(key);
            set => SetFlag(key, value);
        }

        public bool GetFlag(string key)
        {
            int pos = Array.IndexOf(Keys, key);
            if (pos >= 0)
                return (Value & ((uint)Math.Pow(2, pos))) != 0;
            return false;
        }

        public void SetFlag(string key, bool flag)
        {
            if (flag)
                Value |= (uint)Math.Pow(2, Array.IndexOf(Keys, key));
            else
                Value &= ~(uint)Math.Pow(2, Array.IndexOf(Keys, key));
        }

        public override string ToString()
        {
            Dictionary<string, bool> actualKeys = new Dictionary<string, bool>();
            foreach (var key in Keys)
                actualKeys[key] = this[key];

            return string.Join(";\n", actualKeys.Select(kv => kv.Key + ": " + kv.Value).ToArray());
        }
    }
}