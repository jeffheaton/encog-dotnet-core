
using System;

namespace Encog.App.Analyst.CSV.Basic
{
    /// <summary>
    /// A basic cached column. Used internally by some of the Encog CSV quant
    /// classes. All of the file contents for this column are loaded into memory.
    /// </summary>
    ///
    public class BaseCachedColumn
    {
        /// <summary>
        /// The data for this column.
        /// </summary>
        ///
        private double[] data;

        /// <summary>
        /// Construct the cached column.
        /// </summary>
        ///
        /// <param name="theName">The name of the column.</param>
        /// <param name="theInput">Is this column used for input?</param>
        /// <param name="theOutput">Is this column used for output?</param>
        public BaseCachedColumn(String theName, bool theInput,
                                bool theOutput)
        {
            Name = theName;
            Input = theInput;
            Output = theOutput;
            Ignore = false;
        }


        /// <value>The data for this column.</value>
        public double[] Data
        {
            get { return data; }
        }


        /// <summary>
        /// Set the name of this column.
        /// </summary>
        public String Name { get; set; }


        /// <summary>
        /// Set if this column is to be ignored?
        /// </summary>
        public bool Ignore { get; set; }


        /// <summary>
        /// Set if this column is used for input.
        /// </summary>
        public bool Input { get; set; }


        /// <summary>
        /// Set if this column is used for output.
        /// </summary>
        public bool Output { get; set; }

        /// <summary>
        /// Allocate enough space for this column.
        /// </summary>
        public void Allocate(int length)
        {
            data = new double[length];
        }
    }
}