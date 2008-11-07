using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Data
{
    public interface INeuralData
    {
        double this[int x]
        {
            get;
            set;
        }

        double[] Data
        {
            get;
            set;
        }

        /// <summary>
        /// How many elements in this data structure.
        /// </summary>
        int Count
        {
            get;
        }
    }
}
