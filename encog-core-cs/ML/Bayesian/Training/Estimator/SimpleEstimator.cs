//
// Encog(tm) Core v3.3 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2014 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using Encog.ML.Bayesian.Table;
using Encog.ML.Data;

namespace Encog.ML.Bayesian.Training.Estimator
{
    /// <summary>
    /// A simple probability estimator.
    /// </summary>
    public class SimpleEstimator : IBayesEstimator
    {
        private IMLDataSet _data;
        private int _index;
        private BayesianNetwork _network;

        #region IBayesEstimator Members

        /// <inheritdoc/>
        public void Init(TrainBayesian theTrainer, BayesianNetwork theNetwork, IMLDataSet theData)
        {
            _network = theNetwork;
            _data = theData;
            _index = 0;
        }


        /// <inheritdoc/>
        public bool Iteration()
        {
            BayesianEvent e = _network.Events[_index];
            foreach (TableLine line in e.Table.Lines)
            {
                line.Probability = (CalculateProbability(e, line.Result, line.Arguments));
            }
            _index++;

            return _index < _network.Events.Count;
        }

        #endregion

        /// <summary>
        /// Calculate the probability.
        /// </summary>
        /// <param name="e">The event.</param>
        /// <param name="result">The result.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The probability.</returns>
        public double CalculateProbability(BayesianEvent e, int result, int[] args)
        {
            int eventIndex = _network.Events.IndexOf(e);
            int x = 0;
            int y = 0;

            // calculate overall probability
            foreach (IMLDataPair pair in _data)
            {
                int[] d = _network.DetermineClasses(pair.Input);

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
                        int givenIndex = _network.GetEventIndex(givenEvent);
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


            return num/den;
        }
    }
}
