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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Encog.Parse.Tags;
using Encog.Parse.Tags.Read;
using Encog.Util.HTTP;

namespace Encog.Util
{
    /// <summary>
    /// Perform a search using Yahoo.
    /// </summary>
    public class YahooSearch
    {
        /// <summary>
        /// Perform a Yahoo search.
        /// </summary>
        /// <param name="url">The REST URL.</param>
        /// <returns>The search results.</returns>
        private ICollection<Uri> DoSearch(Uri url)
        {
            ICollection<Uri> result = new List<Uri>();
            // submit the search
            WebRequest http = WebRequest.Create(url);
            var response = (HttpWebResponse) http.GetResponse();

            using (Stream istream = response.GetResponseStream())
            {
                var parse = new ReadHTML(istream);
                var buffer = new StringBuilder();
                bool capture = false;

                // parse the results
                int ch;
                while ((ch = parse.Read()) != -1)
                {
                    if (ch == 0)
                    {
                        Tag tag = parse.LastTag;
                        if (tag.Name.Equals("Url", StringComparison.CurrentCultureIgnoreCase))
                        {
                            buffer.Length = 0;
                            capture = true;
                        }
                        else if (tag.Name.Equals("/Url", StringComparison.CurrentCultureIgnoreCase))
                        {
                            result.Add(new Uri(buffer.ToString()));
                            buffer.Length = 0;
                            capture = false;
                        }
                    }
                    else
                    {
                        if (capture)
                        {
                            buffer.Append((char) ch);
                        }
                    }
                }
            }

            response.Close();

            return result;
        }

        /// <summary>
        /// Perform a Yahoo search.
        /// </summary>
        /// <param name="searchFor">What are we searching for.</param>
        /// <returns>The URLs that contain the specified item.</returns>
        public ICollection<Uri> Search(String searchFor)
        {
            ICollection<Uri> result = null;

            // build the Uri
            var mstream = new MemoryStream();
            var form = new FormUtility(mstream, null);
            form.Add("appid", "YahooDemo");
            form.Add("results", "100");
            form.Add("query", searchFor);
            form.Complete();

            var enc = new ASCIIEncoding();

            String str = enc.GetString(mstream.GetBuffer());
            mstream.Dispose();

            var uri = new Uri(
                "http://search.yahooapis.com/WebSearchService/V1/webSearch?"
                + str);

            int tries = 0;
            bool done = false;
            while (!done)
            {
                try
                {
                    result = DoSearch(uri);
                    done = true;
                }
                catch (IOException e)
                {
                    if (tries == 5)
                    {
                        throw;
                    }
                    Thread.Sleep(5000);
                }
                tries++;
            }

            return result;
        }
    }
}
