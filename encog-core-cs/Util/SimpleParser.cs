using System;
using System.Text;

namespace Encog.Util
{
    public class SimpleParser
    {
        private int currentPosition;
        private int marked;

        public SimpleParser(String line)
        {
            Line = line;
        }

        public String Line { get; set; }

        public int Remaining()
        {
            return Math.Max(Line.Length - currentPosition, 0);
        }

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

        public bool IsIdentifier()
        {
            if (EOL())
                return false;

            return char.IsLetterOrDigit(Peek()) || Peek() == '_';
        }

        public char Peek()
        {
            if (EOL())
                return (char) 0;
            else if (currentPosition >= Line.Length)
                return (char) 0;
            else
                return Line[currentPosition];
        }

        public void Advance()
        {
            if (currentPosition < Line.Length)
            {
                currentPosition++;
            }
        }

        public bool IsWhiteSpace()
        {
            return " \t\n\r".IndexOf(Peek()) != -1;
        }

        public bool EOL()
        {
            return (currentPosition >= Line.Length);
        }

        public void EatWhiteSpace()
        {
            while (!EOL() && IsWhiteSpace())
                Advance();
        }

        public char ReadChar()
        {
            if (EOL())
                return (char) 0;

            char ch = Peek();
            Advance();
            return ch;
        }

        public String ReadToWhiteSpace()
        {
            var result = new StringBuilder();

            while (!IsWhiteSpace() && !EOL())
            {
                result.Append(ReadChar());
            }

            return result.ToString();
        }

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


        public void Advance(int p)
        {
            currentPosition = Math.Min(Line.Length, currentPosition + p);
        }

        public void Mark()
        {
            marked = currentPosition;
        }

        public void Reset()
        {
            currentPosition = marked;
        }
    }
}