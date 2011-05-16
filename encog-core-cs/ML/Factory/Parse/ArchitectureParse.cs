using System;
using System.Collections.Generic;
using System.Text;
using Encog.Util;
using Encog.Util.Logging;

namespace Encog.ML.Factory.Parse
{
    /// <summary>
    /// This class is used to parse a Encog architecture string.
    /// </summary>
    ///
    public sealed class ArchitectureParse
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        ///
        private ArchitectureParse()
        {
        }

        /// <summary>
        /// parse a layer.
        /// </summary>
        ///
        /// <param name="line">The line to parse.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The parsed ArchitectureLayer.</returns>
        public static ArchitectureLayer ParseLayer(String line,
                                                   int defaultValue)
        {
            var layer = new ArchitectureLayer();

            String check = line.Trim().ToUpper();

            // first check for bias
            if (check.EndsWith(":B"))
            {
                check = check.Substring(0, (check.Length - 2) - (0));
                layer.Bias = true;
            }

            // see if simple number
            try
            {
                layer.Count = Int32.Parse(check);
                if (layer.Count < 0)
                {
                    throw new EncogError("Count cannot be less than zero.");
                }
            }
            catch (FormatException f)
            {
                EncogLogging.Log(f);
            }

            // see if it is a default
            if ("?".Equals(check))
            {
                if (defaultValue < 0)
                {
                    throw new EncogError("Default (?) in an invalid location.");
                }
                else
                {
                    layer.Count = defaultValue;
                    layer.UsedDefault = true;
                    return layer;
                }
            }

            // single item, no function
            int startIndex = check.IndexOf('(');
            int endIndex = check.LastIndexOf(')');
            if (startIndex == -1)
            {
                layer.Name = check;
                return layer;
            }

            // function
            if (endIndex == -1)
            {
                throw new EncogError("Illegal parentheses.");
            }

            layer.Name = check.Substring(0, (startIndex) - (0)).Trim();

            String paramStr = check.Substring(startIndex + 1, (endIndex) - (startIndex + 1));
            IDictionary<String, String> paras = ParseParams(paramStr);
            EngineArray.PutAll(paras, layer.Params);
            return layer;
        }

        /// <summary>
        /// Parse all layers from a line of text.
        /// </summary>
        ///
        /// <param name="line">The line of text.</param>
        /// <returns>A list of the parsed layers.</returns>
        public static IList<String> ParseLayers(String line)
        {
            IList<String> result = new List<String>();

            int bs = 0;
            bool done = false;

            do
            {
                String part;
                int index = line.IndexOf("->", bs);
                if (index != -1)
                {
                    part = line.Substring(bs, (index) - (bs)).Trim();
                    bs = index + 2;
                }
                else
                {
                    part = line.Substring(bs).Trim();
                    done = true;
                }

                bool bias = part.EndsWith("b");
                if (bias)
                {
                    part = part.Substring(0, (part.Length - 1) - (0));
                }

                result.Add(part);
            } while (!done);

            return result;
        }

        /// <summary>
        /// Parse a name.
        /// </summary>
        ///
        /// <param name="parser">The parser to use.</param>
        /// <returns>The name.</returns>
        private static String ParseName(SimpleParser parser)
        {
            var result = new StringBuilder();
            parser.EatWhiteSpace();
            while (parser.IsIdentifier())
            {
                result.Append(parser.ReadChar());
            }
            return result.ToString();
        }

        /// <summary>
        /// Parse parameters.
        /// </summary>
        ///
        /// <param name="line">The line to parse.</param>
        /// <returns>The parsed values.</returns>
        public static IDictionary<String, String> ParseParams(String line)
        {
            IDictionary<String, String> result = new Dictionary<String, String>();

            var parser = new SimpleParser(line);

            while (!parser.EOL())
            {
                String name = ParseName(parser)
                    .ToUpper();

                parser.EatWhiteSpace();
                if (!parser.LookAhead("=", false))
                {
                    throw new EncogError("Missing equals(=) operator.");
                }
                else
                {
                    parser.Advance();
                }

                String value_ren = ParseValue(parser);

                result[name.ToUpper()] = value_ren;

                if (!parser.ParseThroughComma())
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Parse a value.
        /// </summary>
        ///
        /// <param name="parser">The parser to use.</param>
        /// <returns>The newly parsed value.</returns>
        private static String ParseValue(SimpleParser parser)
        {
            bool quoted = false;
            var str = new StringBuilder();

            parser.EatWhiteSpace();

            if (parser.Peek() == '\"')
            {
                quoted = true;
                parser.Advance();
            }

            while (!parser.EOL())
            {
                if (parser.Peek() == '\"')
                {
                    if (quoted)
                    {
                        parser.Advance();
                        if (parser.Peek() == '\"')
                        {
                            str.Append(parser.ReadChar());
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        str.Append(parser.ReadChar());
                    }
                }
                else if (!quoted
                         && (parser.IsWhiteSpace() || (parser.Peek() == ',')))
                {
                    break;
                }
                else
                {
                    str.Append(parser.ReadChar());
                }
            }
            return str.ToString();
        }
    }
}