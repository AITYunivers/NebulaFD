using Nebula.Core;
using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Diagnostics;
using System.Reflection;

namespace Nebula.Cli
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("File Path:");
            string filePath = Console.ReadLine()!.Trim('"');
            if (File.Exists(filePath))
            {
                Stopwatch sw = Stopwatch.StartNew();
                ByteReader reader = new ByteReader(new FileStream(filePath, FileMode.Open));
                List<Assembly> assemblies = [];
                foreach (string assemblyFile in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll").Where(x => Path.GetFileName(x).StartsWith("Nebula.Core.") && !x.EndsWith("Core.dll")))
                    assemblies.Add(Assembly.LoadFrom(assemblyFile));
                foreach (Type packageType in assemblies.SelectMany(x => x.GetTypes()).Where(x => x.IsAssignableTo(typeof(IPackageData))))
                {
                    if (packageType == typeof(object))
                        continue;
                    Console.WriteLine($"Checking for file type \"{packageType.FullName}\"");
                    IPackageData? packageData = (IPackageData?)Activator.CreateInstance(packageType);
                    if (packageData != null && packageData.Check(reader))
                    {
                        packageData.Read(reader);
                        break;
                    }
                }
                sw.Stop();
                Console.WriteLine($"File read in {sw.ElapsedMilliseconds}ms.");
            }
            else
                Console.WriteLine("File not found.");
            Console.ReadKey();
        }
    }
}
