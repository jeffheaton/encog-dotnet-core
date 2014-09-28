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

namespace Encog.Parse.Expression.Common
{
    /// <summary>
    /// Render a common expression.
    /// </summary>
    public class RenderCommonExpression : CommonRender
    {
        private EncogProgram holder;

        public RenderCommonExpression()
        {
        }

        public String Render(EncogProgram theHolder)
        {
            this.holder = theHolder;
            ProgramNode node = theHolder.RootNode;
            return RenderNode(node);
        }

        private String RenderConst(ProgramNode node)
        {
            return node.Data[0].ToStringValue();
        }

        private String RenderVar(ProgramNode node)
        {
            int varIndex = (int)node.Data[0].ToIntValue();
            return this.holder.Variables.GetVariableName(varIndex);
        }

        private String RenderFunction(ProgramNode node)
        {
            StringBuilder result = new StringBuilder();
            result.Append(node.Name);
            result.Append('(');
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                if (i > 0)
                {
                    result.Append(',');
                }
                ProgramNode childNode = node.GetChildNode(i);
                result.Append(RenderNode(childNode));
            }
            result.Append(')');
            return result.ToString();
        }

        private String RenderOperator(ProgramNode node)
        {
            StringBuilder result = new StringBuilder();
            result.Append("(");
            result.Append(RenderNode(node.GetChildNode(0)));
            result.Append(node.Name);
            result.Append(RenderNode(node.GetChildNode(1)));
            result.Append(")");
            return result.ToString();
        }

        private String RenderNode(ProgramNode node)
        {
            StringBuilder result = new StringBuilder();

            switch (DetermineNodeType(node))
            {
                case ExpressionNodeType.ConstVal:
                    result.Append(RenderConst(node));
                    break;
                case ExpressionNodeType.Operator:
                    result.Append(RenderOperator(node));
                    break;
                case ExpressionNodeType.Variable:
                    result.Append(RenderVar(node));
                    break;
                case ExpressionNodeType.Function:
                    result.Append(RenderFunction(node));
                    break;
            }

            return result.ToString();
        }
    }
}
