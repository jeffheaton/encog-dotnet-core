using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Analyst.Script.Process
{
    /// <summary>
    /// Holds one field for Encog analyst preprocess.
    /// </summary>
    public class ProcessField
    {
        /// <summary>
        /// The name of this field.
        /// </summary>
        private readonly String name;

        /// <summary>
        /// The command for this field.
        /// </summary>
        private readonly String command;

        /// <summary>
        /// Construct this field.
        /// </summary>
        /// <param name="name">The name of this field.</param>
        /// <param name="command">The command for this field.</param>
        public ProcessField(String name, String command)
        {
            this.name = name;
            this.command = command;
        }

        /// <summary>
        /// The name of this field.
        /// </summary>
        public String Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// The command for this field.
        /// </summary>
        public String Command
        {
            get
            {
                return command;
            }
        }

    }
}
