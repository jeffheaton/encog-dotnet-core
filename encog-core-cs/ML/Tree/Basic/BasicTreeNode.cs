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
using Encog.ML.Tree.Traverse.Tasks;

namespace Encog.ML.Tree.Basic
{
    /// <summary>
    /// A basic tree.
    /// </summary>
    [Serializable]
    public class BasicTreeNode : ITreeNode
    {
        /// <summary>
        /// The child nodes.
        /// </summary>
        private IList<ITreeNode> childNodes = new List<ITreeNode>();

        /// <inheritdoc/>
        public IList<ITreeNode> ChildNodes
        {
            get
            {
                return this.childNodes;
            }
        }

        /// <inheritdoc/>
        public void AddChildNodes(ITreeNode[] args)
        {
            foreach (ITreeNode pn in args)
            {
                this.childNodes.Add(pn);
            }
        }

        /// <inheritdoc/>
        public bool AllLeafChildren()
        {
            bool result = true;

            foreach (ITreeNode node in this.childNodes)
            {
                if (!node.IsLeaf)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public bool IsLeaf
        {
            get
            {
                return this.childNodes.Count == 0;
            }
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                return TaskCountNodes.Process(this);
            }
        }

    }
}
