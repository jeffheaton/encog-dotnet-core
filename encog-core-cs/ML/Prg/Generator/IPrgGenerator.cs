using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Population;
using Encog.MathUtil.Randomize;
using Encog.ML.Prg.ExpValue;

namespace Encog.ML.Prg.Generator
{
    /// <summary>
    /// Generate a random Encog Program.
    /// </summary>
    public interface IPrgGenerator : IPopulationGenerator
    {
        /// <summary>
        /// Create a random node for an Encog Program.
        /// </summary>
        /// <param name="rnd">Random number generator.</param>
        /// <param name="program">The program that the node should be generated for.</param>
        /// <param name="depthRemaining">The depth remaining to generate.</param>
        /// <param name="types">The types to generate.</param>
        /// <returns>The newly created node.</returns>
        ProgramNode CreateNode(EncogRandom rnd, EncogProgram program, int depthRemaining,
                IList<EPLValueType> types);

        /// <summary>
        /// The maximum number of errors to allow during generation.
        /// </summary>
        int MaxGenerationErrors { get; set; }

    }
}
