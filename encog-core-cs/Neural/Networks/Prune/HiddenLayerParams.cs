using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Networks.Prune
{
    /// <summary>
    /// Specifies the minimum and maximum neuron counts for a layer.
    /// </summary>
    public class HiddenLayerParams
    {
        /// <summary>
        /// The minimum number of neurons on this layer.
        /// </summary>
        private int min;

        /// <summary>
        /// The maximum number of neurons on this layer.
        /// </summary>
        private int max;


        /// <summary>
        /// Construct a hidden layer param object with the specified min and max
        /// values.
        /// </summary>
        /// <param name="min">The minimum number of neurons.</param>
        /// <param name="max">The maximum number of neurons.</param>
        public HiddenLayerParams(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// The maximum number of neurons.
        /// </summary>
        public int Max
        {
            get
            {
                return this.max;
            }
        }

        /// <summary>
        /// The minimum number of neurons.
        /// </summary>
        public int Min
        {
            get
            {
                return this.min;
            }
        }

    }

}
