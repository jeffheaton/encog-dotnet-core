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

namespace Encog.ML.Tree
{
    /// <summary>
    /// A node for a tree.
    /// </summary>
    public interface ITreeNode
    {        
        /// <summary>
        /// Add child nodes.
        /// </summary>
        /// <param name="args">The child nodes to add.</param>
        void AddChildNodes(ITreeNode[] args);

        /// <summary>
        /// True, if all children are leaves.
        /// </summary>
        /// <returns>True, if all children are leaves.</returns>
        bool AllLeafChildren();

        /// <summary>
        /// The child nodes.
        /// </summary>
        IList<ITreeNode> ChildNodes { get; }

        /// <summary>
        /// True, if this is a leaf.
        /// </summary>
        bool IsLeaf { get; }

        /// <summary>
        /// The number of nodes from this point. Do not call on cyclic tree.
        /// </summary>
        int Count { get; }
    }
}
