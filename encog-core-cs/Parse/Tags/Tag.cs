// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
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
#if logging
using log4net;
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
        };

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(Tag));
#endif

        /// <summary>
        /// The tag's attributes.
        /// </summary>
        private IDictionary<String, String> attributes =
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
            this.attributes.Clear();
            this.name = "";
            this.type = Type.BEGIN;
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A cloned copy of the object.</returns>
        public Tag Clone()
        {
            Tag result = new Tag();
            result.Name = this.Name;
            result.TagType = this.TagType;
            foreach (String key in this.attributes.Keys)
            {
                String value = this.attributes[key];
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
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error("Exception", e);
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
            get
            {
                return this.attributes;
            }
        }

        /// <summary>
        /// Get the value of the specified attribute.
        /// </summary>
        /// <param name="name">The name of an attribute.</param>
        /// <returns>The value of the specified attribute.</returns>
        public String GetAttributeValue(String name)
        {
            if (!this.attributes.ContainsKey(name.ToLower()))
                return null;

            return this.attributes[name.ToLower()];
        }


        /// <summary>
        /// The tag name.
        /// </summary>
        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// The tag type.
        /// </summary>
        public Type TagType
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }

        /// <summary>
        /// Set a HTML attribute.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public void SetAttribute(String name, String value)
        {
            this.attributes[name.ToLower()] = value;
        }

        /// <summary>
        /// Convert this tag back into string form, with the 
        /// beginning &lt; and ending &gt;.
        /// </summary>
        /// <returns>The Attribute object that was found.</returns>
        public override String ToString()
        {
            StringBuilder buffer = new StringBuilder("<");

            if (this.type == Type.END)
            {
                buffer.Append("/");
            }

            buffer.Append(this.name);

            ICollection<String> set = this.attributes.Keys;
            foreach (String key in set)
            {
                String value = this.attributes[key];
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
