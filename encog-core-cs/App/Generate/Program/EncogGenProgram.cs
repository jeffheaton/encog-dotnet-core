using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Generate.Program
{
    /// <summary>
    /// Holds a generated Encog program. A language specific generator will take this
    /// and generate actual source code from it.
    /// </summary>
    public class EncogGenProgram : EncogTreeNode
    {
        /// <summary>
        /// Construct the program.
        /// </summary>
        public EncogGenProgram()
            : base(null, null)
        {
            this.Program = this;
        }

        /// <summary>
        /// Create a new class. 
        /// </summary>
        /// <param name="className">The class name.</param>
        /// <returns>The newly created class.</returns>
        public EncogProgramNode CreateClass(String className)
        {
            EncogProgramNode node = new EncogProgramNode(this, this,
                    NodeType.Class, className);
            Children.Add(node);
            return node;
        }
    }
}
