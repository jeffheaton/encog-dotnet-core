
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
            /// <returns>The data for this column.</returns>
            get { return data; }
        }


        /// <summary>
        /// Set the name of this column.
        /// </summary>
        ///
        /// <value>The name of this column.</value>
        public String Name { /// <returns>The name of this column</returns>
            get; /// <summary>
            /// Set the name of this column.
            /// </summary>
            ///
            /// <param name="theName">The name of this column.</param>
            set; }


        /// <summary>
        /// Set if this column is to be ignored?
        /// </summary>
        ///
        /// <value>True, if this column is to be ignored.</value>
        public bool Ignore { /// <returns>Is this column ignored?</returns>
            get; /// <summary>
            /// Set if this column is to be ignored?
            /// </summary>
            ///
            /// <param name="theIgnore">True, if this column is to be ignored.</param>
            set; }


        /// <summary>
        /// Set if this column is used for input.
        /// </summary>
        ///
        /// <value>Is this column used for input.</value>
        public bool Input { /// <returns>Is this column used for input?</returns>
            get; /// <summary>
            /// Set if this column is used for input.
            /// </summary>
            ///
            /// <param name="theIgnore">Is this column used for input.</param>
            set; }


        /// <summary>
        /// Set if this column is used for output.
        /// </summary>
        ///
        /// <value>Is this column used for output.</value>
        public bool Output { /// <returns>Is this column used for output?</returns>
            get; /// <summary>
            /// Set if this column is used for output.
            /// </summary>
            ///
            /// <param name="theOutput">Is this column used for output.</param>
            set; }

        /// <summary>
        /// Allocate enough space for this column.
        /// </summary>
        ///
        /// <param name="length">The length of this column.</param>
        public void Allocate(int length)
        {
            data = new double[length];
        }
    }
}