//
// Encog(tm) Core v3.0 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2011 Heaton Research, Inc.
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
#if logging
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Encog.Util;

#endif

using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using Encog.Util;
namespace Encog.Parse.Tags.Read
{
    /// <summary>
    /// Base class used to read tags.  This base class is used by both the
    /// XML and HTML parsing.
    /// </summary>
    public class ReadTags
    {
        /// <summary>
        /// The bullet character.
        /// </summary>
        public int CHAR_BULLET = 149;

        /// <summary>
        /// The bullet character.
        /// </summary>
        public int CHAR_TRADEMARK = 129;

        /// <summary>
        /// Maximum length string to read.
        /// </summary>
        public int MAX_LENGTH = 10000;

        /// <summary>
        /// A mapping of certain HTML encoded values to their actual
        /// character values.
        /// </summary>
        private static IDictionary<string, char> charMap;

        /// <summary>
        /// The stream that we are parsing from.
        /// </summary>
        private readonly PeekableInputStream source;

        /// <summary>
        /// The current HTML tag. Access this property if the read function returns
        /// 0.
        /// </summary>
        private readonly Tag tag = new Tag();


        /// <summary>
        /// Are we locked, looking for an end tag?  Such as the end of a
        /// comment?
        /// </summary>
        private string lockedEndTag;

        /// <summary>
        /// Does a "fake" end-tag need to be added, because of a compound
        /// tag (i.e. <br/>)?  If so, this will hold a string for that tag.
        /// </summary>
        private string insertEndTag;

        /// <summary>
        /// The constructor should be passed an InputStream that we will parse from.
        /// </summary>
        /// <param name="istream">A stream to parse from.</param>
        public ReadTags(Stream istream)
        {
            source = new PeekableInputStream(istream);

            if (charMap == null)
            {
                charMap = new Dictionary<string, char>();
                charMap["nbsp"] = ' ';
                charMap["lt"] = '<';
                charMap["gt"] = '>';
                charMap["amp"] = '&';
                charMap["quot"] = '\"';
                charMap["bull"] = (char) CHAR_BULLET;
                charMap["trade"] = (char) CHAR_TRADEMARK;
            }
        }

        /// <summary>
        /// Remove any whitespace characters that are next in the InputStream.
        /// </summary>
        protected void EatWhitespace()
        {
            while (char.IsWhiteSpace((char) source.Peek()))
            {
                source.Read();
            }
        }

        /// <summary>
        /// Return the last tag found, this is normally called just after the read
        /// function returns a zero.
        /// </summary>
        public Tag LastTag
        {
            get { return tag; }
        }

        /// <summary>
        /// Checks to see if the next tag is the tag specified.
        /// </summary>
        /// <param name="name">The name of the tag desired.</param>
        /// <param name="start">True if a starting tag is desired.</param>
        /// <returns>True if the next tag matches these criteria.</returns>
        public bool IsIt(string name, bool start)
        {
            if (!LastTag.Name.Equals(name))
            {
                return false;
            }

            if (start)
            {
                return LastTag.TagType == Tag.Type.BEGIN;
            }
            else
            {
                return LastTag.TagType == Tag.Type.END;
            }
        }

        /// <summary>
        /// Parse an attribute name, if one is present.
        /// </summary>
        /// <returns>Return the attribute name, or null if none present.</returns>
        protected string ParseAttributeName()
        {
            EatWhitespace();

            if ("\"\'".IndexOf((char) source.Peek()) == -1)
            {
                var buffer = new StringBuilder();
                while (!char.IsWhiteSpace((char) source.Peek())
                       && (source.Peek() != '=')
                       && (source.Peek() != '>')
                       && (source.Peek() != -1))
                {
                    int ch = ParseSpecialCharacter();
                    buffer.Append((char) ch);
                }
                return buffer.ToString();
            }
            else
            {
                return (ParseString());
            }
        }

