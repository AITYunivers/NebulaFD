using Nebula.Core;
using Nebula.Core.ImageTranslation;
using Nebula.Core.Memory;
using System.Buffers;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;

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
                    NebulaAPI.PackageData = (IPackageData)Activator.CreateInstance(packageType)!;
                    if (NebulaAPI.PackageData != null && NebulaAPI.PackageData.Check(reader))
                    {
                        NebulaAPI.PackageData.Read(reader);
                        char key = Console.ReadKey().KeyChar;
                        if (key == 'd' && NebulaAPI.PackageData is Core.CTF25.MFA.PackageData mfaData)
                        {
                            Console.WriteLine("Dumping images from MFA");
                            if (!Directory.Exists("tempimgs"))
                                Directory.CreateDirectory("tempimgs");

                            Stopwatch imgSw = Stopwatch.StartNew();
                            Stopwatch imgSw1 = new Stopwatch();
                            Stopwatch imgSw2 = new Stopwatch();
                            Stopwatch imgSw3 = new Stopwatch();
                            List<Task> saveTasks = [];

                            Core.CTF25.MFA.Image.ImageItem[] images = [.. mfaData.Images.Where(x => x.Type == Core.Data.Image.EImageType.RGBMasked)];

                            foreach (Core.CTF25.MFA.Image.ImageItem imageData in images)
                            {
                                imgSw1.Start();
                                byte[] imgData = imageData.GetImageData(reader, out int dataSize);
                                MemoryStream dataStream = new(imgData, 0, dataSize, false);
                                imgSw1.Stop();

                                imgSw2.Start();
                                int imgDataSize = imageData.Width * imageData.Height * 4;
                                byte[] tempImgData = ArrayPool<byte>.Shared.Rent(imgDataSize);
                                RGBMaskedToRGBA.Translate(dataStream, imageData, tempImgData.AsSpan(0, imgDataSize));
                                imgSw2.Stop();

#pragma warning disable CA1416 // Validate platform compatibility
                                if (!imgSw3.IsRunning)
                                    imgSw3.Start();

                                saveTasks.Add(Task.Factory.StartNew(() =>
                                {
                                    Bitmap bmp = new Bitmap(imageData.Width, imageData.Height);
                                    BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, imageData.Width, imageData.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                                    Marshal.Copy(tempImgData, 0, bmpData.Scan0, imgDataSize);
                                    bmp.UnlockBits(bmpData);
                                    bmp.Save($"tempimgs\\img-{imageData.Handle}.png");
                                    bmp.Dispose();
                                    ArrayPool<byte>.Shared.Return(tempImgData);
                                }));
#pragma warning restore CA1416 // Validate platform compatibility

                                dataStream.Dispose();
                                ArrayPool<byte>.Shared.Return(imgData);
                            }

                            Task.WaitAll(saveTasks);
                            imgSw3.Stop();

                            Console.WriteLine($"Wrote {images.Length} images in {imgSw.Elapsed.TotalSeconds:F2} seconds");
                            Console.WriteLine($"Took {imgSw1.Elapsed.TotalSeconds} seconds reading/decompressing image data");
                            Console.WriteLine($"Took {imgSw2.Elapsed.TotalSeconds} seconds translating");
                            Console.WriteLine($"Took {imgSw3.Elapsed.TotalSeconds} seconds saving to file");
                            Console.ReadKey();
                        }
                        return;
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
