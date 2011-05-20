using System;
using System.Text;

namespace Encog.App.Analyst.Script.Segregate
{
    /// <summary>
    /// This class specifies a target for the segregation process.
    /// </summary>
    ///
    public class AnalystSegregateTarget
    {
        /// <summary>
        /// The file target.
        /// </summary>
        ///
        private String _file;

        /// <summary>
        /// Construct the segregation target.
        /// </summary>
        ///
        /// <param name="theFile">The file.</param>
        /// <param name="thePercent">The percent.</param>
        public AnalystSegregateTarget(String theFile, int thePercent)
        {
            _file = theFile;
            Percent = thePercent;
        }


        /// <value>the file to set</value>
        public String File
        {
            get { return _file; }         
            set { _file = value; }
        }


        /// <value>the percent to set</value>
        public int Percent { 
            get; 
            set; }


        /// <imheritdoc/>
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" file=");
            result.Append(_file);
            result.Append(", percent=");
            result.Append(_file);
            result.Append("]");
            return result.ToString();
        }
    }
}