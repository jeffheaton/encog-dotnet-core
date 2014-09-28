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
using Encog.ML.Prg.ExpValue;
using Encog.ML.Prg.Ext;
using Encog.ML.EA.Exceptions;

namespace Encog.Parse.Expression.Latex
{
    /// <summary>
    /// Render as a LaTeX epression.
    /// </summary>
    public class RenderLatexExpression
    {
        private EncogProgram program;

        public RenderLatexExpression()
        {
        }

        public String Render(EncogProgram theProgram)
        {
            this.program = theProgram;
            return RenderNode(this.program.RootNode);
        }

        private String HandleConst(ProgramNode node)
        {
            ExpressionValue v = node.Data[0];
            return v.ToStringValue();
        }

        private String HandleVar(ProgramNode node)
        {
            int varIndex = (int)node.Data[0].ToIntValue();
            return this.program.Variables.GetVariableName(varIndex);
        }

        private String HandleFunction(ProgramNode node)
        {
            IProgramExtensionTemplate temp = node.Template;
            StringBuilder result = new StringBuilder();

            if (temp == StandardExtensions.EXTENSION_SQRT)
            {
                result.Append("\\sqrt{");
                result.Append(RenderNode(node.GetChildNode(0)));
                result.Append("}");
            }
            else
            {
                result.Append(temp.Name);
                result.Append('(');
                for (int i = 0; i < temp.ChildNodeCount; i++)
                {
                    if (i > 0)
                    {
                        result.Append(',');
                    }
                    result.Append(RenderNode(node.GetChildNode(i)));
                }
                result.Append(')');
            }
            return result.ToString();
        }

        private String HandleOperator(ProgramNode node)
        {
            IProgramExtensionTemplate temp = node.Template;
            StringBuilder result = new StringBuilder();

            if (temp.ChildNodeCount == 2)
            {
                String a = RenderNode(node.GetChildNode(0));
                String b = RenderNode(node.GetChildNode(1));

                if (temp == StandardExtensions.EXTENSION_DIV)
                {
                    result.Append("\\frac{");
                    result.Append(b);
                    result.Append("}{");
                    result.Append(a);
                    result.Append("}");
                }
                else if (temp == StandardExtensions.EXTENSION_MUL)
                {
                    result.Append("(");
                    result.Append(b);
                    result.Append("\\cdot ");
                    result.Append(a);
                    result.Append(")");
                }
                else
                {
                    result.Append("(");
                    result.Append(b);
                    result.Append(temp.Name);
                    result.Append(a);
                    result.Append(")");
                }

            }
            else if (temp.ChildNodeCount == 1)
            {
                String a = RenderNode(node.GetChildNode(0));
                result.Append("(");
                result.Append(temp.Name);
                result.Append(a);
                result.Append(")");
            }
            else
            {
                throw new EACompileError(
                        "An operator must have an arity of 1 or 2, probably should be made a function.");
            }

            return result.ToString();
        }

        public ExpressionNodeType DetermineNodeType(ProgramNode node)
        {
            if (node.Template == StandardExtensions.EXTENSION_CONST_SUPPORT)
            {
                return ExpressionNodeType.ConstVal;
            }
            else if (node.Template == StandardExtensions.EXTENSION_VAR_SUPPORT)
            {
                return ExpressionNodeType.Variable;
            }
            else if (node.Template.NodeType == NodeType.OperatorLeft
                  || node.Template.NodeType == NodeType.OperatorRight)
            {
                return ExpressionNodeType.Operator;
            }
            else
            {
                return ExpressionNodeType.Function;
            }
        }

        private String RenderNode(ProgramNode node)
        {
            switch (DetermineNodeType(node))
            {
                case ExpressionNodeType.ConstVal:
                    return HandleConst(node);
                case ExpressionNodeType.Operator:
                    return HandleOperator(node);
                case ExpressionNodeType.Variable:
                    return HandleVar(node);
                case ExpressionNodeType.Function:
                    return HandleFunction(node);
            }
            throw new EACompileError("Uknown node type: " + node.ToString());
        }
    }
}
