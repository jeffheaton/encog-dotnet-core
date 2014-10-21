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
using System.Text;

namespace Encog.Util
{
    /// <summary>
    /// A very simple text parser.
    /// </summary>
    public class SimpleParser
    {
        /// <summary>
        /// The current position.
        /// </summary>
        private int _currentPosition;

        /// <summary>
        /// The marked position.
        /// </summary>
        private int _marked;

        /// <summary>
        /// Construct the object for the specified line.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        public SimpleParser(String line)
        {
            Line = line;
        }

        /// <summary>
        /// The line being parsed.
        /// </summary>
        public String Line { get; set; }

        /// <summary>
        /// The number of characters remaining.
        /// </summary>
        /// <returns>The number of characters remaining.</returns>
        public int Remaining()
        {
            return Math.Max(Line.Length - _currentPosition, 0);
        }

        /// <summary>
        /// Parse through a comma.
        /// </summary>
        /// <returns>True, if the comma was found.</returns>
        public bool ParseThroughComma()
        {
            EatWhiteSpace();
            if (!EOL())
            {
                if (Peek() == ',')
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// CHeck to see if the next character is an identifier.
        /// </summary>
        /// <returns>True, if the next char is an identifier.</returns>
        public bool IsIdentifier()
        {
            if (EOL())
                return false;

            return char.IsLetterOrDigit(Peek()) || Peek() == '_';
        }

        /// <summary>
        /// Peek ahead to see the next character.  But do not advance beyond it.
        /// </summary>
        /// <returns>The next character.</returns>
        public char Peek()
        {
            if (EOL())
                return (char) 0;
            if (_currentPosition >= Line.Length)
                return (char) 0;
            return Line[_currentPosition];
        }

        /// <summary>
        /// Advance beyond the next character.
        /// </summary>
        public void Advance()
        {
            if (_currentPosition < Line.Length)
            {
                _currentPosition++;
            }
        }

        /// <summary>
        /// Returns true if the next character is a white space.
        /// </summary>
        /// <returns>True, if the next character is a white space.</returns>
        public bool IsWhiteSpace()
        {
            return " \t\n\r".IndexOf(Peek()) != -1;
        }

        /// <summary>
        /// Returns true of there are no more characters to read.
        /// </summary>
        /// <returns>True, if we have reached end of line.</returns>
        public bool EOL()
        {
            return (_currentPosition >= Line.Length);
        }

        /// <summary>
        /// Strip any white space from the current position.
        /// </summary>
        public void EatWhiteSpace()
        {
            while (!EOL() && IsWhiteSpace())
                Advance();
        }

        /// <summary>
        /// Read the next character.
        /// </summary>
        /// <returns>The next character.</returns>
        public char ReadChar()
        {
            if (EOL())
                return (char) 0;

            char ch = Peek();
            Advance();
            return ch;
        }

        /// <summary>
        /// Read text up to the next white space.
        /// </summary>
        /// <returns>The text read up to the next white space.</returns>
        public String ReadToWhiteSpace()
        {
            var result = new StringBuilder();

            while (!IsWhiteSpace() && !EOL())
            {
                result.Append(ReadChar());
            }

            return result.ToString();
        }

        /// <summary>
        /// Look ahead to see if the specified string is present.
        /// </summary>
        /// <param name="str">The string searching for.</param>
        /// <param name="ignoreCase">True if case is to be ignored.</param>
        /// <returns>True if the string is present.</returns>
        public bool LookAhead(String str, bool ignoreCase)
        {
            if (Remaining() < str.Length)
                return false;
            for (int i = 0; i < str.Length; i++)
            {
                char c1 = str[i];
                char c2 = Line[_currentPosition + i];

                if (ignoreCase)
                {
                    c1 = char.ToLower(c1);
                    c2 = char.ToLower(c2);
                }

                if (c1 != c2)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Advance the specified number of characters.
        /// </summary>
        /// <param name="p">The number of characters to advance.</param>
        public void Advance(int p)
        {
            _currentPosition = Math.Min(Line.Length, _currentPosition + p);
        }

        /// <summary>
        /// Mark the current position.
        /// </summary>
        public void Mark()
        {
            _marked = _currentPosition;
        }

        /// <summary>
        /// Reset back to the marked position.
        /// </summary>
        public void Reset()
        {
            _currentPosition = _marked;
        }

        /// <summary>
        /// Read a quoted string.
        /// </summary>
        /// <returns>The string that was read.</returns>
        public String ReadQuotedString()
        {
            if (Peek() != '\"')
                return "";

            var result = new StringBuilder();

            Advance();
            while (Peek() != '\"' && !EOL())
            {
                result.Append(ReadChar());
            }
            Advance();

            return result.ToString();
        }

        /// <summary>
        /// Read forward to the specified characters.
        /// </summary>
        /// <param name="chs">The characters to stop at.</param>
        /// <returns>The string that was read.</returns>
        public String ReadToChars(String chs)
        {
            StringBuilder result = new StringBuilder();

            while (chs.IndexOf(this.Peek()) == -1 && !EOL())
            {
                result.Append(ReadChar());
            }

            return result.ToString();
        }

    }
}
