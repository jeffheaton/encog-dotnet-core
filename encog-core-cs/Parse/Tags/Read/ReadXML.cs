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
using System.IO;
using System.Text;

#if logging

#endif

namespace Encog.Parse.Tags.Read
{
    /// <summary>
    /// Parse XML data.
    /// </summary>
    public class ReadXML : ReadTags
    {

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
                    if (LastTag.Name.Equals(name)
                        && (LastTag.TagType == Tag.Type.BEGIN))
                    {
                        return true;
                    }
                }
                else
                {
                    if (LastTag.Name.Equals(name)
                        && (LastTag.TagType == Tag.Type.END))
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
                if (logger.IsErrorEnabled)
                {
                    logger.Error("Exception", e);
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

            String endingBlock = LastTag.Name;

            while (ReadToTag())
            {
                if (LastTag.Name.Equals(endingBlock)
                    && (LastTag.TagType == Tag.Type.END))
                {
                    break;
                }
                String name = LastTag.Name;
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
            var result = new StringBuilder();
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
                    result.Append((char) ch);
                }
            }
            return result.ToString();
        }
    }
}