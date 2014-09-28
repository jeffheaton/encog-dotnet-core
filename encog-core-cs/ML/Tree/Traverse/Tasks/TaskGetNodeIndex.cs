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
    /// Get a node by index.
    /// </summary>
    public class TaskGetNodeIndex
    {
        /// <summary>
        /// Obtain the specified tree node for the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="node">The tree node to search from.</param>
        /// <returns>The tree node for the specified index.</returns>
        public static ITreeNode process(int index, ITreeNode node)
        {
            ITreeNode result = null;
            int nodeCount = 0;

            DepthFirstTraversal trav = new DepthFirstTraversal();
            trav.Traverse(node, (n) =>
            {
                if ( nodeCount >= index)
                {
                    if (result == null)
                    {
                        result = n;
                    }
                    return false;
                }

                nodeCount++;
                return true;
            });

            return result;
        }
    }
}
