using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Prg;
using Encog.Util;
using Encog.ML.EA.Exceptions;
using Encog.ML.Prg.Ext;
using Encog.ML.Prg.ExpValue;
using Encog.Util.CSV;

namespace Encog.Parse.Expression.EPL
{
    /// <summary>
    /// Parse an EPL string.
    /// </summary>
    public class ParseEPL
    {
        private EncogProgram holder;
        private SimpleParser parser;
        private Stack<ProgramNode> nodeStack = new Stack<ProgramNode>(100);

        public ParseEPL(EncogProgram theHolder)
        {
            this.holder = theHolder;
        }

        public ProgramNode Parse(String expression)
        {
            this.parser = new SimpleParser(expression);

            while (!this.parser.EOL())
            {
                this.parser.EatWhiteSpace();

                // read in the command
                if (this.parser.ReadChar() != '[')
                {
                    throw new EACompileError("Expected [");
                }
                this.parser.EatWhiteSpace();
                StringBuilder cmd = new StringBuilder();
                while (this.parser.Peek() != ']' && !this.parser.EOL())
                {
                    cmd.Append(this.parser.ReadChar());
                }

                if (this.parser.Peek() != ']')
                {
                    throw new EACompileError("Expected ]");
                }
                this.parser.Advance();


                // parse out the command
                string[] tok = cmd.ToString().Split(':');
                int idx = 0;
                String name = tok[idx++];
                int childCount = int.Parse(tok[idx++]);
                IProgramExtensionTemplate temp = EncogOpcodeRegistry.Instance.FindOpcode(name, childCount);
                if (temp == null)
                {
                    throw new EACompileError("Invalid instruction: " + name);
                }

                // build the arguments
                ProgramNode[] args = new ProgramNode[childCount];
                for (int i = args.Length - 1; i >= 0; i--)
                {
                    args[i] = this.nodeStack.Pop();
                }

                // factor the node
                ProgramNode node = this.holder.Functions.FactorProgramNode(name, this.holder, args);
                this.nodeStack.Push(node);

                // add any needed data to the node
                for (int i = 0; i < temp.DataSize; i++)
                {
                    String str = tok[idx++].Trim();
                    int strIdx = str.IndexOf('#');
                    if (strIdx != -1)
                    {
                        int enumType = int.Parse(str.Substring(0, strIdx));
                        int enumVal = int.Parse(str.Substring(strIdx + 1));
                        node.Data[0] = new ExpressionValue(enumType, enumVal);

                    }
                    // is it boolean?
                    else if (str.Length == 1 && "tf".IndexOf(char.ToLower(str[0])) != -1)
                    {
                        node.Data[i] = new ExpressionValue(string.Compare(str, "t", true));
                    }
                    // is it a string?
                    else if (str[0] == '\"')
                    {
                        node.Data[i] = new ExpressionValue(str.Substring(1, str.Length - 1));
                    }
                    // is it an integer
                    else if (str.IndexOf('.') == -1 && str.ToLower().IndexOf('e') == -1)
                    {
                        long l;
                        try
                        {
                            l = long.Parse(str);
                        }
                        catch (FormatException ex)
                        {
                            // sometimes C# will output a long value that is larger than can be parsed
                            // this is very likely not a useful genome and we just set it to zero so that
                            // the population load does not fail.
                            l = 0;
                        }
                        node.Data[i] = new ExpressionValue(l);
                    }
                    // At this point, must be a float
                    else
                    {
                        node.Data[i] = new ExpressionValue(CSVFormat.EgFormat.Parse(str));
                    }
                }
            }

            return this.nodeStack.Pop();
        }
    }
}
