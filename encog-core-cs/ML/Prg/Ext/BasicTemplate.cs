using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util;
using Encog.ML.EA.Exceptions;
using Encog.ML.Prg.ExpValue;
using Encog.MathUtil.Randomize;

namespace Encog.ML.Prg.Ext
{
    /// <summary>
    /// A basic template.
    /// </summary>
    public class BasicTemplate : IProgramExtensionTemplate
    {
        /// <summary>
        /// Defines a very low precidence.
        /// </summary>
        public const int NO_PREC = 100;

        public delegate ExpressionValue DelEvaluate(ProgramNode actual);
        public delegate bool DelIsPossibleReturnType(EncogProgramContext context, EPLValueType rtn);
        public delegate void DelRandomize(EncogRandom rnd, IList<EPLValueType> desiredType, ProgramNode actual, double minValue, double maxValue);



        /// <summary>
        /// The name of this opcode.
        /// </summary>
        private String name;

        /// <summary>
        /// True if this opcode has a variable value, other than variance of its
        /// child nodes.
        /// </summary>
        private bool varValue;

        /// <summary>
        /// The amount of data that is stored with this node.
        /// </summary>
        private int dataSize;

        /// <summary>
        /// The node type.
        /// </summary>
        private NodeType nodeType;

        /// <summary>
        /// The precedence.
        /// </summary>
        private int precedence;

        /// <summary>
        /// The opcode signature.
        /// </summary>
        private String signature;

        /// <summary>
        /// The parameters.
        /// </summary>
        private IList<ParamTemplate> param = new List<ParamTemplate>();

        /// <summary>
        /// The return value.
        /// </summary>
        private ParamTemplate returnValue;

        private DelEvaluate delEvaluate;

        private DelIsPossibleReturnType delIsPossibleReturnType;

        private DelRandomize delRandomize;

        /// <summary>
        /// Construct a basic template object.
        /// </summary>
        /// <param name="thePrecedence">The precedence.</param>
        /// <param name="theSignature">The opcode signature.</param>
        /// <param name="theType">The opcode type.</param>
        /// <param name="isVariable">True, if this opcode is a variable.</param>
        /// <param name="theDataSize">The data size kept for this opcode.</param>
        public BasicTemplate(int thePrecedence, String theSignature,
                NodeType theType, bool isVariable,
                int theDataSize,
                DelEvaluate theEvaluate,
                DelIsPossibleReturnType theIsPossibleReturnType,
                DelRandomize theRandomize)
        {
            this.precedence = thePrecedence;
            this.signature = theSignature;
            this.varValue = isVariable;
            this.dataSize = theDataSize;
            this.nodeType = theType;

            this.delEvaluate = theEvaluate;
            this.delIsPossibleReturnType = theIsPossibleReturnType;
            this.delRandomize = theRandomize;

            if (theSignature.Trim().Equals("("))
            {
                // special case, we add a left-paren for the shunting yard alg.
                this.name = theSignature;
                this.returnValue = null;
            }
            else
            {
                // non-special case, find the name of the function/operator
                SimpleParser parser = new SimpleParser(theSignature);
                bool pass = false;

                parser.EatWhiteSpace();
                this.name = parser.ReadToChars("(").Trim();
                parser.Advance();

                bool done = false;
                while (!done)
                {
                    if (parser.Peek() == ')')
                    {
                        parser.Advance();
                        done = true;
                    }
                    else if (parser.Peek() == ':')
                    {
                        parser.Advance();
                        pass = true;
                    }
                    else if (parser.Peek() == '{')
                    {
                        ParamTemplate temp = ReadParam(parser);
                        temp.PassThrough = pass;
                        pass = false;
                        this.param.Add(temp);
                    }
                    else
                    {
                        parser.Advance();
                        if (parser.EOL())
                        {
                            throw new EncogError("Invalid opcode template.");
                        }
                    }
                }

                // get the return type
                parser.EatWhiteSpace();
                if (!parser.LookAhead(":",true))
                {
                    throw new EACompileError("Return type not specified.");
                }
                parser.Advance();
                parser.EatWhiteSpace();
                this.returnValue = ReadParam(parser);
            }
        }

