using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Prg;
using Encog.ML.Prg.ExpValue;
using Encog.Util.CSV;

namespace Encog.Parse.Expression.EPL
{
    /// <summary>
    /// Render in EPL.
    /// </summary>
    public class RenderEPL : CommonRender
    {
        private EncogProgram program;

        public RenderEPL()
        {
        }

        public String render(EncogProgram theProgram)
        {
            this.program = theProgram;
            return renderNode(this.program.RootNode);
        }

        private String renderNode(ProgramNode node)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                ProgramNode childNode = node.GetChildNode(i);
                result.Append(renderNode(childNode));
            }

            result.Append('[');
            result.Append(node.Name);
            result.Append(':');
            result.Append(node.Template.ChildNodeCount);

            for (int i = 0; i < node.Template.DataSize; i++)
            {
                result.Append(':');
                EPLValueType t = node.Data[i].ExprType;
                if (t == EPLValueType.booleanType)
                {
                    result.Append(node.Data[i].ToBooleanValue() ? 't' : 'f');
                }
                else if (t == EPLValueType.floatingType)
                {
                    result.Append(CSVFormat.EgFormat.Format(node.Data[i].ToFloatValue(), EncogFramework.DefaultPrecision));
                }
                else if (t == EPLValueType.intType)
                {
                    result.Append(node.Data[i].ToIntValue());
                }
                else if (t == EPLValueType.enumType)
                {
                    result.Append(node.Data[i].EnumType);
                    result.Append("#");
                    result.Append(node.Data[i].ToIntValue());
                }
                else if (t == EPLValueType.stringType)
                {
                    result.Append("\"");
                    result.Append(node.Data[i].ToStringValue());
                    result.Append("\"");
                }
            }
            result.Append(']');

            return result.ToString().Trim();
        }
    }
}
