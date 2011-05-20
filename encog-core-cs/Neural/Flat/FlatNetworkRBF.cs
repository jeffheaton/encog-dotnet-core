using System;
using Encog.Engine.Network.Activation;
using Encog.MathUtil.RBF;
using Encog.Util;

namespace Encog.Neural.Flat
{
    /// <summary>
    /// A flat network designed to handle an RBF.
    /// </summary>
    ///
    [Serializable]
    public class FlatNetworkRBF : FlatNetwork
    {
        /// <summary>
        /// The RBF's used.
        /// </summary>
        ///
        private IRadialBasisFunction[] rbf;

        /// <summary>
        /// Default constructor.
        /// </summary>
        ///
        public FlatNetworkRBF()
        {
        }

        /// <summary>
        /// Construct an RBF flat network.
        /// </summary>
        ///
        /// <param name="inputCount">The number of input neurons. (also the number of dimensions)</param>
        /// <param name="hiddenCount">The number of hidden neurons.</param>
        /// <param name="outputCount">The number of output neurons.</param>
        /// <param name="rbf_0"></param>
        public FlatNetworkRBF(int inputCount, int hiddenCount,
                              int outputCount, IRadialBasisFunction[] rbf_0)
        {
            var layers = new FlatLayer[3];
            rbf = rbf_0;

            layers[0] = new FlatLayer(new ActivationLinear(), inputCount, 0.0d);
            layers[1] = new FlatLayer(new ActivationLinear(), hiddenCount, 0.0d);
            layers[2] = new FlatLayer(new ActivationLinear(), outputCount, 0.0d);

            Init(layers);
        }

        /// <summary>
        /// Set the RBF's used.
        /// </summary>
        public IRadialBasisFunction[] RBF
        {
            get { return rbf; }
            set { rbf = value; }
        }

        /// <summary>
        /// Clone the network.
        /// </summary>
        ///
        /// <returns>A clone of the network.</returns>
        public override sealed Object Clone()
        {
            var result = new FlatNetworkRBF();
            CloneFlatNetwork(result);
            result.rbf = rbf;
            return result;
        }

        /// <summary>
        /// Calculate the output for the given input.
        /// </summary>
        ///
        /// <param name="x">The input.</param>
        /// <param name="output">Output will be placed here.</param>
        public override sealed void Compute(double[] x, double[] output)
        {
            int outputIndex = LayerIndex[1];

            for (int i = 0; i < rbf.Length; i++)
            {
                double o = rbf[i].Calculate(x);
                LayerOutput[outputIndex + i] = o;
            }

            // now compute the output
            ComputeLayer(1);
            EngineArray.ArrayCopy(LayerOutput, 0, output, 0,
                                  OutputCount);
        }
    }
}