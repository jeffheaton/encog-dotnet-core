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
