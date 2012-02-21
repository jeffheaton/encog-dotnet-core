using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data;
using Encog.ML.Bayesian.Table;

namespace Encog.ML.Bayesian.Training.Estimator
{
    /// <summary>
    /// A simple probability estimator.
    /// </summary>
    public class SimpleEstimator : IBayesEstimator
    {
        private IMLDataSet data;
        private BayesianNetwork network;
        private TrainBayesian trainer;
        private int index;

        /// <inheritdoc/>
        public void Init(TrainBayesian theTrainer, BayesianNetwork theNetwork, IMLDataSet theData)
        {
            this.network = theNetwork;
            this.data = theData;
            this.trainer = theTrainer;
            this.index = 0;
        }


        /// <summary>
        /// Calculate the probability.
        /// </summary>
        /// <param name="e">The event.</param>
        /// <param name="result">The result.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The probability.</returns>
        public double CalculateProbability(BayesianEvent e, int result, int[] args)
        {
            int eventIndex = this.network.Events.IndexOf(e);
            int x = 0;
            int y = 0;

            // calculate overall probability
            foreach (IMLDataPair pair in this.data)
            {
                int[] d = this.network.DetermineClasses(pair.Input);

                if (args.Length == 0)
                {
                    x++;
                    if (d[eventIndex] == result)
                    {
                        y++;
                    }
                }
                else if (d[eventIndex] == result)
                {
                    x++;

                    int i = 0;
                    bool givenMatch = true;
                    foreach (BayesianEvent givenEvent in e.Parents)
                    {
                        int givenIndex = this.network.GetEventIndex(givenEvent);
                        if (args[i] != d[givenIndex])
                        {
                            givenMatch = false;
                            break;
                        }
                        i++;
                    }

                    if (givenMatch)
                    {
                        y++;
                    }
                }
            }

            double num = y + 1;
            double den = x + e.Choices.Count;


            return num / den;
        }

        /// <inheritdoc/>
        public bool Iteration()
        {
            BayesianEvent e = this.network.Events[this.index];
            foreach (TableLine line in e.Table.Lines)
            {
                line.Probability = (CalculateProbability(e, line.Result, line.Arguments));
            }
            index++;

            return index < this.network.Events.Count;
        }
    }
}
