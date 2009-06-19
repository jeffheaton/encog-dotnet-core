using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Pattern
{
    /// <summary>
    /// GUI constants for creating the patterns.  Specifically default
    /// x and y coordinates.
    /// </summary>
    public class PatternConst
    {
        /// <summary>
        /// The starting x-coordinate.
        /// </summary>
        public const int START_X = 50;

        /// <summary>
        /// The starting y-coordinate.
        /// </summary>
        public const int START_Y = 50;

        /// <summary>
        /// How much to indent on the x-axis.
        /// </summary>
        public const int INDENT_X = 300;

        /// <summary>
        /// How much to increase y by as the network grows.
        /// </summary>
        public const int INC_Y = 150;

        /// <summary>
        /// Private constructor
        /// </summary>
        private PatternConst()
        {
        }
    }
}
