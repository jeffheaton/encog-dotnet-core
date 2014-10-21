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
            Begin,
            /// <summary>
            /// An ending tag.
            /// </summary>
            End,
            /// <summary>
            /// A comment.
            /// </summary>
            Comment,
            /// <summary>
            /// A CDATA section.
            /// </summary>
            CDATA
        } ;


        /// <summary>
        /// The tag's attributes.
        /// </summary>
        private readonly IDictionary<String, String> _attributes =
            new Dictionary<String, String>();

        /// <summary>
        /// The tag name.
        /// </summary>
        private String _name = "";

        /// <summary>
        /// The tag type.
        /// </summary>
        private Type _type;

        /// <summary>
        /// Clear the name, type and attributes.
        /// </summary>
        public void Clear()
        {
            _attributes.Clear();
            _name = "";
            _type = Type.Begin;
        }

        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A cloned copy of the object.</returns>
        public virtual object Clone()
        {
            var result = new Tag {Name = Name, TagType = TagType};
            foreach (String key in _attributes.Keys)
            {
                String value = _attributes[key];
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
            get { return _attributes; }
        }

        /// <summary>
        /// Get the value of the specified attribute.
        /// </summary>
        /// <param name="name">The name of an attribute.</param>
        /// <returns>The value of the specified attribute.</returns>
        public String GetAttributeValue(String name)
        {
            if (!_attributes.ContainsKey(name))
                return null;

            return _attributes[name];
        }


        /// <summary>
        /// The tag name.
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The tag type.
        /// </summary>
        public Type TagType
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Set a HTML attribute.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="valueRen">The value of the attribute.</param>
        public void SetAttribute(String name, String valueRen)
        {
            _attributes[name] = valueRen;
        }

        /// <summary>
        /// Convert this tag back into string form, with the 
        /// beginning &lt; and ending &gt;.
        /// </summary>
        /// <returns>The Attribute object that was found.</returns>
        public override String ToString()
        {
            var buffer = new StringBuilder("<");

            if (_type == Type.End)
            {
                buffer.Append("/");
            }

            buffer.Append(_name);

            ICollection<String> set = _attributes.Keys;
            foreach (String key in set)
            {
                String value = _attributes[key];
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
