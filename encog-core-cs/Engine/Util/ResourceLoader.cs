using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Encog.Engine.Util
{
    /// <summary>
    /// Used to load data from resources.
    /// </summary>
    public sealed class ResourceLoader
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        private ResourceLoader()
        {
        }

        /// <summary>
        /// Create a stream to read the resource.
        /// </summary>
        /// <param name="resource">The resource to load.</param>
        /// <returns>A stream.</returns>
        public static Stream CreateStream(String resource)
        {
#if !SILVERLIGHT
            Stream result = null;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();          

            foreach (Assembly a in assemblies)
            {
                result = a.GetManifestResourceStream(resource);
                if (result != null)
                    break;
            }

            return result;
#else 
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(resource); 
#endif
        }

        /// <summary>
        /// Load a string.
        /// </summary>
        /// <param name="resource">The resource to load.</param>
        /// <returns>The loaded string.</returns>
        public static String LoadString(String resource)
        {
            StringBuilder result = new StringBuilder();
            Stream istream = CreateStream(resource);
            StreamReader sr = new StreamReader(istream);

            String line;
            while ((line = sr.ReadLine()) != null)
            {
                result.Append(line);
                result.Append("\r\n");
            }
            sr.Close();
            istream.Close();

            return result.ToString();
        }


    }
}
