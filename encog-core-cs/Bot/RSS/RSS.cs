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
using System.Xml;

namespace Encog.Bot.RSS
{
    /// <summary>
    /// RSS: This is the class that actually parses the 
    /// RSS and builds a collection of RSSItems.  To make use
    /// of this class call the load method with a URL that
    /// points to RSS.
    /// </summary>
    public class RSS
    {
        /// <summary>
        /// All of the attributes for this RSS document.
        /// </summary>
        private readonly Dictionary<String, String> _attributes = new Dictionary<String, String>();

        /// <summary>
        /// All RSS items, or stories, found.
        /// </summary>
        private readonly List<RSSItem> _items = new List<RSSItem>();

        /// <summary>
        /// All of the attributes for this RSS document.
        /// </summary>
        public Dictionary<String, String> Attributes
        {
            get { return _attributes; }
        }

        /// <summary>
        /// All RSS items, or stories, found.
        /// </summary>
        public List<RSSItem> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Simple utility function that converts a RSS formatted date
        /// into a C# date.
        /// </summary>
        /// <param name="datestr">A date</param>
        /// <returns>A C# DateTime object.</returns>
        public static DateTime ParseDate(String datestr)
        {
            DateTime date = DateTime.Parse(datestr);
            return date;
        }

        /// <summary>
        /// Load the specified RSS item, or story.
        /// </summary>
        /// <param name="item">A XML node that contains a RSS item.</param>
        private void LoadItem(XmlNode item)
        {
            var rssItem = new RSSItem();
            rssItem.Load(item);
            _items.Add(rssItem);
        }

        /// <summary>
        /// Load the channle node.
        /// </summary>
        /// <param name="channel">A node that contains a channel.</param>
        private void LoadChannel(XmlNode channel)
        {
            foreach (XmlNode node in channel.ChildNodes)
            {
                String nodename = node.Name;
                if (String.Compare(nodename, "item", true) == 0)
                {
                    LoadItem(node);
                }
                else
                {
                    _attributes.Remove(nodename);
                    _attributes.Add(nodename, channel.InnerText);
                }
            }
        }

        /// <summary>
        /// Load all RSS data from the specified URL.
        /// </summary>
        /// <param name="url">URL that contains XML data.</param>
        public void Load(Uri url)
        {
            WebRequest http = WebRequest.Create(url);
            var response = (HttpWebResponse) http.GetResponse();
            Stream istream = response.GetResponseStream();

            var d = new XmlDocument();
            d.Load(istream);

            foreach (XmlNode node in d.DocumentElement.ChildNodes)
            {
                String nodename = node.Name;

                // RSS 2.0
                if (String.Compare(nodename, "channel", true) == 0)
                {
                    LoadChannel(node);
                }
                    // RSS 1.0
                else if (String.Compare(nodename, "item", true) == 0)
                {
                    LoadItem(node);
                }
            }
        }

        /// <summary>
        /// Convert the object to a String.
        /// </summary>
        /// <returns>The object as a String.</returns>
        public override String ToString()
        {
            var str = new StringBuilder();

            foreach (String item in _attributes.Keys)
            {
                str.Append(item);
                str.Append('=');
                str.Append(_attributes[item]);
                str.Append('\n');
            }
            str.Append("Items:\n");
            foreach (RSSItem item in _items)
            {
                str.Append(item.ToString());
                str.Append('\n');
            }
            return str.ToString();
        }
    }
}

#endif