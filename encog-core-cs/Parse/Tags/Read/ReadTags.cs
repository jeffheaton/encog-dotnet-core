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
using Encog.Util;
#if logging
using log4net;
#endif
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
        private static IDictionary<String, char> charMap;

        /// <summary>
        /// The stream that we are parsing from.
        /// </summary>
        private PeekableInputStream source;

        /// <summary>
        /// The current HTML tag. Access this property if the read function returns
        /// 0.
        /// </summary>
        private Tag tag = new Tag();

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(ReadTags));
#endif

        /// <summary>
        /// Are we locked, looking for an end tag?  Such as the end of a
        /// comment?
        /// </summary>
        private String lockedEndTag;

        /// <summary>
        /// Does a "fake" end-tag need to be added, because of a compound
        /// tag (i.e. <br/>)?  If so, this will hold a string for that tag.
        /// </summary>
        private String insertEndTag = null;

        /// <summary>
        /// The constructor should be passed an InputStream that we will parse from.
        /// </summary>
        /// <param name="istream">A stream to parse from.</param>
        public ReadTags(Stream istream)
        {
            this.source = new PeekableInputStream(istream);

            if (ReadTags.charMap == null)
            {
                ReadTags.charMap = new Dictionary<String, char>();
                ReadTags.charMap["nbsp"] = ' ';
                ReadTags.charMap["lt"] = '<';
                ReadTags.charMap["gt"] = '>';
                ReadTags.charMap["amp"] = '&';
                ReadTags.charMap["quot"] = '\"';
                ReadTags.charMap["bull"] = (char)CHAR_BULLET;
                ReadTags.charMap["trade"] = (char)CHAR_TRADEMARK;
            }
        }

        /// <summary>
        /// Remove any whitespace characters that are next in the InputStream.
        /// </summary>
        protected void EatWhitespace()
        {
            while (char.IsWhiteSpace((char)this.source.Peek()))
            {
                this.source.Read();
            }
        }

        /// <summary>
        /// Return the last tag found, this is normally called just after the read
        /// function returns a zero.
        /// </summary>
        public Tag LastTag
        {
            get
            {
                return this.tag;
            }
        }

        /// <summary>
        /// Checks to see if the next tag is the tag specified.
        /// </summary>
        /// <param name="name">The name of the tag desired.</param>
        /// <param name="start">True if a starting tag is desired.</param>
        /// <returns>True if the next tag matches these criteria.</returns>
        public bool IsIt(String name, bool start)
        {
            if (!this.LastTag.Name.Equals(name))
            {
                return false;
            }

            if (start)
            {
                return this.LastTag.TagType == Tag.Type.BEGIN;
            }
            else
            {
                return this.LastTag.TagType == Tag.Type.END;
            }
        }

        /// <summary>
        /// Parse an attribute name, if one is present.
        /// </summary>
        /// <returns>Return the attribute name, or null if none present.</returns>
        protected String ParseAttributeName()
        {
            EatWhitespace();

            if ("\"\'".IndexOf((char)this.source.Peek()) == -1)
            {
                StringBuilder buffer = new StringBuilder();
                while (!char.IsWhiteSpace((char)this.source.Peek())
                        && (this.source.Peek() != '=')
                        && (this.source.Peek() != '>')
                        && (this.source.Peek() != -1))
                {
                    int ch = ParseSpecialCharacter();
                    buffer.Append((char)ch);
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
            char result = (char)this.source.Read();
            int advanceBy = 0;

            // is there a special character?
            if (result == '&')
            {
                int ch = 0;
                StringBuilder buffer = new StringBuilder();

                // loop through and read special character
                do
                {
                    ch = this.source.Peek(advanceBy++);
                    if ((ch != '&') && (ch != ';') && !char.IsWhiteSpace((char)ch))
                    {
                        buffer.Append((char)ch);
                    }

                } while ((ch != ';') && (ch != -1) && !char.IsWhiteSpace((char)ch));

                String b = buffer.ToString().Trim().ToLower();

                // did we find a special character?
                if (b.Length > 0)
                {
                    if (b[0] == '#')
                    {
                        try
                        {
                            result = (char)int.Parse(b.Substring(1));
                        }
                        catch (Exception)
                        {
                            advanceBy = 0;
                        }
                    }
                    else
                    {
                        if (ReadTags.charMap.ContainsKey(b))
                        {
                            result = ReadTags.charMap[b];
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
        protected String ParseString()
        {
            StringBuilder result = new StringBuilder();
            EatWhitespace();
            if ("\"\'".IndexOf((char)this.source.Peek()) != -1)
            {
                int delim = this.source.Read();
                while ((this.source.Peek() != delim)
                        && (this.source.Peek() != -1))
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
                    result.Append((char)ch);
                }
                if ("\"\'".IndexOf((char)this.source.Peek()) != -1)
                {
                    this.source.Read();
                }
            }
            else
            {
                while (!char.IsWhiteSpace((char)this.source.Peek())
                        && (this.source.Peek() != -1)
                        && (this.source.Peek() != '>'))
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
            this.tag.Clear();
            this.insertEndTag = null;
            StringBuilder tagName = new StringBuilder();

            this.source.Read();

            // Is it a comment?
            if (this.source.Peek(TagConst.COMMENT_BEGIN))
            {
                this.source.Skip(TagConst.COMMENT_BEGIN.Length);
                while (!this.source.Peek(TagConst.COMMENT_END))
                {
                    int ch = this.source.Read();
                    if (ch != -1)
                    {
                        tagName.Append((char)ch);
                    }
                    else
                    {
                        break;
                    }
                }
                this.source.Skip(TagConst.COMMENT_END.Length);
                this.tag.TagType = Tag.Type.COMMENT;
                this.tag.Name = tagName.ToString();
                return;
            }

            // Is it CDATA?
            if (this.source.Peek(TagConst.CDATA_BEGIN))
            {
                this.source.Skip(TagConst.CDATA_BEGIN.Length);
                while (!this.source.Peek(TagConst.CDATA_END))
                {
                    int ch = this.source.Read();
                    if (ch != -1)
                    {
                        tagName.Append((char)ch);
                    }
                    else
                    {
                        break;
                    }

                }
                this.source.Skip(TagConst.CDATA_END.Length);
                this.tag.TagType = Tag.Type.CDATA;
                this.tag.Name = tagName.ToString();
                return;
            }

            // Find the tag name
            while (this.source.Peek() != -1)
            {
                // if this is the end of the tag, then stop
                if (char.IsWhiteSpace((char)this.source.Peek())
                        || (this.source.Peek() == '>'))
                {
                    break;
                }

                // if this is both a begin and end tag then stop
                if ((tagName.Length > 0) && (this.source.Peek() == '/'))
                {
                    break;
                }

                tagName.Append((char)this.source.Read());
            }

            EatWhitespace();

            if (tagName[0] == '/')
            {
                this.tag.Name = tagName.ToString().Substring(1);
                this.tag.TagType = Tag.Type.END;
            }
            else
            {
                this.tag.Name = tagName.ToString();
                this.tag.TagType = Tag.Type.BEGIN;
            }
            // get the attributes

            while ((this.source.Peek() != '>') && (this.source.Peek() != -1))
            {
                String attributeName = ParseAttributeName();
                String attributeValue = null;

                if (attributeName.Equals("/"))
                {
                    EatWhitespace();
                    if (this.source.Peek() == '>')
                    {
                        this.insertEndTag = this.tag.Name;
                        break;
                    }
                }

                // is there a value?
                EatWhitespace();
                if (this.source.Peek() == '=')
                {
                    this.source.Read();
                    attributeValue = ParseString();
                }

                this.tag.SetAttribute(attributeName, attributeValue);
            }
            this.source.Read();
        }

        /// <summary>
        /// Check to see if the ending tag is present.
        /// </summary>
        /// <param name="name">The type of end tag being sought.</param>
        /// <returns>True if the ending tag was found.</returns>
        private bool PeekEndTag(String name)
        {
            int i = 0;

            // pass any whitespace
            while ((this.source.Peek(i) != -1)
                    && char.IsWhiteSpace((char)this.source.Peek(i)))
            {
                i++;
            }

            // is a tag beginning
            if (this.source.Peek(i) != '<')
            {
                return false;
            }
            else
            {
                i++;
            }

            // pass any whitespace
            while ((this.source.Peek(i) != -1)
                    && char.IsWhiteSpace((char)this.source.Peek(i)))
            {
                i++;
            }

            // is it an end tag
            if (this.source.Peek(i) != '/')
            {
                return false;
            }
            else
            {
                i++;
            }

            // pass any whitespace
            while ((this.source.Peek(i) != -1)
                    && char.IsWhiteSpace((char)this.source.Peek(i)))
            {
                i++;
            }

            // does the name match
            for (int j = 0; j < name.Length; j++)
            {
                if (char.ToLower((char)this.source.Peek(i)) != char
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
            if (this.insertEndTag != null)
            {
                this.tag.Clear();
                this.tag.Name = this.insertEndTag;
                this.tag.TagType = Tag.Type.END;
                this.insertEndTag = null;
                return 0;
            }

            // handle locked end tag
            if (this.lockedEndTag != null)
            {
                if (PeekEndTag(this.lockedEndTag))
                {
                    this.lockedEndTag = null;
                }
                else
                {
                    return this.source.Read();
                }
            }

            // look for next tag
            if (this.source.Peek() == '<')
            {
                ParseTag();

                if ((this.tag.TagType == Tag.Type.BEGIN)
                    && ((StringUtil.EqualsIgnoreCase(this.tag.Name, "script") )
                    || (StringUtil.EqualsIgnoreCase(this.tag.Name, "style") )))
                {
                    this.lockedEndTag = this.tag.Name.ToLower();
                }
                return 0;
            }
            else if (this.source.Peek() == '&')
            {
                return ParseSpecialCharacter();
            }
            else
            {
                return (this.source.Read());
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
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[ReadTags: currentTag=");
            if (this.tag != null)
            {
                result.Append(this.tag.ToString());
            }
            result.Append("]");
            return result.ToString();

        }
    }
}
