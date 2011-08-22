using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.ML.Data.Market.FinanceDataSet
{
    class FinanceError : EncogError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public FinanceError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public FinanceError(Exception e)
            : base(e)
        {
        }

    }

}
