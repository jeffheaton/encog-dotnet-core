using System;
using System.Collections.Generic;
using Encog.MathUtil.RBF;
using Encog.ML.Factory.Parse;
using Encog.Neural;
using Encog.Neural.RBF;
using Encog.Util;

namespace Encog.ML.Factory.Method
{
    /// <summary>
    /// A factory to create RBF networks.
    /// </summary>
    ///
    public class RBFNetworkFactory
    {
        /// <summary>
        /// The max layer count.
        /// </summary>
        ///
        public const int MAX_LAYERS = 3;

        /// <summary>
        /// Create a RBF network.
        /// </summary>
        ///
        /// <param name="architecture">THe architecture string to use.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The RBF network.</returns>
        public MLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != MAX_LAYERS)
            {
                throw new EncogError(
                    "RBF Networks must have exactly three elements, "
                    + "separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer rbfLayer = ArchitectureParse.ParseLayer(
                layers[1], -1);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[2], output);

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            RBFEnum t;

            if (rbfLayer.Name.Equals("Gaussian", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Gaussian;
            }
            else if (rbfLayer.Name.Equals("Multiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.Multiquadric;
            }
            else if (rbfLayer.Name.Equals("InverseMultiquadric", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.InverseMultiquadric;
            }
            else if (rbfLayer.Name.Equals("MexicanHat", StringComparison.InvariantCultureIgnoreCase))
            {
                t = RBFEnum.MexicanHat;
            }
            else
            {
                throw new NeuralNetworkError("Unknown RBF: " + rbfLayer.Name);
            }

            var holder = new ParamsHolder(rbfLayer.Params);

            int rbfCount = holder.GetInt("C", true, 0);

            var result = new RBFNetwork(inputCount, rbfCount,
                                        outputCount, t);

            return result;
        }
    }
}