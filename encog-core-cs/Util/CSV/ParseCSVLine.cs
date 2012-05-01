using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Util.CSV
{
    public class ParseCSVLine
    {
        public CSVFormat Format { get; private set; }

        public ParseCSVLine(CSVFormat theFormat)
        {
            Format = theFormat;
        }

        public IList<String> Parse(string line)
        {
            if (Format.Separator == ' ')
            {
                return ParseSpaceSep(line);
            }
            else
            {
                return ParseCharSep(line);
            }
        }

        /// <summary>
        /// Parse the line into a list of values.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>The elements on this line.</returns>
        private IList<String> ParseCharSep(string line)
        {
            var item = new StringBuilder();
            var result = new List<String>();
            bool quoted = false;
            bool hadQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char ch = line[i];
                if ((ch == Format.Separator) && !quoted)
                {
                    String s = item.ToString();
                    if (!hadQuotes)
                    {
                        s = s.Trim();
                    }
                    result.Add(s);
                    item.Length = 0;
                    quoted = false;
                    hadQuotes = false;
                }
                else if ((ch == '\"') && quoted)
                {
                    quoted = false;
                }
                else if ((ch == '\"') && (item.Length == 0))
                {
                    hadQuotes = true;
                    quoted = true;
                }
                else
                {
                    item.Append(ch);
                }
            }

            if (item.Length > 0)
            {
                String s = item.ToString();
                if (!hadQuotes)
                {
                    s = s.Trim();
                }
                result.Add(s);
            }

            return result;
        }

        /// <summary>
        /// Parse a line with space separators.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>The list of items from the line.</returns>
        private static List<String> ParseSpaceSep(String line)
        {
            var result = new List<String>();
            var parse = new SimpleParser(line);

            while (!parse.EOL())
            {
                result.Add(parse.Peek() == '\"' ? parse.ReadQuotedString() : parse.ReadToWhiteSpace());
                parse.EatWhiteSpace();
            }

            return result;
        }


    }
}
