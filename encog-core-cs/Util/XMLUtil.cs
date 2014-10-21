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
using System.Linq;
using System.Text;
using System.Xml;

namespace Encog.Util
{
    class XMLUtil
    {
        /// <summary>
        /// Add the specified attribute.
        /// </summary>
        /// <param name="e">The node to add the attribute to.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value_ren">The value of the attribute.</param>
        public static void AddAttribute(XmlNode e, String name,
                 String value_ren)
        {
            XmlAttribute attr = e.OwnerDocument.CreateAttribute(name);
            attr.Value = value;
            e.Attributes.SetNamedItem(attr);
        }

        /// <summary>
        /// Create a property element.  Do not append it though!
        /// </summary>
        /// <param name="doc">The document to use.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value_ren">The value to add to the property.</param>
        /// <returns>The newly created property.</returns>
        public static XmlNode CreateProperty(XmlDocument doc, String name,
                 String value_ren)
        {
            XmlNode n = doc.CreateElement(name);
            n.AppendChild(doc.CreateTextNode(value));
            return n;
        }

        /// <summary>
        /// Find a child element.
        /// </summary>
        /// <param name="e">The element to search.</param>
        /// <param name="find">The name to search for.</param>
        /// <returns>The element found, or null if not found.</returns>
        public static XmlElement FindElement(XmlElement e, String find)
        {
            for (XmlNode child = e.FirstChild; child != null; child = child
                    .NextSibling)
            {
                if (!(child is XmlElement))
                {
                    continue;
                }
                XmlElement el = (XmlElement)child;
                if (el.Name.Equals(find))
                {
                    return el;
                }
            }
            return null;
        }

        /// <summary>
        /// Find an element, return as an int.
        /// </summary>
        /// <param name="e">The element that searches.</param>
        /// <param name="find">What we are searching for.</param>
        /// <param name="def">The default value, if we fail to find it.</param>
        /// <returns>The value found, default value otherwise.</returns>
        public static int FindElementAsInt(XmlElement e, String find,
                 int def)
        {
            String str = FindElementAsString(e, find);
            if (str == null)
            {
                return def;
            }

            return int.Parse(str);

        }

        /// <summary>
        /// Find an element, return as a long.
        /// </summary>
        /// <param name="e">The element that searches.</param>
        /// <param name="find">What we are searching for.</param>
        /// <param name="def">The default value, if we fail to find it.</param>
        /// <returns>The value found, default value otherwise.</returns>
        public static long FindElementAsLong(XmlElement e, String find,
                 long def)
        {
            String str = FindElementAsString(e, find);
            if (str == null)
            {
                return def;
            }

            return long.Parse(str);

        }

        /// <summary>
        /// Find an element, return as a string.
        /// </summary>
        /// <param name="e">The element that searches.</param>
        /// <param name="find">What we are searching for.</param>
        /// <returns>The value found, default value otherwise.</returns>
        public static String FindElementAsString(XmlElement e,
                 String find)
        {
            XmlElement el = FindElement(e, find);

            if (el == null)
            {
                return null;
            }

            for (XmlNode child = el.FirstChild; child != null; child = child.NextSibling)
            {
                if (!(child is XmlText))
                {
                    continue;
                }
                return child.Value;
            }
            return null;
        }

        /// <summary>
        /// Get the specified element's text value.
        /// </summary>
        /// <param name="e">The element.</param>
        /// <returns>The text value of the specified element.</returns>
        public static String GetElementValue(XmlElement e)
        {
            for (XmlNode child = e.FirstChild; child != null; child = child.NextSibling)
            {
                if (!(child is XmlText))
                {
                    continue;
                }
                return child.Value;
            }
            return null;
        }

        /// <summary>
        /// Private constructor.
        /// </summary>
        private XMLUtil()
        {
        }

    }
}
