using System;
using System.Collections.Generic;
using System.Text;

namespace Encog.App.Analyst.Script.Task
{
    /// <summary>
    /// Holds a task in the script. A task is a named set of commands.
    /// </summary>
    ///
    public class AnalystTask
    {
        /// <summary>
        /// The "source code" for this task.
        /// </summary>
        ///
        private readonly IList<String> lines;

        /// <summary>
        /// The name of the task.
        /// </summary>
        ///
        private String name;

        /// <summary>
        /// Construct an analyst task.
        /// </summary>
        ///
        /// <param name="theName">The name of this task.</param>
        public AnalystTask(String theName)
        {
            lines = new List<String>();
            name = theName;
        }


        /// <value>the lines</value>
        public IList<String> Lines
        {            
            get { return lines; }
        }


        /// <value>the name to set</value>
        public String Name
        {
            get { return name; }
            set { name = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" name=");
            result.Append(name);
            result.Append("]");
            return result.ToString();
        }
    }
}