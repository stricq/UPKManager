using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;


namespace UpkManager.Repository.Extensions {

  internal static class StreamExtensions {

    public static String GetHash<T>(this FileStream stream, int bufferSize) where T : HashAlgorithm {
      MethodInfo create = typeof(T).GetMethod("Create", new Type[] { });

      using(BufferedStream buffered = new BufferedStream(stream, bufferSize)) {
        using(T crypt = (T) create?.Invoke(null, null)) {
          byte[] hashBytes = crypt?.ComputeHash(buffered);

          return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
      }
    }

  }

}
