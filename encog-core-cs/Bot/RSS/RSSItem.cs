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
using System.Text;
using System.Xml;

namespace Encog.Bot.RSS
{
    /// <summary>
    /// RSSItem: This is the class that holds individual RSS items,
    /// or stories, for the RSS class.
    /// </summary>
    public class RSSItem
    {
        /// <summary>
        /// The date this item was published.
        /// </summary>
        private DateTime _date;

        /// <summary>
        /// The description of this item.
        /// </summary>
        private String _description;

        /// <summary>
        /// The hyperlink to this item.
        /// </summary>
        private String _link;

        /// <summary>
        /// The title of this item.
        /// </summary>
        private String _title;

        /// <summary>
        /// The title of this item.
        /// </summary>
        public String Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// The hyperlink to this item.
        /// </summary>
        public String Link
        {
            get { return _link; }
            set { _link = value; }
        }


        /// <summary>
        /// The description of this item.
        /// </summary>
        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// The date this item was published.
        /// </summary>
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }


        /// <summary>
        /// Load an item from the specified node.
        /// </summary>
        /// <param name="node">The Node to load the item from.</param>
        public void Load(XmlNode node)
        {
            foreach (XmlNode n in node.ChildNodes)
            {
                String name = n.Name;

                if (String.Compare(name, "title", true) == 0)
                    _title = n.InnerText;
                else if (String.Compare(name, "link", true) == 0)
                    _link = n.InnerText;
                else if (String.Compare(name, "description", true) == 0)
                    _description = n.InnerText;
                else if (String.Compare(name, "pubDate", true) == 0)
                {
                    String str = n.InnerText;
                    _date = RSS.ParseDate(str);
                }
            }
        }


        /// <summary>
        /// Convert the object to a String.
        /// </summary>
        /// <returns>The object as a String.</returns>
        public override String ToString()
        {
            var builder = new StringBuilder();
            builder.Append('[');
            builder.Append("title=\"");
            builder.Append(_title);
            builder.Append("\",link=\"");
            builder.Append(_link);
            builder.Append("\",date=\"");
            builder.Append(_date);
            builder.Append("\"]");
            return builder.ToString();
        }
    }
}

#endif