// Encog Neural Network and Bot Library v1.x (DotNet)
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
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Encog.Bot.HTML
{
    /// <summary>
    /// This class implements an HTML parser.  This parser is used
    /// by the Heaton Research spider, but it can also be used as a
    /// stand alone HTML parser.
    /// </summary>
    public class ParseHTML
    {
        /// <summary>
        /// A mapping of certain HTML encoded values(i.e. &amp;nbsp;)
        /// to their actual character values.
        /// </summary>
        private static Dictionary<String, char> charMap;

        /// <summary>
        /// The stream that we are parsing fro
        /// </summary>
        private PeekableInputStream source;

        /// <summary>
        /// The HTML tag just parsed.
        /// </summary>
        private HTMLTag tag;

        /// <summary>
        /// The current HTML tag. Access this property if the read
        /// function returns 0.
        /// </summary>
        public HTMLTag Tag
        {
            get
            {
                return tag;
            }
            set
            {
                tag = value;
            }
        }

        /// <summary>
        /// Is there an end tag we are "locked into", such as 
        /// a comment tag, script tag or similar.
        /// </summary>
        private String lockedEndTag;


        /// <summary>
        /// Construct the HTML parser based in the specified stream.
        /// </summary>
        /// <param name="istream">The stream that will be parsed.</param>
        public ParseHTML(Stream istream)
        {
            this.source = new PeekableInputStream(istream);
            Tag = new HTMLTag();

            if (charMap == null)
            {
                charMap = new Dictionary<String, char>();
                charMap.Add("nbsp", ' ');
                charMap.Add("lt", '<');
                charMap.Add("gt", '>');
                charMap.Add("amp", '&');
                charMap.Add("quot", '\"');
                charMap.Add("bull", (char)149);
                charMap.Add("trade", (char)129);
            }
        }

        /// <summary>
        /// Read a single character from the HTML source, if this function returns zero(0) then you should call getTag to see what tag was found. Otherwise the value returned is simply the next character found.
        /// </summary>
        /// <returns>The character read, or zero if there is an HTML tag. If zero is returned, then call getTag to get the next tag.</returns>
        virtual public int Read()
        {
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
            if (this.source.Peek(0) == '<')
            {
                ParseTag();
                if (!this.Tag.Ending
                    && (String.Compare(this.Tag.Name, "script", true) == 0 || String.Compare(this.Tag.Name, "style", true) == 0))
                {
                    this.lockedEndTag = this.Tag.Name.ToLower();
                }
                return 0;
            }
            else if (this.source.Peek(0) == '&')
            {
                return ParseSpecialCharacter();
            }
            else
            {
                return (this.source.Read());
            }
        }

        /// <summary>
        /// Represent as a string.  Read all text and ignore tags.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {

            StringBuilder result = new StringBuilder();

            int ch = 0;
            StringBuilder text = new StringBuilder();
            do
            {
                ch = Read();
                if (ch == 0)
                {
                    if (text.Length > 0)
                    {
                        text.Length = 0;
                    }
                }
                else if (ch != -1)
                {
                    text.Append((char)ch);
                }
            } while (ch != -1);
            if (text.Length > 0)
            {
            }
            return result.ToString();

        }

        /// <summary>
        /// Parse any special characters(i.e. &amp;nbsp;).
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

                // Loop through and read special character.
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
                        catch (FormatException)
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
        /// See if the next few characters are an end tag.
        /// </summary>
        /// <param name="name">The end tag we are looking for.</param>
        /// <returns></returns>
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
                  .ToLower((char)name[j]))
                {
                    return false;
                }
                i++;
            }

            return true;
        }




        /// <summary>
        /// Remove any whitespace characters that are next in the InputStream.
        /// </summary>
        protected void EatWhitespace()
        {
            while (char.IsWhiteSpace((char)this.source.Peek(0)))
            {
                this.source.Read();
            }
        }


        /// <summary>
        /// Parse an attribute name, if one is present.
        /// </summary>
        /// <returns>The attribute name parsed.</returns>
        protected String ParseAttributeName()
        {
            EatWhitespace();

            if ("\"\'".IndexOf((char)this.source.Peek(0)) == -1)
            {
                StringBuilder buffer = new StringBuilder();
                while (!char.IsWhiteSpace((char)this.source.Peek(0))
                    && (this.source.Peek(0) != '=') && (this.source.Peek(0) != '>')
                    && (this.source.Peek(0) != -1))
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
        /// Called to parse a double or single quote string.
        /// </summary>
        /// <returns>The string parsed.</returns>
        protected String ParseString()
        {
            StringBuilder result = new StringBuilder();
            EatWhitespace();
            if ("\"\'".IndexOf((char)this.source.Peek(0)) != -1)
            {
                int delim = this.source.Read();
                while ((this.source.Peek(0) != delim) && (this.source.Peek(0) != -1))
                {
                    if (result.Length > 1000)
                    {
                        break;
                    }
                    int ch = ParseSpecialCharacter();
                    if ((ch == 13) || (ch == 10))
                    {
                        continue;
                    }
                    result.Append((char)ch);
                }
                if ("\"\'".IndexOf((char)this.source.Peek(0)) != -1)
                {
                    this.source.Read();
                }
            }
            else
            {
                while (!char.IsWhiteSpace((char)this.source.Peek(0))
                    && (this.source.Peek(0) != -1) && (this.source.Peek(0) != '>'))
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
            this.Tag.Clear();
            StringBuilder tagName = new StringBuilder();

            this.source.Read();

            // Is it a comment?
            if ((this.source.Peek(0) == '!') && (this.source.Peek(1) == '-')
                && (this.source.Peek(2) == '-'))
            {
                while (this.source.Peek(0) != -1)
                {
                    if ((this.source.Peek(0) == '-') && (this.source.Peek(1) == '-')
                        && (this.source.Peek(2) == '>'))
                    {
                        break;
                    }
                    if (this.source.Peek(0) != '\r')
                    {
                        tagName.Append((char)this.source.Peek(0));
                    }
                    this.source.Read();
                }
                tagName.Append("--");
                this.source.Read();
                this.source.Read();
                this.source.Read();
                return;
            }

            // Find the tag name
            while (this.source.Peek(0) != -1)
            {
                if (char.IsWhiteSpace((char)this.source.Peek(0))
                    || (this.source.Peek(0) == '>'))
                {
                    break;
                }
                tagName.Append((char)this.source.Read());
            }

            EatWhitespace();
            this.Tag.Name = tagName.ToString();

            // Get the attributes.

            while ((this.source.Peek(0) != '>') && (this.source.Peek(0) != -1))
            {
                String attributeName = ParseAttributeName();
                String attributeValue = null;

                if (attributeName.Equals("/"))
                {
                    EatWhitespace();
                    if (this.source.Peek(0) == '>')
                    {
                        this.Tag.Ending = true;
                        break;
                    }
                }

                // is there a value?
                EatWhitespace();
                if (this.source.Peek(0) == '=')
                {
                    this.source.Read();
                    attributeValue = ParseString();
                }

                this.Tag.SetAttribute(attributeName, attributeValue);
            }
            this.source.Read();
        }


    }
}
