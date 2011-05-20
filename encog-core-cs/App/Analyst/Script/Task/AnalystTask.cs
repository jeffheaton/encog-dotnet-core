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
        private readonly IList<String> _lines;

        /// <summary>
        /// The name of the task.
        /// </summary>
        ///
        private String _name;

        /// <summary>
        /// Construct an analyst task.
        /// </summary>
        ///
        /// <param name="theName">The name of this task.</param>
        public AnalystTask(String theName)
        {
            _lines = new List<String>();
            _name = theName;
        }


        /// <value>the lines</value>
        public IList<String> Lines
        {            
            get { return _lines; }
        }


        /// <value>the name to set</value>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
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
            result.Append(_name);
            result.Append("]");
            return result.ToString();
        }
    }
}