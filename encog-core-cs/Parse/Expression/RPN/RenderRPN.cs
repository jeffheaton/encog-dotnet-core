using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Prg;

namespace Encog.Parse.Expression.RPN
{
    /// <summary>
    /// Render to Reverse Polish Notation (RPN).
    /// </summary>
    public class RenderRPN : CommonRender
    {
        private EncogProgram program;

        public RenderRPN()
        {
        }

        public String Render(EncogProgram theProgram)
        {
            this.program = theProgram;
            return RenderNode(this.program.RootNode);
        }

        private String HandleConst(ProgramNode node)
        {
            return node.Data[0].ToStringValue();
        }

        private String HandleVar(ProgramNode node)
        {
            int varIndex = (int)node.Data[0].ToIntValue();
            return this.program.Variables.GetVariableName(varIndex);
        }



        private String RenderNode(ProgramNode node)
        {
            StringBuilder result = new StringBuilder();

            ExpressionNodeType t = this.DetermineNodeType(node);

            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                ProgramNode childNode = node.GetChildNode(i);
                if (result.Length > 0)
                {
                    result.Append(" ");
                }
                result.Append(RenderNode(childNode));
            }

            if (result.Length > 0)
            {
                result.Append(" ");
            }

            if (t == ExpressionNodeType.ConstVal)
            {
                result.Append(HandleConst(node));
            }
            else if (t == ExpressionNodeType.Variable)
            {
                result.Append(HandleVar(node));
            }
            else if (t == ExpressionNodeType.Function || t == ExpressionNodeType.Operator)
            {
                result.Append('[');
                result.Append(node.Name);

                result.Append(']');
            }

            return result.ToString().Trim();
        }
    }
}
