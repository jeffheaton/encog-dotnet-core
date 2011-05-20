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
        private FileInfo _filename;

        /// <summary>
        /// Percent that this target should get.
        /// </summary>
        ///
        private int _percent;

        /// <summary>
        /// Construct the object.
        /// </summary>
        ///
        /// <param name="outputFile">The output filename.</param>
        /// <param name="thePercent">The target percent.</param>
        public SegregateTargetPercent(FileInfo outputFile, int thePercent)
        {
            _percent = thePercent;
            _filename = outputFile;
        }


        /// <value>the filename to set</value>
        public FileInfo Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }


        /// <value>the numberRemaining to set</value>
        public int NumberRemaining { get; set; }


        /// <value>the percent to set</value>
        public int Percent 
        {
            get { return _percent; }
            set { _percent = value; }
        }


        /// <inheritdoc/>
        public override sealed String ToString()
        {
            var result = new StringBuilder("[");
            result.Append(GetType().Name);
            result.Append(" filename=");
            result.Append(_filename.ToString());
            result.Append(", percent=");
            result.Append(_percent);

            result.Append("]");
            return result.ToString();
        }
    }
}