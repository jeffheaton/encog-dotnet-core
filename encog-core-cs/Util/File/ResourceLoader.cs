//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using System.IO;
using System.Reflection;
using System.Text;
using Encog.Util.Logging;

namespace Encog.Util.File
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
        /// <param name="resource">The resource to load.  This should be in the form Encog.Resources.classes.txt</param>
        /// <returns>A stream.</returns>
        public static Stream CreateStream(String resource)
        {
            Stream result = null;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblies)
            {
                if (a.IsDynamic) // this is the fix (https://github.com/encog/encog-dotnet-core/issues/51)
                    continue;
                try
                {
                    result = a.GetManifestResourceStream(resource);
                    if (result != null)
                        break;
                }
                catch(Exception ex)
                {
                    // Mostly, ignore the error in case one assembly could not be accessed.
                    // This is probably not necessary given the Dynamic check above.
                    EncogLogging.Log(ex);
                }
            }

            if (result == null)
            {
                throw new EncogError("Cannot create stream");
            }

            return result;
        }

        /// <summary>
        /// Load a string.
        /// </summary>
        /// <param name="resource">The resource to load.</param>
        /// <returns>The loaded string.</returns>
        public static String LoadString(String resource)
        {
            var result = new StringBuilder();
            Stream istream = CreateStream(resource);
            var sr = new StreamReader(istream);

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
