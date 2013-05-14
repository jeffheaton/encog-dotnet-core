using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Generate.Program
{
    /// <summary>
    /// A tree node that represents code to be generated.
    /// </summary>
    public class EncogTreeNode
    {
        /// <summary>
        /// The child nodes.
        /// </summary>
        private IList<EncogProgramNode> children = new List<EncogProgramNode>();

        /// <summary>
        /// The parent node.
        /// </summary>
        private EncogTreeNode parent;

        /// <summary>
        /// The program that this node belogs to.
        /// </summary>
        public EncogGenProgram Program { get; set; }

        /// <summary>
        /// Construct a tree node. 
        /// </summary>
        /// <param name="theProgram">The program.</param>
        /// <param name="theParent">The parent.</param>
        public EncogTreeNode(EncogGenProgram theProgram,
                EncogTreeNode theParent)
        {
            this.Program = theProgram;
            this.parent = theParent;
        }

        /// <summary>
        /// Add a comment. 
        /// </summary>
        /// <param name="str">The comment.</param>
        public void AddComment(string str)
        {
            EncogProgramNode node = new EncogProgramNode(this.Program, this,
                    NodeType.Comment, str);
            this.children.Add(node);
        }

        /// <summary>
        /// The children.
        /// </summary>
        public IList<EncogProgramNode> Children
        {
            get
            {
                return this.children;
            }
        }

        /// <summary>
        /// The parent.
        /// </summary>
        public EncogTreeNode Parent
        {
            get
            {
                return this.parent;
            }

        }
    }
}
