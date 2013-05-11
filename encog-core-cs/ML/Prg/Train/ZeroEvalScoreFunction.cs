using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Networks.Training;
using Encog.ML.Data;
using Encog.ML.Data.Basic;

namespace Encog.ML.Prg.Train
{
    /// <summary>
    /// This is a very simple evaluation function that simply passes in all zeros for
    /// the input arguments. Make sure that the genome you are testing supports
    /// floating point numbers for inputs.
    /// 
    /// This evaluation function is useful to test to very quickly verify that a
    /// genome is valid and does not generate any obvious division by zero issues.
    /// This allows a population generator to quickly eliminate some invalid genomes.
    /// </summary>
    [Serializable]
    public class ZeroEvalScoreFunction : ICalculateScore
    {
        /// <inheritdoc/>
        public double CalculateScore(IMLMethod genome)
        {
            EncogProgram prg = (EncogProgram)genome;
            PrgPopulation pop = (PrgPopulation)prg.Population;
            IMLData inputData = new BasicMLData(pop.Context.DefinedVariables.Count);
            prg.Compute(inputData);
            return 0;
        }

        /// <inheritdoc/>
        public bool RequireSingleThreaded
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool ShouldMinimize
        {
            get
            {
                return true;
            }
        }
    }
}
