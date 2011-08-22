using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Encog.Util
{
    public class SerializeRoundTrip
    {
        public static Object RoundTrip(Object obj)
        {
            // first serialize to memory
            var ms = new MemoryStream();
            var b = new BinaryFormatter();
            b.Serialize(ms, obj);

            // now reload
            ms.Seek(0, SeekOrigin.Begin);
            Object result = b.Deserialize(ms);
            return result;
        }
    }
}