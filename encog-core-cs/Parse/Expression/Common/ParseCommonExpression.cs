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
using System.Linq;
using System.Text;
using Encog.ML.Prg;
using Encog.Util;
using Encog.ML.Prg.Ext;
using Encog.ML.EA.Exceptions;
using Encog.ML.Prg.ExpValue;

namespace Encog.Parse.Expression.Common
{
    /// <summary>
    /// This class is used to process a common format equation (in-fix) into the tree
    /// format that Encog uses. To do this I make use of the shunting yard algorithm.
    /// 
    /// One important note on definitions. I consider an operator to be simply a
    /// special type of function. Really, an operator is just a shorthand for writing
    /// certain types of functions. Therefore I do not distinguish between functions
    /// and operators in this implementation.
    /// 
    /// References:
    /// 
    /// http://en.wikipedia.org/wiki/Shunting-yard_algorithm
    /// </summary>
    public class ParseCommonExpression
    {
        private EncogProgram holder;
        private SimpleParser parser;
        private ProgramNode rootNode;
        private bool unary;
        private IProgramExtensionTemplate LEFT_PAREN = new BasicTemplate(
                BasicTemplate.NoPrec, "(", NodeType.None, false, 0, null, null, null);

        private Stack<IProgramExtensionTemplate> functionStack = new Stack<IProgramExtensionTemplate>(
                100);
        private Stack<ProgramNode> outputStack = new Stack<ProgramNode>(
                100);

        public ParseCommonExpression(EncogProgram theHolder)
        {
            this.holder = theHolder;
        }
        
        /// <summary>
        /// Push a leaf onto the output stack.
        /// </summary>
        /// <param name="leaf">The leaf to push onto the output stack.</param>
        private void OutputQueue(ProgramNode leaf)
        {
            outputStack.Push(leaf);
        }

        private void OutputQueue(IProgramExtensionTemplate opp)
        {
            if (opp == this.LEFT_PAREN)
            {
                throw new EACompileError("Unmatched parentheses");
            }

            ProgramNode[] args = new ProgramNode[opp.ChildNodeCount];

            for (int i = args.Length - 1; i >= 0; i--)
            {
                if (this.outputStack.Count == 0)
                {
                    throw new EACompileError("Not enough arguments");
                }
                args[i] = this.outputStack.Pop();
            }

            this.rootNode = this.holder.Functions.FactorProgramNode(opp,
                    this.holder, args);
            outputStack.Push(rootNode);
        }

        private void functionQueue(IProgramExtensionTemplate f)
        {

            // while there is an operator token, o2, at the top of the stack, and
            // either o1 is left-associative and o1 has precedence less than or
            // equal to that of o2,
            // or o1 has precedence less than that of o2,

            while (this.functionStack.Count != 0
                    && this.functionStack.Peek().NodeType != NodeType.None
                    && ((f.NodeType == NodeType.OperatorLeft && f
                            .Precedence >= this.functionStack.Peek()
                            .Precedence) || f.Precedence > this.functionStack
                            .Peek().Precedence))
            {
                OutputQueue(this.functionStack.Pop());
            }

            functionStack.Push(f);
        }

