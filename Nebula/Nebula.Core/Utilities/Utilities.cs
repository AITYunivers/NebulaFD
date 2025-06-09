using Nebula.Core.Memory;
using System.Reflection;

namespace Nebula.Core.Utilities
{
    public static class Utilities
    {
        public static string ClearName(string ogName, params char[] pardons)
        {
            List<char> invalidChars = Path.GetInvalidFileNameChars().ToList();
            invalidChars.Add('?');
            foreach (char pardon in pardons)
                invalidChars.Remove(pardon);
            var str = string.Join("", ogName.Split(invalidChars.ToArray())).TrimEnd('.');
            return str;
        }

        public static byte[] GetBuffer(this ByteWriter writer)
        {
            var buf = ((MemoryStream)writer.BaseStream).GetBuffer();
            Array.Resize(ref buf, (int)writer.Size());
            return buf;
        }

        public static IEnumerable<Type> TypesImplementingInterface(Type interfaceType, params Type[] desiredConstructorSignature)
        {
            if (interfaceType == null) throw new ArgumentNullException("interfaceType");
            if (!interfaceType.IsInterface) throw new ArgumentOutOfRangeException("interfaceType");

            return AppDomain
                   .CurrentDomain
                   .GetAssemblies()
                   .SelectMany(a => a.GetTypes())
                   .Where(t => t.IsAssignableFrom(interfaceType))
                   .Where(t => !t.IsInterface)
                   .Where(t => t.GetConstructor(desiredConstructorSignature) != null);
        }

        public static T ConstructInstance<T>(Type t, params object[] parameterList)
        {
            Type[] signature = parameterList.Select(p => p.GetType()).ToArray();
            ConstructorInfo constructor = t.GetConstructor(signature)!;
            return (T)constructor.Invoke(parameterList);
        }
    }
}

