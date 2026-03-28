using System.Runtime.CompilerServices;
using Nebula.Core.Memory;

[assembly: InternalsVisibleTo("Nebula")]
namespace Nebula.Core.Utilities
{
    internal static class DebugDumper
    {
        public static void Clean()
        {
            if (!Parameters.DumpAllChunks)
                return;

            string dir = Path.Combine(Path.GetDirectoryName(NebulaCore.FilePath)!, "DebugDumps");
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }

        public static void Dump(string fileName, ByteReader reader, int size, string category = "", bool increment = false)
		{
			if (!Parameters.DumpAllChunks)
				return;

			long pos = reader.Tell();
			string dir = Path.Combine(Path.GetDirectoryName(NebulaCore.FilePath)!, "DebugDumps", category);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (increment)
            {
                ulong i = 0;
                while (File.Exists(Path.Combine(dir, fileName + i + ".bin")))
                    i++;
                fileName += i;
			}
            string path = Path.Combine(dir, fileName + ".bin");
            using FileStream file = File.Open(path, FileMode.Create);
            file.Write(reader.ReadBytes(size));
            reader.Seek(pos);
		}
	}
}
