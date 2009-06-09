using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Web;
using Encog.Parse.Tags.Read;
using Encog.Parse.Tags;
using Encog.Util.HTTP;

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
                ReadHTML parse = new ReadHTML(istream);
                StringBuilder buffer = new StringBuilder();
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
