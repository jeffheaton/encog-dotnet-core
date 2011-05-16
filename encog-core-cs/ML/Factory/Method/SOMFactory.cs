using System;
using System.Collections.Generic;
using Encog.ML.Factory.Parse;
using Encog.Neural.Pattern;

namespace Encog.ML.Factory.Method
{
    /// <summary>
    /// A factory that is used to produce self-organizing maps.
    /// </summary>
    ///
    public class SOMFactory
    {
        /// <summary>
        /// Create a SOM.
        /// </summary>
        ///
        /// <param name="architecture">The architecture string.</param>
        /// <param name="input">The input count.</param>
        /// <param name="output">The output count.</param>
        /// <returns>The newly created SOM.</returns>
        public MLMethod Create(String architecture, int input,
                               int output)
        {
            IList<String> layers = ArchitectureParse.ParseLayers(architecture);
            if (layers.Count != 2)
            {
                throw new EncogError(
                    "SOM's must have exactly two elements, separated by ->.");
            }

            ArchitectureLayer inputLayer = ArchitectureParse.ParseLayer(
                layers[0], input);
            ArchitectureLayer outputLayer = ArchitectureParse.ParseLayer(
                layers[1], output);

            int inputCount = inputLayer.Count;
            int outputCount = outputLayer.Count;

            var pattern = new SOMPattern();
            pattern.InputNeurons = inputCount;
            pattern.OutputNeurons = outputCount;
            return pattern.Generate();
        }
    }
}