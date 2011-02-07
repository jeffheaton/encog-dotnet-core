using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.App.Quant.Basic
{
    /// <summary>
    /// A basic cached column.  Used internally by some of the Encog CSV quant classes.
    /// All of the file contents for this column are loaded into memory.
    /// </summary>
    public class BaseCachedColumn
    {
        /// <summary>
        /// The data for this column.
        /// </summary>
        private double[] data;

        /// <summary>
        /// The name of this column.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The data for this column.
        /// </summary>
        public double[] Data { get { return data; } }

        /// <summary>
        /// Construct the cached column.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="input">Is this column used for input?</param>
        /// <param name="output">Is this column used for output?</param>
        public BaseCachedColumn(String name, bool input, bool output)
        {
            this.Name = name;
            this.Input = input;
            this.Output = output;
            this.Ignore = false;
        }

        /// <summary>
        /// Is this column used for output?
        /// </summary>
        public bool Output { get; set; }

        /// <summary>
        /// Is this column used for input?
        /// </summary>
        public bool Input { get; set; }

        /// <summary>
        /// Should this column be ignored.
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// Allocate enough space for this column.
        /// </summary>
        /// <param name="length">The length of this column.</param>
        public void Allocate(int length)
        {
            this.data = new double[length];
        }
    }
}
