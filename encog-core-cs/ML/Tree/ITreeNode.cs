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
