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
            int result = 0;

            DepthFirstTraversal trav = new DepthFirstTraversal();
            trav.Traverse( node, (n) =>
            {
                result++;
                return true;
            });

            return result;

        }
    }
}
