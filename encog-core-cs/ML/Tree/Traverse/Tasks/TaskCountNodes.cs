using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.Tree.Traverse.Tasks
{
    /// <summary>
    /// Count the nodes in a tree.
    /// </summary>
    public class TaskCountNodes
    {
        /// <summary>
        /// Count the nodes from this tree node.
        /// </summary>
        /// <param name="node">The tree node.</param>
        /// <returns>The node count.</returns>
        public static int Process(ITreeNode node)
        {
            DepthFirstTraversal trav = new DepthFirstTraversal();
            return 0;

        }
    }
}
