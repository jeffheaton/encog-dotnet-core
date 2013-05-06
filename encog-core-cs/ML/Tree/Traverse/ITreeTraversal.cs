using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.Tree.Traverse
{
    /// <summary>
    /// Traverse a tree.
    /// </summary>
    public interface ITreeTraversal
    {
        //public delegate bool TreeTraversalTask(ITreeNode node);
        /// <summary>
        /// Traverse the tree.
        /// </summary>
        /// <param name="tree">The tree to traverse.</param>
        /// <param name="task">The task to execute on each tree node.</param>
        void Traverse(ITreeNode tree);
    }
}
