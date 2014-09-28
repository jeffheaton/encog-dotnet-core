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

namespace Encog.ML.Tree.Traverse.Tasks
{
    /// <summary>
    /// Replace one node with another.
    /// </summary>
    public class TaskReplaceNode
    {
        /// <summary>
        /// Replace one node with another.
        /// </summary>
        /// <param name="rootNode">The root node.</param>
        /// <param name="replaceThisNode">The node to replace.</param>
        /// <param name="replaceWith">What to replace with.</param>
        public static void Process(ITreeNode rootNode, ITreeNode replaceThisNode, ITreeNode replaceWith)
        {
            bool done = false;

            DepthFirstTraversal trav = new DepthFirstTraversal();
            trav.Traverse(rootNode, (n) =>
            {
                if (done)
                {
                    return false;
                }

                for (int i = 0; i < n.ChildNodes.Count; i++)
                {
                    ITreeNode childNode = n.ChildNodes[i];
                    if (childNode == replaceThisNode)
                    {
                        n.ChildNodes[i] = replaceWith;
                        done = true;
                        return false;
                    }
                }
                return true;
            });
        }
    }
}
