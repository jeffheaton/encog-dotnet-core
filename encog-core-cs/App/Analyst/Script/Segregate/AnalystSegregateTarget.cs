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
        private String file;

        /// <summary>
        /// Construct the segregation target.
        /// </summary>
        ///
        /// <param name="theFile">The file.</param>
        /// <param name="thePercent">The percent.</param>
        public AnalystSegregateTarget(String theFile, int thePercent)
        {
            file = theFile;
            Percent = thePercent;
        }


        /// <value>the file to set</value>
        public String File
        {
            get { return file; }         
            set { file = value; }
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
            result.Append(file);
            result.Append(", percent=");
            result.Append(file);
            result.Append("]");
            return result.ToString();
        }
    }
}