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
                        && (LastTag.TagType == Tag.Type.Begin))
                    {
                        return true;
                    }
                }
                else
                {
                    if (LastTag.Name.Equals(name)
                        && (LastTag.TagType == Tag.Type.End))
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
                    && (LastTag.TagType == Tag.Type.End))
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
