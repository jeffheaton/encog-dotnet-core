// Encog(tm) Artificial Intelligence Framework v2.5
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

#if !SILVERLIGHT
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

            var Uri = new Uri(
                "http://search.yahooapis.com/WebSearchService/V1/webSearch?"
                + str);

            int tries = 0;
            bool done = false;
            while (!done)
            {
                try
                {
                    result = DoSearch(Uri);
                    done = true;
                }
                catch (IOException e)
                {
                    if (tries == 5)
                    {
                        throw (e);
                    }
                    Thread.Sleep(5000);
                }
                tries++;
            }

            return result;
        }
    }
}

#endif