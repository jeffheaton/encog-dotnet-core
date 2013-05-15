using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Analyst.Script.Process
{
    /// <summary>
    /// Script holder for Encog Analyst preprocessing.
    /// </summary>
    public class AnalystProcess
    {
        /// <summary>
        /// The fields.
        /// </summary>
        private IList<ProcessField> fields = new List<ProcessField>();

        /// <summary>
        /// The fields.
        /// </summary>
        public IList<ProcessField> Fields
        {
            get
            {
                return fields;
            }
        }
    }
}
