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
using System.Linq;
using System.Text;
using Encog.ML.Prg.ExpValue;
using Encog.ML.Prg.Ext;
using Encog.ML.Tree;
using Encog.ML.Tree.Basic;

namespace Encog.ML.Prg
{
    /// <summary>
    ///     Represents a program node in an EPL program.
    /// </summary>
    [Serializable]
    public class ProgramNode : BasicTreeNode
    {
        /// <summary>
        ///     Any data associated with this node. For example, const nodes will store
        ///     their value here.
        /// </summary>
        private readonly ExpressionValue[] _data;

        /// <summary>
        ///     The Encog program that this node belongs to.
        /// </summary>
        private readonly EncogProgram _owner;

        /// <summary>
        ///     The opcode that this node implements.
        /// </summary>
        private readonly IProgramExtensionTemplate _template;

        /// <summary>
        ///     Construct the program node.
        /// </summary>
        /// <param name="theOwner">The owner of the node.</param>
        /// <param name="theTemplate">The opcode that this node is based on.</param>
        /// <param name="theArgs">The child nodes to this node.</param>
        public ProgramNode(EncogProgram theOwner,
                           IProgramExtensionTemplate theTemplate,
                           ITreeNode[] theArgs)
        {
            _owner = theOwner;
            _data = new ExpressionValue[theTemplate.DataSize];
            _template = theTemplate;
            AddChildNodes(theArgs);

            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = new ExpressionValue((long) 0);
            }
        }

        /// <summary>
        ///     The node data.
        /// </summary>
        public ExpressionValue[] Data
        {
            get { return _data; }
        }

        /// <summary>
        ///     The name of this node (from the opcode template).
        /// </summary>
        public String Name
        {
            get { return _template.Name; }
        }

        /// <summary>
        ///     The EncogProgram that owns this node.
        /// </summary>
        public EncogProgram Owner
        {
            get { return _owner; }
        }

        /// <summary>
        ///     The template, or opcode.
        /// </summary>
        public IProgramExtensionTemplate Template
        {
            get { return _template; }
        }

        /// <summary>
        ///     Returns true if this node's value is variable.
        /// </summary>
        public bool IsVariable
        {
            get { return _template.IsVariable; }
        }

        /// <summary>
        ///     True if all children are constant.
        /// </summary>
        /// <returns>True if all children are constant.</returns>
        public bool AllConstChildren()
        {
            return ChildNodes.Cast<ProgramNode>().All(node => !node.IsVariable);
        }

        /// <summary>
        ///     Determine if all descendants are constant.
        /// </summary>
        /// <returns>True if all descendants are constant.</returns>
        public bool AllConstDescendants()
        {
            if (IsVariable)
            {
                return false;
            }

            if (IsLeaf)
            {
                return true;
            }

            return ChildNodes.Cast<ProgramNode>().All(childNode => childNode.AllConstDescendants());
        }

        /// <summary>
        ///     The evaluated value of this node.
        /// </summary>
        /// <returns>The evaluated value of this node.</returns>
        public ExpressionValue Evaluate()
        {
            return _template.Evaluate(this);
        }

        /// <summary>
        ///     Get the specified child node.
        /// </summary>
        /// <param name="index">The index of this node.</param>
        /// <returns>The child node requested.</returns>
        public ProgramNode GetChildNode(int index)
        {
            return (ProgramNode) ChildNodes[index];
        }

        /// <summary>
        ///     The string form of this node.
        /// </summary>
        /// <returns>A string form of this node.</returns>
        public override String ToString()
        {
            var result = new StringBuilder();
            result.Append("[ProgramNode: name=");
            result.Append(_template.Name);
            result.Append(", childCount=");
            result.Append(ChildNodes.Count);
            result.Append(", childNodes=");
            foreach (ITreeNode tn in ChildNodes)
            {
                var node = (ProgramNode) tn;
                result.Append(" ");
                result.Append(node.Template.Name);
            }
            result.Append("]");
            return result.ToString();
        }
    }
}
