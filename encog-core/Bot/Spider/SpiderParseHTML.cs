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
using System.Text;
using System.IO;
using Encog.Bot.HTML;
using Encog.Bot.Spider.Workload;
using Encog.Bot.Spider.Logging;


namespace Encog.Bot.Spider
{
    /// <summary>
    /// SpiderParseHTML: This class layers on top of the
    /// ParseHTML class and allows the spider to extract what
    /// link information it needs. A SpiderParseHTML class can be
    /// used just like the ParseHTML class, with the spider
    /// gaining its information in the background.
    /// </summary>
    public class SpiderParseHTML : ParseHTML
    {
        /// <summary>
        /// The stream that the parser is reading from.
        /// </summary>
        public SpiderInputStream Stream
        {
            get
            {
                return stream;
            }
        }

        /// <summary>
        /// The Spider that this page is being parsed for.
        /// </summary>
        private Spider spider;

        /// <summary>
        /// The URL that is being parsed.
        /// </summary>
        private Uri baseURL;

        /// <summary>
        /// The depth of the page being parsed.
        /// </summary>
        private int depth;

        /// <summary>
        /// The InputStream that is being parsed.
        /// </summary>
        private SpiderInputStream stream;

        /// <summary>
        /// Construct a SpiderParseHTML object. This object allows
        /// you to parse HTML, while the spider collects link
        /// information in the background.
        /// </summary>
        /// <param name="baseURL">The URL that is being parsed, this is used for relative links.</param>
        /// <param name="istream">The InputStream being parsed.</param>
        /// <param name="spider">The Spider that is parsing.</param>
        public SpiderParseHTML(Uri baseURL, SpiderInputStream istream, Spider spider)
            : base(istream)
        {
            this.stream = istream;
            this.spider = spider;
            this.baseURL = baseURL;
            this.depth = spider.Workload.GetDepth(baseURL);
        }

        /// <summary>
        /// Read a single character. This function will process any
        /// tags that the spider needs for navigation, then pass
        /// the character on to the caller. This allows the spider
        /// to transparently gather its links.
        /// </summary>
        /// <returns></returns>
        public override int Read()
        {
            int result = base.Read();
            if (result == 0)
            {
                HTMLTag tag = Tag;

                if (String.Compare(tag.Name, "a", true) == 0)
                {
                    String href = tag["href"];
                    HandleA(href);
                }
                else if (String.Compare(tag.Name, "img", true) == 0)
                {
                    String src = tag["src"];
                    AddURL(src, Spider.URLType.IMAGE);
                }
                else if (String.Compare(tag.Name, "style", true) == 0)
                {
                    String src = tag["src"];
                    AddURL(src, Spider.URLType.STYLE);
                }
                else if (String.Compare(tag.Name, "link", true) == 0)
                {
                    String href = tag["href"];
                    AddURL(href, Spider.URLType.SCRIPT);
                }
                else if (String.Compare(tag.Name, "base", true) == 0)
                {
                    String href = tag["href"];
                    this.baseURL = new Uri(this.baseURL, href);
                }

            }
            return result;
        }

        /// <summary>
        /// Read all characters on the page. This will discard
        /// these characters, but allow the spider to examine the
        /// tags and find links.
        /// </summary>
        public void ReadAll()
        {
            while (Read() != -1)
            {
            }
        }

        /// <summary>
        /// Used internally, to add a URL to the spider's workload.
        /// </summary>
        /// <param name="u">The URL to add.</param>
        /// <param name="type">What type of link this is.</param>
        private void AddURL(String u, Spider.URLType type)
        {
            if (u == null)
            {
                return;
            }

            try
            {
                Uri url = URLUtility.ConstructURL(this.baseURL, u, true);
                url = this.spider.Workload.ConvertURL(url.ToString());

                if ((String.Compare(url.Scheme, "http", true) == 0)
                    || (String.Compare(url.Scheme, "https", true) == 0))
                {
                    if (this.spider.Report.SpiderFoundURL(url, this.baseURL, type))
                    {
                        try
                        {
                            this.spider.AddURL(url, this.baseURL, this.depth + 1);
                        }
                        catch (WorkloadException e)
                        {
                            throw new IOException(e.Message);
                        }
                    }
                }
            }

            catch (UriFormatException)
            {
                spider.Logging.Log(Logger.Level.INFO, "Malformed URL found:" + u);
            }
            catch (WorkloadException)
            {
                spider.Logging.Log(Logger.Level.INFO, "Invalid URL found:" + u);
            }
        }

        /// <summary>
        /// This method is called when an anchor(A) tag is found.
        /// </summary>
        /// <param name="href">The link found.</param>
        private void HandleA(String href)
        {

            String cmp = null;
            if (href != null)
            {
                href = href.Trim();
                cmp = href.ToLower();
            }


            if ((cmp != null) && !URLUtility.ContainsInvalidURLCharacters(href))
            {
                if (!cmp.StartsWith("javascript:")
                    && !cmp.StartsWith("rstp:")
                    && !cmp.StartsWith("rtsp:")
                    && !cmp.StartsWith("news:")
                    && !cmp.StartsWith("irc:")
                    && !cmp.StartsWith("mailto:"))
                {
                    AddURL(href, Spider.URLType.HYPERLINK);
                }
            }
        }
    }
}
