// Encog Neural Network and Bot Library for DotNet v0.5
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
//
using System;
using System.Collections.Generic;
using System.Text;

namespace Encog.Bot.HTML
{
    
    /// <summary>
    /// HTMLTag: This class holds a single HTML tag. This class
    /// subclasses the AttributeList class. This allows the
    /// HTMLTag class to hold a collection of attributes, just as
    /// an actual HTML tag does.
    /// </summary>
    public class HTMLTag
    {
        private String name;
        private bool ending;

        /// <summary>
        /// The name of the tag.
        /// </summary>
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        /// <summary>
        /// Is this tag both a beginning and an
        /// ending tag.
        /// </summary>
        public Boolean Ending
        {
            get
            {
                return ending;
            }
            set
            {
                ending = value;
            }
        }

        /// <summary>
        /// The attributes of this tag.
        /// </summary>
        private Dictionary<String, String> attributes = new Dictionary<String, String>();

        /// <summary>
        /// Clear out this tag.
        /// </summary>
        public void Clear()
        {
            this.attributes.Clear();
            this.Name = "";
            this.Ending = false;
        }

        /// <summary>
        /// Access the individual attributes by name.
        /// </summary>
        public String this[string key]
        {
            get
            {
                if( attributes.ContainsKey(key.ToLower()) )
                    return this.attributes[key.ToLower()];
                else
                    return null;
            }
            set
            {
                this.attributes.Add(key.ToLower(), value);
            }
        }

        /// <summary>
        /// Convert this tag back into string form, with the
        /// beginning &lt; and ending &gt;.
        /// </summary>
        /// <returns>The attribute value that was found.</returns>
        public override String ToString()
        {
            StringBuilder buffer = new StringBuilder("<");
            buffer.Append(this.Name);

            foreach (String key in attributes.Keys)
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

            if (this.Ending)
            {
                buffer.Append('/');
            }
            buffer.Append(">");
            return buffer.ToString();
        }

        /// <summary>
        /// Set the specified attribute.
        /// </summary>
        /// <param name="key">The attribute name.</param>
        /// <param name="value">The attribute value.</param>
        public void SetAttribute(String key, String value)
        {
            attributes.Remove(key.ToLower());
            attributes.Add(key.ToLower(), value);
        }
    }
}

