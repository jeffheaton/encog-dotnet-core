using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data;
using Encog.Util;
using Encog.ML.Data.Basic;
using Encog.MathUtil;

namespace Encog.ML.HMM.Distributions
{
    /// <summary>
    /// A discrete distribution is a distribution with a finite set of states that it
    /// can be in.
    /// </summary>
    public class DiscreteDistribution : IStateDistribution
    {
        /// <summary>
        /// The probabilities of moving between states.
        /// </summary>
        private double[][] probabilities;


        /// <summary>
        /// Construct a discrete distribution with the specified probabilities.
        /// </summary>
        /// <param name="theProbabilities">The probabilities.</param>
        public DiscreteDistribution(double[][] theProbabilities)
        {

            if (theProbabilities.Length == 0)
            {
                throw new EncogError("Invalid empty array");
            }

            this.probabilities = new double[theProbabilities.Length][];

            for (int i = 0; i < theProbabilities.Length; i++)
            {

                if (theProbabilities[i].Length == 0)
                {
                    throw new EncogError("Invalid empty array");
                }

                this.probabilities[i] = new double[theProbabilities[i].Length];

                for (int j = 0; j < probabilities[i].Length; j++)
                {
                    if ((this.probabilities[i][j] = theProbabilities[i][j]) < 0.0)
                    {
                        throw new EncogError("Invalid probability.");
                    }
                }
            }
        }

        /// <summary>
        /// Construct a discrete distribution.
        /// </summary>
        /// <param name="cx">The count of each.</param>
        public DiscreteDistribution(int[] cx)
        {
            this.probabilities = new double[cx.Length][];
            for (int i = 0; i < cx.Length; i++)
            {
                int c = cx[i];
                this.probabilities[i] = new double[c];

                for (int j = 0; j < c; j++)
                {
                    this.probabilities[i][j] = 1.0 / c;
                }
            }
        }

        /// <summary>
        /// Fit this distribution to the specified data.
        /// </summary>
        /// <param name="co">THe data to fit to.</param>
        public void Fit(IMLDataSet co)
        {
            if (co.Count < 1)
            {
                throw new EncogError("Empty observation set");
            }

            for (int i = 0; i < this.probabilities.Length; i++)
            {

                for (int j = 0; j < this.probabilities[i].Length; j++)
                {
                    this.probabilities[i][j] = 0.0;
                }

                foreach (IMLDataPair o in co)
                {
                    this.probabilities[i][(int)o.Input[i]]++;
                }

                for (int j = 0; j < this.probabilities[i].Length; j++)
                {
                    this.probabilities[i][j] /= co.Count;
                }
            }
        }

        /// <summary>
        /// Fit this distribution to the specified data, with weights. 
        /// </summary>
        /// <param name="co">The data to fit to.</param>
        /// <param name="weights">The weights.</param>
        public void Fit(IMLDataSet co, double[] weights)
        {
            if ((co.Count < 1) || (co.Count != weights.Length))
            {
                throw new EncogError("Invalid weight size.");
            }

            for (int i = 0; i < this.probabilities.Length; i++)
            {
                EngineArray.Fill(this.probabilities[i], 0.0);

                int j = 0;
                foreach (IMLDataPair o in co)
                {
                    this.probabilities[i][(int)o.Input[i]] += weights[j++];
                }
            }
        }


        /// <summary>
        /// Generate a random sequence.
        /// </summary>
        /// <returns>The random element.</returns>
        public IMLDataPair Generate()
        {
            IMLData result = new BasicMLData(this.probabilities.Length);

            for (int i = 0; i < this.probabilities.Length; i++)
            {
                double rand = ThreadSafeRandom.NextDouble();

                result[i] = this.probabilities[i].Length - 1;
                for (int j = 0; j < (this.probabilities[i].Length - 1); j++)
                {
                    if ((rand -= this.probabilities[i][j]) < 0.0)
                    {
                        result[i] = j;
                        break;
                    }
                }
            }

            return new BasicMLDataPair(result);
        }

        /// <summary>
        /// Determine the probability of the specified data pair. 
        /// </summary>
        /// <param name="o">THe data pair.</param>
        /// <returns>The probability.</returns>
        public double Probability(IMLDataPair o)
        {

            double result = 1;

            for (int i = 0; i < this.probabilities.Length; i++)
            {
                if (o.Input[i] > (this.probabilities[i].Length - 1))
                {
                    throw new EncogError("Wrong observation value");
                }
                result *= this.probabilities[i][(int)o.Input[i]];
            }

            return result;
        }

        /// <summary>
        /// The state probabilities.
        /// </summary>
        public double[][] Probabilities
        {
            get
            {
                return this.probabilities;
            }
        }
        
        /// <summary>
        /// Clone.
        /// </summary>
        /// <returns>A clone of the distribution.</returns>
        IStateDistribution IStateDistribution.Clone()
        {
            return null;
        }
    }
}
