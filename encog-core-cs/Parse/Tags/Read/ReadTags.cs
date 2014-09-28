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
        public int CharBullet = 149;

        /// <summary>
        /// The bullet character.
        /// </summary>
        public int CharTrademark = 129;

        /// <summary>
        /// Maximum length string to read.
        /// </summary>
        public int MaxLength = 10000;

        /// <summary>
        /// A mapping of certain HTML encoded values to their actual
        /// character values.
        /// </summary>
        private static IDictionary<string, char> _charMap;

        /// <summary>
        /// The stream that we are parsing from.
        /// </summary>
        private readonly PeekableInputStream _source;

        /// <summary>
        /// The current HTML tag. Access this property if the read function returns
        /// 0.
        /// </summary>
        private readonly Tag _tag = new Tag();


        /// <summary>
        /// Are we locked, looking for an end tag?  Such as the end of a
        /// comment?
        /// </summary>
        private string _lockedEndTag;

        /// <summary>
        /// Does a "fake" end-tag need to be added, because of a compound
        /// tag (i.e. <br/>)?  If so, this will hold a string for that tag.
        /// </summary>
        private string _insertEndTag;

        /// <summary>
        /// The constructor should be passed an InputStream that we will parse from.
        /// </summary>
        /// <param name="istream">A stream to parse from.</param>
        public ReadTags(Stream istream)
        {
            _source = new PeekableInputStream(istream);

            if (_charMap == null)
            {
                _charMap = new Dictionary<string, char>();
                _charMap["nbsp"] = ' ';
                _charMap["lt"] = '<';
                _charMap["gt"] = '>';
                _charMap["amp"] = '&';
                _charMap["quot"] = '\"';
                _charMap["bull"] = (char) CharBullet;
                _charMap["trade"] = (char) CharTrademark;
            }
        }

        /// <summary>
        /// Remove any whitespace characters that are next in the InputStream.
        /// </summary>
        protected void EatWhitespace()
        {
            while (char.IsWhiteSpace((char) _source.Peek()))
            {
                _source.Read();
            }
        }

        /// <summary>
        /// Return the last tag found, this is normally called just after the read
        /// function returns a zero.
        /// </summary>
        public Tag LastTag
        {
            get { return _tag; }
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
                return LastTag.TagType == Tag.Type.Begin;
            }
            return LastTag.TagType == Tag.Type.End;
        }

        /// <summary>
        /// Parse an attribute name, if one is present.
        /// </summary>
        /// <returns>Return the attribute name, or null if none present.</returns>
        protected string ParseAttributeName()
        {
            EatWhitespace();

            if ("\"\'".IndexOf((char) _source.Peek()) == -1)
            {
                var buffer = new StringBuilder();
                while (!char.IsWhiteSpace((char) _source.Peek())
                       && (_source.Peek() != '=')
                       && (_source.Peek() != '>')
                       && (_source.Peek() != -1))
                {
                    int ch = ParseSpecialCharacter();
                    buffer.Append((char) ch);
                }
                return buffer.ToString();
            }
            return (ParseString());
        }

        /// <summary>
        /// Parse any special characters
        /// </summary>
        /// <returns>The character that was parsed.</returns>
        private char ParseSpecialCharacter()
        {
            var result = (char) _source.Read();
            int advanceBy = 0;

            // is there a special character?
            if (result == '&')
            {
                int ch;
                var buffer = new StringBuilder();

                // loop through and read special character
                do
                {
                    ch = _source.Peek(advanceBy++);
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
                        if (_charMap.ContainsKey(b))
                        {
                            result = _charMap[b];
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
            if ("\"\'".IndexOf((char) _source.Peek()) != -1)
            {
                int delim = _source.Read();
                while ((_source.Peek() != delim)
                       && (_source.Peek() != -1))
                {
                    if (result.Length > MaxLength)
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
                if ("\"\'".IndexOf((char) _source.Peek()) != -1)
                {
                    _source.Read();
                }
            }
            else
            {
                while (!char.IsWhiteSpace((char) _source.Peek())
                       && (_source.Peek() != -1)
                       && (_source.Peek() != '>'))
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
            _tag.Clear();
            _insertEndTag = null;
            var tagName = new StringBuilder();

            _source.Read();

            // Is it a comment?
            if (_source.Peek(TagConst.CommentBegin))
            {
                _source.Skip(TagConst.CommentBegin.Length);
                while (!_source.Peek(TagConst.CommentEnd))
                {
                    int ch = _source.Read();
                    if (ch != -1)
                    {
                        tagName.Append((char) ch);
                    }
                    else
                    {
                        break;
                    }
                }
                _source.Skip(TagConst.CommentEnd.Length);
                _tag.TagType = Tag.Type.Comment;
                _tag.Name = tagName.ToString();
                return;
            }

            // Is it CDATA?
            if (_source.Peek(TagConst.CDATABegin))
            {
                _source.Skip(TagConst.CDATABegin.Length);
                while (!_source.Peek(TagConst.CDATAEnd))
                {
                    int ch = _source.Read();
                    if (ch != -1)
                    {
                        tagName.Append((char) ch);
                    }
                    else
                    {
                        break;
                    }
                }
                _source.Skip(TagConst.CDATAEnd.Length);
                _tag.TagType = Tag.Type.CDATA;
                _tag.Name = tagName.ToString();
                return;
            }

            // Find the tag name
            while (_source.Peek() != -1)
            {
                // if this is the end of the tag, then stop
                if (char.IsWhiteSpace((char) _source.Peek())
                    || (_source.Peek() == '>'))
                {
                    break;
                }

                // if this is both a begin and end tag then stop
                if ((tagName.Length > 0) && (_source.Peek() == '/'))
                {
                    break;
                }

                tagName.Append((char) _source.Read());
            }

            EatWhitespace();

            if (tagName[0] == '/')
            {
                _tag.Name = tagName.ToString().Substring(1);
                _tag.TagType = Tag.Type.End;
            }
            else
            {
                _tag.Name = tagName.ToString();
                _tag.TagType = Tag.Type.Begin;
            }
            // get the attributes

            while ((_source.Peek() != '>') && (_source.Peek() != -1))
            {
                string attributeName = ParseAttributeName();
                string attributeValue = null;

                if (attributeName.Equals("/"))
                {
                    EatWhitespace();
                    if (_source.Peek() == '>')
                    {
                        _insertEndTag = _tag.Name;
                        break;
                    }
                }

                // is there a value?
                EatWhitespace();
                if (_source.Peek() == '=')
                {
                    _source.Read();
                    attributeValue = ParseString();
                }

                _tag.SetAttribute(attributeName, attributeValue);
            }
            _source.Read();
        }

        /// <summary>
        /// Check to see if the ending tag is present.
        /// </summary>
        /// <param name="name">The type of end tag being sought.</param>
        /// <returns>True if the ending tag was found.</returns>
        private bool PeekEndTag(IEnumerable<char> name)
        {
            int i = 0;

            // pass any whitespace
            while ((_source.Peek(i) != -1)
                   && char.IsWhiteSpace((char) _source.Peek(i)))
            {
                i++;
            }

            // is a tag beginning
            if (_source.Peek(i) != '<')
            {
                return false;
            }
            i++;

            // pass any whitespace
            while ((_source.Peek(i) != -1)
                   && char.IsWhiteSpace((char) _source.Peek(i)))
            {
                i++;
            }

            // is it an end tag
            if (_source.Peek(i) != '/')
            {
                return false;
            }
            i++;

            // pass any whitespace
            while ((_source.Peek(i) != -1)
                   && char.IsWhiteSpace((char) _source.Peek(i)))
            {
                i++;
            }

            // does the name match
            foreach (char t in name)
            {
                if (char.ToLower((char) _source.Peek(i)) != char
                                                                .ToLower(t))
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
            if (_insertEndTag != null)
            {
                _tag.Clear();
                _tag.Name = _insertEndTag;
                _tag.TagType = Tag.Type.End;
                _insertEndTag = null;
                return 0;
            }

            // handle locked end tag
            if (_lockedEndTag != null)
            {
                if (PeekEndTag(_lockedEndTag))
                {
                    _lockedEndTag = null;
                }
                else
                {
                    return _source.Read();
                }
            }

            // look for next tag
            if (_source.Peek() == '<')
            {
                ParseTag();

                if ((_tag.TagType == Tag.Type.Begin)
                    && ((StringUtil.EqualsIgnoreCase(_tag.Name, "script"))
                        || (StringUtil.EqualsIgnoreCase(_tag.Name, "style"))))
                {
                    _lockedEndTag = _tag.Name.ToLower();
                }
                return 0;
            }
            if (_source.Peek() == '&')
            {
                return ParseSpecialCharacter();
            }
            return (_source.Read());
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
            if (_tag != null)
            {
                result.Append(_tag.ToString());
            }
            result.Append("]");
            return result.ToString();
        }
    }
}
