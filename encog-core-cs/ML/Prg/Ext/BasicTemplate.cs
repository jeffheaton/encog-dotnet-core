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
using System.Collections.Generic;
using System.Text;
using Encog.ML.EA.Exceptions;
using Encog.ML.Prg.ExpValue;
using Encog.MathUtil.Randomize;
using Encog.Util;

namespace Encog.ML.Prg.Ext
{
    /// <summary>
    ///     A basic template.
    /// </summary>
    [Serializable]
    public class BasicTemplate : IProgramExtensionTemplate
    {
        /// <summary>
        /// Delegate to evaluate the node.
        /// </summary>
        /// <param name="actual">Actual node.</param>
        /// <returns>Evaluated value.</returns>
        public delegate ExpressionValue DelEvaluate(ProgramNode actual);

        /// <summary>
        /// Delegate to determine return types.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="rtn">The return type to check.</param>
        /// <returns>True, if this is a return type.</returns>
        public delegate bool DelIsPossibleReturnType(EncogProgramContext context, EPLValueType rtn);

        /// <summary>
        /// Delegate to randomize.
        /// </summary>
        /// <param name="rnd">Random number generator.</param>
        /// <param name="desiredType">The desired type.</param>
        /// <param name="actual">The actual node.</param>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        public delegate void DelRandomize(
            EncogRandom rnd, IList<EPLValueType> desiredType, ProgramNode actual, double minValue, double maxValue);

        /// <summary>
        ///     Defines a very low precidence.
        /// </summary>
        public const int NoPrec = 100;


        /// <summary>
        ///     The amount of data that is stored with this node.
        /// </summary>
        private readonly int _dataSize;

        /// <summary>
        /// The delegate to evaluate.
        /// </summary>
        private readonly DelEvaluate _delEvaluate;

        /// <summary>
        /// The delegate for return types.
        /// </summary>
        private readonly DelIsPossibleReturnType _delIsPossibleReturnType;

        /// <summary>
        /// The delegate for randomize.
        /// </summary>
        private readonly DelRandomize _delRandomize;

        /// <summary>
        ///     The name of this opcode.
        /// </summary>
        private readonly String _name;

        /// <summary>
        ///     The node type.
        /// </summary>
        private readonly NodeType _nodeType;

        /// <summary>
        ///     The parameters.
        /// </summary>
        private readonly IList<ParamTemplate> _param = new List<ParamTemplate>();

        /// <summary>
        ///     The precedence.
        /// </summary>
        private readonly int _precedence;

        /// <summary>
        ///     The return value.
        /// </summary>
        private readonly ParamTemplate _returnValue;

        /// <summary>
        ///     The opcode signature.
        /// </summary>
        private readonly String _signature;

        /// <summary>
        ///     True if this opcode has a variable value, other than variance of its
        ///     child nodes.
        /// </summary>
        private readonly bool _varValue;

        /// <summary>
        ///     Construct a basic template object.
        /// </summary>
        /// <param name="thePrecedence">The precedence.</param>
        /// <param name="theSignature">The opcode signature.</param>
        /// <param name="theType">The opcode type.</param>
        /// <param name="isVariable">True, if this opcode is a variable.</param>
        /// <param name="theDataSize">The data size kept for this opcode.</param>
        /// <param name="theEvaluate">The evaluator delegate.</param>
        /// <param name="theIsPossibleReturnType">The return type delegate.</param>
        /// <param name="theRandomize">The randomizer delegate.</param>
        public BasicTemplate(int thePrecedence, String theSignature,
                             NodeType theType, bool isVariable,
                             int theDataSize,
                             DelEvaluate theEvaluate,
                             DelIsPossibleReturnType theIsPossibleReturnType,
                             DelRandomize theRandomize)
        {
            _precedence = thePrecedence;
            _signature = theSignature;
            _varValue = isVariable;
            _dataSize = theDataSize;
            _nodeType = theType;

            _delEvaluate = theEvaluate;
            _delIsPossibleReturnType = theIsPossibleReturnType;
            _delRandomize = theRandomize;

            if (theSignature.Trim().Equals("("))
            {
                // special case, we add a left-paren for the shunting yard alg.
                _name = theSignature;
                _returnValue = null;
            }
            else
            {
                // non-special case, find the name of the function/operator
                var parser = new SimpleParser(theSignature);
                bool pass = false;

                parser.EatWhiteSpace();
                _name = parser.ReadToChars("(").Trim();
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
                        _param.Add(temp);
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
                if (!parser.LookAhead(":", true))
                {
                    throw new EACompileError("Return type not specified.");
                }
                parser.Advance();
                parser.EatWhiteSpace();
                _returnValue = ReadParam(parser);
            }
        }

        /// <summary>
        ///     Construct a function based on the provided signature.
        /// </summary>
        /// <param name="theSignature">The signature.</param>
        /// <param name="theEvaluate">The evaluate delegate.</param>
        public BasicTemplate(String theSignature, DelEvaluate theEvaluate)
            : this(0, theSignature, NodeType.Function, false, 0, theEvaluate, null, null)
        {
        }

        /// <summary>
        ///     The signature.
        /// </summary>
        public String Signature
        {
            get { return _signature; }
        }

        /// <inheritdoc />
        public int ChildNodeCount
        {
            get { return _param.Count; }
        }

        /// <inheritdoc />
        public int DataSize
        {
            get { return _dataSize; }
        }

        /// <inheritdoc />
        public String Name
        {
            get { return _name; }
        }

        /// <inheritdoc />
        public NodeType NodeType
        {
            get { return _nodeType; }
        }

        /// <inheritdoc />
        public IList<ParamTemplate> Params
        {
            get { return _param; }
        }

        /// <inheritdoc />
        public int Precedence
        {
            get { return _precedence; }
        }

        /// <inheritdoc />
        public ParamTemplate ReturnValue
        {
            get { return _returnValue; }
        }

        /// <inheritdoc />
        public bool IsVariable
        {
            get { return _varValue; }
        }


        public ExpressionValue Evaluate(ProgramNode actual)
        {
            return _delEvaluate(actual);
        }

        /// <inheritdoc />
        public bool IsPossibleReturnType(EncogProgramContext context, EPLValueType rtn)
        {
            if (_delIsPossibleReturnType == null)
            {
                return _returnValue.PossibleTypes.Contains(rtn);
            }
            if (!_returnValue.PossibleTypes.Contains(rtn))
            {
                return false;
            }

            return _delIsPossibleReturnType(context, rtn);
        }


        /// <inheritdoc />
        public void Randomize(EncogRandom rnd, IList<EPLValueType> desiredType, ProgramNode actual, double minValue,
                              double maxValue)
        {
            if (_delRandomize != null)
            {
                _delRandomize(rnd, desiredType, actual, minValue, maxValue);
            }
        }

        /// <summary>
        ///     Read the specified parameter.
        /// </summary>
        /// <param name="parser">The parser to use.</param>
        /// <returns>The parsed parameter.</returns>
        private ParamTemplate ReadParam(SimpleParser parser)
        {
            var result = new ParamTemplate();

            if (!parser.LookAhead("{", true))
            {
                throw new EACompileError("Expected {");
            }
            parser.Advance();

            bool done = false;
            var buffer = new StringBuilder();

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

        /// <inheritdoc />
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[BasicTemplate:");
            result.Append(_signature);
            result.Append(",type=");
            result.Append(_nodeType.ToString());
            result.Append(",argCount=");
            result.Append(ChildNodeCount);
            result.Append("]");
            return result.ToString();
        }
    }
}
