using System;
using Encog.ML;
using Encog.Neural.Networks;

namespace Encog.MathUtil.Randomize
{
    /// <summary>
    /// Implementation of <i>Nguyen-Widrow</i> weight initialization. This is the
    /// default weight initialization used by Encog, as it generally provides the
    /// most trainable neural network.
    /// </summary>
    ///
    public class NguyenWidrowRandomizer : RangeRandomizer,
                                          IRandomizer
    {
        private double beta;
        private int inputCount;

        /// <summary>
        /// Construct a Nguyen-Widrow randomizer.
        /// </summary>
        ///
        /// <param name="min">The min of the range.</param>
        /// <param name="max">The max of the range.</param>
        public NguyenWidrowRandomizer(double min, double max) : base(min, max)
        {
        }

        #region IRandomizer Members

        /// <summary>
        /// The <i>Nguyen-Widrow</i> initialization algorithm is the following :
        /// <br>
        /// 1. Initialize all weight of hidden layers with (ranged) random values<br>
        /// 2. For each hidden layer<br>
        /// 2.1 calculate beta value, 0.7/// Nth(#neurons of input layer) root of
        /// #neurons of current layer <br>
        /// 2.2 for each synapse<br>
        /// 2.1.1 for each weight <br>
        /// 2.1.2 Adjust weight by dividing by norm of weight for neuron and
        /// multiplying by beta value
        /// </summary>
        ///
        /// <param name="method">The network to randomize.</param>
        public override sealed void Randomize(MLMethod method)
        {
            if (!(method is BasicNetwork))
            {
                throw new EncogError("Ngyyen Widrow only works on BasicNetwork.");
            }

            var network = (BasicNetwork) method;

            new RangeRandomizer(Min, Max).Randomize(network);

            int hiddenNeurons = 0;

            for (int i = 1; i < network.LayerCount - 1; i++)
            {
                hiddenNeurons += network.GetLayerTotalNeuronCount(i);
            }

            // can't really do much, use regular randomization
            if (hiddenNeurons < 1)
            {
                return;
            }

            inputCount = network.InputCount;
            beta = 0.7d*Math.Pow(hiddenNeurons, 1.0d/network.InputCount);

            base.Randomize(network);
        }

        #endregion

        /// <summary>
        /// Randomize one level of a neural network.
        /// </summary>
        ///
        /// <param name="network">The network to randomize</param>
        /// <param name="fromLayer">The from level to randomize.</param>
        public override void Randomize(BasicNetwork network, int fromLayer)
        {
            int fromCount = network.GetLayerTotalNeuronCount(fromLayer);
            int toCount = network.GetLayerNeuronCount(fromLayer + 1);

            for (int fromNeuron = 0; fromNeuron < fromCount; fromNeuron++)
            {
                for (int toNeuron = 0; toNeuron < toCount; toNeuron++)
                {
                    double v = network.GetWeight(fromLayer, fromNeuron, toNeuron);
                    v = (beta*v)/inputCount;
                    network.SetWeight(fromLayer, fromNeuron, toNeuron, v);
                }
            }
        }
    }
}