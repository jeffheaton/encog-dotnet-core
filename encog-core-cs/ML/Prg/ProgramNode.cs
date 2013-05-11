using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Prg.ExpValue;
using Encog.ML.Tree.Basic;
using Encog.ML.Tree;
using Encog.ML.Prg.Ext;

namespace Encog.ML.Prg
{
    /// <summary>
    /// Represents a program node in an EPL program.
    /// </summary>
    [Serializable]
    public class ProgramNode : BasicTreeNode
    {
        /// <summary>
        /// The opcode that this node implements.
        /// </summary>
        private IProgramExtensionTemplate template;

        /// <summary>
        /// The Encog program that this node belongs to.
        /// </summary>
        private EncogProgram owner;

        /// <summary>
        /// Any data associated with this node. For example, const nodes will store
        /// their value here.
        /// </summary>
        private ExpressionValue[] data;

        /// <summary>
        /// Construct the program node.
        /// </summary>
        /// <param name="theOwner">The owner of the node.</param>
        /// <param name="theTemplate">The opcode that this node is based on.</param>
        /// <param name="theArgs">The child nodes to this node.</param>
        public ProgramNode(EncogProgram theOwner,
                IProgramExtensionTemplate theTemplate,
                ProgramNode[] theArgs)
        {
            this.owner = theOwner;
            this.data = new ExpressionValue[theTemplate.DataSize];
            this.template = theTemplate;
            AddChildNodes(theArgs);

            for (int i = 0; i < this.data.Length; i++)
            {
                this.data[i] = new ExpressionValue((long)0);
            }
        }

        /// <summary>
        /// True if all children are constant.
        /// </summary>
        /// <returns>True if all children are constant.</returns>
        public bool AllConstChildren()
        {
            bool result = true;

            foreach (ITreeNode tn in ChildNodes)
            {
                ProgramNode node = (ProgramNode)tn;
                if (node.IsVariable)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Determine if all descendants are constant.
        /// </summary>
        /// <returns>True if all descendants are constant.</returns>
        public bool AllConstDescendants()
        {
            if (IsVariable)
            {
                return false;
            }

            if (IsLeaf)
            {
                return true;
            }

            foreach (ITreeNode tn in ChildNodes)
            {
                ProgramNode childNode = (ProgramNode)tn;
                if (!childNode.AllConstDescendants())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// The evaluated value of this node.
        /// </summary>
        /// <returns>The evaluated value of this node.</returns>
        public ExpressionValue Evaluate()
        {
            return this.template.Evaluate(this);
        }

        /// <summary>
        /// Get the specified child node. 
        /// </summary>
        /// <param name="index">The index of this node.</param>
        /// <returns>The child node requested.</returns>
        public ProgramNode GetChildNode(int index)
        {
            return (ProgramNode)ChildNodes[index];
        }

        /// <summary>
        /// The node data.
        /// </summary>
        public ExpressionValue[] Data
        {
            get
            {
                return this.data;
            }
        }

        /// <summary>
        /// The name of this node (from the opcode template).
        /// </summary>
        public String Name
        {
            get
            {
                return this.template.Name;
            }
        }

        /// <summary>
        /// The EncogProgram that owns this node.
        /// </summary>
        public EncogProgram Owner
        {
            get
            {
                return this.owner;
            }
        }

        /// <summary>
        /// The template, or opcode.
        /// </summary>
        public IProgramExtensionTemplate Template
        {
            get
            {
                return this.template;
            }
        }

        /// <summary>
        /// Returns true if this node's value is variable.
        /// </summary>
        public bool IsVariable
        {
            get
            {
                return this.template.IsVariable;
            }
        }

        /// <summary>
        /// The string form of this node.
        /// </summary>
        /// <returns>A string form of this node.</returns>
        public String ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append("[ProgramNode: name=");
            result.Append(this.template.Name);
            result.Append(", childCount=");
            result.Append(ChildNodes.Count);
            result.Append(", childNodes=");
            foreach (ITreeNode tn in ChildNodes)
            {
                ProgramNode node = (ProgramNode)tn;
                result.Append(" ");
                result.Append(node.Template.Name);
            }
            result.Append("]");
            return result.ToString();
        }
    }
}