        private void HandleNumeric()
        {
            double value, exponent;
            char sign = '+';
            bool isFloat = false;
            bool neg = false;

            value = 0.0;
            exponent = 0;

            // should we just make this negative, due to an unary minus?
            if (this.functionStack.Count > 0
                    && this.functionStack.Peek() == StandardExtensions.EXTENSION_NEG)
            {
                this.functionStack.Pop();
                neg = true;
            }

            // whole number part

            while (char.IsDigit(this.parser.Peek()))
            {
                value = (10.0 * value) + (this.parser.ReadChar() - '0');
            }

            // Optional fractional
            if (this.parser.Peek() == '.')
            {
                isFloat = true;
                this.parser.Advance();

                int i = 1;
                while (char.IsDigit(this.parser.Peek()))
                {
                    double f = (this.parser.ReadChar() - '0');
                    f /= Math.Pow(10.0, i);
                    value += f;
                    i++;
                }
            }

            // Optional exponent

            if (char.ToUpper(this.parser.Peek()) == 'E')
            {
                this.parser.Advance();

                if ((this.parser.Peek() == '+') || (this.parser.Peek() == '-'))
                {
                    sign = this.parser.ReadChar();
                }

                while (char.IsDigit(this.parser.Peek()))
                {
                    exponent = (int)(10.0 * exponent)
                            + (this.parser.ReadChar() - '0');
                }

                if (sign == '-')
                {
                    isFloat = true;
                    exponent = -exponent;
                }

                value = value * Math.Pow(10, exponent);
            }

            if (neg)
            {
                value = -value;
            }

            ProgramNode v = this.holder.Functions.FactorProgramNode("#const",
                    holder, new ProgramNode[] { });

            if (isFloat)
            {
                v.Data[0] = new ExpressionValue(value);
            }
            else
            {
                v.Data[0] = new ExpressionValue((int)value);
            }

            OutputQueue(v);
        }

        private void handleAlpha(bool neg)
        {
            StringBuilder varName = new StringBuilder();
            while (char.IsLetterOrDigit(this.parser.Peek()))
            {
                varName.Append(this.parser.ReadChar());
            }

            this.parser.EatWhiteSpace();

            if (varName.ToString().Equals("true"))
            {
                if (neg)
                {
                    throw new EACompileError("Invalid negative sign.");
                }
                ProgramNode v = this.holder.Functions.FactorProgramNode("#const",
                        holder, new ProgramNode[] { });
                v.Data[0] = new ExpressionValue(true);
                OutputQueue(v);
            }
            else if (varName.ToString().Equals("false"))
            {
                if (neg)
                {
                    throw new EACompileError("Invalid negative sign.");
                }
                ProgramNode v = this.holder.Functions.FactorProgramNode("#const",
                        holder, new ProgramNode[] { });
                v.Data[0] = new ExpressionValue(false);
                OutputQueue(v);
            }
            else if (this.parser.Peek() != '(')
            {
                ProgramNode v;
                // either a variable or a const, see which
                if (this.holder.Functions.IsDefined(varName.ToString(), 0))
                {
                    v = this.holder.Functions.FactorProgramNode(
                            varName.ToString(), holder, new ProgramNode[] { });
                }
                else
                {
                    this.holder.Variables.SetVariable(varName.ToString(),
                            new ExpressionValue((long)0));
                    v = this.holder.Functions.FactorProgramNode("#var", holder,
                            new ProgramNode[] { });
                    v.Data[0] = new ExpressionValue((int)this.holder.Variables
                            .GetVariableIndex(varName.ToString()));
                }

                if (neg)
                {
                    v = this.holder.Functions.FactorProgramNode("-", holder,
                            new ProgramNode[] { v });
                }
                OutputQueue(v);
            }
            else
            {
                IProgramExtensionTemplate temp = this.holder.Functions
                        .FindFunction(varName.ToString());
                if (temp == null)
                {
                    throw new EACompileError("Undefined function: "
                            + varName.ToString());
                }
                functionQueue(temp);
            }
        }

        private void handleSymbol()
        {
            char ch1 = this.parser.ReadChar();

            // handle unary
            if (this.unary)
            {
                if (ch1 == '+')
                {
                    return;
                }
                else if (ch1 == '-')
                {
                    this.functionStack.Push(StandardExtensions.EXTENSION_NEG);
                    return;
                }
            }

            // handle regular operator
            char ch2 = (char)0;
            if (!this.parser.EOL())
            {
                ch2 = this.parser.Peek();
            }

            IProgramExtensionTemplate temp = this.holder.Functions.FindOperator(ch1, ch2);

            // did we find anything?
            if (temp != null)
            {
                // if we found a 2-char operator, then advance beyond the 2nd
                // char
                if (temp.Name.Length > 1)
                {
                    this.parser.Advance();
                }
                functionQueue(temp);
            }
            else
            {
                throw new EACompileError("Unknown symbol: " + ch1);
            }

        }