        /// <summary>
        /// Parse any special characters
        /// </summary>
        /// <returns>The character that was parsed.</returns>
        private char ParseSpecialCharacter()
        {
            var result = (char) source.Read();
            int advanceBy = 0;

            // is there a special character?
            if (result == '&')
            {
                int ch = 0;
                var buffer = new StringBuilder();

                // loop through and read special character
                do
                {
                    ch = source.Peek(advanceBy++);
                    if ((ch != '&') && (ch != ';') && !char.IsWhiteSpace((char) ch))
                    {
                        buffer.Append((char) ch);
                    }
                } while ((ch != ';') && (ch != -1) && !char.IsWhiteSpace((char) ch));

                string b = buffer.ToString().Trim().ToLower();

                // did we find a special character?
                if (b.Length > 0)
                {
                    if (b[0] == '#')
                    {
                        try
                        {
                            result = (char) int.Parse(b.Substring(1));
                        }
                        catch (Exception)
                        {
                            advanceBy = 0;
                        }
                    }
                    else
                    {
                        if (charMap.ContainsKey(b))
                        {
                            result = charMap[b];
                        }
                        else
                        {
                            advanceBy = 0;
                        }
                    }
                }
                else
                {
                    advanceBy = 0;
                }
            }

            while (advanceBy > 0)
            {
                Read();
                advanceBy--;
            }

            return result;
        }

