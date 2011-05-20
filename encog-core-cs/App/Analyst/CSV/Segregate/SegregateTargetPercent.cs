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
            get { return filename; }
            set { filename = value; }
        }


        /// <value>the numberRemaining to set</value>
        public int NumberRemaining { get; set; }


        /// <value>the percent to set</value>
        public int Percent 
        {
            get { return percent; }
            set { percent = value; }
        }


        /// <inheritdoc/>
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