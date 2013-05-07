using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.Tree.Traverse
{
    /// <summary>
    /// A depth first tree traversal.
    /// </summary>
    public class DepthFirstTraversal : ITreeTraversal
    {
        /// <inheritdoc/>
        public void Traverse(ITreeNode treeNode, MLDelegates.TreeTraversalTask task)
        {
            if (!task(treeNode))
                return;

            foreach (ITreeNode childNode in treeNode.ChildNodes)
            {
                Traverse(childNode, task);
            }
        }
    }
}