        /// <summary>
        /// Construct a function based on the provided signature.
        /// </summary>
        /// <param name="theSignature">The signature.</param>
        public BasicTemplate(String theSignature, DelEvaluate theEvaluate)
            : this(0, theSignature, NodeType.Function, false, 0, theEvaluate, null, null)
        {

        }

        /// <inheritdoc/>
        public int ChildNodeCount
        {
            get
            {
                return this.param.Count;
            }
        }

        /// <inheritdoc/>
        public int DataSize
        {
            get
            {
                return this.dataSize;
            }
        }

        /// <inheritdoc/>
        public String Name
        {
            get
            {
                return this.name;
            }
        }

        /// <inheritdoc/>
        public NodeType NodeType
        {
            get
            {
                return this.nodeType;
            }
        }

        /// <inheritdoc/>
        public IList<ParamTemplate> Params
        {
            get
            {
                return this.param;
            }
        }

        /// <inheritdoc/>
        public int Precedence
        {
            get
            {
                return this.precedence;
            }
        }

        /// <inheritdoc/>
        public ParamTemplate ReturnValue
        {
            get
            {
                return this.returnValue;
            }
        }

        /// <summary>
        /// The signature.
        /// </summary>
        public String Signature
        {
            get
            {
                return this.signature;
            }
        }

        /// <inheritdoc/>
        public bool IsVariable
        {
            get
            {
                return this.varValue;
            }
        }

        /// <summary>
        /// Read the specified parameter.
        /// </summary>
        /// <param name="parser">The parser to use.</param>
        /// <returns>The parsed parameter.</returns>
        private ParamTemplate ReadParam(SimpleParser parser)
        {
            ParamTemplate result = new ParamTemplate();

            if (!parser.LookAhead("{",true))
            {
                throw new EACompileError("Expected {");
            }
            parser.Advance();

            bool done = false;
            StringBuilder buffer = new StringBuilder();

            while (!done)
            {
                if (parser.Peek() == '}')
                {
                    done = true;
                    parser.Advance();
                }
                else if (parser.Peek() == '{')
                {
                    throw new EACompileError("Unexpected {");
                }
                else if (parser.Peek() == '{')
                {
                    done = true;
                    parser.Advance();
                }
                else if (parser.Peek() == ',')
                {
                    result.AddType(buffer.ToString().Trim().ToLower());
                    parser.Advance();
                    buffer.Length = 0;
                }
                else
                {
                    buffer.Append(parser.ReadChar());
                }
            }

            String s = buffer.ToString().Trim();
            if (s.Length > 0)
            {
                result.AddType(s);
            }

            return result;
        }

        /// <inheritdoc/>
        public String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[BasicTemplate:");
            result.Append(this.signature);
            result.Append(",type=");
            result.Append(this.nodeType.ToString());
            result.Append(",argCount=");
            result.Append(ChildNodeCount);
            result.Append("]");
            return result.ToString();
        }


        public ExpressionValue Evaluate(ProgramNode actual)
        {
            return delEvaluate(actual);
        }

        /// <inheritdoc/>
        public bool IsPossibleReturnType(EncogProgramContext context, EPLValueType rtn)
        {
            if (delIsPossibleReturnType == null)
            {
                return this.returnValue.PossibleTypes.Contains(rtn);
            }
            else if (!this.returnValue.PossibleTypes.Contains(rtn))
            {
                return false;
            }
            
            return delIsPossibleReturnType(context, rtn);
        }


        /// <inheritdoc/>
        public void Randomize(EncogRandom rnd, IList<EPLValueType> desiredType, ProgramNode actual, double minValue, double maxValue)
        {
            if (delRandomize != null)
            {
                delRandomize(rnd, desiredType, actual, minValue, maxValue);
            }
        }
    }
}