        /// <summary>
        /// Called to parse a double or single quote string.
        /// </summary>
        /// <returns>The string parsed.</returns>
        protected string ParseString()
        {
            var result = new StringBuilder();
            EatWhitespace();
            if ("\"\'".IndexOf((char) source.Peek()) != -1)
            {
                int delim = source.Read();
                while ((source.Peek() != delim)
                       && (source.Peek() != -1))
                {
                    if (result.Length > MAX_LENGTH)
                    {
                        break;
                    }
                    int ch = ParseSpecialCharacter();
                    if ((ch == '\r') || (ch == '\n'))
                    {
                        continue;
                    }
                    result.Append((char) ch);
                }
                if ("\"\'".IndexOf((char) source.Peek()) != -1)
                {
                    source.Read();
                }
            }
            else
            {
                while (!char.IsWhiteSpace((char) source.Peek())
                       && (source.Peek() != -1)
                       && (source.Peek() != '>'))
                {
                    result.Append(ParseSpecialCharacter());
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Called when a tag is detected. This method will parse the tag.
        /// </summary>
        protected void ParseTag()
        {
            tag.Clear();
            insertEndTag = null;
            var tagName = new StringBuilder();

            source.Read();

            // Is it a comment?
            if (source.Peek(TagConst.COMMENT_BEGIN))
            {
                source.Skip(TagConst.COMMENT_BEGIN.Length);
                while (!source.Peek(TagConst.COMMENT_END))
                {
                    int ch = source.Read();
                    if (ch != -1)
                    {
                        tagName.Append((char) ch);
                    }
                    else
                    {
                        break;
                    }
                }
                source.Skip(TagConst.COMMENT_END.Length);
                tag.TagType = Tag.Type.COMMENT;
                tag.Name = tagName.ToString();
                return;
            }

            // Is it CDATA?
            if (source.Peek(TagConst.CDATA_BEGIN))
            {
                source.Skip(TagConst.CDATA_BEGIN.Length);
                while (!source.Peek(TagConst.CDATA_END))
                {
                    int ch = source.Read();
                    if (ch != -1)
                    {
                        tagName.Append((char) ch);
                    }
                    else
                    {
                        break;
                    }
                }
                source.Skip(TagConst.CDATA_END.Length);
                tag.TagType = Tag.Type.CDATA;
                tag.Name = tagName.ToString();
                return;
            }

            // Find the tag name
            while (source.Peek() != -1)
            {
                // if this is the end of the tag, then stop
                if (char.IsWhiteSpace((char) source.Peek())
                    || (source.Peek() == '>'))
                {
                    break;
                }

                // if this is both a begin and end tag then stop
                if ((tagName.Length > 0) && (source.Peek() == '/'))
                {
                    break;
                }

                tagName.Append((char) source.Read());
            }

            EatWhitespace();

            if (tagName[0] == '/')
            {
                tag.Name = tagName.ToString().Substring(1);
                tag.TagType = Tag.Type.END;
            }
            else
            {
                tag.Name = tagName.ToString();
                tag.TagType = Tag.Type.BEGIN;
            }
            // get the attributes

            while ((source.Peek() != '>') && (source.Peek() != -1))
            {
                string attributeName = ParseAttributeName();
                string attributeValue = null;

                if (attributeName.Equals("/"))
                {
                    EatWhitespace();
                    if (source.Peek() == '>')
                    {
                        insertEndTag = tag.Name;
                        break;
                    }
                }

                // is there a value?
                EatWhitespace();
                if (source.Peek() == '=')
                {
                    source.Read();
                    attributeValue = ParseString();
                }

                tag.SetAttribute(attributeName, attributeValue);
            }
            source.Read();
        }

        /// <summary>
        /// Check to see if the ending tag is present.
        /// </summary>
        /// <param name="name">The type of end tag being sought.</param>
        /// <returns>True if the ending tag was found.</returns>
        private bool PeekEndTag(string name)
        {
            int i = 0;

            // pass any whitespace
            while ((source.Peek(i) != -1)
                   && char.IsWhiteSpace((char) source.Peek(i)))
            {
                i++;
            }

            // is a tag beginning
            if (source.Peek(i) != '<')
            {
                return false;
            }
            else
            {
                i++;
            }

            // pass any whitespace
            while ((source.Peek(i) != -1)
                   && char.IsWhiteSpace((char) source.Peek(i)))
            {
                i++;
            }

            // is it an end tag
            if (source.Peek(i) != '/')
            {
                return false;
            }
            else
            {
                i++;
            }

            // pass any whitespace
            while ((source.Peek(i) != -1)
                   && char.IsWhiteSpace((char) source.Peek(i)))
            {
                i++;
            }

            // does the name match
            for (int j = 0; j < name.Length; j++)
            {
                if (char.ToLower((char) source.Peek(i)) != char
                                                               .ToLower(name[j]))
                {
                    return false;
                }
                i++;
            }

            return true;
        }

        /// <summary>
        /// Read a single character from the HTML source, if this function returns
        /// zero(0) then you should call getTag to see what tag was found. Otherwise
        /// the value returned is simply the next character found.
        /// </summary>
        /// <returns>The character read, or zero if there is an HTML tag. If zero is
        /// returned, then call getTag to get the next tag.</returns>
        public int Read()
        {
            // handle inserting a "virtual" end tag
            if (insertEndTag != null)
            {
                tag.Clear();
                tag.Name = insertEndTag;
                tag.TagType = Tag.Type.END;
                insertEndTag = null;
                return 0;
            }

            // handle locked end tag
            if (lockedEndTag != null)
            {
                if (PeekEndTag(lockedEndTag))
                {
                    lockedEndTag = null;
                }
                else
                {
                    return source.Read();
                }
            }

            // look for next tag
            if (source.Peek() == '<')
            {
                ParseTag();

                if ((tag.TagType == Tag.Type.BEGIN)
                    && ((StringUtil.EqualsIgnoreCase(tag.Name, "script"))
                        || (StringUtil.EqualsIgnoreCase(tag.Name, "style"))))
                {
                    lockedEndTag = tag.Name.ToLower();
                }
                return 0;
            }
            else if (source.Peek() == '&')
            {
                return ParseSpecialCharacter();
            }
            else
            {
                return (source.Read());
            }
        }

        /// <summary>
        /// Read until we reach the next tag.
        /// </summary>
        /// <returns>True if a tag was found, false on EOF.</returns>
        public bool ReadToTag()
        {
            int ch;
            while ((ch = Read()) != -1)
            {
                if (ch == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns this object as a string.
        /// </summary>
        /// <returns>This object as a string.</returns>
        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append("[ReadTags: currentTag=");
            if (tag != null)
            {
                result.Append(tag.ToString());
            }
            result.Append("]");
            return result.ToString();
        }
    }
}
