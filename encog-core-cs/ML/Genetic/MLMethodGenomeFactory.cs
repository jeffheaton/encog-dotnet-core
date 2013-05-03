using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.EA.Genome;
using Encog.ML.EA.Population;

namespace Encog.ML.Genetic
{
    /// <summary>
    /// A factory to create MLMethod based genomes.
    /// </summary>
    public class MLMethodGenomeFactory : IGenomeFactory
    {
        public delegate IMLMethod CreateMethod();

        /// <summary>
        /// The MLMethod factory.
        /// </summary>
        private CreateMethod factory;

        /// <summary>
        /// The population.
        /// </summary>
        private IPopulation population;

        /// <summary>
        /// Construct the genome factory.
        /// </summary>
        /// <param name="theFactory">The factory.</param>
        /// <param name="thePopulation">The population.</param>
        public MLMethodGenomeFactory(CreateMethod theFactory,
                IPopulation thePopulation)
        {
            this.factory = theFactory;
            this.population = thePopulation;
        }

        /// <inheritdoc/>
        public IGenome Factor()
        {
            IGenome result = new MLMethodGenome(
                    (IMLEncodable)this.factory());
            result.Population = this.population;
            return result;
        }

        /// <inheritdoc/>
        public IGenome Factor(IGenome other)
        {
            MLMethodGenome result = (MLMethodGenome)Factor();
            result.Copy(other);
            result.Population = this.population;
            return result;
        }
    }
}
