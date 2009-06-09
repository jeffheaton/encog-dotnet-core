using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Encog.Parse.Units
{
    /// <summary>
    /// Used to provide unit conversion for the parser.
    /// </summary>
    public class UnitConversion
    {
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(UnitConversion));

        /// <summary>
        /// The from unit.
        /// </summary>
        private String from;

        /// <summary>
        /// The to unit.
        /// </summary>
        private String to;

        /// <summary>
        /// The number to add before the ratio.
        /// </summary>
        private double addPreRatio;

        /// <summary>
        /// The number to add after the ratio.
        /// </summary>
        private double addPostRatio;

        /// <summary>
        /// The conversion ratio.
        /// </summary>
        private double ratio;

        /// <summary>
        /// Used to specify how a unit conversion works.
        /// </summary>
        /// <param name="from">The from unit.</param>
        /// <param name="to">The to unit.</param>
        /// <param name="addPreRatio">The number to be added before the ratio.</param>
        /// <param name="addPostRatio">The number to be added after the ratio.</param>
        /// <param name="ratio">The ratio.</param>
        public UnitConversion(String from, String to,
                 double addPreRatio, double addPostRatio,
                 double ratio)
        {
            this.from = from;
            this.to = to;
            this.addPreRatio = addPreRatio;
            this.addPostRatio = addPostRatio;
            this.ratio = ratio;
        }

        /// <summary>
        /// Perform the conversion.
        /// </summary>
        /// <param name="input">The number to convert.</param>
        /// <returns>The converted value.</returns>
        public double Convert(double input)
        {
            return (((input + this.addPreRatio) * this.ratio) + this.addPostRatio);
        }

        /// <summary>
        /// The value to add before the ratio is applied.
        /// </summary>
        public double AddPostRatio
        {
            get
            {
                return this.addPostRatio;
            }
        }

        /// <summary>
        /// The value to add after the ratio is applied.
        /// </summary>
        public double AddPreRatio
        {
            get
            {
                return this.addPreRatio;
            }
        }

        /// <summary>
        /// The conversion ratio.
        /// </summary>
        public String From
        {
            get
            {
                return this.from;
            }
        }

        /// <summary>
        /// The conversion ratio.
        /// </summary>
        public double Ratio
        {
            get
            {
                return this.ratio;
            }
        }

        /// <summary>
        /// The to unit.
        /// </summary>
        public String To
        {
            get
            {
                return this.to;
            }
        }
    }

}
