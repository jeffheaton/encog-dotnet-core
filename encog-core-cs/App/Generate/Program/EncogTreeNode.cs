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
using System.Collections.Generic;

namespace Encog.App.Generate.Program
{
    /// <summary>
    ///     A tree node that represents code to be generated.
    /// </summary>
    public class EncogTreeNode
    {
        /// <summary>
        ///     The child nodes.
        /// </summary>
        private readonly IList<EncogProgramNode> children = new List<EncogProgramNode>();

        /// <summary>
        ///     The parent node.
        /// </summary>
        private readonly EncogTreeNode parent;

        /// <summary>
        ///     Construct a tree node.
        /// </summary>
        /// <param name="theProgram">The program.</param>
        /// <param name="theParent">The parent.</param>
        public EncogTreeNode(EncogGenProgram theProgram,
                             EncogTreeNode theParent)
        {
            Program = theProgram;
            parent = theParent;
        }

        /// <summary>
        ///     The program that this node belogs to.
        /// </summary>
        public EncogGenProgram Program { get; set; }

        /// <summary>
        ///     The children.
        /// </summary>
        public IList<EncogProgramNode> Children
        {
            get { return children; }
        }

        /// <summary>
        ///     The parent.
        /// </summary>
        public EncogTreeNode Parent
        {
            get { return parent; }
        }

        /// <summary>
        ///     Add a comment.
        /// </summary>
        /// <param name="str">The comment.</param>
        public void AddComment(string str)
        {
            var node = new EncogProgramNode(Program, this,
                                            NodeType.Comment, str);
            children.Add(node);
        }
    }
}
