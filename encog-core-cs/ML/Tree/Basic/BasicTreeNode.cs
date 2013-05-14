using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Tree.Traverse.Tasks;

namespace Encog.ML.Tree.Basic
{
    /// <summary>
    /// A basic tree.
    /// </summary>
    [Serializable]
    public class BasicTreeNode : ITreeNode
    {
        /// <summary>
        /// The child nodes.
        /// </summary>
        private IList<ITreeNode> childNodes = new List<ITreeNode>();

        /// <inheritdoc/>
        public IList<ITreeNode> ChildNodes
        {
            get
            {
                return this.childNodes;
            }
        }

        /// <inheritdoc/>
        public void AddChildNodes(ITreeNode[] args)
        {
            foreach (ITreeNode pn in args)
            {
                this.childNodes.Add(pn);
            }
        }

        /// <inheritdoc/>
        public bool AllLeafChildren()
        {
            bool result = true;

            foreach (ITreeNode node in this.childNodes)
            {
                if (!node.IsLeaf)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public bool IsLeaf
        {
            get
            {
                return this.childNodes.Count == 0;
            }
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                return TaskCountNodes.Process(this);
            }
        }

    }
}