        private void handleString()
        {
            StringBuilder str = new StringBuilder();

            char ch;

            if (this.parser.Peek() == '\"')
            {
                this.parser.Advance();
            }
            do
            {
                ch = this.parser.ReadChar();
                if (ch == 34)
                {
                    // handle double quote
                    if (this.parser.Peek() == 34)
                    {
                        this.parser.Advance();
                        str.Append(ch);
                        ch = this.parser.ReadChar();
                    }
                }
                else
                {
                    str.Append(ch);
                }
            } while ((ch != 34) && (ch > 0));

            if (ch != 34)
            {
                throw (new EACompileError("Unterminated string"));
            }

            ProgramNode v = this.holder.Functions.FactorProgramNode("#const",
                    holder, new ProgramNode[] { });
            v.Data[0] = new ExpressionValue(str.ToString());
            OutputQueue(v);
        }

        private void handleRightParen()
        {
            // move past the paren
            this.parser.Advance();

            // Until the token at the top of the stack is a left parenthesis, pop
            // operators off the stack onto the output queue.

            while (this.functionStack.Peek() != this.LEFT_PAREN)
            {
                OutputQueue(this.functionStack.Pop());
            }

            // Pop the left parenthesis from the stack, but not onto the output
            // queue.
            this.functionStack.Pop();

            // If the token at the top of the stack is a function token, pop it onto
            // the output queue.
            if (this.functionStack.Count > 0
                    && this.functionStack.Peek().NodeType == NodeType.Function)
            {
                OutputQueue(this.functionStack.Pop());
            }

            // If the stack runs out without finding a left parenthesis, then there
            // are mismatched parentheses.
        }

        private void HandleFunctionSeparator()
        {
            // advance past
            this.parser.Advance();

            // Until the token at the top of the stack is a left parenthesis,
            // pop operators off the stack onto the output queue.
            while (this.functionStack.Peek() != this.LEFT_PAREN)
            {
                OutputQueue(this.functionStack.Pop());
            }

            // If no left parentheses are encountered, either the separator was
            // misplaced or parentheses were mismatched.

        }

        public ProgramNode Parse(String expression)
        {
            this.parser = new SimpleParser(expression);
            this.unary = true;

            while (!parser.EOL())
            {
                parser.EatWhiteSpace();
                char ch = parser.Peek();
                if (ch == '.' || char.IsDigit(ch))
                {
                    HandleNumeric();
                    this.unary = false;
                }
                else if (char.IsLetter(ch))
                {
                    handleAlpha(false);
                    this.unary = false;
                }
                else if (ch == '(')
                {
                    this.parser.Advance();
                    this.functionStack.Push(LEFT_PAREN);
                    this.unary = true;
                }
                else if (ch == ')')
                {
                    handleRightParen();
                    this.unary = false;
                }
                else if ("<>^*/+-=&|".IndexOf(ch) != -1)
                {
                    handleSymbol();
                    this.unary = true;
                }
                else if (ch == '\"')
                {
                    handleString();
                    this.unary = false;
                }
                else if (ch == ',')
                {
                    HandleFunctionSeparator();
                    this.unary = true;
                }
                else
                {
                    throw new EACompileError("Unparsable character: " + ch);
                }
            }

            // pop off any functions still on the stack
            while (this.functionStack.Count > 0)
            {
                IProgramExtensionTemplate f = this.functionStack.Pop();
                OutputQueue(f);
            }

            // were there no operators?
            if (this.rootNode == null)
            {
                this.rootNode = this.outputStack.Pop();
            }

            return this.rootNode;
        }
    }
}
