using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util
{
    public class SimpleParser
    {
        public String Line { get; set; }
        private int currentPosition;
        private int marked;

        public SimpleParser(String line)
        {
            this.Line = line;
        }

        public int Remaining()
        {
            return Math.Max(this.Line.Length - this.currentPosition, 0);
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
                return (char)0;
            else if (currentPosition >= this.Line.Length)
                return (char)0;
            else
                return this.Line[this.currentPosition];
        }

        public void Advance()
        {
            if (currentPosition < this.Line.Length)
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
            return (this.currentPosition >= this.Line.Length);
        }

        public void EatWhiteSpace()
        {
            while (!EOL() && IsWhiteSpace())
                Advance();
        }

        public char ReadChar()
        {
            if (EOL())
                return (char)0;

            char ch = Peek();
            Advance();
            return ch;
        }

        public String ReadToWhiteSpace()
        {
            StringBuilder result = new StringBuilder();

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
                char c2 = this.Line[this.currentPosition+i];

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
            this.currentPosition=Math.Min(Line.Length,this.currentPosition+p);
        }

        public void Mark()
        {
            this.marked = this.currentPosition;
        }

        public void Reset()
        {
            this.currentPosition = this.marked;
        }
    }
}
