using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Encog.App.Quant.Segregate
{
    /// <summary>
    /// Specifies a segregation target, and what percent that target should need.
    /// </summary>
    public class SegregateTargetPercent
    {
        /// <summary>
        /// Percent that this target should get.
        /// </summary>
        public int Percent { get; set; }

        /// <summary>
        /// Used internally to track the number of items remaining for this target.
        /// </summary>
        public int NumberRemaining { get; set; }

        /// <summary>
        /// Used internally to hold the target filename.
        /// </summary>
        public String Filename { get; set; }

        /// <summary>
        /// Construct the object.
        /// </summary>
        /// <param name="outputFile">The output filename.</param>
        /// <param name="percent">The target percent.</param>
        public SegregateTargetPercent(String outputFile, int percent)
        {
            this.Percent = percent;
            this.Filename = outputFile;
        }
    }
}
