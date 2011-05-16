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

using System;
using System.Collections.Generic;
using System.Text;

#if logging

#endif

namespace Encog.Parse.Tags
{
    /// <summary>
    /// HTMLTag: This class holds a single HTML tag. This class subclasses the
    /// AttributeList class. This allows the HTMLTag class to hold a collection of
    /// attributes, just as an actual HTML tag does.
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Tag types.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// A beginning tag.
            /// </summary>
            BEGIN,
            /// <summary>
            /// An ending tag.
            /// </summary>
            END,
            /// <summary>
            /// A comment.
            /// </summary>
            COMMENT,
            /// <summary>
            /// A CDATA section.
            /// </summary>
            CDATA
        } ;


        /// <summary>
        /// The tag's attributes.
        /// </summary>
        private readonly IDictionary<String, String> attributes =
            new Dictionary<String, String>();

        /// <summary>
        /// The tag name.
        /// </summary>
        private String name = "";

        /// <summary>
        /// The tag type.
        /// </summary>
        private Type type;

        /// <summary>
        /// Clear the name, type and attributes.
        /// </summary>
        public void Clear()
        {
            attributes.Clear();
            name = "";
            type = Type.BEGIN;
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A cloned copy of the object.</returns>
        public virtual object Clone()
        {
            var result = new Tag();
            result.Name = Name;
            result.TagType = TagType;
            foreach (String key in attributes.Keys)
            {
                String value = attributes[key];
                result.Attributes[key] = value;
            }
            return result;
        }

        /// <summary>
        /// Get the specified attribute as an integer.
        /// </summary>
        /// <param name="attributeId">The attribute name.</param>
        /// <returns>The attribute value.</returns>
        public int GetAttributeInt(String attributeId)
        {
            try
            {
                String str = GetAttributeValue(attributeId);
                return int.Parse(str);
            }
            catch (Exception e)
            {
#if logging
                if (logger.IsErrorEnabled)
                {
                    logger.Error("Exception", e);
                }
#endif
                throw new ParseError(e);
            }
        }


        /// <summary>
        /// The attributes for this tag as a dictionary.
        /// </summary>
        public IDictionary<String, String> Attributes
        {
            get { return attributes; }
        }

        /// <summary>
        /// Get the value of the specified attribute.
        /// </summary>
        /// <param name="name">The name of an attribute.</param>
        /// <returns>The value of the specified attribute.</returns>
        public String GetAttributeValue(String name)
        {
            if (!attributes.ContainsKey(name))
                return null;

            return attributes[name];
        }


        /// <summary>
        /// The tag name.
        /// </summary>
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// The tag type.
        /// </summary>
        public Type TagType
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Set a HTML attribute.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public void SetAttribute(String name, String value)
        {
            attributes[name] = value;
        }

        /// <summary>
        /// Convert this tag back into string form, with the 
        /// beginning &lt; and ending &gt;.
        /// </summary>
        /// <returns>The Attribute object that was found.</returns>
        public override String ToString()
        {
            var buffer = new StringBuilder("<");

            if (type == Type.END)
            {
                buffer.Append("/");
            }

            buffer.Append(name);

            ICollection<String> set = attributes.Keys;
            foreach (String key in set)
            {
                String value = attributes[key];
                buffer.Append(' ');

                if (value == null)
                {
                    buffer.Append("\"");
                    buffer.Append(key);
                    buffer.Append("\"");
                }
                else
                {
                    buffer.Append(key);
                    buffer.Append("=\"");
                    buffer.Append(value);
                    buffer.Append("\"");
                }
            }

            buffer.Append(">");
            return buffer.ToString();
        }
    }
}