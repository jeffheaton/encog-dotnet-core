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
