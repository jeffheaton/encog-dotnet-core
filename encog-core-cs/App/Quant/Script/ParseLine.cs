using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Script
{
    public class ParseLine
    {
        public String Command { get; set; }

        public String PrimaryParameter { get; set; }

        public IDictionary<String, String> Parameters
        {
            get
            {
                return this.parameters;
            }
        }

        public String Line { get; set; }

        private IDictionary<String, String> parameters = new Dictionary<string, string>();

        private int currentPosition;

        public ParseLine(String line)
        {
            bool more = true;

            this.Line = line.Trim();
            this.currentPosition = 0;

            if (IsComment())
                return;

            ParseCommand();

            // look for a "primary parameter"
            EatWhiteSpace();
            if (Peek() == '\"')
            {
                this.PrimaryParameter = ParseValue();
                ParseThroughComma();
            }

            if (more)
            {
                while (!EOL())
                {
                    if (IsComment())
                        return;
                    
                    String name = ParseName();
                    Expect('=');
                    String Value = ParseValue();

                    this.parameters[name] = Value;
                    if (!ParseThroughComma())
                        break;
                }
            }

            if (IsComment())
                return;

            if (!EOL())
            {
                ThrowParseError();
            }
        }

        private bool IsComment()
        {
            EatWhiteSpace();

            if (Remaining() < 2)
                return false;

            if (this.Line[this.currentPosition] == '/' && this.Line[this.currentPosition + 1] == '/')
            {
                return true;
            }
            return false;
        }

        private int Remaining()
        {
            return Math.Max(this.Line.Length - this.currentPosition, 0);
        }

        private bool ParseThroughComma()
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

        private void Expect(char ch)
        {
            EatWhiteSpace();
            if (ReadChar() != ch)
                ThrowParseError();
        }

        private bool IsIdentifier()
        {
            if (EOL())
                return false;

            return char.IsLetterOrDigit(Peek()) || Peek() == '_';
        }

        private String ParseName()
        {
            StringBuilder result = new StringBuilder();
            EatWhiteSpace();
            while (IsIdentifier())
            {
                result.Append(ReadChar());
            }
            return result.ToString();
        }

        private char Peek()
        {
            if (EOL())
                return (char)0;
            else if (currentPosition >= this.Line.Length)
                return (char)0;
            else
                return this.Line[this.currentPosition];
        }

        private void Advance()
        {
            if (currentPosition < this.Line.Length)
            {
                currentPosition++;
            }
        }

        private bool IsWhiteSpace()
        {
            return " \t\n\r".IndexOf(Peek()) != -1;
        }

        private bool EOL()
        {
            return(this.currentPosition>=this.Line.Length);
        }

        private void EatWhiteSpace()
        {
            while (!EOL() && IsWhiteSpace() )
                Advance();
        }

        private char ReadChar()
        {
            if (EOL())
                return (char)0;

            char ch = Peek();
            Advance();
            return ch;
        }

        private String ReadToWhiteSpace()
        {
            StringBuilder result = new StringBuilder();

            while (!IsWhiteSpace() && !EOL())
            {
                result.Append(ReadChar());
            }

            return result.ToString();
        }

        private void ParseCommand()
        {
            StringBuilder str = new StringBuilder();

            EatWhiteSpace();
            this.Command = ReadToWhiteSpace();
        }

        private String ParseValue()
        {
            bool quoted = false;
            StringBuilder str = new StringBuilder();

            EatWhiteSpace();

            if (Peek() == '\"')
            {
                quoted = true;
                Advance();
            }

            while (!EOL())
            {
                if (Peek() == '\"')
                {
                    if (quoted)
                    {
                        Advance();
                        if (Peek() == '\"')
                        {
                            str.Append(ReadChar());
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        str.Append(ReadChar());
                    }
                }
                else if (!quoted && (IsWhiteSpace() || Peek() == ','))
                {
                    break;
                }
                else
                {
                    str.Append(ReadChar());
                }
            }
            return str.ToString();

        }

        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append('[');
            result.Append(Command);
            result.Append(':');
            result.Append(PrimaryParameter);
            result.Append(':');
            foreach (String key in Parameters.Keys)
            {
                result.Append(key);
                result.Append('=');
                result.Append(Parameters[key]);
                result.Append(',');
            }

            result.Append(']');
            return result.ToString();
        }

        private void ThrowParseError()
        {
            throw new QuantError("Invalid command: " + Line);
        }
    }
}
