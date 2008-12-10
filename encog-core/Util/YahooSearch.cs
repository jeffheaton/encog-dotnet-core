// Encog Neural Network and Bot Library v1.x (DotNet)
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Web;
using Encog.Bot.HTML;

namespace Encog.Util
{
    class YahooSearch
    {

        private ICollection<Uri> DoSearch(Uri url)
        {

            ICollection<Uri> result = new List<Uri>();
            // submit the search
            WebRequest http = HttpWebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)http.GetResponse();

            using (Stream istream = response.GetResponseStream())
            {
                ParseHTML parse = new ParseHTML(istream);
                StringBuilder buffer = new StringBuilder();
                bool capture = false;

                // parse the results
                int ch;
                while ((ch = parse.Read()) != -1)
                {
                    if (ch == 0)
                    {
                        HTMLTag tag = parse.Tag;
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
                            buffer.Append((char)ch);
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
            MemoryStream mstream = new MemoryStream();
            FormUtility form = new FormUtility(mstream, null);
            form.Add("appid", "YahooDemo");
            form.Add("results", "100");
            form.Add("query", searchFor);
            form.Complete();

            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();

            String str = enc.GetString(mstream.GetBuffer());
            mstream.Dispose();

            Uri Uri = new Uri(
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
