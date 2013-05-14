using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Population;
using Encog.ML.Data;
using Encog.ML.EA.Species;
using Encog.ML.EA.Genome;
using Encog.Parse.Expression.Common;

namespace Encog.ML.Prg.Train
{
    /// <summary>
    /// A population that contains EncogProgram's. The primary difference between
    /// this class and BasicPopulation is that a "compute" method is provided that
    /// automatically uses the "best" genome to provide a MLRegression compute
    /// method. This population type also holds the common context that all of the
    /// EncogProgram genomes make use of.
    /// </summary>
    [Serializable]
    public class PrgPopulation : BasicPopulation, IMLRegression
    {
        /// <summary>
        /// The context.
        /// </summary>
        private EncogProgramContext context;

        /// <summary>
        /// Construct the population.
        /// </summary>
        /// <param name="theContext">The context.</param>
        /// <param name="thePopulationSize">The population size.</param>
        public PrgPopulation(EncogProgramContext theContext,
                int thePopulationSize)
            : base(thePopulationSize, new PrgGenomeFactory(theContext))
        {
            this.context = theContext;
        }

        /// <summary>
        /// Compute the output from the best Genome. Note: it is not safe to call
        /// this method while training is progressing.
        /// </summary>
        /// <param name="input">The input to the program.</param>
        /// <returns>The output.</returns>
        public IMLData Compute(IMLData input)
        {
            EncogProgram best = (EncogProgram)BestGenome;
            return best.Compute(input);
        }

        /// <summary>
        /// Dump the specified number of genomes.
        /// </summary>
        /// <param name="i">The specified number of genomes.</param>
        public void DumpMembers(int i)
        {

            RenderCommonExpression render = new RenderCommonExpression();

            int index = 0;
            foreach (ISpecies species in Species)
            {
                Console.Out.WriteLine("** Species: " + species.ToString());
                foreach (IGenome obj in species.Members)
                {
                    EncogProgram prg = (EncogProgram)obj;
                    Console.WriteLine(index + ": Score " + prg.Score + " : "
                            + render.Render(prg));
                    index++;
                    if (index > i)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// The context for the programs.
        /// </summary>
        public EncogProgramContext Context
        {
            get
            {
                return this.context;
            }
        }

        /// <inheritdoc/>
        public int InputCount
        {
            get
            {
                return Context.DefinedVariables.Count;
            }
        }

        /// <inheritdoc/>
        public int OutputCount
        {
            get
            {
                return 1;
            }
        }
    }
}
