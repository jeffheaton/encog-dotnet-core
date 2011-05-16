using System;
using System.Collections.Generic;
using Encog.Engine.Network.Activation;
using Encog.ML.Factory.Parse;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;

namespace Encog.ML.Factory.Method
{
    /// <summary>
    /// A factor to create feedforward networks.
    /// </summary>
    ///
    public class FeedforwardFactory
    {
        /// <summary>
        /// Error.
        /// </summary>
        ///
        public const String CANT_DEFINE_ACT = "Can't define activation function before first layer.";

        /// <summary>
        /// Create a feed forward network.
        /// </summary>
        ///
        /// <param name="architecture">The architecture string to use.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The feedforward network.</returns>
        public MLMethod Create(String architecture, int input,
                               int output)
        {
            if (input <= 0)
            {
                throw new EncogError(
                    "Must have at least one input for feedforward.");
            }

            if (output <= 0)
            {
                throw new EncogError(
                    "Must have at least one output for feedforward.");
            }

            var result = new BasicNetwork();
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            IActivationFunction af = new ActivationLinear();

            int questionPhase = 0;

            foreach (String layerStr  in  layers)
            {
                int defaultCount;
                // determine default
                if (questionPhase == 0)
                {
                    defaultCount = input;
                }
                else
                {
                    defaultCount = output;
                }

                ArchitectureLayer layer = ArchitectureParse.ParseLayer(
                    layerStr, defaultCount);
                bool bias = layer.Bias;

                String part = layer.Name;
                if (part != null)
                {
                    part = part.Trim();
                }
                else
                {
                    part = "";
                }

                if ("tanh".Equals(part, StringComparison.InvariantCultureIgnoreCase))
                {
                    af = new ActivationTANH();
                }
                else if ("linear".Equals(part, StringComparison.InvariantCultureIgnoreCase))
                {
                    af = new ActivationLinear();
                }
                else if ("sigmoid".Equals(part, StringComparison.InvariantCultureIgnoreCase))
                {
                    af = new ActivationSigmoid();
                }
                else
                {
                    if (layer.UsedDefault)
                    {
                        questionPhase++;
                        if (questionPhase > 2)
                        {
                            throw new EncogError("Only two ?'s may be used.");
                        }
                    }

                    if (layer.Count == 0)
                    {
                        throw new EncogError("Unknown architecture element: "
                                             + architecture + ", can't parse: " + part);
                    }

                    result.AddLayer(new BasicLayer(af, bias, layer.Count));
                }
            }

            result.Structure.FinalizeStructure();
            result.Reset();

            return result;
        }
    }
}