// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
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
using System.IO;
#if logging
using log4net;
#endif

namespace Encog.Parse.Tags.Read
{
    /// <summary>
    /// Parse XML data.
    /// </summary>
    public class ReadXML : ReadTags
    {
#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(ReadXML));
#endif
        /// <summary>
        /// Construct an XML reader.
        /// </summary>
        /// <param name="istream">The input stream to read from.</param>
        public ReadXML(Stream istream)
            : base(istream)
        {
        }

        /// <summary>
        /// Advance until the specified tag is found.
        /// </summary>
        /// <param name="name">The name of the tag we are looking for.</param>
        /// <param name="beginTag">True if this is a begin tage, false otherwise.</param>
        /// <returns>True if the tag was found.</returns>
        public bool FindTag(String name, bool beginTag)
        {
            while (ReadToTag())
            {
                if (beginTag)
                {
                    if (this.LastTag.Name.Equals(name)
                            && (this.LastTag.TagType == Tag.Type.BEGIN))
                    {
                        return true;
                    }
                }
                else
                {
                    if (this.LastTag.Name.Equals(name)
                            && (this.LastTag.TagType == Tag.Type.END))
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        /// <summary>
        /// Read an integer that is contained between the current position, and the
        /// next tag.
        /// </summary>
        /// <returns>The integer that was found.</returns>
        public int ReadIntToTag()
        {
            try
            {
                String str = ReadTextToTag();
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
        /// Read all property data until an end tag, which corrisponds to the current
        /// tag, is found. The properties found will be returned in a map.
        /// </summary>
        /// <returns>The properties found.</returns>
        public IDictionary<String, String> ReadPropertyBlock()
        {
            IDictionary<String, String> result = new Dictionary<String, String>();

            String endingBlock = this.LastTag.Name;

            while (ReadToTag())
            {
                if (this.LastTag.Name.Equals(endingBlock)
                        && (this.LastTag.TagType == Tag.Type.END))
                {
                    break;
                }
                String name = this.LastTag.Name;
                String value = ReadTextToTag().Trim();
                result[name] = value;
            }

            return result;
        }

        /// <summary>
        /// Read all text between the current position and the next tag.
        /// </summary>
        /// <returns>The string that was read.</returns>
        public String ReadTextToTag()
        {
            StringBuilder result = new StringBuilder();
            bool done = false;

            while (!done)
            {
                int ch = Read();
                if ((ch == -1) || (ch == 0))
                {
                    done = true;
                }
                else
                {
                    result.Append((char)ch);
                }

            }
            return result.ToString();
        }
    }
}
