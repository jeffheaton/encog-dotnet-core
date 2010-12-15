using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util;

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
       
        private IDictionary<String, String> parameters = new Dictionary<string, string>();
        private SimpleParser parser;
        

        public ParseLine(String line)
        {
            bool more = true;

            this.parser = new SimpleParser(line);

            if (this.parser.LookAhead("//",false) )
                return;

            ParseCommand();

            // look for a "primary parameter"
            parser.EatWhiteSpace();
            if (parser.Peek() == '\"')
            {
                this.PrimaryParameter = ParseValue();
                parser.ParseThroughComma();
            }

            if (more)
            {
                while (!parser.EOL())
                {
                    if (this.parser.LookAhead("//",false))
                        return;
                    
                    String name = ParseName().ToLower();

                    this.parser.EatWhiteSpace();
                    if (!this.parser.LookAhead("=", false))
                        ThrowParseError();
                    else
                        parser.Advance();

                    String Value = ParseValue();

                    this.parameters[name] = Value;
                    if (!parser.ParseThroughComma())
                        break;
                }
            }

            if (parser.LookAhead("//",false))
                return;

            if (!parser.EOL())
            {
                ThrowParseError();
            }
        }        

        private String ParseName()
        {
            StringBuilder result = new StringBuilder();
            parser.EatWhiteSpace();
            while (parser.IsIdentifier())
            {
                result.Append(parser.ReadChar());
            }
            return result.ToString();
        }


        private void ParseCommand()
        {
            StringBuilder str = new StringBuilder();

            this.parser.EatWhiteSpace();
            this.Command = this.parser.ReadToWhiteSpace().ToLower();
        }

        private String ParseValue()
        {
            bool quoted = false;
            StringBuilder str = new StringBuilder();

            this.parser.EatWhiteSpace();

            if (this.parser.Peek() == '\"')
            {
                quoted = true;
                this.parser.Advance();
            }

            while (!this.parser.EOL())
            {
                if (this.parser.Peek() == '\"')
                {
                    if (quoted)
                    {
                        this.parser.Advance();
                        if (this.parser.Peek() == '\"')
                        {
                            str.Append(this.parser.ReadChar());
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        str.Append(this.parser.ReadChar());
                    }
                }
                else if (!quoted && (this.parser.IsWhiteSpace() || this.parser.Peek() == ','))
                {
                    break;
                }
                else
                {
                    str.Append(this.parser.ReadChar());
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
            throw new QuantError("Invalid command: " + this.parser.Line);
        }

        public String GetParameterString(String name, bool required)
        {
            if (!Parameters.ContainsKey(name) )
            {
                if (required)
                    throw new ScriptError("Must define:" + name);
                else
                    return null;
            }

            return Parameters[name];
        }

        internal int GetParameterInt(string name, bool required)
        {
            int result;
            String str = GetParameterString(name, required);

            if (str == null)
                return 0;

            if (!int.TryParse(str, out result))
                throw new ScriptError("Invalid number: " + str);

            return result;
        }
    }
}
