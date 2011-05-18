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
        private int currentPosition;

        /// <summary>
        /// The marked position.
        /// </summary>
        private int marked;

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
            return Math.Max(Line.Length - currentPosition, 0);
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
            else if (currentPosition >= Line.Length)
                return (char) 0;
            else
                return Line[currentPosition];
        }

        /// <summary>
        /// Advance beyond the next character.
        /// </summary>
        public void Advance()
        {
            if (currentPosition < Line.Length)
            {
                currentPosition++;
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
            return (currentPosition >= Line.Length);
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
                char c2 = Line[currentPosition + i];

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
            currentPosition = Math.Min(Line.Length, currentPosition + p);
        }

        /// <summary>
        /// Mark the current position.
        /// </summary>
        public void Mark()
        {
            marked = currentPosition;
        }

        /// <summary>
        /// Reset back to the marked position.
        /// </summary>
        public void Reset()
        {
            currentPosition = marked;
        }
    }
}