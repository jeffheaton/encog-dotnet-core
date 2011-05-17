using System;
using System.IO;
using System.Text;

namespace Encog.App.Analyst.CSV.Segregate
{
    /// <summary>
    /// Specifies a segregation target, and what percent that target should need.
    /// </summary>
    ///
    public class SegregateTargetPercent
    {
        /// <summary>
        /// Used internally to hold the target filename.
        /// </summary>
        ///
        private FileInfo filename;

        /// <summary>
        /// Percent that this target should get.
        /// </summary>
        ///
        private int percent;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="outputFile">The output filename.</param>
        /// <param name="thePercent">The target percent.</param>
        public SegregateTargetPercent(FileInfo outputFile, int thePercent)
        {
            percent = thePercent;
            filename = outputFile;
        }


        /// <value>the filename to set</value>
        public FileInfo Filename
        {
            /// <returns>the filename</returns>
            get { return filename; }
            /// <param name="theFilename">the filename to set</param>
            set { filename = value; }
        }


        /// <value>the numberRemaining to set</value>
        public int NumberRemaining { /// <returns>the numberRemaining</returns>
            get; /// <param name="theNumberRemaining">the numberRemaining to set</param>
            set; }


        /// <value>the percent to set</value>
        public int Percent
        {
            /// <returns>the percent</returns>
            get { return percent; }
            /// <param name="thePercent">the percent to set</param>
            set { percent = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" filename=");
            result.Append(filename.ToString());
            result.Append(", percent=");
            result.Append(percent);

            result.Append("]");
            return result.ToString();
        }
    }
}