using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Prg.Ext;

namespace Encog.App.Analyst.Script.ML
{
    /// <summary>
    /// An opcode, stored in the script.
    /// </summary>
    public class ScriptOpcode
    {
        /// <summary>
        /// The name of the opcode.
        /// </summary>
        private String name;

        /// <summary>
        /// The argument count of the opcode.
        /// </summary>
        private int argCount;

        /// <summary>
        /// Construct the opcode.
        /// </summary>
        /// <param name="name">The name of the opcode.</param>
        /// <param name="argCount">The argument count.</param>
        public ScriptOpcode(String name, int argCount)
        {
            this.name = name;
            this.argCount = argCount;
        }

        /// <summary>
        /// Construct using an extension template.
        /// </summary>
        /// <param name="temp">The template.</param>
        public ScriptOpcode(IProgramExtensionTemplate temp)
            : this(temp.Name, temp.ChildNodeCount)
        {
            
        }

        /// <summary>
        /// The name.
        /// </summary>
        public String Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// The argument count.
        /// </summary>
        public int ArgCount
        {
            get
            {
                return argCount;
            }
        }
    }
}
