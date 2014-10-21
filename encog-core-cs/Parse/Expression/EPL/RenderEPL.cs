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

        public String Render(EncogProgram theProgram)
        {
            this.program = theProgram;
            return RenderNode(this.program.RootNode);
        }

        private String RenderNode(ProgramNode node)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                ProgramNode childNode = node.GetChildNode(i);
                result.Append(RenderNode(childNode));
            }

            result.Append('[');
            result.Append(node.Name);
            result.Append(':');
            result.Append(node.Template.ChildNodeCount);

            for (int i = 0; i < node.Template.DataSize; i++)
            {
                result.Append(':');
                EPLValueType t = node.Data[i].ExprType;
                if (t == EPLValueType.BooleanType)
                {
                    result.Append(node.Data[i].ToBooleanValue() ? 't' : 'f');
                }
                else if (t == EPLValueType.FloatingType)
                {
                    result.Append(CSVFormat.EgFormat.Format(node.Data[i].ToFloatValue(), EncogFramework.DefaultPrecision));
                }
                else if (t == EPLValueType.IntType)
                {
                    result.Append(node.Data[i].ToIntValue());
                }
                else if (t == EPLValueType.EnumType)
                {
                    result.Append(node.Data[i].EnumType);
                    result.Append("#");
                    result.Append(node.Data[i].ToIntValue());
                }
                else if (t == EPLValueType.StringType)
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
