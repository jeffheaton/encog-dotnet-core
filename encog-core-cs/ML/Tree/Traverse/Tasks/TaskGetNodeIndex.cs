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
