using System;
using System.IO;
using System.Reflection;

namespace Encog.Util.File
{
    /// <summary>
    /// A stream that is used to read resources.
    /// </summary>
    public class ResourceInputStream
    {
        /// <summary>
        /// Construct a location to read from the specified resource.
        /// An example of the format for a file stored this way is:
        /// org/encog/data/classes.txt
        /// </summary>
        ///
        /// <param name="resource">The resource to read from.</param>
        public static Stream OpenResourceInputStream(String resource)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(resource);
            return stream;
        }
    }
}