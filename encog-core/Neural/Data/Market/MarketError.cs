using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.NeuralData.Market
{
    public class MarketError: EncogError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public MarketError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public MarketError(Exception e)
            : base(e)
        {
        }
    }
}
