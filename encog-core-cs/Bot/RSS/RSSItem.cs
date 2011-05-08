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
        /// The title of this item.
        /// </summary>
        public String Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }

        /// <summary>
        /// The hyperlink to this item.
        /// </summary>
        public String Link
        {
            get
            {
                return link;
            }
            set
            {
                link = value;
            }
        }


        /// <summary>
        /// The description of this item.
        /// </summary>
        public String Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        /// <summary>
        /// The date this item was published.
        /// </summary>
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                date = value;
            }
        }

        /// <summary>
        /// The title of this item.
        /// </summary>
        private String title;

        /// <summary>
        /// The hyperlink to this item.
        /// </summary>
        private String link;

        /// <summary>
        /// The description of this item.
        /// </summary>
        private String description;

        /// <summary>
        /// The date this item was published.
        /// </summary>
        private DateTime date;


        /// <summary>
        /// Load an item from the specified node.
        /// </summary>
        /// <param name="node">The Node to load the item from.</param>
        public void Load(XmlNode node)
        {
            
            foreach(XmlNode n in node.ChildNodes )
            {
                String name = n.Name;

                if (String.Compare(name, "title", true) == 0)
                    title = n.InnerText;
                else if (String.Compare(name, "link", true) == 0)
                    link = n.InnerText;
                else if (String.Compare(name, "description", true) == 0)
                    description = n.InnerText;
                else if (String.Compare(name, "pubDate", true) == 0)
                {
                    String str = n.InnerText;
                    if (str != null)
                        date = RSS.ParseDate(str);
                }

            }
        }


        /// <summary>
        /// Convert the object to a String.
        /// </summary>
        /// <returns>The object as a String.</returns>
        public override String ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('[');
            builder.Append("title=\"");
            builder.Append(title);
            builder.Append("\",link=\"");
            builder.Append(link);
            builder.Append("\",date=\"");
            builder.Append(date);
            builder.Append("\"]");
            return builder.ToString();
        }
    }
}
#endif
