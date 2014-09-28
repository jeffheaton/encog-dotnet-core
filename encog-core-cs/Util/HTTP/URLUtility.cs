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
using System.Linq;
using System.Text;

namespace Encog.Util.HTTP
{
    /// <summary>
    /// URLUtility: THis class contains several useful methods
    /// for dealing with URL's.
    /// </summary>
    public class URLUtility
    {
        /// <summary>
        /// The name of an HTML index file.
        /// </summary>
        public const String IndexFile = "index.html";

        /// <summary>
        /// Construct a URL from a string.
        /// </summary>
        /// <param name="baseURL">The page that the URL was found on.</param>
        /// <param name="url">The URL found.</param>
        /// <param name="stripFragment">Should fragments be stripped.  Fragments are the part of a URL after the # sign.  They do not specify actual pages, but rather part of a page.  As a result, they are usually not needed by a spider or bot.</param>
        /// <returns>The constructed URL.</returns>
        public static Uri ConstructURL(Uri baseURL, String url, bool stripFragment)
        {
            var result = new Uri(baseURL, url);
            String file = result.PathAndQuery;
            String protocol = result.Scheme;
            String host = result.Host;
            String fragment = result.Fragment;
            int port = result.Port;

            file = file.Replace(" ", "%20");

            var sb = new StringBuilder();
            sb.Append(protocol);
            sb.Append("://");
            sb.Append(host);
            if (port == 80 && String.Compare(protocol, "http") != 0)
            {
                sb.Append(':');
                sb.Append("80");
            }
            else if (port == 443 && String.Compare(protocol, "https") != 0)
            {
                sb.Append(':');
                sb.Append("443");
            }

            if (!file.StartsWith("/"))
                sb.Append('/');
            sb.Append(file);


            if (!stripFragment)
            {
                sb.Append('#');
                sb.Append(fragment);
            }

            return new Uri(sb.ToString());
        }

        /// <summary>
        /// Does the URL contain invalid characters?
        /// </summary>
        /// <param name="url">The URL</param>
        /// <returns>True if the URL contains invalid characters.</returns>
        public static bool ContainsInvalidURLCharacters(String url)
        {
            return url.Any(ch => ch > 255);
        }

        /// <summary>
        /// Convert a filename for local storage. Also create the
        /// directory tree.
        /// </summary>
        /// <param name="basePath">The local path that forms the base of the
        /// downloaded web tree.</param>
        /// <param name="url">The URL path.</param>
        /// <param name="mkdir">True if a directory structure should be created
        /// to support this file.  Directories will only be
        /// created, if needed.</param>
        /// <returns></returns>
        public static String ConvertFilename(String basePath, Uri url, bool mkdir)
        {
            String result = basePath;

            // append the host name
            String host = url.Host.Replace('.', '_');
            result = Path.Combine(result, host);
            Directory.CreateDirectory(result);

            // determine actual filename
            int lastSegment = url.Segments.Length;
            if (lastSegment > 0)
                lastSegment--;
            String filename = url.Segments[lastSegment];
            if (filename.Equals('/'))
                filename = IndexFile;

            for (int i = 0; i < lastSegment; i++)
            {
                String segment = url.Segments[i];
                if (!segment.Equals("/"))
                {
                    result = Path.Combine(result, segment);
                    if (mkdir)
                        Directory.CreateDirectory(result);
                }
            }

            // attach name
            if (filename.EndsWith("/"))
            {
                String dir = filename.Substring(0, filename.Length - 1);
                result = Path.Combine(result, dir);
                if (mkdir)
                    Directory.CreateDirectory(result);
                filename = IndexFile;
            }
            else if (filename.IndexOf('.') == -1)
            {
                filename = "/" + IndexFile;
            }


            result = Path.Combine(result, filename);

            result = result.Replace('?', '_');
            return result;
        }
    }
}
