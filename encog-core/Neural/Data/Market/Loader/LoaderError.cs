using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Data.Market.Loader
{
    public class LoaderError : MarketError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public LoaderError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public LoaderError(Exception e)
            : base(e)
        {
        }
    }
}
