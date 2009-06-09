using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Data;
using log4net;

namespace Encog.Neural.Networks
{
    /// <summary>
    /// Holds the output from each layer of the neural network. This is very useful
 /// for the propagation algorithms that need to examine the output of each
 /// individual layer.
    /// </summary>
    public class NeuralOutputHolder
    {

        /// <summary>
        /// The results from each of the synapses.
        /// </summary>
        private IDictionary<ISynapse, INeuralData> result;

        /// <summary>
        /// The output from the entire neural network.
        /// </summary>
        private INeuralData output;

        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(NeuralOutputHolder));

        /// <summary>
        /// Construct an empty holder.
        /// </summary>
        public NeuralOutputHolder()
        {
            this.result = new Dictionary<ISynapse, INeuralData>();
        }

        /// <summary>
        /// The output from the neural network.
        /// </summary>
        public INeuralData Output
        {
            get
            {
                return this.output;
            }
            set
            {
                this.output = value;
            }
        }

        /// <summary>
        /// The result from the synapses in a map.
        /// </summary>
        public IDictionary<ISynapse, INeuralData> Result
        {
            get
            {
                return this.result;
            }
        }
    }
}
